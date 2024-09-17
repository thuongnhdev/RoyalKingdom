using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FilterTable", menuName = "Uniflow/Common/FilterTable")]
public class FilterTable : ScriptableObject
{
    [SerializeField]
    private Filter[] _filters;
    public Filter[] Filters => _filters;

    public Filter GetFilter(int filterValue)
    {
        return _filters[filterValue % _filters.Length];
    }

    [System.Serializable]
    public class Filter
    {
        public string filterName;
        public string[] displayFieldsName;
        public string[] pocoFieldsName;
    }
}


