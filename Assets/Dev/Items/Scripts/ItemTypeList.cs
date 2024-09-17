using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemType", menuName = "Uniflow/Resource/ItemTypeList")]
public class ItemTypeList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<ItemTypePoco> _itemTypes;
    public int TypeCount => _itemTypes.Count;

    public void Init(ApiGetLowData data)
    {
        _itemTypes.Clear();
        int itemTypeCount = data.ItemTypeTemplateLength;
        for (int i = 0; i < itemTypeCount; i++)
        {
            var itemTypeData = data.ItemTypeTemplate(i).Value;
            Enum.TryParse(itemTypeData.Name, out ItemType type);
            ItemTypePoco itemType = new()
            {
                type = type,
                order = i,
                taxable = itemTypeData.Taxation == 1,
                usable = itemTypeData.Useable == 1,
                disassemble = itemTypeData.Useable == 1,
                discardable = itemTypeData.Disassemble == 1,
                tradable = itemTypeData.Tradeable == 1,
                superPosition = itemTypeData.SuperPosition == 1,
                maxValue = itemTypeData.MaxValue,
                suppliable = itemTypeData.Suppliable,
                warehouseId = itemTypeData.IdWareHouse
            };

            _itemTypes.Add(itemType);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public ItemTypePoco GetItemTypeDescription(ItemType itemType)
    {
        for (int i = 0; i < _itemTypes.Count; i++)
        {
            if (_itemTypes[i].type == itemType)
            {
                return _itemTypes[i];
            }
        }

        Logger.LogError($"Invalid ItemType [{itemType}]");
        return null;
    }

    public int GetContainedWarehouse(ItemType itemType)
    {
        var type = GetItemTypeDescription(itemType);
        if (type == null)
        {
            return 0;
        }

        return type.warehouseId;
    }

    public int GetSlotLimit(ItemType itemType)
    {
        var type = GetItemTypeDescription(itemType);
        if (type == null)
        {
            return 0;
        }

        return type.maxValue;
    }

    public int GetOrder(ItemType itemType)
    {
        var type = GetItemTypeDescription(itemType);
        if (type == null)
        {
            return int.MaxValue;
        }

        return type.order;
    }

    public bool IsUsable(ItemType type)
    {
        var itemType = GetItemTypeDescription(type);
        if (itemType == null)
        {
            return false;
        }

        return itemType.usable;
    }

    public bool IsTradable(ItemType type)
    {
        var itemType = GetItemTypeDescription(type);
        if (itemType == null)
        {
            return false;
        }

        return itemType.tradable;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        if (parsedSheets.Count == 0)
        {
            return;
        }

        var sheet = parsedSheets[7];
        if (sheet.Count == 0)
        {
            return;
        }

        _itemTypes.Clear();

        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];

            var itemType = new ItemTypePoco();

            row.GetValue("ItemType", out string itemTypeStr);
            Enum.TryParse(itemTypeStr, out ItemType type);
            itemType.type = type;

            row.GetValue("Taxation", out int taxable);
            itemType.taxable = taxable == 1;
            row.GetValue("Usable", out int usable);
            itemType.usable = usable == 1;
            row.GetValue("Disassemble", out int disassemble);
            itemType.disassemble = disassemble == 1;
            row.GetValue("Discard", out int discard);
            itemType.discardable = discard == 1;
            row.GetValue("Tradeable", out int tradable);
            itemType.tradable = tradable == 1;
            row.GetValue("Superposition", out int superPosition);
            itemType.superPosition = superPosition == 1;
            row.GetValue("MaxValue", out int maxValue);
            itemType.maxValue = maxValue;

            _itemTypes.Add(itemType);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
