using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Cysharp.Threading.Tasks;

public class RequestSenderAsync
{
    public static async UniTask<WebRequestResponse> SendGetRequest(string url, string token = null, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAsync(url, RequestType.GET, null, token, customHeader);
    }

    public static async UniTask<WebRequestResponse> SendPostRequest(string url, byte[] data = null, string token = null, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAsync(url, RequestType.POST, data, token, customHeader);
    }

    public static async UniTask<WebRequestResponse> SendPutRequest(string url, byte[] data = null, string token = null, params Tuple<string, string>[] customHeader)
    {
        return await SendRequestAsync(url, RequestType.PUT, data, token, customHeader);
    }

    private static async UniTask<WebRequestResponse> SendRequestAsync(string url, RequestType requestType, byte[] data, string token, Tuple<string, string>[] customHeader)
    {
        using UnityWebRequest request = RequestHelper.GetUnityWebRequest(url, requestType, data, token, customHeader);
        try
        {
            await request.SendWebRequest();
        }
        catch (UnityWebRequestException ex)
        {
            var failedRequest = ex.UnityWebRequest;
            if (failedRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Request [{url}] got error, error code [{request.responseCode}]");
            }

            if (failedRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"Request [{url}] got error, cannot connect to server");
            }

            return new() { responseCode = failedRequest.responseCode, rawdata = null };
        }

        return new WebRequestResponse
        {
            responseCode = request.responseCode,
            rawdata = request.downloadHandler.data,
        };
    }
}

public class WebRequestResponse
{
    public long responseCode;
    public byte[] rawdata;
}