using Cysharp.Threading.Tasks;
using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingSynchronizer : MonoBehaviour
{
    [Header("Referencen - Read/Write")]
    [SerializeField]
    private LandTradingItemList _landTradingItems;
    [SerializeField]
    private CaravanList _caravans;
    [SerializeField]
    private UserItemStorage _inventory;

    [Header("Reference - Read")]
    [SerializeField]
    private ApiList _apis;
    [SerializeField]
    private ItemSellingForm _sellingForm;
    [SerializeField]
    private IntegerVariable _selectedPack;
    [SerializeField]
    private IntegerVariable _selectedLand;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _requestFetchLandTradingItems;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onLandTradingItemsFetched;
    [SerializeField]
    private GameEvent _onLandTradingItemsChanged;

    public void RequestSellItem()
    {
        ServerSync_SellItem().Forget();
    }
    private async UniTaskVoid ServerSync_SellItem()
    {
        byte[] sellRequestBory = TradingNetworkHelper.CreateSellPackRequestBody(_sellingForm.Pack);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeSetItemPack>(_apis.SellAPack, sellRequestBory, blockUi: true);
        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to sell items");
            return;
        }

        var sellingPack = _sellingForm.Pack;
        _caravans.SubItem(sellingPack.caravanId,
            new ResourcePoco { itemId = sellingPack.itemId, itemCount = sellingPack.count },
            new RandomOption { star = sellingPack.star, prefix = sellingPack.prefix });
        _landTradingItems.AddTradingItem(new(sellingPack));

        _onLandTradingItemsChanged.Raise();
    }

    public void RequestBuyItem()
    {
        int packId = _selectedPack.Value;
        if (packId == 0)
        {
            return;
        }

        ServerSync_BuyItem().Forget();
    }
    private async UniTaskVoid ServerSync_BuyItem()
    {
        byte[] requestBody = TradingNetworkHelper.CreateOneIntRequestBody(_selectedPack.Value);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeBuyItemPack>(_apis.BuyAPack, requestBody, blockUi: true);

        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError($"Failed to buy an itemPack with id [{_selectedPack.Value}]");
            return;
        }

        var sellingPack = _landTradingItems.GetTradingItem(_selectedPack.Value);
        int caravanId = sellingPack.caravanId;
        var caravan = _caravans.GetCaravan(caravanId);
        if (caravan == null)
        {
            return;
        }

        _caravans.AddItem(caravan.id,
            new ResourcePoco { itemId = sellingPack.itemId, itemCount = sellingPack.count },
            new RandomOption { star = sellingPack.star, prefix = sellingPack.prefix });

        _landTradingItems.ChangePackStatus(sellingPack.packId, TradingItemStatus.Sold);
        _onLandTradingItemsChanged.Raise();

        _inventory.Sub(new() { itemId = sellingPack.paymentItemId, itemCount = sellingPack.Price });
    }

    public void RequestClaimIncome()
    {
        int packId = _selectedPack.Value;
        if (packId == 0)
        {
            return;
        }
    }
    private async UniTaskVoid ServerSync_ClaimIncome()
    {
        // Server logic here
        var sellingPack = _landTradingItems.GetTradingItem(_selectedPack.Value);
        int caravanId = sellingPack.caravanId;
        var caravan = _caravans.GetCaravan(caravanId);
        if (caravan == null)
        {
            return;
        }

        int paymentItem = sellingPack.paymentItemId == 0 ? ResourcePoco.GOLD : sellingPack.paymentItemId;
        _caravans.AddItem(caravan.id,
            new ResourcePoco { itemId = paymentItem, itemCount = sellingPack.Price });

        _landTradingItems.ChangePackStatus(sellingPack.packId, TradingItemStatus.Claimed);

        _onLandTradingItemsChanged.Raise();
    }

    public void FetchUserCaravans()
    {

    }
    private async UniTaskVoid ServerSync_GetUserCaravans()
    {

    }

    public void FetchLandCaravans()
    {

    }
    private async UniTaskVoid ServerSync_GetLandCaravans()
    {
        byte[] requestBody = TradingNetworkHelper.CreateOneIntRequestBody(_selectedLand.Value);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeGetInfoCaravan>(_apis.GetLandCaravans, requestBody);
        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError($"Failed to fetch land [{_selectedLand.Value}] caravans");
            return;
        }
    }

    private void FetchLandTradingItems(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int landId = (int)args[0];
        ServerSync_FetchLandTradingItems(landId).Forget();
    }
    private async UniTaskVoid ServerSync_FetchLandTradingItems(int landId)
    {
        
    }


    private void OnEnable()
    {
        _requestFetchLandTradingItems.Subcribe(FetchLandTradingItems);
    }

    private void OnDisable()
    {
        _requestFetchLandTradingItems.Unsubcribe(FetchLandTradingItems);
    }


}
