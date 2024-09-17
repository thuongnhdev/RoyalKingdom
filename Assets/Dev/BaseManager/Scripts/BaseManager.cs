using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BaseManager : MonoBehaviour
{
    #region Base Singleton
    private static BaseManager instance = null;
    private static bool IsQuitApplication = false;
    public static BaseManager Instance
    {
        get
        {
            if (IsQuitApplication)
            {
                return null;
            }
            if (instance == null)
            {
                GameObject obj = Instantiate(Resources.Load("Prefabs/BaseManager")) as GameObject;
                obj.name = "BaseManager";
                instance = obj.GetComponent<BaseManager>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitObj_Awake();
    }
    public void OnDestroy()
    {
        IsQuitApplication = true;
    }
    #endregion

    [Header("Sub manager")]
    [SerializeField]
    private SoundManager ScrSoundManager;


    [Header("Base object")]
    public PopLoding Loading;
    public GameObject ToastRoot;

    [SerializeField]
    private GameObject LockUI;    
    [SerializeField]
    private GameObject Loading_Simple;
    [SerializeField]
    private GameObject BgFadeInOut;

    [SerializeField]
    private GameObject PopToast;
    [SerializeField]
    private GameObject PopRestart_Base;
    [SerializeField]
    private GameObject PopError_Base;
    [SerializeField]
    private GameObject Loading_Reconnect;
    

    private float FadeTimeRate;
    private float FadeTimeIn;
    private float FadeTimeOut;
    private float FadeTimeWait;
    private FunPointer FadeActionIn;
    private FunPointer FadeActionOut;    

    private int TYPE_RESTART = -1;

    private const int TYPE_RESTART_TITLE = 0;
    private const int TYPE_RESTART_MAIN = 1;


    private List<GameObject> SingletonListForReset = new List<GameObject>();    

    private AsyncOperation AsyncLoad;

    private bool ProcessRestart = false;
    private bool ActiveRestartPop = false;
    private int RestartType;

    private const int TYPE_TITLE = 0;
    private const int TYPE_MAIN = 1;

    private bool IsInit = false;

    public void InitObj()
    {

    }
    private void InitObj_Awake()
    {
        if (!IsInit)
        {
            IsInit = true;

            Application.targetFrameRate = ConstGlobal._FRAME_RATE;
            QualitySettings.vSyncCount = ConstGlobal._V_SYNC_COUNT;
            EventSystem.current.pixelDragThreshold = (int)(ConstGlobal._DRAG_THRESHOLD_CM * Screen.dpi / ConstGlobal._INCH_TO_CM);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            LockUI.SetActive(false);
            Loading_Simple.SetActive(false);
            BgFadeInOut.SetActive(false);
        }
    }
    public SoundManager GetSoundManager()
    {
        return ScrSoundManager;
    }    
    public void AddSingletonForReset(GameObject obj)
    {
        if (!SingletonListForReset.Contains(obj))
        {
            SingletonListForReset.Add(obj);
        }
    }


    
    public void ActiveLockUI(bool active)
    {
        if (LockUI.activeInHierarchy == active)
        {
            return;
        }
        LockUI.SetActive(active);
    }
    public void ActiveLoadingSimple(bool active, bool immediately = false)
    {
        ActiveLockUI(active);

        if (active == false)
        {
            CancelInvoke("DelaySimpleLoading");
            Loading_Simple.SetActive(active);
        }
        else
        {
            if (immediately == true)
            {
                DelaySimpleLoading();
            }
            else
            {
                Invoke("DelaySimpleLoading", 0.5f);
            }
        }
    }
    private void DelaySimpleLoading()
    {
        Loading_Simple.SetActive(true);
    }
    public void ActiveLoadingReconnect(bool active)
    {
        if (Loading_Reconnect != null)
        {
            Loading_Reconnect.SetActive(active);
        }
    }



    public void ActiveErrorPopup(string retCode, bool bReStart = false, bool bReTry = false)
    {
        TYPE_RESTART = TYPE_RESTART_TITLE;
        SetActiveRestartPop();
        PopError_Base.SetActive(true);
        //PopError_Base.GetComponent<Pop_Base>().SetTitle("");
        //PopError_Base.GetComponent<Pop_Base>().SetContents(ConstGlobal.STR_TEMP);
        //if (bReStart)
        //{
        //    PopError_Base.GetComponent<Pop_Type_Info>().SetAction_ReStart(UI_OnClick_RestartOk);
        //}
        //else
        //{
        //    PopError_Base.GetComponent<Pop_Type_Info>().SetAction_Ok(UI_OnClick_Hide);
        //}

        //if (bReTry)
        //{
        //    PopError_Base.GetComponent<Pop_Type_Info>().SetAction_Try(UI_OnClick_ReTry);
        //}

        //PopError_Base.GetComponent<Pop_Type_Info>().SetButton(bReStart, bReTry);
    }

    public void ActiveRestartPop_Main(string retCode)
    {
        TYPE_RESTART = TYPE_RESTART_MAIN;
        SetActiveRestartPop();
        PopRestart_Base.SetActive(true);
        //PopRestart_Base.GetComponent<Pop_Base>().SetContents(ConstGlobal.STR_TEMP);
        //PopRestart_Base.GetComponent<Pop_Type_Info>().SetAction_Ok(UI_OnClick_RestartOk);
    }
    public void ActiveRestartPop_Title(string retCode)
    {
        TYPE_RESTART = TYPE_RESTART_TITLE;
        SetActiveRestartPop();
        PopRestart_Base.SetActive(true);
        //PopRestart_Base.GetComponent<Pop_Base>().SetContents(ConstGlobal.STR_TEMP);
        //PopRestart_Base.GetComponent<Pop_Type_Info>().SetAction_Ok(UI_OnClick_RestartOk);
    }

    public void UI_OnClick_RestartOk()
    {
        switch (TYPE_RESTART)
        {
            case TYPE_RESTART_TITLE:
                OnRestart_Title();
                break;
            case TYPE_RESTART_MAIN:
                OnRestart_Main();
                break;
        }
        PopRestart_Base.SetActive(false);
        PopError_Base.SetActive(false);
    }

    public void UI_OnClick_ReTry()
    {        
        PopRestart_Base.SetActive(false);
        PopError_Base.SetActive(false);
    }
    public void UI_OnClick_Hide()
    {
        PopRestart_Base.SetActive(false);
        PopError_Base.SetActive(false);
    }

    public void ShowToast(string value)
    {
        Instantiate(PopToast, ToastRoot.transform).GetComponent<PopToast>().ShowToast(value);
    }





    public bool IsProcessRestart()
    {
        return ProcessRestart;
    }
    public bool IsActiveRestartPop()
    {
        return ActiveRestartPop;
    }
    public void SetActiveRestartPop()
    {
        ActiveRestartPop = true;
    }
    public void OnRestart_Title()
    {        
        if (ProcessRestart)
        {
            return;
        }
        ProcessRestart = true;
        RestartType = TYPE_TITLE;

        OnRestart_Base();
    }
    public void OnRestart_Main()
    {
        if (ProcessRestart)
        {
            return;
        }
        ProcessRestart = true;
        RestartType = TYPE_MAIN;

        OnRestart_Base();
    }
    private void OnRestart_Base()
    {
        if (Loading.gameObject.activeInHierarchy)
        {
            Loading.ResetAction();
        }
        else
        {
            Loading.SetActiveObj(false);
        }
        ActiveLoadingReconnect(false);        
        Time.timeScale = 1f;

        OnRestart_Step_1();
    }
    private void OnRestart_Step_1()
    {
        StartCoroutine(CoLoadRestartScene(ConstGlobal._SCE_RESTART, OnRestart_Step_2));
    }
    private void OnRestart_Step_2()
    {
        for (int i = 0; i < SingletonListForReset.Count; i++)
        {
            if (SingletonListForReset[i] != null)
            {
                Destroy(SingletonListForReset[i]);
            }
        }
        SingletonListForReset.Clear();

        OnRestart_Step_3();
    }
    private void OnRestart_Step_3()
    {
        switch (RestartType)
        {
            case TYPE_TITLE:
                StartCoroutine(CoLoadRestartScene("Title", OnRestart_Finish_Title));
                break;
            case TYPE_MAIN:
                StartCoroutine(CoLoadRestartScene(ConstGlobal._SCE_MAIN, OnRestart_Finish_Main));
                break;
        }

    }
    private void OnRestart_Finish_Title()
    {
        Loading.Complete();
        ProcessRestart = false;
    }
    private void OnRestart_Finish_Main()
    {
        ActiveRestartPop = false;
        ProcessRestart = false;
    }

    private IEnumerator CoLoadRestartScene(string sceneToLoad, FunPointer loadRestartScene)
    {
        AsyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!AsyncLoad.isDone)
        {
            yield return null;
        }

        if (loadRestartScene != null)
        {
            loadRestartScene();
        }
    }





    public void SetFadeInOutAction(FunPointer actionIn, FunPointer actionOut)
    {
        FadeActionIn = actionIn;
        FadeActionOut = actionOut;
    }
    public void DoFadeInOut(Color32 baseColor, float timeWait, float rate = 1f, float timeIn = 0.16f, float timeOut = 0.16f)
    {
        if (BgFadeInOut != null)
        {
            Color tempColor = baseColor;
            tempColor.a = 0f;

            BgFadeInOut.GetComponent<Image>().color = tempColor;
            BgFadeInOut.SetActive(true);
            FadeTimeRate = rate;
            FadeTimeWait = timeWait;
            FadeTimeIn = timeIn;
            FadeTimeOut = timeOut;
            DoFadeInOut_StartIn();
        }
    }
    public void DoFadeInOut_DataSet(Color32 baseColor, float rate = 1f, float timeIn = 0.16f, float timeOut = 0.16f)
    {
        if (BgFadeInOut != null)
        {
            Color tempColor = baseColor;
            tempColor.a = 0f;

            BgFadeInOut.GetComponent<Image>().color = tempColor;
            BgFadeInOut.SetActive(true);
            FadeTimeRate = rate;
            FadeTimeIn = timeIn;
            FadeTimeOut = timeOut;
        }
    }
    public void CallDoFadeInOut_StartIn()
    {
        BgFadeInOut.GetComponent<Image>().DOFade(FadeTimeRate, FadeTimeIn).OnComplete(DoFadeInOut_WaitOut_Complete);
    }
    public void DoFadeInOut_WaitOut_Complete()
    {
        if (FadeActionIn != null)
        {
            FadeActionIn();
            FadeActionIn = null;
        }
    }
    public void CallDoFadeInOut_StartOut()
    {
        BgFadeInOut.GetComponent<Image>().DOFade(0, FadeTimeOut).OnComplete(DoFadeInOut_Done);
    }
    private void DoFadeInOut_StartIn()
    {
        BgFadeInOut.GetComponent<Image>().DOFade(FadeTimeRate, FadeTimeIn).OnComplete(DoFadeInOut_WaitOut);
    }
    private void DoFadeInOut_WaitOut()
    {
        StartCoroutine(CoSimpleWait(FadeTimeWait, done =>
        {
            if (done)
            {
                if (FadeActionIn != null)
                {
                    FadeActionIn();
                    FadeActionIn = null;
                }
                BgFadeInOut.GetComponent<Image>().DOFade(0, FadeTimeOut).OnComplete(DoFadeInOut_Done);
            }
        }));
    }
    private void DoFadeInOut_Done()
    {
        BgFadeInOut.SetActive(false);
        if (FadeActionOut != null)
        {
            FadeActionOut();
            FadeActionOut = null;
        }
    }
    private IEnumerator CoSimpleWait(float waitTime, System.Action<bool> OnComplete)
    {
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        OnComplete(true);
    }
}
