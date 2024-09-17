using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTownMapTerrainList", menuName = "Uniflow/Map/TownMapTerrainList")]
public class TownMapTerrainSOList : ScriptableObject
{
    [SerializeField]
    private List<TownMapTileTerrain> _townTileTerrains;
    private Dictionary<TileTerrainType, TownMapTileTerrain> _townTileTerrainsMap = new Dictionary<TileTerrainType, TownMapTileTerrain>();
    private Dictionary<TileTerrainType, TownMapTileTerrain> TownTileTerrainsMap
    {
        get
        {
            if (_townTileTerrains.Count != _townTileTerrainsMap.Count)
            {
                _townTileTerrainsMap.Clear();

                for (int i = 0; i < _townTileTerrains.Count; i++)
                {
                    var terrain = _townTileTerrains[i];
                    _townTileTerrainsMap[terrain.Terrain] = terrain;
                }
            }

            return _townTileTerrainsMap;
        }
    }

    [SerializeField]
    private GameObject _defaultTerrain;

    public GameObject GetTerrainPrefab(TileTerrainType terrain)
    {
        TownTileTerrainsMap.TryGetValue(terrain, out var townTerrain);
        if (townTerrain == null)
        {
            Debug.LogError($"Invalid terrain value [{terrain}]");
        }

        return townTerrain.Prefab;
    }
     public GameObject GetDefaultTerrainPrefab()
    {
        return _defaultTerrain;
    }
}

[System.Serializable]
public class TownMapTileTerrain
{
    public TileTerrainType Terrain;
    public GameObject Prefab;
}
