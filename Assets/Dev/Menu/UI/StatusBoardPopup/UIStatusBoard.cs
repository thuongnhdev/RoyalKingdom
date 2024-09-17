using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataCore;
using TMPro;
using System;
using UnityEngine.UI;
using CoreData.UniFlow.Common;
using CoreData.UniFlow;
using DG.Tweening;

public class UIStatusBoard : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private Color _colorGreen;

    [SerializeField]
    private Color _colorWhite;

    [SerializeField]
    private Color _colorRed;

    [SerializeField]
    private Color _colorBlack;

    [SerializeField]
    private GameEvent _onUpdateManagerTask;

    [SerializeField]
    private GameObject _prefabItemContent;

    [SerializeField]
    private IntegerVariable _cameraFocusTile;

    [SerializeField]
    private Vector3Variable _cameraFocusPoint;

    [SerializeField]
    private UserBuildingList _userbuildingList;

    [SerializeField]
    private BuildingObjectFinder _buildingObjectFinder;

    [SerializeField]
    private RectTransform _parentItemContent;

    private List<ItemTitleStatusBoard> ItemTitleStatusBoards;

    private List<ItemStatusBoardContent> ItemContents = new List<ItemStatusBoardContent>();

    MasterDataStore masterDataStore;
    private Dictionary<int, bool> dicTypeDefaul = new Dictionary<int, bool>();

    public override void SetData(object[] data)
    {
        base.SetData(data);
        previousPanel = (BasePanel)data[0];
    }

    private void OnEnable()
    {
        _onUpdateManagerTask.Subcribe(OnUpdateManagerTask);

    }

    private void OnDisable()
    {
        _onUpdateManagerTask.Unsubcribe(OnUpdateManagerTask);
    }

    public override void Open()
    {
        base.Open();
        InitData();
    }

    public void OnUpdateManagerTask(object[] eventParam)
    {
        InitData();
    }

    private void InitData()
    {
        masterDataStore = MasterDataStore.Instance;
        ItemTitleStatusBoards = new List<ItemTitleStatusBoard>();
        List<BaseDataPriority> listItemTitle = masterDataStore.BaseDataPrioritys;
        List<TaskData> taskDatas = GameData.Instance.SavedPack.SaveData.ListTask;
        dicTypeDefaul = new Dictionary<int, bool>();
        InitContent(taskDatas);
        List<WorkerAssignment> listItem = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments;

    }

    private void InitContent(List<TaskData> taskDatas)
    {
        foreach (Transform child in _parentItemContent)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemContents.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListDataEnums.Count; i++)
        {
            List<TaskData> typeTaskList = new List<TaskData>();
            var itemEnum = GameData.Instance.SavedPack.SaveData.ListDataEnums[i];
            for (int j = 0; j < taskDatas.Count; j++)
            {
                int type = (int)Utils.GetTypeJob(taskDatas[j].TypeJobAction);
                if (type == itemEnum.Id)
                {
                    typeTaskList.Add(taskDatas[j]);
                }
            }
            if (itemEnum.Enum == "PRODUCE" || itemEnum.Enum == "BUILDING")
            {
                GameObject item = Instantiate(_prefabItemContent, new Vector3(0, 0, 0), Quaternion.identity);
                item.transform.parent = _parentItemContent;

                ItemStatusBoardContent itemStatusBoardContent = item.GetComponent<ItemStatusBoardContent>();
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                itemStatusBoardContent.InitData(i, itemEnum, typeTaskList, (taskData) => { OnPlace(taskData); }, (index) => { OnSortItemPin(index); }, (index, indexDefaul) => { OnSortItemPinReset(index, indexDefaul); });
                ItemContents.Add(itemStatusBoardContent);
            }
        }
    }

    private void OnPlace(TaskData taskData)
    {
        var point = Utils.GetTilingTransform(taskData.BuildingObjectId,_userbuildingList, _buildingObjectFinder);
        _cameraFocusPoint.Value = point.GetLocalPosition();
        Close();
    }

    private void OnSortItemPin(int index)
    {
        ItemContents[index].gameObject.transform.SetSiblingIndex(0);
        ItemContents.Clear();
        foreach (Transform child in _parentItemContent)
        {
           var item = child.gameObject.GetComponent<ItemStatusBoardContent>();
            ItemContents.Add(item);
        }
        GameData.Instance.SavedPack.SaveData.ListDataEnums.Clear();
        for (int i = 0; i < ItemContents.Count; i++)
        {
            GameData.Instance.SavedPack.SaveData.ListDataEnums.Add(ItemContents[i].GetBaseDataEnumGame());
            ItemContents[i].OnUpdateData(i, ItemContents[i].GetBaseDataEnumGame());
        }
        GameData.Instance.RequestSaveGame();
    }

    private void OnSortItemPinReset(int index, int indexDefaul)
    {
        ItemContents[index].gameObject.transform.SetSiblingIndex(indexDefaul);
        ItemContents.Clear();
        foreach (Transform child in _parentItemContent)
        {
            var item = child.gameObject.GetComponent<ItemStatusBoardContent>();
            ItemContents.Add(item);
        }

        for (int i = ItemContents.Count -1; i > 0; i--)
        {
            if (ItemContents[i].IsPin())
            {
                ItemContents[i].gameObject.transform.SetSiblingIndex(0);
                var indexPin = GameData.Instance.SavedPack.SaveData.ListDataEnums.FindIndex(t => t.Id == ItemContents[i].BaseDataEnumGame().Id);
                GameData.Instance.SavedPack.SaveData.ListDataEnums[indexPin].IsPin = true;
            }
        }

        GameData.Instance.SavedPack.SaveData.ListDataEnums.Clear();
        for (int i = 0; i < ItemContents.Count; i++)
        {
            GameData.Instance.SavedPack.SaveData.ListDataEnums.Add(ItemContents[i].GetBaseDataEnumGame());
            ItemContents[i].OnUpdateData(i, ItemContents[i].GetBaseDataEnumGame());
        }
        GameData.Instance.RequestSaveGame();
    }

    public override void Close()
    {
        base.Close();
        if (previousPanel != null)
        {
            previousPanel.Open();
            previousPanel = null;
        }
    }

    public void OnActionShowDetail(int index , int priority)
    {
        for (var i = 0; i < ItemTitleStatusBoards.Count; i++)
        {
            if (ItemTitleStatusBoards[i].BaseDataPriority.Id != priority && i != index)
                ItemTitleStatusBoards[i].ResetDetail();
        }
    }

    public void OnClickCurrentWork()
    {

    }

    public void OnClickScrollDefaul()
    {

    }

    private void OnClickScrollAuto()
    {

    }

    public void OnClickNoti()
    {
        UIManagerTown.Instance.ShowUINotification();
    }
}

