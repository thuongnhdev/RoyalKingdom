using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingNetworkHelper
{
    public static byte[] CreateSellPackRequestBody(TradingItem pack)
    {
        FlatBufferBuilder builder = new(1);
        RequestSetItempack.StartRequestSetItempack(builder);

        var itemPackOffset = CreateItemPackOffset(builder, pack);
        RequestSetItempack.AddRequestItemPack(builder, itemPackOffset);

        var offset = RequestSetItempack.EndRequestSetItempack(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    private static Offset<ItemPack> CreateItemPackOffset(FlatBufferBuilder builder, TradingItem pack)
    {
        ItemPack.StartItemPack(builder);

        ItemPack.AddIdPack(builder, pack.packId);
        ItemPack.AddIdItem(builder, pack.itemId);
        ItemPack.AddCount(builder, pack.count);
        ItemPack.AddPrefix(builder, pack.prefix);
        ItemPack.AddStar(builder, pack.star);
        ItemPack.AddPrice(builder, pack.Price);
        ItemPack.AddMarketPrice(builder, pack.marketPrice);
        ItemPack.AddAppliedTax(builder, pack.appliedTariff);
        ItemPack.AddSeller(builder, pack.seller);
        ItemPack.AddIdCaraval(builder, pack.caravanId);
        ItemPack.AddRemainTime(builder, (long)pack.remainTime);
        ItemPack.AddStatus(builder, (int)pack.status);
        ItemPack.AddPaymentUnit(builder, pack.paymentItemId);

        return ItemPack.EndItemPack(builder);
    }

    public static byte[] CreateOneIntRequestBody(int packId)
    {
        FlatBufferBuilder builder = new(1);

        RequestBuyItemPack.StartRequestBuyItemPack(builder);
        RequestBuyItemPack.AddIdPack(builder, packId);
        var offset = RequestBuyItemPack.EndRequestBuyItemPack(builder);

        builder.Finish(offset.Value);
        return builder.SizedByteArray();
    }
}
