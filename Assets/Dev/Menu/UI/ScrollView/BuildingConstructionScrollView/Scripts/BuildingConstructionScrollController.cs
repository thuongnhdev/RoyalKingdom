using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConstructionScrollController : BaseScrollController
{
    public void ChangeDisplayData(List<BaseBuildingPoco> newBuildings)
    {
        _dataList.Clear();
        for (int i = 0; i < newBuildings.Count; i++)
        {
            _dataList.Add(newBuildings[i]);
        }

        ReloadScrollView();
    }
}
