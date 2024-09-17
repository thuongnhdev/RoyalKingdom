using DataCore;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System;
using DataCore;

public class SoundController : MonoSingleton<SoundController>
{
    [SerializeField]
    private AudioSource bgMusicAudioSource;
    [SerializeField]
    private AudioClip[] musics;
    [SerializeField]
    private AudioClip[] effects;

    [SerializeField]
    private AudioSource audioClick;
    [SerializeField]
    private List<string> listMusicActive = new List<string>();

    public List<string> ListMusicActive { get => listMusicActive; set => listMusicActive = value; }

    [SerializeField]
    private GameEvent _onUpdateMusic;

    private void OnEnable()
    {
        _onUpdateMusic.Subcribe(OnMusicMute);
    }

    private void OnDisable()
    {
        _onUpdateMusic.Unsubcribe(OnMusicMute);
    }

    private void OnMusicMute(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }
        bool isMute = (bool)eventParam[0];

        if (!isMute)
            StopBGMusicEffect();
        else
            PlayMainBackgroundMusic();
    }

    public void PlayMainBackgroundMusic()
    {
        if (!ShareUIManager.IsMusicActive)
        {
            return;
        }
        string mainBackgroundMusicPath = "Assets/Bundles/Music/ROK_Theme.mp3";
        AssetManager.Instance.LoadPathAsync<AudioClip>(mainBackgroundMusicPath, (bgMusic) =>
        {
            if (bgMusic != null)
            {
                PlayBGMusic(bgMusic, mainBackgroundMusicPath, 0.7f);
            }
        });
    }

    public void PlaySfxWin()
    {
        if (!ShareUIManager.IsSoundActive)
        {
            return;
        }

        GameObject go = new GameObject("SfxWin");
        go.transform.parent = transform;
        AudioSource audio = go.AddComponent<AudioSource>();
        audio.clip = MCache.Instance.SfxWin;
        audio.Play();
        Destroy(go.gameObject, 3);
    }

    public void PlaySfxClick()
    {
        if (!ShareUIManager.IsSoundActive)
        {
            return;
        }
        audioClick.volume = 0.7f;
        audioClick.clip = effects[0];
        audioClick.Play();
    }

    public void PlaySfxClickTing()
    {
        if (!ShareUIManager.IsSoundActive)
        {
            return;
        }
        audioClick.volume = 1.0f;
        audioClick.clip = effects[1];
        audioClick.Play();
    }

    public void StopBGMusic()
    {
        StopBGMusicEffect();
    }

    private void StopBGMusicEffect(Action onComplete = null)
    {
        if (bgMusicAudioSource.isPlaying)
        {
            DOVirtual.Float(1f, 0.0f, 2f, (value) =>
            {
                bgMusicAudioSource.volume = value;
            });

            bgMusicAudioSource.DOFade(0, 2f).OnComplete(() =>
             {
                 bgMusicAudioSource.Stop();
                 bgMusicAudioSource.volume = 1;
                 if (!String.IsNullOrEmpty(playingBackgroundMusicName)) {
                     playingBackgroundMusicName = string.Empty;
                     AssetManager.Instance.ReleasePath(playingBackgroundMusicName);
                 }                 
                 onComplete?.Invoke();
             });
        }
        else
            onComplete?.Invoke();
    }

    public void MuteBgMusic(bool isMute)
    {
        bgMusicAudioSource.volume = isMute == true ? 0 : 1;
    }

    private string playingBackgroundMusicName = string.Empty;
    
    public void PlayBGMusic(AudioClip clip, string path ,float volume = 1)
    {
        if (!ShareUIManager.IsMusicActive)
        {
            bgMusicAudioSource.clip = clip;
            return;
        }
        if (clip == null) return;
        if (!string.IsNullOrEmpty(playingBackgroundMusicName)) {
            Debug.Log($"playingBackgroundMusicName: {path} clip.name: {path}");
            if (playingBackgroundMusicName == clip.name) return;
        }

        StopBGMusicEffect(() =>
        {
            playingBackgroundMusicName = path;
            bgMusicAudioSource.Stop();
            bgMusicAudioSource.clip = clip;
            bgMusicAudioSource.loop = true;

            bgMusicAudioSource.volume = 0;
            bgMusicAudioSource.Play();
            
            DOVirtual.Float(0.0f, volume, 5f, (value) =>
            {
                bgMusicAudioSource.volume = value;
            });
        });
    }

    public void PlayBgMusicAgain()
    {
        bgMusicAudioSource.volume = 1;
        bgMusicAudioSource.Play();
        bgMusicAudioSource.loop = true;
    }

    public void PlayVibrate()
    {
        if (!ShareUIManager.IsHapticActive)
        {
            return;
        }

        Debug.Log("Play Vibrate");

        // Handheld.Vibrate();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
