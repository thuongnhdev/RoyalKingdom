using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserItemTooltip : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private IntegerVariable _tooltipForItem;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onDisplayedItemTooltip;

    [Header("Config")]
    [SerializeField]
    private TMP_Text _groupName;
    [SerializeField]
    private Transform _itemListContent;
    [SerializeField]
    private InventoryItemCellWithName _itemTemplateObject;
    [SerializeField]
    private UITooltip _tooltip;
    [SerializeField]
    private Image[] _arrows;

    private List<GameObject> _displayItemObjs = new();

    public void SetUp()
    {
        RemoveAllItems();
        SetUpArrowImage();

        int itemId = _tooltipForItem.Value;
        bool isGroupId = ItemHelper.IsGroupItemId(itemId);
        int groupId = ItemHelper.GetGroupItemId(itemId);

        _groupName.text = _items.GetItemGroupName(groupId);

        List<int> items = isGroupId ? _items.GetAllItemsOfGroup(itemId) : new() { itemId };
        for (int i = 0; i < items.Count; i++)
        {
            SetUpAUserItem(items[i]);
        }
    }

    private void SetUpArrowImage()
    {
        for (int i = 0; i < _arrows.Length; i++)
        {
            if (i != (int)_tooltip.CurrentPadding)
            {
                _arrows[i].enabled = false;
                continue;
            }
            _arrows[i].enabled = true;
        }
    }

    private void SetUpAUserItem(int itemId)
    {
        List<InventoryScrollRow.CellData> displayData = PrepareDisplayDataForAnItem(itemId);

        for (int i = 0; i < displayData.Count; i++)
        {
            var displayItem = Instantiate(_itemTemplateObject, _itemListContent);
            displayItem.SetUp(displayData[i], _items.GetItemName(itemId));
            displayItem.gameObject.SetActive(true);
            _displayItemObjs.Add(displayItem.gameObject);
        }
    }

    private List<InventoryScrollRow.CellData> PrepareDisplayDataForAnItem(int itemId)
    {
        List<InventoryScrollRow.CellData> displayData = new();
        int itemCount = _userItems.GetItemCount(itemId);
        if (itemCount == 0)
        {
            displayData.Add(new() { itemId = itemId, itemCount = 0 });
            return displayData;
        }

        List<RandomOption> itemOptions = _userItems.GetRandomOptionsForItem(itemId);
        for (int i = 0; i < itemOptions.Count; i++)
        {
            var option = itemOptions[i];
            if (CheckExistingAndMerge(displayData, option))
            {
                continue;
            }

            displayData.Add(new()
            {
                itemId = itemId,
                itemCount = 1,
                star = option.star,
                prefix = option.prefix,
                grade = _items.GetItemGrade(itemId)
            });
        }

        itemCount -= itemOptions.Count;
        if (0 < itemCount)
        {
            displayData.Add(new()
            {
                itemId = itemId,
                itemCount = itemCount
            });
        }

        return displayData;
    }

    private bool CheckExistingAndMerge(List<InventoryScrollRow.CellData> current, RandomOption option)
    {
        for (int i = 0; i < current.Count; i++)
        {
            var data = current[i];
            if (data.star == option.star && data.prefix == option.prefix)
            {
                data.itemCount++;
                return true;
            }
        }

        return false;
    }

    private void RemoveAllItems()
    {
        for (int i = _displayItemObjs.Count - 1; 0 <= i; i--)
        {
            Destroy(_displayItemObjs[i]);
            _displayItemObjs.RemoveAt(i);
        }
    }
}
