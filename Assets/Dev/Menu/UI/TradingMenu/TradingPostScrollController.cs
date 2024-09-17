using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingPostScrollController : BaseScrollController
{
    [SerializeField]
    private int _itemPerRow = 3;

    public void RefreshView(List<TradingItem> items)
    {
        _dataList.Clear();

        TradingItem[] rowData = new TradingItem[_itemPerRow];
        for (int i = 0; i < items.Count; i++)
        {
            rowData[i % _itemPerRow] = items[i];

            if ((i + 1) % _itemPerRow == 0 && 0 < i)
            {
                _dataList.Add(rowData);
                rowData = new TradingItem[_itemPerRow];
            }
        }

        if (items.Count % _itemPerRow != 0)
        {
            _dataList.Add(rowData);
        }

        ReloadScrollView();
    }

    public override void AdjustItemPerRow(int itemPerRow)
    {
        _itemPerRow = itemPerRow;
    }
}
