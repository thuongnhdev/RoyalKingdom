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
    internal class Chandragupta_Maurya : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
            at.tempDamageIncrease += actionAmount0;

            actionCount0--;
           
            if (actionCount0 == 0)
                actionAmount0 = 0;
        }

        bool togle = true;
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Increases all damage by 40 for 3 seconds after casting the skill. Blessing effect acquired once. 10 seconds duration 4 limit
            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0}[Wolho King] Blessing effect acquired once. All damage increased by 40%. lasts 3 seconds", at.site);
            actionAmount0 += 1;
            actionAmount0 = Math.Min(4, actionAmount0);
            actionCount0 = 10;

            //AddBeforeSkillBonus(at, 3, ActiveBonusStart, ActiveBonusEnd);

            togle = !togle;

            at.isSkillUsed = true;
        }
        public void ActiveBonusStart(CommanderBase at, CommanderBase df)
        {
            if (at.activeDamageIncrease_bf < 40)
            {
                at.activeDamageIncrease_bf = 40;
            }
        }

        public void ActiveBonusEnd(CommanderBase at, CommanderBase df)
        {
            if (at.activeDamageIncrease_bf == 40)
            {
                at.activeDamageIncrease_bf = 0;
            }
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Conquering)
            {
                actionAmount1 = 10;
                at.tempDamageDecrease += actionAmount1;
            }
        }
        bool togle2 = true;
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // 10% damage when attacking bases. When casting a skill, obtain 1 additional blessing effect
            if (at.battleState == CommanderBase.BattleState.Conquering)
            {
                if (togle != togle2)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0}[Imperial Elephant] Blessing effect 1 time", at.site);
                    togle2 = togle;
                    actionAmount0 += 1;
                    actionAmount0 = Math.Min(4, actionAmount0);
                    actionCount0 = 10;
                }
            }
        }

        double actionAmount2_2 = 0;
        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Cavalry)
            {
                actionAmount2 = 20;
                at.tempHealth += actionAmount2;
                df.tempHealth -= actionAmount2_2;
                df.tempDefence -= actionAmount2_2;
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // If it is an All Horseman, the HP is increased by 20%. 50% chance each time to reduce target live and room by 5%, stacks 3 times for 5 seconds
            if (at.armyType == CommanderBase.ArmyType.Cavalry)
            {
                if (actionCount2 == 0)
                    actionAmount2_2 = 0;

                Random random = new Random();
                if (random.Next(0, 2) == 0)
                {
                    actionAmount2_2 += 5;
                    actionAmount2_2 = Math.Min(15, actionAmount2_2);
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0} [Spiritual Theory] Decreases the target's HP and DEF by {1}%", at.site, actionAmount2_2);
                    actionCount2 = 5;
                }
                actionCount2--;
            }   
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            if (df.armyType == CommanderBase.ArmyType.Cavalry)
            {
                actionAmount3 = 5;
                at.tempDamageDecrease += actionAmount3;
            }
            at.tempSpeedIncrease += actionAmount3_2;
        }
        double actionAmount3_2 = 0;
        double skillDamage = 0;
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Reduces damage taken from cavalry by 5%. Increases movement speed by 25% for 3 seconds when casting a skill. 500* blessing effect damage to target when casting skill
            if (actionCount3 == 0)
                actionAmount3_2 = 0;
            if (at.isSkillUsed == true && actionCount3 <= 0)
            {
                actionAmount3_2 = 25;
                actionCount3 = 3;
                if (UsingLog.usingLog == true)
                    Console.Write("- {0}[asceticism]", at.site);
                CalcDamage.CalcActiveSkillDamage(at, df, 500 * actionAmount0);
                actionAmount0 = 0;
            }
            actionCount3--;
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // When attacking within alliance territory, there is a 50% chance to acquire a blessing effect once. Acquire 2 times when attacking from outside. Activates once every 5 seconds.
            Random random = new Random();
            if (df.normalAttackDamage > 0 && random.Next(0, 2) == 0 && actionCountNew <= 0)
            {
                if (at.battleState == CommanderBase.BattleState.Garrison)
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0} [Mauriya Dynasty] Blessing effect acquired once", at.site);
                    actionAmount0 += 1;
                }
                else
                {
                    if (UsingLog.usingLog == true)
                        Console.WriteLine("- {0} [Mauriyah Dynasty] Blessing effect 2 times", at.site);
                    actionAmount0 += 2;
                }   

                actionAmount0 = Math.Min(4, actionAmount0);
                actionCount0 = 10;
                actionCountNew = 5;
            }
            actionCountNew--;
        }
    }
}
