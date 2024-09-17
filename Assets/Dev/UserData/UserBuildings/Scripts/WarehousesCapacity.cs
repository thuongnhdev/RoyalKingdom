using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WarehousesCapacity", menuName = "Uniflow/User/Warehouses Capacity")]
public class WarehousesCapacity : ScriptableObject
{
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgrades;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private ItemTypeList _itemTypes;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private GameEvent _onWarehouseCapacityChanged;
    [SerializeField]
    private GameEvent _notifyCannotAddItem;
    [SerializeField]
    private List<WarehouseCapacity> _warehousesCapacity;

    public void Init(List<UserBuilding> buildings)
    {
        _warehousesCapacity.Clear();
        for (int i = 0; i < buildings.Count; i++)
        {
            AddCapacity(buildings[i], true);
        }
    }

    public void AddCapacity(UserBuilding building, bool dontNotify = false)
    {
        if (building.status != BuildingStatus.Idle &&
            building.status != BuildingStatus.WaitingForUpgradeResource &&
            building.status != BuildingStatus.Upgrading)
        {
            return;
        }

        int capacity = _buildingUpgrades.GetCapacity(building.buildingId, building.buildingLevel);
        if (capacity == 0)
        {
            return;
        }

        var warehouse = GetWarehouse(building.buildingId);
        if (warehouse == null)
        {
            warehouse = new() { id = building.buildingId, warehouseName = building.editorBuildingName };
            _warehousesCapacity.Add(warehouse);
        }
        warehouse.capacity += capacity;

        if (dontNotify)
        {
            return;
        }

        _onWarehouseCapacityChanged.Raise(building.buildingId);
    }

    public void SubCapacity(UserBuilding building)
    {
        int capacity = _buildingUpgrades.GetCapacity(building.buildingId, building.buildingLevel);
        if (capacity == 0)
        {
            return;
        }

        var warehouse = GetWarehouse(building.buildingId);
        if (warehouse == null)
        {
            return;
        }

        warehouse.capacity -= capacity;

        _onWarehouseCapacityChanged.Raise(building.buildingId);
    }

    public int GetCapacity(int warehouseId)
    {
        var warehouse = GetWarehouse(warehouseId);
        if (warehouse == null)
        {
            return 0;
        }

        return warehouse.capacity;
    }

    public bool CanAddItem(List<ResourcePoco> items, bool notify = false)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (!CanAddItem(items[i], notify))
            {
                return false;
            }
        }

        return true;
    }

    public bool CanAddItem(ResourcePoco item, bool notify = false)
    {
        return CanAddItem(item.itemId, item.itemCount, notify);
    }

    public bool CanAddItem(int itemId, int count, bool notify = false)
    {
        if (count == 0)
        {
            return true;
        }

        int correspondWarehouse = GetCorespondingWarehouse(itemId);
        var warehouse = GetWarehouse(correspondWarehouse);
        if (warehouse == null)
        {
            if (notify)
            {
                _notifyCannotAddItem.Raise(itemId);
            }
            return false;
        }

        int loaded = _userItems.GetCountBasedOnContainWarehouse(correspondWarehouse, true);
        int capacity = warehouse.capacity;

        bool canAdd = loaded + count <= capacity;
        if (!canAdd && notify)
        {
            _notifyCannotAddItem.Raise(itemId);
        }

        return canAdd;
    }

    public int GetSlotAvailableForItem(int itemId)
    {
        ItemType type = _items.GetItemType(itemId);
        int correspondWarehouse = _itemTypes.GetContainedWarehouse(type);
        int capacity = GetCapacity(correspondWarehouse);
        int loaded = _userItems.GetCountBasedOnContainWarehouse(correspondWarehouse, true);
        return capacity - loaded;
    }

    public int GetWarehouseForItem(int itemId)
    {
        ItemType type = _items.GetItemType(itemId);
        return _itemTypes.GetContainedWarehouse(type);
    }

    private WarehouseCapacity GetWarehouse(int buildingId)
    {
        for (int i = 0; i < _warehousesCapacity.Count; i++)
        {
            var warehouse = _warehousesCapacity[i];
            if (warehouse.id == buildingId)
            {
                return warehouse;
            }
        }

        return null;
    }

    private int GetCorespondingWarehouse(int itemId)
    {
        ItemType type = _items.GetItemType(itemId);
        int correspondWarehouse = _itemTypes.GetContainedWarehouse(type);

        return correspondWarehouse;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    [System.Serializable]
    private class WarehouseCapacity
    {
        public string warehouseName;
        public int id;
        public int capacity;
    }
}
