using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiGameEventsListener : MonoBehaviour
{
    [SerializeField]
    private List<GameEvent> _gameEvents;
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

        _handler.Invoke();
    }
}
