using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField]
    private GameEvent _gameEvent;
    [SerializeField]
    private UnityEvent _handler;

    private void OnEnable()
    {
        if (_gameEvent == null)
        {
            return;
        }

#if UNITY_EDITOR
        _gameEvent.SubcribeEditor(HandleEvent, gameObject.name);
        return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
        _gameEvent.Subcribe(HandleEvent);
#pragma warning restore CS0162 // Unreachable code detected
    }

    private void OnDisable()
    {
        if (_gameEvent == null)
        {
            return;
        }

#if UNITY_EDITOR
        _gameEvent.UnsubcribeEditor(HandleEvent, gameObject.name);
        return;
#endif
        _gameEvent.Unsubcribe(HandleEvent);
    }

    private void HandleEvent(params object[] args)
    {
        if (_gameEvent == null)
        {
            return;
        }

        _handler.Invoke();
    }
}
