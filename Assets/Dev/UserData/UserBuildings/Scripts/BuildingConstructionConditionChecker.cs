using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConstructionConditionChecker : MonoSingleton<BuildingConstructionConditionChecker>
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _selectedBcmBuildingId;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private IntegerVariable _cameraFocusTile;

    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _openBcmThenFocus;
    [SerializeField]
    private GameEvent _openBuildingUpgradePopupCommand;

    public bool CheckConditions(List<BuildingCondition> conditions)
    {
        if (conditions == null || conditions.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            if (!CheckACondition(condition))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsBuildingMaximumCountReached(int buildingId)
    {
        int currentBuildingCount = _userBuildings.GetBuildingCount(buildingId);
        int maxBuildingCount = _userBuildings.GetMaxBuildingCount(buildingId);
        return maxBuildingCount <= currentBuildingCount;
    }

    public List<BuildingCondition> CheckAndReturnFailedConditions(List<BuildingCondition> conditions)
    {
        List<BuildingCondition> failedConditions = new();
        if (conditions == null || conditions.Count == 0)
        {
            return failedConditions;
        }

        for (int i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            if (!CheckACondition(condition))
            {
                failedConditions.Add(condition);
            }
        }

        return failedConditions;
    }

    public bool CheckACondition(BuildingCondition condition)
    {
        var conditionType = condition.conditionType;

        switch (conditionType)
        {
            case BuildingConditionType.BUILDING_LEVEL:
                return CheckBuildingLevelCondition(condition.conditionValues);
            default:
                return false;
        }
    }

    public void GoToCondition(BuildingCondition condition)
    {
        if (condition.conditionType == BuildingConditionType.BUILDING_LEVEL)
        {
            GoToBuildingLevelCondition(condition.conditionValues[0]);
            return;
        }

        // TODO RK other conditions type logic here
    }

    private void GoToBuildingLevelCondition(int buildingId)
    {
        int conditionBuilding = _userBuildings.GetLowestLevelIdleBuildingOfId(buildingId);
        bool hasBuilding = conditionBuilding != 0;
        if (!hasBuilding)
        {
            _selectedBcmBuildingId.Value = buildingId;
            _openBcmThenFocus.Raise();
            return;
        }

        _selectedBuildingObjId.Value = conditionBuilding;
        _cameraFocusTile.Value = _userBuildings.GetBuildingLocation(conditionBuilding);
        _openBuildingUpgradePopupCommand.Raise();
    }

    private bool CheckBuildingLevelCondition(List<int> conditionValues)
    {
        int buildingId = conditionValues[0];
        int buildingLevel = conditionValues[1];

        return _userBuildings.HasBuildingReachLevel(buildingId, buildingLevel);
    }
}
