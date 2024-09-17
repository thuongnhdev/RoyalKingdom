using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUpgradeMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingList;
    [SerializeField]
    private TownBuildingAssetsInfoList _bulidingsAssets;
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;
    [SerializeField]
    private UserItemStorage _userItemList;
    [SerializeField]
    private UserBuildingList _userBuildingList;
    [SerializeField]
    private IntegerVariable _selectedBuildingObj;

    [Header("Config")]
    [SerializeField]
    private Button _upgradeButton;
    [SerializeField]
    private Image _buildingImg;
    [SerializeField]
    private TMP_Text _buildingNameText;
    [SerializeField]
    private TMP_Text _currentLevelText;
    [SerializeField]
    private TMP_Text _nextLevelText;
    [SerializeField]
    private GameObject _itemIconAndLabelPrefab;
    [SerializeField]
    private ConditionButton _conditionButtonTemplate;
    [SerializeField]
    private RectTransform _costLayoutGroup;

    private List<GameObject> _costAndConditionObjs = new();

    public void In_ShowUpgradeInfo()
    {
        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObj.Value);
        if (userBuilding == null)
        {
            return;
        }

        var buildingInfo = _buildingList.GetBaseBuilding(userBuilding.buildingId);
        if (buildingInfo == null)
        {
            return;
        }

        if (userBuilding.buildingLevel == buildingInfo.maxLevel)
        {
            return;
        }

        _buildingImg.overrideSprite = _bulidingsAssets.GetBuildingSprite(buildingInfo.id);
        _buildingNameText.text = buildingInfo.name;
        _currentLevelText.text = $"Level {userBuilding.buildingLevel}";
        _nextLevelText.text = $"{userBuilding.buildingLevel + 1}";

        int buildingId = userBuilding.buildingId;
        int nextLevel = userBuilding.buildingLevel + 1;

        var upgradeConditions = _buildingUpgradeInfos.GetBuildingUpgradeConditions(buildingId, nextLevel);
        var failedCondition = BuildingConstructionConditionChecker.Instance.CheckAndReturnFailedConditions(upgradeConditions);

        _upgradeButton.enabled = true;
        List<ResourcePoco> costs = _buildingUpgradeInfos.GetUpgradeCost(buildingId, nextLevel);
        SetUpCost(costs);

        if (failedCondition.Count != 0)
        {
            SetUpConditions(failedCondition);
            _upgradeButton.enabled = false;
        }
    }

    private void SetUpConditions(List<BuildingCondition> conditions)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            var conditionObj = Instantiate(_conditionButtonTemplate, _costLayoutGroup);
            _costAndConditionObjs.Add(conditionObj.gameObject);
            int buildingId = condition.conditionValues[0];
            string conditionText = $"{_buildingList.GetBuildingName(buildingId)} reaches level {condition.conditionValues[1]}";
            conditionObj.SetUp(conditionText, () =>
            {
                BuildingConstructionConditionChecker.Instance.GoToCondition(condition);
            });
            conditionObj.gameObject.SetActive(true);
        }
    }

    private void SetUpCost(List<ResourcePoco> costs)
    {
        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            int userItemCount = _userItemList.GetItemCount(cost.itemId);
            var itemUIObj = Instantiate(_itemIconAndLabelPrefab, _costLayoutGroup);
            itemUIObj.GetComponent<ResourceIconAndLabel>().SetUp(cost.itemId, ItemHelper.FormatItemCostColor(userItemCount, cost.itemCount));

            itemUIObj.SetActive(true);
            _costAndConditionObjs.Add(itemUIObj);
        }
    }

    public void In_ResetCostInfo()
    {
        for (int i = _costAndConditionObjs.Count - 1; 0 <= i; i--)
        {
            Destroy(_costAndConditionObjs[i]);
            _costAndConditionObjs.RemoveAt(_costAndConditionObjs.Count - 1);
        }
    }
}
