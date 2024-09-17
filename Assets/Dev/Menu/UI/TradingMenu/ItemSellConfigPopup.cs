using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSellConfigPopup : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private ItemSellingForm _sellingForm;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedCaravan;
    [SerializeField]
    private CaravanList _caravans;
    [SerializeField]
    private UserItemStorage _userItems;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onSelectedItem;

    [Header("Configs")]
    [SerializeField]
    private InventoryItemCellWithName _itemDisplayer;
    [SerializeField]
    private TMP_Text _lastestPrice;
    [SerializeField]
    private TMP_Text _lowestPrice;
    [SerializeField]
    private TMP_InputField _perItemPrice;
    [SerializeField]
    private TMP_InputField _packItemCount;
    [SerializeField]
    private TMP_Text _price;
    [SerializeField]
    private TMP_Text _netIncome;
    [SerializeField]
    private TMP_Dropdown _durationDropdown;
    [SerializeField]
    private int[] _durationConfigs;
    [SerializeField]
    private TMP_Text _tariff;

    [Header("Inspec")]
    [SerializeField]
    private InventoryScrollRow.CellData _selectedItemData;

    private void SetUp()
    {
        int itemId = _selectedItemData.itemId;
        int star = _selectedItemData.star;
        int prefix = _selectedItemData.prefix;
        RandomOption option = new() { star = star, prefix = prefix };
        DisplayMarketInfo();
        DisplayDefaultSellValue();

        if (_selectedCaravan.Value == 0)
        {
            DisplayItemFromInventory(itemId, option);
            return;
        }

        DisplayItemFromCaravan(itemId, option);
    }
    private void SetSelectedItemData(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        _selectedItemData = (InventoryScrollRow.CellData)args[0];
        SetUp();
    }

    public void AdjustPerItemPrice(string priceText)
    {
        int.TryParse(priceText, out int newPrice);
        newPrice = Mathf.Clamp(newPrice, 0, int.MaxValue);
        _sellingForm.Pack.perItemPrice = newPrice;

        RefreshSellConfigDisplay();
    }

    public void AdjustPerItemPrice(int increase)
    {
        int newPrice = _sellingForm.Pack.perItemPrice + increase;
        _sellingForm.Pack.perItemPrice = Mathf.Clamp(newPrice, 0, int.MaxValue);
        RefreshSellConfigDisplay();
    }

    public void AdjustCount(string countText)
    {
        int.TryParse(countText, out int count);
        int maxCount = GetMaxValueOfSelectedItem();
        _sellingForm.Pack.count = Mathf.Clamp(count, 0, maxCount);

        RefreshSellConfigDisplay();
    }

    public void AdjustCount(int increase)
    {
        int count = _sellingForm.Pack.count + increase;
        _sellingForm.Pack.count = Mathf.Clamp(count, 0, GetMaxValueOfSelectedItem());

        RefreshSellConfigDisplay();
    }

    public void AdjustDuration(int dropdownIndex)
    {
        _sellingForm.Pack.remainTime = _durationConfigs[dropdownIndex];
    }

    public void ResetDisplay()
    {
        _sellingForm.ResetForm();
        _sellingForm.Pack.remainTime = _durationConfigs[0];

        RefreshSellConfigDisplay();
    }

    private void RefreshSellConfigDisplay()
    {
        var pack = _sellingForm.Pack;
        _perItemPrice.text = TextUtils.FormatNumber(pack.perItemPrice);
        _price.text = TextUtils.FormatNumber(pack.Price);
        _packItemCount.text = TextUtils.FormatNumber(pack.count);
        _netIncome.text = TextUtils.FormatNumber(pack.Price - (int)(pack.Price * pack.appliedTariff));
    }

    private void DisplayDefaultSellValue()
    {
        _perItemPrice.text = "0";
        _packItemCount.text = "0";
        _price.text = "0";
        _netIncome.text = "0";
        _durationDropdown.value = 0;
    }

    private void DisplayItemFromInventory(int itemId, RandomOption option)
    {
        _itemDisplayer.SetUp(new() { itemId = itemId, star = option.star, prefix = option.prefix });
        _itemDisplayer.SetItemCount(_userItems.GetItemCount(itemId, option));
    }

    private void DisplayItemFromCaravan(int itemId, RandomOption option)
    {
        _itemDisplayer.SetUp(new() { itemId = itemId, star = option.star, prefix = option.prefix });
        _itemDisplayer.SetItemCount(_caravans.GetItemCountWithOption(_selectedCaravan.Value, itemId, option));
    }

    private void DisplayMarketInfo()
    {
        _lastestPrice.text = "No Info";
        _lowestPrice.text = "No Info";
        _tariff.text = "No Info";
    }

    private int GetMaxValueOfSelectedItem()
    {
        int itemId = _selectedItemData.itemId;
        int star = _selectedItemData.star;
        int prefix = _selectedItemData.prefix;
        RandomOption option = new() { star = star, prefix = prefix };
        if (_selectedCaravan.Value == 0)
        {
            return _userItems.GetItemCount(itemId, option);
        }

        return _caravans.GetItemCountWithOption(_selectedCaravan.Value, itemId, option);
    }

    private void OnEnable()
    {
        _onSelectedItem.Subcribe(SetSelectedItemData);

        _durationDropdown.ClearOptions();
        for (int i = 0; i < _durationConfigs.Length; i++)
        {
            _durationDropdown.options.Add(new() { text = TimeUtils.FormatTime(_durationConfigs[i]) });
        }
    }

    private void OnDisable()
    {
        _onSelectedItem.Unsubcribe(SetSelectedItemData);
    }
}
