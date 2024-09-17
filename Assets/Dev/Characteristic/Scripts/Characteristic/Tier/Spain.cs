using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Spain : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 232;
            commander.baseDefence = 212;
            commander.baseHealth = 216;
            commander.baseSkill = 5;
            commander.baseExperiencePoints = 0;
            commander.armyType = CommanderBase.ArmyType.Cavalry;
        }
    }
}
