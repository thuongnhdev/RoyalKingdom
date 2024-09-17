using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LandDynamicInfoList", menuName = "Uniflow/World/LandDynamicInfoList")]
public class LandDynamicInfoList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private LandStaticInfoList _staticLandInfos;
    [SerializeField]
    private List<LandDynamicInfo> _lands;
    private Dictionary<int, LandDynamicInfo> _landDict = new();
    private Dictionary<int, LandDynamicInfo> LandDict
    {
        get
        {
            if (_lands.Count != _landDict.Count)
            {
                _landDict.Clear();
                for (int i = 0; i < _lands.Count; i++)
                {
                    _landDict.Add(_lands[i].id, _lands[i]);
                }
            }

            return _landDict;
        }
    }

    public void Init(responeInfoLand serverData)
    {
        _lands.Clear();
        _landDict.Clear();

        int landCount = serverData.LandWorldMapLength;
        for (int i = 0; i < landCount; i++)
        {
            var landData = serverData.LandWorldMap(i).Value;
            var staticLandInfo = _staticLandInfos.GetLand(landData.IdLand);
            if (staticLandInfo == null)
            {
                Logger.LogError($"land with id {landData.IdLand} was not defined in masterData");
                continue;
            }
            List<long> landMembers = new();
            int townCount = landData.TownmemberLength;
            for (int j = 0; j < townCount; j++)
            {
                landMembers.Add(landData.Townmember(j));
            }
            LandDynamicInfo land = new()
            {
                id = landData.IdLand,
                landName = staticLandInfo.landName,
                // TODO RK lookup kingdom info
                owner = landData.IdPlayerOwner,
                members = landMembers,
                hp = landData.Hp,
                idParent = landData.IdLandParent
                
            };

            _lands.Add(land);
            _landDict.Add(land.id, land);
        }
    }

    public LandDynamicInfo GetLand(int landId)
    {
        LandDict.TryGetValue(landId, out var land);
        if (land == null)
        {
            Logger.Log($"landId [{landId}] is invalid or this land currently has no member. Create default land!", Color.green);
            var landStatic = _staticLandInfos.GetLand(landId);
            land = new()
            {
                id = landId,
                landName = landStatic.landName,
            };

            _lands.Add(land);
            _landDict.Add(landId, land);
        }

        return land;
    }

    public long GetKingdom(int landId)
    {
        var land = GetLand(landId);
        if (land == null) {
            return -1;
        }
        return land.kingdom;
    }

    public int GetMemberCount(int landId)
    {
        var land = GetLand(landId);
        if (land == null)
        {
            return 0;
        }

        return land.members.Count;
    }

    public void AddMember(int landId, long memberId)
    {
        var land = GetLand(landId);
        if (land == null)
        {
            return;
        }

        List<long> members = land.members;
        if (members.Contains(memberId))
        {
            return;
        }

        members.Add(memberId);
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        _lands.Clear();

        var sheet = parsedSheets[0];
        for (int i = 0; i < sheet.Count; i++)
        {
            var row = sheet[i];
            row.GetValue("Key", out int landId);
            row.GetValue("LandName", out string landName);

            var land = new LandDynamicInfo
            {
                id = landId,
                landName = landName,
                members = new()
            };

            _lands.Add(land);
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
