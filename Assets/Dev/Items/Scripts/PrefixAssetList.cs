using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefixAssetList", menuName = "Uniflow/Resource/PrefixAssetList")]
public class PrefixAssetList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<PrefixAsset> _prefixes;
    private Dictionary<int, PrefixAsset> _prefixDict = new();
    private Dictionary<int, PrefixAsset> PrefixDict
    {
        get
        {
            if (_prefixDict.Count != _prefixes.Count)
            {
                _prefixDict.Clear();
                for (int i = 0; i < _prefixes.Count; i++)
                {
                    _prefixDict[_prefixes[i].id] = _prefixes[i];
                }
            }

            return _prefixDict;
        }
    }

    public PrefixAsset GetPrefixAsset(int prefixId)
    {
        PrefixDict.TryGetValue(prefixId, out var prefix);
        if (prefix == null)
        {
            Logger.LogWarning($"Prefix [{prefixId}]'s assets are not set");
        }

        return prefix;
    }

    public Sprite GetPrefixIcon(int prefixId)
    {
        var prefix = GetPrefixAsset(prefixId);
        if (prefix == null)
        {
            return null;
        }

        return prefix.icon;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        var sheet = parsedSheets[0];
        if (sheet.Count == 0)
        {
            return;
        }

        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("PrefixKey", out int id);
            row.GetValue("Name", out string name);
            PrefixDict.TryGetValue(id, out var prefix);
            if (prefix == null)
            {
                prefix = new() { id = id, name = name };
                _prefixes.Add(prefix);
                _prefixDict[prefix.id] = prefix;
                continue;
            }

            prefix.name = name;
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

[System.Serializable]
public class PrefixAsset
{
    public string name;
    public int id;
    public Sprite icon;
}
