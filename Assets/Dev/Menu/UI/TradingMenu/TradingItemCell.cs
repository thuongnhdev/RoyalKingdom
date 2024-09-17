using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingItemCell : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _selectedPackId;
    [SerializeField]
    private UserProfile _profile;

    [Header("Reference - Read")]
    [SerializeField]
    private ItemList _items;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onSelectedLandSellingItem;
    [SerializeField]
    private GameEvent _onSelectedCaravanSellingItem;

    [Header("Configs")]
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private TMP_Text _itemName;
    [SerializeField]
    private TMP_Text _price;
    [SerializeField]
    private TMP_Text _marketPrice;
    [SerializeField]
    private TMP_Text _remainTime;
    [SerializeField]
    private Sprite _lowerArrow;
    [SerializeField]
    private Sprite _higherArrow;
    [SerializeField]
    private Image _marketPriceArrow;
    [SerializeField]
    private InventoryItemCell _itemDisplayer;
    [SerializeField]
    private GameObject _soldOutGroup;
    [SerializeField]
    private GameObject _claimGroup;

    private TradingItem _referenceData;

    public void SetUp(TradingItem item)
    {
        if (item == null)
        {
            ActiveContent(false);
            return;
        }

        _referenceData = item;

        ActiveContent(true);
        _itemName.text = _items.GetItemName(item.itemId);
        bool lowerPrice = item.Price <= item.marketPrice;
        _marketPriceArrow.overrideSprite = lowerPrice ? _lowerArrow : _higherArrow;

        _marketPrice.text = TextUtils.FromRatioToPercent(Mathf.Abs(1f - (float)item.Price / item.marketPrice));
        _marketPrice.color = lowerPrice ? Color.green : Color.red;

        _price.text = TextUtils.FormatNumber(item.Price);

        ItemGrade grade = _items.GetItemGrade(item.itemId);
        _itemDisplayer.SetUp(new() 
        {
            itemId = item.itemId,
            star = item.star,
            prefix = item.prefix,
            grade = grade,
            itemCount = item.count
        });

        SetUpPackStatus(item);
    }

    public void ActiveContent(bool active)
    {
        _canvasGroup.alpha = active ? 1f : 0f;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;
    }

    public void ShowDetailTradingPopup()
    {
        if (_referenceData == null)
        {
            return;
        }

        bool isMyPack = _referenceData.seller == _profile.id;
        if (isMyPack)
        {
            ShowCaravanSellingItemDetail();
            return;
        }

        ShowLandSellingItemDetail();
    }
    private void ShowLandSellingItemDetail()
    {
        _selectedPackId.Value = _referenceData.packId;
        _onSelectedLandSellingItem.Raise();
    }
    private void ShowCaravanSellingItemDetail()
    {
        _selectedPackId.Value = _referenceData.packId;
        _onSelectedCaravanSellingItem.Raise();
    }

    private void SetUpPackStatus(TradingItem pack)
    {
        _soldOutGroup.SetActive(false);
        _claimGroup.SetActive(false);

        TradingItemStatus status = pack.status;
        if (status == TradingItemStatus.Selling)
        {
            PerFrame_CountRemainTime().Forget();
            return;
        }

        if (status != TradingItemStatus.Sold)
        {
            return;
        }

        if (pack.seller == _profile.id)
        {
            _claimGroup.SetActive(true);
            return;
        }

        _soldOutGroup.SetActive(true);
    }

    private CancellationTokenSource _updateToken;
    private async UniTaskVoid PerFrame_CountRemainTime()
    {
        if (_referenceData == null)
        {
            return;
        }

        _updateToken?.Cancel();
        _updateToken = new();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            _referenceData.remainTime -= Time.deltaTime * TimeUtils.TimeScale;
            if (_referenceData.remainTime < 0f)
            {
                _referenceData.remainTime = 0f;
                _referenceData.status = TradingItemStatus.Sold;
                _updateToken.Cancel();
                SetUpPackStatus(_referenceData);
                break;
            }
            _remainTime.text = TimeUtils.FormatTime(_referenceData.remainTime);
        }
    }
}
