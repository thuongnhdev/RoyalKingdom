using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefixList", menuName = "Uniflow/Resource/PrefixList")]
public class PrefixList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<Prefix> _prefixes = new();
    private Dictionary<int, Prefix> _prefixDict = new();
    private Dictionary<int, Prefix> PrefixDict
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

    public void Init(ApiGetLowData data)
    {
        int prefixCount = data.ItemPrefixTemplateLength;
        if (prefixCount == 0)
        {
            return;
        }

        _prefixes.Clear();
        _prefixDict.Clear();
        for (int i = 0; i < prefixCount; i++)
        {
            var prefixData = data.ItemPrefixTemplate(i).Value;
            Enum.TryParse(prefixData.BuffID1, out Buff buff);
            _prefixes.Add(new()
            {
                id = prefixData.PreFixKey,
                name = prefixData.Name,
                relatedStat = prefixData.RelatedStatKey,
                productionRate = prefixData.RelatedStatKey,
                buff = buff,
                buffType = prefixData.BuffType1,
                buffValue = prefixData.BuffValue1

            });
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public Prefix GetPrefix(int prefixId)
    {
        if (!PrefixDict.TryGetValue(prefixId, out var prefix))
        {
            Logger.LogError($"Invalid prefixId [{prefixId}]");
        }

        return prefix;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        var sheet = parsedSheets[0];
        if (sheet == null || sheet.Count == 0)
        {
            return;
        }

        _prefixes.Clear();
        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("PrefixKey", out int prefixId);
            row.GetValue("Name", out string prefixName);
            row.GetValue("RelatedStatKey", out int relatedStat);
            row.GetValue("ProductionRate", out float productionRate);
            row.GetValue("BuffID_1", out string buffString);
            Enum.TryParse(buffString, out Buff buff);
            row.GetValue("BuffType_1", out int buffType);
            row.GetValue("BuffValue_1", out float buffValue);

            _prefixes.Add(new()
            {
                id = prefixId,
                name = prefixName,
                relatedStat= relatedStat,
                productionRate = productionRate,
                buff = buff,
                buffType = buffType,
                buffValue = buffValue
            });
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
