using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class InputListener : MonoBehaviour
{
    [SerializeField]
    private bool _listenOnEnable = true;
    [SerializeField]
    private KeyCode[] listenedKeys;
    [SerializeField]
    private UnityEvent _handler;
    
    private CancellationTokenSource _updateToken;

    public async UniTaskVoid StartListenInput()
    {
        _updateToken?.Cancel();
        _updateToken = new();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            for (int i = 0; i < listenedKeys.Length; i++)
            {
                if (Input.GetKeyDown(listenedKeys[i]))
                {
                    _handler.Invoke();
                    break;
                }
            }
        }
    }
    private void OnEnable()
    {
        StartListenInput().Forget();
    }

    private void OnDisable()
    {
        _updateToken?.Cancel();
    }
}
