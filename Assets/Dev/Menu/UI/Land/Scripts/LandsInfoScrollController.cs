using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandsInfoScrollController : BaseScrollController
{
    public void SetData(List<int> landIds)
    {
        _dataList.Clear();
        for (int i = 0; i < landIds.Count; i++)
        {
            _dataList.Add(landIds[i]);
        }

        ReloadScrollView();
    }
}
