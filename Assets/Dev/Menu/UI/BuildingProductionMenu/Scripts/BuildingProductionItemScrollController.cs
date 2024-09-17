using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProductionItemScrollController : BaseScrollController
{
    [SerializeField]
    private ItemAssetList _itemAssetList;

    public void ChangeDisplayData(List<int> products)
    {
        _dataList.Clear();
        if (products == null)
        {
            return;
        }

        for (int i = 0; i < products.Count; i++)
        {
            var itemAsset = _itemAssetList.GetItemAssets(products[i]);
            _dataList.Add(itemAsset);
        }

        ReloadScrollView();
    }
}
