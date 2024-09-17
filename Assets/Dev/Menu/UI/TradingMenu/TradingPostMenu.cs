using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingPostMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _profile;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private LandTradingItemList _landTradingItems;
    [SerializeField]
    private CaravanList _caravans;
    [SerializeField]
    private IntegerVariable _selectedCaravan;

    [Header("Configs")]
    [SerializeField]
    private TradingPostScrollController _tradingPostScroller;
    [SerializeField]
    private InventoryMenuScrollController _storageScroller;

    public void DisplayCaravanStorage()
    {
        List<ResourcePoco> items;
        List<ItemWithRandomOption> options;
        if (_selectedCaravan.Value == 0)
        {
            items = _userItems.GetAllItems();
            options = _userItems.GetAllItemOptions();
        }
        else
        {
            items = _caravans.GetStorage(_selectedCaravan.Value);
            options = _caravans.GetStorageRandomOptions(_selectedCaravan.Value);
        }

        var optionDict = CollectionUtils.CreateDictionaryFromList<int, ItemWithRandomOption>(options, nameof(ItemWithRandomOption.itemId));
        _storageScroller.PrepareDataAndShowAll(items, optionDict);
    }

    public void DisplayLandTradingItems()
    {
        List<TradingItem> showItems = new();
        List<TradingItem> landItems = _landTradingItems.TradingItems;

        for (int i = 0; i < landItems.Count; i++)
        {
            var item = landItems[i];
            if (item.seller == _profile.id)
            {
                continue;
            }

            showItems.Add(item);
        }

        _tradingPostScroller.RefreshView(showItems);
    }

    public void DisplayCaravanSellingItems()
    {
        if (_selectedCaravan.Value == 0)
        {
            return;
        }

        List<int> packs = _caravans.GetItemPack(_selectedCaravan.Value);
        List<TradingItem> sellingItems = new();
        for (int i = 0; i < packs.Count; i++)
        {
            var item = _landTradingItems.GetTradingItem(packs[i]);
            if (item == null)
            {
                continue;
            }

            sellingItems.Add(item);
        }

        _tradingPostScroller.RefreshView(sellingItems);
    }

    public void Remove_DisplayCaravansSelling()
    {
        if (_selectedCaravan.Value == 0)
        {
            _selectedCaravan.Value = 1;
        }

        DisplayCaravanSellingItems();
    }

    public void Refresh()
    {
        DisplayCaravanSellingItems();
        DisplayLandTradingItems();
        DisplayCaravanStorage();
    }
}
