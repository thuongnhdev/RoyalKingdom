using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, ISceneGameObjectDragHandler, ISceneGameObjectPoiterDownHandler
{
    [SerializeField]
    private bool _isDraggable;
    public bool IsDraggable
    {
        get
        {
            return _isDraggable;
        }
        set
        {
            _isDraggable = value;
        }
    }

    [SerializeField]
    private UnityEvent<Vector3> _onPointerDown;
    [SerializeField]
    private UnityEvent<Vector3> _onDraggedT1;
    [SerializeField]
    private UnityEvent<Vector3> _onEndDragT1;

    public bool IsDragActive()
    {
        return _isDraggable;
    }

    public void OnPointerDown(Vector3 downPointer)
    {
        _onPointerDown.Invoke(downPointer);
    }

    public void OnDragged(Vector3 dragPointer)
    {
        _onDraggedT1.Invoke(dragPointer);
    }

    public void OnEndDrag(Vector3 userPointedPos)
    {
        _onEndDragT1.Invoke(userPointedPos);
    }
}
