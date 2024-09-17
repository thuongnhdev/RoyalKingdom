
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
    internal class Artemisia : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
        }

        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Damage factor 1800, damage factor 300 for me
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Battle of Salamis]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 1800);
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Battle of Salamis]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, at, 300);

            at.isSkillUsed = true;
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {       
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases archer release by 20
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount2 = 10;
                at.tempNormalDamageDecrease += actionAmount2;
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Reduces normal attack damage by 10% when stationed, 10% chance for normal attack for 1 sec. (No normal attack)
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                Random random = new Random();
                if (df.normalAttackDamage > 0 && random.Next(0, 10) == 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Xerxes' Ally] The embargo effect on the target unit lasts for 1 second", at.site);
                    df.forbiddenTurn = 2;
                }
            }
        }

        bool togle = true;
        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            at.tempDamageIncrease += actionAmount3;
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            if (at.isSkillUsed == true)
                togle = true;

            // When Rage increases to 80%, there is a 50% chance to silence itself for 3 seconds, and all damage increases by 50% for 5 seconds.
            if (actionCount3 == 0)
                actionAmount3 = 0;

            Random random = new Random();
            if (at.rage >= at.maxRage * 0.8)
            {
                if (togle == true && random.Next(0, 2) == 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Queen Caria] Silences the target unit for 3 seconds, increases the damage of the commanded unit by 50%, lasts for 5 seconds", at.site);
                    if (at.silenceTurn <= 1)
                        at.silenceTurn = 4;
                    actionAmount3 = 50;
                    actionCount3 = 5;
                }
                togle = false;
            }
            actionCount3--;
        }

        double actionAmountNew_2 = 0;
        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
            df.tempSkillDamageIncrease += actionAmountNew;
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
        
        }
        public void NewBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Law of Survival]", at.site);
            CalcDamage.CalcAdditionalSkillDamage(df, actionAmountNew_2);
        }
    }
}
