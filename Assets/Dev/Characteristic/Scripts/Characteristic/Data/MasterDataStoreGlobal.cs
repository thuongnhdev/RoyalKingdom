using CoreData.UniFlow.Common;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataStoreGlobal
{
    private static MasterDataStoreGlobal _instance;

    public static MasterDataStoreGlobal Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MasterDataStoreGlobal();
            }
            return _instance;
        }
    }

    public List<BaseLand> BaseLands;

    public List<BaseCities> BaseCitiesList;

    public List<PlayerInWorldMap> PlayerInWorldMaps;

    public List<BaseMilitaryBranch> BaseMilitaryBranchs;

    public List<BaseMilitaryFomation> BaseMilitaryFomations;

    public List<BaseMilitaryInfo> BaseMilitaryInfos;

    public List<BaseMilitaryProficiency> BaseMilitaryProficiencys;

    public List<BaseDataSymbolBg> BaseDataSymbolBgs;

    public List<BaseDataSymbolColor> BaseDataSymbolColors;

    public List<BaseDataSymbolDisplay> BaseDataSymbolDisplays;

    public List<BaseDataSymbolIconMain> BaseDataSymbolIconMains;

    public List<BaseDataSymbolIconSub> BaseDataSymbolIconSubs;

    public List<BaseDataSymbolLayout> BaseDataSymbolLayouts;

    public List<BaseBandDefaut> BaseBandDefauts;
    public Dictionary<int,BaseBandDefaut> BaseBandDefautsDict;

    public List<TroopData> BaseTroopDatas;

    public List<BasePlayerTroopInfo> BasePlayerTroopInfos;

    public void InitTroopData()
    {
        List<TroopData> converteds = new();
        for (int i = 0; i < BasePlayerTroopInfos.Count; i++)
        {
            VassalSlot vassal = new VassalSlot(0, 0, 0, 0, 0, 0, 0, 0);
            List<BaseBandDefaut> band = new List<BaseBandDefaut>();
            BuffData buff = new BuffData(0, 0, 0, 0, 0);
            TroopData troopData = new TroopData(BasePlayerTroopInfos[i].IdType, BasePlayerTroopInfos[i].IdTroop, vassal, band, 0, buff);

            converteds.Add(troopData);
        }
        BaseTroopDatas = converteds;
    }

    public static Dictionary<K,V> ConvertToDict<K,V>(List<V> list , System.Func<V,K> keyOf )
    {
        Dictionary<K, V> dict = new Dictionary<K, V>();
        foreach (var i in list) {
            dict.Add(keyOf(i), i);
        }
        return dict;
    }

}
