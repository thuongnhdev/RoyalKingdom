using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraInteraction : MonoBehaviour
{
    [Header("Refrence - Read/Write")]
    [SerializeField]
    private FloatVariable _mainCamSize;

    [Header("Config")]
    [SerializeField]
    private Transform _cameraHolder;
    [SerializeField]
    private Camera _refererenceCamera;
    [SerializeField]
    private float _dragSensitivity = 2f;
    [SerializeField]
    private Vector2 _camSizeClamp = new Vector2(5f, 20f);

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onDragSceen;
    [SerializeField]
    private GameEvent _onActive2PointDrag;
    [SerializeField]
    private GameEvent _onDrag2Points;

    [Header("Inspect")]
    [SerializeField]
    private float _meterPerPixel;
    [SerializeField]
    private float _cameraAngle;
    [SerializeField]
    private float _cameraSinAngle;

    private float _ratioCamSize;

    private void DragCamera(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        PointerEventData eventData = (PointerEventData)args[0];
        Vector2 dragDelta = eventData.delta;

        float height = _refererenceCamera.pixelHeight;
        _meterPerPixel = _refererenceCamera.orthographicSize * 2f / height;

        Vector3 worldDragDelta = _cameraHolder.right * dragDelta.x * _meterPerPixel
                                + _cameraHolder.forward * dragDelta.y * _meterPerPixel / _cameraSinAngle;

        _cameraHolder.position -= worldDragDelta * _dragSensitivity;      
    }

    private void SetRatioCamSize(object[] args)
    { 
        _ratioCamSize = _mainCamSize.Value;
    }

    private void AdjustZoom(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        float zoomRatio = 1f / (float)args[0];
        float newCamsize = Mathf.Clamp(_ratioCamSize * zoomRatio, _camSizeClamp.x, _camSizeClamp.y);
        _mainCamSize.Value = newCamsize;
    }

    private void OnEnable()
    {
        _onDragSceen.Subcribe(DragCamera);
        _onActive2PointDrag.Subcribe(SetRatioCamSize);
        _onDrag2Points.Subcribe(AdjustZoom);

        _cameraAngle = Vector3.Angle(_refererenceCamera.transform.forward, _cameraHolder.forward);
        _cameraSinAngle = Mathf.Abs(Mathf.Sin(_cameraAngle * Mathf.Deg2Rad));

    }

    private void OnDisable()
    {
        _onDragSceen.Unsubcribe(DragCamera);
        _onActive2PointDrag.Unsubcribe(SetRatioCamSize);
        _onDrag2Points.Unsubcribe(AdjustZoom);

    }
}
