using System.Collections.Generic;

[System.Serializable]
public class Caravan
{
    public int id;
    public long owner;
    public int locationLand;
    public List<int> packs = new();
    public List<ResourcePoco> storages = new();
    public List<ItemWithRandomOption> storageRandomOptions = new();
}

[System.Serializable]
public class TradingItem
{
    public int packId;
    public int sellingLand;
    public int itemId;
    public int prefix;
    public int star;
    public int count;
    public int perItemPrice;
    public int paymentItemId;
    public int Price 
    {
        get
        {
            return count * perItemPrice;
        }
    }
    public int marketPrice;
    public float appliedTariff;
    public long seller;
    public int caravanId;
    public float remainTime;
    public TradingItemStatus status;

    public TradingItem() { }
    public TradingItem(TradingItem item)
    {
        packId = item.packId;
        sellingLand = item.sellingLand;
        itemId = item.itemId;
        prefix = item.prefix;
        star = item.star;
        count = item.count;
        perItemPrice = item.count;
        perItemPrice = item.perItemPrice;
        paymentItemId = item.paymentItemId;
        marketPrice = item.marketPrice;
        appliedTariff = item.appliedTariff;
        seller = item.seller;
        caravanId = item.caravanId;
        remainTime = item.remainTime;
        status = item.status;
    }
}

public enum TradingItemStatus
{
    None = -1,
    Selling = 0,
    Sold = 1,
    Expired = 2,
    Claimed = 3
}