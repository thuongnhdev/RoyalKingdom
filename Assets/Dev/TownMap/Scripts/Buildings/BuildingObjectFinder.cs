using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingObjectFinder : MonoSingleton<BuildingObjectFinder>
{
    public int BuildingCount => _buildingLocationDict.Count;

    private Dictionary<int, GameObject> _buildingLocationDict = new();
    private Dictionary<int, GameObject> _buildingObjIdDict = new();

    public void AddBuildingByLocation(int topLeftTile, GameObject buildingObj)
    {
        _buildingLocationDict[topLeftTile] = buildingObj;
    }

    public void AddBuildingByObjectId(int buildingObjectId, GameObject buildingObj)
    {
        _buildingObjIdDict[buildingObjectId] = buildingObj;
    }

    public GameObject GetBuildingObjectByLocation(int locationTileId)
    {
        _buildingLocationDict.TryGetValue(locationTileId, out GameObject building);
        if (building == null)
        {
            Debug.LogWarning($"No building at tileId [{locationTileId}]");
        }
        return building;
    }

    public GameObject GetBuildingByBuildingObjectId(int objectId)
    {
        _buildingObjIdDict.TryGetValue(objectId, out GameObject building);
        if (building == null)
        {
            Debug.LogWarning($"Cannot find building with ObjectId [{objectId}]");
        }

        return building;
    }

#if UNITY_EDITOR
    private void UpdateBuildingLocation(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingId = (int)args[0];
        int oldLocationCenterTileId = (int)args[1];
        int newLocationCenterTileId = (int)args[2];
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(buildingId);
        int oldLocationTopLeftTileId = TownMapHelper.FromCenterToTopLeftTile(50, 50, buildingSize, oldLocationCenterTileId);
        int newLocationTopLeftTileId = TownMapHelper.FromCenterToTopLeftTile(50, 50, buildingSize, newLocationCenterTileId);

        var relocatedBuilding = _buildingLocationDict[oldLocationTopLeftTileId];
        _buildingLocationDict[newLocationTopLeftTileId] = relocatedBuilding;
        _buildingLocationDict.Remove(oldLocationTopLeftTileId);
    }

    protected override void DoOnEnable()
    {
        base.DoOnEnable();

        var buildingPosChanged = UnityEditor.AssetDatabase.LoadAssetAtPath<GameEvent>("Assets\\Dev\\TownMap\\SOs\\Events\\BuildingEvents\\Legacy\\Evt_TownMap_OnBuildingPosChanged.asset");
        buildingPosChanged.Subcribe(UpdateBuildingLocation);
    }

    protected override void DoOnDisable()
    {
        base.DoOnDisable();

        var buildingPosChanged = UnityEditor.AssetDatabase.LoadAssetAtPath<GameEvent>("Assets\\Dev\\TownMap\\SOs\\Events\\BuildingEvents\\Legacy\\Evt_TownMap_OnBuildingPosChanged.asset");
        buildingPosChanged.Unsubcribe(UpdateBuildingLocation);
    }
#endif
}
