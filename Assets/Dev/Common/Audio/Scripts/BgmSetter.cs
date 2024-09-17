using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BgmSetter : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private bool _setOnEnable;
    [SerializeField]
    private AssetReference _audioClipAsset;

    [Header("Reference - Read/Write")]
    [SerializeField]
    private AssetReferenceVariable _currentClipAsset;

    [Header("Reference - Write")]
    [SerializeField]
    private AudioClipVariable _currentBgmClip;

    public async UniTaskVoid SetBgm()
    {
        await UniTask.DelayFrame(1);

        if (_currentClipAsset.Value.RuntimeKey == _audioClipAsset.RuntimeKey)
        {
            return;
        }

        var clip =  await Addressables.LoadAssetAsync<AudioClip>(_audioClipAsset);
        if (_currentBgmClip.Value != null)
        {
            Addressables.Release(_currentBgmClip.Value);
        }

        _currentBgmClip.Value = clip;

        _currentClipAsset.Value = _audioClipAsset;
    }

    private void OnEnable()
    {
        if (_setOnEnable)
        {
            SetBgm().Forget();
        }
    }
}
