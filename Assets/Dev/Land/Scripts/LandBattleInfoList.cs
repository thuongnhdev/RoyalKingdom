using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LandBattleList", menuName = "Uniflow/World/LandBattleList")]
public class LandBattleInfoList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<LandBattleInfo> _battleInfos;
    private Dictionary<int, LandBattleInfo> _battleDict = new();
    private Dictionary<int, LandBattleInfo> BattleDict
    {
        get
        {
            if (_battleDict.Count != _battleInfos.Count)
            {
                _battleDict.Clear();
                for (int i = 0; i < _battleInfos.Count; i++)
                {
                    _battleDict.Add(_battleInfos[i].id, _battleInfos[i]);
                }
            }

            return _battleDict;
        }
    }

    public LandBattleInfo GetLandBattle(int landId)
    {
        if (!BattleDict.TryGetValue(landId, out var land))
        {
            Logger.Log($"landId [{landId}] is invalid or this land currently has no member", Color.green);
        }

        return land;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        _battleInfos.Clear();
        _battleDict.Clear();

        var sheet = parsedSheets[0];
        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("Key", out int landId);
            row.GetValue("LandName", out string landName);

            var land = new LandBattleInfo
            {
                id = landId,
                landName = landName
            };

            _battleInfos.Add(land);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
