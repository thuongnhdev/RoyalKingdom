using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private ItemGroupList _itemGroups;
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgrades;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _askBcmFocusToBuilding;
    [SerializeField]
    private GameEvent _openUpgradePopup;

    [Header("Configs")]
    [SerializeField]
    private Button _plusButton;
    [SerializeField]
    private InventoryMenuScrollController _scrollController;
    [SerializeField]
    private TMP_Text _currentLoaded;
    [SerializeField]
    private TMP_Text _capacity;
    [SerializeField]
    private Transform _filterTabsContent;
    [SerializeField]
    private ButtonDisplaySwitcher _filterTabTemplate;

    [Header("Inspec")]
    [SerializeField]
    private List<Button> _filterTabs = new();

    private Vector3 _originFilterTabsContentPos;
    private int _currentFilterWarehouse;
    private UIPanel _panel;

    public void Refresh()
    {
        if (_panel.Status == UIPanel.PanelStatus.Closed || _panel.Status == UIPanel.PanelStatus.IsClosing)
        {
            return;
        }

        int buildingId = 0;
        if (_selectedBuildingObjId.Value != 0)
        {
            buildingId = _userBuildings.GetBuildingId(_selectedBuildingObjId.Value);
        }

        List<ResourcePoco> items = _userItems.GetAllItems();
        Dictionary<int, ItemWithRandomOption> randomOptionItems = _userItems.RandomOptionsDict;
        _scrollController.PrepareDataAndShowAll(items, randomOptionItems);

        ApplyFilter(buildingId);
        _filterTabsContent.transform.localPosition = _originFilterTabsContentPos;
    }

    public void ApplyFilter(int filterWarehouse)
    {
        _currentFilterWarehouse = filterWarehouse;
        _plusButton.interactable = filterWarehouse != 0;
        _scrollController.ApplyFilter(filterWarehouse);

        _currentLoaded.text = TextUtils.FormatNumber(_userItems.GetCountBasedOnContainWarehouse(filterWarehouse));

        List<int> warehouses;
        if (filterWarehouse == 0)
        {
            warehouses = _userBuildings.GetAllWarehouses();
        }
        else
        {
            warehouses = _userBuildings.GetAllBuildingsOfId(filterWarehouse);
        }

        int capacity = 0;
        for (int i = 0; i < warehouses.Count; i++)
        {
            int warehouseObjId = warehouses[i];
            int warehouseLevel = _userBuildings.GetBuildingLevel(warehouseObjId);
            int warehouseId = _userBuildings.GetBuildingId(warehouseObjId);
            capacity += _buildingUpgrades.GetCapacity(warehouseId, warehouseLevel);
        }
        _capacity.text = TextUtils.FormatNumber(capacity);

        DisplayFilterTabs(filterWarehouse);
    }

    public void IncreaseCapacity()
    {
        List<int> userWarehouses = _userBuildings.GetAllBuildingsOfId(_currentFilterWarehouse);
        int maxWarehouseCount = _userBuildings.GetMaxBuildingCount(_currentFilterWarehouse);
        if (userWarehouses.Count < maxWarehouseCount)
        {
            BuildingCommunicator.Instance.OpenBcmAndFocusBuilding(_currentFilterWarehouse);
            return;
        }

        int lowestLevel = _buildings.GetBuldingMaxLevel(_currentFilterWarehouse);
        int warehouseToUpgrade = 0;
        for (int i = 0; i < userWarehouses.Count; i++)
        {
            int ojbId = userWarehouses[i];
            int level = _userBuildings.GetBuildingLevel(ojbId);
            if (level < lowestLevel)
            {
                lowestLevel = level;
                warehouseToUpgrade = ojbId;
            }
        }

        BuildingCommunicator.Instance.FocusThenOpenUpgradePopup(warehouseToUpgrade);
    }

    private void CreateFilterTabs()
    {
        CreateAFilterTab(0); // ALL Tab

        var buildings = _buildings.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            int capacity = _buildingUpgrades.GetCapacity(building.id, 1);
            if (capacity == 0)
            {
                continue;
            }

            CreateAFilterTab(building.id);
        }
    }

    private void CreateAFilterTab(int buildingId)
    {
        var filterTab = Instantiate(_filterTabTemplate, _filterTabsContent);
        filterTab.gameObject.SetActive(true);

        filterTab.SetText("ALL");
        if (buildingId != 0)
        {
            filterTab.SetText(_buildings.GetBuildingName(buildingId).Replace("Warehouse", ""));
        }

        filterTab.name = buildingId.ToString();
        var button = filterTab.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            ApplyFilter(buildingId);
        });

        _filterTabs.Add(button);
    }

    private void DisplayFilterTabs(int filterWarehouse)
    {
        for (int i = 0; i < _filterTabs.Count; i++)
        {
            var tab = _filterTabs[i];
            int.TryParse(tab.name, out int warehouse);
            if (warehouse == filterWarehouse)
            {
                tab.interactable = false;
                continue;
            }

            tab.interactable = true;
        }
    }

    private void Awake()
    {
        _panel = GetComponent<UIPanel>();
    }

    private void OnEnable()
    {
        CreateFilterTabs();
        _originFilterTabsContentPos = _filterTabsContent.transform.localPosition;
    }
}
