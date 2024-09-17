using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutOfWarehouseCapacityPopup : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _bcmSelectedBuildingId;

    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private ItemTypeList _itemTypes;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private WarehousesCapacity _warehousesCapacity;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onAskOpenWarehouseCapacityWarning;
    [SerializeField]
    private GameEvent _onNotifiedCannotAddItem;

    [Header("Configs")]
    [SerializeField]
    private Button _buildingButton;
    [SerializeField]
    private Button _upgradeButton;
    [SerializeField]
    private TMP_Text _warehouseName;
    [SerializeField]
    private TMP_Text _currentBuildingCount;
    [SerializeField]
    private TMP_Text _currentCapacity;

    private UIPanel _panel;
    private int _showForWarehouseId;

    private void SetUp(params object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        _panel.Open();

        _showForWarehouseId = (int)args[0];
        _warehouseName.text = _buildings.GetBuildingName(_showForWarehouseId);

        int currentWarehouseCount = _userBuildings.GetBuildingCount(_showForWarehouseId);
        int maxWarehouseCount = _userBuildings.GetMaxBuildingCount(_showForWarehouseId);
        _currentBuildingCount.text = $"{currentWarehouseCount}/{maxWarehouseCount}";

        SetUpBuildAndUpgradeButtons(currentWarehouseCount, maxWarehouseCount);

        int capacity = _warehousesCapacity.GetCapacity(_showForWarehouseId);
        int loaded = _userItems.GetCountBasedOnContainWarehouse(_showForWarehouseId);
        _currentCapacity.text = $"<color=red>{TextUtils.FormatNumber(loaded)}</color>/{TextUtils.FormatNumber(capacity)}";
    }

    private void SetUpForItem(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int itemId = (int)args[0];
        int warehouse = _warehousesCapacity.GetWarehouseForItem(itemId);

        SetUp(warehouse);
    }

    public void Upgrade()
    {
        int targetWarehouse = _userBuildings.GetLowestLevelIdleBuildingOfId(_showForWarehouseId);
        BuildingCommunicator.Instance.FocusThenOpenUpgradePopup(targetWarehouse);
    }

    public void OpenBCM()
    {
        BuildingCommunicator.Instance.OpenBcmAndFocusBuilding(_showForWarehouseId);
    }

    private void SetUpBuildAndUpgradeButtons(int currentWarehouseCount, int maxWarehouseCount)
    {
        _buildingButton.interactable = currentWarehouseCount < maxWarehouseCount;
        int currentIdleWarehouseCount = _userBuildings.GetLowestLevelIdleBuildingOfId(_showForWarehouseId);
        _upgradeButton.interactable = currentIdleWarehouseCount != 0;
    }

    private void OnEnable()
    {
        _panel = GetComponent<UIPanel>();
        _onAskOpenWarehouseCapacityWarning.Subcribe(SetUp);
        _onNotifiedCannotAddItem.Subcribe(SetUpForItem);
    }

    private void OnDisable()
    {
        _onAskOpenWarehouseCapacityWarning.Unsubcribe(SetUp);
        _onNotifiedCannotAddItem.Unsubcribe(SetUpForItem);
    }
}
