using CoreData.UniFlow;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserVassalList", menuName = "Uniflow/User/UserVassalList")]
public class UserVassalSOList : ScriptableObject
{
    [Header("Reference - Read")]
    [SerializeField]
    private VassalExpAndLevel _expAndLevel;

    [SerializeField]
    private List<UserVassal> _vassals;
    private Dictionary<int, UserVassal> _vassalDict = new();
    private Dictionary<int, UserVassal> VassalDict
    {
        get
        {
            if (_vassals.Count != _vassalDict.Count)
            {
                _vassalDict.Clear();
                for (int i = 0; i < _vassals.Count; i++)
                {
                    _vassalDict[_vassals[i].vassalId] = _vassals[i];
                }
            }

            return _vassalDict;
        }
    }

    public void Init(ApiLoginResult data)
    {
        int vassalCount = data.LoginVassalInfoLength;
        if (vassalCount == 0)
        {
            return;
        }

        _vassals.Clear();
        for (int i = 0; i < vassalCount; i++)
        {
            var vassalData = data.LoginVassalInfo(i).Value;
            UserVassal vassal = new();
            vassal.vassalId = vassalData.IdVassalTemplate;
            vassal.level = vassalData.Level;

            int vassalStatCount = vassalData.VassalStatsInfoPlayerLength;
            vassal.stats = new int[vassalStatCount];
            for (int j = 0; j < vassalStatCount; j++)
            {
                var statData = vassalData.VassalStatsInfoPlayer(j).Value;
                vassal.stats[statData.KeyStat - 1] = statData.ValueStat;
            }

            _vassals.Add(vassal);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public int[] GetAllVassalIds()
    {
        int[] ids = new int[_vassals.Count];
        for (int i = 0; i < _vassals.Count; i++)
        {
            ids[i] = _vassals[i].vassalId;
        }

        return ids;
    }

    public UserVassal GetVassal(int vassalId)
    {
        VassalDict.TryGetValue(vassalId, out var vassal);
        if (vassal == null)
        {
            Debug.Log($"<color=green>User does not own vassal with Id [{vassalId}]</color>");
        }

        return vassal;
    }

    public float GetLucky(int vassalId)
    {
        var vassal = GetVassal(vassalId);
        if (vassal == null)
        {
            return 0f;
        }

        return vassal.stats[15] / 10000f;
    }

    public int GetRandomPrefixBasedOnStats(int vassalId)
    {
        var vassal = GetVassal(vassalId);
        if (vassal == null)
        {
            return 0;
        }

        return MathUtils.RandomChooseFrom(vassal.stats);
    }

    public int[] GetGreatestStats(int id, int statNumber)
    {
        var vassal = GetVassal(id);
        if (vassal == null)
        {
            return new int[0];
        }

        return vassal.GetGreatestStatsId(statNumber);
    }

    public int GetVassalExpOfStat(int vassalId, int statId)
    {
        var savePack = GameData.Instance.SavedPack;
        if (savePack == null)
        {
            return 0;
        }

        var vassals = savePack.SaveData.VassalInfos;
        VassalDataInGame targetVassal = null;
        for (int i = 0; i < vassals.Count; i++)
        {
            var vassal = vassals[i];
            if (vassal.Data.idVassalTemplate == vassalId)
            {
                targetVassal = vassal;
                break;
            }
        }

        if (targetVassal == null)
        {
            return 0;
        }

        var stats = targetVassal.Data.BaseDataVassalStats;
        BaseDataVassalStatValue targetStat = null;
        for (int i = 0; i < stats.Count; i++)
        {
            if (stats[i].Key == statId)
            {
                targetStat = stats[i];
                break;
            }
        }

        if (targetStat == null)
        {
            return 0;
        }

        return (int)targetStat.Value;
    }

    /// <summary>
    /// (level, exp)
    /// </summary>
    public (int, int) GetVassalStatLevelAndExp(int vassalId, int statId)
    {
        int statExp = GetVassalExpOfStat(vassalId, statId);
        int level = _expAndLevel.FromExpToLevel(statExp);

        return (level, statExp);
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}

[System.Serializable]
public class UserVassal
{
    public int vassalId;
    public int level;
    [SerializeField]
    private int[] _stats;
    public int[] stats
    {
        get
        {
            return _stats;
        }
        set
        {
            _stats = value;
            _needRefreshSortedStats = true;
        }
    }

    [Header("Inspec")]
    [SerializeField]
    private bool _needRefreshSortedStats = true;

    private List<int> _sortedStats = new();

    public int[] GetGreatestStatsId(int statsNumber) // TODO RK Listen stats changed event to refresh data
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

    public void UpdateStat(int statId, int value)
    {
        int statIndex = statId - 1;
        int currentValue = _stats[statIndex];
        if (currentValue == value)
        {
            return;
        }

        _stats[statIndex] = value;
        _needRefreshSortedStats = true;
    }

    private void SortStats()
    {
        if (!_needRefreshSortedStats)
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

        _needRefreshSortedStats = false;
    }
}
