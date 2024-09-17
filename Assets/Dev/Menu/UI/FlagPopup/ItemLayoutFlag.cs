using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLayoutFlag : MonoBehaviour
{
    [SerializeField]
    private Image _imgIconFlag;

    [SerializeField]
    private GameObject _objHighlight;

    private int _index;
    private Action<int, BaseDataSymbolLayout> _onComplete;
    private BaseDataSymbolLayout _baseSymbolLayout;
    public BaseDataSymbolLayout BaseSymbolLayout => _baseSymbolLayout;
    public void InitData(int index,Sprite icon, BaseDataSymbolLayout baseSymbolLayout, Action<int, BaseDataSymbolLayout> onComplete)
    {
        _index = index;
        _onComplete = onComplete;
        _imgIconFlag.sprite = icon;
        _baseSymbolLayout = baseSymbolLayout;
    }

    public void OnClickSelect()
    {
        _objHighlight.SetActive(true);
        _onComplete?.Invoke(_index,_baseSymbolLayout);
    }

    public void OnSelect()
    {
        _objHighlight.SetActive(true);
    }

    public void OnReset()
    {
        _objHighlight.SetActive(false);
    }
}
