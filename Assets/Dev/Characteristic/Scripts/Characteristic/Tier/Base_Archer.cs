using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Base_Archer : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 150;
            commander.baseDefence = 100;
            commander.baseHealth = 150;
            commander.baseExperiencePoints = 0;
            commander.baseSkill = 5;
            commander.armyType = CommanderBase.ArmyType.Archer;
        }
    }
}
