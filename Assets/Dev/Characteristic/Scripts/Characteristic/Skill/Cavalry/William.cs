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
    internal class William : SkillBase
    {
        bool actionBool = false;
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
            df.tempSkillDamageIncreaseCancel = actionBool;

            actionCount0--;

            if (actionCount0 == 0)
            {
                actionBool = false;
            }
        }

        bool togle = true;
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // 1500 counting skills
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[bloodline secret]", at.site);
             CalcDamage.CalcActiveSkillDamage(at, df, 1500);
            
            at.isSkillUsed = true;
            togle = !togle;

            if (UsingLog.usingLog == true)
                Console.WriteLine("- {0} [Secret of Bloodline] Increases target unit skill damage, disables buff, decreases march speed by 30%. lasts 3 seconds", at.site);
            //AddBeforeSkillBonus(at, 3, ActiveBonusStart, ActiveBonusEnd);
            actionBool = true;
            actionCount0 = 3;
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
            at.tempDamageIncrease += actionAmount1;
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases all damage by 10% outside of Federation territory by 20% while riding by 15%
            if (at.battleState == CommanderBase.BattleState.Field || at.battleState == CommanderBase.BattleState.Conquering)
                actionAmount1 = 10;
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            at.tempAttack += actionAmount2;
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry by 30% on the field, 10% chance to deal 1000 multiplier damage with normal attacks.
            if (at.battleState == CommanderBase.BattleState.Field || at.battleState == CommanderBase.BattleState.Conquering)
            {
                actionAmount2 = 30;
                Random random = new Random();
                if (random.Next(0, 10) == 0)
                {
                    if (UsingLog.usingLog == true)
                        Console.Write("- {0} [Norman Conquest]", at.site);
                    CalcDamage.CalcActiveSkillDamage(at, df, 1000);
                }
            }
        }

        bool togle2 = true;
        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
            at.tempDefence += actionAmount3;

        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            if (actionCount3 == 0)
                actionAmount3 = 0;

            // When hit with an active skill, the unit's defense increases by 20% and lasts for 3 seconds.
            if (togle != togle2)
            {
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0} [Disaster in the North] Increases the unit's defense by 20%. lasts 3 seconds", at.site);
                togle2 = togle;
                actionAmount3 = 20;
                actionCount3 = 3;
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
