using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Korea : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 221;
            commander.baseDefence = 227;
            commander.baseHealth = 212;
            commander.baseSkill = 5;
            commander.baseExperiencePoints = 0;
            commander.armyType = CommanderBase.ArmyType.Archer;
        }
    }
}
