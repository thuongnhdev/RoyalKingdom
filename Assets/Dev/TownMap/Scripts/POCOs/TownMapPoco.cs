using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using Town.Tile;
using UnityEngine;

[System.Serializable]
public class TilePoco
{
    public string name;
    public int tileId;
    public bool buildable = true;
    public int baseBuildingValue = 0;
    public int baseBuildingRootTile;
    public TileTerrainType terrain;
    public List<ResourcePoco> containedResources = new();

    public TilePoco(int tileId)
    {
        this.tileId = tileId;
        name = (tileId + 1).ToString();
    }

    public TilePoco(Tile flatBufferData)
    {
        tileId = flatBufferData.TileId;
        name = (tileId + 1).ToString();
        buildable = flatBufferData.Buildable;
        baseBuildingValue = flatBufferData.BaseBuildingValue;
        baseBuildingRootTile = flatBufferData.BaseBuildingRootTile;
        terrain = (TileTerrainType)flatBufferData.Terrain;
    }
}

[System.Serializable]
public class BaseBuildingPoco
{
    public string name;
    public int id;
    public int maxLevel;
    public string description;
    public BuildingCategory category;
    public Vector2 size;
    public List<Vector2> doorsDirection;
    public float destructionTimeFactor;
    public bool unbreakable;
    public BuildingCommandKey command1;
    public BuildingCommandKey command2;

    public void SetData(BaseBuildingPoco data)
    {
        name = data.name;
        id = data.id;
        maxLevel = data.maxLevel;
        description = data.description;
        category = (BuildingCategory)(id / 10000 * 10000);
        size = data.size;
        doorsDirection = data.doorsDirection;
        destructionTimeFactor = data.destructionTimeFactor;
        unbreakable = data.unbreakable;
        command1 = data.command1;
        command2 = data.command2;
    }
}

[System.Serializable]
public class BuildingCondition
{
    public BuildingConditionType conditionType;
    public List<int> conditionValues = new();
}

public enum BuildingConditionType
{
    None = 0,
    BUILDING_LEVEL = 1,
    TRADE_VALUE = 2,
}
