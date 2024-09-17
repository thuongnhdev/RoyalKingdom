using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustObjScaleFollowingCamSize : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _cameraSize;

    [Header("Config")]
    [SerializeField]
    private Transform _adjustedTransform;
    [SerializeField]
    private float _referenceCamSize = 10f;

    private void AdjustScale(float camSize)
    {
        float scaleRatio = _cameraSize.Value / _referenceCamSize;
        _adjustedTransform.localScale = Vector3.one * scaleRatio;
    }

    private void OnEnable()
    {
        AdjustScale(_cameraSize.Value);
        _cameraSize.OnValueChange += AdjustScale;
    }

    private void OnDisable()
    {
        _cameraSize.OnValueChange -= AdjustScale;
    }
}
