using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "UserItemStorage", menuName = "Uniflow/User/UserItemList")]
public class UserItemStorage : ScriptableObject
{
    [Header("Referenece - Read")]
    [SerializeField]
    private ItemList _itemList;
    [SerializeField]
    private ItemTypeList _itemTypes;
    [SerializeField]
    private WarehousesCapacity _warehouses;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _gold;
    [SerializeField]
    private IntegerVariable _diamond;
    [SerializeField]
    private IntegerVariable _scoin;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onInventoryChanged;

    [Header("Inspec")]
    [SerializeField]
    private List<UserItemPoco> _userItemList;
    // key = itemId, value = itemCount
    private Dictionary<int, UserItemPoco> _userItemDict = new();
    private Dictionary<int, UserItemPoco> ItemDict
    {
        get
        {
            if (_userItemList.Count != _userItemDict.Count)
            {
                _userItemDict.Clear();

                for (int i = 0; i < _userItemList.Count; i++)
                {
                    _userItemDict[_userItemList[i].itemId] = _userItemList[i];
                }
            }

            return _userItemDict;
        }
    }
    [SerializeField]
    private List<ItemWithRandomOption> _userItemsWithRandomOption;
    private Dictionary<int, ItemWithRandomOption> _randomOptionsDict = new();
    public Dictionary<int, ItemWithRandomOption> RandomOptionsDict
    {
        get
        {
            if (_randomOptionsDict.Count != _userItemsWithRandomOption.Count)
            {
                _randomOptionsDict.Clear();
                for (int i = 0; i < _userItemsWithRandomOption.Count; i++)
                {
                    var item = _userItemsWithRandomOption[i];
                    _randomOptionsDict[item.itemId] = item;
                }
            }

            return _randomOptionsDict;
        }
    }

    private List<ResourcePoco> _cachedUserItems = new();
    private int _recentlyChangedItem;
    private int RecentlyChangedItem // Set it after changing inventory
    {
        get
        {
            return _recentlyChangedItem;
        }
        set
        {
            _recentlyChangedItem = value;
            _onInventoryChanged.Raise(_recentlyChangedItem);
        }
    }

    public void Init(ApiLoginResult data)
    {
        InitItems(data);
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    private void InitItems(ApiLoginResult data)
    {
        int itemTypeCount = data.LoginInventoryLength;
        if (itemTypeCount == 0)
        {
            return;
        }

        ResetInventory();

        for (int i = 0; i < itemTypeCount; i++)
        {
            var itemData = data.LoginInventory(i).Value;
            _userItemDict.TryGetValue(itemData.IdItem, out var item);
            if (item == null)
            {
                item = new UserItemPoco() { itemId = itemData.IdItem, itemCount = 0 };
                UpdateBasicItems(item);
                _userItemList.Add(item);
                _userItemDict.Add(item.itemId, item);
            }

            item.itemCount += itemData.Quantity;
            InitAddItemWithOption(itemData);
        }

        // Init Cached Items
        GetAllItemsCount();
        GetAllItems();
    }

    public void RefreshItems(GetInfoInventory data)
    {
        ResetInventory();

        int itemCount = data.InfoInventoryLength;
        if (itemCount == 0)
        {
            return;
        }

        for (int i = 0; i < itemCount; i++)
        {
            var itemData = data.InfoInventory(i).Value;

            ItemDict.TryGetValue(itemData.IdItem, out var userItem);
            if (userItem == null)
            {
                var newItem = new UserItemPoco() { itemId = itemData.IdItem, itemCount = itemData.Quantity };
                _userItemList.Add(newItem);
                _userItemDict.Add(newItem.itemId, newItem);
                UpdateBasicItems(newItem);
                InitAddItemWithOption(itemData);
                continue;
            }

            userItem.itemCount = itemData.Quantity;
            InitAddItemWithOption(itemData);
        }
    }
    private void InitAddItemWithOption(LoginInventory itemData)
    {
        var optionData = itemData.ItemOptionLogin.Value;
        if (optionData.Prefix <= 0 && optionData.Star <= 0)
        {
            return;
        }

        _randomOptionsDict.TryGetValue(itemData.IdItem, out var itemOption);
        if (itemOption == null)
        {
            itemOption = new() {itemId = itemData.IdItem };
            _userItemsWithRandomOption.Add(itemOption);
            _randomOptionsDict.Add(itemOption.itemId, itemOption);
        }

        itemOption.randomOptions.Add(new() { star = optionData.Star, prefix = optionData.Prefix });
    }

    public int GetItemCount(int itemId)
    {
        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            return GetItemGroupCount(itemId);
        }
        return item.ActualCount;
    }

