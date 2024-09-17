using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    public class Base_Farmer : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseHealth = 100;
            commander.baseHarvest = 50;
            commander.civiliansType = CommanderBase.CiviliansType.Farmer;
        }
    }
}