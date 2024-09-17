using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGetter : MonoSingleton<CameraGetter>
{
    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;
    [SerializeField]
    private Transform _mainCameraHolder;
    public Transform MainCameraHolder => _mainCameraHolder;
    [SerializeField]
    private Camera _uiCamera;
    public Camera UICamera => _uiCamera;
}
