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
    internal class Aetius : SkillBase
    {
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // Ratio 2300 If the opponent's blood is less than 50%, 150 damage over 3 seconds
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[empire spear]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 2300);
            at.isSkillUsed = true;
            if (df.troop * 2 <= df.maxTroop)
            {
                actionAmount0 = CalcDamage.CalcActiveSkillDamage(at, df, 150, false);
                //AddAfterSkillBonus(at, 1, 3, ActiveBonus);
            }
            
        }
        public void ActiveBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[empire spear]", at.site);
            CalcDamage.CalcAdditionalSkillDamage(df, actionAmount0);
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                if (at.armyType == CommanderBase.ArmyType.Infantry)
                    actionAmount1 = 15;
                else if (at.armyType == CommanderBase.ArmyType.Mixed)
                    actionAmount1 = 5;
                else
                    actionAmount1 = 0;
                at.tempDefence += actionAmount1;
                at.tempHealth += actionAmount1;
            }
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Infantry Atk 15, Infantry Release +15 when garrisoned
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (at.armyType == CommanderBase.ArmyType.Infantry)
            {
                df.tempDamageDecrease -= actionAmount2;
            }
            else if (at.armyType == CommanderBase.ArmyType.Mixed)
            {
                df.tempDamageDecrease -= (actionAmount2 / 3);
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Counterattack damage increased by 20. Debuff to target unit when stationed Increases damage received from infantry Lasts 15 seconds Stacks 10 times Activates once every 10 seconds
            if (actionCount2 == 0)
                actionAmount2 = 0;

            if (at.battleState == CommanderBase.BattleState.Garrison && df.normalAttackDamage > 0 && actionCount2 <= 5)
            {
                actionAmount2++;
                actionAmount2 = Math.Min(10, actionAmount2);
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Gaul Defense Line] Activates a debuff that increases damage taken from infantry by {1}%.", at.site, actionAmount2);
                actionCount2 = 15;
            }
            actionCount2--;
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
       
        }
        public void Passive3Bonus(CommanderBase at, CommanderBase df)
        {
            if (df.silenceTurn <= 1)
                df.silenceTurn = 3; 
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // Reduces normal damage by 10%. 30% chance to debuff twice when casting a skill. Triggers once every 10 seconds
            Random random = new Random();
            if (at.isSkillUsed == true && random.Next(0, 10) < 3 && actionCountNew <= 0)
            {
                actionAmount2 += 2;
                actionAmount2 = Math.Min(10, actionAmount2);
                if (UsingLog.usingLog == true)
                    Console.WriteLine("- {0}[Song of the Nibelung] Activates a debuff that increases damage taken from infantry by {1}%.", at.site, actionAmount2);
                actionCountNew = 10;
            }
            actionCountNew--;
        }
    }
}
