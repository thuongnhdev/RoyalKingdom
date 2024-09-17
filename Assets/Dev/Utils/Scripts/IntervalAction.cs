using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class IntervalAction : MonoBehaviour
{
    [SerializeField]
    private bool _startActionOnEnable = false;
    [SerializeField]
    private bool _doActionBeforeFirstInterval = false;
    [SerializeField][Tooltip("time unit in second")]
    private float _interval = 1f;
    [SerializeField]
    private UnityEvent _action;

    private bool _isOnInterval = false;
    private CancellationTokenSource _intervalActionToken;

    public void StartIntervalAction()
    {
        UniTask_StartIntervalAction().Forget();
    }

    public void StopIntervalAction()
    {
        _intervalActionToken?.Cancel();
        _isOnInterval = false;
    }


    private async UniTaskVoid UniTask_StartIntervalAction()
    {
        _intervalActionToken?.Cancel();
        _intervalActionToken = new CancellationTokenSource();

        _isOnInterval = true;

        if (_doActionBeforeFirstInterval)
        {
            _action.Invoke();
        }

        while (_isOnInterval)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_interval), cancellationToken: _intervalActionToken.Token);
            _action.Invoke();
        }
    }

    private void OnEnable()
    {
        if (_startActionOnEnable)
        {
            StartIntervalAction();
        }
    }

    private void OnDisable()
    {
        StopIntervalAction();
    }
}
