using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class FloatLinearChange : MonoBehaviour
{
    [SerializeField]
    private bool _autoChangeOnEnable;
    [SerializeField]
    private FloatVariable _refValueSO;
    [SerializeField]
    private float _refValue;
    [SerializeField]
    private FloatVariable _changedValue;
    [SerializeField][Tooltip("Unit per sec")]
    private float _changeSpeed = 1f;
    [SerializeField]
    private ChangeType _changeType;

    [SerializeField]
    private UnityEvent _onFinishChange;

    private CancellationTokenSource _updateToken;

    public enum ChangeType
    {
        Increase,
        Decrease
    }

    public void AutoChangeBySpeed()
    {
        if (_changeType == ChangeType.Increase)
        {
            AutoIncreaseBySpeed().Forget();
            return;
        }

        AutoDecreaseBySpeed().Forget();
    }

    private async UniTaskVoid AutoDecreaseBySpeed()
    {
        _changedValue.Value = _refValueSO == null ? _refValue : _refValueSO.Value;

        _updateToken.Cancel();
        _updateToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            if (_changedValue.Value <= 0f)
            {
                _changedValue.Value = 0f;
                _onFinishChange.Invoke();

                _updateToken.Cancel();
                break;
            }
            _changedValue.Value -= _changeSpeed * Time.deltaTime;
        }
    }

    private async UniTaskVoid AutoIncreaseBySpeed()
    {
        float targetValue = _refValueSO == null ? _refValue : _refValueSO.Value;
        _changedValue.Value = 0f;

        _updateToken.Cancel();
        _updateToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            if (_changedValue.Value >= targetValue)
            {
                _changedValue.Value = targetValue;
                _onFinishChange.Invoke();

                _updateToken.Cancel();
                break;
            }

            _changedValue.Value += _changeSpeed * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        if (!_autoChangeOnEnable)
        {
            return;
        }
        AutoChangeBySpeed();
    }
}
