using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class TownInfoScrollCell : BaseCustomCellView
{
    [Header("Reference - Read")]
    [SerializeField]
    private FilterTable _filterTable;
    [SerializeField]
    private IntegerVariable _filter;

    [Header("Configs")]
    [SerializeField]
    private TMP_Text _townNameText;
    [SerializeField]
    private List<TMP_Text> _optionsText;

    private TownInLandInfo _town;
    private Type _townType;

    public override void SetUp(object town)
    {
        _town = (TownInLandInfo)town;
        _townType = town.GetType();
        _townNameText.text = _town.townName;
        ApplyFilter(_filter.Value);
    }

    private void ApplyFilter(int filterValue)
    {
        var filter = _filterTable.GetFilter(filterValue);
        for (int i = 0; i < _optionsText.Count; i++)
        {
            string fieldName = filter.pocoFieldsName[i];
            FieldInfo field = _townType.GetField(fieldName);
            if (field == null)
            {
                continue;
            }

            _optionsText[i].text = field.GetValue(_town).ToString();
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
