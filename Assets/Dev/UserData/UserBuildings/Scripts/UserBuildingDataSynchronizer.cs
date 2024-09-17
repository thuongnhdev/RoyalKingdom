using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Fbs;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UserBuildingDataSynchronizer : MonoBehaviour
{
    [Header("Reference - Read Network")]
    [SerializeField]
    private ApiList _apis;

    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingList;
    [SerializeField]
    private TownMapSO _userTownMap;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private FloatVariable _newBuildingRotation;
    [SerializeField]
    private IntegerVariable _newBuildingTopLeftTile;
    [SerializeField]
    private IntegerVariable _selectedBuildingId;

    [Header("Reference - Write")]
    [SerializeField]
    private UserBuildingList _userBuildingList;
    [SerializeField]
    private WarehousesCapacity _warehousesCapacity;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onRequestBuildingDataUpdated;
    [SerializeField]
    private GameEvent _onBuildingFinishConstruction;
    [SerializeField]
    private GameEvent _onBuildingFinishUpgrade;
    [SerializeField]
    private GameEvent _onBuildingFinishDestroy;
    [SerializeField]
    private GameEvent _onRequestSwitchBuildingStatus;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onFetchedUserBuildings;
    [SerializeField]
    private GameEvent _onAddedNewBuilding;
    [SerializeField]
    private GameEvent _onBuildingDataUpdated;
    [SerializeField]
    private GameEvent _onBuildingChangedToWaitForUpgradeResource;
    [SerializeField]
    private GameEvent _onCanceledUpgrade;
    [SerializeField]
    private GameEvent _onBuildingStartedDestroyProgress;
    [SerializeField]
    private GameEvent _onCanceledDestroyProgress;
    [SerializeField]
    private GameEvent _onBuildingChangedToDestructedStatus;
    [SerializeField]
    private GameEvent _onDestroyABuildingData;
    [SerializeField]
    private GameEvent _onVassalChanged;
    [SerializeField]
    private GameEvent _askForStorageReservation;
    [SerializeField]
    private GameEvent _askForRevokeReservation;

    [Header("Cheating - Area")]
    [SerializeField]
    private BoolVariable _allowCheat;
    [SerializeField]
    private GameEvent _onBuildingPosChanged;

    private Dictionary<int, BuildingStatus> _buildingsLastStatus = new();

    public void CheckAndResumeDestroyProgress()
    {
        var buildings = _userBuildingList.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            if(building.status != BuildingStatus.OnDestruction)
            {
                continue;
            }

            _onBuildingDataUpdated.Raise(building.buildingObjectId);
        }
    }

    public void RequestUpdateRotation()
    {

        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        Vector2 buildingSize = _buildingList.GetBuildingSize(userBuilding.buildingId);
        bool isSquareBuilding = (int)buildingSize.x == (int)buildingSize.y;

        userBuilding.rotation += isSquareBuilding ? 90f : 180f;
        userBuilding.rotation %= 360f;

        ServerSync_UpdateRotation(userBuilding.buildingObjectId, (int)userBuilding.rotation);

        EditorOnly_SaveUserBuilding();
    }
    private void ServerSync_UpdateRotation(int buildingObjectId, int rotation)
    {
        var rd = RequestDispatcher.Instance;
        rd.SetThrottleForRequest(_apis.RotateBuilding, 2f);

        var body = UserBuildingNetworkHelper.CreateFBRotateBuildingRequestBody(buildingObjectId, rotation);
        rd.SendPostRequest<ResponeDataRotateBuilding>(_apis.RotateBuilding, body).Forget();
    }

    public void RequestAddNewBuilding()
    {
        ServerSync_AddNewBuilding().Forget();
    }
    private async UniTaskVoid ServerSync_AddNewBuilding()
    {
#if UNITY_EDITOR
        if (_allowCheat.Value)
        {
            Cheating_AddABuilding();
            return;
        }
#endif
        var requestBody = UserBuildingNetworkHelper.CreateFBAddBuildingRequestBody(new AddBuildingRequestBody()
        {
            buildingId = _selectedBuildingId.Value,
            location = _newBuildingTopLeftTile.Value,
            rotation = (int)_newBuildingRotation.Value
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeBuildingTownMap>(_apis.AddNewBuilding, requestBody, true);
        if (response.ByteBuffer == null || response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to build Building");
            return;
        }

        LocalSync_AddNewBuilding(response);
    }
    private void LocalSync_AddNewBuilding(ResponeBuildingTownMap response)
    {
        var userBuilding = new UserBuilding
        {
            buildingObjectId = response.IdBuildingPlayer,
            buildingId = _selectedBuildingId.Value,
            rotation = _newBuildingRotation.Value,
            status = BuildingStatus.WaitForConstructingResource,
            locationTileId = _newBuildingTopLeftTile.Value,
            buildingLevel = 0,
            currentContructionRate = 0f,
        };

        _userBuildingList.UpdateBuilding(userBuilding);
        _onAddedNewBuilding.Raise(response.IdBuildingPlayer, userBuilding.locationTileId);

        EditorOnly_SaveUserBuilding();
    }

    public void RequestUpgradeBuilding()
    {
        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        ServerSync_RequestUpgradeBuilding(userBuilding.buildingObjectId).Forget();

        userBuilding.status = BuildingStatus.WaitingForUpgradeResource;

        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);
        _onBuildingChangedToWaitForUpgradeResource.Raise(userBuilding.buildingObjectId);

        EditorOnly_SaveUserBuilding();
    }
    private async UniTaskVoid ServerSync_RequestUpgradeBuilding(int buildingObjId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBUpgradeRequestBody(buildingObjId);
        var response = await RequestDispatcher.Instance.SendPostRequest<UpgradeBuildingRespone>(_apis.UpgradeBuilding, body);
        if (response.ByteBuffer == null)
        {
            return;
        }

        Debug.Log("Successfully Request Upgrade Building");
    }

    public void RequestCancelUpgrade()
    {
        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        List<ResourcePoco> refund = CalculateRefundResources(userBuilding.buildingObjectId);
        bool canReserve = _userItems.TryReserve(refund);
        if (!canReserve)
        {
            return;
        }

        ServerSync_CancelUpgrade(userBuilding.buildingObjectId).Forget();

        userBuilding.constructionMaterial.Clear();
        userBuilding.refundMaterial = refund;
        userBuilding.status = BuildingStatus.Idle;

        _onCanceledUpgrade.Raise(userBuilding.buildingObjectId);
        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);
    }
    private async UniTaskVoid ServerSync_CancelUpgrade(int buildingObjId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBCancelUpgradeRequestBody(buildingObjId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeCancelUpgradeBuilding>(_apis.CancelUpgradeBuilding, body);
        if (response.ByteBuffer == null)
        {
            return;
        }

        Debug.Log("Successfully Cancel Upgrade!");
    }

    public void RequestStartDestroyBuildingProgress()
    {
        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        var refund = CalculateRefundResources(userBuilding.buildingObjectId);
        if (!_userItems.TryReserve(refund))
        {
            return;
        }

        if (userBuilding.status == BuildingStatus.WaitForConstructingResource ||
            userBuilding.status == BuildingStatus.OnConstruction)
        {
            RequestDestroyBuilding(userBuilding.buildingObjectId);
            return;
        }

        _onBuildingStartedDestroyProgress.Raise(userBuilding.buildingObjectId);
        ServerSync_RequestStartDestroy(userBuilding).Forget();
    }
    private async UniTask ServerSync_RequestStartDestroy(UserBuilding building)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBStartDestroyRequestBody(building.buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeDestroyBuilding>(_apis.StartDestroyBuilding, body);
        if (response.ByteBuffer == null)
        {
            return;
        }

        Debug.Log("Successfully Request Start Destroy Progress!");

        LocalSync_StartDestroyProgress(building);
    }
    private void LocalSync_StartDestroyProgress(UserBuilding userBuilding)
    {
        userBuilding.status = BuildingStatus.OnDestruction;

        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);

        _warehousesCapacity.SubCapacity(userBuilding);

        EditorOnly_SaveUserBuilding();
    }

    public void RequestCancelDestroyProgress()
    {
        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        ServerSync_RequestCancelDestroy(userBuilding.buildingObjectId).Forget();

        userBuilding.status = BuildingStatus.Idle;
        userBuilding.destructedTime = 0f;

        _onCanceledDestroyProgress.Raise(userBuilding.buildingObjectId);
        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);

        _warehousesCapacity.AddCapacity(userBuilding);

        EditorOnly_SaveUserBuilding();
    }
    private async UniTaskVoid ServerSync_RequestCancelDestroy(int buildingObjectId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBCancelDestroyRequestBody(buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeCancelDestroyBuilding>(_apis.CancelDestroyingBuilding, body);
        if (response.ByteBuffer == null)
        {
            return;
        }

        Debug.Log("Successfully Request Cancel Destroy Progress!");
    }

    private void RequestUpdateBuildingData(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        var newUserBuildingInfo = (UserBuilding)args[0];
        _userBuildingList.UpdateBuilding(newUserBuildingInfo);

        if (newUserBuildingInfo.IsJustChangedVassal())
        {
            _onVassalChanged.Raise(newUserBuildingInfo.buildingObjectId);
        }

        if (newUserBuildingInfo.status == BuildingStatus.Destructed &&
            ItemHelper.IsResourcesEmpty(newUserBuildingInfo.refundMaterial))
        {
            RequestDestroyBuildingCompletely(newUserBuildingInfo.locationTileId);
            return;
        }

        ServerSync_UpdateBuildingData(newUserBuildingInfo).Forget();
    }
    private async UniTaskVoid ServerSync_UpdateBuildingData(UserBuilding building)
    {
        await SendStartBuildRequestIfNeeded(building);
        await SendStartUpgradeRequestIfNeeded(building);
        UpdateLastStatus(building.buildingObjectId, building.status);

        _onBuildingDataUpdated.Raise(building.buildingObjectId);
    }
    private async UniTask SendStartBuildRequestIfNeeded(UserBuilding building)
    {
        BuildingStatus currentStatus = building.status;
        if (currentStatus != BuildingStatus.OnConstruction)
        {
            return;
        }

        _buildingsLastStatus.TryGetValue(building.buildingObjectId, out var lastStatus);
        if (lastStatus == BuildingStatus.OnConstruction)
        {
            return;
        }

        byte[] body = UserBuildingNetworkHelper.CreateFBStartConstructionRequestBody(building.buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeStartBuildProgress>(_apis.StartBuildingConstruction, body);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        Debug.Log("Request Start Construction Successfully!");
    }
    private async UniTask SendStartUpgradeRequestIfNeeded(UserBuilding building)
    {
        BuildingStatus currentStatus = building.status;
        if (currentStatus != BuildingStatus.Upgrading)
        {
            return;
        }

        _buildingsLastStatus.TryGetValue(building.buildingObjectId, out var lastStatus);
        if (lastStatus == BuildingStatus.Upgrading)
        {
            return;
        }

        byte[] body = UserBuildingNetworkHelper.CreateFBStartUpgradingRequestBody(building.buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeStartProgressUpgrading>(_apis.StartUpgradingBuilding, body);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        Debug.Log("Request Start Upgrading Successfully!");
    }

    private void RequestUpdateConstructedBuilding(object[] args)
    {
        // TODO Server update here

        if (args.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)args[0];
        ServerSync_RequestCompleteConstruction(buildingObjId).Forget();

        var userBuilding = _userBuildingList.GetBuilding(buildingObjId);
        if (userBuilding == null)
        {
            return;
        }

        userBuilding.buildingLevel = 1;
        userBuilding.constructedTime = 0f;
        userBuilding.status = BuildingStatus.Idle;
        UpdateLastStatus(userBuilding.buildingObjectId, BuildingStatus.Idle);
        userBuilding.ConsumeConstructionResource();

        _warehousesCapacity.AddCapacity(userBuilding);
        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);

        EditorOnly_SaveUserBuilding();
    }
    private async UniTaskVoid ServerSync_RequestCompleteConstruction(int buildingObjId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBCompleteConstrucionRequestBody(buildingObjId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeBuildComplete>(_apis.CompleteConstruction, body);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        Debug.Log("Request Complete Building Construction Successfully!");
    }

    private void RequestCompleteUpgrade(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingObjectId = (int)args[0];
        var userBuilding = _userBuildingList.GetBuilding(buildingObjectId);
        if (userBuilding == null)
        {
            return;
        }

        ServerSync_RequestCompleteUpgrade(userBuilding.buildingObjectId).Forget();

        userBuilding.buildingLevel += 1;
        userBuilding.constructedTime = 0f;
        userBuilding.status = BuildingStatus.Idle;
        UpdateLastStatus(userBuilding.buildingObjectId, BuildingStatus.Idle);
        userBuilding.ConsumeConstructionResource();

        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);

        EditorOnly_SaveUserBuilding();
    }
    private async UniTaskVoid ServerSync_RequestCompleteUpgrade(int buildingObjId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBCompleteUpgradeRequestBody(buildingObjId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeBuildComplete>(_apis.CompleteUpgrade, body);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        Debug.Log("Request Complete Upgrade Successfully!");
    }

    private void RequestDestroyBuilding(params object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingObjectId = (int)args[0];

        var userBuilding = _userBuildingList.GetBuilding(buildingObjectId);
        if (userBuilding == null)
        {
            return;
        }

        userBuilding.refundMaterial = CalculateRefundResources(userBuilding.buildingObjectId);
        userBuilding.constructionMaterial.Clear();
        if (ItemHelper.IsResourcesEmpty(userBuilding.refundMaterial))
        {
            RequestDestroyBuildingCompletely(userBuilding.locationTileId);
            return;
        }

        if (!_userItems.TryReserve(userBuilding.refundMaterial))
        {
            return;
        }

        ServerSync_RequestDestroyBuilding(userBuilding.buildingObjectId).Forget();

        userBuilding.status = BuildingStatus.Destructed;
        UpdateLastStatus(userBuilding.buildingObjectId, BuildingStatus.Destructed);

        _onBuildingDataUpdated.Raise(userBuilding.buildingObjectId);
        _onBuildingChangedToDestructedStatus.Raise(userBuilding.buildingObjectId);

        EditorOnly_SaveUserBuilding();
    }
    private async UniTaskVoid ServerSync_RequestDestroyBuilding(int buildingObjectId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBFinishDestroyRequestBody(buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeCompleteDestroyBuilding>(_apis.FinishDestroyBuildingProgress, body);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        Debug.Log("Request Complete Destroy Progress Successfully!");
    }

    private void RequestDestroyBuildingCompletely(params object[] eventParam)
    {
        int buildingTopLeftTileId = (int)eventParam[0];

        var userBuilding = _userBuildingList.GetBuildingByLocation(buildingTopLeftTileId);
        if (userBuilding == null)
        {
            return;
        }

        ServerSync_RequestDestroyBuildingCompletely(userBuilding.buildingObjectId).Forget();

        userBuilding.status = BuildingStatus.None;
        UpdateLastStatus(userBuilding.buildingObjectId, BuildingStatus.None);
        _onDestroyABuildingData.Raise(userBuilding.buildingObjectId);

        EditorOnly_SaveUserBuilding();
    }
    private async UniTaskVoid ServerSync_RequestDestroyBuildingCompletely(int buildingObjId)
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBCompletelyRemoveRequestBody(buildingObjId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeFinalDestroyBuilding>(_apis.CompletelyRemoveBuilding, body);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        Debug.Log("Request Completely Destroy Building Successfully!");
    }

    private async UniTaskVoid FetchUserBuildings()
    {
        byte[] body = UserBuildingNetworkHelper.CreateFBGetUserBuildingBody(-1);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeInfoBuildingTownMap>(_apis.GetUserBuilding, body);

        if (response.ByteBuffer == null)
        {
            return;
        }

        int buildingCount = response.BuldingTownMapLength;
        List<UserBuilding> buildings = new();
        for (int i = 0; i < buildingCount; i++)
        {
            BuildingTownMap data = response.BuldingTownMap(i).Value;
            UserBuilding building = new(data);
            buildings.Add(building);

        }

        _userBuildingList.Init(buildings);
        _warehousesCapacity.Init(buildings);

        CollectCurrentBuildingsStatus();

        _onFetchedUserBuildings.Raise();
    }

    private List<ResourcePoco> CalculateRefundResources(int buildingOjbId)
    {
        List<ResourcePoco> consumedResources = _userBuildingList.GetConsumedResoucesForConstruction(buildingOjbId);
        if (consumedResources == null)
        {
            return new List<ResourcePoco>();
        }

        BuildingStatus status = _userBuildingList.GetBuildingStatus(buildingOjbId);
        float refundRate = BuildingHelper.GetDestructionRefundRate(status);

        return ItemHelper.MultiplyResources(consumedResources, refundRate);
    }

    private void RequestSwitchStatus(object[] args)
    {
        // TODO Server update here
        if (args.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)args[0];
        BuildingStatus nextStatus = (BuildingStatus)args[1];

        var userBuilding = _userBuildingList.GetBuilding(buildingObjId);
        if (userBuilding == null || nextStatus == userBuilding.status)
        {
            return;
        }

        userBuilding.status = nextStatus;
        UpdateLastStatus(userBuilding.buildingObjectId, nextStatus);

        _onBuildingDataUpdated.Raise(buildingObjId);
    }

    private void UpdateLastStatus(int buildingObjId, BuildingStatus newStatus)
    {
        if (!_buildingsLastStatus.TryGetValue(buildingObjId, out var lastStatus))
        {
            _buildingsLastStatus.Add(buildingObjId, newStatus);
            return;
        }

        if (newStatus == lastStatus)
        {
            return;
        }

        _buildingsLastStatus[buildingObjId] = newStatus;
    }

    private void CollectCurrentBuildingsStatus()
    {
        _buildingsLastStatus.Clear();

        List<UserBuilding> buildings = _userBuildingList.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];

            _buildingsLastStatus[building.buildingObjectId] = building.status;
        }
    }

    private void OnEnable()
    {
        _onRequestBuildingDataUpdated.Subcribe(RequestUpdateBuildingData);
        _onBuildingFinishConstruction.Subcribe(RequestUpdateConstructedBuilding);
        _onBuildingFinishUpgrade.Subcribe(RequestCompleteUpgrade);
        _onBuildingFinishDestroy.Subcribe(RequestDestroyBuilding);
        _onRequestSwitchBuildingStatus.Subcribe(RequestSwitchStatus);

        _onBuildingPosChanged.Subcribe(Cheating_RequestChangeBuildingPos);

#if UNITY_EDITOR
        if (_allowCheat.Value)
        {
            _onFetchedUserBuildings.Raise();
            return;
        }
#endif
        FetchUserBuildings().Forget();
    }

    private void OnDisable()
    {
        _onRequestBuildingDataUpdated.Unsubcribe(RequestUpdateBuildingData);
        _onBuildingFinishConstruction.Unsubcribe(RequestUpdateConstructedBuilding);
        _onBuildingFinishUpgrade.Unsubcribe(RequestCompleteUpgrade);
        _onBuildingFinishDestroy.Unsubcribe(RequestDestroyBuilding);
        _onRequestSwitchBuildingStatus.Unsubcribe(RequestSwitchStatus);

        _onBuildingPosChanged.Unsubcribe(Cheating_RequestChangeBuildingPos);
    }

    #region EditorOnly
    private void Cheating_RequestChangeBuildingPos(object[] args)
    {
#if UNITY_EDITOR
        if (args.Length == 0)
        {
            return;
        }

        int newLocation = (int)args[2];

        var userBuilding = _userBuildingList.GetBuilding(_selectedBuildingObjId.Value);
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(userBuilding.buildingId);
        userBuilding.locationTileId = TownMapHelper.FromCenterToTopLeftTile(_userTownMap.xSize, _userTownMap.ySize, buildingSize, newLocation);

        EditorOnly_SaveUserBuilding();
#endif
    }

    private void Cheating_AddABuilding()
    {
        var userBuilding = new UserBuilding
        {
            buildingObjectId = UnityEngine.Random.Range(1, 1000),
            buildingId = _selectedBuildingId.Value,
            rotation = _newBuildingRotation.Value,
            status = BuildingStatus.Idle,
            locationTileId = _newBuildingTopLeftTile.Value,
            buildingLevel = 0,
            currentContructionRate = 0f,
        };

        _userBuildingList.UpdateBuilding(userBuilding);

        _onAddedNewBuilding.Raise(userBuilding.buildingObjectId, userBuilding.locationTileId);

        EditorOnly_SaveUserBuilding();
    }
    private void EditorOnly_SaveUserBuilding()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(_userBuildingList);
#endif
    }  
    #endregion

}
