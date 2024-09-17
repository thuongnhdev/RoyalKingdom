using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Viking : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 221;
            commander.baseDefence = 216;
            commander.baseHealth = 222;
            commander.baseSkill = 5;
            commander.baseExperiencePoints = 0;
            commander.armyType = CommanderBase.ArmyType.Infantry;
        }
    }
}
