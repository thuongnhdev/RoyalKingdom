using Cysharp.Threading.Tasks;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MasterDataFetcher : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private ApiList _apiList;
    [SerializeField]
    private MasterDataParser _parser;

    public async UniTask FetchNewMasterData()
    {
        var fbMasterData = await RequestDispatcher.Instance.SendPostRequest<ApiGetLowData>(_apiList.MasterData, null);
        if (fbMasterData.ApiResultCode != 1)
        {
            return;
        }

        _parser.ParseMasterDataFromServer(fbMasterData);
    }

    private void OnEnable()
    {
        FetchNewMasterData().Forget();
    }

}
