using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuScrollController : BaseScrollController
{
    [Header("Reference - Read")]
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private ItemTypeList _itemTypes;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _showItemDetail;
    [SerializeField]
    private GameEvent _onNoItemToShow;

    [Header("Configs")]
    [SerializeField]
    private int _itemPerRow = 6;

    private List<InventoryScrollRow.CellData> _currentCells = new();

    public override void AdjustItemPerRow(int itemPerRow)
    {
        _itemPerRow = itemPerRow;
    }

    public void PrepareDataAndShowAll(List<ResourcePoco> items, Dictionary<int, ItemWithRandomOption> randomOptions)
    {
        _currentCells = PrepareCellsData(items, randomOptions);

        RefreshView(0);
    }

    public void ApplyFilter(int filterWarehouse)
    {
        RefreshView(filterWarehouse);
    }

    public void FocusFirstItem()
    {
        if (_dataList.Count == 0)
        {
            return;
        }

        if (_showItemDetail == null)
        {
            return;
        }

        _showItemDetail.Raise(((InventoryScrollRow.CellData[])_dataList[0])[0]);
    }

    private void RefreshView(int filterWarehouse)
    {
        _dataList.Clear();

        var row = new InventoryScrollRow.CellData[_itemPerRow];
        int cellIndex = -1;
        for (int i = 0; i < _currentCells.Count; i++)
        {
            var cell = _currentCells[i];
            ItemType type = _items.GetItemType(cell.itemId);

            if (!IsCellShown(type, filterWarehouse))
            {
                continue;
            }

            cellIndex++;
            cell.cellId = cellIndex;

            int indexInRow = cellIndex % _itemPerRow;
            if (indexInRow == 0 && cellIndex > 0)
            {
                _dataList.Add(row);
                row = new InventoryScrollRow.CellData[_itemPerRow];
            }

            row[indexInRow] = _currentCells[i];
        }

        if (cellIndex == -1)
        {
            ReloadScrollView();
            if (_onNoItemToShow != null)
            {
                _onNoItemToShow.Raise();
            }
            return;
        }

        if (cellIndex + 1 % _itemPerRow != 0)
        {
            _dataList.Add(row);
        }

        ReloadScrollView();
        FocusFirstItem();
    }

    private bool IsCellShown(ItemType itemType, int filterWarehouse)
    {
        if (filterWarehouse == 0)
        {
            return true;
        }

        int containWarehouse = _itemTypes.GetContainedWarehouse(itemType);
        return containWarehouse == filterWarehouse;
    }

    private List<InventoryScrollRow.CellData> PrepareCellsData(List<ResourcePoco> userItems, Dictionary<int, ItemWithRandomOption> randomOptionItems)
    {
        List<InventoryScrollRow.CellData> cellsData = new();
        if (userItems == null || userItems.Count == 0)
        {
            return cellsData;
        }

        for (int i = 0; i < userItems.Count; i++)
        {
            var item = userItems[i];
            randomOptionItems.TryGetValue(item.itemId, out var randomOptionItem);
            List<RandomOption> randomOption = new();
            if (randomOptionItem != null)
            {
                randomOption = randomOptionItem.randomOptions;
            }

            if (randomOption == null || randomOption.Count == 0)
            {
                cellsData.AddRange(CreateCellsForItems(item, RandomOption.NoAddition));
                continue;
            }

            var itemApCells = ProcessAPItem(item.itemId, randomOption);
            cellsData.AddRange(itemApCells);
            item.itemCount -= randomOption.Count;

            if (item.itemCount == 0)
            {
                continue;
            }

            cellsData.AddRange(CreateCellsForItems(item, RandomOption.NoAddition));
        }

        return cellsData;
    }

    private List<InventoryScrollRow.CellData> ProcessAPItem(int itemId, List<RandomOption> additionalProps)
    {
        List<InventoryScrollRow.CellData> apCells = new();
        for (int i = 0; i < additionalProps.Count; i++)
        {
            var ap = additionalProps[i];
            if (CheckExistingAndMerge(apCells, ap))
            {
                continue;
            }

            apCells.Add(new()
            {
                itemId = itemId,
                itemCount = 1,
                star = ap.star,
                prefix = ap.prefix,
                grade = _items.GetItemGrade(itemId)
            });
        }

        List<InventoryScrollRow.CellData> cellsDataAfterProcess = new();
        for (int i = 0; i < apCells.Count; i++)
        {
            var cell = apCells[i];
            cellsDataAfterProcess.AddRange(CreateCellsForItems(
                new ResourcePoco() { itemId = cell.itemId, itemCount = cell.itemCount },
                new RandomOption() { star = cell.star, prefix = cell.prefix}));
        }

        return cellsDataAfterProcess;
    }
    private bool CheckExistingAndMerge(List<InventoryScrollRow.CellData> cells, RandomOption checkedAP)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            if (cell.prefix == checkedAP.prefix && 
                cell.star == checkedAP.star)
            {
                cell.itemCount++;
                return true;
            }
        }

        return false;
    }

    private List<InventoryScrollRow.CellData> CreateCellsForItems(ResourcePoco items, RandomOption ap)
    {
        ItemType itemType = _items.GetItemType(items.itemId);
        int slotLimit = _itemTypes.GetSlotLimit(itemType);
        int cellCount = slotLimit == 0 ? 1 : Mathf.CeilToInt((float)(items.itemCount) / slotLimit);

        List<InventoryScrollRow.CellData> cells = new();
        for (int i = 0; i < cellCount; i++)
        {
            cells.Add(new()
            {
                itemId = items.itemId,
                itemCount = (i == cellCount - 1) ? items.itemCount - i * slotLimit : slotLimit,
                star = ap.star,
                prefix = ap.prefix,
                grade = _items.GetItemGrade(items.itemId)
            });
        }

        cells.Sort(Comparer);
        return cells;
    }

    private int Comparer(InventoryScrollRow.CellData a, InventoryScrollRow.CellData b)
    {
        if (a.grade < b.grade)
        {
            return -1;
        }
        if (a.grade > b.grade)
        {
            return 1;
        }

        if (a.star < b.star)
        {
            return -1;
        }
        if (a.star > b.star)
        {
            return 1;
        }

        if (a.prefix < b.prefix)
        {
            return -1;
        }
        if (a.prefix > b.prefix)
        {
            return 1;
        }

        return 0;
    }
}
