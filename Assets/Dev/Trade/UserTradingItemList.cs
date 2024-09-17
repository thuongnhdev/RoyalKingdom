using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserTradingItemList", menuName = "Uniflow/Trading/UserTradingItemList")]
public class UserTradingItemList : ScriptableObject
{
    [SerializeField]
    private List<TradingItem> _tradingItems;
}
