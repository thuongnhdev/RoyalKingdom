using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTitleStatusBoard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private GameObject _objPriority;

    [SerializeField]
    private TextMeshProUGUI _tmpPriority;

    [SerializeField]
    private Color _colorGreen;

    [SerializeField]
    private Color _colorWhite;

    [SerializeField]
    private Color _colorRed;

    [SerializeField]
    private Color _colorBlack;

    [SerializeField]
    private Image _imgBgTitle;

    [SerializeField]
    private Sprite _sprChoice;

    [SerializeField]
    private Sprite _sprNormal;

    public int Index;
    public BaseDataPriority BaseDataPriority;
    MasterDataStore masterDataStore;
    private Action<int, int> _onClickDetail;
    public void InitData(int index,int count, BaseDataPriority baseDataPriority, Action<int,int> onClickDetail)
    {
        Index = index;
        BaseDataPriority = baseDataPriority;
        _onClickDetail = onClickDetail;
        _imgBgTitle.sprite = _sprNormal;
        _tmpName.color = _colorBlack;
        _tmpPriority.text = baseDataPriority.Id.ToString();
        _tmpName.text = string.Format("{0}", BaseDataPriority.Name.ToString());
        SwitchColor(baseDataPriority.Id);
        _tmpName.gameObject.SetActive(true);
        _objPriority.gameObject.SetActive(true);

        _tmpName.color = _colorBlack;
        _imgBgTitle.sprite = _sprNormal;
        if (Index == 0)
        {
            _tmpName.color = _colorWhite;
            _imgBgTitle.sprite = _sprChoice;
        }
      
    }

    private void SwitchColor(int priority)
    {
        switch(priority)
        {
            case 1:
                _tmpPriority.color = _colorGreen;
                break;
            case 2:
            case 3:
                _tmpPriority.color = _colorWhite;
                break;
            case 4:
                _tmpPriority.color = _colorRed;
                break;
        }
    }

    public void OnClickShowDetail()
    {
        _tmpName.color = _colorWhite;
        _onClickDetail?.Invoke(Index, BaseDataPriority.Id);
        _imgBgTitle.sprite = _sprChoice;
    }

    public void ResetDetail()
    {
        _tmpName.color = _colorBlack;
        _imgBgTitle.sprite = _sprNormal;
    }

}
