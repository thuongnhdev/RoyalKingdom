using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VassalList", menuName = "Uniflow/Character/VassalList")]
public class VassalSOList : ScriptableObject
{
    [SerializeField]
    private List<VassalInSOList> _vassals;
    private Dictionary<int, VassalInSOList> _vassalDict = new();
    private Dictionary<int, VassalInSOList> VassalDict
    {
        get
        {
            if (_vassals.Count != _vassalDict.Count)
            {
                _vassalDict.Clear();
                for (int i = 0; i < _vassals.Count; i++)
                {
                    var vassal = _vassals[i];
                    _vassalDict[vassal.id] = vassal;
                }
            }

            return _vassalDict;
        }
    }

    public void Init(List<VassalInSOList> data)
    {
        if (data == null || data.Count == 0)
        {
            return;
        }

        _vassals.Clear();

        for (int i = 0; i < data.Count; i++)
        {
            _vassals.Add(data[i]);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public VassalInSOList GetVassal(int vassalId)
    {
        VassalDict.TryGetValue(vassalId, out var vassal);
        if (vassal == null)
        {
            Debug.LogError($"Invalid vassalId [{vassalId}]");
        }

        return vassal;
    }

    public int[] GetVassalGreatestStatsId(int vassalId, int statNumber)
    {
        var vassal = GetVassal(vassalId);
        if (vassal == null)
        {
            return new int[0];
        }

        return vassal.GetGreatestStatsId(statNumber);
    }

    public string GetFullName(int vassalId)
    {
        var vassal = GetVassal(vassalId);
        if (vassal == null)
        {
            return "";
        }

        return vassal.firstName + " " + vassal.lastName;
    }

    public string GetDescription(int id)
    {
        var vassal = GetVassal(id);
        if (vassal == null)
        {
            return "";
        }

        return vassal.description;
    }
}

[System.Serializable]
public class VassalInSOList
{
    public int id;
    public string firstName;
    public string lastName;
    public int birth;
    public int gender;
    public string homeTown;
    public int grade;
    public int nature;
    public int religion;
    public int loyalty;
    public string description;
    public int[] stats;
    [Header("Inspec")]
    [SerializeField]
    private List<int> _sortedStats = new();

    public VassalInSOList(VassalTemplate template)
    {
        id = template.Key;
        firstName = template.FirstName;
        lastName = template.LastName;
        birth = template.Birth;
        gender = template.Gender;
        homeTown = template.HomeTown;
        grade = template.Grade;
        description = template.DescriptVassal;
    }

    public int[] GetGreatestStatsId(int statsNumber)
    {
        if (stats.Length < statsNumber)
        {
            statsNumber = stats.Length;
        }

        int[] result = new int[statsNumber];
        SortStats();

        for (int i = 0; i < statsNumber; i++)
        {
            result[i] = _sortedStats[i];
        }

        return result;
    }

    public void SortStats()
    {
        if (_sortedStats.Count != 0)
        {
            return;
        }

        List<int> copyStats = new(stats);

        int i = 0;
        for (int k = 0; k < copyStats.Count; k++)
        {
            int greatestValue = copyStats[i];

            if (greatestValue == -1)
            {
                i++;
                continue;
            }

            int greatestIndex = i;

            for (int j = i + 1; j < copyStats.Count; j++)
            {
                if (greatestValue < copyStats[j])
                {
                    greatestValue = copyStats[j];
                    greatestIndex = j;
                }
            }
            
            copyStats[greatestIndex] = -1;
            _sortedStats.Add(greatestIndex + 1);
        }
    }
}



