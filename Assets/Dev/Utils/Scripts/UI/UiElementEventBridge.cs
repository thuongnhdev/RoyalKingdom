using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiElementEventBridge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Image _targetImage;
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp.Invoke();
    }
}
