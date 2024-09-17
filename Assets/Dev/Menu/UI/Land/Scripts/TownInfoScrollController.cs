using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownInfoScrollController : BaseScrollController
{
    public void SetTowns(List<TownInLandInfo> towns)
    {
        _dataList.Clear();
        for (int i = 0; i < towns.Count; i++)
        {
            _dataList.Add(towns[i]);
        }

        ReloadScrollView();
    }
}
