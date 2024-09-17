using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatusBoardContent : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private TextMeshProUGUI _tmpPriority;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberWorker;

    [SerializeField]
    private TextMeshProUGUI _tmpSizeTaskRemain;

    [SerializeField]
    private GameObject _prefabItemJob;

    [SerializeField]
    private GameObject _pinEnable;

    [SerializeField]
    private GameObject _pinDisable;

    [SerializeField]
    private RectTransform _parentItemJob;

    [SerializeField]
    private GameObject _objBtnAdd;

    [SerializeField]
    private GameEvent _onUpdateWorkerInProgress;

    [SerializeField]
    private GameEvent _onUpdateProductionStatusBoard;

    private int _index;
    private int _priority;
    private int _indexDefaul;
    private Action<TaskData> _onPlace;
    private Action<int> _onClickPin;
    private Action<int,int> _onClickPinDisable;
    private BaseDataEnumGame _baseDataEnumGame;
    public BaseDataEnumGame BaseDataEnumGame() => _baseDataEnumGame;
    private List<TaskData> _taskDatas;
    private List<ItemJobStatusBoard> _itemJobs = new List<ItemJobStatusBoard>();


    public enum PriorityTypeShow
    {
        Defaul = 1,
        PrePriority = 2,
        SamePriority = 3,
        OtherPriority = 4
    }

    private void OnEnable()
    {
        _onUpdateWorkerInProgress.Subcribe(OnUpdateWorkerInProgress);
        _onUpdateProductionStatusBoard.Subcribe(OnUpdateProductionStatusBoard);

    }

    private void OnDisable()
    {
        _onUpdateWorkerInProgress.Unsubcribe(OnUpdateWorkerInProgress);
        _onUpdateProductionStatusBoard.Unsubcribe(OnUpdateProductionStatusBoard);
    }

    public void OnUpdateWorkerInProgress(object[] eventParam)
    {
        TypeJobAction type = (TypeJobAction)_baseDataEnumGame.Id;
        var ListData = GameData.Instance.SavedPack.SaveData.ListTask.FindAll(t => t.TypeJobAction == type);
        if (ListData.Count > 0)
            _taskDatas = ListData;
        _tmpPriority.text = IsPriorityInfinity(_taskDatas);
        UpdateUI();
    }

    public void OnUpdateProductionStatusBoard(object[] eventParam)
    {
        TypeJobAction type = (TypeJobAction)_baseDataEnumGame.Id;
        var ListData = GameData.Instance.SavedPack.SaveData.ListTask.FindAll(t => t.TypeJobAction == type);
        if (ListData.Count > 0)
            _taskDatas = ListData;
        UpdateUI();
        InitJob(_taskDatas);
    }

    private string IsPriorityInfinity(List<TaskData> taskDatas)
    {
        if (taskDatas.Count == 0)
        {
            if (_baseDataEnumGame.PrePriority > 0)
            {
                return _baseDataEnumGame.PrePriority.ToString();
            }
            else
            {
                _baseDataEnumGame.PrePriority = -1;
                return "-";
            }
        }
        else
        {
            int priorityDefaul = taskDatas[0].Priority;
            for (int i = 1; i < taskDatas.Count; i++)
            {
                if (taskDatas[i].Priority != priorityDefaul)
                {
                    _baseDataEnumGame.PrePriority = -1;
                    return "-";
                }
            }
            if (_baseDataEnumGame.PrePriority > 0)
                return priorityDefaul.ToString();
            return "-";
        }
    }

    public bool IsPin()
    {
        if (_pinEnable.activeInHierarchy)
            return true;
        return false;
    }

    public BaseDataEnumGame GetBaseDataEnumGame()
    {
        return _baseDataEnumGame;
    }

    public void InitData(int index,BaseDataEnumGame baseDataEnumGame, List<TaskData> taskData,Action<TaskData> onPlace,Action<int> onClickPin, Action<int,int> onClickPinDisable)
    {
        _index = index;
        _indexDefaul = index;
        _priority = 0;
        if (taskData.Count > 0)
            _priority = taskData[0].Priority;
        _taskDatas = taskData;
        _onPlace = onPlace;
        _onClickPin = onClickPin;
        _onClickPinDisable = onClickPinDisable;
        _baseDataEnumGame = baseDataEnumGame;
        _tmpSizeTaskRemain.text = "0";
        _pinDisable.SetActive(true);
        _pinEnable.SetActive(false);
        _objBtnAdd.SetActive(false);
        UpdateUI();
        InitJob(_taskDatas);
        if(baseDataEnumGame.IsPin)
        {
            _pinDisable.SetActive(false);
            _pinEnable.SetActive(true);
        }

    }

    public void OnUpdateData(int index, BaseDataEnumGame baseDataEnumGame)
    {
        _index = index;
        _baseDataEnumGame = baseDataEnumGame;
    }

    private void UpdateUI()
    {
        this._tmpName.text = _baseDataEnumGame.Enum.ToString();
        _tmpPriority.text = IsPriorityInfinity(_taskDatas);
        int numberWorker = 0;
        for (int i = 0; i < _taskDatas.Count; i++)
        {
            numberWorker = numberWorker + _taskDatas[i].Man;
        }
        this._tmpNumberWorker.text = numberWorker.ToString();
        if (_taskDatas.Count > 4)
        {
            _objBtnAdd.SetActive(true);
            _tmpSizeTaskRemain.text = string.Format("+{0}", (_taskDatas.Count - 4).ToString());
        }
    }

    private void InitJob(List<TaskData> taskDatas)
    {
        foreach (Transform child in _parentItemJob)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < taskDatas.Count; i++)
        {
            GameObject item = Instantiate(_prefabItemJob, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentItemJob;

            ItemJobStatusBoard itemJobStatusBoard = item.GetComponent<ItemJobStatusBoard>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            itemJobStatusBoard.InitData(i, taskDatas[i], (priority, b) => { });
            _itemJobs.Add(itemJobStatusBoard);
        }
    }

    public void OnEndEdit()
    {
        int newPriority = Int32.Parse(_tmpPriority.text);
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListTask.Count; i++)
        {
            for(int j = 0; j < _taskDatas.Count;j++)
            {
                if (_taskDatas[j].BuildingId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingId &&
                _taskDatas[j].BuildingObjectId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingObjectId &&
                _taskDatas[j].Name == GameData.Instance.SavedPack.SaveData.ListTask[i].Name)
                {
                    GameData.Instance.SavedPack.SaveData.ListTask[i].Priority = newPriority;
                }
            }
            
        }
        for (int i = 0; i < _taskDatas.Count; i++)
        {
            var itemWorkerAssignments = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Find(t => t.Priority == _taskDatas[i].Priority);
            if (itemWorkerAssignments != null)
            {
                var itemTask = itemWorkerAssignments.DataAssignments.Find(t => t.TaskId == _taskDatas[i].TaskId);
                if (itemTask != null)
                {
                    var index = itemWorkerAssignments.DataAssignments.FindIndex(t => t.TaskId == _taskDatas[i].TaskId);
                    itemWorkerAssignments.DataAssignments.RemoveAt(index);
                }
            }
        }
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListTask.Count; i++)
        {
            for (int j = 0; j < _taskDatas.Count; j++)
            {
                if (_taskDatas[j].BuildingId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingId &&
                _taskDatas[j].BuildingObjectId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingObjectId &&
                _taskDatas[j].Name == GameData.Instance.SavedPack.SaveData.ListTask[i].Name)
                {
                    _taskDatas[j].Priority = newPriority;
                    GameData.Instance.RemoveJobWorkerAssignment(_taskDatas[j].TaskId);
                    GameData.Instance.AddJobWorkerAssignment(newPriority, _taskDatas[j].TaskId, _taskDatas[j].Man, _taskDatas[j].Name, _taskDatas[j].TypeJobAction);
                }
            }

        }
        
        GameData.Instance.RequestSaveGame();
        _onUpdateWorkerInProgress?.Raise();
    }


    public void OnClickShowDetail()
    {
        UIManagerTown.Instance.ShowUIStatusBoardDetail(_baseDataEnumGame, (taskData)=> { OnClickPlace(taskData); });
    }

    public void OnClickEditSortEnable()
    {
        _pinDisable.SetActive(true);
        _pinEnable.SetActive(false);
        _baseDataEnumGame.IsPin = false;
        _onClickPinDisable?.Invoke(_index, _indexDefaul);
    }

    public void OnClickEditSortDisable()
    {
        _pinDisable.SetActive(false);
        _pinEnable.SetActive(true);
        _baseDataEnumGame.IsPin = true;
        _onClickPin?.Invoke(_index);
    }

    public void OnClickChangeAllPriority()
    {

    }

    public void OnClickPlace(TaskData taskData)
    {
        _onPlace?.Invoke(taskData);
    }

    public void OnClickChangePriority()
    {
        int priorityDefaul;
        if (string.Compare(_tmpPriority.text, "-") != 0)
            priorityDefaul = int.Parse(_tmpPriority.text);
        else
            priorityDefaul = 1;

        if (priorityDefaul < 1) return;
        if (priorityDefaul == 1)
            priorityDefaul = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count;
        else
            priorityDefaul--;
        int newPriority = priorityDefaul;
        _tmpPriority.text = newPriority.ToString();
        _baseDataEnumGame.PrePriority = newPriority;
        OnEndEdit();
    }

}
