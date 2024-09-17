using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoreData.UniFlow.Common;
using System;

public class ItemTroopNull : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpIndex;

    private int _index;
    private TroopData _troopData;
    private List<TroopData> _troopDatas;
    private Action<TroopData> _onComplete;
    private Action<List<TroopData>> _onSelectComplete;
    public void InitData(int index, TroopData troopData,List<TroopData> troopDatas, Action<TroopData> onComplete, Action<List<TroopData>> onSelectComplete)
    {
        _index = index;
        _troopData = troopData;
        _troopDatas = troopDatas;
        _onComplete = onComplete;
        _onSelectComplete = onSelectComplete;

        _tmpIndex.text = index.ToString();
    }

    public void OnClickOpenSelectVassal(int index)
    {
        UIManagerTown.Instance.ShowUIVassarChoiceMilitary(_troopData.IdTroop, _troopDatas, index, (troopDatas) => { OnSelectVassal(troopDatas); });
    }

    public void OnSelectVassal(List<TroopData> troopDatas)
    {
        _troopDatas = troopDatas;
        _onSelectComplete?.Invoke(_troopDatas);
    }

    public void OnClickOpenBandItem()
    {
        _onComplete?.Invoke(_troopData);
    }
}
