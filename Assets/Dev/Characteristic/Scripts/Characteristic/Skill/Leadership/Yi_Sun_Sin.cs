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
    internal class Yi_Sun_Sin : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
        }

        double extraDamage;
        public override void Active(CommanderBase at, CommanderBase df)
        {
     
        }
        public void ActiveBonusStart(CommanderBase at, CommanderBase df)
        {
            if (df.activeSpeedIncrease_dbf < 30)
            {
                df.activeSpeedIncrease_dbf = 30;
            }
        }

        public void ActiveBonusEnd(CommanderBase at, CommanderBase df)
        {
            if (df.activeSpeedIncrease_dbf == 30)
            {
                df.activeSpeedIncrease_dbf = 0;
            }
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            at.tempDefence += (actionAmount1 * 4);
            at.tempDamageIncrease += (actionAmount1 * 3);
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // 10% chance to increase defense by 20 during normal attacks while stationed in alliance territory or bases. Increases all damage by 15% for 3 seconds
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                if (actionCount1 == 0)
                    actionAmount1 = 0;

                Random random = new Random();
                if (df.normalAttackDamage > 0 && random.Next(0, 10) == 0 && actionCount1 <= 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0} [Battle of Myeongnyang] Increases troop defense by 20% and all damage by 15%. lasts 3 seconds", at.site);
                    actionAmount1 = 5;
                    actionCount1 = 3;
                }
                actionCount1--;
            }
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            at.tempDamageIncrease += actionAmount2;
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases the defense of the commanded unit by 30%. If the bag is less than 50%, it is 20%
            if (at.troop * 2 <= at.maxTroop)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0} [Turtle Ship Revenue] Increases all damage by 20%", at.site);
                actionAmount2 = 20;
            }
        }

        double actionAmount3_2 = 0;
        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount3 = 20;
                at.tempAttack += actionAmount3;
            }
            at.tempCounterDamageIncrease += actionAmount3_2;
            // You need to add a shield.
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Increases attack by 20 when stationed at a base. When attacked, 10% chance to increase shield by 500 and counterattack damage by 30%, lasting for 3 seconds
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                if (actionCount3 == 0)
                    actionAmount3_2 = 0;

                Random random = new Random();
                if (at.normalAttackDamage > 0 && random.Next(0, 10) == 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}Counterattack damage increased by 30%. lasts 3 seconds", at.site);
                    if (UsingLog.usingLog == true)
                        Console.Write("- {0} Shield {1} factor activated", at.site, 500);
                    CalcDamage.CalcShieldEffect(at, 500, 3);
                    actionAmount3_2 = 30;
                    actionCount3 = 3;
                }
                actionCount3--;
            }   
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Mixed)
            {
                actionAmountNew = 20;
                at.tempAttack += actionAmountNew;
                at.tempDefence += actionAmountNew;
            }
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // Increases attack by 20 and room by 20 when there are 2 or more types
        }
    }
}
