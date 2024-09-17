using Cysharp.Threading.Tasks;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class LandDataSynchronizer : MonoBehaviour
{
    [SerializeField]
    private ApiList _apis;
    [Header("Reference - Write")]
    [SerializeField]
    private LandDynamicInfoList _landsDynamicInfos;
    [SerializeField]
    private KingdomList _kingdoms;

    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _passport;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onSetLandOwner;
    [SerializeField]
    private GameEvent _onAddedLandMember;

    public void RefreshLandStaticInfo()
    {
        FetchLandsDynamicInfo().Forget();
    }

    public void UpdateRelatedInfoInPassportIfNeeded()
    {
        if (_passport.landAuthority != LandAuthority.LandOwner)
        {
            return;
        }

        var land = _landsDynamicInfos.GetLand(_passport.ownedLandId);
        if (land == null || land.owner == _passport.id)
        {
            return;
        }

        land.owner = _passport.id;
    }

    private void RequestSetLandOwner(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        // Server logic here
        long userId = (long)args[0];
        int landId = (int)args[1];
        var land = _landsDynamicInfos.GetLand(landId);
        if (land == null)
        {
            return;
        }

        land.owner = userId;
        List<long> members = land.members;
        if (members.Contains(userId))
        {
            return;
        }

        members.Add(userId);
    }

    private void RequestAddLandMember(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        // Server logic here
        long memberId = (long)args[0];
        int landId = (int)args[1];
        
        _landsDynamicInfos.AddMember(landId, memberId);
    }

    private async UniTaskVoid FetchLandsDynamicInfo()
    {
        byte[] requestBody = UserLandNetworkHelper.CreateFbGetLandRequestBody(-1);
        var response = await RequestDispatcher.Instance.SendPostRequest<responeInfoLand>(_apis.FetchLandInfo, requestBody);
        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to fetch lands dynamic infos");
            return;
        }

        _landsDynamicInfos.Init(response);
        Logger.Log("Fetched lands dynamic infos");
    }

    private void OnEnable()
    {
        FetchLandsDynamicInfo().Forget();
        _onSetLandOwner.Subcribe(RequestSetLandOwner);
        _onAddedLandMember.Subcribe(RequestAddLandMember);
    }

    private void OnDisable()
    {
        _onSetLandOwner.Unsubcribe(RequestSetLandOwner);
        _onAddedLandMember.Unsubcribe(RequestAddLandMember);
    }
}
