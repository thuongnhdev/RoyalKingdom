using _0.PersonalWork.Harry.Scripts.Common.Static;
using Cysharp.Threading.Tasks;
using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using Network.Common.Static;

public class UserLogin : MonoSingleton<UserLogin>
{
    [Header("Reference - Read")]
    [SerializeField]
    private ApiList _apis;
    [SerializeField]
    private InternalAccountList _interalAccounts;
    [SerializeField]
    private IntegerVariable _chosenTokenIndex;

    [Header("Reference - Write")]
    [SerializeField]
    private List<StringVariable> _tokens;
    [SerializeField]
    private UserProfile _userProfile;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private UserVassalSOList _vassals;

    public static PopulationBase populationBase;
    public async UniTaskVoid Login()
    {
        StringBuilder userId = new();
        string deviceName = SystemInfo.deviceName;
        userId.Append(SystemInfo.deviceUniqueIdentifier).Append(_interalAccounts.GetUserName(deviceName));
        if (_chosenTokenIndex.Value != 0)
        {
            userId.Append(_chosenTokenIndex.Value);
        }
        userId.Append("|0");

        WWWForm form = new();
        form.AddField("P", userId.ToString());
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string twoLetterISO = currentCulture.TwoLetterISOLanguageName;
        form.AddField("Language", twoLetterISO);

        _tokens[_chosenTokenIndex.Value].Value = userId.ToString().Split('|')[0]; // TODO after server define real token, add logic for updating this value

        Tuple<string, string> header = new("Content-Type", form.headers["Content-Type"]);

        var loginResponse = await RequestDispatcher.Instance.SendPostRequest<ApiLoginResult>(_apis.Login, form.data, customHeader: header);
        if (loginResponse.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }

        ParseLoginResult(loginResponse);
    }

    private void ParseLoginResult(ApiLoginResult loginResponse)
    {
        ParseUserProfile(loginResponse);
        ParseItems(loginResponse);
        ParseVassal(loginResponse);

        StatesTown.LoadPopulationInfo(loginResponse);
        StatesGlobal.UID_PLAYER = loginResponse.Uid;

        PacketManager.Instance.InitWebSocket_Lobby();
    }

    private void ParseUserProfile(ApiLoginResult loginResponse)
    {
        _userProfile.Init(loginResponse);
    }

    private void ParseItems(ApiLoginResult loginResponse)
    {
        _userItems.Init(loginResponse);
    }

    private void ParseVassal(ApiLoginResult loginResponse)
    {
        _vassals.Init(loginResponse);
    }

    public void OnLogin()
    {
        Login().Forget();
    }

  
}
