using CoreData.UniFlow.Equip;
using CoreData.UniFlow.Equip.Epic;
using CoreData.UniFlow.Equip.Legendary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Common
{
    internal class Equipment
    {
        public enum EquipmentList
        {
            JudgmentRing = 1, ShaiteCall, SethsCall, KaruaksWarDrums, ClarkBattleBook, MoraWeb, MoraSpiderWeb,
            ScholarLuckyCoin, ScholarsLuckyCoin, HornOfAnger, HornsOfAnger, SharpDagger, SharpsDagger,
            QuietJudgment, QuietJudgmentSpecial, None
        }
        public static Dictionary<EquipmentList, EquipmentBase> atkeyValuePairs = new Dictionary<EquipmentList, EquipmentBase>
        {
            { EquipmentList.JudgmentRing, new Ring_of_Doom() },
            { EquipmentList.ShaiteCall, new Seths_Call() },
            { EquipmentList.SethsCall, new Seths_Call() },
            { EquipmentList.KaruaksWarDrums, new Karuaks_War_Drums() },
            { EquipmentList.ClarkBattleBook, new Karuaks_War_Drums() },
            { EquipmentList.MoraWeb, new Moras_Web() },
            { EquipmentList.MoraSpiderWeb, new Moras_Web() },
            { EquipmentList.ScholarLuckyCoin, new Scolas_Lucky_Coin() },
            { EquipmentList.ScholarsLuckyCoin, new Scolas_Lucky_Coin() },
            { EquipmentList.HornOfAnger, new Horn_of_Fury() },
            { EquipmentList.HornsOfAnger, new Horn_of_Fury() },
            { EquipmentList.SharpDagger, new Concealed_Dagger() },
            { EquipmentList.SharpsDagger, new Concealed_Dagger() },
            { EquipmentList.QuietJudgment, new Silent_Trial() },
            { EquipmentList.QuietJudgmentSpecial, new Silent_Trial() },
            { EquipmentList.None, new EquipmentBase() }
        };
        public static Dictionary<EquipmentList, EquipmentBase> dfkeyValuePairs = new Dictionary<EquipmentList, EquipmentBase>
        {
            { EquipmentList.JudgmentRing, new Ring_of_Doom() },
            { EquipmentList.ShaiteCall, new Seths_Call() },
            { EquipmentList.SethsCall, new Seths_Call() },
            { EquipmentList.KaruaksWarDrums, new Karuaks_War_Drums() },
            { EquipmentList.ClarkBattleBook, new Karuaks_War_Drums() },
            { EquipmentList.MoraWeb, new Moras_Web() },
            { EquipmentList.MoraSpiderWeb, new Moras_Web() },
            { EquipmentList.ScholarLuckyCoin, new Scolas_Lucky_Coin() },
            { EquipmentList.ScholarsLuckyCoin, new Scolas_Lucky_Coin() },
            { EquipmentList.HornOfAnger, new Horn_of_Fury() },
            { EquipmentList.HornsOfAnger, new Horn_of_Fury() },
            { EquipmentList.SharpDagger, new Concealed_Dagger() },
            { EquipmentList.SharpsDagger, new Concealed_Dagger() },
            { EquipmentList.QuietJudgment, new Silent_Trial() },
            { EquipmentList.QuietJudgmentSpecial, new Silent_Trial() },
            { EquipmentList.None, new EquipmentBase() }
        };
    }
}
