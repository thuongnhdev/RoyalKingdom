using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreData.UniFlow.Calculate;
using static CoreData.UniFlow.Commander.CommanderBase;
using CoreData.UniFlow.Common;

namespace CoreData.UniFlow.Skill
{
    internal class Xiang_Yu : SkillBase
    {
        public override void Init(CommanderBase bs, bool isFirst, int cnt)
        {
            base.Init(bs, isFirst);
            if (isFirst == true)
                bs.maxRage -= 150;
            else
                bs.maxRage -= 50;
        }

        bool togle = true;
        public override void Active(CommanderBase at, CommanderBase df)
        {
      
        }

        public void ActiveBonusStart(CommanderBase at, CommanderBase df)
        {
            if (df.activeDefence_dbf < 30)
            {
                df.activeDefence_dbf = 30;
            }
        }

        public void ActiveBonusEnd(CommanderBase at, CommanderBase df)
        {
            if (df.activeDefence_dbf == 30)
            {
                df.activeDefence_dbf = 0;
            }
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // +40 cavalry
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == BattleState.Conquering)
            {
                actionAmount2 = 5;
            }
            at.tempDamageIncrease += actionAmount2;
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == BattleState.Conquering)
            {
                // Increases damage by 5% during siege, 10% chance to deal 400 multiplier. Activates once every 3 seconds
                Random random = new Random();
                if (random.Next(0, 10) == 0 && actionCount2 <= 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.Write("- {0}[heaven and hell]", at.site);
                    CalcDamage.CalcActiveSkillDamage(at, df, 400);
                    actionCount2 = 3;
                }
                actionCount2--;
            }   
        }

        bool togle2 = true;
        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            at.tempDamageIncrease += actionAmount3;
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            if (actionCount3 == 0)
                actionAmount3 = 0;

            // Reduces rage by 50, increases cavalry damage by 5% when casting skills, repeats 6 times for 10 seconds
            if (togle != togle2)
            {
                togle2 = togle;
                
                if (at.armyType == ArmyType.Cavalry)
                {
                    actionAmount3 += 5;
                    actionAmount3 = Math.Min(30, actionAmount3);
                }
                else if (at.armyType == ArmyType.Mixed)
                {
                    actionAmount3 += (5 / 3);
                    actionAmount3 = Math.Min(10, actionAmount3);
                }
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Excellence Increase] Increases cavalry damage by {1}%", at.site, actionAmount3);

                actionCount3 = 10;
            }

            actionCount3--;
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // Skill damage increased by 10%. Increases skill damage by 10% for 3 seconds when Rage is acquired for 2 turns or more. Activates once every 5 seconds
        }
    }
}
