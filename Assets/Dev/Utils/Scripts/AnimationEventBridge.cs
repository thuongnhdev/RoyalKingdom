using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventBridge : MonoBehaviour
{
    public UnityEvent onAnimationEvent;

    public void TriggerEventBridge()
    {
        onAnimationEvent.Invoke();
    }
}
