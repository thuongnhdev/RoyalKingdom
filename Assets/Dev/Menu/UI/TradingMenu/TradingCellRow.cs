using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingCellRow : BaseCustomCellView
{
    [SerializeField]
    private TradingItemCell[] _cells;

    public override void SetUp(object data)
    {
        var items = (TradingItem[])data;
        DeactiveUnusedCell(_cells.Length - items.Length);

        for (int i = 0; i < items.Length; i++)
        {
            var cell = _cells[i];
            cell.SetUp(items[i]);
        }
    }

    private void DeactiveUnusedCell(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _cells[^(i + 1)].gameObject.SetActive(false);
        }
    }
}
