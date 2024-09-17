using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class FeededAction : MonoBehaviour
{
    [SerializeField]
    private bool _startOnEnable = false;
    [SerializeField]
    private float _aliveTimeAfterFeed = 0.2f;

    [SerializeField]
    private UnityEvent _initAction;
    [SerializeField]
    private UnityEvent _onFeededAction;
    [SerializeField]
    private UnityEvent _tempDisableAction;

    [Header("Inspec")]
    [SerializeField]
    private float _tempDisableAfter;

    private bool _tempDisable;
    private bool _initFeed = true;

    private CancellationTokenSource updateToken;

    public void Feed()
    {
        if (_initFeed)
        {
            InitFeed();
            _initFeed = false;
        }

        _tempDisableAfter = _aliveTimeAfterFeed;
        _tempDisable = false;

        _onFeededAction.Invoke();
    }

    public void PermanentDisableAction()
    {
        updateToken?.Cancel();
        _initFeed = true;
    }

    private void InitFeed()
    {
        TrackLifeTime().Forget();
        _initAction.Invoke();
    }

    private async UniTaskVoid TrackLifeTime()
    {
        updateToken?.Cancel();
        updateToken = new();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(updateToken.Token))
        {
            if (_tempDisableAfter <= 0f && !_tempDisable)
            {
                _tempDisable = true;
                _tempDisableAction.Invoke();
            }

            _tempDisableAfter -= Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        _initFeed = true;
        if (_startOnEnable)
        {
            Feed();
        }
    }

    private void OnDisable()
    {
        PermanentDisableAction();
    }
}