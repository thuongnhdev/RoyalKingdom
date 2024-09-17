using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TutorialScriptHelper : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private UserBuildingProductionList _userProductions;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Configs")]
    [SerializeField]
    private float _defaultBuildingBoostRate = 60f;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _updateBuildingCommand;
    [SerializeField]
    private GameEvent _updateProductionCommand;

    public void ChangeStatusOfRecentlyAddedBuilding(int status)
    {
        var building = _userBuildings.BuildingList[^1];
        building.status = (BuildingStatus)status;

        _updateBuildingCommand.Raise(building);
    }

    public void OpenBuildingContextMenu(int buildingId)
    {
        List<int> buildings = _userBuildings.GetAllBuildingsOfId(buildingId);
        if (buildings.Count == 0)
        {
            return;
        }

        GameObject buildingObj = BuildingObjectFinder.Instance.GetBuildingByBuildingObjectId(buildings[0]);
        _selectedBuildingObjId.Value = buildings[0];
        buildingObj.GetComponent<BuildingComponentGetter>().BuildingObjectUI.In_OpenOrCloseBasedOnObjId();
    }

    public void BoostConstructionRateOfRecentlyAddedBuilding(float rate)
    {
        var building = _userBuildings.BuildingList[^1];
        building.currentContructionRate = rate;

        _updateBuildingCommand.Raise(building);

        TrackBuildProgressAndKeepBoosting(building, rate).Forget();
    }

    public void BoostProductRateOfRecentlyAddedBuilding(float rate)
    {
        var production = _userProductions.BuildingProductionList[^1];
        production.productRate = rate;

        _updateProductionCommand.Raise(production);

        TrackProductProgressAndKeepBoosting(production, rate).Forget();
    }

    public void BoostProductRateOfBuilding(int buildingId)
    {
        List<int> buildings = _userBuildings.GetAllBuildingsOfId(buildingId);
        if (buildings.Count == 0)
        {
            Logger.LogWarning($"Failed to Boost building [{buildingId}]");
            return;
        }

        var production = _userProductions.GetBuildingProduction(buildings[0]);
        if (production == null)
        {
            return;
        }

        production.productRate = _defaultBuildingBoostRate;
        _updateProductionCommand.Raise(production);

        TrackProductProgressAndKeepBoosting(production, _defaultBuildingBoostRate).Forget();
    }

    private CancellationTokenSource _buildToken;
    private async UniTaskVoid TrackBuildProgressAndKeepBoosting(UserBuilding building, float rate)
    {
        _buildToken?.Cancel();
        _buildToken = new();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_buildToken.Token))
        {
            if (building == null)
            {
                _buildToken.Cancel();
                break;
            }

            if (building.status != BuildingStatus.WaitForConstructingResource && building.status != BuildingStatus.OnConstruction)
            {
                _buildToken.Cancel();
                break;
            }

            if (building.currentContructionRate == rate)
            {
                continue;
            }

            building.currentContructionRate = rate;
            _updateBuildingCommand.Raise(building);
        }
    }

    private CancellationTokenSource _productToken;
    private async UniTaskVoid TrackProductProgressAndKeepBoosting(UserBuildingProduction production, float rate)
    {
        _productToken?.Cancel();
        _productToken = new();
        var building = _userBuildings.GetBuilding(production.buildingObjectId);
        if (building == null)
        {
            return;
        }

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_productToken.Token))
        {
            if (building == null)
            {
                _productToken.Cancel();
                break;
            }

            if (building.status != BuildingStatus.WaitingForProductResource && building.status != BuildingStatus.Producing)
            {
                _productToken.Cancel();
                break;
            }

            if (production.productRate == rate)
            {
                continue;
            }

            production.productRate = rate;
            _updateProductionCommand.Raise(production);
        }
    }
}
