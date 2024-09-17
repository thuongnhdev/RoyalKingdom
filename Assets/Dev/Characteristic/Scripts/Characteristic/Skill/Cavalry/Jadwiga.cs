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
    internal class Jadwiga : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
        }

        public override void Active(CommanderBase at, CommanderBase df)
        {
            // 1500 counts. Morphosis increases by 20% and lasts for 3 seconds
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[power of godliness]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 1500);
            at.isSkillUsed = true;

            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0}[Power of Piety] Increases all damage by 20%. lasts 3 seconds", at.site);
            //AddBeforeSkillBonus(at, 3, ActiveBonusStart, ActiveBonusEnd);
        }

        public void ActiveBonusStart(CommanderBase at, CommanderBase df)
        {
            if (at.activeDamageIncrease_bf < 20)
            {
                at.activeDamageIncrease_bf = 20;
            }
        }

        public void ActiveBonusEnd(CommanderBase at, CommanderBase df)
        {
            if (at.activeDamageIncrease_bf == 20)
            {
                at.activeDamageIncrease_bf = 0;
            }
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount1 = 20;
                at.tempDefence += actionAmount1;
                at.tempHealth += actionAmount1;
            }
            else
            {
                actionAmount1 = 5;
                at.tempDefence += (actionAmount1 * 2);
                at.tempSpeedIncrease += (actionAmount1 * 3);
            }
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases room by 10 and row by 15 in the field. When garrisoned, mount 20 and life increase by 20.
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount2 = 5;
                if (df.armyType == CommanderBase.ArmyType.Archer)
                {
                    if (at.armyType == CommanderBase.ArmyType.Cavalry)
                        at.tempDamageIncrease += actionAmount2;
                    else if (at.armyType == CommanderBase.ArmyType.Mixed)
                        at.tempDamageIncrease += (actionAmount2 / 3);
                }
                if (df.armyType == CommanderBase.ArmyType.Infantry)
                {
                    if (at.armyType == CommanderBase.ArmyType.Cavalry)
                        at.tempDamageDecrease -= actionAmount2;
                    else if (at.armyType == CommanderBase.ArmyType.Mixed)
                        at.tempDamageDecrease -= (actionAmount2 / 3);
                }
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases speed by 10%, and increases damage dealt by horsemen to archers by 5% when stationed at a base. Increases damage taken from infantry by 5%
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
       
        }
        public void Passive3Bonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0} [Blade of Evasion] Recover 50 Rage", at.site);
            at.ragePlus += actionAmount3;
        }

        double actionAmountNew2 = 0;
        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
            if (df.battleState == CommanderBase.BattleState.Conquering)
            {
                actionAmountNew = 10;
                at.tempDamageIncrease += actionAmountNew;
            }
            at.tempDamageDecrease += actionAmountNew2;
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            if (actionCountNew == 0)
                actionAmountNew2 = 0;
            // Increases damage dealt to assembly units by 10%. When casting a skill, 10% damage lasts for 4 seconds
            if (at.isSkillUsed == true)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0} [The Holy King of Poland] Reduces damage taken by 10%. lasts 4 seconds", at.site);
                actionAmountNew2 = 10;
                actionCountNew = 4;
            }
            actionCountNew--;
        }
    }
}
