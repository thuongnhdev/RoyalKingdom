using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellItemPopup : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private ItemSellingForm _sellingForm;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedCaravan;
    [SerializeField]
    private IntegerListVariable _selectedItemInfo;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private CaravanList _caravans;

    [Header("Configs")]
    [SerializeField]
    private InventoryItemCellWithName _itemDisplayer;
    [SerializeField]
    private TMP_Text _latestPrice;
    [SerializeField]
    private TMP_Text _lowestPrice;
    [SerializeField]
    private TMP_InputField _perItemPrice;
    [SerializeField]
    private TMP_InputField _count;
    [SerializeField]
    private TMP_Text _price;
    [SerializeField]
    private TMP_Dropdown _duration;
    [SerializeField]
    private TMP_Text _tariff;
    [SerializeField]
    private int[] _durationConfigs;

    private int _inStorage = 0;

    public void SetUp()
    {
        int itemId = _selectedItemInfo[0];
        int star = _selectedItemInfo[1];
        int prefix = _selectedItemInfo[2];
        int count = _selectedItemInfo[3];

        _inStorage = count;

        _itemDisplayer.SetUp(new()
        {
            itemId = itemId,
            star = star,
            prefix = prefix,
            itemCount = count
        });

        InitPackDefaultValue();
        DisplayMarketInfo();
    }

    public void IncreaseCount(int increase)
    {
        var pack = _sellingForm.Pack;
        int currentCount = pack.count + increase;
        RefreshCount(currentCount);
    }

    public void ChangeCount(string countText)
    {
        int.TryParse(countText, out int count);
        RefreshCount(count);
    }

    public void ChangeDuration(int selection)
    {
        int duration = _durationConfigs[selection % _durationConfigs.Length];
        _duration.captionText.text = TimeUtils.FormatTime(duration);
        _sellingForm.Pack.remainTime = duration;
    }

    private void RefreshCount(int newValue)
    {
        var pack = _sellingForm.Pack;
        pack.count = Mathf.Clamp(newValue, 0, _inStorage);
        _count.text = TextUtils.FormatNumber(pack.count);
    }

    private void InitPackDefaultValue()
    {
        _perItemPrice.text = "0";
        _count.text = "0";
        _price.text = "0";
        _duration.captionText.text = "";
    }

    private void DisplayMarketInfo()
    {
        _latestPrice.text = "No Info";
        _lowestPrice.text = "No Info";
    }
}
