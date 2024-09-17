using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;



public interface IGetAddressableLoad
{
    bool isComplete { get; }
}

public interface IGetAddressableLoad<T> : IGetAddressableLoad
{
    void OnAddressableLoadComplete(string file,T asset);

    void OnAddressableCompleteEnd();
    
}

public interface IReceiveAddressableSystemEvent
{
    void OnDownLoadStart();
    void OnAddressableCompleteEnd();
    void OnAddressableDownLoadComplete();
    //다운로드 실패 
    void AddressableDownLoadFail(string errorMsg);

}

public interface IAddressableLoad
{
    void LoadStart();
    bool isComplete { get; }
}



public abstract class AbstractAbbressableLoad<T> : MonoBehaviour, IAddressableLoad
{
    [SerializeField]
    protected AssetLabelReference _labelReference;

    protected UnityAction<string,T> OnLoadCompleteEvent;
    protected UnityAction OnIsCompleteEvent;

    protected IList<IResourceLocation> _locations;
    protected List<T> _gameObjects = new List<T>();

    public bool isComplete { private set; get; }

    protected virtual void Awake()
    {

        if (AddressableLoadManager.Instance == null)
            return;

        IReceiveAddressableSystemEvent IAddressableSystemReadyComplete = gameObject.GetComponent<IReceiveAddressableSystemEvent>();

        if (IAddressableSystemReadyComplete != null)
        {
            AddressableLoadManager.Instance.AddAddressableSystemReadyComplete(IAddressableSystemReadyComplete);
        }

        IGetAddressableLoad<T>  iAddressableLoad = gameObject.GetComponent<IGetAddressableLoad<T>>();

        if (iAddressableLoad != null)
        {
            
            OnLoadCompleteEvent = iAddressableLoad.OnAddressableLoadComplete;
            OnIsCompleteEvent = iAddressableLoad.OnAddressableCompleteEnd;

        }

        AddressableLoadManager.Instance.AddAddressableLoader(this);

    }

 

    public void LoadStart()
    {
        isComplete = false;
        GetLocations();
    }

    protected void GetLocations()
    {
        // 빌드타겟의 경로를 가져온다.
        // 경로이기 때문에 메모리에 에셋이 로드되진 않는다.
        Addressables.LoadResourceLocationsAsync(_labelReference).Completed +=
            (handle) =>
            {
                _locations = handle.Result;

                Addressables.Release(handle);

                LoadAsset();
            };
    }

    protected virtual void LoadAsset()
    {
        IPatchUI _iPatchUI = AddressableLoadManager.Instance._iPatchUI;


        string info = string.Format(GameTextData.PatchTextInit, _labelReference.labelString, "0", _locations.Count.ToString());

        if (_iPatchUI != null)
            _iPatchUI.SetPatchInfo(0, info);

        for (int i = 0; i < _locations.Count; i++)
        {
            var location = _locations[i];

            string path = "";
            // 경로를 인자로 GameObject를 생성한다.
            // 실제로 메모리에 GameObject가 로드된다.
            Addressables.LoadAssetAsync<T>(location).Completed +=
                (handle) =>
                {

                    Debug.Log(location.InternalId);

                    OnLoadCompleteEvent?.Invoke(path, handle.Result);
                    // 생성된 개체의 참조값 캐싱
                    _gameObjects.Add(handle.Result);


                    if (_locations.Count == _gameObjects.Count)
                    {
                        if (_iPatchUI != null)
                            _iPatchUI.SetPatchInfo(1, _labelReference.labelString + GameTextData.PatchTextComplete);

                        Invoke("Complete", 0.1f);
                    }
                    else
                    {
                        string info = string.Format(GameTextData.PatchTextInit, _labelReference.labelString, _gameObjects.Count.ToString(), _locations.Count.ToString());
                        float per = (float)_gameObjects.Count / (float)_locations.Count;
                        //Debug.Log("Per : " + per + " Info : " + info);

                        if (_iPatchUI != null)
                            _iPatchUI.SetPatchInfo(per, info);
                    }


                };
        }
    }

    void Complete()
    {
        isComplete = true;
        OnIsCompleteEvent?.Invoke();
    }


}



public class AddressableLoad : AbstractAbbressableLoad<GameObject>
{

    protected override void LoadAsset()
    {
        IPatchUI _iPatchUI = AddressableLoadManager.Instance._iPatchUI;


        string info = string.Format(GameTextData.PatchTextInit, _labelReference.labelString, "0", _locations.Count.ToString());

        if(_iPatchUI != null)
            _iPatchUI.SetPatchInfo(0, info);

        for (int i = 0; i < _locations.Count; i++)
        {
            var location = _locations[i];
            string path = "";
            // 경로를 인자로 GameObject를 생성한다.
            // 실제로 메모리에 GameObject가 로드된다.
            Addressables.InstantiateAsync(location, Vector3.one, Quaternion.identity).Completed +=
                (handle) =>
                {

                    Debug.Log(location.InternalId);
       
                    OnLoadCompleteEvent?.Invoke(path, handle.Result);
                    // 생성된 개체의 참조값 캐싱
                    _gameObjects.Add(handle.Result);


                    if (_locations.Count == _gameObjects.Count)
                    {
                        if (_iPatchUI != null)
                            _iPatchUI.SetPatchInfo(1, _labelReference.labelString + GameTextData.PatchTextComplete);

                        Invoke("Complete", 0.1f);
                    }
                    else
                    {
                        string info = string.Format(GameTextData.PatchTextInit, _labelReference.labelString, _gameObjects.Count.ToString(), _locations.Count.ToString());
                        float per = (float)_gameObjects.Count / (float)_locations.Count;
                        //Debug.Log("Per : " + per + " Info : " + info);

                        if (_iPatchUI != null)
                            _iPatchUI.SetPatchInfo(per, info);
                    }


                };
        }


    }

  

}
