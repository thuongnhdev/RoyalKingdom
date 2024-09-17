using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldMapStrategyKit;

public class NeighborLandsPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _profile;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private LandDynamicInfoList _landsDynamicInfos;
    [SerializeField]
    private LandBattleInfoList _landBattleInfos;

    [Header("Configs")]
    [SerializeField]
    private FilterTableAdjuster _filterAdjuster;
    [SerializeField]
    private LandsInfoScrollController _scroller;
    [SerializeField]
    private TMP_Text _option1Text;
    [SerializeField]
    private TMP_Text _option2Text;
    [SerializeField]
    private Toggle _option1SortToggle;
    [SerializeField]
    private Toggle _option2SortToggle;

    [Header("Scene Configs")]
    [SerializeField]
    private WMSK _map;

    private List<int> _neighborIds = new();
    private string _sortField;
    private int _ascendent = 1;

    public void SetUp()
    {
        //int landGeoId = _landsStaticInfos.GetLandGeoId(_profile.landId);
        if (_neighborIds.Count == 0)
        {
            //InitNeighbors(landGeoId);
        }

        _filterAdjuster.SetDropdownValue(0);
        ApplyFilter(0);
        Sort(1);
    }

    public void ApplyFilter(int option)
    {
        _filterAdjuster.SetFilter(option);
        _ascendent = 1;

        if (1 < option)
        {
            _option1SortToggle.isOn = true;
            _option2SortToggle.isOn = true;
        }
    }

    public void SetSortOption(bool ascendent)
    {
        _ascendent = ascendent ? 1 : -1;
    }

    public void Sort(int column)
    {
        SetSortFieldName(column);
        _neighborIds.Sort(CompareLandByLandId);
        _scroller.SetData(_neighborIds);
    }

    private void SetSortFieldName(int column)
    {
        switch (column)
        {

            case 2:
                {
                    _sortField = "kingdom";
                    break;
                }
            case 3: 
                {
                    _sortField = _filterAdjuster.GetPocoFieldName(0);
                    break;
                }
            case 4:
                {
                    _sortField = _filterAdjuster.GetPocoFieldName(1);
                    break;
                }
            default:
                {
                    _sortField = "landName";
                    break;
                }
        }
    }

    private void InitNeighbors(int landGeoId)
    {
        int landIndex = _map.GetProvinceIndex(landGeoId);
        Debug.Log(landIndex);
        var neighbors = _map.ProvinceNeighbours(landIndex);
        Debug.Log(neighbors.Count);
        neighbors.ForEach(land => 
        {
            //int landId = _landsStaticInfos.GetLandIdByGeoId(land.uniqueId);
            //if (landId == 0)
            //{
             //   return;
            //}

            //_neighborIds.Add(landId); 
        });
    }

    private int CompareLandByLandId(int landId1, int landId2)
    {
        var staticInfo1 = _landsStaticInfos.GetLand(landId1);
        var staticInfo2 = _landsStaticInfos.GetLand(landId2);
        var dynamicInfo1 = _landsDynamicInfos.GetLand(landId1);
        var dynamicInfo2 = _landsDynamicInfos.GetLand(landId2);
        var battle1 = _landBattleInfos.GetLandBattle(landId1);
        var battle2 = _landBattleInfos.GetLandBattle(landId2);

        string compareField = _sortField;
        bool collectionType = compareField.Contains("List");
        if (collectionType)
        {
            compareField = compareField.Remove(compareField.Length - 5, 4); // Remove 'List'
        }

        FieldInfo field = staticInfo1.GetType().GetField(compareField);
        if (field != null)
        {
            return CollectionUtils.ObjectCompare(staticInfo1, staticInfo2, compareField, collectionType) * _ascendent;
        }

        field = dynamicInfo1.GetType().GetField(compareField);
        if (field != null)
        {
            return CollectionUtils.ObjectCompare(dynamicInfo1, dynamicInfo2, compareField, collectionType) * _ascendent;
        }

        field = battle1.GetType().GetField(compareField);
        if (field == null)
        {
            return 0;
        }
        return CollectionUtils.ObjectCompare(battle1, battle2, compareField, collectionType) * _ascendent;
    }

    private void OnEnable()
    {
        _map = WMSK.instance;
    }
}
