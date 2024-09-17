using Cysharp.Threading.Tasks;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfileSynchronizer : MonoBehaviour
{
    [SerializeField]
    private ApiList _apis;
    [Header("Reference - Read/Write")]
    [SerializeField]
    private UserProfile _userProfile;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedLandGeoId;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onNewUserDetected;
    [SerializeField]
    private GameEvent _onUserPassportChanged;

    public void JoinLand()
    {
        Async_JoinLand().Forget();
    }
    private async UniTaskVoid Async_JoinLand()
    {
        int landId = _selectedLandGeoId.Value;//_landsStaticInfos.GetLandIdByGeoId(_selectedLandGeoId.Value);
        await ServerSync_JoinOrBuyLand(landId);

        _userProfile.landId = landId;
        _userProfile.ownedLandId = 0;
        _userProfile.landAuthority = LandAuthority.TownOwner;

        _onUserPassportChanged.Raise();
        Editor_SaveInfo();
    }

    public void BuyLand()
    {
        Async_BuyLand().Forget();
    }
    private async UniTaskVoid Async_BuyLand()
    {
        int landId = _selectedLandGeoId.Value;//_landsStaticInfos.GetLand(_selectedLandGeoId.Value);
        await ServerSync_JoinOrBuyLand(landId);

        _userProfile.landId = landId;
        _userProfile.ownedLandId = landId;
        _userProfile.landAuthority = LandAuthority.LandOwner;

        Editor_SaveInfo();
        _onUserPassportChanged.Raise();
    }

    private async UniTask ServerSync_JoinOrBuyLand(int landId)
    {
        byte[] requestBody = UserLandNetworkHelper.CreateFbChooseLandRequestBody(landId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeChooseLand>(_apis.ChooseLand, requestBody, blockUi: true);
        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError($"Failed to join/buy land with Id {landId}");
            return;
        }
        Logger.Log($"Success to join/buy land with Id {landId}");
    }

    private async UniTaskVoid NextFrame_CheckIsNewUser()
    {
        await UniTask.NextFrame();
        if (_userProfile.landId <= 0)
        {
            _onNewUserDetected.Raise();
        }
    }

    private void OnEnable()
    {
        NextFrame_CheckIsNewUser().Forget();
    }

    private void Editor_SaveInfo()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_userProfile);
#endif
    }

}
