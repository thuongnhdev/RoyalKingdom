using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataCore;

public class MCache : MonoBehaviour
{
    public static MCache Instance;
    [SerializeField] ConfigVariables config;
    [SerializeField] Sprite[] arrSprite;
    [SerializeField] AudioClip sfxWin;

    public ConfigVariables Config { get => config; set => config = value; }
    public Sprite[] ArrSprite { get => arrSprite; set => arrSprite = value; }
    public AudioClip SfxWin { get => sfxWin; set => sfxWin = value; }
    public AudioClip SfxClick { get => sfxWin; set => sfxWin = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