    public int GetItemCount(int itemId, RandomOption option)
    {
        RandomOptionsDict.TryGetValue(itemId, out var itemWithOptions);
        if (itemWithOptions == null)
        {
            return GetItemCount(itemId);
        }
        var options = itemWithOptions.randomOptions;
        if (option.IsNoAddition())
        {
            return GetItemCount(itemId) - options.Count;
        }

        int count = 0;
        for (int i = 0; i < options.Count; i++)
        {
            var inventoryOption = options[i];
            if (inventoryOption.Equals(option))
            {
                count++;
            }
        }

        return count;
    }

    public int GetAllItemsCount()
    {
        int allItemsCount = 0;
        for (int i = 0; i < _userItemList.Count; i++)
        {
            var item = _userItemList[i];
            allItemsCount += item.ActualCount;
        }

        return allItemsCount;
    }

    public int GetCountOfType(ItemType type, bool includeIncoming = false)
    {
        if (type == ItemType.None)
        {
            return GetAllItemsCount();
        }

        int count = 0;
        for (int i = 0; i < _userItemList.Count; i++)
        {
            var item = _userItemList[i];
            if (_itemList.GetItemType(item.itemId) != type)
            {
                continue;
            }

            count += item.ActualCount;
            if (includeIncoming)
            {
                count += item.incomingCount;
            }
        }

        return count;
    }

    public int GetCountBasedOnContainWarehouse(int warehouseId, bool includeIncoming = false)
    {
        if (warehouseId == 0)
        {
            return GetAllItemsCount();
        }
        int count = 0;
        for (int i = 0; i < _userItemList.Count; i++)
        {
            var item = _userItemList[i];
            ItemType type = _itemList.GetItemType(item.itemId);
            int containeWarehouse = _itemTypes.GetContainedWarehouse(type);
            if (containeWarehouse != warehouseId)
            {
                continue;
            }

            count += item.ActualCount;
            if (includeIncoming)
            {
                count += item.incomingCount;
            }
        }

        return count;
    }

    public List<ResourcePoco> GetAllItems()
    {
        _cachedUserItems.Clear();
        for (int i = 0; i < _userItemList.Count; i++)
        {
            var item = _userItemList[i];
            _cachedUserItems.Add(new() { itemId = item.itemId, itemCount = item.ActualCount });
        }

        SortItem(_cachedUserItems);

        return _cachedUserItems;
    }

    public List<ItemWithRandomOption> GetAllItemOptions()
    {
        return _userItemsWithRandomOption;
    }

    private void SortItem(List<ResourcePoco> items)
    {
        Dictionary<int, List<ResourcePoco>> perOrderList = new(); // key = order of ItemType
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            ItemType type = _itemList.GetItemType(item.itemId);
            int order = _itemTypes.GetOrder(type);
            perOrderList.TryGetValue(order, out var itemsForType);
            if (itemsForType == null)
            {
                itemsForType = new();
                perOrderList.Add(order, itemsForType);
            }
            itemsForType.Add(item);
        }

