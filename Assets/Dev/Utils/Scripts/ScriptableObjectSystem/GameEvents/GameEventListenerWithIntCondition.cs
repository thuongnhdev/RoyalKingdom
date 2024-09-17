using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerWithIntCondition : MonoBehaviour
{
    [SerializeField]
    private List<GameEvent> _gameEvents;
    [SerializeField]
    private List<EventCondition> _conditions;
    [SerializeField]
    private UnityEvent _handler;

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
                return;
            }
        }

        _handler.Invoke();
    }
}

[System.Serializable]
public class EventCondition
{
    [SerializeField]
    EventConditionType _conditionType;
    [SerializeField]
    private IntegerVariable _refValue;
    [SerializeField]
    private int _compareWith;

    public bool CheckCondition()
    {
        switch (_conditionType)
        {
            case EventConditionType.Equal:
                return _refValue.Value == _compareWith;
            case EventConditionType.NotEqual:
                return _refValue.Value != _compareWith;
            case EventConditionType.GreaterThan:
                return _refValue.Value > _compareWith;
            case EventConditionType.GreaterThanOrEqual:
                return _refValue.Value >= _compareWith;
            case EventConditionType.LessThan:
                return _refValue.Value < _compareWith;
            case EventConditionType.LessThanOrEqual:
                return _refValue.Value <= _compareWith;
            default:
                return false;
        }
    }
}

public enum EventConditionType
{
    Equal = 0,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    NotEqual
}
