using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefixAppearances", menuName = "Uniflow/Resource/PrefixAppearance")]
public class PrefixAppearanceRateList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<PrefixAppearance> _prefixAppearances = new();

    [System.Serializable]
    private class PrefixAppearance
    {
        public int lucky;
        public float prob;
    }

    public float GetPrefixProps(int lucky)
    {
        if (_prefixAppearances.Count < lucky)
        {
            return _prefixAppearances[^1].prob;
        }

        return _prefixAppearances[lucky - 1].prob;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        var sheet = parsedSheets[1];
        if (sheet == null || sheet.Count == 0)
        {
            return;
        }

        _prefixAppearances.Clear();
        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("LuckyStatKey", out int lucky);
            row.GetValue("AppearPrefixProb", out int prob);

            _prefixAppearances.Add(new() { lucky = lucky, prob = prob / 10000f });
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}