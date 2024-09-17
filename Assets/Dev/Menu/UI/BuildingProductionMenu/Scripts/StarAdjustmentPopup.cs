using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarAdjustmentPopup : MonoBehaviour
{
    [Header("Reference - Read/Write")]
    [SerializeField]
    private ProductionMaterialsStar _materialStars;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _currentCostCount;
    [SerializeField]
    private IntegerVariable _currentCostItemId;
    [SerializeField]
    private Vector3Variable _starAdjustmentPopupPos;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private ItemList _items;

    [Header("Configs")]
    [SerializeField]
    private Transform _adjustStarGroup;
    [SerializeField]
    private ResourceIconAndLabel[] _starOverCostObjs;
    [SerializeField]
    private Slider[] _starSliders;
    [SerializeField]
    private TMP_Text[] _itemNames;

    private ResourcePoco _bufferMat = new();

    public void SetUp()
    {
        _adjustStarGroup.position = _starAdjustmentPopupPos.Value;

        string itemName = _items.GetItemName(_currentCostItemId.Value);
        for (int i = 0; i < _itemNames.Length; i++)
        {
            _itemNames[i].text = TextUtils.RichTextHeavy($"Lv. {i} {itemName}");
        }
        SetUpStarSliders();
    }

    private void SetUpStarSliders()
    {
        int itemId = _currentCostItemId.Value;
        int itemCost = _currentCostCount.Value;

        int[] stars = _materialStars.GetStars(_currentCostItemId.Value);
        if (stars.Length == 0)
        {
            _starOverCostObjs[0].SetUp(itemId, TextUtils.RichTextHeavy($"{itemCost}/{itemCost}"));
            Slider oneStarSlider = _starSliders[0];
            oneStarSlider.maxValue = itemCost;
            oneStarSlider.value = itemCost;
            for (int i = 1; i < _starOverCostObjs.Length; i++)
            {
                _starSliders[i].maxValue = itemCost;
                _starSliders[i].value = 0;
                _starOverCostObjs[i].SetUp(itemId, TextUtils.RichTextHeavy($"0/{itemCost}"));
            }

            return;
        }

        int star1 = itemCost;
        for (int i = 1; i < _starOverCostObjs.Length; i++)
        {
            star1 -= stars[i];
            _starOverCostObjs[i].SetUp(itemId, TextUtils.RichTextHeavy($"{stars[i]}/{itemCost}"));
            _starSliders[i].SetValueWithoutNotify(stars[i]);
        }
        _starOverCostObjs[0].SetUp(itemId, TextUtils.RichTextHeavy($"{star1}/{itemCost}"));
        _starSliders[0].SetValueWithoutNotify(star1);
    }

    public void AdjustStarSlider(int sliderIndex)
    {
        _bufferMat.itemId = _currentCostItemId.Value;
        _bufferMat.itemCount = _currentCostCount.Value;

        int star = sliderIndex + 1;
        _materialStars.UpdateMaterial(_bufferMat, star, (int)_starSliders[sliderIndex].value);
        RefreshStarValue(star);
    }

    private void RefreshStarValue(int star)
    {
        int index = star - 1;
        int starCount = _materialStars.GetStar(_currentCostItemId.Value, star);
        int itemCost = _currentCostCount.Value;

        _starSliders[index].SetValueWithoutNotify(starCount);
        _starOverCostObjs[index].Label = TextUtils.RichTextHeavy($"{starCount}/{itemCost}");

        int oneStarCount = _materialStars.GetStar(_currentCostItemId.Value, 1);
        _starSliders[0].SetValueWithoutNotify(oneStarCount);
        _starOverCostObjs[0].Label = TextUtils.RichTextHeavy($"{oneStarCount}/{itemCost}");
    }
}
