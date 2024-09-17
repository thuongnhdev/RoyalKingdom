using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScrollRow : BaseCustomCellView
{
    [SerializeField]
    private List<InventoryItemCell> _cells;
    public class CellData
    {
        public int cellId;
        public int itemId;
        public int itemCount;
        public int prefix;
        public int star;
        public ItemGrade grade;
    }

    public override void SetUp(object data)
    {
        var row = (CellData[])data;
        AdjustPerRowItem(row.Length);

        for (int i = 0; i < _cells.Count; i++)
        {
            if (row.Length - 1 < i)
            {
                _cells[i].SetUp(null);
                continue;
            }
            _cells[i].SetUp(row[i]);
        }
    }

    private void AdjustPerRowItem(int dataPerRowItem)
    {
        int remove = _cells.Count - dataPerRowItem;
        if (remove <= 0)
        {
            return;
        }

        for (int i = 0; i < remove; i++)
        {
            _cells[^1].gameObject.SetActive(false);
            _cells.RemoveAt(_cells.Count - 1);
        }
    }
}
