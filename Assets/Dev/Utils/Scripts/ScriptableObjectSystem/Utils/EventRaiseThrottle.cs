using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

public class EventRaiseThrottle : MonoBehaviour
{
    [SerializeField]
    private GameEvent _eventIn;
    [SerializeField]
    private List<GameEvent> _eventsOut;
    [SerializeField]
    private float _throttle;
    [Header("Inspec")]
    [SerializeField]
    private float _timeUntilNextEvent;

    private CancellationTokenSource _updateToken;

    public void SetThrottle(float throttle)
    {
        _throttle = throttle;
    }

    private void OnEnable()
    {
        _eventIn.Subcribe(RaiseEvent);
    }

    private void OnDisable()
    {
        _eventIn.Unsubcribe(RaiseEvent);
    }

    private void RaiseEvent(params object[] args)
    {
        UniTask_RaiseEvent(args).Forget();
    }

    private async UniTaskVoid UniTask_RaiseEvent(params object[] args)
    {
        if (_timeUntilNextEvent > 0f)
        {
            return;
        }

        _updateToken.Cancel();
        _updateToken = new CancellationTokenSource();

        _timeUntilNextEvent = _throttle;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            if (_timeUntilNextEvent <= 0f)
            {
                _updateToken.Cancel();
                break;
            }

            _timeUntilNextEvent -= Time.deltaTime;
        }

        for (int i = 0; i < _eventsOut.Count; i++)
        {
            _eventsOut[i].Raise();
        }
    }
}
