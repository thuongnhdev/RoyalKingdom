using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScreenPosition : MonoBehaviour
{
    [SerializeField]
    private Transform _trackedTarget;
    [SerializeField]
    private Vector3Variable _outputScreenPos;
    [SerializeField]
    private Canvas _renderCanvas;

    private Canvas _rootCanvas;
    private Canvas RootCanvas
    {
        get
        {
            if (_rootCanvas == null)
            {
                _rootCanvas = _renderCanvas.rootCanvas;
            }

            return _rootCanvas;
        }
    }

    private void OnEnable()
    {
        CalculateScreenPos();
    }

    private void CalculateScreenPos()
    {
        _outputScreenPos.Value = RootCanvas.worldCamera.WorldToScreenPoint(_trackedTarget.position);
    }

    private void Update()
    {
        if (_trackedTarget.hasChanged)
        {
            CalculateScreenPos();
            _trackedTarget.hasChanged = false;
        }
    }
}
