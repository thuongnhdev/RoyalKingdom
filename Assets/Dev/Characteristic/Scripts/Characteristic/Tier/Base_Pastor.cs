using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Base_Pastor : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseHealth = 100;
            commander.baseEvangelism = 50;
            commander.civiliansType = CommanderBase.CiviliansType.Pastor;
        }
    }
}