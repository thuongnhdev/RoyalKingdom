
using CoreData.UniFlow.Commander;
using CoreData.UniFlow.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Equip.Legendary
{
    internal class Concealed_Dagger : EquipmentBase
    {
        public override void BeforeAction(CommanderBase at)
        {
            at.tempAttack += actionAmount;
        }

        public override void AfterAction(CommanderBase at)
        {

        }
    }
}
