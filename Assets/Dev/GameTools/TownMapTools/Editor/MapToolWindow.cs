using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;


public class MapToolWindow : EditorWindow
{
    private const string TOWNMAP_SO_DIR = "Assets\\CommonData\\GeneratedTownMapData\\";
    public string townMapIdTextField;
    public string currentMapFileName;

    private string sizeXStr = "0";
    public int mapXSize = 0;
    private string sizeYStr = "0";
    public int mapYSize = 0;

    public string townPrefabNo;
    public string selectedTilesString = "No tile selected";

    public bool buildable = false;
    public int baseBuildingValue = 0;
    private string baseBuildingValStr;
    public TileTerrainType tileTerrain;

    private Vector2 _scrollPos;

    private Dictionary<int, TilePoco> _currentTownSoTiles = new Dictionary<int, TilePoco>();
    private HashSet<int> _selectedTiles = new HashSet<int>();

    private TownMapSO _currentTownMapReference;

    [MenuItem("Uniflow/Tools/MapTool", false, 0)]
    public static void ShowWindow()
    {
        GetWindow(typeof(MapToolWindow), false, "Map Tools");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Settings", EditorStyles.boldLabel);
        CreateMapSettings();
        GUILayout.Space(20);

        GUILayout.Label("Tile Settings", EditorStyles.boldLabel);
        CreateTileSettings();
        GUILayout.Space(25);

        CreateMapTilesVisual();

        CheckTileHover();
    }

    private void Update()
    {
        Repaint();
        CheckTileUnderBuildingFoundation();
    }

