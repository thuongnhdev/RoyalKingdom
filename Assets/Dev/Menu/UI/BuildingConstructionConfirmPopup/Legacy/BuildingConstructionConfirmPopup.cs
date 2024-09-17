using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingConstructionConfirmPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingList;
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssetsList;
    [SerializeField]
    private UserItemStorage _userItemList;
    [SerializeField]
    private IntegerVariable _selectedBuildingId;

    [Header("Config")]
    [SerializeField]
    private GameObject _costItemPrefab;
    [SerializeField]
    private TMP_Text _buildingName;
    [SerializeField]
    private Image _buildingImage;
    [SerializeField]
    private TMP_Text _buildingDes;
    [SerializeField]
    private TMP_Text _manPowerText;
    [SerializeField]
    private TMP_Text _timeText;
    [SerializeField]
    private RectTransform _costGroupRect;

    private List<GameObject> _costItems = new();

    public void SetUpInfo()
    {
        var buildingInfo = _buildingList.GetBaseBuilding(_selectedBuildingId.Value);
        if (buildingInfo == null)
        {
            return;
        }

        _buildingName.text = buildingInfo.name;
        _buildingDes.text = buildingInfo.description;

        List<ResourcePoco> cost = _buildingUpgradeInfos.GetUpgradeCost(_selectedBuildingId.Value, 1);
        for (int i = 0; i < cost.Count; i++)
        {
            int userItemCount = _userItemList.GetItemCount(cost[i].itemId);
            var resource = cost[i];
            var costItemObj = Instantiate(_costItemPrefab, _costGroupRect);
            costItemObj.GetComponent<ResourceIconAndLabel>().SetUp(resource.itemId, $"{userItemCount}/{resource.itemCount}");
            _costItems.Add(costItemObj);
        }

        _timeText.text = TimeUtils.FormatTime(_buildingUpgradeInfos.GetTimeCost(_selectedBuildingId.Value, 1));

        _buildingImage.sprite = _buildingAssetsList.GetBuildingSprite(_selectedBuildingId.Value);
    }

    public void ResetCostInfo()
    {
        for (int i = _costItems.Count - 1; 0 <= i; i--)
        {
            Destroy(_costItems[i]);
            _costItems.RemoveAt(_costItems.Count - 1);
        }
    }
}
