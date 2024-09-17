using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;

public class UIItemVassalChoice : MonoBehaviour
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

    [SerializeField]
    protected Image _sprGrade;

    [SerializeField]
    protected Sprite _sprGradeMale;

    [SerializeField]
    protected Sprite _sprGradeFeMale;

    private string _timeDiscount;

    protected Action<UIItemVassalChoice, bool, UIVassalChoice.TypeClickChoice> _onComplete;

    public int Key;
    protected int _buildingObjectId;
    protected VassalDataInGame _vassal;
    public void InitData(int buildingObjectId,int key,VassalDataInGame vassal, string name,Sprite avatar, float percent, string time,  Action<UIItemVassalChoice, bool, UIVassalChoice.TypeClickChoice> onComplete)
    {
        Key = key;
        _vassal = vassal;
        _tmpName.text = name;
        _avatarSprite.sprite = avatar;
        _buildingObjectId = buildingObjectId;
        _tmpPercent.text = percent.ToString();
        _timeDiscount = time;
        _onComplete = onComplete;
        _objChoice.SetActive(false);
        _objBtnChoice.SetActive(false);
        if (_vassal.IsWorking)
            _objChoice.SetActive(true);

      
        int vassalGrade = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == vassal.Data.idVassalTemplate).Grade;
        _sprGrade.sprite = _sprGradeMale;
        if (vassalGrade == 4)
            _sprGrade.sprite = _sprGradeFeMale;
    }

    private void FixedUpdate()
    {
        if (!_isCountTime) return;
        _countUpdate++;
        if (_countUpdate == 100)
        {
            _countUpdate = 0;
            UpdateProgress();
        }
    }

    private int _countUpdate = 0;
    private bool _isCountTime = false;
    private void UpdateProgress()
    {
        var _taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == _buildingObjectId);
        if (_taskData == null) return;
        if (_taskData.TimeJob <= 0) return;
        var timeCurrent = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent;

        float totalAdd = (timeCurrent.Second + (timeCurrent.Minutes * 60) + (timeCurrent.Hour * 3600) + ((int)timeCurrent.Day * 24 * 3600));
        float timeBegin = (Utils.GetTimeDataSecond(_taskData.TimeBegin) + (Utils.GetTimeDataMinutes(_taskData.TimeBegin) * 60) + (Utils.GetTimeDataHour(_taskData.TimeBegin) * 3600) + ((int)Utils.GetTimeDataDay(_taskData.TimeBegin) * 24 * 3600));
        float timeGame = (totalAdd - timeBegin) / GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
        TimeSpan time = TimeSpan.FromSeconds(timeGame);
        _tmpTimer.text = string.Format("{0}D {1}H {2}M", time.Days.ToString("00"), time.Hours.ToString("00"), time.Minutes.ToString("00"));
    }

    public void OnClickChoice()
    {
        if (_buildingObjectId < 1) return;
        UserBuilding userBuilding = _userBuildingList.GetBuilding(_buildingObjectId);
        if (userBuilding.vassalInCharge > 0)
        {
            if(_objChoice.activeInHierarchy)
            {
                if(userBuilding.vassalInCharge == _vassal.Data.idVassalTemplate)
                {
                    _objBtnChoice.SetActive(false);
                    _onComplete?.Invoke(this, false, UIVassalChoice.TypeClickChoice.NotConfirm);
                }
            }
            else
            {
                _objBtnChoice.SetActive(true);
                _onComplete?.Invoke(this, true, UIVassalChoice.TypeClickChoice.Swap);
            }
        }
        else
        {
            if(!_objChoice.activeInHierarchy)
            {
                _objBtnChoice.SetActive(true);
                _onComplete?.Invoke(this, true, UIVassalChoice.TypeClickChoice.Confirm);
            }
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

    public void OnClickOpenTimer()
    {
        _timerRectTransform.gameObject.SetActive(true);
        BtnTimerOpen.DOLocalMoveY(btnPositionOpen.localPosition.y, 0.5f).SetEase(Ease.Linear).OnComplete(()=> {
            BtnTimerClose.gameObject.SetActive(true);
            BtnTimerOpen.gameObject.SetActive(false);
            BtnTimerClose.localPosition = btnPositionOpen.localPosition;
            BtnTimerOpen.localPosition = btnPositionClose.localPosition;
        });
        _timerRectTransform.DOLocalMoveY(timerPositionOpen.localPosition.y, 0.5f).SetEase(Ease.Linear);
        var _taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == _buildingObjectId);
        if (_taskData != null)
            UpdateProgress();

    }

    public void OnClickCloseTimer()
    {
   
        BtnTimerClose.DOLocalMoveY(btnPositionClose.localPosition.y, 0.5f).SetEase(Ease.Linear).OnComplete(()=> {
            BtnTimerClose.gameObject.SetActive(false);
            BtnTimerOpen.gameObject.SetActive(true);
            BtnTimerClose.localPosition = btnPositionOpen.localPosition;
            BtnTimerOpen.localPosition = btnPositionClose.localPosition;
        });
        _timerRectTransform.DOLocalMoveY(timerPositionClose.localPosition.y, 0.5f).SetEase(Ease.Linear);
    }
}
