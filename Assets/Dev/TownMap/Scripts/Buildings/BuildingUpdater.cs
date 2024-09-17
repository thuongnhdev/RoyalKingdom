using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingUpdater : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private UserBuildingList _userBuildingList;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onAddedNewBuilding;
    [SerializeField]
    private GameEvent _onBuildingDataUpdated;
    [SerializeField]
    private GameEvent _onDestroyedABuildingData;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onAllOperationDataInit;
    [SerializeField]
    private GameEvent _onAskedAStarRescan;

    [Header("Config")]
    [SerializeField]
    private BuildingObjectFinder _buildingFinder;

    public void In_InitUserBuildingObject()
    {
        Delay_InitUserBuildingObjects().Forget();
    }

    private async UniTaskVoid Delay_InitUserBuildingObjects()
    {
        await UniTask.NextFrame(); // Wait for building objects are spawned

        List<UserBuilding> buildings = _userBuildingList.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            UpdateBuildingOperationAndTransform(buildings[i]);
        }

        _onAllOperationDataInit.Raise();
    }

    public void UpdateBuildingOperationAndTransform(UserBuilding buildingInfo)
    {
        var building = _buildingFinder.GetBuildingObjectByLocation(buildingInfo.locationTileId);
        if (building == null)
        {
            return;
        }

        var compGetter = building.GetComponent<BuildingComponentGetter>();
        var operation = compGetter.Operation;
        operation.UpdateInfo(buildingInfo);

        var tilingTransform = compGetter.TilingTransform;
        Vector2 buildingSize = _buildings.GetBuildingSize(buildingInfo.buildingId);
        tilingTransform.SetObjectSize((int)buildingSize.x, (int)buildingSize.y);
        tilingTransform.RotateTo(buildingInfo.rotation);

        _buildingFinder.AddBuildingByObjectId(operation.BuildingObjId, building);
    }

    public void RotateBuilding()
    {
        int buildingLocation = _userBuildingList.GetBuildingLocation(_selectedBuildingObjId.Value);
        var building = _buildingFinder.GetBuildingObjectByLocation(buildingLocation);
        if (building == null)
        {
            return;
        }

        building.GetComponent<BuildingComponentGetter>().TilingTransform.Rotate();
    }

    private void AddNewBuilding(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        int buildingObjectId = (int)eventParam[0];
        int buildingLocation = (int)eventParam[1];

        Unitask_AddNewBuilding(buildingObjectId, buildingLocation).Forget();
    }

    private async UniTaskVoid Unitask_AddNewBuilding(int buildingObjectId, int buildingLocation)
    {
        await UniTask.DelayFrame(1);
        var building = _buildingFinder.GetBuildingObjectByLocation(buildingLocation);
        if (building == null)
        {
            return;
        }

        var userBuildingInfo = _userBuildingList.GetBuilding(buildingObjectId);
        if (userBuildingInfo == null)
        {
            return;
        }

        UpdateBuildingOperationAndTransform(userBuildingInfo);
    }

    private void UpdateBuilding(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)eventParam[0];
        UserBuilding userBuildingInfo = _userBuildingList.GetBuilding(buildingObjId);
        if (userBuildingInfo == null)
        {
            return;
        }

        var buildingObj = _buildingFinder.GetBuildingObjectByLocation(userBuildingInfo.locationTileId);
        if (buildingObj == null)
        {
            return;
        }

        if (userBuildingInfo.status == BuildingStatus.None)
        {
            Destroy(buildingObj);
            return;
        }

        var buildingOperation = buildingObj.GetComponent<BuildingComponentGetter>().Operation;
        BuildingStatus newStatus = userBuildingInfo.status;
        BuildingStatus currentStatus = buildingOperation.Status;
        if (newStatus == BuildingStatus.OnConstruction && newStatus != currentStatus)
        {
            _onAskedAStarRescan.Raise(userBuildingInfo.locationTileId, _buildings.GetBuildingSize(userBuildingInfo.buildingId));
        }

        buildingOperation.UpdateInfo(userBuildingInfo);
    }

    private void DestroyBuiding(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        int buildingObjectId = (int)eventParam[0];
        var userBuildingInfo = _userBuildingList.GetBuilding(buildingObjectId);
        if (userBuildingInfo == null)
        {
            return;
        }

        var buildingObj = _buildingFinder.GetBuildingObjectByLocation(userBuildingInfo.locationTileId);
        if (buildingObj == null)
        {
            return;
        }

        Destroy(buildingObj);

        RequestRescanGraph(userBuildingInfo.locationTileId, _buildings.GetBuildingSize(userBuildingInfo.buildingId)).Forget();
    }

    private async UniTaskVoid RequestRescanGraph(int location, Vector2 buildingSize)
    {
        await UniTask.NextFrame(); // wait for next frame to ensure the building object is destroyed
        _onAskedAStarRescan.Raise(location, buildingSize);
    }

    private void OnEnable()
    {
        _onAddedNewBuilding.Subcribe(AddNewBuilding);
        _onBuildingDataUpdated.Subcribe(UpdateBuilding);
        _onDestroyedABuildingData.Subcribe(DestroyBuiding);
    }

    private void OnDisable()
    {
        _onAddedNewBuilding.Unsubcribe(AddNewBuilding);
        _onBuildingDataUpdated.Unsubcribe(UpdateBuilding);
        _onDestroyedABuildingData.Unsubcribe(DestroyBuiding);
    }

}
