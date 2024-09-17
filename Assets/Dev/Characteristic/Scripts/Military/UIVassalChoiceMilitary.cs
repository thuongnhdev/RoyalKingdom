using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CoreData.UniFlow;
using UnityEngine.UI;
using CoreData.UniFlow.Common;

public class UIVassalChoiceMilitary : BasePanel
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
    
    [SerializeField]
    private RectTransform parentRectTransform;

    private int _idTroop;
    private int _indexVassal;
    private List<TroopData> _troopDatas;
    private Action<List<TroopData>> _onSelectComplete;

    [SerializeField]
    protected UserBuildingList _userBuildingList;

    private List<UIItemVassalChoiceMilitary> _keyChoices = new List<UIItemVassalChoiceMilitary>();

    private List<UIItemVassalChoiceMilitary> uIItemVassalChoices = new List<UIItemVassalChoiceMilitary>();
    public override void SetData(object[] data)
    {
        base.SetData(data);
        if (data.Length == 0) return;
        _idTroop = (int)data[0];
        _troopDatas = (List<TroopData>)data[1];
        _indexVassal = (int)data[2];
        _onSelectComplete = (Action<List<TroopData>>)data[3];
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
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentRectTransform;
            UIItemVassalChoiceMilitary uiItemVassalChoiceMilitary = item.GetComponent<UIItemVassalChoiceMilitary>();
            BaseLoginVassalInfo vassarData = GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data;
            string vassalName = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == vassarData.idVassalTemplate).LastName;
            uiItemVassalChoiceMilitary.InitData(_idTroop, _troopDatas, _indexVassal, vassarData.idVassalTemplate, GameData.Instance.SavedPack.SaveData.VassalInfos[i],
                vassalName, _sprAvatar[i], -13, "1", (itemSelect, isSelect, typeClickChoice) =>
                {
                    ConfirmAction(itemSelect, isSelect, typeClickChoice);
                });
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            uIItemVassalChoices.Add(uiItemVassalChoiceMilitary);
        }

        ResetButton();
        _btnCancel.gameObject.SetActive(true);
        _btnConfirm.gameObject.SetActive(true);
        _imgFadeConfirm.SetActive(true);
        _btnFadeConfirm.enabled = false;
        _imgFadeRemove.SetActive(true);
        var troopData = _troopDatas.Find(t => t.IdTroop == _idTroop);
        if (troopData != null)
        {
            if (troopData.Vassal.IdVassal_1 > 0 || troopData.Vassal.IdVassal_2 > 0 || troopData.Vassal.IdVassal_3 > 0)
            {
                _btnRemove.gameObject.SetActive(true);
                _btnSwap.gameObject.SetActive(true);
                _imgFadeSwap.SetActive(false);
                _btnFadeSwap.enabled = true;
            }
        }
    }

    private void ResetButton()
    {
        _btnCancel.gameObject.SetActive(false);
        _btnConfirm.gameObject.SetActive(false);
        _btnRemove.gameObject.SetActive(false);
        _btnSwap.gameObject.SetActive(false);
    }

    private void ConfirmAction(UIItemVassalChoiceMilitary itemSelect, bool isSelect,UIVassalChoice.TypeClickChoice typeClickChoice)
    {
        if (itemSelect == null && !isSelect)
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
            if (typeClickChoice == UIVassalChoice.TypeClickChoice.Confirm)
            {
                _btnFadeConfirm.enabled = true;
                _imgFadeConfirm.SetActive(false);
                _btnCancel.gameObject.SetActive(true);
                _btnConfirm.gameObject.SetActive(true);
            }
            else if (typeClickChoice == UIVassalChoice.TypeClickChoice.Swap)
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
        int _keyChoice = _keyChoices[0].Key;
        var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == _keyChoice);
        if (vassal != null)
        {
            _onSelectComplete?.Invoke(_troopDatas);
        }
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
        int _keyChoice = 0;
        var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == _keyChoices[0].Key);
        if (vassal != null) vassal.IsWorking = false;
        Close();
    }

    public void OnClickNoti()
    {
        UIManagerTown.Instance.ShowUINotification();
    }
}
