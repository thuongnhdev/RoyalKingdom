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
using CoreData.UniFlow.Common;

public class UIBand : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private GameObject _prefabItemBand;

    [SerializeField]
    private RectTransform _parentItemBand;

    [SerializeField]
    private GameObject _fadeBtnConfirm;

    private Action<TroopData> _onComplete;
    private TroopData _troopData;
    private List<ItemBand> ItemBands = new List<ItemBand>();
    public List<BaseBandDefaut> ItemBandChoiced = new List<BaseBandDefaut>();

    public enum TypeSelectBand
    {
        Select = 1,
        NoSelect = 2,
        Remove = 3
    }

    public override void SetData(object[] data)
    {
        base.SetData(data);
        _troopData = (TroopData)data[0];
        _onComplete = (Action<TroopData>)data[1];
    }

    public override void Open()
    {
        base.Open();
        foreach (Transform child in _parentItemBand)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemBands.Clear();
        ItemBandChoiced.Clear();
        for (int i = 0; i < MasterDataStoreGlobal.Instance.BaseBandDefauts.Count; i++)
        {
            GameObject item = Instantiate(_prefabItemBand, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentItemBand;

            ItemBand itemBand = item.GetComponent<ItemBand>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            itemBand.InitData(this, i, _troopData, MasterDataStoreGlobal.Instance.BaseBandDefauts[i], (isSelect,type, itemB) => { OnSelectBandItem(isSelect, type, itemB); });
            ItemBands.Add(itemBand);
        }
        _fadeBtnConfirm.SetActive(true);
    }

    public RectTransform GetBandCellPosition(int bandIndex)
    {
        if (ItemBands.Count - 1 < bandIndex)
        {
            return new();
        }

        return ItemBands[bandIndex].GetComponent<RectTransform>();
    }

    private void OnSelectBandItem(bool isSelect, TypeSelectBand type, BaseBandDefaut baseBandDefaut)
    {
        _fadeBtnConfirm.SetActive(false);
        if (isSelect)
        {
            var itemBand = _troopData.Band.Find(t => t.BandKey == baseBandDefaut.BandKey);
            if (itemBand == null)
            {
                _troopData.Band.Add(baseBandDefaut);
                ItemBandChoiced.Add(baseBandDefaut);
            }
        }
        else
        {
            if(baseBandDefaut != null)
            {
                if(type == TypeSelectBand.Remove)
                {
                    var itemTroop = UIManagerTown.Instance.UITroop.TroopDatas.Find(t => t.IdTroop == _troopData.IdTroop);
                    if (itemTroop == null) return;
                    var itemBand = itemTroop.Band.Find(t => t.BandKey == baseBandDefaut.BandKey);
                    if (itemBand != null)
                    {
                        var indexBand = itemTroop.Band.FindIndex(t => t.BandKey == baseBandDefaut.BandKey);
                        _troopData.Band.RemoveAt(indexBand);
                        var indexRemove = ItemBandChoiced.FindIndex(t => t.BandKey == baseBandDefaut.BandKey);
                        if (indexRemove > -1) ItemBandChoiced.RemoveAt(indexRemove);
                    }
                }
                else
                {
                    var itemBand = _troopData.Band.Find(t => t.BandKey == baseBandDefaut.BandKey);
                    if (itemBand != null)
                    {
                        var indexBand = _troopData.Band.FindIndex(t => t.BandKey == baseBandDefaut.BandKey);
                        _troopData.Band.RemoveAt(indexBand);
                        var indexRemove = ItemBandChoiced.FindIndex(t => t.BandKey == baseBandDefaut.BandKey);
                        if (indexRemove > -1) ItemBandChoiced.RemoveAt(indexRemove);
                    }
                }
            
            }
        }
     
    }

    public void OnClickConfirm()
    {
        _onComplete?.Invoke(_troopData);
        Close();
    }

    public void OnClickCloseNotification()
    {
        //_objNotification.SetActive(false);
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
