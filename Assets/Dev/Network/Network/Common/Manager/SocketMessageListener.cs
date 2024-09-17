using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketMessageListener : MonoBehaviour
{
    [SerializeField] // RK TODO if message count increases, consider using a Dictionary for fast lookup
    private List<MessageAndEventPair> _messageAndEventPairs;
    private void DispatchMessage(int messageType, byte[] data)
    {
        for (int i = 0; i < _messageAndEventPairs.Count; i++)
        {
            var messEventPair = _messageAndEventPairs[i];
            if ((int)messEventPair.messageType != messageType)
            {
                continue;
            }
            messEventPair.corespondingEvent.Raise(data);
        }
    }

    private void OnEnable()
    {
        PacketManager.OnReceivedSocketMessage += DispatchMessage;
    }

    private void OnDisable()
    {
        PacketManager.OnReceivedSocketMessage -= DispatchMessage;
    }

    [System.Serializable]
    private class MessageAndEventPair
    {
        public Fbs.WorldMap messageType;
        public GameEvent corespondingEvent;
    }
}
