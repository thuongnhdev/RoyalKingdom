using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Base_Cavalry : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 250;
            commander.baseDefence = 200;
            commander.baseHealth = 250;
            commander.baseExperiencePoints = 0;
            commander.baseSkill = 5;
            commander.armyType = CommanderBase.ArmyType.Cavalry;
        }
    }
}
