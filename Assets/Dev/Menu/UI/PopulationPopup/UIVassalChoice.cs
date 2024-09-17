using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CoreData.UniFlow;
using UnityEngine.UI;

public class UIVassalChoice : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private Button _btnFadeConfirm;

    [SerializeField]
    private GameObject _imgFadeConfirm;

    [SerializeField]
    private Button _btnFadeSwap;

    [SerializeField]
    private GameObject _imgFadeSwap;

    [SerializeField]
    private Button _btnSwap;

    [SerializeField]
    private Button _btnRemove;

    [SerializeField]
    private GameObject _imgFadeRemove;

    [SerializeField]
    private Button _btnCancel;

    [SerializeField]
    private Button _btnConfirm;

    [SerializeField]
    private GameObject _popupNotification;

    [SerializeField]
    private Sprite[] _sprAvatar;

    [Header("Reference - Events out")]
    [SerializeField]
    private GameEvent _onUpdateVassalChoice;

    [SerializeField]
    private RectTransform parentRectTransform;

    [SerializeField]
    protected UserBuildingList _userBuildingList;

    public enum TypeClickChoice
    {
        Confirm =1,
        Swap,
        NotConfirm
    }

    private int _buildingObjectId = -1;
    private List<UIItemVassalChoice> _keyChoices = new List<UIItemVassalChoice>();

    private List<UIItemVassalChoice> uIItemVassalChoices = new List<UIItemVassalChoice>();
    public override void SetData(object[] data)
    {
        base.SetData(data);
        if (data.Length > 0) _buildingObjectId = (int)data[0];
    }

    public override void Open()
    {
        base.Open();
        _keyChoices.Clear();
        foreach (Transform child in parentRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        _popupNotification.SetActive(false);
        uIItemVassalChoices.Clear();
        _btnFadeConfirm.enabled = true;
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentRectTransform;
            UIItemVassalChoice uiItemVassalChoice = item.GetComponent<UIItemVassalChoice>();
            BaseLoginVassalInfo vassarData = GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data;
            string vassalName = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == vassarData.idVassalTemplate).LastName;
            uiItemVassalChoice.InitData(_buildingObjectId, vassarData.idVassalTemplate, GameData.Instance.SavedPack.SaveData.VassalInfos[i],
                vassalName, _sprAvatar[i], -13, "1", (itemSelect, isSelect, typeClickChoice) =>
            {
                ConfirmAction(itemSelect, isSelect, typeClickChoice);
            });
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            uIItemVassalChoices.Add(uiItemVassalChoice);

            if (!GameData.Instance.SavedPack.SaveData.VassalInfos[i].IsWorking)
                _btnFadeConfirm.enabled = false;
        }

        ResetButton();
        UserBuilding userBuilding = _userBuildingList.GetBuilding(_buildingObjectId);
        _imgFadeRemove.SetActive(true);
        if (userBuilding.vassalInCharge > 0)
        {
            _btnRemove.gameObject.SetActive(true);
            _btnSwap.gameObject.SetActive(true);
            _imgFadeSwap.SetActive(true);
            _btnFadeSwap.enabled = false;
        }
        else
        {
            _btnCancel.gameObject.SetActive(true);
            _btnConfirm.gameObject.SetActive(true);
            _imgFadeConfirm.SetActive(true);
            _btnFadeConfirm.enabled = false;
        }

    }

    private void ResetButton()
    {
        _btnCancel.gameObject.SetActive(false);
        _btnConfirm.gameObject.SetActive(false);
        _btnRemove.gameObject.SetActive(false);
        _btnSwap.gameObject.SetActive(false);
    }

    private void ConfirmAction(UIItemVassalChoice itemSelect, bool isSelect, TypeClickChoice typeClickChoice)
    {
        if(itemSelect == null && !isSelect)
        {
            return;
        }
        ResetButton();
        if (!isSelect)
        {
            _keyChoices.Clear();
            _keyChoices.Add(itemSelect);
            _btnRemove.gameObject.SetActive(true);
            _btnSwap.gameObject.SetActive(true);
            _imgFadeSwap.SetActive(true);
            _btnFadeSwap.enabled = false;
            _imgFadeRemove.SetActive(false);
        }
        else 
        {
            _keyChoices.Clear();
            _keyChoices.Add(itemSelect);
            _imgFadeRemove.SetActive(true);
            if (typeClickChoice == TypeClickChoice.Confirm)
            {
                _btnFadeConfirm.enabled = true;
                _imgFadeConfirm.SetActive(false);
                _btnCancel.gameObject.SetActive(true);
                _btnConfirm.gameObject.SetActive(true);
            }
            else if(typeClickChoice == TypeClickChoice.Swap)
            {
                _btnRemove.gameObject.SetActive(true);
                _btnSwap.gameObject.SetActive(true);
                _imgFadeSwap.SetActive(false);
                _btnFadeSwap.enabled = true;
            }
        
        }
        
        foreach (var item in uIItemVassalChoices)
        {
            item.Reset();
            var itemS = _keyChoices.Find(t => t.Key == itemSelect.Key);
            if (itemS != null) itemS.OnChoice();
        }
    }

    public override void Close()
    {
        base.Close();
        foreach (Transform child in parentRectTransform.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (previousPanel != null)
        {
            previousPanel.Open();
            previousPanel = null;
        }
    }

    public void OnClickConfirm()
    {
        if (_keyChoices.Count == 0) return;
        //_onUpdateVassalChoice?.Raise(_buildingObjectId, _keyChoice);
        var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == _buildingObjectId);
        var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == _keyChoices[0].Key);
        int taskId = 0;
        if (taskData != null) taskId = taskData.TaskId;
        _ = APIManager.Instance.RequestVassalChoice(_buildingObjectId, vassal.Data.idVassalPlayer, 1, 0, taskId);
        Close();
    }

    public void OnClickCancel()
    {
        Close();
    }

    public void OnClickSwap()
    {
        OnClickConfirm();
    }

    public void OnClickRemove()
    {
        if (_keyChoices.Count == 0) return;
        var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == _buildingObjectId);
        var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == _keyChoices[0].Key);
        _ = APIManager.Instance.RequestVassalChoice(_buildingObjectId, vassal.Data.idVassalPlayer, 0, 1, taskData.TaskId);
        //int _keyChoice = 0;
        //_onUpdateVassalChoice?.Raise(_buildingObjectId, _keyChoice);
       
        //if (vassal != null) vassal.IsWorking = false;
        Close();
    }

    public void OnClickNoti()
    {
        UIManagerTown.Instance.ShowUINotification();
    }
}
