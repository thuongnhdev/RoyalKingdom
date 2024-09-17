using DataCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoSingleton<CoreManager>
{
    public AssetManager AssetManager;
    public MixerController MixerController;
    public ShareUIManager ShareUIManager;
    public SoundController SoundController;
}
