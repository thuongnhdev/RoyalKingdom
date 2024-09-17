using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestErrorPopup : MonoBehaviour
{
    [SerializeField]
    private GameEvent _onUserChoseActionOnFailedRequest;

    public void Retry()
    {
        bool retry = true;
        _onUserChoseActionOnFailedRequest.Raise(retry);
    }

    public void Reboot()
    {
        bool retry = false;
        _onUserChoseActionOnFailedRequest.Raise(retry);
    }
}
