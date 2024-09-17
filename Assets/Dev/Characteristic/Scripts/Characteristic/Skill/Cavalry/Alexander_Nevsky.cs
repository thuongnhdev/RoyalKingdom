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
    internal class Alexander_Nevsky : SkillBase
    {
        
        double extraDamage;
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // 2300 counting skills
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Lake Peipus]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 2300);
            at.isSkillUsed = true;
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Cavalry)
                actionAmount1 = 20;
            at.tempHealth += actionAmount1;
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry by 20, cavalry increases by 20 outside of alliance territory.
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry by 20, all damage increases by 10 when the target is in a pincer attack. Reduces all damage taken by 5.
        }

        double actionAmount3_2 = 0;
        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Cavalry)
            {
                actionAmount3 = 25;
            }
            at.tempSkillDamageIncrease += actionAmount3;
            at.tempSkillDamageIncrease += actionAmount3_2;

        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Cavalry)
            {
                // If it is a horseman, skill damage is increased by 25. Increases skill damage by 35 after casting a skill. Lasts 4 seconds, triggers once every 5 seconds
                if (actionCount3 == 1)
                    actionAmount3_2 = 0;

                if (at.isSkillUsed && actionCount3 <= 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Russ Iron Horseman] Increases skill damage by 35% after casting the skill. lasts 4 seconds", at.site);
                    actionAmount3_2 = 35;
                    actionCount3 = 5;
                }
                actionCount3--;
            }   
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
            at.tempHealth += actionAmountNew;
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // Increases normal damage by 5%. When attacked, there is a 10% chance to increase the life of the horse by 30. Lasts 3 seconds, triggers once every 5 seconds
            if (actionCountNew == 2)
                actionAmountNew = 0;

            Random random = new Random();
            if (at.normalAttackDamage > 0 && random.Next(0, 10) == 0 && actionCountNew <= 0)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Winter Storm] Increases the HP of horsemen by 30%. lasts 3 seconds", at.site);
                if (at.armyType == CommanderBase.ArmyType.Cavalry)
                    actionAmountNew = 30;
                actionCountNew = 5;
            }
            actionCountNew--;
        }
    }
}
