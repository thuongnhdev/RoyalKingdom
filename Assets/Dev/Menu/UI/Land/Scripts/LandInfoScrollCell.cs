using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandInfoScrollCell : BaseCustomCellView
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _filter;
    [SerializeField]
    private LandStaticInfoList _staticInfos;
    [SerializeField]
    private LandDynamicInfoList _dynamicInfos;
    [SerializeField]
    private KingdomList _kingdoms;

    [Header("Configs")]
    [SerializeField]
    private Image _landFlag;
    [SerializeField]
    private Image _kingdomFlag;
    [SerializeField]
    private TMP_Text _landNameText;
    [SerializeField]
    private TMP_Text _kingdomNameText;
    [SerializeField]
    private TMP_Text _option1ValueText;
    [SerializeField]
    private TMP_Text _option2ValueText;

    private LandStaticInfo _landStatic;
    private LandDynamicInfo _landDynamic;
    private LandBattleInfo _landBattle;
    public override void SetUp(object data)
    {
        int landId = (int)data;
        _landStatic = _staticInfos.GetLand(landId);
        if (_landStatic == null)
        {
            Debug.Log(landId);
            return;
        }

        _landDynamic = _dynamicInfos.GetLand(landId);
        _landNameText.text = _landStatic.landName;
        _kingdomNameText.text = _kingdoms.GetKingdomName(_landDynamic.kingdom);

        ApplyFilter(_filter.Value);
    }

    private void ApplyFilter(int option)
    {
        switch (option)
        {
            case 1:
                {
                    _option1ValueText.text = _landDynamic.prosperity.ToString();
                    _option2ValueText.text = _landDynamic.reputation.ToString();
                    break;
                }
            case 2:
                {
                    _option1ValueText.text = _landBattle.kills.ToString();
                    _option2ValueText.text = _landBattle.totalTroops.ToString();
                    break;
                }
            case 3:
                {
                    _option1ValueText.text = _landStatic.culture.ToString();
                    _option2ValueText.text = _landStatic.religion.ToString();
                    break;
                }
            default:
                {
                    _option1ValueText.text = _landDynamic.members.Count.ToString();
                    _option2ValueText.text = _landDynamic.population.ToString();
                    break;
                }
        }
    }

    private void OnEnable()
    {
        _filter.OnValueChange += ApplyFilter;
    }

    private void OnDisable()
    {
        _filter.OnValueChange -= ApplyFilter;
    }
}
