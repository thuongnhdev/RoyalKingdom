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
    internal class Amanitore : SkillBase
    {
        public override void Active(CommanderBase at, CommanderBase df)
        {
        
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
            actionAmount1 = 5;
            if (df.armyType == CommanderBase.ArmyType.Infantry)
            {
                if (at.armyType == CommanderBase.ArmyType.Archer)
                    at.tempDamageIncrease += actionAmount1;
                else if (at.armyType == CommanderBase.ArmyType.Mixed)
                    at.tempDamageIncrease += (actionAmount1 / 3);
            }
            if (df.armyType == CommanderBase.ArmyType.Cavalry)
            {
                if (at.armyType == CommanderBase.ArmyType.Archer)
                    at.tempDamageDecrease -= actionAmount1;
                else if (at.armyType == CommanderBase.ArmyType.Mixed)
                    at.tempDamageDecrease -= (actionAmount1 / 3);
            }
                
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases archers by 40. Increases damage from infantry by 5% and increases damage from cavalry by 5%
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount2 = 20;
                at.tempDefence += actionAmount2;
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // When stationed, the defense of the archers increases by 20. Each attack has a 10% chance to neutralize the increase in the opponent's attack power. Triggers once every 10 seconds
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                Random random = new Random();
                if (df.normalAttackDamage > 0 && random.Next(0, 10) == 0 && actionCount2 <= 0)
                {
                    // How to reset halal stack..
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0} [Tarseti's Patience] Removes the effect of increasing opponent's continuous attack power", at.site);
                    actionCount2 = 10;
                }
                actionCount2--;
            }
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // When you take skill damage, you take 800 damage. When it is a garrison command, 50% chance for 500 damage, 20% chance for 400 damage, triggers once every 10 seconds
            if (at.skillDamage > 0 && actionCount3 <= 0)
            {
                if (UsingLog.usingLog == true)
                    Console.Write("- {0}[queen of fury]", at.site);
                CalcDamage.CalcActiveSkillDamage(at, df, 800);

                if (at.battleState == CommanderBase.BattleState.Garrison)
                {
                    Random random = new Random();
                    if (random.Next(0, 2) == 0)
                    {
                        if (UsingLog.usingLog == true)
                            Console.Write("- {0}[queen of fury]", at.site);
                        CalcDamage.CalcActiveSkillDamage(at, df, 500);
                    }
                    if (random.Next(0, 5) == 0)
                    {
                        if (UsingLog.usingLog == true)
                            Console.Write("- {0}[queen of fury]", at.site);
                        CalcDamage.CalcActiveSkillDamage(at, df, 400);
                    }
                }
                actionCount3 = 10;
            }
            actionCount3--;
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
            at.silenceTurn = 0;
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
         
        }
        public void NewBonus(CommanderBase at, CommanderBase df)
        {
            df.rageMinus += actionAmountNew;
        }
    }
}
