using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    public abstract class TierBase
    {
        public abstract void Init(CommanderBase commander);
    }
}
