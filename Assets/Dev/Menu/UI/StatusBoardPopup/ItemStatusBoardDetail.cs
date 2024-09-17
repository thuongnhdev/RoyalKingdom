using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoreData.UniFlow;
using System;
using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using Fbs;
using TaskData = CoreData.UniFlow.Common.TaskData;

public class ItemStatusBoardDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgFillWorkScroll;

    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private TextMeshProUGUI _tmpTime;

    [SerializeField]
    private TextMeshProUGUI _tmpWorker;

    [SerializeField]
    private TextMeshProUGUI _tmpPriority;

    [SerializeField]
    private Image _imgAvatar;

    [SerializeField]
    private Sprite _sprAvatarDefaul;

    [SerializeField]
    private GameEvent _onUpdateWorkerInProgress;

    [Header("Worker")]
    [SerializeField]
    private GameEvent _onUpdateVassalStatusBoard;

    private int _index;
    private TaskData _taskData;
    private Action<TaskData> _onPlace;
    private Sprite[] _sprAvatar;
    private UserBuildingList _userBuildingList;
    private void OnEnable()
    {
        _onUpdateVassalStatusBoard.Subcribe(OnUpdateAvatarVassal);

    }

    private void OnDisable()
    {
        _onUpdateVassalStatusBoard.Unsubcribe(OnUpdateAvatarVassal);
    }

    private async void OnUpdateAvatarVassal(object[] eventParam)
    {
        await UniTask.DelayFrame(4);
        if (eventParam.Length == 0)
        {
            return;
        }
        VassalChoiceData reponse = (VassalChoiceData)eventParam[0];
        UserBuilding userBuilding = _userBuildingList.GetBuilding(reponse.BuildingPlayerId);
        var data = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalPlayer  == reponse.IdVassal);
        if (_taskData.BuildingObjectId == reponse.BuildingPlayerId)
        {
            if (data == null)
            {
                int idAvatar = userBuilding.vassalInCharge - 1;
                _imgAvatar.sprite = _sprAvatarDefaul;
            }
            else
            {
                int idAvatar = userBuilding.vassalInCharge - 1;
                _imgAvatar.sprite = _sprAvatar[idAvatar];
            }
        }
    }

    public void InitData(int index,TaskData taskData, Sprite spriteIcon, string buildingName,Action<TaskData> onPlace, UserBuildingList userBuildingList, Sprite[] sprAvatar)
    {
        _index = index;
        _onPlace = onPlace;
        _taskData = taskData;
        _sprAvatar = sprAvatar;
        var timeData = taskData.TimeBegin;
        _tmpWorker.text = taskData.Man.ToString();
        var nameJob = buildingName;
        _imgAvatar.sprite = spriteIcon;
        _tmpName.text = string.Format("{0}: {1}", taskData.Name.ToString(), nameJob);
        _tmpPriority.text = taskData.Priority.ToString();
        imgFillWorkScroll.fillAmount = 0;
        _userBuildingList = userBuildingList;
        UpdateProgress();

    }

    private float _timeCountDown;
    private void UpdateProgress()
    {
        if (_taskData == null) return;
        if (_taskData.TimeJob <= 0 || _taskData.Man <= 0) return;
        var timeCurrent = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent;
        int level = (int)TypeJobActionBuilding.CONTRUCTION;
        if (_taskData.TypeJobAction == TypeJobAction.UPGRADE)
            level = (int)TypeJobActionBuilding.UPGRADE;

        float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(_taskData.BuildingId, level);
        if (_taskData.TypeJobAction == TypeJobAction.PRODUCE)
            workload = VariableManager.Instance.ProductFormulaList.GetTimeCost(_taskData.CurrentProductId);

        float totalAdd = _taskData.TimeJob;
        float timeCountDown = _taskData.Workload / _taskData.SpeedRate;
        _timeCountDown = timeCountDown;


        float timeGameDone = (totalAdd - timeCountDown) / GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
        float percent = (float)timeGameDone / _taskData.TimeJob;
        TimeSpan timeCountDownSpan = TimeSpan.FromSeconds(timeCountDown);
        if (timeCountDownSpan.Days > 0)
            _tmpTime.text = string.Format("{0}D {1}H", timeCountDownSpan.Days.ToString("00"), timeCountDownSpan.Hours.ToString("00"));
        else
        {
            if (timeCountDownSpan.Hours > 0)
                _tmpTime.text = string.Format("{0}H {1}M", timeCountDownSpan.Hours.ToString("00"), timeCountDownSpan.Minutes.ToString("00"));
            else
            {
                _tmpTime.text = string.Format("{0}M {1}S", timeCountDownSpan.Minutes.ToString("00"), timeCountDownSpan.Seconds.ToString("00"));
            }
        }
        imgFillWorkScroll.fillAmount = percent;
    }

    private void Update()
    {
        _timeCountDown -= Time.deltaTime;
        if (_timeCountDown > 0)
        {
            UpdateProgressTimer();
        }
    }

    private void UpdateProgressTimer()
    {
        float timeGameDone = (_taskData.TimeJob - _timeCountDown) / GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
        float percent = (float)timeGameDone / _taskData.TimeJob;
        TimeSpan timeCountDownSpan = TimeSpan.FromSeconds(_timeCountDown);
        if (timeCountDownSpan.Days > 0)
            _tmpTime.text = string.Format("{0}D {1}H", timeCountDownSpan.Days.ToString("00"), timeCountDownSpan.Hours.ToString("00"));
        else
        {
            if (timeCountDownSpan.Hours > 0)
                _tmpTime.text = string.Format("{0}H {1}M", timeCountDownSpan.Hours.ToString("00"), timeCountDownSpan.Minutes.ToString("00"));
            else
            {
                _tmpTime.text = string.Format("{0}M {1}S", timeCountDownSpan.Minutes.ToString("00"), timeCountDownSpan.Seconds.ToString("00"));
            }
        }
        imgFillWorkScroll.fillAmount = percent;
    }

    private void UpdateUI(float value)
    {
        imgFillWorkScroll.fillAmount = value;
    }

    public void OnClickChangePriority()
    {
        int priorityDefaul = _taskData.Priority;
        if (priorityDefaul == 1)
            priorityDefaul = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count;
        else
            priorityDefaul--;
        int newPriority = priorityDefaul;
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListTask.Count; i++)
        {
            if (_taskData.BuildingId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingId &&
                _taskData.BuildingObjectId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingObjectId &&
                _taskData.Name == GameData.Instance.SavedPack.SaveData.ListTask[i].Name)
            {
                GameData.Instance.SavedPack.SaveData.ListTask[i].Priority = newPriority;
                break;
            }
        }
        var itemWorkerAssignments = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Find(t => t.Priority == priorityDefaul);
        if(itemWorkerAssignments != null)
        {
            var itemTask = itemWorkerAssignments.DataAssignments.Find(t => t.TaskId == _taskData.TaskId);
            if(itemTask != null)
            {
                var index = itemWorkerAssignments.DataAssignments.FindIndex(t => t.TaskId == _taskData.TaskId);
                itemWorkerAssignments.DataAssignments.RemoveAt(index);
            }
        }
        _tmpPriority.text = newPriority.ToString();
        GameData.Instance.RemoveJobWorkerAssignment(_taskData.TaskId);
        GameData.Instance.AddJobWorkerAssignment(newPriority, _taskData.TaskId, _taskData.Man, _taskData.Name, _taskData.TypeJobAction);
        GameData.Instance.RequestSaveGame();
        _onUpdateWorkerInProgress?.Raise();
    }

    public void OnClickPlace()
    {
        _onPlace?.Invoke(_taskData);
    }

    public void OnClickChangeVassal()
    {
        UIManagerTown.Instance.ShowUIVassarChoice(_taskData.BuildingObjectId);
    }
}
