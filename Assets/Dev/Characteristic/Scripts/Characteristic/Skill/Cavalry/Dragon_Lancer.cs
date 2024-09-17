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
    internal class Dragon_Lancer : SkillBase
    {

        public double extraDamage;
        public override void Active(CommanderBase at, CommanderBase df)
        {
   
        }

        public void ActiveBonus(CommanderBase at, CommanderBase df)
        {
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[Threat]", at.site);
            CalcDamage.CalcAdditionalSkillDamage(df, extraDamage);
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry movement speed by 10%
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry attack power by 5%
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Increases cavalry defense by 5%
        }

        public override void NewBefore(CommanderBase at, CommanderBase df)
        {
        }
        public override void NewAfter(CommanderBase at, CommanderBase df)
        {
            // No awakening skill
        }
    }
}
