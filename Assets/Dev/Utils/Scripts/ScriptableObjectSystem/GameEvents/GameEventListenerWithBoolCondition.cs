using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerWithBoolCondition : MonoBehaviour
{
    [SerializeField]
    private List<GameEvent> _gameEvents;
    [SerializeField]
    private List<BoolEventCondition> _conditions;
    [SerializeField]
    private UnityEvent _handler;
    [SerializeField]
    private UnityEvent _failedHandler;

    private void OnEnable()
    {
        if (_gameEvents == null)
        {
            return;
        }

        for (int i = 0; i < _gameEvents.Count; i++)
        {
#if UNITY_EDITOR
            _gameEvents[i].SubcribeEditor(HandleEvent, gameObject.name);
#else
            _gameEvents[i].Subcribe(HandleEvent);
#endif

        }
    }

    private void OnDisable()
    {
        if (_gameEvents == null)
        {
            return;
        }
        for (int i = 0; i < _gameEvents.Count; i++)
        {
#if UNITY_EDITOR
            _gameEvents[i].UnsubcribeEditor(HandleEvent, gameObject.name);
#else
            _gameEvents[i].Unsubcribe(HandleEvent);
#endif
        }
    }

    private void HandleEvent(params object[] args)
    {
        if (_gameEvents == null)
        {
            return;
        }

        for (int i = 0; i < _conditions.Count; i++)
        {
            if (!_conditions[i].CheckCondition())
            {
                _failedHandler.Invoke();
                return;
            }
        }

        _handler.Invoke();
    }
}

[System.Serializable]
public class BoolEventCondition
{
    [SerializeField]
    private BoolVariable _refValue;
    [SerializeField]
    private bool _compareWith;

    public bool CheckCondition()
    {
        return _refValue.Value == _compareWith;
    }
}
