using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemScrollCell : BaseCustomCellView
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _selectedProductId;

    [Header("Reference - Read")]
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private StarAndGradeAssetList _starsAndGrades;

    [Header("Config")]
    [SerializeField]
    private Image _grade;
    [SerializeField]
    private Image _itemImage;

    private int _itemId;

    public override void SetUp(object data)
    {
        var itemData = (ItemAssetPoco)data;
        _itemId = itemData.itemId;
        _itemImage.sprite = itemData.sprite;
        _grade.sprite = _starsAndGrades.GetGradeIcon(_items.GetItemGrade(_itemId));
    }

    public void SelectProduct()
    {
        _selectedProductId.Value = _itemId;
    }
}
