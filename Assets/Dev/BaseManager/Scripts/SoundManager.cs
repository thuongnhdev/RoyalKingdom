using System.Collections;
using UnityEngine;
using Network.Common.Static;

public class SoundManager : MonoBehaviour
{
    #region Sub Singleton of BaseManager
    private static SoundManager instance = null;
    
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                if (BaseManager.Instance == null)
                {
                    return null;
                }
                else
                {
                    instance = BaseManager.Instance.GetSoundManager();
                }
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;        
    }
    #endregion


    [Header("Audio Source")]
    [SerializeField]
    private AudioSource SourceBgm;
    [SerializeField]
    private AudioSource SourceMfx;
    [SerializeField]
    private AudioSource SourceEffect;

    [Header("File_Include")]
    [SerializeField]
    private AudioClip BgmTitle;

    [Header("File_Addressable")]
    private AudioClip[] BgmList;
    private AudioClip[] MfxList;
    private AudioClip[] EffectList;

    private int CurrentBgmIndex = -1;
    private int CurrentMfxIndex = -1;

    private bool IsInit = false;

    private void Start()
    {
        InitObj();
    }

    private void InitObj()
    {
        if (!IsInit)
        {
            IsInit = true;
            SourceBgm.loop = false;
            SourceMfx.loop = false;
            SourceEffect.loop = false;
        }
    }
    public void BgmPlay(int type)
    {
        if (CurrentBgmIndex == type)
        {
            return;
        }

        if (StatesGlobal.OPT_BGM)
        {
            CurrentBgmIndex = type;
            StartCoroutine(CoPlayBgmSound());
        }
    }
    IEnumerator CoPlayBgmSound()
    {
        bool isDecress = SourceBgm.isPlaying;

        if (!isDecress)
        {
            SourceBgm.volume = 0f;
            LoadBgmInfo();
        }
        float offsetVolum = StatesGlobal.OPT_BGM_VOLUME * 0.1f;
        while (true)
        {
            if (isDecress)
            {
                if (SourceBgm.volume <= 0)
                {
                    isDecress = false;
                    LoadBgmInfo();
                }
                SourceBgm.volume -= offsetVolum;
            }
            else
            {
                if (SourceBgm.volume >= StatesGlobal.OPT_BGM_VOLUME)
                {
                    break;
                }
                SourceBgm.volume += offsetVolum;
            }
            yield return null;
        }
        if (isDecress)
        {
            SourceBgm.volume = 0f;
        }
        else
        {
            SourceBgm.volume = StatesGlobal.OPT_BGM_VOLUME;
        }

        SourceBgm.Play();
    }
    private void LoadBgmInfo()
    {
        InitObj();
        SourceBgm.Stop();
        if (CurrentBgmIndex == ConstSound._BGM_NONE)
        {
            SourceBgm.clip = null;
        }
        else
        {
            if (CurrentBgmIndex == ConstSound._BGM_TITLE)
            {
                SourceBgm.clip = BgmTitle;
            }
            else
            {
                SourceBgm.clip = BgmList[CurrentBgmIndex];
            }
        }
        switch (CurrentBgmIndex)
        {
            case ConstSound._BGM_TITLE:
                SourceBgm.loop = false;
                break;
            case ConstSound._BGM_TEMP_1:
                SourceBgm.loop = true;
                break;
        }
    }

    public void MfxPlay(int type)
    {
        if (CurrentMfxIndex == type)
        {
            return;
        }
        if (StatesGlobal.OPT_EFFECT)
        {
            CurrentMfxIndex = type;
            SourceMfx.clip = MfxList[CurrentMfxIndex];
            SourceMfx.volume = StatesGlobal.OPT_EFFECT_VOLUME;
            SourceMfx.Play();
        }
    }    
    public void MfxStop()
    {
        CurrentMfxIndex = -1;
        SourceMfx.Stop();
    }

    public void PlayOneShot_Effect(int index, float volumeScale = 1f)
    {
        if (index < 0) return;

        if (StatesGlobal.OPT_EFFECT)
        {
            if (EffectList.Length > index)
            {
                if (EffectList[index] != null)
                {
                    SourceEffect.PlayOneShot(EffectList[index], volumeScale * StatesGlobal.OPT_EFFECT_VOLUME);
                }
            }
        }
    }
}
