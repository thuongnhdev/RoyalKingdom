using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TradingItemPack", menuName = "Uniflow/Trading/TradingItemPack")]
public class ItemSellingForm : ScriptableObject
{
    [SerializeField]
    private TradingItem _pack;
    public TradingItem Pack => _pack;

    public void Set(TradingItem pack)
    {
        _pack = pack;
    }

    public void ResetForm()
    {
        if (_pack == null)
        {
            return;
        }

        Type type = typeof(TradingItem);
        var fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].SetValue(_pack, default);
        }
    }
}
