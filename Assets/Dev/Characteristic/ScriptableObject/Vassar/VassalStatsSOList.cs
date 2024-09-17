using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VassalStats", menuName = "Uniflow/Character/VassalStats")]
public class VassalStatsSOList : ScriptableObject
{
    [SerializeField]
    private List<VassalStat> _vassalStats;
    private Dictionary<int, VassalStat> _vassalStatsDict = new();
    private Dictionary<int, VassalStat> VassalStatsDict
    {
        get
        {
            if (_vassalStats.Count != _vassalStatsDict.Count)
            {
                _vassalStatsDict.Clear();
                for (int i = 0; i < _vassalStats.Count; i++)
                {
                    var stat = _vassalStats[i];
                    _vassalStatsDict[stat.id] = stat;
                }
            }

            return _vassalStatsDict;
        }
    }

    public void Init(List<VassalStat> data)
    {
        if (data == null || data.Count == 0)
        {
            return;
        }

        for (int i = 0; i < data.Count; i++)
        {
            var statData = data[i];
            VassalStatsDict.TryGetValue(statData.id, out var stat);
            if (stat == null)
            {
                _vassalStats.Add(stat);
                _vassalStatsDict[stat.id] = stat;
                continue;
            }

            stat.name = statData.name;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public VassalStat GetVassalStat(int statId)
    {
        VassalStatsDict.TryGetValue(statId, out var stat);
        if (stat == null)
        {
            Debug.LogError($"Invalid statId[{statId}]");
        }

        return stat;
    }

    public int GetStatId(string statName)
    {
        for (int i = 0; i < _vassalStats.Count; i++)
        {
            var stat = _vassalStats[i];
            if (stat.name.ToLower() == statName.ToLower())
            {
                return stat.id;
            }
        }

        return 0;
    }

    public string GetStatName(int statId)
    {
        var stat = GetVassalStat(statId);
        if (stat == null)
        {
            return "";
        }

        return stat.name;
    }

    public Sprite GetStatIcon(int statId)
    {
        var stat = GetVassalStat(statId);
        if (stat == null)
        {
            return null;
        }

        return stat.icon;
    }
}

[System.Serializable]
public class VassalStat
{
    public string name;
    public int id;
    public Sprite icon;

    public VassalStat(DataVassaleStats data)
    {
        name = data.StatName;
        id = data.Key;
    }
}