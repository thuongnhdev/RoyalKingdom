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
    internal class Minamoto_no_Yoshitsune : SkillBase
    {
        
        double extraDamage;
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // 1400 count. 75% chance for 600 additional damage for 2 seconds
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[light eagle]", at.site);
            extraDamage = CalcDamage.CalcActiveSkillDamage(at, df, 1400);
            Random random = new Random();
            if (random.Next(0, 4) < 3)
            {
                extraDamage = CalcDamage.CalcActiveSkillDamage(at, df, 600, false);
                //AddAfterSkillBonus(at, 1, 2, ActiveBonus);
            }
            at.isSkillUsed = true;
        }
        public void ActiveBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[light eagle]", at.site);
            CalcDamage.CalcAdditionalSkillDamage(df, extraDamage);
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Horseman 20%, rowing 10%
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // 50% barbarian damage
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            df.tempDamageDecrease -= actionAmount3;

        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // With a normal attack, there is a 10% chance to increase the damage received by the target unit by 30% for 3 seconds. Triggers once every 5 seconds
            if (actionCount3 == 2)
                actionAmount3 = 0;

            Random random = new Random();
            if (df.normalAttackDamage > 0 && random.Next(0, 10) == 0 && actionCount3 <= 0)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0} [Master] Increases the damage received by the target unit by 30%. lasts 3 seconds", at.site);
                actionAmount3 = 30;
                actionCount3 = 5;
            }
            actionCount3--;

        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
        }
    }
}
