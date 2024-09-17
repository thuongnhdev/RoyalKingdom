using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static CoreData.UniFlow.Common.Commanders;
using static CoreData.UniFlow.Common.Tiers;

namespace CoreData.UniFlow.Common
{
    internal static class ViewBuff
    {
        public static void ViewAdditionalBuff(CommanderBase at, CommanderBase df)
        {
            Debug.Log("▶attack side buff");
            Debug.LogFormat("Commander: major {0}, minor {1}", at.commanderClassList[0].Item1.GetType().Name, at.commanderClassList[1].Item1.GetType().Name);
            Debug.LogFormat("Attack power: {0}\nDefense power: {1}\nLife: {2}\nDamage increase: {3}\nDamage decrease: {4}\nSkill damage increase: {5}\nSkill damage decrease: {6}\n" +
                 "Normal damage increase: {7}\nNormal damage reduction: {8}\n Counterattack damage increase: {9}\n Counterattack damage reduction: {10}\nHealing effect increase: {11}\n", at.additionalAttack, at.additionalDefence, at.additionalHealth,
                at.additionalDamageIncrease, at.additionalDamageDecrease, at.additionalSkillDamageIncrease, at.additionalSkillDamageDecrease, at.additionalNormalDamageIncrease,
                at.additionalNormalDamageDecrease, at.additionalCounterDamageIncrease, at.additionalCounterDamageDecrease, at.additionalHealingEffect);
            Debug.Log("▶defensive side buff");
            Debug.LogFormat("Commander: major {0}, minor {1}", df.commanderClassList[0].Item1.GetType().Name, df.commanderClassList[1].Item1.GetType().Name);
            Debug.LogFormat("Attack power: {0}\nDefense power: {1}\nLife: {2}\nDamage increase: {3}\nDamage decrease: {4}\nSkill damage increase: {5}\nSkill damage decrease: {6}\n" +
                 "Normal damage increase: {7}\nNormal damage reduction: {8}\n Counterattack damage increase: {9}\n Counterattack damage reduction: {10}\nHealing effect increase : {11}\n", df.additionalAttack, df.additionalDefence, df.additionalHealth,
                df.additionalDamageIncrease, df.additionalDamageDecrease, df.additionalSkillDamageIncrease, df.additionalSkillDamageDecrease, df.additionalNormalDamageIncrease,
                df.additionalNormalDamageDecrease, df.additionalCounterDamageIncrease, df.additionalCounterDamageDecrease, df.additionalHealingEffect);
        }
       
    }
}
