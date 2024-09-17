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
    internal class Theodora : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
        }

        double extraDamage;
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Damage Factor 1700 Skill Cast Debuff Purification
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Byzantine Empress]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 1700);
            at.isSkillUsed = true;
            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0}[Byzantine Empress] Removed all debuffs", at.site);
            // Purification must be implemented.
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount1 = 10;
                at.tempDefence += actionAmount1;
                if (df.battleState == CommanderBase.BattleState.Conquering)
                {
                    actionAmount1 = 10;
                    at.tempDamageDecrease += actionAmount1;
                }
            }
            
            
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases defense by 10% while garrisoning Reduces damage received from gathering units by 10%
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            at.tempDamageIncrease += actionAmount2;
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases troop ATK by 10% Increases damage by 10% when troops are 50% or more
            if (at.troop * 2 >= at.maxTroop)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Co-ruler] Increases all damage by 10%", at.site);
                actionAmount2 = 10;
            }
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            at.tempDamageIncrease += actionAmount3;

        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // When attacked while stationed, there is a 10% chance to increase damage by 40% for 3 seconds
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                if (actionCount3 == 0)
                    actionAmount3 = 0;

                Random random = new Random();
                if (at.normalAttackDamage > 0 && random.Next(0, 10) == 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Imperial Power] Increases all damage by 40%. lasts 3 seconds", at.site);
                    actionAmount3 = 40;
                    actionCount3 = 3;
                }
                actionCount3--;
            }
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
        }
    }
}
