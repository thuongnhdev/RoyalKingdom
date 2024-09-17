using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFoundationMove : DraggableTilingMovement
{
    [Header("Reference - Read")]
    [Header("Child fields")]
    [SerializeField]
    private IntegerVariable _selectedBuildingId;
    [SerializeField]
    private Vector3Variable _groundTouchPos;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _newBuildingTopLeftTileId;
    [SerializeField]
    private IntegerVariable _newBuildingCenterTileId;
    [SerializeField]
    private FloatVariable _newBuildingRotation;
    [SerializeField]
    private Vector3Variable _newBuildingLocalPosition;

    [Header("Reference - Events out")]
    [SerializeField]
    private GameEvent _onConfirmedNewBuildingLocation;

    public void NotifyNewBuildingLocation()
    {
        int mapX = _userTownMapSO.xSize;
        int mapY = _userTownMapSO.ySize;

        int buildingCenterTileId = _tileTransform.GetCurrentTileId();
        int buildingTopLeftTileId = TownMapHelper.FromCenterToTopLeftTile(mapX, mapY, _size, buildingCenterTileId);

        _newBuildingTopLeftTileId.Value = buildingTopLeftTileId;
        _newBuildingCenterTileId.Value = buildingCenterTileId;
        _newBuildingRotation.Value = _tileTransform.GetRotation();
        _newBuildingLocalPosition.Value = _tileTransform.GetLocalPosition();

        _onConfirmedNewBuildingLocation.Raise(_selectedBuildingId.Value, buildingTopLeftTileId);
    }

    protected override bool ValidatePosition(int currentTileIdPos)
    {
        return TownMapHelper.CenterCanBuildBuildingOnTile(_userTownMapSO.TileDict, _userTownMapSO.xSize, _userTownMapSO.ySize, _size, currentTileIdPos);
    }

    protected override void DoOnEnable()
    {
        base.DoOnEnable();
        _groundTouchPos.OnValueChange += MoveToTouchPoint;
    }

    protected override void DoOnDisable()
    {
        base.DoOnDisable();
        _groundTouchPos.OnValueChange -= MoveToTouchPoint;
    }

    private void MoveToTouchPoint(Vector3 touchPoint)
    {
        int destination = TownMapHelper.GetTileIdAtPosition(_userTownMapSO.xSize, _userTownMapSO.ySize, touchPoint);
        _tileTransform.MoveTo(destination);
    }
}
