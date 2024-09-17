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
    internal class Zenobia : SkillBase
    {
        public override void ActiveBefore(CommanderBase at, CommanderBase df)
        {
            at.tempHealth += actionAmount0;
 
            actionCount0--;
            if (actionCount0 == 0)
                actionAmount0 = 0;
        }

        public override void Active(CommanderBase at, CommanderBase df)
        {
          
        }
        public void ActiveBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Queen of Palmyra]", at.site);
            CalcDamage.CalcHealingEffect(at, df, 1100);
        }

        public void ActiveBonusStart(CommanderBase at, CommanderBase df)
        {
            if (at.activeDamageIncrease_bf < 30)
            {
                at.activeDamageIncrease_bf = 30;
            }
        }

        public void ActiveBonusEnd(CommanderBase at, CommanderBase df)
        {
            if (at.activeDamageIncrease_bf == 30)
            {
                at.activeDamageIncrease_bf = 0;
            }
        }



        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
            if (at.battleState == CommanderBase.BattleState.Garrison)
            {
                actionAmount1 = 15;
                at.tempNormalDamageDecrease += actionAmount1;
                at.tempNormalDamageIncrease += actionAmount1;
            }
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // When defending a base, normal attack damage is reduced by 15%, normal attack damage is increased by 15%
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
            if (df.battleState == CommanderBase.BattleState.Conquering)
            {
                actionAmount2 = 10;
                at.tempDamageIncrease += actionAmount2;
            }
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Infantryman 20 balls increased by 20. Increases damage to assembly units by 10%
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
      
        }
        public void Passive3Bonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[The Rule of Pluralism]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, actionAmount3);
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
        }
    }
}
