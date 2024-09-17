using CoreData.UniFlow.Commander;
using CoreData.UniFlow.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Skill
{
    internal class SkillBase : MethodBase
    {
        public override void Init(CommanderBase bs, bool isFirst = true, int cnt = 0)
        {
          
        }

        public virtual void ActiveBefore(CommanderBase at, CommanderBase df) { }
        protected int actionCount0 = 0;
        protected double actionAmount0 = 0;
        public virtual void Active(CommanderBase at, CommanderBase df) { }

        protected int actionCount1 = 0;
        protected double actionAmount1 = 0;
        public virtual void Passive1Before(CommanderBase at, CommanderBase df) { }
        public virtual void Passive1After(CommanderBase at, CommanderBase df) { }

        protected int actionCount2 = 0;
        protected double actionAmount2 = 0;
        public virtual void Passive2Before(CommanderBase at, CommanderBase df) { }
        public virtual void Passive2After(CommanderBase at, CommanderBase df) { }

        protected int actionCount3 = 0;
        protected double actionAmount3 = 0;
        public virtual void Passive3Before(CommanderBase at, CommanderBase df) { }
        public virtual void Passive3After(CommanderBase at, CommanderBase df) { }

        protected int actionCountNew = 0;
        protected double actionAmountNew = 0;
        public virtual void NewBefore(CommanderBase at, CommanderBase df) { }
        public virtual void NewAfter(CommanderBase at, CommanderBase df) { }
    }
}
