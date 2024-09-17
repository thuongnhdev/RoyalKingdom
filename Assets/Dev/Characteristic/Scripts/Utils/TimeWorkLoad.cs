using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public static class TimeWorkLoad
    {
        public static float TimeOneWorkloadWorker()
        {
            var masterDataStore = MasterDataStore.Instance;
            var amountWorker = masterDataStore.BaseConsts.Find(t=>t.NameConst == "TIMETASK_DEFAULT_AMOUNT_WORKER");
            var speedCriteria = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_SPEED_CRITERIA");
            var amountVassal = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_AMOUNT_VASSAL");
            var defaulWorkload = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_INCLUDE_SMALLWORKLOAD");
            float time = (amountWorker.Value / speedCriteria.Value) * defaulWorkload.Value;
            float timeInGame = time * GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
            return timeInGame;
        }

        public static float TimeOneWorkloadVassal()
        {
            var masterDataStore = MasterDataStore.Instance;
            var amountWorker = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_AMOUNT_WORKER");
            var speedCriteria = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_SPEED_CRITERIA");
            var amountVassal = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_AMOUNT_VASSAL");
            var defaulWorkload = masterDataStore.BaseConsts.Find(t => t.NameConst == "TIMETASK_DEFAULT_INCLUDE_SMALLWORKLOAD");
            float time = (amountVassal.Value / speedCriteria.Value) * defaulWorkload.Value;
            float timeInGame = time * GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
            return timeInGame;
        }

        public static float SpeedRate(int worker)
        {
            float speed = worker / GameData.Instance.SavedPack.SaveData.TimeWorkloadOneWorker;
            return speed;
        }

        public static float TimeJob(int count, TaskData taskData)
        {
            int level = (int)TypeJobActionBuilding.CONTRUCTION;
            if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
                level = (int)TypeJobActionBuilding.UPGRADE;

            float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(taskData.BuildingId, level);
            if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                workload = VariableManager.Instance.ProductFormulaList.GetTimeCost(taskData.CurrentProductId);
            float time = (GameData.Instance.SavedPack.SaveData.TimeWorkloadOneWorker / count) * workload;
            return time;
        }
    }
}
