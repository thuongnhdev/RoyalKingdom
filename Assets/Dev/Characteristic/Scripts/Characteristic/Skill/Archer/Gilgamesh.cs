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
    internal class Gilgamesh : SkillBase
    {
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Damage factor 1500, target unit's HP reduced by 30%, lasting for 3 seconds
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[lion's roar]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 1500);
            
            at.isSkillUsed = true;

            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0} [Lion's Roar] Reduces target unit's HP by 30% and lasts for 3 seconds", at.site);
            AddBeforeSkillBonus(at, 3, ActiveBonusStart, ActiveBonusEnd);
        }

        public void ActiveBonusStart(CommanderBase at)
        {
           
        }

        public void ActiveBonusEnd(CommanderBase at)
        {
            
        }


        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Archer)
            {
                at.tempDamageIncrease += actionAmount1;
            }
            if (at.armyType == CommanderBase.ArmyType.Mixed)
            {
                at.tempDamageIncrease += (actionAmount1 / 3);
            }
                
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases the number of archers by 30. If the enemy unit is less than 50%, all archers damage is increased by 20%.
            if (df.troop * 2 <= df.maxTroop)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0} [Uruk Archer] Increases all archer damage by 20%", at.site);
                actionAmount1 = 20;
            }
        }

        double actionAmount2_2 = 0;
        new int actionCount2 = 6;
        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (df.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount2 = 20;
                if (at.armyType == CommanderBase.ArmyType.Archer)
                {
                    at.tempAttack += actionAmount2;
                    at.tempAttack += actionAmount2_2;
                }
                else if (at.armyType == CommanderBase.ArmyType.Mixed)
                {
                    at.tempAttack += (actionAmount2 / 3);
                    at.tempAttack += (actionAmount2_2 / 3);
                }   
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases archer attack by 20 when attacking cities and bases. Adds 5 Atk buff every 6 turns, stacks 6 times for 10 seconds
            if (df.battleState == CommanderBase.BattleState.Garrison)
            {
                if (actionCount2 == 0)
                {
                    actionAmount2_2 += 5;
                    actionAmount2_2 = Math.Min(30, actionAmount2_2);
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[King's Law] Increases archer attack power by {1}%", at.site, actionAmount2_2);
                    actionCount2 = 6;
                }
                actionCount2--;
            }
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            df.tempSkillDamageDecrease -= actionAmount3;
            if (actionCount3 == 4)
            {
                df.thirstForBloodTurn = 4;
            }
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Reduces normal damage taken by 15%. When attacking, there is a 30% chance to add a bloodthirst effect lasting 4 seconds.
            // When receiving treatment for the target unit, fixed damage (factor 700) and skill damage received increases by 15%. Triggers once every 5 seconds
            if (actionCount3 == 1)
            {
                actionAmount3 = 0;
            }   

            Random random = new Random();
            if (df.normalAttackDamage > 0 && random.Next(0, 10) < 3 && actionCount3 <= 0)
            {
                actionAmount3 = 15;
                df.thirstForBloodFactor = 700;
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Friendship Accompaniment] Gives the target unit the effect of thirst for blood. Increases the skill damage received by the target unit by {1}%. lasts 4 seconds", at.site, actionAmount3);
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
