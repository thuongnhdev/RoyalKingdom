using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TownListUiContent : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownInLandList _towns;

    [Header("Configs")]
    [SerializeField]
    private TownInfoScrollController _scroller;
    [SerializeField]
    private FilterTableAdjuster _filterAdjuster;

    private string _compareField;
    private int _ascendent = 1;

    public void SetUpContent()
    {
        _filterAdjuster.SetFilter(0);
        Sort(1);
    }

    public void SetSortOption(bool ascendent)
    {
        _ascendent = ascendent ? 1 : -1;
    }

    public void Sort(int column)
    {
        List<TownInLandInfo> towns = _towns.Towns;
        if (column == 1)
        {
            _compareField = "townName";
        }
        else
        {
            _compareField = _filterAdjuster.GetPocoFieldName(column - 2);
        }

        towns.Sort(CompareTown);
        _scroller.SetTowns(towns);
    }

    private int CompareTown(TownInLandInfo town1, TownInLandInfo town2)
    {
        return CollectionUtils.ObjectCompare(town1, town2, _compareField) * _ascendent;
    }
}
