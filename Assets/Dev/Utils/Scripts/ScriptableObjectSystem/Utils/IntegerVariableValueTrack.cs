using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntegerVariableValueTrack : MonoBehaviour
{
    [SerializeField]
    private IntegerVariable _trackedInt;
    [SerializeField]
    private int _compareWith;
    [SerializeField]
    private ValueTrackType _trackType;
    [SerializeField]
    private UnityEvent _onValueMeetCondition;

    private void OnEnable()
    {
        _trackedInt.OnValueChange += ValueChangeAction;
        ValueChangeAction(_trackedInt.Value);
    }

    private void OnDisable()
    {
        _trackedInt.OnValueChange -= ValueChangeAction;
    }

    private void ValueChangeAction(int value)
    {
        if (!ValidateCondition())
        {
            return;
        }

        _onValueMeetCondition.Invoke();

    }

    private bool ValidateCondition()
    {
        switch (_trackType)
        {
            case ValueTrackType.Equal:
                return _trackedInt.Value == _compareWith;
            case ValueTrackType.NotEqual:
                return _trackedInt.Value != _compareWith;
            case ValueTrackType.GreaterThan:
                return _trackedInt.Value > _compareWith;
            case ValueTrackType.GreaterThanOrEqual:
                return _trackedInt.Value >= _compareWith;
            case ValueTrackType.LessThan:
                return _trackedInt.Value < _compareWith;
            case ValueTrackType.LessThanOrEqual:
                return _trackedInt.Value <= _compareWith;
            default:
                return false;
        }
    }
}

public enum ValueTrackType
{
    Equal = 0,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    NotEqual
}
