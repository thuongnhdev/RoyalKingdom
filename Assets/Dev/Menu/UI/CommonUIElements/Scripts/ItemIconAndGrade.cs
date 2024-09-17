using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemIconAndGrade : MonoBehaviour
{
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private Image _itemIcon;
    [SerializeField]
    private Image _itemGrade;
    [SerializeField]
    private UnityEvent _onSetUpDisplay;
    [SerializeField]
    private UnityEvent _onResetDisplay;
    
    public void SetUp(Sprite itemIcon, Sprite itemGrade)
    {
        _itemIcon.enabled = true;
        _itemIcon.overrideSprite = itemIcon;
        _itemGrade.overrideSprite = itemGrade;
        _onSetUpDisplay.Invoke();
    }

    public void SetUp(int itemId)
    {
        if (_itemAssets == null)
        {
            Logger.LogError("Item Assets list is not assigned!");
            return;
        }
        _itemIcon.enabled = true;
        _itemIcon.overrideSprite = _itemAssets.GetItemSprite(itemId);
        _itemGrade.overrideSprite = _itemAssets.GetItemGradeSprite(itemId);
    }

    public void ResetDisplay()
    {
        _itemIcon.enabled = false;
        _itemGrade.overrideSprite = null;
        _onResetDisplay.Invoke();
    }
}