    private void CreateMapSettings()
    {
        currentMapFileName = _currentTownMapReference != null ? _currentTownMapReference.townMapId : "NA";
        GUILayout.Label($"Current TownMap Id: {currentMapFileName}", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        townMapIdTextField = EditorGUILayout.TextField("TownMapId", townMapIdTextField);
        if (GUILayout.Button("Load/Create"))
        {
            LoadOrCreateTownMapData(townMapIdTextField);
        }
        CreateLoadSelectedTownMapFileButton();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Size");
        sizeXStr = EditorGUILayout.TextField("X", sizeXStr);
        int.TryParse(sizeXStr, out mapXSize);
        sizeYStr = EditorGUILayout.TextField("Y", sizeYStr);
        int.TryParse(sizeYStr, out mapYSize);
        GUILayout.EndHorizontal();

        townPrefabNo = EditorGUILayout.TextField("Prefab No.", townPrefabNo);

    }

    private void CreateLoadSelectedTownMapFileButton()
    {
        var selected = Selection.activeObject;
        string loadSelectedText = "NA";
        if (selected is TownMapSO)
        {
            loadSelectedText = $"Load [{selected.name}]";
        }
        if (GUILayout.Button(loadSelectedText))
        {
            if (!(selected is TownMapSO))
            {
                return;
            }

            _currentTownMapReference = (TownMapSO)(selected);
            FloodTownMapSOToWindowFields();
        }
    }

    private void LoadOrCreateTownMapData(string mapIdString)
    {
        if (string.IsNullOrEmpty(mapIdString))
        {
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append(TOWNMAP_SO_DIR).Append(mapIdString).Append(".asset");

        string path = sb.ToString();
        _currentTownMapReference = AssetDatabase.LoadAssetAtPath<TownMapSO>(path);
        if (_currentTownMapReference == null)
        {
            _currentTownMapReference = CreateNewTownMapSO(path);
            FloodWindowFieldsToTownMapSO();
            return;
        }

        FloodTownMapSOToWindowFields();
    }

    private TownMapSO CreateNewTownMapSO(string path)
    {
        TownMapSO townMap = CreateInstance<TownMapSO>();
        AssetDatabase.CreateAsset(townMap, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();

        Selection.activeObject = townMap;

        return townMap;
    }    

    private void ApplyMapSize()
    {
        int currentX = _currentTownMapReference.xSize;
        int currentY = _currentTownMapReference.ySize;

        if (mapXSize == currentX && mapYSize == currentY)
        {
            return;
        }

        _currentTownMapReference.xSize = mapXSize;
        _currentTownMapReference.ySize = mapYSize;
    }

    private void CreateTileSettings()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Selected Tile No.");

        StringBuilder sb = new StringBuilder();
        foreach (int value in _selectedTiles)
        {
            sb.Append(value + 1).Append(" ");
        }
        selectedTilesString = sb.ToString();

        GUILayout.Label(selectedTilesString);
        GUILayout.EndHorizontal();

        buildable = EditorGUILayout.Toggle("Buildable", buildable);
        baseBuildingValStr = EditorGUILayout.TextField("Base Building", baseBuildingValStr);
        int.TryParse(baseBuildingValStr, out baseBuildingValue);
        tileTerrain = (TileTerrainType)EditorGUILayout.EnumPopup("Terrain", tileTerrain);

        if (GUILayout.Button("Tile Apply"))
        {
            ApplyTilesSettings();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Deselect Selected Tiles"))
        {
            _selectedTiles.Clear();
        }

        if (GUILayout.Button("Reset"))
        {
            foreach (int value in _selectedTiles)
            {
                _currentTownSoTiles.Remove(value);
            }
            _selectedTiles.Clear();
        }

        if (GUILayout.Button("Reset All"))
        {
            _currentTownSoTiles.Clear();
            _selectedTiles.Clear();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(25);

        if (GUILayout.Button("SAVE"))
        {
            if (string.IsNullOrEmpty(currentMapFileName.Trim()))
            {
                return;
            }

            string path = TOWNMAP_SO_DIR + townMapIdTextField + ".asset";

            if (currentMapFileName != townMapIdTextField)
            {
                _currentTownMapReference = AssetDatabase.LoadAssetAtPath<TownMapSO>(path);
            }

            if (_currentTownMapReference == null)
            {
                _currentTownMapReference = CreateNewTownMapSO(path);
            }

            FloodWindowFieldsToTownMapSO();

            EditorUtility.SetDirty(_currentTownMapReference);
        }
    }

    private void ApplyTilesSettings()
    {
        foreach (int id in _selectedTiles)
        {
            _currentTownSoTiles.TryGetValue(id, out var tile);

            if (tile == null)
            {
                tile = new TilePoco(id);
            }

            _currentTownSoTiles[id] = tile;

            tile.tileId = id;
            tile.name = (id + 1).ToString();
            tile.buildable = buildable;
            tile.terrain = tileTerrain;

            if (baseBuildingValue != 0)
            {
                tile.baseBuildingValue = baseBuildingValue;
            }
        }

        _selectedTiles.Clear();
    }

    private void CreateMapTilesVisual()
    {
        if (mapXSize == 0 || mapYSize == 0)
        {
            return;
        }

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        int tileId = 0;
        for (int i = 0; i < mapYSize; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < mapXSize; j++) 
            {
                SetUpTileColor(tileId);
                string tileDisPlayStr = GetTileDisplayString(tileId);
                float tileSizeWidth = Mathf.Clamp(position.width * 0.9f / mapXSize, 20f, 60f);
                if (GUILayout.Button(new GUIContent(tileDisPlayStr, (tileId + 1).ToString()), GUILayout.Height(60), GUILayout.Width(tileSizeWidth)))
                {
                    if (baseBuildingValue != 0)
                    {
                        _selectedTiles.Clear();
                        SetOrUnsetBaseBuilding(tileId);
                        GUILayout.EndHorizontal();
                        return;
                    }
                    SelectOrDeselectTile(tileId);
                }
                tileId++;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    private void SetOrUnsetBaseBuilding(int tileId)
    {
        _currentTownSoTiles.TryGetValue(tileId, out var tile);
        if (tile != null && tile.baseBuildingValue != 0)
        {
            UnsetBaseBuilding(tile.baseBuildingValue, tileId);
            return;
        }

        SetBaseBuilding(tileId);
    }

    private void UnsetBaseBuilding(int buildingId, int tileId)
    {
        _currentTownSoTiles.TryGetValue(tileId, out var selectedTile);
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(buildingId);
        List<int> tileUnderBuilding = TownMapHelper.GetAllTilesOfBuildingOccupancy(_currentTownSoTiles, mapXSize, mapYSize, buildingSize, selectedTile);
        for (int i = 0; i < tileUnderBuilding.Count; i++)
        {
            _currentTownSoTiles.TryGetValue(tileUnderBuilding[i], out var tile);
            if (tile == null)
            {
                continue;
            }

            tile.baseBuildingValue = 0;
            tile.baseBuildingRootTile = tile.tileId;
        }
    }

    private void SetBaseBuilding(int tileId)
    {
        Vector2 tileAxis = TownMapHelper.FromTileIdToAxis(mapXSize, mapYSize, tileId);
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(baseBuildingValue);
        bool canBuild = TownMapHelper.TopLeftCanBuildBuildingOnTile(_currentTownSoTiles, mapXSize, mapXSize, buildingSize, (int)tileAxis.x, (int)tileAxis.y);

        if (!canBuild)
        {
            EditorUtility.DisplayDialog("Cannot set building!", "One or more tiles are not [Buildable] or occupied by other building", "OK");
            return;
        }

        List<int> tileUnderBuildingFoundation = TownMapHelper.TopLeftGetAllTilesInArea(mapXSize, mapYSize, buildingSize, tileId);

        if (tileUnderBuildingFoundation.Count < buildingSize.x * buildingSize.y)
        {
            return;
        }

        for (int i = 0; i < tileUnderBuildingFoundation.Count; i++)
        {
            int id = tileUnderBuildingFoundation[i];
            _currentTownSoTiles.TryGetValue(id, out var tileUnderBuilding);
            if (tileUnderBuilding == null)
            {
                tileUnderBuilding = new TilePoco(id);
                _currentTownSoTiles[id] = tileUnderBuilding;
            }

            tileUnderBuilding.baseBuildingValue = baseBuildingValue;
            tileUnderBuilding.baseBuildingRootTile = tileId;
        }
    }

    private void SelectOrDeselectTile(int tileId)
    {
        if (!buildable)
        {
            _currentTownSoTiles.TryGetValue(tileId, out var tile);
            if (tile != null && tile.baseBuildingValue != 0)
            {
                EditorUtility.DisplayDialog("Cannot Select Tile!", "Please remove Building at this tile or toggle [Buildable] for setting this tile", "OK");
                return;
            }
        }

        if (_selectedTiles.Contains(tileId))
        {
            _selectedTiles.Remove(tileId);
            return;
        }

        Event e = Event.current;
        if (e.shift && _anchorTileId != -1)
        {
            MultipleTilesSelect(tileId);
            return;
        }

        _selectedTiles.Add(tileId);
        _anchorTileId = tileId;
    }

    private int _anchorTileId = -1;
    private void MultipleTilesSelect(int tileId)
    {
        List<int> selectedTiles = TownMapHelper.GetTileRectAreaDefineBy2Tiles(mapXSize, mapYSize, _anchorTileId, tileId);

        bool raiseMessage = false;
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            _currentTownSoTiles.TryGetValue(selectedTiles[i], out var tile);
            if (tile == null)
            {
                _selectedTiles.Add(selectedTiles[i]);
                continue;
            }

            if (!buildable && tile.baseBuildingValue != 0)
            {
                raiseMessage = true;
                continue;
            }

            _selectedTiles.Add(selectedTiles[i]);
        }

        if (raiseMessage)
        {
            EditorUtility.DisplayDialog("Warning!", "One or more tiles cannot be selected because they were occupied by building and current settings is Not [Buildable]", "OK");
        }
        _anchorTileId = -1;
    }

    private void SetUpTileColor(int tileId)
    {
        Color tileColor = GetTileColor(tileId);
        if (_selectedTiles.Contains(tileId))
        {
            float t = Mathf.Sin((4f * (float)EditorApplication.timeSinceStartup));

            t = MathUtils.Remap(-1f, 1f, 0f, 1f, t);

            GUI.backgroundColor = Color.Lerp(tileColor, Color.white, t);
            return;
        }

        GUI.backgroundColor = tileColor;
    }

    private Color GetTileColor(int tileId)
    {
        if (baseBuildingValue != 0)
        {
            return GetTileColorForBuildingSettings(tileId);
        }

        return GetTileColorForTerrainSettings(tileId);
    }

    private Color GetTileColorForTerrainSettings(int tileId)
    {
        Color tileColor = Color.green;

        _currentTownSoTiles.TryGetValue(tileId, out var tile);
        if (tile == null)
        {
            return tileColor;
        }

        if (!tile.buildable)
        {
            return Color.red;
        }

        if (tile.terrain == TileTerrainType.Mountain)
        {
            tileColor = new Color(0.4f, 0.28f, 0.29f);
        }

        if (tile.terrain == TileTerrainType.Desert)
        {
            tileColor = Color.yellow;
        }

        if (tile.terrain == TileTerrainType.River)
        {
            tileColor = new Color(0f, 1f, 0.83f);
        }

        if (tile.terrain == TileTerrainType.Sea)
        {
            tileColor = Color.blue;
        }

        if (tile.baseBuildingValue != 0)
        {
            tileColor -= Color.black * 0.6f;
        }

        return tileColor;
    }

    private HashSet<int> _tilesUnderBuildingFoundation = new HashSet<int>();
    private Color GetTileColorForBuildingSettings(int tileId)
    {
        if (_hoveringTile != (tileId + 1).ToString() && !_tilesUnderBuildingFoundation.Contains(tileId))
        {
            return GetTileColorForTerrainSettings(tileId);
        }

        if (!TownMapHelper.IsTileOccupiable(_currentTownSoTiles, tileId))
        {
            return Color.magenta;
        }

        return Color.cyan;
    }

    private string GetTileDisplayString(int tileId)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(tileId + 1).AppendLine();
        _currentTownSoTiles.TryGetValue(tileId, out var tile);
        if (tile == null)
        {
            return sb.Append("F").ToString();
        }

        sb.Append(tile.terrain.ToString()[0]);
        string buildingValueStr = tile.baseBuildingValue == 0 ? "" : $"|{tile.baseBuildingValue}";
        sb.Append(buildingValueStr);

        return sb.ToString();
    }

    private void FloodWindowFieldsToTownMapSO()
    {
        if (currentMapFileName == null)
        {
            return;
        }

        _currentTownMapReference.townMapId = townMapIdTextField;
        _currentTownMapReference.xSize = mapXSize;
        _currentTownMapReference.ySize = mapYSize;

        var tiles = _currentTownMapReference.tiles;
        tiles.Clear();

        foreach (var entry in _currentTownSoTiles)
        {
            tiles.Add(entry.Value);
        }
    }

    private void FloodTownMapSOToWindowFields()
    {
        townMapIdTextField = _currentTownMapReference.townMapId;

        mapXSize = _currentTownMapReference.xSize;
        sizeXStr = mapXSize.ToString();
        mapYSize = _currentTownMapReference.ySize;
        sizeYStr = mapYSize.ToString();

        var tiles = _currentTownMapReference.tiles;
        _currentTownSoTiles.Clear();

        for (int i = 0; i < tiles.Count; i++)
        {
            var tile = tiles[i];
            _currentTownSoTiles[tile.tileId] = tile;
        }
    }

    private string _hoveringTile = "";
    /// <summary>
    /// Unity currently does not provide a hover state for editor elements.
    /// Workaround by adding tooltip into editor elements then check the tooltip content for detecting which element is beging hovered.
    /// In this case, tooltip is added to tile button (tooltip content = tileId + 1)
    /// 
    /// ** GUI.tooltip always empty unless getting it inside OnGUI
    /// </summary>
    private void CheckTileHover()
    {
        _hoveringTile = GUI.tooltip;
    }

    string _lastHoveredTile;
    private void CheckTileUnderBuildingFoundation()
    {
        if (baseBuildingValue == 0)
        {
            return;
        }

        if (string.IsNullOrEmpty(_hoveringTile))
        {
            return;
        }

        if (_lastHoveredTile == _hoveringTile)
        {
            return;
        }

        _tilesUnderBuildingFoundation.Clear();

        _lastHoveredTile = _hoveringTile;

        int.TryParse(_hoveringTile, out int tileName);
        if (tileName == 0)
        {
            return;
        }

        int tileId = tileName - 1;
        Vector2 tileAxis = TownMapHelper.FromTileIdToAxis(mapXSize, mapYSize, tileId);
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(baseBuildingValue);

        // Go right then Go down
        for (int i = 0; i < buildingSize.y; i++)
        {
            for (int j = 0; j < buildingSize.x; j++)
            {
                int neighborTileId = TownMapHelper.FromAxisToTileId(mapXSize, mapYSize, (int)tileAxis.x + j, (int)tileAxis.y + i);

                _tilesUnderBuildingFoundation.Add(neighborTileId);
            }
        }
    }
}