        _cachedUserItems.Clear();
        for (int i = 0; i < _itemTypes.TypeCount; i++)
        {
            perOrderList.TryGetValue(i, out var list);
            if (list == null || list.Count == 0)
            {
                continue;
            }

            list.Sort(ItemKeyCompare);
            _cachedUserItems.AddRange(list);
        }
    }
    private int ItemKeyCompare(ResourcePoco a, ResourcePoco b)
    {
        return CollectionUtils.ObjectCompare(a, b, nameof(ResourcePoco.itemId));
    }

    public List<RandomOption> GetRandomOptionsForItem(int itemId)
    {
        RandomOptionsDict.TryGetValue(itemId, out var item);
        if (item == null || item.randomOptions.Count == 0)
        {
            return new();
        }

        return item.randomOptions;
    }

    public void Add(ResourcePoco resource, RandomOption randomOption = default)
    {
        if (resource.itemId == 0)
        {
            return;
        }


        if (ItemHelper.IsGroupItemId(resource.itemId))
        {
            AddItemGroup(resource);
            return;
        }

        ItemDict.TryGetValue(resource.itemId, out var userItem);
        if (userItem == null)
        {
            userItem = new UserItemPoco
            {
                itemId = resource.itemId,
            };
            _userItemList.Add(userItem);
            _userItemDict[resource.itemId] = userItem;
        }
        if (userItem.incomingCount == 0)
        {
            userItem.itemCount += resource.itemCount;
        }
        userItem.incomingCount -= Mathf.Clamp(userItem.incomingCount - resource.itemCount, 0, userItem.incomingCount);

        if (!randomOption.IsNoAddition())
        {
            AddItemWithRandomOptions(resource, randomOption);
        }

        UpdateBasicItems(userItem);

        RecentlyChangedItem = resource.itemId;
    }

    public List<RandomOption> Sub(ResourcePoco resource, RandomOption randomOption = default)
    {
        Sub(resource, out var subOptions, randomOption);
        return subOptions;
    }

    public bool Sub(ResourcePoco resource, out List<RandomOption> subOptions, RandomOption randomOption = default)
    {
        subOptions = new();
        int itemId = resource.itemId;
        if (ItemHelper.IsGroupItemId(itemId))
        {
            itemId = GetGreatestCountItemIdInGroup(itemId);
        }

        if (itemId == 0)
        {
            Logger.LogError($"Invalid ItemId [{itemId}]");
            return false;
        }

        if (!CanAfford(resource))
        {
            return false;
        }

        for (int i = 0; i < resource.itemCount; i++)
        {
            subOptions.Add(SubOne(resource.itemId, randomOption, false));
        }

        return true;
    }

    public RandomOption SubOne(int itemId, RandomOption randomOptions = default, bool checkGroupItem = true)
    {
        if (!CanAfford(new ResourcePoco { itemId = itemId, itemCount = 1 }))
        {
            return default;
        }

        RandomOption subOption = RandomOption.NoAddition;
        if (checkGroupItem && ItemHelper.IsGroupItemId(itemId))
        {
            itemId = GetGreatestCountItemIdInGroup(itemId);
        }

        if (itemId == 0)
        {
            Logger.LogError($"Invalid ItemId [{itemId}]");
            return subOption;
        }

        ItemDict.TryGetValue(itemId, out var userItem);
        if (userItem == null || userItem.ActualCount == 0)
        {
            return subOption;
        }

        userItem.itemCount--;
        RecentlyChangedItem = itemId;

        return SubAnOptionItemIfNeeded(userItem.itemId, randomOptions);
    }

    public void AddOptionForItem(int itemId, int count, RandomOption option)
    {
        if (option.Equals(RandomOption.NoAddition))
        {
            return;
        }

        RandomOptionsDict.TryGetValue(itemId, out var options);
        if (options == null)
        {
            options = new()
            {
                itemId = itemId,
                randomOptions = new()
            };
            _userItemsWithRandomOption.Add(options);
            _randomOptionsDict.Add(options.itemId, options);
        }

        for (int i = 0; i < count; i++)
        {
            options.randomOptions.Add(option);
        }
    }

    public bool TryReserve(int itemId, int count)
    {
        if (count == 0)
        {
            return true;
        }
        if (!_warehouses.CanAddItem(itemId, count, true))
        {
            return false;
        }

        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            item = new() { itemId = itemId };
            _userItemList.Add(item);
            _userItemDict.Add(item.itemId, item);
        }
        item.itemCount += count;
        item.incomingCount += count;

        RecentlyChangedItem = itemId;

        return true;
    }

    public bool TryReserve(List<ResourcePoco> items)
    {
        if (ItemHelper.IsResourcesEmpty(items))
        {
            return true;
        }

        if (!_warehouses.CanAddItem(items, true))
        {
            return false;
        }

        for (int i = 0; i < items.Count; i++)
        {
            TryReserve(items[i].itemId, items[i].itemCount);
        }

        return true;
    }

    public void RevokeReserve(int itemId, int count)
    {
        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            return;
        }

        item.itemCount = Mathf.Clamp(item.itemCount - 1, 0, item.itemCount);
        item.incomingCount = Mathf.Clamp(item.incomingCount - count, 0, item.incomingCount);

        RecentlyChangedItem = itemId;
    }

    public bool CanAfford(List<ResourcePoco> resources)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            var resource = resources[i];
            if (!CanAfford(resource))
            {
                return false;
            }
        }

        return true;
    }

    public bool CanAfford(ResourcePoco resource)
    {
        if (ResourcePoco.IsZero(resource))
        {
            return true;
        }

        int userOwn = GetItemCount(resource.itemId);
        if (userOwn < resource.itemCount)
        {
            return false;
        }

        return true;
    }

    public bool TryTakeAnItem(int itemId)
    {
        ItemDict.TryGetValue(itemId, out var userItem);
        if (userItem.ActualCount == 0)
        {
            return false;
        }

        Sub(new() { itemId = itemId, itemCount = 1 });
        return true;
    }

    public bool TryTakeAnItemWithOption(int itemId, out RandomOption outOption)
    {
        outOption = RandomOption.NoAddition;
        ItemDict.TryGetValue(itemId, out var userItem);
        if (userItem.ActualCount == 0)
        {
            return false;
        }

        Sub(new() { itemId = itemId, itemCount = 1 });
        return true;
    }

    public bool TryTakeAnItemFromGroup(int groupId, out int itemId)
    {
        if (!TryFindItem(groupId, out itemId))
        {
            return false;
        }

        ItemDict.TryGetValue(itemId, out var item);
        if (item.ActualCount == 0)
        {
            return false;
        }

        Sub(new() { itemId = itemId, itemCount = 1 });
        return true;
    }

    public bool TryFindItem(int groupId, out int itemId)
    {
        itemId = 0;
        if (!ItemHelper.IsGroupItemId(groupId))
        {
            return false;
        }

        List<UserItemPoco> groupItems = GetGroupItems(groupId);
        if (groupItems.Count == 0)
        {
            return false;
        }

        itemId = groupItems[0].itemId;
        if (groupItems.Count == 1)
        {
            return true;
        }

        UserItemPoco returnResource = groupItems[0];
        for (int i = 1; i < groupItems.Count; i++)
        {
            if (CompareItem(returnResource, groupItems[i]) == -1)
            {
                continue;
            }

            returnResource = groupItems[i];
        }

        itemId = returnResource.itemId;
        return true;
    }

    public bool TryTakeItemsFromGroup(ResourcePoco groupItems, out List<ResourcePoco> items)
    {
        items = new();
        for (int i = 0; i < groupItems.itemCount; i++)
        {
            if (!TryTakeAnItemFromGroup(groupItems.itemId, out int itemId))
            {
                RevertTryTakeResources(items);
                items.Clear();
                return false;
            }

            for (int j = 0; j < items.Count; j++)
            {
                if (items[j].itemId == itemId)
                {
                    var item = items[j];
                    item.itemCount++;
                    items[j] = item;
                    goto TRY_GET_NEXT;
                }
            }
            items.Add(new() { itemId = itemId, itemCount = 1 });

        TRY_GET_NEXT:
            continue;
        }

        return true;
    }

    public int GetGreatestCountItemIdInGroup(int itemGroupId)
    {
        int greatestItemId = 0;
        int greatestCount = 0;
        List<UserItemPoco> groupItems = GetGroupItems(itemGroupId);
        for (int i = 0; i < groupItems.Count; i++)
        {
            var item = groupItems[i];
            if (greatestCount < item.ActualCount)
            {
                greatestItemId = item.itemId;
                greatestCount = item.ActualCount;
            }
        }

        return greatestItemId;
    }

    private void RevertTryTakeResources(List<ResourcePoco> tryTakeResources)
    {
        for (int i = 0; i < tryTakeResources.Count; i++)
        {
            Add(tryTakeResources[i]);
        }
    }

    private int GetItemGroupCount(int itemGroupId)
    {
        if (itemGroupId % 10000 != 0) // not an item group id
        {
            return 0;
        }

        int itemGroupCount = 0;
        for (int i = 0; i < _userItemList.Count; i++)
        {
            var userItem = _userItemList[i];
            if (userItem.itemId / 10000 != itemGroupId / 10000)
            {
                continue;
            }

            itemGroupCount += userItem.ActualCount;
        }

        return itemGroupCount;
    }

    private void AddItemGroup(ResourcePoco resource)
    {
        List<ItemPoco> randomItems = _itemList.GetRandomItemsInGroup(resource.itemId, resource.itemCount);
        for (int i = 0; i < randomItems.Count; i++)
        {
            int itemId = randomItems[i].itemId;
            Add(new ResourcePoco { itemId = itemId, itemCount = 1 });

            RecentlyChangedItem = itemId;
        }
    }

    private void AddItemWithRandomOptions(ResourcePoco addedResource, RandomOption randomOptions)
    {
        RandomOptionsDict.TryGetValue(addedResource.itemId, out var item);
        if (item == null)
        {
            item = new ItemWithRandomOption
            {
                itemId = addedResource.itemId
            };

            _userItemsWithRandomOption.Add(item);
            _randomOptionsDict[item.itemId] = item;
        }

        item.randomOptions.Add(randomOptions);
    }

    private RandomOption SubAnOptionItemIfNeeded(int itemId, RandomOption randomOption)
    {
        RandomOption subOption = RandomOption.NoAddition;
        RandomOptionsDict.TryGetValue(itemId, out var itemOptions);
        if (itemOptions == null || itemOptions.randomOptions.Count == 0)
        {
            return subOption;
        }

        List<RandomOption> options = itemOptions.randomOptions;
        ItemDict.TryGetValue(itemId, out var userItem);

        if (randomOption.IsNoAddition())
        {
            return SubAnOptionItemIfNotEnoughPureItem(userItem);
        }

        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].Equals(randomOption))
            {
                subOption = options[i];
                options.RemoveAt(i);
                return subOption;
            }
        }

        return SubAnOptionItemIfNotEnoughPureItem(userItem);
    }

    private RandomOption SubAnOptionItemIfNotEnoughPureItem(UserItemPoco userItem)
    {
        RandomOption subOption = RandomOption.NoAddition;
        RandomOptionsDict.TryGetValue(userItem.itemId, out var itemOptions);
        if (itemOptions == null || itemOptions.randomOptions.Count == 0)
        {
            return subOption;
        }

        List<RandomOption> options = itemOptions.randomOptions;
        subOption = options[^1];
        options.RemoveAt(options.Count - 1);

        return subOption;
    }

    private void SubItemGroup(ResourcePoco resource)
    {
        List<UserItemPoco> itemGroups = GetGroupItems(resource.itemId);
        
        if (itemGroups.Count == 0)
        {
            return;
        }

        if (itemGroups.Count == 1)
        {
            var item = itemGroups[0];
            if (item.ActualCount < resource.itemCount)
            {
                return;
            }

            item.itemCount = Mathf.Clamp(item.itemCount - resource.itemCount, 0, item.ActualCount);
            return;
        }

        itemGroups.Sort(CompareItem);

        for (int i = 0; i < resource.itemCount; i++)
        {
            var greatest = itemGroups[0];

            if (greatest.ActualCount == 0)
            {
                return;
            }

            greatest.itemCount--;
            if (CompareItem(greatest, itemGroups[1]) == -1)
            {
                itemGroups.Sort(CompareItem);
            }
        }

    }

    private List<UserItemPoco> GetGroupItems(int itemGroupId)
    {
        List<UserItemPoco> itemGroups = new();
        if (itemGroupId % 10000 != 0)
        {
            return itemGroups;
        }

        for (int i = 0; i < _userItemList.Count; i++)
        {
            var item = _userItemList[i];
            if (item.itemId / 10000 != itemGroupId / 10000)
            {
                continue;
            }

            if (item.ActualCount == 0)
            {
                continue;
            }

            itemGroups.Add(item);
        }

        return itemGroups;
    }

    private int CompareItem(UserItemPoco a, UserItemPoco b)
    {
        string itemActualCount = nameof(UserItemPoco.ActualCount);
        string itemIdFieldName = nameof(UserItemPoco.itemId);

        int compare = CollectionUtils.ObjectCompareProperty(a, b, itemActualCount);
        if (compare == 0)
        {
            compare = CollectionUtils.ObjectCompare(a, b, itemIdFieldName);
        }

        return compare;
    }

    private void UpdateBasicItems(UserItemPoco resource)
    {
        int itemId = resource.itemId;
        if (itemId == 990099)
        {
            _gold.Value = resource.ActualCount;
            return;
        }

        if (itemId == 990100)
        {
            _diamond.Value = resource.ActualCount;
            return;
        }

        if (itemId == 990101)
        {
            _scoin.Value = resource.ActualCount;
        }
    }

    private void ResetInventory()
    {
        _userItemList.Clear();
        _userItemDict.Clear();
        _userItemsWithRandomOption.Clear();
        _randomOptionsDict.Clear();
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;

        _gold.Value = GetItemCount(990099);
        _diamond.Value = GetItemCount(990100);
        _scoin.Value = GetItemCount(990101);
    }

    [System.Serializable]
    private class UserItemPoco
    {
        public int itemId;
        public int itemCount;
        public int ActualCount => itemCount - incomingCount;
        public int incomingCount;
    }
}
