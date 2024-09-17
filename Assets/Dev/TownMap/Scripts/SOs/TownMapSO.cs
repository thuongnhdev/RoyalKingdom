using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewTownMap", menuName = "Uniflow/Map/TownMap")]
public class TownMapSO : ScriptableObject
{
    public string townMapId;
    public int landId;
    public int xSize;
    public int ySize;
    public float xOffset = 1f;
    public float yOffset = 1f;
    public List<TilePoco> tiles = new List<TilePoco>();

    private static string MAP_FILE_PATH = "Assets\\Dev\\UserData\\UserTown\\Var_Town_UserTownMap.asset";
    private Dictionary<int, TilePoco> _tileDict = new Dictionary<int, TilePoco>();
    public Dictionary<int, TilePoco> TileDict
    {
        get
        {
            if (_tileDict.Count != tiles.Count)
            {
                _tileDict.Clear();
                for (int i = 0; i < tiles.Count; i++)
                {
                    var tile = tiles[i];
                    _tileDict[tile.tileId] = tile;
                }
            }
            return _tileDict;
        }
    }
    
    public TilePoco GetTileInfo(int tileId)
    {
        TileDict.TryGetValue(tileId, out var tile);
        if (tile == null)
        {
            tile = new TilePoco(tileId);
        }

        return tile;
    }

    public void AddOrUpdateATile(TilePoco tile)
    {
        if (!TileDict.ContainsKey(tile.tileId))
        {
            tiles.Add(tile);
            _tileDict[tile.tileId] = tile;
        }

        // tile data is reference type, no need to re-update if it is modified from outside 
    }
#if UNITY_EDITOR
    public void EditorOnly_ApplyDataToSoUsedInGame()
    {
        var userMapData = AssetDatabase.LoadAssetAtPath<TownMapSO>(MAP_FILE_PATH);
        if (userMapData == null)
        {
            Debug.LogError($"Cannot find Var_Town_UserTownMap at path [{MAP_FILE_PATH}]", this);
            return;
        }

        userMapData.tiles = tiles;
        userMapData.TileDict.Clear();
        EditorUtility.SetDirty(userMapData);

        //var userBuildings = AssetDatabase.LoadAssetAtPath<UserBuildingList>("Assets\\Dev\\UserData\\UserBuildings\\SOs\\UserBuildingList.asset");
        //userBuildings.ClearData();

        //List<UserBuilding> userBuildingList = userBuildings.BuildingList;
        
        //for (int i = 0; i < tiles.Count; i++)
        //{
        //    var tile = tiles[i];
        //    if (tile.baseBuildingValue == 0)
        //    {
        //        continue;
        //    }

        //    if (tile.baseBuildingRootTile != tile.tileId)
        //    {
        //        continue;
        //    }

        //    var userBuilding = new UserBuilding();
        //    userBuilding.buildingObjectId = System.DateTime.Now.Millisecond + i;
        //    userBuilding.buildingId = tile.baseBuildingValue;
        //    userBuilding.buildingLevel = 1;
        //    userBuilding.status = BuildingStatus.Idle;
        //    userBuilding.locationTileId = tile.tileId;

        //    userBuildingList.Add(userBuilding);
        //}

        //EditorUtility.SetDirty(userBuildings);
    }

    

    private void EditorOnly_CheckBuildingDataVersion()
    {
        string townMapBuildingDataVersionKey = "TownMapBuildingDataVersion" + GetInstanceID();

        string townMapBuildingDataVersion = EditorPrefs.GetString(townMapBuildingDataVersionKey, "");
        string buildingDataVersion = EditorPrefs.GetString(TownBaseBuildingSOList.EDITORONLY_BUILDINGDATAVERSION_KEY);
        if (townMapBuildingDataVersion == buildingDataVersion)
        {
            return;
        }

        EditorOnly_UpdateBuildingDataInTownMap();

        EditorPrefs.SetString(townMapBuildingDataVersionKey, buildingDataVersion);
    }

    private void EditorOnly_UpdateBuildingDataInTownMap()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            if (tile.baseBuildingValue == 0)
            {
                continue;
            }

            int buildingId = tile.baseBuildingValue;
            int rootTile = tile.baseBuildingRootTile;

            if (tile.tileId == tile.baseBuildingRootTile)
            {
                EditorOnly_GenerateBuildingDataFromRootTile(buildingId, rootTile);
                continue;
            }

            EditorOnly_ProcessNonRootBuildingTile(tile);
        }

        EditorUtility.SetDirty(this);
    }

    private void EditorOnly_GenerateBuildingDataFromRootTile(int buildingId, int rootTile)
    {
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(buildingId);
        List<int> buildingTiles = TownMapHelper.TopLeftGetAllTilesInArea(xSize, ySize, buildingSize, rootTile);
        for (int i = 0; i < buildingTiles.Count; i++)
        {
            var tile = GetTileInfo(buildingTiles[i]);
            tile.baseBuildingValue = buildingId;
            tile.baseBuildingRootTile = rootTile;

            AddOrUpdateATile(tile);
        }
    }

    private void EditorOnly_ProcessNonRootBuildingTile(TilePoco tile)
    {
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(tile.baseBuildingValue);
        Vector2 directionToRoot = TownMapHelper.GetDirectionOf2Tiles(xSize, ySize, tile.baseBuildingRootTile, tile.tileId);
        Vector2 xyDistance = new Vector2(Mathf.Abs(directionToRoot.x), Mathf.Abs(directionToRoot.y));

        if (xyDistance.x <= buildingSize.x && xyDistance.y <= buildingSize.y)
        {
            return;
        }

        tile.baseBuildingValue = 0;
        tile.baseBuildingRootTile = tile.tileId;
    }
    private void OnValidate()
    {
        TownBaseBuildingSOList.EditorOnly_OnBuildingDataChanged += EditorOnly_CheckBuildingDataVersion;
    }
#endif
}
