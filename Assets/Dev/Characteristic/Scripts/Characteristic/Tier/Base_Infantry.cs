using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Base_Infantry : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 200;
            commander.baseDefence = 150;
            commander.baseHealth = 200;
            commander.baseExperiencePoints = 0;
            commander.baseSkill = 5;
            commander.armyType = CommanderBase.ArmyType.Infantry;
        }
    }
}
