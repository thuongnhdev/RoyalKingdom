using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionHRChangePopup : MonoBehaviour
{
    private Action _saveAction;
    private Action _discardAction;

    public void SetSaveAction(Action save)
    {
        _saveAction = save;
    }

    public void SetDiscardAction(Action discard)
    {
        _discardAction = discard;
    }

    public void ExecuteSave()
    {
        _saveAction?.Invoke();
    }

    public void ExecuteDiscard()
    {
        _discardAction?.Invoke();
    }
}
