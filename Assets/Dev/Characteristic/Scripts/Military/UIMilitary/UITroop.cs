using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreData.UniFlow;
using System;
using System.Globalization;
using Assets.Dev.Tutorial.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using CoreData.UniFlow.Common;

public class UITroop : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private GameObject _prefabItemTroop;

    [SerializeField]
    private RectTransform _parentItemTroop;

    [SerializeField]
    private TextMeshProUGUI _tmpTroopBattleTotal;

    [SerializeField]
    private TextMeshProUGUI _tmpTroopTradeTotal;

    [SerializeField]
    private GameObject _prefabItemTroopNull;

    [SerializeField]
    private RectTransform _parentItemTroopNull;

    [SerializeField]
    private GameObject _fadeBtnConfirm;

    private Action _onComplete;
    [SerializeField]
    private List<ItemTroop> ItemBattle = new List<ItemTroop>();
    [SerializeField]
    private List<ItemTroop> ItemTrade = new List<ItemTroop>();

    public List<TroopData> TroopDatas;

    public enum TYPETROOP
    {
        Battle = 0,
        Trade = 1
    }
    public override void SetData(object[] data)
    {
        base.SetData(data);
        _onComplete = (Action)data[0];
    }

    public override void Open()
    {
        base.Open();
        _fadeBtnConfirm.SetActive(true);
         TroopDatas = new List<TroopData>();
        for(int i = 0;i < GameData.Instance.SavedPack.SaveData.TroopDatas.Count;i++)
        {
            var item = GameData.Instance.SavedPack.SaveData.TroopDatas[i];
            TroopDatas.Add(item);
        }

        int totalBatle = 0,totalTrade = 0;
        for (int i = 0; i < ItemBattle.Count; i++)
        {
            var troopData = MasterDataStoreGlobal.Instance.BaseTroopDatas;
            var troopSelected = TroopDatas.Find(t => t.IdTroop == troopData[i].IdTroop);
            if (troopSelected != null)
            {
                totalBatle++;
                ItemBattle[i].InitData(i, troopSelected, TroopDatas, (itemB) => { OnSelectTroopItem(itemB); },(troopDatas) => { OnSelectVassal(troopDatas); });
            }
            else
            {
                MasterDataStoreGlobal.Instance.BaseTroopDatas[i].Band.Clear();
                ItemBattle[i].InitData(i, troopData[i], TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
        }

        for (int i = 0; i < ItemTrade.Count; i++)
        {
            var troopData = MasterDataStoreGlobal.Instance.BaseTroopDatas;
            var index = ItemBattle.Count + i;
            var troopSelected = TroopDatas.Find(t => t.IdTroop == troopData[index].IdTroop);
            if (troopSelected != null)
            {
                totalTrade++;
                ItemTrade[i].InitData(i, troopSelected, TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
            else
            {
                MasterDataStoreGlobal.Instance.BaseTroopDatas[index].Band.Clear();
                ItemTrade[i].InitData(i, troopData[index], TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
        }

        _tmpTroopBattleTotal.text = string.Format("BATTLE TROOP ({0}/5)", totalBatle);
        _tmpTroopTradeTotal.text = string.Format("TRADE TROOP ({0}/3)", totalTrade);
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

    public void OnSelectTroopItem(TroopData troopData)
    {
        UIManagerTown.Instance.ShowUIBand(troopData, (troopData) =>
        {
            _fadeBtnConfirm.SetActive(false);
            RefeshData(troopData);
        });
    }

    private void RefeshData(TroopData data)
    {
        var itemTroopNew = TroopDatas.Find(t => t.IdTroop == data.IdTroop);
        if (itemTroopNew == null)
            TroopDatas.Add(data);
        else
            itemTroopNew = data;

        int totalBatle = 0, totalTrade = 0;
        for (int i = 0; i < ItemBattle.Count; i++)
        {
            var troopData = MasterDataStoreGlobal.Instance.BaseTroopDatas;
            var troopSelected = TroopDatas.Find(t => t.IdTroop == troopData[i].IdTroop);
            if (troopSelected != null)
            {
                totalBatle++;
                ItemBattle[i].InitData(i, troopSelected, TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
            else
            {
                MasterDataStoreGlobal.Instance.BaseTroopDatas[i].Band.Clear();
                ItemBattle[i].InitData(i, troopData[i], TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
        }

        for (int i = 0; i < ItemTrade.Count; i++)
        {
            var troopData = MasterDataStoreGlobal.Instance.BaseTroopDatas;
            var index = ItemBattle.Count + i;
            var troopSelected = TroopDatas.Find(t => t.IdTroop == troopData[index].IdTroop);
            if (troopSelected != null)
            {
                totalTrade++;
                ItemTrade[i].InitData(i, troopSelected, TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
            else
            {
                MasterDataStoreGlobal.Instance.BaseTroopDatas[index].Band.Clear();
                ItemTrade[i].InitData(i, troopData[index], TroopDatas, (itemB) => { OnSelectTroopItem(itemB); }, (troopDatas) => { OnSelectVassal(troopDatas); });
            }
        }
        _tmpTroopBattleTotal.text = string.Format("BATTLE TROOP ({0}/5)", totalBatle);
        _tmpTroopTradeTotal.text = string.Format("TRADE TROOP ({0}/3)", totalTrade);
    }

    public void OnSelectVassal(List<TroopData> troopDatas)
    {
        TroopDatas = troopDatas;
        for (int i = 0; i < troopDatas.Count; i++)
        {
            RefeshData(TroopDatas[i]);
        }
    }


    private int GetTotalPawns(TroopData _troopData)
    {
        int total = 0;
        for (int i = 0; i < _troopData.Band.Count; i++)
        {
            total += _troopData.Band[i].Pawns;
        }
        return total;
    }

    public void OnClickConfirm()
    {
        if (!TutorialWorldmap.Instance.GetTutorialData().TutorialTracker.IsNeedTutorial())
        {
            for (int i = 0; i < TroopDatas.Count; i++)
            {
                TroopDatas[i].Pawns = GetTotalPawns(TroopDatas[i]);
            }
            GameData.Instance.SavedPack.SaveData.TroopDatas = TroopDatas;
            GameData.Instance.saveGameData();

            _ = APIManager.Instance.RequestPlayerTroopInfo(TroopDatas);

        }

        Close();
    }
}
