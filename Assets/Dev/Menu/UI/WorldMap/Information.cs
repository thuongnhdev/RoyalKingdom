using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoreData.UniFlow.Common;
using System;


public class Information : MonoBehaviour
{

    [Header("Info Vassal")]
    
    [SerializeField]
    private Image _vassalImage;
    
    [SerializeField]
    private TextMeshProUGUI _vassalName;
    
    [SerializeField]
    private TextMeshProUGUI _constuction;

    [Header("Cusades")]
    
    [SerializeField]
    private Image _avaVassalImage1;
    
    [SerializeField]
    private Image _avaVassalImage2;
    
    [SerializeField]
    private TextMeshProUGUI _infantryValue;
    
    [SerializeField]
    private TextMeshProUGUI _calvaryValue;
    
    [SerializeField]
    private TextMeshProUGUI _archerValue;
    
    [SerializeField]
    private TextMeshProUGUI _tmpMood;
    
    [SerializeField]
    private TextMeshProUGUI _moodValue;
    
    [SerializeField]
    private Image _iconMood;    
    [SerializeField]
    private TextMeshProUGUI _buff1;


    private TroopData _troopData;
    private BuffData _buffData;


    public void InitData()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _infantryValue.text = GetTotalMilitaryType(MilitaryType.Infantry).ToString();
        _calvaryValue.text = GetTotalMilitaryType(MilitaryType.Cavalry).ToString();
        _archerValue.text = GetTotalMilitaryType(MilitaryType.Archer).ToString();
        _moodValue.text = GetMood().ToString();

    }

    private List<TroopDataMax> TroopDataMaxs = new List<TroopDataMax>();
    private int GetTotalMilitaryType(MilitaryType type)
    {
        int total = 0;
        var listItem = TroopDataMaxs.FindAll(t => t.IdMilitary == (int)type);
        for (int i = 0; i < listItem.Count; i++)
        {
            total += listItem[i].Number;
        }
        return total;
    }

    private int GetMood()
    {
        int mood = 0;
        if (_troopData.Band.Count == 0) return mood;

        for (int i = 0; i < _troopData.Band.Count; i++)
        {
            mood += _troopData.Band[i].MoraleCount;
        }
        return (mood / _troopData.Band.Count);
    }

}
