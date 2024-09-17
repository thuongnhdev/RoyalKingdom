using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class SequentialAction : MonoBehaviour
{
    [SerializeField]
    private bool _startOnEnable = false;

    [SerializeField]
    private List<ActionInSequence> _actions;

    public void StartActions()
    {
        UniTask_StartActions().Forget();
    }

    private async UniTaskVoid UniTask_StartActions()
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            var action = _actions[i];
            await UniTask.Delay(System.TimeSpan.FromSeconds(action.startAfter));
            action.action.Invoke();
        }
    }

    private void OnEnable()
    {
        if (_startOnEnable)
        {
            UniTask_StartActions().Forget();
        }
    }
}

[System.Serializable]
public class ActionInSequence
{
    public float startAfter;
    public UnityEvent action;
}
