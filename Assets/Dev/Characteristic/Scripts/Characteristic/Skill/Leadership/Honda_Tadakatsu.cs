using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreData.UniFlow.Calculate;
using CoreData.UniFlow.Common;

namespace CoreData.UniFlow.Skill
{
    internal class Honda_Tadakatsu : SkillBase
    {
        bool isFirst = true;
        public override void Init(CommanderBase bs, bool isFirst, int cnt)
        {
            base.Init(bs, isFirst);
            this.isFirst = isFirst;
            actionCountNew = 57;
        }
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
        }

        public override void Active(CommanderBase at, CommanderBase df)
        {
            // 2500 for the main commander, 1250 for the deputy commander
            if (UsingLog.usingLog == true)
                Console.Write("- {0} [Tombokiri]", at.site);
            if (isFirst == true)
                CalcDamage.CalcActiveSkillDamage(at, df, 2500);
            else
                CalcDamage.CalcActiveSkillDamage(at, df, 1250);
            at.isSkillUsed = true;
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            at.tempAttack += actionAmount1;
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases attack power by 10%. Line speed increased by 20%. If the attack target is a unit, attack increases by 30%
            if (df.battleState == CommanderBase.BattleState.Field || df.battleState == CommanderBase.BattleState.Conquering)
                actionAmount1 = 30;
        }

        double actionAmount2_2 = 0;
        double actionAmount2_3 = 0;
        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            actionAmount2 = 5;
            at.tempDamageDecrease += actionAmount2;
            df.tempSpeedIncrease -= actionAmount2_2;
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
           
        }
        public void Passive2Bonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Japanese-style room]", at.site);
            CalcDamage.CalcAdditionalSkillDamage(df, actionAmount2_3);
        }


        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            at.tempSkillDamageIncrease += actionAmount3;

        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            int factor;
            // Increase the number of troops by 10%. Increases skill damage by 5% for every 8% decrease in troops (for every 5% decrease in case of 2 types). up to 60 percent
            if (at.armyType == CommanderBase.ArmyType.Mixed)
                factor = 5;
            else
                factor = 8;

            int troopDecreaseRate = (int)((1 - (at.troop / at.maxTroop)) * 100);
            actionAmount3 = (troopDecreaseRate / factor) * 5;
            actionAmount3 = Math.Min(actionAmount3, 60);
            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0}Increases skill damage by {1}%", at.site, actionAmount3);
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // When receiving normal attack damage in the field, the damage is reduced by 30%, up to 57 times
            if (at.battleState != CommanderBase.BattleState.Garrison && at.normalAttackDamage > 0 && actionCountNew > 0)
            {
                actionCountNew--;
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Invincible Reverse] Damage reduced by {1}. {2} turns left", at.site, Math.Round(at.normalAttackDamage * 0.3), actionCountNew);
                at.normalAttackDamage *= 0.7;
            }
        }
    }
}
