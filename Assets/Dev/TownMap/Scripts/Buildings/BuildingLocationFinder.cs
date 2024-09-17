using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLocationFinder : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownMapSO _userTownMap;
    [SerializeField]
    private TownBaseBuildingSOList _buildingList;
    [SerializeField]
    private IntegerVariable _selectedBuildingId;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _foundTopLeftTileId;
    [SerializeField]
    private IntegerVariable _foundCenterTileId;
    [SerializeField]
    private Vector3Variable _foundBuildingPos;

    [Header("Config")]
    [SerializeField]
    private Camera _mainCam;
    [SerializeField]
    private float _maxCastDistance = 500f;
    [SerializeField]
    private LayerMask _groundLayer;

    [Header("Inspec")]
    [SerializeField]
    private int _pointedTile;

    public void In_FindLocationForNewBuilding()
    {
        int mapX = _userTownMap.xSize;
        int mapY = _userTownMap.ySize;

        Vector2 buildingSize = _buildingList.GetBuildingSize(_selectedBuildingId.Value);

        Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out var hit, _maxCastDistance, _groundLayer);
        if (hit.collider == null)
        {
            _pointedTile = (_userTownMap.xSize * _userTownMap.ySize - 1) / 2;
        }
        else
        {
            _pointedTile = TownMapHelper.GetTileIdAtPosition(mapX, mapY, hit.point);
        }

        int topLeftBuildingTile = TownMapHelper.FindLocationForBuildingAroundTile(_userTownMap.TileDict, mapX, mapY, buildingSize, _pointedTile);
        _foundTopLeftTileId.Value = topLeftBuildingTile;

        int centerTileId = TownMapHelper.FromTopLeftToCenterTile(mapX, mapY, buildingSize, topLeftBuildingTile);
        _foundCenterTileId.Value = centerTileId;

        _foundBuildingPos.Value = TownMapHelper.GetBuildingPosition(mapX, mapY, centerTileId, buildingSize);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_mainCam.transform.position, _mainCam.transform.forward * 500f, Color.white);
    }
}
