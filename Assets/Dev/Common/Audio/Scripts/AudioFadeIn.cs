using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeIn : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private bool _groupFade = false;
    [SerializeField]
    private FloatVariable _groupVolume;
    [SerializeField]
    private Ease _fadeType;

    [Header("Reference - Read")]
    [SerializeField]
    private AudioSource _audioSource;

    public void DoFadeIn(float fadeTime)
    {
        if (_groupFade)
        {
            _groupVolume.Value = 0f;
            DOTween.To(() => _groupVolume.Value, value => _groupVolume.Value = value, 1f, fadeTime).SetEase(_fadeType);

            return;
        }

        AudioSourceFade(fadeTime);
    }

    private void AudioSourceFade(float fadeTime)
    {
        _audioSource.volume = 0f;
        DOTween.To(() => _audioSource.volume, value => _audioSource.volume = value, 1f, fadeTime).SetEase(_fadeType);
    }
}
