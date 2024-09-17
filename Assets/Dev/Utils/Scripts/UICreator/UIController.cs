﻿using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField]
    private bool _isGlobalUI = false;
    public bool IsGlobalUI => _isGlobalUI;
    [SerializeField]
    private bool _showInitUIFromStart = true;
    [SerializeField]
    private List<UIPanel> _UIPanels;

    [Header("Inspec")]
    [SerializeField]
    private List<string> _uiStack;
    // Store UI panel following opening order
    private Stack<UIPanel> _panelStack = new Stack<UIPanel>();
    public int ShowingPanelCount => _panelStack.Count;

    public static event Action OnGlobalClickerBlockerActive;
    public static event Action OnAllGlobalClickerBlockersInactive;

    public event Action OnOpenAPanel;

    public UIPanel TopPanel
    {
        get
        {
            if (_panelStack.Count == 0)
            {
                return null;
            }

            return _panelStack.Peek();
        }
    }

    public void ShowInitPanels()
    {
        InitAllUIPanels().Forget();
    }

    public bool IsOnTop(UIPanel panel)
    {
        if (_panelStack.Count == 0)
        {
            return false;
        }

        var topPanel = _panelStack.Peek();

        return panel.GetInstanceID() == topPanel.GetInstanceID();
    }

    public void PushToStack(UIPanel panel)
    {
        if (0 < _panelStack.Count)
        {
            var recentPanel = _panelStack.Peek();
            recentPanel.ActiveClickBlocker(!panel.HasClickBlocker);
        }

        _panelStack.Push(panel);
        _uiStack.Add(panel.name);

        OnOpenAPanel?.Invoke();
    }

    public void PopFromStack()
    {
        if (_panelStack.Count == 0)
        {
            return;
        }

        _panelStack.Pop();
        _uiStack.RemoveAt(_uiStack.Count - 1);

        if (_panelStack.Count == 0)
        {
            return;
        }

        var previous = _panelStack.Peek();
        if (previous.Status == UIPanel.PanelStatus.Opened || previous.Status == UIPanel.PanelStatus.IsOpening)
        {
            previous.ActiveClickBlocker(true);
            return;
        }
        PopFromStack();
    }

    public void CloseAllButInitPanels()
    {
        while (_panelStack.Count > 0)
        {
            var panel = _panelStack.Peek();
            if (panel.ShowFromStart)
            {
                return;
            }

            panel.Close();
        }
    }

    public void NotifyGlobalClickerBlockerActive()
    {
        OnGlobalClickerBlockerActive?.Invoke();
    }

    public void NotifyAllGlobalClickBlockerInactive()
    {
        OnAllGlobalClickerBlockersInactive?.Invoke();
    }

    private void Start()
    {
        if (!_showInitUIFromStart)
        {
            return;
        }

        InitAllUIPanels().Forget();

    }

    private async UniTaskVoid InitAllUIPanels()
    {
        for (int i = 0; i < _UIPanels.Count; i++)
        {
            var panel = _UIPanels[i];
            if (panel == null)
            {
                Logger.LogError($"Panel at index [{i}] is null");
                continue;
            }

            panel.Init(this);

            if (panel.ShowFromStart)
            {
                // the init frame handles very heavy logic, showing animation from beginning often causes lagging
                await UniTask.DelayFrame(2);
                panel.Open();
            }
        }
    }
}
