using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LandTradingItemList", menuName = "Uniflow/Trading/LandTradingItemList")]
public class LandTradingItemList : ScriptableObject
{
    [SerializeField]
    private List<TradingItem> _tradingItems;
    public List<TradingItem> TradingItems => _tradingItems;
    // key = packId
    private Dictionary<int, TradingItem> _tradingItemDict = new();
    private Dictionary<int, TradingItem> TradingItemDict
    {
        get
        {
            if (_tradingItemDict.Count != _tradingItems.Count)
            {
                _tradingItemDict.Clear();
                for (int i = 0; i < _tradingItems.Count; i++)
                {
                    var item = _tradingItems[i];
                    _tradingItemDict.Add(item.packId, item);
                }
            }

            return _tradingItemDict;
        }
    }

    public void Init()
    {

    }

    public TradingItem GetTradingItem(int packId, bool friendlyWarning = false)
    {
        TradingItemDict.TryGetValue(packId, out var item);
        if (item == null)
        {
            if (friendlyWarning)
            {
                Logger.Log($"Pack withId [{packId}] is not registered", Color.green);
            }
            else
            {
                Logger.LogError($"Invalid packId [{packId}]");
            }
        }

        return item;
    }

    public void AddTradingItem(TradingItem pack)
    {
        if (pack.packId == 0)
        {
            return;
        }
        if (TradingItemDict.ContainsKey(pack.packId))
        {
            return;
        }

        _tradingItems.Add(pack);
        _tradingItemDict.Add(pack.packId, pack);
    }

    public void ChangePackStatus(int packId, TradingItemStatus status)
    {
        if (packId == 0)
        {
            return;
        }
        var pack = GetTradingItem(packId);
        if (pack == null)
        {
            return;
        }

        pack.status = status;
    }
}
