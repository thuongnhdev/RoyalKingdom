using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Reference - Write")]
    [SerializeField]
    private Vector3Variable _currentTouchScreenPos;
    [Header("Config")]
    [SerializeField]
    private int _cancelClickDistanceSqr = 100;

    [SerializeField]
    private UnityEvent _onClicked;
    [SerializeField]
    private UnityEvent _onStartDrag;
    [SerializeField]
    private UnityEvent _onEndDrag;
    [SerializeField]
    private UnityEvent _onEndRightDrag;
    [SerializeField]
    private UnityEvent _onEndLeftDrag;

    private Vector2 _downPoint;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _onStartDrag.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _currentTouchScreenPos.Value = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onEndDrag.Invoke();
        if (_downPoint.x < eventData.position.x)
        {
            Debug.Log("Right!");
            _onEndRightDrag.Invoke();
            return;
        }

        Debug.Log("Left");
        _onEndLeftDrag.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _downPoint = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_cancelClickDistanceSqr < Vector2.SqrMagnitude(eventData.position - _downPoint))
        {
            return;
        }

        _onClicked.Invoke();
    }
}
