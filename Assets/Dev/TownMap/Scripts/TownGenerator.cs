using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreData.UniFlow.Common;
using System;
using Cysharp.Threading.Tasks;

public class TownGenerator : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownMapTerrainSOList _terrainSOList;
    [SerializeField]
    private TownBaseBuildingSOList _buildingSOList;
    [SerializeField]
    private TownMapSO _townMapSO;

    [Header("Config")]
    [SerializeField]
    private Transform _tilesHolder;
    [SerializeField]
    private Transform _buildingsHolder;
    [SerializeField]
    private BuildingSpawner _buildingSpawner;
    [SerializeField]
    private ResourceSpawner _resourceSpawner;

    public void Generate()
    {
        if (_townMapSO == null || _tilesHolder == null || _buildingsHolder == null || _terrainSOList == null)
        {
            return;
        }
        int x = _townMapSO.xSize;
        int y = _townMapSO.ySize;
        float xOffset = _townMapSO.xOffset;
        float yOffset = _townMapSO.yOffset;

        // Top Left tile, also remap y axis in data to z axis in map
        Vector3 firstTilePos = new Vector3
        {
            x = -x / 2f + xOffset / 2f,
            y = -0.15f,
            z = y / 2f + yOffset / 2f // a litle trick here. first tile zPosition should be y/2 - yOffset/2, we will subtract it by offset in the first loop
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

                var genTile = GenerateTerrainTile(currentTilePos, tile);

#if UNITY_EDITOR
                var text = genTile.transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
                text.text = tileId.ToString();
#endif

                currentTilePos.x += xOffset;
                tileId++;

                if (tile == null)
                {
                    continue;
                }
                GenerateResourceObjects(tile.containedResources, tile.tileId);
            }
        }
    }

    private GameObject GenerateTerrainTile(Vector3 currentTilePos, TilePoco tile)
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

        GameObject genTile = Instantiate(tilePrefab, _tilesHolder);
        genTile.transform.localPosition = currentTilePos;

        return genTile;
    }

    private void GenerateResourceObjects(List<ResourcePoco> resources, int location)
    {
        if (resources == null || resources.Count == 0)
        {
            return;
        }

        _resourceSpawner.SpawnInitResourceFollowMapData(resources, location);
    }

    private async UniTaskVoid Start()
    {
        await UniTask.DelayFrame(1); // Wait for other objects subcribe events
        Generate();
    }
  
}


