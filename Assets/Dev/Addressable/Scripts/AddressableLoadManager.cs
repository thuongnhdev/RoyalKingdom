using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


[DefaultExecutionOrder(-1)]
public class AddressableLoadManager : MonoBehaviour
{
    [SerializeField]
    private AssetLabelReference _loadAssetLabel;      

    [SerializeField]
    PatchUI _patchUI;

    public IPatchUI _iPatchUI => _patchUI;

    private static AddressableLoadManager _instance = null;

    UnityAction AddressableStartDownload;
    //addressable Load 관련 완료 callback
    UnityAction AddressableSystemReadyComplete;
    //다운로드 성공
    UnityAction AddressableDownLoadComplete;
    //다운로드 실패 
    UnityAction<string> AddressableDownLoadfail;

    List<IAddressableLoad> _addressableLoader = new List<IAddressableLoad>();

    Coroutine _CheckaddressableAllLoad = null;

    public static string ConfigVersion = "v0.0.0";

    private IList<IResourceLocation> _completelocations;

    
    bool _completeDownLoaded = false;

    private bool _AddAssetPack = false;

    bool _startPatch = false;

    public static AddressableLoadManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        Init();
    }

 
    void Init()
    {
        _AddAssetPack = false;
        _CheckaddressableAllLoad = null;

        if (_patchUI != null)
            _patchUI.ShowUI(false);
    }

    private void Start()
    {
        //StartCoroutine(SetCountry());
    }

    public void StartPatchAssetpack()
    {
        _AddAssetPack = true;
        ResourceLocationsAsync();

    }

    public void StartPatchSystem()
    {
        if (_startPatch == true)
            return;

        _startPatch = true;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //네트워크 에러 
            return;
        }
#if UNITY_EDITOR        
        DownLoadComplete();
#else
        //StartTargetConfigPath();
        DownLoadComplete();
