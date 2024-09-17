using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;
using CoreData.UniFlow.Common;

public class ShareUIManager : MonoSingleton<ShareUIManager>
{
    public static event System.Action<float, Action> OnUpdateData = delegate { };
    public static event System.Action OnPlayAnimInkIcon = delegate { };
    [SerializeField] public ConfigVariables Config;
    private bool _isInit = false;

    public static bool IsMusicActive;
    public static bool IsHapticActive;
    public static bool IsSoundActive;
    public void Init()
    {
        if (_isInit)
        {
            return;
        }
        _isInit = true;
        UpdateCurrencyData(0);
    }

    

    public void UpdateCurrencyData(float animDuration, Action onComplete = null)
    {
        OnUpdateData?.Invoke(animDuration, onComplete);
    }

    public void PlayAnimInkIcon()
    {
        OnPlayAnimInkIcon?.Invoke();
    }

    public void OnEnableCoin(bool isActive)
    {
        if (isActive) UpdateCurrencyData(0);
    }
  
    public void OnBackBtnPress()
    {

    }

    public void OnSettingBtnPress()
    {

    }

    public void OnAddCoinPress()
    {

    }
}
