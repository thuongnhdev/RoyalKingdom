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
    internal class Bertrand_du_Guesclin : SkillBase
    {
        
        double extraDamage;
        public override void Active(CommanderBase at, CommanderBase df)
        {
       
        }
        public void ActiveBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[ex-cat]", at.site);
            extraDamage = CalcDamage.CalcActiveSkillDamage(at, df, 700);

            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0}[Enhancement before] Reduces target's rage by 20", at.site);
            actionAmount0 = 20;
            df.rageMinus += actionAmount0;
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState != CommanderBase.BattleState.Garrison)
            {
                actionAmount1 = 5;
                at.tempSpeedIncrease += (actionAmount1 * 2);
                at.tempDamageDecrease += actionAmount1;
            }
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry attack power by 10 and vitality by 10. Outside the Alliance Territory, cavalry conduct 10, dungeon 5
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Conquering)
            {
                actionAmount2 = 10;
                at.tempDamageIncrease += actionAmount2;
                at.tempDamageIncrease += actionAmount2_2;
            }
        }
        double actionAmount2_2 = 0;
        new int actionCount2 = 10;
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Damage from base attack 10. Increases horse disease damage by 1% every 10 seconds, stacks 5 times for 15 seconds
            if (at.battleState == CommanderBase.BattleState.Conquering)
            {
                if (actionCount2 == 10)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Fabius Strategy] Increases Cavalry Damage by 1% every 10 seconds (Max 5%)", at.site);
                    if (at.armyType == CommanderBase.ArmyType.Cavalry)
                    {
                        actionAmount2_2 += 1;
                        actionAmount2_2 = Math.Min(5, actionAmount2_2);
                    }
                    else if (at.armyType == CommanderBase.ArmyType.Mixed)
                    {
                        actionAmount2_2 += (1/3);
                        actionAmount2_2 = Math.Min(5/3, actionAmount2_2);
                    }
                    actionCount2 = 0;
                }
                actionCount2++;
            }
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Mounted Defense 20, 10% chance of healing when attacked (250 factor) Lasts 3 seconds, triggers once every 5 seconds

            Random random = new Random();
            if (at.normalAttackDamage > 0 && random.Next(0, 10) == 0 && actionCount3 <= 0)
            {
                actionAmount3 = 250;
                if (UsingLog.usingLog == true)
                    Console.Write("- {0}[Iris]", at.site);
                CalcDamage.CalcHealingEffect(at, df, actionAmount3);
                //AddAfterSkillBonus(at, 0, 2, PassiveBonus);
                actionCount3 = 5;
            }
            actionCount3--;

        }
        public void PassiveBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Iris]", at.site);
            CalcDamage.CalcHealingEffect(at, df, actionAmount3);
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // Increases speed by 5%, skill damage multiplies by 400 to target unit when receiving skill damage. Triggers once every 5 seconds
            if (at.skillDamage > 0 && actionCountNew <= 0)
            {
                if (UsingLog.usingLog == true)
                    Console.Write("- {0}[Royal Commander]", at.site);
                CalcDamage.CalcActiveSkillDamage(at, df, 400);
                actionCountNew = 5;
            }
            actionCountNew--;
        }

    }
}
