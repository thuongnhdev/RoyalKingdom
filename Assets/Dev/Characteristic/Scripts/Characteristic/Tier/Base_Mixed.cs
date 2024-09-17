using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Base_Mixed : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 220;
            commander.baseDefence = 213;
            commander.baseHealth = 215;
            commander.armyType = CommanderBase.ArmyType.Mixed;
        }
    }
}
