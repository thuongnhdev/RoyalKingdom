using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;

public class UIItemVassalChoiceMilitary : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private Image _avatarSprite;

    [SerializeField]
    private TextMeshProUGUI _tmpPercent;

    [SerializeField]
    private TextMeshProUGUI _tmpTimer;

    [SerializeField]
    protected GameObject _objChoice;

    [SerializeField]
    protected GameObject _objBtnChoice;

    [SerializeField]
    private RectTransform _timerRectTransform;

    [SerializeField]
    private RectTransform BtnTimerOpen;

    [SerializeField]
    private RectTransform BtnTimerClose;

    [SerializeField]
    private RectTransform timerPositionOpen;

    [SerializeField]
    private RectTransform timerPositionClose;

    [SerializeField]
    private RectTransform btnPositionOpen;

    [SerializeField]
    private RectTransform btnPositionClose;

    [SerializeField]
    protected UserBuildingList _userBuildingList;

    private string _timeDiscount;

    protected Action<UIItemVassalChoiceMilitary, bool, UIVassalChoice.TypeClickChoice> _onComplete;

    public int Key;
    protected int _idTroop;
    protected int _indexVassal;
    protected VassalDataInGame _vassal;
    protected List<TroopData> _troopDatas;
    public void InitData(int idTroop,List<TroopData> troopDatas ,int indexVassal, int key, VassalDataInGame vassal, string name, Sprite avatar, float percent, string time, Action<UIItemVassalChoiceMilitary, bool, UIVassalChoice.TypeClickChoice> onComplete)
    {
        Key = key;
        _vassal = vassal;
        _tmpName.text = name;
        _avatarSprite.sprite = avatar;
        _idTroop = idTroop;
        _indexVassal = indexVassal;
        _troopDatas = troopDatas;
        _tmpPercent.text = percent.ToString();
        _timeDiscount = time;
        _onComplete = onComplete;
        _objChoice.SetActive(false);
        _objBtnChoice.SetActive(false);
        var troopData = _troopDatas.Find(t => t.IdTroop == _idTroop);
        if(troopData != null)
        {
            if (troopData.Vassal.IdVassal_1 > 0 && troopData.Vassal.IdVassal_1 == _vassal.Data.idVassalTemplate)
            {
                _objChoice.SetActive(true);
            }
            if (troopData.Vassal.IdVassal_2 > 0 && troopData.Vassal.IdVassal_2 == _vassal.Data.idVassalTemplate)
            {
                _objChoice.SetActive(true);
            }
            if (troopData.Vassal.IdVassal_3 > 0 && troopData.Vassal.IdVassal_3 == _vassal.Data.idVassalTemplate)
            {
                _objChoice.SetActive(true);
            }
        }
    }

    public void OnClickChoice()
    {
        if (_idTroop < 1) return;
        var troopData = _troopDatas.Find(t => t.IdTroop == _idTroop);
        if (troopData != null)
        {
            bool isVassalSelected = false;
            if(troopData.Vassal.IdVassal_1 == _vassal.Data.idVassalTemplate || troopData.Vassal.IdVassal_2 == _vassal.Data.idVassalTemplate
                || troopData.Vassal.IdVassal_3 == _vassal.Data.idVassalTemplate)
            {
                isVassalSelected = true;
            }
                var vassalSelect = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == _vassal.Data.idVassalTemplate);
            switch (_indexVassal)
            {
                case 0:
                    {
                        if (troopData.Vassal.IdVassal_1 != _vassal.Data.idVassalTemplate && troopData.Vassal.IdVassal_2 != _vassal.Data.idVassalTemplate
                && troopData.Vassal.IdVassal_3 != _vassal.Data.idVassalTemplate)
                        {
                            _objBtnChoice.SetActive(true);
                            troopData.Vassal.IdVassal_1 = _vassal.Data.idVassalTemplate;
                            _onComplete?.Invoke(this, true, UIVassalChoice.TypeClickChoice.Confirm);
                        }
                        else if (troopData.Vassal.IdVassal_1 == _vassal.Data.idVassalTemplate || troopData.Vassal.IdVassal_2 == _vassal.Data.idVassalTemplate
                || troopData.Vassal.IdVassal_3 == _vassal.Data.idVassalTemplate)
                        {
                            _objBtnChoice.SetActive(false);
                            _onComplete?.Invoke(this, false, UIVassalChoice.TypeClickChoice.NotConfirm);
                        }
                    }
                    break;
                case 1:
                    {
                        if (troopData.Vassal.IdVassal_1 != _vassal.Data.idVassalTemplate && troopData.Vassal.IdVassal_2 != _vassal.Data.idVassalTemplate
            && troopData.Vassal.IdVassal_3 != _vassal.Data.idVassalTemplate)
                        {
                            _objBtnChoice.SetActive(true);
                            troopData.Vassal.IdVassal_1 = _vassal.Data.idVassalTemplate;
                            _onComplete?.Invoke(this, true, UIVassalChoice.TypeClickChoice.Confirm);
                        }
                        else if (troopData.Vassal.IdVassal_1 == _vassal.Data.idVassalTemplate || troopData.Vassal.IdVassal_2 == _vassal.Data.idVassalTemplate
                || troopData.Vassal.IdVassal_3 == _vassal.Data.idVassalTemplate)
                        {
                            _objBtnChoice.SetActive(false);
                            _onComplete?.Invoke(this, false, UIVassalChoice.TypeClickChoice.NotConfirm);
                        }
                    }
                    break;
                case 2:
                    {
                        if (troopData.Vassal.IdVassal_1 != _vassal.Data.idVassalTemplate && troopData.Vassal.IdVassal_2 != _vassal.Data.idVassalTemplate
           && troopData.Vassal.IdVassal_3 != _vassal.Data.idVassalTemplate)
                        {
                            _objBtnChoice.SetActive(true);
                            troopData.Vassal.IdVassal_1 = _vassal.Data.idVassalTemplate;
                            _onComplete?.Invoke(this, true, UIVassalChoice.TypeClickChoice.Confirm);
                        }
                        else if (troopData.Vassal.IdVassal_1 == _vassal.Data.idVassalTemplate || troopData.Vassal.IdVassal_2 == _vassal.Data.idVassalTemplate
                || troopData.Vassal.IdVassal_3 == _vassal.Data.idVassalTemplate)
                        {
                            _objBtnChoice.SetActive(false);
                            _onComplete?.Invoke(this, false, UIVassalChoice.TypeClickChoice.NotConfirm);
                        }
                    }
                    break;
            }
        }
        else
        {
            _objBtnChoice.SetActive(false);
            _onComplete?.Invoke(this, false, UIVassalChoice.TypeClickChoice.NotConfirm);
        }
    }

    public void OnChoice()
    {
        _objBtnChoice.SetActive(true);
    }

    public void Reset()
    {
        _objBtnChoice.SetActive(false);
        if (!_vassal.IsWorking) _objChoice.SetActive(false);
    }


   
}
