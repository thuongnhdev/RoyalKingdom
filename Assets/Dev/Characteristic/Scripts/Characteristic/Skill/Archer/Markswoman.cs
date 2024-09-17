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
    internal class Markswoman : SkillBase
    {
        public override void Active(CommanderBase at, CommanderBase df)
        {
            // 100 counting skills
            if (UsingLog.usingLog == true)
                Console.Write("- {0}[surprise attack]", at.site);
            CalcDamage.CalcActiveSkillDamage(at, df, 100);
        }

        public override void Passive1Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive1After(CommanderBase at, CommanderBase df)
        {
            // Barbarian damage increased by 10%
        }

        public override void Passive2Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive2After(CommanderBase at, CommanderBase df)
        {
            // Increases archer attack power by 5%
        }

        public override void Passive3Before(CommanderBase at, CommanderBase df)
        {
        }
        public override void Passive3After(CommanderBase at, CommanderBase df)
        {
            // Increases archer defense by 5%
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
