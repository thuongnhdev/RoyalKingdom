using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingCancelConstructionConfirmPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;
    [SerializeField]
    private UserBuildingList _userBuildingList;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjectId;

    [Header("Config")]
    [SerializeField]
    private GameObject _itemIconAndLabelPrefab;
    [SerializeField]
    private RectTransform _refundInfoLayoutGroup;
    [SerializeField]
    private TMP_Text _timeText;

    [Header("Config")]
    [SerializeField]
    private CancelType _cancelType;
    [SerializeField]
    private TMP_Text _buildingNameText;

    private List<GameObject> _itemUiObjs = new();

    private enum CancelType
    {
        Construction,
        Upgrade
    }

    public void SetUpInfo()
    {
        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjectId.Value);
        if (userBuilding == null)
        {
            return;
        }
        int buildingLevel = Mathf.Clamp(userBuilding.buildingLevel, 1, userBuilding.buildingLevel);

        var buildingUpgrade = _buildingUpgradeInfos.GetBuildingUpgradeInfo(userBuilding.buildingId, buildingLevel);
        _buildingNameText.text = buildingUpgrade.name;
        float timeCost = _cancelType == CancelType.Construction ? _buildingUpgradeInfos.GetTimeCost(userBuilding.buildingId, buildingLevel) : 0f;
        _timeText.text = TimeUtils.FormatTime(timeCost * BuildingHelper.GetDestructionTimeCostRate(userBuilding.status));

        List<ResourcePoco> consumedResources = _userBuildingList.GetConsumedResoucesForConstruction(userBuilding.buildingObjectId);

        float refundRate = _cancelType == CancelType.Construction ?
            BuildingHelper.GetDestructionRefundRate(userBuilding.status) :
            BuildingHelper.GetCancelUpgradeRefundRate(userBuilding.status);

        List<ResourcePoco> refundResources = ItemHelper.MultiplyResources(consumedResources, refundRate);

        for (int i = 0; i < consumedResources.Count; i++)
        {
            var itemUiObj = Instantiate(_itemIconAndLabelPrefab, _refundInfoLayoutGroup);
            itemUiObj.GetComponent<ResourceIconAndLabel>().SetUp(refundResources[i].itemId, $"{refundResources[i].itemCount}/{consumedResources[i].itemCount}");
            itemUiObj.SetActive(true);

            _itemUiObjs.Add(itemUiObj);
        }
    }

    public void In_ResetRefundInfo()
    {
        for (int i = _itemUiObjs.Count - 1; 0 <= i; i--)
        {
            Destroy(_itemUiObjs[i]);
            _itemUiObjs.RemoveAt(_itemUiObjs.Count - 1);
        }
    }
}
