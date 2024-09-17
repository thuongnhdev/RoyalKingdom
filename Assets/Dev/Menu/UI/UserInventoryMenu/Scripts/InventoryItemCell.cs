using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemCell : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private PrefixAssetList _prefixAssets;
    [SerializeField]
    private StarAndGradeAssetList _starAndGradeIcons;
    [SerializeField]
    private IntegerVariable _selectedInventoryCellId;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onOpenedDetailPopup;

    [Header("Configs")]
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private Image _selectedFrame;
    [SerializeField]
    private Image _grade;
    [SerializeField]
    private Image _itemIcon;
    [SerializeField]
    private Image _prefix;
    [SerializeField]
    private Image _prefixBG;
    [SerializeField]
    private Image _star;
    [SerializeField]
    private Image _starBG;

    [SerializeField]
    private TMP_Text _itemCount;
    [SerializeField]
    private TMP_Text _itemCountForStarItem;

    private InventoryScrollRow.CellData _currentData;

    public void SetUp(InventoryScrollRow.CellData data)
    {
        Deactivate();

        if (data == null)
        {
            return;
        }

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        _currentData = data;

        _itemIcon.overrideSprite = _itemAssets.GetItemSprite(data.itemId);
        _grade.overrideSprite = _itemAssets.GetItemGradeWithStarSlot(data.itemId);

        bool isStarItem = data.star != 0;
        _star.enabled = isStarItem;
        if (_starBG != null)
        {
            _starBG.enabled = isStarItem;
            _starBG.overrideSprite = _itemAssets.GetItemStarBG(data.itemId);
            _grade.overrideSprite = _itemAssets.GetItemGradeSprite(data.itemId);
        }

        if (isStarItem)
        {
            _star.overrideSprite = _starAndGradeIcons.GetStarIcon(data.star);
        }

        _itemCount.enabled = !isStarItem;
        _itemCountForStarItem.enabled = isStarItem;
        _itemCount.text = data.itemCount.ToString();
        _itemCountForStarItem.text = data.itemCount.ToString();

        if (data.prefix != 0)
        {
            _prefix.enabled = true;
            _prefixBG.enabled = true;
            _prefix.overrideSprite = _prefixAssets.GetPrefixIcon(data.prefix);
        }

        if (_selectedFrame != null)
        {
            _selectedFrame.enabled = _selectedInventoryCellId.Value == _currentData.cellId;
        }
    }

    public void SetItemCount(int itemCount)
    {
        _itemCount.text = itemCount.ToString();
        _itemCountForStarItem.text = itemCount.ToString();
    }

    public void SelectItem()
    {
        _selectedInventoryCellId.Value = _currentData.cellId;
        _onOpenedDetailPopup.Raise(_currentData);
    }

    private void UpdateSeletectedFrame(int selectedCellId)
    {
        if (_selectedFrame == null || _currentData == null)
        {
            return;
        }

        _selectedFrame.enabled = selectedCellId == _currentData.cellId;
    }

    private void Deactivate()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        _prefix.enabled = false;
        _prefixBG.enabled = false;

        if (_selectedFrame != null)
        {
            _selectedFrame.enabled = false;
        }

        _star.enabled = false;
        _itemCount.enabled = false;
        _itemCountForStarItem.enableAutoSizing = false;
    }

    private void OnEnable()
    {
        _selectedInventoryCellId.OnValueChange += UpdateSeletectedFrame;
    }

    private void OnDisable()
    {
        _selectedInventoryCellId.OnValueChange -= UpdateSeletectedFrame;
    }
}
