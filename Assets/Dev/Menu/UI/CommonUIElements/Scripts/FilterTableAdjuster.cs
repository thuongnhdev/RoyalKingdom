using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FilterTableAdjuster : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _filter;

    [Header("Reference - Read")]
    [SerializeField]
    private FilterTable _filterTable;

    [Header("Configs")]
    [SerializeField]
    private TMP_Dropdown _filterDropdown;
    [SerializeField]
    private TMP_Text[] _valuesNameText;

    public void SetDropdownValue(int value)
    {
        _filterDropdown.value = value;
    }

    public void SetFilter(int filterValue)
    {
        _filter.Value = filterValue;
        var filter = _filterTable.GetFilter(filterValue);
        for (int i = 0; i < _valuesNameText.Length; i++)
        {
            _valuesNameText[i].text = filter.displayFieldsName[i];
        }
    }

    public string GetPocoFieldName(int index)
    {
        var filter = _filterTable.GetFilter(_filter.Value);
        return filter.pocoFieldsName[index];
    }

    private void OnEnable()
    {
        _filterDropdown.options.Clear();
        var filters = _filterTable.Filters;
        for (int i = 0; i < filters.Length; i++)
        {
            _filterDropdown.options.Add(new() { text = filters[i].filterName });
        }
    }
}
