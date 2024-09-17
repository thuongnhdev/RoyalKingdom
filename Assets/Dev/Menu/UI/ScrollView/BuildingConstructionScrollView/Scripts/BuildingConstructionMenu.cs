using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingConstructionMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingListSO;
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;
    [SerializeField]
    private UserItemStorage _userItemList;

    [Header("Reference - Read/Write")]
    [SerializeField]
    private IntegerVariable _selectedBuilding;

    [SerializeField]
    private UnityEvent _onOpenBeforeFocus;
    [SerializeField]
    private UnityEvent _onBuildButtonClickSuccess;
    [SerializeField]
    private UnityEvent _onBuildButtonClickFailed;

    [Header("Config")]
    [SerializeField]
    private GameObject _itemIconAndLabelPrefab;
    [SerializeField]
    private GameObject _conditionIconAndLabelPrefab;
    [SerializeField]
    private RectTransform _requirementLayoutGroup;
    [SerializeField]
    private EnhancedScroller _scroller;
    [SerializeField]
    private BuildingConstructionScrollController _scrollerController;
    [SerializeField]
    private List<TabAndCatergoryPair> _tabButtons;

    [Header("Inspec")]
    [SerializeField]
    private BuildingCategory _recentlyTab = BuildingCategory.ForTown;

    [Header("Cheating Area")]
    [SerializeField]
    private BoolVariable _allowCheat;

    private List<GameObject> _buildingReqUiObjs = new();

    public void In_ShowRecentlyTab()
    {
        In_DisplayBuilding((int)_recentlyTab);
        In_RemoveAllCostInfo();
    }
    public void In_RemoveAllCostInfo()
    {
        for (int i = _buildingReqUiObjs.Count - 1; 0 <= i; i--)
        {
            Destroy(_buildingReqUiObjs[i]);
            _buildingReqUiObjs.RemoveAt(_buildingReqUiObjs.Count - 1);
        }
    }

    public void In_ResetAllViewValue()
    {
        _selectedBuilding.Value = 0;
    }

    public void In_DisplayBuilding(int category)
    {
        BuildingCategory tab = (BuildingCategory)category;
        _scrollerController.ChangeDisplayData(GetAllBuildingsOfCategory(tab));
        SwichTab(tab);
    }

    public void In_OpenThenFocus()
    {
        UniTask_FocusAfterOpen().Forget();
    }

    public void In_CheckBuildConditionAndAction()
    {
#if UNITY_EDITOR
        if (_allowCheat.Value)
        {
            _onBuildButtonClickSuccess.Invoke();
            return;
        }
#endif
        var checker = BuildingConstructionConditionChecker.Instance;
        bool reachMaxNumber = checker.IsBuildingMaximumCountReached(_selectedBuilding.Value);
        var buildConditions = _buildingUpgradeInfos.GetBuildingUpgradeConditions(_selectedBuilding.Value, 1);
        bool available = BuildingConstructionConditionChecker.Instance.CheckConditions(buildConditions);

        if (!available || reachMaxNumber)
        {
            _onBuildButtonClickFailed.Invoke();
            return;
        }

        _onBuildButtonClickSuccess.Invoke();
    }

    public RectTransform GetBuildingCellPosition(int buildingId)
    {
        BuildingCategory category = _buildingListSO.GetBuildingCategory(buildingId);
        List<BaseBuildingPoco> buildings = GetAllBuildingsOfCategory(category);

        int dataIndex = 0;
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].id == buildingId)
            {
                dataIndex = i;
                break;
            }
        }

        var buildingCell = _scroller.GetCellViewAtDataIndex(dataIndex);
        return buildingCell.gameObject.GetComponent<RectTransform>();
    }

    public void FocusBuilding(int selectedBuilding)
    {
        if (selectedBuilding == 0)
        {
            return;
        }

        int dataIndex = -1;
        BuildingCategory category = _buildingListSO.GetBuildingCategory(selectedBuilding);
        In_DisplayBuilding((int)category);

        List<BaseBuildingPoco> buildings = GetAllBuildingsOfCategory(category);
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].id == selectedBuilding)
            {
                dataIndex = i;
                break;
            }
        }

        if (dataIndex == -1)
        {
            return;
        }


        _scroller.JumpToDataIndex(dataIndex, tweenType: EnhancedScroller.TweenType.easeOutQuad, tweenTime: 0.3f, jumpComplete: () =>
        {
            _selectedBuilding.Value = selectedBuilding;
        });
    }

    private async UniTaskVoid UniTask_FocusAfterOpen()
    {
        int focusBuilding = _selectedBuilding.Value; // Keep this value because _selecetedBuilding value is reset when opening BCM
        _recentlyTab = _buildingListSO.GetBuildingCategory(focusBuilding);
        _onOpenBeforeFocus.Invoke();

        await UniTask.Delay(System.TimeSpan.FromSeconds(0.4f));
        FocusBuilding(focusBuilding);
    }

    private List<BaseBuildingPoco> GetAllBuildingsOfCategory(BuildingCategory targetCategory)
    {
        List<BaseBuildingPoco> allBuildings = _buildingListSO.BuildingList;
        List<BaseBuildingPoco> targetBuildings = new(allBuildings.Count);
        List<BaseBuildingPoco> availables = new();
        List<BaseBuildingPoco> unavailables = new();
        List<BaseBuildingPoco> limited = new();
        for (int i = 0; i < allBuildings.Count; i++)
        {
            var building = allBuildings[i];
            if (building.category != targetCategory)
            {
                continue;
            }

            var buildingConditions = _buildingUpgradeInfos.GetBuildingUpgradeConditions(building.id, 1);
            var checker = BuildingConstructionConditionChecker.Instance;
            bool conditionPass = checker.CheckConditions(buildingConditions);
            bool maximumCountReach = checker.IsBuildingMaximumCountReached(building.id);
            if (maximumCountReach)
            {
                limited.Add(building);
                continue;
            }

            if (!conditionPass)
            {
                unavailables.Add(building);
                continue;
            }

            availables.Add(building);
        }

        targetBuildings.AddRange(availables);
        targetBuildings.AddRange(unavailables);
        targetBuildings.AddRange(limited);
        return targetBuildings;
    }

    private void SwichTab(BuildingCategory tab)
    {
        _recentlyTab = tab;
        In_RemoveAllCostInfo();

        _selectedBuilding.Value = 0;

        SetTabStatus();
    }

    private void SetTabStatus()
    {
        for (int i = 0; i < _tabButtons.Count; i++)
        {
            var tabCatPair = _tabButtons[i];
            if (tabCatPair.category == _recentlyTab)
            {
                tabCatPair.tab.interactable = false;
                continue;
            }

            tabCatPair.tab.interactable = true;
        }
    }

    private void UpdateCostInfo(int buildingId)
    {
        In_RemoveAllCostInfo();

        if (_selectedBuilding.Value == 0)
        {
            return;
        }

        ShowBuildingCost(buildingId);
    }

    private void ShowBuildingCost(int buildingId)
    {
        List<ResourcePoco> cost = _buildingUpgradeInfos.GetUpgradeCost(buildingId, 1);

        for (int i = 0; i < cost.Count; i++)
        {
            int userItemCount = _userItemList.GetItemCount(cost[i].itemId);
            string costText = ItemHelper.FormatItemCostColor(userItemCount, cost[i].itemCount);

            var costObj = Instantiate(_itemIconAndLabelPrefab, _requirementLayoutGroup);
            costObj.GetComponent<ResourceIconAndLabel>().SetUp(cost[i].itemId, costText);
            costObj.SetActive(true);

            _buildingReqUiObjs.Add(costObj);
        }
    }

    private void ShowBuildingConditions(List<BuildingCondition> conditions)
    {
        // TODO RK Add sprite for other conditions
        for (int i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            var conditionUiObj = Instantiate(_conditionIconAndLabelPrefab, _requirementLayoutGroup);
            conditionUiObj.GetComponent<ConstructionConditionIconAndLabel>().SetUp(condition);
            conditionUiObj.SetActive(true);

            _buildingReqUiObjs.Add(conditionUiObj);
        }
    }

    private void OnEnable()
    {
        _selectedBuilding.OnValueChange += UpdateCostInfo;
    }

    private void OnDisable()
    {
        _selectedBuilding.OnValueChange -= UpdateCostInfo;
    }

    [System.Serializable]
    private struct TabAndCatergoryPair
    {
        public Button tab;
        public BuildingCategory category;
    }
}
