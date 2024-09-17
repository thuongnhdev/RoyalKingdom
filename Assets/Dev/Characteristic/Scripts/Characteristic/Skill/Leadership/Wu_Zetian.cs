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
    internal class Wu_Zetian : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
        }

        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Damage factor of 1000, healing factor of 500
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Yongdongbonggyeong]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 1000);

            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Yongdongbonggyeong]", at.site);
            CalcDamage.CalcHealingEffect(at, df, 500);

            at.isSkillUsed = true;
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount1 = 10;
                at.tempDamageIncrease += actionAmount1;
            }
            if (df.battleState == CommanderBase.BattleState.Conquering)
            {
                actionAmount1 = 10;
                at.tempDamageIncrease += actionAmount1;
            }
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases all damage by 10% when stationed at a base. Increases damage to assembly units by 10%.
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
         
        }
        public void Passive2Bonus(CommanderBase at, CommanderBase df)
        {
            if (df.silenceTurn <= 1)
                df.silenceTurn = 3;
        }

        double actionAmount3_2 = 0;
        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount3 = 15;
                at.tempSkillDamageDecrease += actionAmount3;
            }
            at.tempDefence += actionAmount3_2;
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Reduces skill damage received by 15% when stationed at a base, 50% chance to increase defense by 20 for 3 seconds when receiving skill damage
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                if (actionCount3 == 0)
                    actionAmount3_2 = 0;

                Random random = new Random();
                if (at.skillDamage > 0 && random.Next(0, 2) == 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Uncharacteristic Monument] Increases troops defense by 20%. lasts 3 seconds", at.site);
                    actionAmount3_2 = 20;
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
            // Counterattack damage increased by 20%. When being attacked, there is a 10% chance of a damage factor of 500
            Random random = new Random();
            if (at.normalAttackDamage > 0 && random.Next(0, 10) == 0)
            {
                if (UsingLog.usingLog == true)
                    Console.Write("- {0}[Long live the queen]", at.site);
                CalcDamage.CalcActiveSkillDamage(at, df, 500);
            }
        }
    }
}
