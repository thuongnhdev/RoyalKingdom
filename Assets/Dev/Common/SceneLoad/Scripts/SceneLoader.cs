using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private bool _loadByAddressable = false;
    [SerializeField]
    private string _requestSceneName;
    [SerializeField]
    private float _loadDelay = 1f;

    [Header("Reference - Read")]
    [SerializeField]
    private SceneInBuild _sceneList;

    [Header("Reference - Read/Write")]
    [SerializeField]
    private StringVariable _requestLoadScene;
    [SerializeField]
    private BoolVariable _isLoadByAddressable;

    [SerializeField]
    private UnityEvent _onReceivedLoadRequest;
    [SerializeField]
    private UnityEvent _onStartLoad;

    public void LoadRequestedScene()
    {
        LoadRequestedScene(_requestSceneName).Forget();
    }

    public void LoadScene(string sceneName)
    {
        _requestSceneName = sceneName;
        LoadRequestedScene();
    }

    public AsyncOperation LoadRequestedSceneAsync()
    {
        _onReceivedLoadRequest.Invoke();
        _onStartLoad.Invoke();

        _requestLoadScene.Value = _requestSceneName;
        _isLoadByAddressable.Value = _loadByAddressable;

       return SceneManager.LoadSceneAsync(_requestLoadScene.Value);
    }

    public async UniTaskVoid LoadRequestedScene(string sceneName)
    {
        _onReceivedLoadRequest.Invoke();
        await UniTask.Delay(System.TimeSpan.FromSeconds(_loadDelay));
        _onStartLoad.Invoke();

        _requestLoadScene.Value = sceneName;
        _isLoadByAddressable.Value = _loadByAddressable;

        // BufferScene is responsible for loading target scene
        SceneManager.LoadScene("BufferScene");
    }

    public void BufferSceneOnly_LoadScene()
    {
        if (_isLoadByAddressable.Value)
        {
            var sceneAsset = _sceneList.GetSceneReference(_requestLoadScene.Value);
            if (sceneAsset == null)
            {
                return;
            }

            Addressables.LoadSceneAsync(sceneAsset);
            return;
        }

        SceneManager.LoadScene(_requestLoadScene.Value);
    }
}
