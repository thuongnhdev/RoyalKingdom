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
using Cysharp.Threading.Tasks;

public class UIStatusBoardDetail : BasePanel
{
    [SerializeField]
    private TextMeshProUGUI _tmpTitle;

    [SerializeField]
    private GameObject _prefabItemDetail;

    [SerializeField]
    private RectTransform _parentItemDetail;

    [SerializeField]
    private Sprite _sprIconDefaul;

    [SerializeField]
    private Sprite[] _sprAvatar;

    [SerializeField]
    private UserBuildingList _userBuildingList;

    [SerializeField]
    private GameEvent _onUpdateWorkerStatusBoard;

    [SerializeField]
    private GameEvent _onUpdateProductionStatusBoard;

    [Header("Town Buildings")]
    [SerializeField]
    private TownBaseBuildingSOList _townBuildingList;

    private List<ItemStatusBoardDetail> ItemDetails = new List<ItemStatusBoardDetail>();

    private Action<TaskData> _onPlace;

    MasterDataStore masterDataStore;

    private BaseDataEnumGame baseDataEnumGame;
    public override void SetData(object[] data)
    {
        base.SetData(data);
        baseDataEnumGame = (BaseDataEnumGame)data[0];
        _onPlace = (Action<TaskData>)data[1];
    }

    private void OnEnable()
    {
        _onUpdateWorkerStatusBoard.Subcribe(OnUpdateWorkerStatusBoard);
        _onUpdateProductionStatusBoard.Subcribe(OnUpdateProductionStatusBoard);

    }

    private void OnDisable()
    {
        _onUpdateWorkerStatusBoard.Unsubcribe(OnUpdateWorkerStatusBoard);
        _onUpdateProductionStatusBoard.Unsubcribe(OnUpdateProductionStatusBoard);
    }

    public void OnUpdateProductionStatusBoard(object[] eventParam)
    {
        InitData();
    }

    public override void Open()
    {
        base.Open();
        InitData();
    }

    private void OnUpdateWorkerStatusBoard(object[] eventParam)
    {
        InitData();
    }

    private void InitData()
    {
        _tmpTitle.text = baseDataEnumGame.Enum.ToString();
        masterDataStore = MasterDataStore.Instance;
        ItemDetails.Clear();
        List<BaseDataPriority> listItemTitle = masterDataStore.BaseDataPrioritys;
        List<TaskData> taskDatas = GameData.Instance.SavedPack.SaveData.ListTask;

        InitContent(taskDatas);
        List<WorkerAssignment> listItem = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments;
    }

    private void InitContent(List<TaskData> taskDatas)
    {
        foreach (Transform child in _parentItemDetail)
        {
            GameObject.Destroy(child.gameObject);
        }
        List<TaskData> typeTaskList = new List<TaskData>();
        for (int i = 0; i < taskDatas.Count; i++)
        {
            int type = (int)Utils.GetTypeJob(taskDatas[i].TypeJobAction);
            if (type == baseDataEnumGame.Id)
            {
                typeTaskList.Add(taskDatas[i]);
            }
        }
        for (int i = 0; i < typeTaskList.Count; i++)
        {
      
            GameObject item = Instantiate(_prefabItemDetail, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentItemDetail;

            ItemStatusBoardDetail itemStatusBoardDetail = item.GetComponent<ItemStatusBoardDetail>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            Sprite avatarIcon = _sprIconDefaul;
            UserBuilding userBuilding = _userBuildingList.GetBuilding(typeTaskList[i].BuildingObjectId);
            CharacterTaskData vassal = CharacterManager.Instance.GetCharacterTaskData().Find(t => t.BuildingObjectId == typeTaskList[i].BuildingObjectId && t.Type == Character.CharacterList.Vassar);
            if (vassal != null)
            {
                int idAvatar = userBuilding.vassalInCharge - 1;
                avatarIcon = _sprAvatar[idAvatar];
            }
            var buildingName = _townBuildingList.GetBuildingName(userBuilding.buildingId);
            itemStatusBoardDetail.InitData(i, typeTaskList[i], avatarIcon, buildingName,(taskData)=> {
                base.Close();
                _onPlace?.Invoke(taskData);
            },_userBuildingList,_sprAvatar);
            ItemDetails.Add(itemStatusBoardDetail);
        }

        int countSize = typeTaskList.Count / 2;
        int countM = typeTaskList.Count / 2;
        if (countM > 0)
            countSize++;
        int sizeheight = countSize * 400;
        if (sizeheight < 1000) sizeheight = 1000;
        _parentItemDetail.sizeDelta = new Vector2(0, sizeheight);
        _parentItemDetail.transform.localPosition = new Vector3(_parentItemDetail.transform.localPosition.x, -sizeheight / 2, _parentItemDetail.transform.localPosition.z);
    }

    public override void Close()
    {
        base.Close();
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

