using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneGameObjectInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
{
    [Header("Config")]
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private LayerMask _interactableLayer;
    [SerializeField][Tooltip("Distance of dragging to cancel a Click, unit in pixel")]
    private float _clickCancelDistance = 20f;
    [SerializeField]
    private float _maxInteractDistance = 1000f;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onChangeClickObject;
    [SerializeField]
    private GameEvent _onDragScreen;
    [SerializeField]
    private GameEvent _onActive2PointsDrag;
    [SerializeField]
    private GameEvent _on2PointsDrag;

    private ISceneGameObjectDragHandler _draggedObject;

    private Vector3 _dragPointerPos;
    private float _dragDistance = 0f;

    private int _lastClickObjectId = -1;
    private List<PointerEventData> _touches = new();
    private bool _active2Touches = false;
    private float _2pointsInitDistance = 0f;
    private float _2pointsCurrentDistanceWithInitial;

    public void OnDrag(PointerEventData eventData)
    {
        if (_active2Touches)
        {
            Active2TouchesFlow();
            return;
        }

        _dragDistance += eventData.delta.magnitude;

        if (_draggedObject == null)
        {
            _onDragScreen.Raise(eventData);
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(eventData.position);
        if (!Physics.Raycast(ray, out var hit, _maxInteractDistance, _interactableLayer))
        {
            return;
        }

        _dragPointerPos = hit.point;

        _draggedObject.OnDragged(_dragPointerPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedObject == null)
        {
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(eventData.position);
        if (!Physics.Raycast(ray, out var hit, _maxInteractDistance, _interactableLayer))
        {
            return;
        }

        _dragPointerPos = hit.point;
        _draggedObject.OnEndDrag(_dragPointerPos);
        _dragDistance = 0f;
        _draggedObject = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_active2Touches)
        {
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(eventData.position);

        if (!Physics.Raycast(ray, out var hit, 1000f, _interactableLayer))
        {
            _lastClickObjectId = -1;
            return;
        }
        Logger.Log($"Clicked object {hit.collider.name}");
        int clickObjId = hit.collider.GetInstanceID();
        if (clickObjId != _lastClickObjectId)
        {
            _onChangeClickObject.Raise();
            _lastClickObjectId = clickObjId;
        }

        hit.collider.GetComponent<ISceneGameObjectClickHandler>()?.OnPointerClicked(hit.point);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragDistance = 0f;

        if (!IsValidTouch(eventData))
        {
            return;
        }

        _touches.Add(eventData);
        if (_touches.Count == 2)
        {
            _active2Touches = true;
            CalculateInit2TouchesDistance(eventData);
            _onActive2PointsDrag.Raise();
            return;
        }

        var ray = _mainCamera.ScreenPointToRay(eventData.position);
        if (!Physics.Raycast(ray, out var hit, 1000f, _interactableLayer))
        {
            return;
        }

        hit.collider.GetComponent<ISceneGameObjectPoiterDownHandler>()?.OnPointerDown(hit.point);

        var draggable = hit.collider.GetComponent<ISceneGameObjectDragHandler>();

        if (draggable == null || !draggable.IsDragActive())
        {
            return;
        }

        _draggedObject = draggable;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RemoveTouch(eventData);
        if (_touches.Count > 0)
        {
            _active2Touches = false;
            return;
        }

        if (_dragDistance < _clickCancelDistance)
        {
            OnPointerClick(eventData);
        }

        var ray = _mainCamera.ScreenPointToRay(eventData.position);
        if (!Physics.Raycast(ray, out var hit, 1000f, _interactableLayer))
        {
            return;
        }

        hit.collider.GetComponent<ISceneGameObjectPointerUpHandler>()?.OnPointerUp(hit.point);
    }

    private bool IsValidTouch(PointerEventData touch)
    {
        if (_active2Touches)
        {
            return false;
        }

        if (_touches.Count >= 2)
        {
            return false;
        }

        for (int i = 0; i < _touches.Count; i++)
        {
            if (_touches[i].pointerId == touch.pointerId)
            {
                return false;
            }
        }

        return true;
    }

    private void RemoveTouch(PointerEventData touch)
    {
        for (int i = 0; i < _touches.Count; i++)
        {
            if (_touches[i].pointerId == touch.pointerId)
            {
                _touches.RemoveAt(i);
                return;
            }
        }
    }

    private void CalculateInit2TouchesDistance(PointerEventData secondTouch)
    {
        _2pointsInitDistance = Vector2.Distance(_touches[0].position, secondTouch.position);
    }

    private void Calculate2PointsDistanceRatio()
    {
        _2pointsCurrentDistanceWithInitial = Vector2.Distance(_touches[0].position, _touches[1].position) / _2pointsInitDistance;
        Debug.Log("pinch ratio: " + _2pointsCurrentDistanceWithInitial);
    }

    private void Active2TouchesFlow()
    {
        Calculate2PointsDistanceRatio();
        _on2PointsDrag.Raise(_2pointsCurrentDistanceWithInitial);
    }

    private void OnEnable()
    {
        _touches.Capacity = 2;
    }
}
