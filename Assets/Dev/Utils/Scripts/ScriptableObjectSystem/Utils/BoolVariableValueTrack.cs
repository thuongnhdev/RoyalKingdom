using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolVariableValueTrack : MonoBehaviour
{
    [SerializeField]
    private BoolVariable _trackedBool;
    [SerializeField]
    private UnityEvent _actionWhenTrue;
    [SerializeField]
    private UnityEvent _actionWhenFalse;

    private void OnEnable()
    {
        _trackedBool.OnValueChange += ValueChangeAction;
        ValueChangeAction(_trackedBool.Value);
    }

    private void OnDisable()
    {
        _trackedBool.OnValueChange -= ValueChangeAction;
    }

    private void ValueChangeAction(bool value)
    {
        if (_trackedBool.Value)
        {
            _actionWhenTrue.Invoke();
            return;
        }

        _actionWhenFalse.Invoke();
    }
}
