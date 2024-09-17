using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmEffect : MonoBehaviour
{
    [SerializeField]
    private AudioSource _bgmSource;
    [SerializeField]
    private float _fadeTime = 0.5f;
    [SerializeField]
    private AudioFadeIn _fadeIn;
    [SerializeField]
    private AudioClipVariable _currentBgmClip;

    private void DoFadeInEffect(AudioClip newValue)
    {
        _fadeIn.DoFadeIn(_fadeTime);
        _bgmSource.Stop();
        _bgmSource.clip = newValue;
        _bgmSource.Play();
    }

    private void OnEnable()
    {
        _currentBgmClip.OnValueChange += DoFadeInEffect;
    }

    private void OnDisable()
    {
        _currentBgmClip.OnValueChange -= DoFadeInEffect;
    }


}
