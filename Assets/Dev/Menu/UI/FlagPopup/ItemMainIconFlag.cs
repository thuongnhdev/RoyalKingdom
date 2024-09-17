using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMainIconFlag : MonoBehaviour
{
    [SerializeField]
    private Image _imgIconFlag;

    [SerializeField]
    private GameObject _objHighlight;

    private int _index;
    private Action<int, BaseDataSymbolIconMain> _onComplete;
    private BaseDataSymbolIconMain _baseSymbolIconMain;
    public BaseDataSymbolIconMain BaseSymbolIconMain => _baseSymbolIconMain;
    public void InitData(int index,Sprite icon, BaseDataSymbolIconMain baseSymbolIconMain,Action<int, BaseDataSymbolIconMain> onComplete)
    {
        _index = index;
        _onComplete = onComplete;
        _imgIconFlag.sprite = icon;
        _baseSymbolIconMain = baseSymbolIconMain;
    }

    public void OnClickSelect()
    {
        _objHighlight.SetActive(true);
        _onComplete?.Invoke(_index, _baseSymbolIconMain);
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
