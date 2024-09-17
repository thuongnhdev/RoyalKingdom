using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TownMapSynchronizer : MonoBehaviour
{
    [Header("Reference - Read/Write")]
    [SerializeField]
    private TownMapSO _userTownMap;

    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingListSO;
    [SerializeField]
    private UserBuildingList _userBuildingList;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onAddedNewBuilding;
    [SerializeField]
    private GameEvent _onDestroyBuildingData;


    [Header("Events out")]
    [SerializeField]
    private GameEvent _onResourceAddedToTileData;

    [Header("Cheating Events in")]
    [SerializeField]
    private GameEvent _onBuildingPosChanged;

    public void UpdateUserBuildingsToMap()
    {
        List<TilePoco> map = _userTownMap.tiles;
        for (int i = 0; i < map.Count; i++)
        {
            var tile = map[i];
            tile.baseBuildingValue = 0;
            tile.baseBuildingRootTile = tile.tileId;
        }

        List<UserBuilding> buildings = _userBuildingList.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            RequestAddNewBuilding(building.buildingId, building.locationTileId);
        }
    }

    private void RequestAddNewBuilding(params object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingId = (int)args[0];
        int buildingTopLeftTileId = (int)args[1];

        Vector2 buildingSize = _buildingListSO.GetBuildingSize(buildingId);
        List<int> buildingTiles = TownMapHelper.TopLeftGetAllTilesInArea(_userTownMap.xSize, _userTownMap.ySize, buildingSize, buildingTopLeftTileId);

        for (int i = 0; i < buildingTiles.Count; i++)
        {
            int tileId = buildingTiles[i];

            var tile = _userTownMap.GetTileInfo(tileId);

            tile.baseBuildingValue = buildingId;
            tile.baseBuildingRootTile = buildingTopLeftTileId;

            _userTownMap.AddOrUpdateATile(tile);
        }
    }

    private void RequestRemoveBuilding(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)eventParam[0];

        UserBuilding userBuilding = _userBuildingList.GetBuilding(buildingObjId);
        if (userBuilding == null)
        {
            return;
        }

        Vector2 buildingSize = _buildingListSO.GetBuildingSize(userBuilding.buildingId);

        int mapX = _userTownMap.xSize;
        int mapY = _userTownMap.ySize;

        int locationTile = userBuilding.locationTileId;

        List<int> buildingArea = TownMapHelper.TopLeftGetAllTilesInArea(mapX, mapY, buildingSize, locationTile);
        
        for (int i = 0; i < buildingArea.Count; i++)
        {
            int tileId = buildingArea[i];
            var tile = _userTownMap.GetTileInfo(tileId);
            if (tile == null)
            {
                continue;
            }

            tile.baseBuildingValue = 0;
            tile.baseBuildingRootTile = tileId;
        }
    }

    private void Cheating_RequestChangeBuildingPos(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        // TODO Server logic here

        int buildingId = (int)args[0];
        int oldCenterTile = (int)args[1];
        int newCenterTile = (int)args[2];

        int mapX = _userTownMap.xSize;
        int mapY = _userTownMap.ySize;

        Vector2 buildingSize = _buildingListSO.GetBuildingSize(buildingId);
        int oldTopLeftTileId = TownMapHelper.FromCenterToTopLeftTile(mapX, mapY, buildingSize, oldCenterTile);
        int newTopLeftTileId = TownMapHelper.FromCenterToTopLeftTile(mapX, mapY, buildingSize, newCenterTile);

        List<int> oldArea = TownMapHelper.TopLeftGetAllTilesInArea(mapX, mapY, buildingSize, oldTopLeftTileId);
        List<int> newArea = TownMapHelper.TopLeftGetAllTilesInArea(mapX, mapY, buildingSize, newTopLeftTileId);

        for (int i = 0; i < oldArea.Count; i++)
        {
            var oldTile = _userTownMap.GetTileInfo(oldArea[i]);
            oldTile.baseBuildingValue = 0;
            oldTile.baseBuildingRootTile = oldTile.tileId;
        }

        for (int i = 0; i < newArea.Count; i++)
        {
            var newTile = _userTownMap.GetTileInfo(newArea[i]);
            newTile.baseBuildingValue = buildingId;
            newTile.baseBuildingRootTile = newTopLeftTileId;

            _userTownMap.AddOrUpdateATile(newTile);
        }
    }

    private void OnEnable()
    {
        _onAddedNewBuilding.Subcribe(RequestAddNewBuilding);
        _onDestroyBuildingData.Subcribe(RequestRemoveBuilding);

        _onBuildingPosChanged.Subcribe(Cheating_RequestChangeBuildingPos);
    }

    private void OnDisable()
    {
        _onAddedNewBuilding.Unsubcribe(RequestAddNewBuilding);
        _onDestroyBuildingData.Unsubcribe(RequestRemoveBuilding);

        _onBuildingPosChanged.Unsubcribe(Cheating_RequestChangeBuildingPos);
    }

    private void Editor_SaveUserMap()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(_userTownMap);
#endif
    }
}
