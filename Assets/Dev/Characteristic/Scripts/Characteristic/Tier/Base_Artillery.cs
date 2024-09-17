using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Base_Artillery : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 300;
            commander.baseDefence = 250;
            commander.baseHealth = 300;
            commander.baseExperiencePoints = 0;
            commander.baseSkill = 5;
            commander.armyType = CommanderBase.ArmyType.Artillery;
        }
    }
}