#endif

    }

    private void OnDestroy()
    {
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler -= Handler;
    }
    public void AddAddressableSystemReadyComplete(IReceiveAddressableSystemEvent complete)
    {
        AddressableStartDownload += complete.OnDownLoadStart;
        AddressableSystemReadyComplete += complete.OnAddressableCompleteEnd;
        AddressableDownLoadComplete += complete.OnAddressableDownLoadComplete;
        AddressableDownLoadfail += complete.AddressableDownLoadFail;
    }

    public void AddAddressableLoad(IAddressableLoad load)
    {
        //_addressableLoad.Add(load);
        
    }

    public void AddAddressableLoader(IAddressableLoad loader)
    {
        _addressableLoader.Add(loader);

        if(_completeDownLoaded == true)
        {
            loader.LoadStart();
        }
    }
    



    
    void ConfigError()
    {
        _startPatch = false;

        StartTargetConfigPath();
    }

    void StartTargetConfigPath()
    {
        Debug.Log("StartTargetConfigPath");

        StartCoroutine(DownLoadTargetConfigPath(StartConfigFile, ConfigError));
    }

    void StartConfigFile()
    {
        StartCoroutine(DownLoadConfigFile());
    }

    public static IEnumerator DownLoadTargetConfigPath(UnityAction OnSuccess,UnityAction OnError)
    {

        Debug.Log("DownLoadTargetConfigPath");
        string platform = PlatformMappingService.GetPlatform().ToString();
        string targetBuild = "Dev";
#if BUILD_SERVICE
        targetBuild = "/Service";
#endif

        string path = "http://app-hinode.vespainc.com/ca_manager/Addressables/TargetPath.config";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.error != null)
            {
                OnError?.Invoke();
                Debug.LogError("Error: " + webRequest.error+"configfile path info error");
            }
            else
            {

                

                Debug.Log("Received: " + webRequest.downloadHandler.text);
                TargetConfigPath  targetpath = JsonUtility.FromJson<TargetConfigPath>(webRequest.downloadHandler.text);


                switch (platform) 
                { 
                    case "iOS":
                        if(targetBuild == "Dev")
                        {
                            ConstServer._targetConfigPath = targetpath.iOS_DEV;
                        }
                        else
                        {
                            ConstServer._targetConfigPath = targetpath.iOS_Service;
                        }
                        break;
                    case "Android":
                        if (targetBuild == "Dev")
                        {
                            ConstServer._targetConfigPath = targetpath.AOS_DEV;
                        }
                        else
                        {
                            ConstServer._targetConfigPath = targetpath.AOS_Service;
                        }
                        break;
                    default:
                        break;
                }

                if(string.IsNullOrEmpty(ConstServer._targetConfigPath) == true)
                {
                    Debug.LogError("플랫폼에 맞는 path가 없습니다.");
                    OnError?.Invoke();
                }
            }
        }

        if (string.IsNullOrEmpty(ConstServer._targetConfigPath) == false)
            OnSuccess?.Invoke();
    }


    IEnumerator DownLoadConfigFile()
    {
        Debug.Log("DownLoadConfigFile");
        string configpath = ConstServer._targetConfigPath+"/"+ConfigVersion+".config";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(configpath))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.error != null)
            {
                StartConfigFile();
                Debug.LogError("Error: " + webRequest.error + "Config file Download Fail");
            }
            else
            {
                Debug.Log("Received: " + webRequest.downloadHandler.text);

                ConstServer._config = JsonUtility.FromJson<ConfigFile>(webRequest.downloadHandler.text); ;

                CheckConfigInfo();
            }
        }
    }

    void CheckConfigInfo()
    {

        Debug.Log("CheckConfigInfo");
        if (ConstServer._config == null)
        {
            StartConfigFile();
            Debug.LogError("Error: Config file Create Fail");
        }
        else
        {
            
            if(ConstServer._config.s_State == 1)
            {
                //서버 점검
                Debug.Log(ConstServer._config.s_Msg);
                return;
            }

            if(ConstServer._config.c_State == 1)
            {
                //클라이언트 updata
                Debug.Log(ConstServer._config.c_Msg);
                Application.OpenURL(ConstServer._config.c_UpdateUrl);
                return;
            }
            

            AddressableInit();
        }
    }

    void AddressableInit()
    {
        Debug.Log("AddressableInit");
        AsyncOperationHandle<IResourceLocator> handle = Addressables.InitializeAsync();
        handle.Completed += CompleteAddressableInit;
    }

    void CompleteAddressableInit(AsyncOperationHandle<IResourceLocator> handle)
    {
        Debug.Log("CompleteAddressableInit");
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
#if UNITY_EDITOR
            //리소스 다운로드 완료!!!
            CheckCatalogInfo();
#else
            CheckCatalogInfo();
#endif
        }
        else
        {
            AddressableDownLoadfail?.Invoke("Addressable Init Fail");
            Debug.LogError("Addressable Init Fail!!!!!");
        }
    }

    void CheckCatalogInfo()
    {
        Debug.Log("CheckCatalogInfo");
        if (ConstServer._config != null && string.IsNullOrEmpty(ConstServer._config.version) == false)
        {
            Addressables.ClearResourceLocators();

            string catalogPath = GetAddressablepath();

            catalogPath = catalogPath + "/catalog_" + ConstServer._config.version + ".hash";
            AsyncOperationHandle<IResourceLocator> handle = Addressables.LoadContentCatalogAsync(catalogPath);
            handle.Completed += CompleteLoadCatalog;
        }
        else
        {
            ResourceLocationsAsync();
        }
        
    }

    void CompleteLoadCatalog(AsyncOperationHandle<IResourceLocator> handle)
    {
        Debug.Log("loadCatalogsCompleted ==> " + handle.Status);
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ResourceLocationsAsync();
        }
        else
        {
            AddressableDownLoadfail?.Invoke("LoadContentCatalogAsync is failed");
            Debug.LogError("LoadContentCatalogAsync is failed");
        }
    }

    void ResourceLocationsAsync()
    {
        Debug.Log("ResourceLocationsAsync");
        // 빌드타겟의 경로를 가져온다.
        // 경로이기 때문에 메모리에 에셋이 로드되진 않는다.
        Addressables.LoadResourceLocationsAsync(_loadAssetLabel).Completed += LoadResourceLocationsAsync_Completed;
    }

    private void LoadResourceLocationsAsync_Completed(AsyncOperationHandle<IList<IResourceLocation>> obj)
    {
        Debug.Log("LoadResourceLocationsAsync_Completed");
        _completelocations = obj.Result;
        GetDownLoadSize(_loadAssetLabel);
    }

    void GetDownLoadSize(AssetLabelReference label)
    {
        Addressables.GetDownloadSizeAsync(label).Completed += GetDownloadSizeAsync_Completed;
    }

    private void GetDownloadSizeAsync_Completed(AsyncOperationHandle<long> obj)
    {
        Debug.Log("GetDownloadSizeAsync_Completed");
        int mb = 1024 * 1024;

        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("다운로드 용량 : " + Mathf.RoundToInt(obj.Result / mb));

            if (obj.Result == 0)
            {
                //다운로드 완료
                DownLoadComplete();
            }
            else
            {
                mb = Mathf.RoundToInt(obj.Result / (1024 * 1024));
                //addressable Download

                if (_AddAssetPack == false)
                    StartDownLoadLocationAddressable();
                else
                    StartDownLoadLocationListAddressable();

            }
        }
        else if (obj.Status == AsyncOperationStatus.Failed)
        {
            AddressableDownLoadfail?.Invoke("GetDownloadSizeAsync fail Error");
            Debug.LogError("GetDownloadSizeAsync fail Error");
        }
        else
        {
            AddressableDownLoadfail?.Invoke("GetDownloadSizeAsync fail");
            Debug.LogError("GetDownloadSizeAsync fail");
        }

        Addressables.Release(obj);
    }

    void StartDownLoadLocationAddressable()
    {
        AddressableStartDownload?.Invoke();
        StartCoroutine(DownLoadLocationAddressable());
    }

    string curProgress;

    IEnumerator DownLoadLocationAddressable()
    {

        SetPatchInfo(1.0f, "0 / " + _completelocations.Count.ToString());

        int mb = 1024 * 1024;
        bool error = false;




        for (int i = 0; i < _completelocations.Count; i++)
        {
            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(_completelocations[i].PrimaryKey);



            //Debug.Log("1 - Start Name :" + handle.DebugName);
            UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler += Handler;


            while (!handle.IsDone)
            {
                //Debug.Log("2 - Downloading Name :" + handle.DebugName);

                
                float percent = handle.GetDownloadStatus().Percent;
                long downBytes = handle.GetDownloadStatus().DownloadedBytes;
                long TotalBytes = handle.GetDownloadStatus().TotalBytes;

                curProgress = string.Format("DownLoad {0}/{1}  ({2} MB / {3} MB)", i + 1, _completelocations.Count, Mathf.RoundToInt(downBytes / mb), Mathf.RoundToInt(TotalBytes / mb));

                //Debug.Log("3: "+ curProgress);

                SetPatchInfo(percent, curProgress);
                yield return null;
            }

            

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                SetPatchInfo(1.0f, i.ToString() + " / " + _completelocations.Count.ToString());
            }
            else if (handle.Status == AsyncOperationStatus.Failed)
            {
                error = true;
                AddressableDownLoadfail?.Invoke("DownLoadAddressable fail Error");
                Debug.LogError("DownLoadAddressable fail Error");
                yield break;
            }
            else
            {
                error = true;
                AddressableDownLoadfail?.Invoke("DownLoadAddressable fail");
                Debug.LogError("DownLoadAddressable fail");
                yield break;
            }

            //Debug.Log("4: End");
            //yield return new WaitForSeconds(3.0f);
            
        }

        if(error == false)
        {
            DownLoadComplete();
        }
    }


    void StartDownLoadLocationListAddressable()
    {
        AddressableStartDownload?.Invoke();
        StartCoroutine(DownLoadLocationListAddressable());
    }

   
    IEnumerator DownLoadLocationListAddressable()
    {
 
        int mb = 1024 * 1024;
        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(_completelocations);

        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler += Handler;


        while (!handle.IsDone)
        {

            float percent = handle.GetDownloadStatus().Percent;
            long downBytes = handle.GetDownloadStatus().DownloadedBytes;
            long TotalBytes = handle.GetDownloadStatus().TotalBytes;

            curProgress = string.Format("{1} / {2} MB", percent, Mathf.RoundToInt(downBytes / mb), Mathf.RoundToInt(TotalBytes / mb));

            SetPatchInfo(percent, "");

            yield return null;
        }

        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            DownLoadComplete();
        }
        else if(handle.Status == AsyncOperationStatus.Failed)
        {
            AddressableDownLoadfail?.Invoke("DownLoadAddressable fail Error");
            Debug.LogError("DownLoadAddressable fail Error");
        }
        else
        {
            AddressableDownLoadfail?.Invoke("DownLoadAddressable fail");
            Debug.LogError("DownLoadAddressable fail");
        }


        Addressables.Release(handle);
    }

    private void Handler(AsyncOperationHandle handle, Exception e)
    {
        if (e is InvalidKeyException exception)
        {
            Debug.LogError("Exception key : " + exception.Key);
            Debug.LogError("Exception Type : " + exception.Type);
            Debug.LogError("Exception Message : " + exception.Message);
        }
    }

    public static string GetAddressablepath()
    {
        string configpath = ConstServer._targetConfigPath +"/" +ConfigVersion;

        return configpath;
    }

    public void DownLoadComplete()
    {            
        curProgress = "DownLoad Complete";

        //다운로드 완료!!!
        if (_AddAssetPack == true)
            return;

       SetPatchInfo(1.0f, "Download Complete");

        if (_patchUI != null)
            _patchUI.ShowUI(false);

        _completeDownLoaded = true;

        AddressableDownLoadComplete?.Invoke();

        if (_CheckaddressableAllLoad == null)
            _CheckaddressableAllLoad = StartCoroutine(CheckaddressableAllLoad());
    }

    IEnumerator CheckaddressableAllLoad()
    {
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < _addressableLoader.Count; i++)
        {
            _addressableLoader[i].LoadStart();

            yield return new WaitUntil(() => _addressableLoader[i].isComplete == true);
        }

        AddressableSystemReadyComplete?.Invoke();
        StopCoroutine(_CheckaddressableAllLoad);
        _CheckaddressableAllLoad = null;

    }


    void SetPatchInfo(float per,string info)
    {
        if (_patchUI != null)
            _patchUI.SetPatchInfo(per, info);
    }


    private void OnGUI()
    {
        /*
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        string text = curProgress;
        GUI.Label(rect, text, style);

        */

    }

    public  IEnumerator SetCountry()
    {
        string ip = new System.Net.WebClient().DownloadString("https://api.ipify.org");
        string uri = $"https://ipapi.co/{ip}/json/";


        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            //IpApiData ipApiData = IpApiData.CreateFromJSON(webRequest.downloadHandler.text);

            //Debug.Log(ipApiData.country_name);
        }
    }

    public static IEnumerator AddressableLoadAsync<T>(AssetReference asset,  UnityAction<T,bool> callback)
    {
        T data = default(T);

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(asset);

        while (!handle.IsDone)
        {
            yield return null;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {

            if (handle.Result != null)
            {
                data = handle.Result;

                callback?.Invoke(data, true);


            }
        }
        else
        {
            callback?.Invoke(data, false);
        }
    }
}
