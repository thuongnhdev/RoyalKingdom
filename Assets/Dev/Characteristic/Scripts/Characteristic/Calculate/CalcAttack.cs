using CoreData.UniFlow.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CoreData.UniFlow.Common.MethodBase;

namespace CoreData.UniFlow.Calculate
{
    internal static class CalcAttack
    {
        public static void CalcActiveSkill(CommanderBase at, CommanderBase df)
        {
      
        }
        public static void CalcNormalAttack(CommanderBase at, CommanderBase df)
        {
            if (at.forbiddenTurn <= 0)
                CalcDamage.CalcNormalDamage(at, df);// No normal attack in embargoed state - Không có cuộc tấn công bình thường trong trạng thái bị cấm vận..
        }
        public static void CalcCounterAttack(CommanderBase at, CommanderBase df)
        {
            CalcDamage.CalcCounterDamage(at, df);
        }
    }
}
