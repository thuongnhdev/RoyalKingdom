using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemList", menuName = "Uniflow/Resource/ItemList")]
public class ItemList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private ItemGroupList _itemGroups;
    [SerializeField]
    private List<ItemPoco> _items;
    public List<ItemPoco> Items => _items;
    private Dictionary<int, ItemPoco> _itemsDict = new();
    private Dictionary<int, ItemPoco> ItemDict
    {
        get
        {
            if (_items.Count != _itemsDict.Count)
            {
                _itemsDict.Clear();

                for (int i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    _itemsDict[item.itemId] = item;
                }
            }

            return _itemsDict;
        }
    }

    public void Init(Fbs.ApiGetLowData data)
    {
        int itemDataCount = data.ItemTemplateLength;
        if (itemDataCount == 0)
        {
            return;
        }

        _items.Clear();
        _itemsDict.Clear();
        for (int i = 0; i < itemDataCount; i++)
        {
            var itemData = data.ItemTemplate(i).Value;
            Enum.TryParse(itemData.ItemType, out ItemType itemType);
            var item = new ItemPoco
            {
                itemId = itemData.IdItemTemplate,
                name = itemData.Name,
                itemType = itemType,
                price = itemData.Price,
                grade = (ItemGrade)itemData.IdItemGrade
            };

            _items.Add(item);
            _itemsDict.Add(item.itemId, item);
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    public ItemPoco GetItem(int itemId)
    {
        if (ItemHelper.IsGroupItemId(itemId))
        {
            Logger.Log($"Id [{itemId}] is group Id, return null as default", Color.green);
            return null;
        }

        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            Logger.LogError($"Invalid ItemId [{itemId}]");
        }

        return item;
    }

    public string GetItemName(int itemId)
    {
        var item = GetItem(itemId);
        if (item == null)
        {
            return "";
        }

        return item.name;
    }

    public string GetItemGroupName(int itemId)
    {
        return _itemGroups.GetGroupName(itemId);
    }

    public bool IsPrefixable(int itemId)
    {
        var item = GetItem(itemId);
        if (item == null)
        {
            return false;
        }

        return item.prefixable;
    }

    public ItemType GetItemType(int itemId)
    {
        if (ItemHelper.IsGroupItemId(itemId))
        {
            itemId++;
        }

        var item = GetItem(itemId);
        if (item == null)
        {
            return ItemType.None;
        }

        return item.itemType;
    }

    public ItemGrade GetItemGrade(int itemId)
    {
        var item = GetItem(itemId);
        if (item == null)
        {
            return ItemGrade.None;
        }

        return item.grade;
    }

    public List<int> GetAllItemsOfGroup(int groupItemId)
    {
        List<int> groupItems = new();
        if (!ItemHelper.IsGroupItemId(groupItemId))
        {
            Logger.Log($"[{groupItemId}] is not ItemGroupId, return empty as default!", Color.green);
            return groupItems;
        }

        for (int i = 0; i < _items.Count; i++)
        {
            int itemId = _items[i].itemId;
            if (itemId / 10000 * 10000 != groupItemId)
            {
                continue;
            }

            groupItems.Add(itemId);
        }

        return groupItems;
    }

    public List<ItemPoco> GetRandomItemsInGroup(int itemGroupId, int count)
    {
        List<ItemPoco> randomItems = new();
        if (count <= 0)
        {
            return randomItems;
        }

        List<ItemPoco> itemsInGroup = new();
        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            if (item.itemId / 10000 != itemGroupId / 10000)
            {
                continue;
            }

            itemsInGroup.Add(item);
        }

        for (int i = 0; i < count; i++)
        {
            int roll = UnityEngine.Random.Range(0, itemsInGroup.Count);
            randomItems.Add(itemsInGroup[roll]);
        }

        return randomItems;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        if (parsedSheets.Count == 0)
        {
            return;
        }

        var sheet = parsedSheets[1];
        if (sheet.Count == 0)
        {
            return;
        }

        _items.Clear();
        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("Key", out int itemId);
            row.GetValue("Name", out string itemName);

            row.GetValue("ItemType", out string itemTypeString);
            Enum.TryParse(itemTypeString, out ItemType itemType); // this is case sensitive, check enum string in case of fail converting

            row.GetValue("Price", out float price);
            row.GetValue("ItemGrade", out int gradeInt);
            ItemGrade grade = (ItemGrade)gradeInt;

            row.GetValue("ItemPrefixSet", out int prefixableInt);
            bool prefixable = prefixableInt == 1;


            var item = new ItemPoco
            {
                itemId = itemId,
                name = itemName,
                itemType = itemType,
                price = price,
                grade = grade,
                prefixable = prefixable
            };

            _items.Add(item);
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
