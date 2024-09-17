using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSubIconFlag : MonoBehaviour
{
    [SerializeField]
    private Image _imgIconFlag;

    [SerializeField]
    private GameObject _objHighlight;

    private int _index;
    private Action<int, BaseDataSymbolIconSub> _onComplete;
    private BaseDataSymbolIconSub _baseSymbolIconSub;
    public BaseDataSymbolIconSub BaseSymbolIconSub => _baseSymbolIconSub;
    public void InitData(int index,Sprite icon, BaseDataSymbolIconSub baseSymbolIconSub, Action<int, BaseDataSymbolIconSub> onComplete)
    {
        _index = index;
        _imgIconFlag.sprite = icon;
        _onComplete = onComplete;
        _baseSymbolIconSub = baseSymbolIconSub;
    }

    public void OnClickSelect()
    {
        _objHighlight.SetActive(true);
        _onComplete?.Invoke(_index, _baseSymbolIconSub);
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
