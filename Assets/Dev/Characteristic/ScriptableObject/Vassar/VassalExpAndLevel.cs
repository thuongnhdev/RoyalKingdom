using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VassalExpAndLevel", menuName = "Uniflow/Character/VassalExpAndLevel")]
public class VassalExpAndLevel : ScriptableObject
{
    [SerializeField]
    private List<int> _exps;

    public void Init(ApiGetLowData serverData)
    {
        _exps.Clear();

        int levelCount = serverData.DataVassalStatsLevelLength;
        int accumulatedExp = 0;
        for (int i = 0; i < levelCount; i++)
        {
             var levelData = serverData.DataVassalStatsLevel(i).Value;
            accumulatedExp += levelData.MaxExp;
            _exps.Add(accumulatedExp);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public int GetExpRequiredForLevel(int level)
    {
        if (level <= 1)
        {
            return 0;
        }

        int index = Mathf.Clamp(level - 2, 0, _exps.Count);
        return _exps[index];
    }

    public int FromExpToLevel(int exp)
    {
        for (int i = _exps.Count - 1; 0 <= i; i--)
        {
            if (_exps[i] <= exp)
            {
                return i + 2;
            }
        }

        return 1;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
