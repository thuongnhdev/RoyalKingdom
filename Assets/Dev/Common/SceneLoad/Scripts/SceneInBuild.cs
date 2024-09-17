using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SceneInBuildList", menuName = "Uniflow/Common/Scenes In Build List")]
public class SceneInBuild : ScriptableObject
{
    [SerializeField]
    private List<AssetReference> _sceneInBuildList;

    [Header("DO NOT modify")]
    [SerializeField]
    private List<SceneAssetInfo> _sceneAssetInfos;

    public AssetReference GetSceneReference(string sceneName)
    {
        for (int i = 0; i < _sceneAssetInfos.Count; i++)
        {
            var scene = _sceneAssetInfos[i];

            if (scene.sceneName == sceneName)
            {
                return scene.sceneAsset;
            }
        }

        Debug.LogError($"Scene [{sceneName}] does not exist in SceneInBuildList");

        return null;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _sceneAssetInfos.Clear();
        for (int i = 0; i < _sceneInBuildList.Count; i++)
        {
            var sceneAsset = _sceneInBuildList[i];
            string scenePathWithoutExtension = AssetDatabase.GUIDToAssetPath(sceneAsset.AssetGUID).Split('.')[0];
            string[] splitPath = scenePathWithoutExtension.Split('/');
            string name = splitPath[splitPath.Length - 1];

            _sceneAssetInfos.Add(new SceneAssetInfo
            {
                sceneName = name,
                sceneAsset = sceneAsset
            });
        }

        EditorUtility.SetDirty(this);
    }
#endif
}

[System.Serializable]
public class SceneAssetInfo
{
    public string sceneName;
    public AssetReference sceneAsset;
}