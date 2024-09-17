using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStats : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private TextMeshProUGUI _tmpPoint;

    [SerializeField]
    private GameObject _objIncrease;

    [SerializeField]
    private Image _imgIcon;

    public int Index;
    MasterDataStore masterDataStore;
    private BaseDataVassalStatValue baseDataVassalStatValue;
    public void InitData(int index, JobData jobData, BaseDataVassalStatValue statsValue, Sprite icon)
    {
        _objIncrease.SetActive(false);
        masterDataStore = MasterDataStore.Instance;
        Index = index;
        if (statsValue.Value < 0) statsValue.Value = Math.Abs(statsValue.Value);
        baseDataVassalStatValue = statsValue;
        int levelExp = 1;
        for (int j = 0; j < MasterDataStore.Instance.BaseDataVassalStatsExpIngames.Count; j++)
        {
            var item = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j];
            var itemNext = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j];
            if (j < MasterDataStore.Instance.BaseDataVassalStatsExpIngames.Count - 1)
                itemNext = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j + 1];
                
            if (baseDataVassalStatValue.Value >= item.TotalExpLevelUp && baseDataVassalStatValue.Value < itemNext.TotalExpLevelUp)
            {
                levelExp = item.Lv;
                break;
            }
        }

        _tmpPoint.text = levelExp.ToString();
        var nameStats = masterDataStore.BaseDataVassalStats.Find(t => t.Key == statsValue.Key).StatName;
        _tmpName.text = nameStats.ToString();
        if (icon != null) _imgIcon.sprite = icon;

        if(jobData != null)
        {
            var jobInfoBase = masterDataStore.BaseVassalJobInfos.Find(t => t.Key == jobData.Key);
            int add_StatKey_1 = jobInfoBase.Add_StatKey_1;
            int add_StatKey_2 = jobInfoBase.Add_StatKey_2;
            int add_StatKey_3 = jobInfoBase.Add_StatKey_3;
            int add_StatKey_4 = jobInfoBase.Add_StatKey_4;
            if (statsValue.Key == add_StatKey_1 || statsValue.Key == add_StatKey_2 || statsValue.Key == add_StatKey_3 || statsValue.Key == add_StatKey_4)
                _objIncrease.SetActive(true);
        }
    }

    public void UpdateUI(BaseDataVassalStatValue statsValue)
    {
        if (statsValue.Value < 0) statsValue.Value = Math.Abs(statsValue.Value);
        baseDataVassalStatValue = statsValue;
        int levelExp = 1;
        for (int j = 0; j < MasterDataStore.Instance.BaseDataVassalStatsExpIngames.Count; j++)
        {
            var item = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j];
            var itemNext = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j];
            if (j < MasterDataStore.Instance.BaseDataVassalStatsExpIngames.Count - 1)
                itemNext = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j + 1];

            if (baseDataVassalStatValue.Value >= item.TotalExpLevelUp && baseDataVassalStatValue.Value < itemNext.TotalExpLevelUp)
            {
                levelExp = item.Lv;
                break;
            }
        }
        _tmpPoint.text = levelExp.ToString();
        var stats = masterDataStore.BaseDataVassalStats.Find(t => t.Key == statsValue.Key);
        var nameStats = stats.StatName;
        _tmpName.text = nameStats.ToString();
    }
}
