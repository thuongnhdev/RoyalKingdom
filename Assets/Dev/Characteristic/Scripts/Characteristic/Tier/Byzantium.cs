using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Tier
{
    internal class Byzantium : TierBase
    {
        public override void Init(CommanderBase commander)
        {
            commander.baseAttack = 221;
            commander.baseDefence = 222;
            commander.baseHealth = 216;
            commander.baseExperiencePoints = 0;
            commander.baseSkill = 5;
            commander.armyType = CommanderBase.ArmyType.Cavalry;
        }
    }
}
