using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KingdomList", menuName = "Uniflow/World/KingdomList")]
public class KingdomList : ScriptableObject
{
    [SerializeField]
    private List<KingdomPoco> _kingdoms;
    public List<KingdomPoco> Kingdoms => _kingdoms;
    private Dictionary<long, KingdomPoco> _kingdomDict = new();
    private Dictionary<long, KingdomPoco> KingdomDict
    {
        get
        {
            if (_kingdomDict.Count != _kingdoms.Count)
            {
                _kingdomDict.Clear();
                for (int i = 0; i < _kingdoms.Count; i++)
                {
                    _kingdomDict[_kingdoms[i].kingdomId] = _kingdoms[i];
                }
            }

            return _kingdomDict;
        }
    }

    public void Init(responeInfoKingdom serverData)
    {
        _kingdoms.Clear();
        _kingdomDict.Clear();

        int kingdomCount = serverData.KingdomWorldMapLength;
        for (int i = 0; i < kingdomCount; i++)
        {
            var kingdomData = serverData.KingdomWorldMap(i).Value;

            List<int> kingdomMembers = new();
            int landCount = kingdomData.MemberLength;
            for (int j = 0; j < landCount; j++)
            {
                kingdomMembers.Add(kingdomData.Member(j));
            }
            KingdomPoco kingdom = new()
            {
                kingdomId = kingdomData.IdKingdom,
                name = kingdomData.Name,
                masterLand = kingdomData.IdLandOwner,
                members = kingdomMembers
            };

            _kingdoms.Add(kingdom);
            _kingdomDict.Add(kingdom.kingdomId, kingdom);
        }
    }

    public KingdomPoco GetKingdom(long kingdomId)
    {
        KingdomDict.TryGetValue(kingdomId, out var kingdom);
        if (kingdom == null)
        {
            Logger.Log($"Kingdom with Id {kingdomId} does not exist", Color.green);
        }

        return kingdom;
    }

    public string GetKingdomName(long kingdomId)
    {
        var kingdom = GetKingdom(kingdomId);
        if (kingdom == null)
        {
            return "No Kingdom";
        }

        return kingdom.name;
    }

    public void Local_TransferLand(long fromKingdom, long toKingdom, int land)
    {
        var winKingdom = GetKingdom(toKingdom);
        var loseKingdom = GetKingdom(fromKingdom);
        winKingdom.members.Add(land);
        loseKingdom.members.Remove(land);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
