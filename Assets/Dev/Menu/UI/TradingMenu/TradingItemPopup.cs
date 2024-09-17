using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingItemPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedItemPackId;
    [SerializeField]
    private IntegerVariable _selectedCaravan;
    [SerializeField]
    private LandTradingItemList _landSellingItems;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private UserProfile _profile;

    [Header("Configs")]
    [SerializeField]
    private InventoryItemCellWithName _itemDisplayer;
    [SerializeField]
    private TMP_Text _packItemCount;
    [SerializeField]
    private TMP_Text _lastestPrice;
    [SerializeField]
    private TMP_Text _lowestPrice;
    [SerializeField]
    private TMP_Text _perItemPrice;
    [SerializeField]
    private TMP_Text _remainTimeText;
    [SerializeField]
    private TMP_Text _price;
    [SerializeField]
    private TMP_Text _tariff;

    public void SetUp()
    {
        var tradingItem = _landSellingItems.GetTradingItem(_selectedItemPackId.Value);
        if (tradingItem == null)
        {
            return;
        }

        DisplayItemBasicInfo(tradingItem);
        DisplayItemMarketInfo();
        DisplayPackInfo();

        bool isMySellingItem = tradingItem.seller == _profile.id;
        if (isMySellingItem)
        {
            DisplayMySellingItem(tradingItem);
            return;
        }

        DisplayLandSellingItem(tradingItem);
    }

    private void DisplayMySellingItem(TradingItem pack)
    {
        TradingItemStatus status = pack.status;
        if (status == TradingItemStatus.Selling)
        {
            return;
        }

        _price.text = TextUtils.FormatNumber(pack.Price);
    }

    private void DisplayLandSellingItem(TradingItem pack)
    {
        
        _price.text = TextUtils.FormatNumber(pack.Price);
    }

    private void DisplayItemBasicInfo(TradingItem sellingItem)
    {
        ItemGrade grade = _items.GetItemGrade(sellingItem.itemId);
        _itemDisplayer.SetUp(new()
        {
            itemId = sellingItem.itemId,
            grade = grade,
            prefix = sellingItem.prefix,
            star = sellingItem.star,
            itemCount = sellingItem.count
        });
    }

    private void DisplayItemMarketInfo()
    {
        _lastestPrice.text = "No Info";
        _lowestPrice.text = "No Info";
        if (_tariff == null)
        {
            return;
        }

        _tariff.text = "No Info";
    }

    private void DisplayPackInfo()
    {
        var tradingItem = _landSellingItems.GetTradingItem(_selectedItemPackId.Value, true);
        if (tradingItem == null)
        {
            tradingItem = new();
        }
        _perItemPrice.text = TextUtils.FormatNumber(tradingItem.Price / tradingItem.count);
        _packItemCount.text = TextUtils.FormatNumber(tradingItem.count);
        _remainTimeText.text = TimeUtils.FormatTime(tradingItem.remainTime);
    }
}
