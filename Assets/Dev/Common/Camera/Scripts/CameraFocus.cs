using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _focusPoint;
    [SerializeField]
    private IntegerVariable _focusTile;
    [SerializeField]
    private TownMapSO _map;
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Config")]
    [SerializeField]
    private float _easeTime = 0.5f;
    [SerializeField]
    private Ease _cameraEase;
    [SerializeField]
    private LayerMask _groundLayer;

    [Header("Inspec")]
    [SerializeField]
    private Transform _cameraHolder;
    [SerializeField]
    private Camera _mainCamera;

    public void FocusToBuilding(int buildingId)
    {
        List<int> buildings = _userBuildings.GetAllBuildingsOfId(buildingId);
        if (buildings.Count == 0)
        {
            Logger.LogWarning($"No building of Id [{buildingId}] to focus");
            return;
        }
        int location = _userBuildings.GetBuildingLocation(buildings[0]);
        MoveToFocusTile(location);
    }

    public void MoveToFocusPoint(Vector3 buildingPosition)
    {
        Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward * 500f, out var hit, _groundLayer);

        Vector3 currentFocusPoint = hit.point;
        Vector3 cameraMoveDirection = buildingPosition - currentFocusPoint;

        _cameraHolder.DOMove(_cameraHolder.position + cameraMoveDirection, _easeTime).SetEase(_cameraEase).onComplete = () => 
        {
            _focusPoint.SetValueWithoutNotify(Vector3.zero);
        };
    }

    public void MoveToFocusTile(int tile)
    {
        Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward * 500f, out var hit, _groundLayer);
        Vector3 currentFocusPoint = hit.point;
        Vector3 focusTilePosition = TownMapHelper.GetTileLocalPosition(_map.xSize, _map.ySize, tile);
        Vector3 camMoveDirection = focusTilePosition - currentFocusPoint;

        _cameraHolder.DOMove(_cameraHolder.position + camMoveDirection, 0.3f).SetEase(_cameraEase).onComplete = () => 
        {
            _focusTile.SetValueWithoutNotify(0);
        };
    }

    private void OnEnable()
    {
        var camGetter = CameraGetter.Instance;
        _mainCamera = camGetter.MainCamera;
        _cameraHolder = camGetter.MainCameraHolder;

        _focusPoint.OnValueChange += MoveToFocusPoint;
        _focusTile.OnValueChange += MoveToFocusTile;
    }

    private void OnDisable()
    {
        _focusPoint.OnValueChange -= MoveToFocusPoint;
        _focusTile.OnValueChange -= MoveToFocusTile;
    }
}
