using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIconAndLabel : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private ItemAssetList _itemAssets;

    [Header("Configs")]
    [SerializeField]
    private Image _itemIcon;
    [SerializeField]
    private Image _itemGrade;
    [SerializeField]
    private TMP_Text _label;

    public int ItemId { get; private set; }

    public string Label
    {
        get
        {
            return _label.text;
        }
        set
        {
            _label.text = value;
        }
    }

    public void SetUp(int itemId, string label)
    {
        ItemId = itemId;
        _itemIcon.sprite = _itemAssets.GetItemSprite(itemId);
        _itemGrade.sprite = _itemAssets.GetItemGradeSprite(itemId);
        _label.text = label;
    }

    public void SetLabelColor(Color color)
    {
        _label.color = color;
    }
}
