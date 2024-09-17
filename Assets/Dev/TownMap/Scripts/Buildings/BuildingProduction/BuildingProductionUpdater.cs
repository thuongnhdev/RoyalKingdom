using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProductionUpdater : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingProductionList _userBuildingProductions;

    [Header("Config")]
    [SerializeField]
    private BuildingObjectFinder _buildingFinder;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onBuildingProductionDataChanged;

    public void In_InitBuildingProductionObject()
    {
        List<UserBuildingProduction> productionList = _userBuildingProductions.BuildingProductionList;
        for (int i = 0;  i < productionList.Count; i++)
        {
            UpdateBuildingObjectProduction(productionList[i].buildingObjectId);
        }
    }

    private void UpdateBuildingProduction(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)args[0];
        UpdateBuildingObjectProduction(buildingObjId);
    }

    private void UpdateBuildingObjectProduction(int buildingObjId)
    {
        GameObject buildingObj = _buildingFinder.GetBuildingByBuildingObjectId(buildingObjId);
        if (buildingObj == null)
        {
            return;
        }

        var buildingProductionInfo = _userBuildingProductions.GetBuildingProduction(buildingObjId);
        if (buildingProductionInfo == null)
        {
            return;
        }

        var buildingProduction = buildingObj.GetComponent<BuildingComponentGetter>().Production;
        buildingProduction.UpdateProductionInfo(buildingProductionInfo);
    }

    private void OnEnable()
    {
        _onBuildingProductionDataChanged.Subcribe(UpdateBuildingProduction);
    }

    private void OnDisable()
    {
        _onBuildingProductionDataChanged.Unsubcribe(UpdateBuildingProduction);
    }
}
