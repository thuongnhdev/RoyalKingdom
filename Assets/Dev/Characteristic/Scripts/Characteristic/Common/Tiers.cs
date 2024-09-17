using CoreData.UniFlow.Tier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Common
{
    internal class Tiers
    {
        public enum TierList {
            Infantry = 1, HorseSoldier, Archer, Artillery, Byeongjong, Civilians, Builders, Farmer, Blacksmith, Merchant, Pastor,
            KoreanArcher, RomanInfantry, SpanishHorseman, ArabianHorseman, BritishArcher, OttomanArchers, JapaneseInfantry, GermanCavalry, FrenchInfantry, ChineseArcher, ByzantineCavalry, VikingInfantry
        };

        public static Dictionary<TierList, TierBase> atkeyValuePairs = new Dictionary<TierList, TierBase> {
            { TierList.Civilians, new Base_Civilians() },
            { TierList.Builders, new Base_Builders() },
            { TierList.Farmer, new Base_Farmer() },
            { TierList.Blacksmith, new Base_Blacksmith() }, 
            { TierList.Merchant, new Base_Merchant() },
            { TierList.Pastor, new Base_Pastor() },
            { TierList.Infantry, new Base_Infantry() },
            { TierList.HorseSoldier, new Base_Cavalry() },
            { TierList.Archer, new Base_Archer() },
            { TierList.Artillery, new Base_Artillery() },
            { TierList.Byeongjong, new Base_Mixed() },
            { TierList.KoreanArcher, new Korea() },
            { TierList.RomanInfantry, new Rome() },
            { TierList.SpanishHorseman, new Spain() },
            { TierList.ArabianHorseman, new Arabia() },
            { TierList.BritishArcher, new Britain() },
            { TierList.OttomanArchers, new Ottoman() },
            { TierList.JapaneseInfantry, new Japan() },
            { TierList.GermanCavalry, new Germany() },
            { TierList.FrenchInfantry, new France() },
            { TierList.ChineseArcher, new China() },
            { TierList.ByzantineCavalry, new Byzantium() },
            { TierList.VikingInfantry, new Viking() }
        };
        public static Dictionary<TierList, TierBase> dfkeyValuePairs = new Dictionary<TierList, TierBase> {
            { TierList.Civilians, new Base_Civilians() },
            { TierList.Builders, new Base_Builders() },
            { TierList.Farmer, new Base_Farmer() },
            { TierList.Blacksmith, new Base_Blacksmith() },
            { TierList.Merchant, new Base_Merchant() },
            { TierList.Pastor, new Base_Pastor() },
            { TierList.Infantry, new Base_Infantry() },
            { TierList.HorseSoldier, new Base_Cavalry() },
            { TierList.Archer, new Base_Archer() },
            { TierList.Artillery, new Base_Artillery() },
            { TierList.Byeongjong, new Base_Mixed() },
            { TierList.KoreanArcher, new Korea() },
            { TierList.RomanInfantry, new Rome() },
            { TierList.SpanishHorseman, new Spain() },
            { TierList.ArabianHorseman, new Arabia() },
            { TierList.BritishArcher, new Britain() },
            { TierList.OttomanArchers, new Ottoman() },
            { TierList.JapaneseInfantry, new Japan() },
            { TierList.GermanCavalry, new Germany() },
            { TierList.FrenchInfantry, new France() },
            { TierList.ChineseArcher, new China() },
            { TierList.ByzantineCavalry, new Byzantium() },
            { TierList.VikingInfantry, new Viking() }
        };
    }
}
