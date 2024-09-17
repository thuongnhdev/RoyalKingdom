using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TownBuildingList", menuName = "Uniflow/Building/Town Building List")]
public class TownBaseBuildingSOList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<BaseBuildingPoco> _buildings;
    public List<BaseBuildingPoco> BuildingList => _buildings;

    // key = building Id
    private Dictionary<int, BaseBuildingPoco> _buildingMap = new Dictionary<int, BaseBuildingPoco>();
    private Dictionary<int, BaseBuildingPoco> BuildingMap
    {
        get
        {
            if (_buildingMap.Count != _buildings.Count)
            {
                _buildingMap.Clear();

                for (int i = 0; i < _buildings.Count; i++)
                {
                    var building = _buildings[i];
                    _buildingMap[building.id] = building;
                }
            }

            return _buildingMap;
        }
    }

    [Header("Reference - Read")]
    [SerializeField]
    private BuildingCommandList _buildingCommands;
    
    public void Init(List<BaseBuildingPoco> buildings)
    {
        if (buildings == null)
        {
            return;
        }

        _buildings.Clear();
        _buildingMap.Clear();

        for (int i = 0; i < buildings.Count; i++)
        {
            var buildingData = buildings[i];
            BaseBuildingPoco building = new();
            building.SetData(buildingData);

            _buildings.Add(building);
            _buildingMap.Add(building.id, building);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    public BaseBuildingPoco GetBaseBuilding(int id)
    {
        BuildingMap.TryGetValue(id, out var building);

        if (building == null)
        {
            Debug.LogError($"Invalid buildingId [{id}]");
        }

        return building;
    }

    public string GetBuildingName(int id)
    {
        var building = GetBaseBuilding(id);
        if (building == null)
        {
            return "";
        }

        return building.name;
    }

    public BuildingCategory GetBuildingCategory(int id)
    {
        return (BuildingCategory)(id / 10000 * 10000);
    }

    public Vector2 GetBuildingSize(int buildingId)
    {
        BuildingMap.TryGetValue(buildingId, out var building);

        if (building == null)
        {
            Debug.LogError($"Invalid buildingId [{buildingId}]");
            return Vector2.zero;
        }

        return building.size;
    }

    public List<Vector2> GetBuildingDoorsDirection(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return null;
        }

        return building.doorsDirection;
    }

    public int GetBuldingMaxLevel(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return 0;
        }

        return building.maxLevel;
    }

    public bool IsBuildingBreakable(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return true;
        }

        return !building.unbreakable;
    }

    public BuildingCommandKey GetBuildingCommand1Key(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return BuildingCommandKey.None;
        }

        return building.command1;
    }

    public BuildingCommandKey GetBuildingCommand2Key(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return BuildingCommandKey.None;
        }

        return building.command2;
    }

    public void ExecuteBuildingCommand1(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return;
        }

        _buildingCommands.ExecuteBuildingCommand(building.command1);
    }

    public void ExecuteBuildingCommand2(int buildingId)
    {
        var building = GetBaseBuilding(buildingId);
        if (building == null)
        {
            return;
        }

        _buildingCommands.ExecuteBuildingCommand(building.command2);
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    public const string EDITORONLY_BUILDINGDATAVERSION_KEY = "BuildingDataVersion";
    public delegate void EditorOnly_BuildingDataChangedDel();
    public static event EditorOnly_BuildingDataChangedDel EditorOnly_OnBuildingDataChanged;
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        var buildingSheet = parsedSheets[0];
        if (buildingSheet.Count == 0)
        {
            return;
        }

        _buildings.Clear();
        for (int i = 0; i < buildingSheet.Count; i++)
        {
            var row = buildingSheet[i];

            row.GetValue("Key", out int buildingId);
            var buildingInfo = new BaseBuildingPoco();
            buildingInfo.id = buildingId;

            row.GetValue("Name", out string buildingName);
            buildingInfo.name = buildingName;

            row.GetValue("BuildingSize", out int buildingSize);
            buildingInfo.size.x = buildingSize / 10;
            buildingInfo.size.y = buildingSize % 10;

            row.GetValue("MaxLevel", out int maxLevel);
            buildingInfo.maxLevel = maxLevel;

            row.GetValue("Unbreakable", out int unbreakable);
            buildingInfo.unbreakable = unbreakable == 1;

            row.GetValue("CommandKey_1", out int command1);
            buildingInfo.command1 = (BuildingCommandKey)command1;
            row.GetValue("CommandKey_2", out int command2);
            buildingInfo.command2 = (BuildingCommandKey)command2;

            buildingInfo.destructionTimeFactor = 1f;

            _buildings.Add(buildingInfo);
        }

        EditorUtility.SetDirty(this);
        EditorPrefs.SetString(EDITORONLY_BUILDINGDATAVERSION_KEY, System.DateTime.Now.ToString());

        EditorOnly_OnBuildingDataChanged?.Invoke();
    }
#endif
}
