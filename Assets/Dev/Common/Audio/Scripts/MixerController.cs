using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoSingleton<MixerController>
{
    private const string MASTER_VOLUME = "Master_Volume";
    private const string BGM_VOLUME = "BGM_Volume";
    private const string MFX_VOLUME = "MFX_Volume";
    private const string VOICE_VOLUME = "Voice_Volume";
    private const string EFFECT_VOLUME = "Effect_Volume";

    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _masterVolume;
    [SerializeField]
    private FloatVariable _bgmVolume;
    [SerializeField]
    private FloatVariable _mfxVolume;
    [SerializeField]
    private FloatVariable _effectVolume;
    [SerializeField]
    private FloatVariable _voiceVolume;

    [Header("Config")]
    [SerializeField]
    private AudioMixer _mixer;

    private float FromLinearVolumeToDB(float linearVolume)
    {
        if (linearVolume == 0f)
        {
            return -144f;
        }
        return 20f * Mathf.Log10(linearVolume);
    }

    protected override void DoOnEnable()
    {
        _masterVolume.OnValueChange += UpdateMasterVolume;
        _bgmVolume.OnValueChange += UpdateBGMVolume;
        _mfxVolume.OnValueChange += UpdateMFXVolume;
        _effectVolume.OnValueChange += UpdateEffectVolume;
        _voiceVolume.OnValueChange += UpdateVoiceVolume;

        _masterVolume.Value = PlayerPrefs.GetFloat(MASTER_VOLUME, 1f);
        _bgmVolume.Value = PlayerPrefs.GetFloat(BGM_VOLUME, 1f);
        _effectVolume.Value = PlayerPrefs.GetFloat(EFFECT_VOLUME, 1f);
        _voiceVolume.Value = PlayerPrefs.GetFloat(VOICE_VOLUME, 1f);
    }

    protected override void DoOnDisable()
    {
        _masterVolume.OnValueChange -= UpdateMasterVolume;
        _bgmVolume.OnValueChange -= UpdateBGMVolume;
        _mfxVolume.OnValueChange -= UpdateMFXVolume;
        _effectVolume.OnValueChange -= UpdateEffectVolume;
        _voiceVolume.OnValueChange -= UpdateVoiceVolume;
    }

    private void UpdateVoiceVolume(float newValue)
    {
        _mixer.SetFloat(VOICE_VOLUME, FromLinearVolumeToDB(newValue));
        PlayerPrefs.SetFloat(VOICE_VOLUME, newValue);
    }

    private void UpdateEffectVolume(float newValue)
    {
        _mixer.SetFloat(EFFECT_VOLUME, FromLinearVolumeToDB(newValue));
        PlayerPrefs.SetFloat(EFFECT_VOLUME, newValue);
    }

    private void UpdateBGMVolume(float newValue)
    {
        _mixer.SetFloat(BGM_VOLUME, FromLinearVolumeToDB(newValue));
        PlayerPrefs.SetFloat(BGM_VOLUME, newValue);
    }

    private void UpdateMFXVolume(float newValue)
    {
        _mixer.SetFloat(MFX_VOLUME, FromLinearVolumeToDB(newValue));
        PlayerPrefs.SetFloat(MFX_VOLUME, newValue);
    }

    private void UpdateMasterVolume(float newValue)
    {
        _mixer.SetFloat(MASTER_VOLUME, FromLinearVolumeToDB(newValue));
        PlayerPrefs.SetFloat(MASTER_VOLUME, newValue);
    }
}
