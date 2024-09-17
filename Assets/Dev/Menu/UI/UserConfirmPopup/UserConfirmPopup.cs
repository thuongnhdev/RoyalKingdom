using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserConfirmPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private StringVariable _userConfirmPopupContent;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onOpenUserConfirmPopup;

    [Header("Config")]
    [SerializeField]
    private TMP_Text _confirmText;

    [Header("Event out - Inspec")]
    [SerializeField]
    private GameEvent _yesButtonEvent;

    public delegate void YesAction(params object[] args);
    private YesAction _yesAction;

    private object[] _yesEventParams;

    public void In_ExecuteYesCommand()
    {
        _yesButtonEvent?.Raise(_yesEventParams);
        _yesButtonEvent = null;

        _yesAction?.Invoke();
        _yesAction = null;
    }

    public void SetYesAction(YesAction action)
    {
        _yesAction = action;
    }
    
    public void SetConfirmText(string confirmText)
    {
        _confirmText.text = confirmText;
    }

    private void SetUpContent(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        _yesButtonEvent = (GameEvent)args[0];

        if (args.Length == 1)
        {
            return;
        }

        _yesEventParams = (object[])args[1];
    }

    private void OnEnable()
    {
        _onOpenUserConfirmPopup.Subcribe(SetUpContent);
    }

    private void OnDisable()
    {
        _onOpenUserConfirmPopup.Unsubcribe(SetUpContent);
    }
}
