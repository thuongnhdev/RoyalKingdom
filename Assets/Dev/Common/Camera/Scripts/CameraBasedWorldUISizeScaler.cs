using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBasedWorldUISizeScaler : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _cameraSize;

    [Header("Config")]
    [SerializeField]
    private bool _refreshOnEnable = true;
    [SerializeField]
    private List<RectTransform> _targetsForScale;
    [SerializeField]
    private float _referenceCameraSize = 10f;

    private List<Vector3> _initConstrainedObjPos = new();

    public void Refresh()
    {
        UpdateObjectSize(_cameraSize.Value);
    }

    private void UpdateObjectSize(float newValue)
    {
        for (int i = 0; i < _targetsForScale.Count; i++)
        {
            _targetsForScale[i].localScale = Vector3.one * (_cameraSize.Value / _referenceCameraSize);
        }
    }

    private void OnEnable()
    {
        _cameraSize.OnValueChange += UpdateObjectSize;

        if (!_refreshOnEnable)
        {
            return;
        }
        Refresh();
    }

    private void OnDisable()
    {
        _cameraSize.OnValueChange -= UpdateObjectSize;
    }


}
