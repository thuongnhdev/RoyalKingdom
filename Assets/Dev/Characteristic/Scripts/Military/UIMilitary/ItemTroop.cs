using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoreData.UniFlow.Common;
using System;

public class ItemTroop : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tmpIndex;

    [Header("Vassal")]
    [SerializeField]
    private Image _imgAvatarVassal_1;

    [SerializeField]
    private Image _imgAvatarVassal_2;

    [SerializeField]
    private Image _imgAvatarVassal_3;

    [SerializeField]
    private TextMeshProUGUI _tmpTroopName;

    [SerializeField]
    private VassalSpriteSOList _vassalSpriteSOList;

    [Header("Military")]
    [SerializeField]
    private Image _iconBranch_1;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberMaxBranch_1;

    [SerializeField]
    private Image _iconBranch_2;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberMaxBranch_2;

    [SerializeField]
    private Image _iconBranch_3;

    [SerializeField]
    private TextMeshProUGUI _tmpNumberMaxBranch_3;

    [SerializeField]
    private Image _iconMood;

    [SerializeField]
    private TextMeshProUGUI _tmpMood;

    [SerializeField]
    private GameObject _objItem_1;

    [SerializeField]
    private GameObject _objItemEmpty_1;

    [SerializeField]
    private GameObject _objItem_2;

    [SerializeField]
    private GameObject _objItemEmpty_2;

    private int _index;
    private int _typeTroop;
    private TroopData _troopData;
    private List<TroopData> _troopDatas;
    private Action<TroopData> _onComplete;
    private Action<List<TroopData>> _onSelectComplete;
    public void InitData(int index,TroopData troopData , List<TroopData> troopDatas, Action<TroopData> onComplete, Action<List<TroopData>> onSelectComplete)
    {
        _index = index;
        _troopData = troopData;
        _troopDatas = troopDatas;
        _onComplete = onComplete;
        _onSelectComplete = onSelectComplete;

        TroopDataMaxs.Clear();
        for (int i = 0; i < _troopData.Band.Count; i++)
        {
            SetMaxMilitary(_troopData.Band[i]);
        }
     
        UpdateUI();
    }

    private void UpdateUI()
    {
        _tmpIndex.text = (_index + 1).ToString();
        _tmpMood.text = GetMood().ToString();
        if(_troopData.IdType == 0) _tmpTroopName.text = "Battle";
        if (_troopData.IdType == 1) _tmpTroopName.text = "Trade";

        if (_troopData.Vassal.IdVassal_1 > 0)
        {
            _imgAvatarVassal_1.sprite = _vassalSpriteSOList.GetHexaIcon(_troopData.Vassal.IdVassal_1);
        }
        if (_troopData.Vassal.IdVassal_2 > 0)
        {
            _imgAvatarVassal_2.sprite = _vassalSpriteSOList.GetHexaIcon(_troopData.Vassal.IdVassal_2);
        }
        if (_troopData.Vassal.IdVassal_3 > 0)
        {
            _imgAvatarVassal_3.sprite = _vassalSpriteSOList.GetHexaIcon(_troopData.Vassal.IdVassal_3);
        }

        if (_troopData.Band.Count > 0)
        {
            _objItem_1.SetActive(true);
            _objItem_2.SetActive(true);
            _objItemEmpty_1.SetActive(false);
            _objItemEmpty_2.SetActive(false);
            _tmpNumberMaxBranch_1.text = GetTotalMilitaryType(MilitaryType.Infantry).ToString();
            _tmpNumberMaxBranch_2.text = GetTotalMilitaryType(MilitaryType.Archer).ToString();
            _tmpNumberMaxBranch_3.text = GetTotalMilitaryType(MilitaryType.Cavalry).ToString();
        }
        else
        {
            _objItem_1.SetActive(false);
            _objItem_2.SetActive(false);
            _objItemEmpty_1.SetActive(true);
            _objItemEmpty_2.SetActive(true);
        }

    }

    private List<TroopDataMax> TroopDataMaxs = new List<TroopDataMax>();
    private int GetTotalMilitaryType(MilitaryType type)
    {
        int total = 0;
        var listItem = TroopDataMaxs.FindAll(t => t.IdMilitary == (int)type);
        for (int i = 0;i < listItem.Count;i++)
        {
            total += listItem[i].Number;
        }
        return total;
    }

    private void SetMaxMilitary(BaseBandDefaut band)
    {
        int idMilitaly = (int)MilitaryType.Infantry;
        int number = 0;
        if (band.Proficiency_1 > band.Proficiency_2 && band.Proficiency_1 > band.Proficiency_3)
        {
            number = band.Pawns;
            idMilitaly = (int)MilitaryType.Infantry;
        }
        if (band.Proficiency_2 > band.Proficiency_1 && band.Proficiency_2 > band.Proficiency_3)
        {
            number = band.Pawns;
            idMilitaly = (int)MilitaryType.Archer;
        }
        if (band.Proficiency_3 > band.Proficiency_1 && band.Proficiency_3 > band.Proficiency_2)
        {
            number = band.Pawns;
            idMilitaly = (int)MilitaryType.Cavalry;
        }
        TroopDataMax data = new TroopDataMax(idMilitaly, number);
        TroopDataMaxs.Add(data);
    }

    public int GetMood()
    {
        int mood = 0;
        if (_troopData.Band.Count == 0) return mood;
        for (int i = 0; i < _troopData.Band.Count; i++)
        {
            mood += _troopData.Band[i].MoraleCount;
        }
        return (mood / _troopData.Band.Count);
    }

    public void OnClickOpenSelectVassal(int index)
    {
        UIManagerTown.Instance.ShowUIVassarChoiceMilitary(_troopData.IdTroop, _troopDatas, index,(troopDatas) => { OnSelectVassal(troopDatas); });
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
