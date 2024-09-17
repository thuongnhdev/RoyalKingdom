#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CoreData.UniFlow.Common;
using TMPro;

public class TownMapGeneratorForEditor : MonoBehaviour
{
    [Header("For Designer")]
    [SerializeField]
    private TownMapSO _townMapSO;

    [Header("Reference - Read")]
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssetsList;

    [Header("Config")]
    [SerializeField]
    private Transform _tilesHolder;
    [SerializeField]
    private Transform _buildingsHolder;

    [Header("Look up")]
    [SerializeField]
    private TownMapTerrainSOList _terrainSOList;
    [SerializeField]
    private TownBaseBuildingSOList _buildingSOList;

    public void Editor_GenerateMap()
    {
        if (_townMapSO == null || _tilesHolder == null || _buildingsHolder == null || _terrainSOList == null)
        {
            return;
        }
        int x = _townMapSO.xSize;
        int y = _townMapSO.ySize;
        float xOffset = _townMapSO.xOffset;
        float yOffset = _townMapSO.yOffset;

        RemoveExitingTiles();
        RemoveExitingBuildings();

        // Top Left tile, also remap y axis in data to z axis in map
        Vector3 firstTilePos = new Vector3
        {
            x = -x / 2f + xOffset / 2f,
            y = 0f,
            z = y / 2f + yOffset / 2f // a litle trick here. first tile zPosition should be y/2 - yOffset/2, we will subtract it by -1f in the first loop
        };

        Dictionary<int, TilePoco> townMapSOTileDict = new Dictionary<int, TilePoco>();
        var tiles = _townMapSO.tiles;
        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            townMapSOTileDict[tile.tileId] = tile;
        }

        int tileId = 0;
        Vector3 currentTilePos = firstTilePos;
        for (int i = 0; i < x; i++)
        {
            currentTilePos.z -= yOffset;
            currentTilePos.x = firstTilePos.x;
            for (int j = 0; j < y; j++)
            {
                townMapSOTileDict.TryGetValue(tileId, out var tile);

                GenerateTerrainTile(currentTilePos, tile, tileId);
                currentTilePos.x += xOffset;
                tileId++;

                if (tile == null || tile.baseBuildingValue == 0 || tile.baseBuildingRootTile != tile.tileId)
                {
                    continue;
                }

                GenerateBaseBuilding(tile.baseBuildingValue, tileId - 1);
            }
        }
    }

    private void GenerateTerrainTile(Vector3 currentTilePos, TilePoco tile, int tileId)
    {
        GameObject tilePrefab;
        if (tile == null)
        {
            tilePrefab = _terrainSOList.GetDefaultTerrainPrefab();
        }
        else
        {
            tilePrefab = _terrainSOList.GetTerrainPrefab(tile.terrain);
        }

        GameObject genTile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab);
        genTile.transform.parent = _tilesHolder;
        genTile.transform.localPosition = currentTilePos;

        genTile.name = tileId.ToString();

        genTile.transform.GetChild(0).GetComponent<TMP_Text>().text = genTile.name;
    }

    private void GenerateBaseBuilding(int buildingId, int tileId)
    {
        GameObject buildingPrefab = _buildingAssetsList.GetBuildingModel(buildingId);

        if (buildingPrefab == null)
        {
            return;
        }

        GameObject genBuilding = (GameObject)PrefabUtility.InstantiatePrefab(buildingPrefab);
        genBuilding.transform.parent = _buildingsHolder;

        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(buildingId);

        Vector2 axis = TownMapHelper.FromTileIdToAxis(_townMapSO.xSize, _townMapSO.ySize, tileId);
        Vector3 position = new Vector3
        {
            x = axis.x - _townMapSO.xSize / 2f,
            y = 0f,
            z = -axis.y + _townMapSO.ySize / 2f
        };
        position.x += buildingSize.x / 2f * _townMapSO.xOffset;
        position.z -= buildingSize.y / 2f * _townMapSO.yOffset;

        genBuilding.transform.localPosition = position;
    }

    private void RemoveExitingTiles()
    {
        List<GameObject> removes = new List<GameObject>();
        int count = _tilesHolder.childCount;
        for (int i = 0; i < count; i++)
        {
            removes.Add(_tilesHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < removes.Count; i++)
        {
            DestroyImmediate(removes[i]);
        }
    }

    private void RemoveExitingBuildings()
    {
        List<GameObject> removes = new List<GameObject>();
        int count = _buildingsHolder.childCount;
        for (int i = 0; i < count; i++)
        {
            removes.Add(_buildingsHolder.GetChild(i).gameObject);
        }

        for (int i = 0; i < removes.Count; i++)
        {
            DestroyImmediate(removes[i]);
        }
    }

}
#endif