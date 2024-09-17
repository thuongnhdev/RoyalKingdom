using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItembackgroundFlag : MonoBehaviour
{
    [SerializeField]
    private Image _imgIconFlag;

    [SerializeField]
    private GameObject _objHighlight;

    private int _index;
    private Action<int, BaseDataSymbolBg> _onComplete;
    private BaseDataSymbolBg _baseSymbolIconBG;
    public BaseDataSymbolBg BaseSymbolIconBG => _baseSymbolIconBG;
    public void InitData(int index,Sprite icon, BaseDataSymbolBg baseSymbolIconBG ,Action<int, BaseDataSymbolBg> onComplete)
    {
        _index = index;
        _imgIconFlag.sprite = icon;
        _onComplete = onComplete;
        _baseSymbolIconBG = baseSymbolIconBG;
    }

    public void OnClickSelect()
    {
        _objHighlight.SetActive(true);
        _onComplete?.Invoke(_index, _baseSymbolIconBG);
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
