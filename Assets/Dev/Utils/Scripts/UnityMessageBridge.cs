using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityMessageBridge : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onEnable;
    [SerializeField]
    private UnityEvent _1FrameAfterOnEnable;
    [SerializeField]
    private UnityEvent _onDisable;
    [SerializeField]
    private UnityEvent _1FrameAfterOnDisable;

    private void OnEnable()
    {
        _onEnable.Invoke();
        OnNextFrameAfterEnable().Forget();
    }

    private async UniTaskVoid OnNextFrameAfterEnable()
    {
        await UniTask.NextFrame();
        _1FrameAfterOnEnable.Invoke();
    }

    private void OnDisable()
    {
        _onDisable.Invoke();
        OnNextFrameAfterDisable().Forget();
    }

    private async UniTaskVoid OnNextFrameAfterDisable()
    {
        await UniTask.NextFrame();
        _1FrameAfterOnDisable.Invoke();
    }
}
