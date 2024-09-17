using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemJobStatusBoard : MonoBehaviour
{
    [SerializeField] 
    public TextMeshProUGUI _tmpName;

    [SerializeField]
    private Image imgFillWorkScroll;

    [Header("Town Buildings")]
    [SerializeField]
    private TownBaseBuildingSOList _townBuildingList;

    [SerializeField]
    private GameEvent _onUpdateProductionStatusBoard;

    [SerializeField]
    private GameEvent _onUpdateWorkerInProgress;

    private int _index;
    private TaskData _taskData;
    private Action<int, int> _onClickDetail;
    MasterDataStore masterDataStore;

    private void OnEnable()
    {
        _onUpdateWorkerInProgress.Subcribe(OnUpdateProductionStatusBoard);
        _onUpdateProductionStatusBoard.Subcribe(OnUpdateProductionStatusBoard);

    }

    private void OnDisable()
    {
        _onUpdateWorkerInProgress.Unsubcribe(OnUpdateProductionStatusBoard);
        _onUpdateProductionStatusBoard.Unsubcribe(OnUpdateProductionStatusBoard);
    }

    public void OnUpdateProductionStatusBoard(object[] eventParam)
    {
        var ListData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.TaskId == _taskData.TaskId);
        if (ListData != null)
            _taskData = ListData;
        UpdateProgress();
    }

    public void InitData(int index,TaskData taskData, Action<int,int> onClickDetail)
    {
        _index = index;
        _taskData = taskData;
        _onClickDetail = onClickDetail;
        var buildingName = _townBuildingList.GetBuildingName(taskData.BuildingId);
        _tmpName.text = string.Format("{0}", buildingName.ToString());
        imgFillWorkScroll.fillAmount = 0;
        UpdateProgress();
    }

    private float _timeCountDown;
    private void Update()
    {
        _timeCountDown -= Time.deltaTime;
        if (_timeCountDown > 0)
        {
            UpdateProgressTimer();
        }
    }

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
        imgFillWorkScroll.fillAmount = percent;
    }

    private void UpdateProgressTimer()
    {
        float timeGameDone = (_taskData.TimeJob - _timeCountDown) / GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
        float percent = (float)timeGameDone / _taskData.TimeJob;
        imgFillWorkScroll.fillAmount = percent;
    }


    public void OnClickShowDetail()
    {
        masterDataStore = MasterDataStore.Instance;
    }

    public void ResetDetail(int indexParent)
    {
       
    }

}
