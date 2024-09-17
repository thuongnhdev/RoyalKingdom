using Cysharp.Threading.Tasks;
using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class KingdomSynchronizer : MonoBehaviour
{
    [SerializeField]
    private ApiList _apis;
    [Header("Reference - Read")]
    [SerializeField]
    private KingdomList _kingdoms;
    [SerializeField]
    private LandStaticInfoList _landStaticInfos;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onBattleEndMessage;
    [Header("In and Out, After demo, change to In")] // TODO AfterDemo Change to In only
    [SerializeField]
    private GameEvent _onKingdomTeritoryChanged;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onKingdomDataFetched;

    [Header("Scene Configs")]
    [SerializeField]
    private WmskBridge _mapBridge;

    public void RefreshKingdomData()
    {
        FetchKingdomsInfo().Forget();
    }

    private void RemoveAfterDemo_MergeLand(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        byte[] data = (byte[])eventParam[0];
        FlatBufferResponseConverter<EndBattel> converter = new();
        var battleEndData = converter.Convert(data);
        _onKingdomTeritoryChanged.Raise(battleEndData.UidPlayerWin, battleEndData.UidPlayerLose, battleEndData.IdLandUserLose);
    }

    private void HandleKingdomTeritoryChange(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }
        long winKingdomId = (long)args[0];
        long loseKingdomId = (long)args[1];
        int transferLandId = (int)args[2];

        TransferLand(winKingdomId, loseKingdomId, transferLandId);
    }

    private void TransferLand(long toKingdomId, long fromKingdomId, int transferredLandId)
    {
        var toKingdom = _kingdoms.GetKingdom(toKingdomId);
        var fromKingdom = _kingdoms.GetKingdom(fromKingdomId);
        toKingdom.members.Add(transferredLandId);
        fromKingdom.members.Remove(transferredLandId);

        _mapBridge.TransferALand(toKingdomId, transferredLandId);
    }

    private async UniTaskVoid FetchKingdomsInfo()
    {
        byte[] requestBody = UserLandNetworkHelper.CreateFbGetKingdomRequestBody(-1);
        var response = await RequestDispatcher.Instance.SendPostRequest<responeInfoKingdom>(_apis.FetchKingdomInfo, requestBody);
        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to fetch kingdom info");
            return;
        }

        _kingdoms.Init(response);
        _onKingdomDataFetched.Raise();
        Logger.Log("Fetched kingdom info!");
    }

    private void OnEnable()
    {
        FetchKingdomsInfo().Forget();
        _onKingdomTeritoryChanged.Subcribe(HandleKingdomTeritoryChange);
        _onBattleEndMessage.Subcribe(RemoveAfterDemo_MergeLand);
    }

    private void OnDisable()
    {
        _onKingdomTeritoryChanged.Unsubcribe(HandleKingdomTeritoryChange);
        _onBattleEndMessage.Unsubcribe(RemoveAfterDemo_MergeLand);
    }
}
