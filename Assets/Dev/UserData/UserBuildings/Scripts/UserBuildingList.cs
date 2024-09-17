using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserBuildingList", menuName = "Uniflow/User/User Buildings List")]
public class UserBuildingList : ScriptableObject
{
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;
    [SerializeField]
    private BuildingMaxCountList _buildingMaxCountInfos;

    [SerializeField]
    private List<UserBuilding> _buildingList;
    public List<UserBuilding> BuildingList => _buildingList;

    private Dictionary<int, UserBuilding> _buildingDict = new Dictionary<int, UserBuilding>();
    private Dictionary<int, UserBuilding> BuildingDict
    {
        get
        {
            if (_buildingDict.Count != _buildingList.Count)
            {
                _buildingDict.Clear();
                for (int i = 0; i < _buildingList.Count; i++)
                {
                    var building = _buildingList[i];
                    _buildingDict[building.buildingObjectId] = building;
                }
            }

            return _buildingDict;
        }
    }

    public void Init(List<UserBuilding> userBuildingsData)
    {
        if (userBuildingsData == null || userBuildingsData.Count == 0)
        {
            return;
        }

        _buildingList.Clear();

        for (int i = 0; i < userBuildingsData.Count; i++)
        {
            var buildingData = userBuildingsData[i];

            _buildingList.Add(buildingData);
            _buildingDict[buildingData.buildingObjectId] = buildingData;
        }

        Logger.Log("Initilized User Buildings!");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public UserBuilding GetBuilding(int buildingObjectId)
    {
        BuildingDict.TryGetValue(buildingObjectId, out var building);
        if (building == null)
        {
            Logger.LogError($"Invalid buildingObjectId [{buildingObjectId}]");
        }

        return building;
    }

    /// <summary>
    /// Slower than getting by buildingObjectId
    /// </summary>
    public UserBuilding GetBuildingByLocation(int locationTileId)
    {
        for (int i = 0; i < _buildingList.Count; i++)
        {
            var building = _buildingList[i];
            if (building.locationTileId == locationTileId)
            {
                return building;
            }
        }

        Logger.LogError($"There is no building at tile [{locationTileId}]");
        return null;
    }

    public int GetBuildingId(int buildingObjectId)
    {
        var building = GetBuilding(buildingObjectId);
        if (building == null)
        {
            return 0;
        }

        return building.buildingId;
    }

    public int GetBuildingLocation(int buildingObjectId)
    {
        BuildingDict.TryGetValue(buildingObjectId, out var building);
        if (building == null)
        {
            Logger.LogError($"Invalid buildingObjectId [{buildingObjectId}]");
            return -1;
        }

        return building.locationTileId;
    }

    public int GetBuildingLevel(int buildingObjectId)
    {
        var building = GetBuilding(buildingObjectId);
        if (building == null)
        {
            return 0;
        }

        return building.buildingLevel;
    }

    public int GetVassalInCharge(int buildingObjectId)
    {
        var building = GetBuilding(buildingObjectId);
        if (building == null)
        {
            return 0;
        }

        return building.vassalInCharge;
    }

    public int GetBuildingCount(int buildingId)
    {
        return GetAllBuildingsOfId(buildingId).Count;
    }

    public List<int> GetAllBuildingsOfId(int buildingId)
    {
        List<int> targetBuildings = new();
        for (int i = 0; i < _buildingList.Count; i++)
        {
            var building = _buildingList[i];
            if (building.buildingId != buildingId || 
                building.status == BuildingStatus.None || 
                building.status == BuildingStatus.Destructed)
            {
                continue;
            }

            targetBuildings.Add(building.buildingObjectId);
        }

        return targetBuildings;
    }

    public int GetHighestLevelIdleBuildingOfId(int buildingId)
    {
        int highestLevelBuilding = 0;
        int highestLevel = 0;
        for (int i = 0; i < _buildingList.Count; i++)
        {
            var building = _buildingList[i];
            if (building.buildingId != buildingId)
            {
                continue;
            }

            if (building.buildingLevel <= highestLevel)
            {
                continue;
            }

            if (building.status != BuildingStatus.Idle)
            {
                continue;
            }

            highestLevel = building.buildingLevel;
            highestLevelBuilding = building.buildingObjectId;
        }

        return highestLevelBuilding;
    }

    public int GetLowestLevelIdleBuildingOfId(int buildingId)
    {
        int lowestLevelBuilding = 0;
        int lowestLevel = int.MaxValue;
        for (int i = 0; i < _buildingList.Count; i++)
        {
            var building = _buildingList[i];
            if (building.buildingId != buildingId)
            {
                continue;
            }
            if (lowestLevel <= building.buildingLevel)
            {
                continue;
            }
            if (building.status != BuildingStatus.Idle)
            {
                continue;
            }

            lowestLevel = building.buildingLevel;
            lowestLevelBuilding = building.buildingObjectId;
        }

        return lowestLevelBuilding;
    }

    public List<int> GetAllWarehouses()
    {
        List<int> warehouseObjIds = new();
        for (int i = 0; i < _buildingList.Count; i++)
        {
            var building = _buildingList[i];
            if (building.status == BuildingStatus.None || building.status == BuildingStatus.Destructed)
            {
                continue;
            }

            int capacity = _buildingUpgradeInfos.GetCapacity(building.buildingId, building.buildingLevel);
            if (capacity == 0)
            {
                continue;
            }

            warehouseObjIds.Add(building.buildingObjectId);
        }

        return warehouseObjIds;
    }

    public BuildingStatus GetBuildingStatus(int buildingObjectId)
    {
        BuildingDict.TryGetValue(buildingObjectId, out var building);
        if (building == null)
        {
            Logger.LogError($"Invalid buildingObjectId [{buildingObjectId}]");
            return BuildingStatus.None;
        }

        return building.status;
    }

    public List<ResourcePoco> GetConsumedResoucesForConstruction(int buildingObjectId)
    {
        var userBuilding = GetBuilding(buildingObjectId);
        if (userBuilding == null)
        {
            return null;
        }

        if (userBuilding.status != BuildingStatus.WaitForConstructingResource &&
            userBuilding.status != BuildingStatus.OnConstruction &&
            userBuilding.status != BuildingStatus.WaitingForUpgradeResource &&
            userBuilding.status != BuildingStatus.Upgrading)
        {
            return userBuilding.ConsumeResources;
        }

        return userBuilding.constructionMaterial;
    }

    public void UpdateBuilding(UserBuilding buildingInfo)
    {
        if (!BuildingDict.ContainsKey(buildingInfo.buildingObjectId))
        {
            _buildingList.Add(buildingInfo);
        }

        _buildingDict[buildingInfo.buildingObjectId] = buildingInfo;
    }

    public bool HasBuildingReachLevel(int buildingId, int compareLevel)
    {
        for (int i = 0; i < _buildingList.Count; i++)
        {
            var building = _buildingList[i];
            if (building.buildingId != buildingId)
            {
                continue;
            }

            if (compareLevel <= building.buildingLevel)
            {
                return true;
            }
        }

        return false;
    }

    public int GetMaxBuildingCount(int buildingId)
    {
        List<BuildingMaxCountInfo> maxCountInfos = _buildingMaxCountInfos.GetMaxCountInfo(buildingId);
        if (maxCountInfos == null || maxCountInfos.Count == 0)
        {
            return int.MaxValue;
        }
        for (int i = maxCountInfos.Count - 1; 0 < i; i--)
        {
            var maxCountInfo = maxCountInfos[i];
            bool pass = BuildingConstructionConditionChecker.Instance.CheckConditions(maxCountInfo.conditions);
            if (!pass)
            {
                continue;
            }

            return maxCountInfo.maxCount;
        }

        return maxCountInfos[0].maxCount;
    }

    public bool CanAddBuilding(int buildingId)
    {
        int maxCount = GetMaxBuildingCount(buildingId);
        return GetBuildingCount(buildingId) < maxCount;
    }

    public void ClearData()
    {
        _buildingList.Clear();
        _buildingDict.Clear();
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}

