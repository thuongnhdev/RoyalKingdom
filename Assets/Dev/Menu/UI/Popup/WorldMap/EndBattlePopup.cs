using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EndBattlePopup : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TextMeshProUGUI _tmpNotice;

    private Action _onComplete;
    public void InitData(string msg,Action onComplete)
    {
        _tmpNotice.text = msg;
        _onComplete = onComplete;
    }

    public void OnCLose()
    {
        this.gameObject.SetActive(false);
        _onComplete?.Invoke();
    }
}
