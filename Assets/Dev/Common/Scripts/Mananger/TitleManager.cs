using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Network.Common.Static;

public class TitleManager : MonoBehaviour, IReceiveAddressableSystemEvent
{
    public GameObject PopupNotice;
    public GameObject PopupServerCheck;
    public GameObject PopupVersionCheck;
    public GameObject PopupLogin;
    public GameObject PopupGuestWarning;
    public GameObject PopupServerChange;

    public Button BtStart;
    public GameObject _touchTxt;

    private string ServerCheckExp = "";
    private string VersionCheckExp = "";

    private AsyncOperation AsyncLoad;



    void Awake()
    {
        BtStart.enabled = false;

        BaseManager.Instance.InitObj();               

        //addressable load를 위해 한번 call한다.
        DataMgCommonPref commomPreMgr = DataMgCommonPref.Instance;
        SoundManager.Instance.BgmPlay(ConstSound._BGM_TITLE);
        //DontDestroyBase.Instance.InitObj();
        InitObj();
        ReadyTitleOnClick();
    }
    public void ReadyTitleOnClick()
    {
        BtStart.enabled = true;
    }
    private void InitObj()
    {
        PlayerPrefsManager.Instance.LoadPlayerPrefs();
    }
    public void UI_OnClickTouchToScreen()
    {
        if (AddressableLoadManager.Instance != null)
        {
            if (_touchTxt != null)
            {
                _touchTxt.SetActive(false);
            }


            AddressableLoadManager.Instance.StartPatchSystem();
        }
        else
        {
            Debug.LogError("AddressableLoadManager.Instance is null");
        }
        //InitLogInState();
        //DontDestroyBase.Instance.Loading.SetActiveObj(StartSceneLoad);
    }
    public void StartSceneLoad()
    {
        StartCoroutine(CoLoadMainScene(ConstGlobal._SCE_MAIN));
    }
    public void FinishSceneLoad()
    {
        //Debug.Log("FinishSceneLoad");
        //AsyncLoad.allowSceneActivation = true;
    }

    private void InitLogInState()
    {
        StatesGlobal.IS_TITLE_START = true;

        StatesGlobal.TYPE_SERVER = SERVER_TYPE.Remote;
        StatesGlobal.TYPE_ACCESS = ACCESS_TYPE.Plan;
    }

    IEnumerator CoLoadMainScene(string sceneToLoad)
    {
        AsyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        //AsyncLoad.allowSceneActivation = false;
        while (!AsyncLoad.isDone)
        {
            yield return null;
        }

        //MainManager::LoadLoginData 완료후 로딩 팝업을 닫음
        //Loading.Complete(null);        
    }

    #region IAddressableSystemReadyComplete
    public void OnDownLoadStart()
    {

    }

    public void OnAddressableCompleteEnd()
    {
        InitLogInState();
        BaseManager.Instance.Loading.SetActiveObj(true, StartSceneLoad);
    }

    public void OnAddressableDownLoadComplete()
    {

    }
    //다운로드 실패 
    public void AddressableDownLoadFail(string errorMsg)
    {
        
    }

    #endregion
}
