using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class BuildingConstructionScrollCell : BaseCustomCellView
{
    [Header("Reference - Read/Write")]
    [SerializeField]
    private IntegerVariable _selectedBuildingId;

    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssets;
    [SerializeField]
    private BuildingUpgradeInfoList _upgrades;
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Config")]
    [SerializeField]
    private Button _goToConditionButton;
    [SerializeField]
    private Color _conditionColor;
    [SerializeField]
    private Image _buildingSprite;
    [SerializeField]
    private Image _blockImage;
    [SerializeField]
    private Image _lockIcon;
    [SerializeField]
    private List<Image> _selectedGroup;
    [SerializeField]
    private TMP_Text _buildingName;
    [SerializeField]
    private TMP_Text _buildingDes;
    [SerializeField]
    private TMP_Text _buildingCountLimitText;
    [SerializeField]
    private TMP_Text _conditionText;
    [SerializeField]
    private TMP_Text _maxCountText;

    [Header("Inspec")]
    [SerializeField]
    private int _buildingId;
    [SerializeField]
    private bool _selected = false;

    public override void SetUp(object data)
    {
        _selected = false;

        var buildingData = (BaseBuildingPoco)data;
        _buildingName.text = buildingData.name;
        _buildingId = buildingData.id;
        ActiveSelectedGroup(false);

        bool reachMaxCount = CheckAndSetUpBuildingCount(buildingData.id);

        var conditions = _upgrades.GetBuildingUpgradeConditions(buildingData.id, 1);
        var failedConditions = BuildingConstructionConditionChecker.Instance.CheckAndReturnFailedConditions(conditions);

        SetUpBuildingLock(reachMaxCount, failedConditions);
    }

    public void In_OnBuildingSelected()
    {
        if (_selected)
        {
            _selectedBuildingId.Value = 0;
            _selected = false;

            ActiveSelectedGroup(false);
            return;
        }

        _selectedBuildingId.Value = _buildingId;
        _selected = true;

        ActiveSelectedGroup(true);
    }

    private void SetCellStatus(int selectedBuildingId)
    {
        if (selectedBuildingId == 0)
        {
            return;
        }

        bool isSelected = selectedBuildingId == _buildingId;
        _selected = isSelected;
        ActiveSelectedGroup(isSelected);
    }

    private void ActiveSelectedGroup(bool active)
    {
        for (int i = 0; i < _selectedGroup.Count; i++)
        {
            _selectedGroup[i].enabled = active;
        }
    }

    private bool CheckAndSetUpBuildingCount(int buildingId)
    {
        int currentBuildingCount = _userBuildings.GetBuildingCount(buildingId);
        int maxBuildingCount = _userBuildings.GetMaxBuildingCount(buildingId);
        _buildingCountLimitText.text = $"{currentBuildingCount}/{maxBuildingCount}";

        if (maxBuildingCount == int.MaxValue)
        {
            _buildingCountLimitText.text = "";
            return false;
        }

        return maxBuildingCount <= currentBuildingCount;
    }

    private StringBuilder sb = new();
    private void SetUpBuildingLock(bool reachMaxCount, List<BuildingCondition> failedConditions)
    {
        sb.Clear();

        bool available = failedConditions.Count == 0;
        _lockIcon.enabled = !available;
        _goToConditionButton.gameObject.SetActive(!available);
        if (!available)
        {
            for (int i = 0; i < failedConditions.Count; i++)
            {
                var failCondition = failedConditions[i];
                _conditionText.color = _conditionColor;
                sb.AppendLine($"{_buildings.GetBuildingName(failCondition.conditionValues[0])} reaches level {failCondition.conditionValues[1]}");
            }

            _conditionText.text = sb.ToString();
            _goToConditionButton.onClick.AddListener(() =>
            {
                BuildingConstructionConditionChecker.Instance.GoToCondition(failedConditions[0]);
            });
        }

        _maxCountText.enabled = reachMaxCount;

        if (!available || reachMaxCount)
        {
            _buildingSprite.overrideSprite = _buildingAssets.GetBuildingGreyscaleSprite(_buildingId);
            _blockImage.enabled = true;
            return;
        }
        _buildingSprite.overrideSprite = _buildingAssets.GetBuildingSprite(_buildingId);
        _blockImage.enabled = false;
    }

    private void OnEnable()
    {
        SetCellStatus(_selectedBuildingId.Value);
        _selectedBuildingId.OnValueChange += SetCellStatus;
    }

    private void OnDisable()
    {
        _selectedBuildingId.OnValueChange -= SetCellStatus;
    }
}
