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
    internal class Scipio : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
            df.tempHealth -= actionAmount0;
 
            actionCount0--;
            if (actionCount0 == 0)
                actionAmount0 = 0;
        }

        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Ratio 2000 Reduces health by 30% for 3 seconds
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[invincible power]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 2000);
            at.isSkillUsed = true;

            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0}[Invincible Power] Reduces target unit's HP by {1)%, lasts for {2} seconds", at.site, actionAmount0, actionCount0);
            actionAmount0 = 30;
            actionCount0 = 3;
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
           
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Infantry attack +40 Conquest in all directions
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (df.battleState != CommanderBase.BattleState.Garrison)
            {
                actionAmount2 = 20;
                at.tempHealth += actionAmount2;
            }
        }
        double actionAmount2_2 = 0;
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
          
        }
        public void Passive2Bonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Strategy of oppression]", at.site);
            CalcDamage.CalcAdditionalSkillDamage(df, actionAmount2_2);
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            Random random = new Random();
            // When taking skill damage on the field, there is a 50% chance to reduce 30% at the same time, while simultaneously triggering a 3 second shield 500 factor once every 8 seconds
            if (at.skillDamage > 0 && random.Next(0,2) == 0 && at.battleState != CommanderBase.BattleState.Garrison && actionCount3 <= 0)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Armor Formation] Reduces this damage by 30%", at.site);
                at.skillDamage *= 0.7;

                actionAmount3 = 500;
                if (UsingLog.usingLog == true)
                    Console.Write("- {0}[Armor Formation]", at.site);
                CalcDamage.CalcShieldEffect(at, actionAmount3, 3);
                actionCount3 = 8;
            }
            actionCount3--;
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // Increases skill damage by 10% When the target is silent, anger recovery increases by 30% Burning Rage
            if (df.silenceTurn > 0)
            {
                actionAmountNew = 30;
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Burning Rage] Increases Rage recovery speed by {1}%", at.site, actionAmountNew);
                at.ragePlus *= 1.3;
            }
        }
    }
}
