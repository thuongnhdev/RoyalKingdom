using CoreData.UniFlow.Commander;
using CoreData.UniFlow.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Equip
{
    internal class EquipmentBase : MethodBase
    {
        protected bool isStrengthen = false;
        public override void Init(CommanderBase bs, bool _isStrengthen = true, int cnt = 0)
        {
            isStrengthen = _isStrengthen; 
            bs.before_skill.Add(new DelegateMethod(BeforeAction));
            bs.after_skill.Add(new DelegateMethod(AfterAction));
        }
        public virtual void BeforeAction(CommanderBase at)
        {

        }
        protected int actionCount = 0;
        protected double actionAmount = 0;
        public virtual void AfterAction(CommanderBase at)
        {

        }

    }
}
