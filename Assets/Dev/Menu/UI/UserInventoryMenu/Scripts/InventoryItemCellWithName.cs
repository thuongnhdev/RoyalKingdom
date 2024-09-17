using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItemCellWithName : InventoryItemCell
{
    [SerializeField]
    private TMP_Text _itemName;

    public void SetUp(InventoryScrollRow.CellData cell, string itemName)
    {
        SetUp(cell);
        _itemName.text = itemName;
    }
}
