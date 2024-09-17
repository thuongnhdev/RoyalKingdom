using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreData.UniFlow;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

public class UIBandItem : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private Image _iconBand;

    [SerializeField]
    private TextMeshProUGUI _tmpBranch;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberBand;

    [SerializeField]
    private GameObject _prefabItemBand;

    [SerializeField]
    private RectTransform _parentItemBand;

    [SerializeField]
    private GameObject _prefabItemWarehouse;

    [SerializeField]
    private RectTransform _parentItemWarehouse;

    private Action _onComplete;
    private List<ItemBandBranch> ItemBands = new List<ItemBandBranch>();
    private List<ItemBandWarehouse> ItemWarehouses = new List<ItemBandWarehouse>();
    public override void SetData(object[] data)
    {
        base.SetData(data);
        _onComplete = (Action)data[0];
    }

    public override void Open()
    {
        base.Open();
        foreach (Transform child in _parentItemBand)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemBands.Clear();
        foreach (Transform child in _parentItemWarehouse)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemWarehouses.Clear();

        //for (int i = 0; i < MasterDataStoreGlobal.Instance.BaseBandDefauts.Count; i++)
        //{
        //    GameObject item = Instantiate(_prefabItemBand, new Vector3(0, 0, 0), Quaternion.identity);
        //    item.transform.parent = _parentItemBand;

        //    ItemBand itemBand = item.GetComponent<ItemBand>();
        //    item.transform.localPosition = Vector3.zero;
        //    item.transform.localScale = Vector3.one;
        //    itemStatusBoardContent.InitData(i, itemEnum, typeTaskList, (taskData) => { OnPlace(taskData); }, (index) => { OnSortItemPin(index); }, (index, indexDefaul) => { OnSortItemPinReset(index, indexDefaul); });
        //    ItemBands.Add(itemStatusBoardContent);
        //}
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

}
