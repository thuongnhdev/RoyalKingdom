using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Google.FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class RequestDispatcher : MonoSingleton<RequestDispatcher>
{
    private const long EXPIRED_TOKEN = 403;

    [Header("Reference - Read")]
    [SerializeField]
    private StringVariable[] _tokens;
    [SerializeField]
    private IntegerVariable _tokenIndex;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onUserChoseActionOnFailedRequest;

    [SerializeField]
    private UnityEvent _onStartFirstBlockUiRequest;
    [SerializeField]
    private UnityEvent _onEndLastBlockUIRequest;
    [SerializeField]
    private UnityEvent _onRequestError;

    [Header("Inspec")]
    [SerializeField]
    private int _sendingBlockUiRequestCount = 0;
    [SerializeField]
    private bool _acquiringNewToken = false;

    private bool _isNextActionOnFailedRequestConfirmed = false;
    private bool _retry = false;
    private Dictionary<string, RequestData> _registeredRequests = new();

    public void SetThrottleForRequest(string url, float throttle)
    {
        _registeredRequests.TryGetValue(url, out var request);
        if (request == null)
        {
            request = new()
            {
                url = url,
                throttle = throttle
            };

            _registeredRequests[url] = request;
        }
    }

    public async UniTask<T> SendGetRequest<T>(string url, bool blockUi = false, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAndConvertResponse<T>(url, RequestType.GET, null, customHeader, blockUi);
    }

    public async UniTask<T> SendPostRequest<T>(string url, byte[] body, bool blockUi = false, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAndConvertResponse<T>(url, RequestType.POST, body, customHeader, blockUi);
    }

    public async UniTask<T> SendPutRequest<T>(string url, byte[] body, bool blockUi = false, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAndConvertResponse<T>(url, RequestType.PUT, body, customHeader, blockUi);
    }

    private async UniTask<T> SendRequestAndConvertResponse<T>(string url, RequestType requestType, byte[] body, Tuple<string, string>[] customHeader, bool blockUi)
    {
        bool sendNow = CheckSendRequestNow(url);
        if (!sendNow)
        {
            _registeredRequests[url].StoreDataForSendingLater(requestType, customHeader, body);
            PollingToSendPendingRequest().Forget();
            Logger.Log("Request is throttled down, it will be sent later", Color.green);
            return default;
        }

        WebRequestResponse response = await SendRequest(url, requestType, blockUi, body, customHeader);

        IResponseRawDataConverter<T> converter = GetResponseConverter<T>(typeof(T));
        return converter.Convert(response.rawdata);
    }

    private async UniTask<WebRequestResponse> SendRequest(string url, RequestType requestType, bool blockUi, byte[] body, Tuple<string, string>[] customHeader)
    {
        WebRequestResponse response;
    RETRY:
        if (blockUi)
        {
            IncreaseBlockUiRequestCount();
        }

        if (_acquiringNewToken)
        {
            await UniTask.WaitUntilValueChanged(this, _acquiringNewToken => !_acquiringNewToken);
        }

        RecordSendRequestTime(url);

        switch (requestType)
        {
            case RequestType.POST:
                {
                    response = await RequestSenderAsync.SendPostRequest(url, body, _tokens[_tokenIndex.Value].Value, customHeader);
                    break;
                }
            case RequestType.PUT:
                {
                    response = await RequestSenderAsync.SendPutRequest(url, body, _tokens[_tokenIndex.Value].Value, customHeader);
                    break;
                }
            default:
                {
                    response = await RequestSenderAsync.SendGetRequest(url, _tokens[_tokenIndex.Value].Value, customHeader);
                    break;
                }
        }

        if (blockUi)
        {
            DecreaseBlockRequestCount();
        }

        if (response.responseCode == EXPIRED_TOKEN)
        {
            await AcquireNewToken();
            goto RETRY;
        }

        if (response.responseCode == 0 || (400 <= response.responseCode && response.responseCode <= 599))
        {
            _onRequestError.Invoke();
            await WaitForUserAction();
            if (_retry)
            {
                goto RETRY;
            }
        }

        return response;
    }

    private IResponseRawDataConverter<T> GetResponseConverter<T>(Type responseType)
    {
        if (typeof(IFlatbufferObject).IsAssignableFrom(responseType))        
        {
            return new FlatBufferResponseConverter<T>();
        }

        return new JsonResponseConverter<T>();
    }

    private bool CheckSendRequestNow(string url)
    {
        _registeredRequests.TryGetValue(url, out var requestSetting);
        if (requestSetting == null)
        {
            return true;
        }

        if (requestSetting.timeNextRequestSent < Time.realtimeSinceStartup)
        {
            return true;
        }

        return false;
    }

    private void RecordSendRequestTime(string url)
    {
        Logger.Log($"request [{url}] sent!");
        _registeredRequests.TryGetValue(url, out var request);
        if (request == null)
        {
            return;
        }

        request.timeNextRequestSent = Time.realtimeSinceStartup + request.throttle;
    }

    private async UniTask AcquireNewToken()
    { 
        if (_acquiringNewToken)
        {
            return;
        }

        _acquiringNewToken = true;
        await UniTask.DelayFrame(3); // Fake sending renew token request
        _tokens[_tokenIndex.Value].Value = "newToken";
        _acquiringNewToken = false;
    }

    private CancellationTokenSource _pollingToken;
    private async UniTaskVoid PollingToSendPendingRequest()
    {
        _pollingToken?.Cancel();
        _pollingToken = new();

        int sentRequestCount = 0;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_pollingToken.Token))
        {
            int pendingRequest = _registeredRequests.Count;
            if (pendingRequest == 0 || sentRequestCount == pendingRequest)
            {
                _registeredRequests.Clear();
                _pollingToken.Cancel();
                break;
            }

            foreach (var requestPair in _registeredRequests)
            {
                var request = requestPair.Value;
                if (Time.realtimeSinceStartup < request.timeNextRequestSent)
                {
                    continue;
                }

                SendRequest(request.url, request.requestType, false, request.body, request.header).Forget();
                request.isPending = false;
                sentRequestCount++;
            }
        }
    }

    private void IncreaseBlockUiRequestCount()
    {
        _sendingBlockUiRequestCount++;
        if (_sendingBlockUiRequestCount != 1)
        {
            return;
        }

        _onStartFirstBlockUiRequest.Invoke();
    }

    private void DecreaseBlockRequestCount()
    {
        _sendingBlockUiRequestCount--;
        if (0 < _sendingBlockUiRequestCount)
        {
            return;
        }

        _sendingBlockUiRequestCount = 0;
        _onEndLastBlockUIRequest.Invoke();
    }

    private async UniTask WaitForUserAction()
    {
        await UniTask.WaitUntil(() => _isNextActionOnFailedRequestConfirmed == true);
        await UniTask.NextFrame();

        _isNextActionOnFailedRequestConfirmed = false;
    }

    private void RetryFailedRequest(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        _retry = (bool)args[0];
        _isNextActionOnFailedRequestConfirmed = true;
    }

    protected override void DoOnEnable()
    {
        base.DoOnEnable();
        _onUserChoseActionOnFailedRequest.Subcribe(RetryFailedRequest);
    }

    protected override void DoOnDisable()
    {
        base.DoOnDisable();
        _onUserChoseActionOnFailedRequest.Unsubcribe(RetryFailedRequest);
    }


    private class RequestData
    {
        public string url;
        public float throttle;
        public RequestType requestType;
        public Tuple<string, string>[] header;
        public byte[] body;
        public float timeNextRequestSent;
        public bool isPending;

        public RequestData()
        {
        }

        public RequestData(string url, RequestType requestType, Tuple<string, string>[] header, byte[] body)
        {
            this.url = url;
            this.requestType = requestType;
            this.header = header;
            this.body = body;
        }

        public void StoreDataForSendingLater(RequestType requestType, Tuple<string, string>[] header, byte[] body)
        {
            this.requestType = requestType;
            this.header = header;
            this.body = body;
            isPending = true;
        }
    }
}
