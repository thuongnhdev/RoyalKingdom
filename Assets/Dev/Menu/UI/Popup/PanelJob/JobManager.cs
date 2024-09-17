using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;

public class JobManager : MonoSingleton<JobManager>
{
    [SerializeField]
    private UserBuildingList _userBuildingList;

    [SerializeField]
    private ProductFormulaList _productFormulaList;

    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;

    [SerializeField]
    private UserBuildingProductionList _userBuildingProductionList;

    private MasterDataStore masterDataStore;

    public void InitData()
    {
        masterDataStore = MasterDataStore.Instance;
        for(int i = 0;i <masterDataStore.BaseVassalJobInfos.Count;i++)
        {
            var data = masterDataStore.BaseVassalJobInfos[i];
            var itemJob = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == data.Key);
            if(itemJob == null)
            {
                var converted = new JobData();
                converted.Key = data.Key;
                converted.JobName = data.JobName;
                converted.JobClass = data.JobClass;
                converted.JobGrade = data.JobGrade;
                converted.Level = 0;

                int statsCount = masterDataStore.BaseDataVassalStats.Count;
                List<BaseStatsOfJob> convertedStats = new();
                for (int j = 0; j < statsCount; j++)
                {
                    var fbStatsData = masterDataStore.BaseDataVassalStats[j];
                    var convertedStat = new BaseStatsOfJob()
                    {
                        Key = fbStatsData.Key,
                        StatName = fbStatsData.StatName,
                        Value = 0
                    };
                    convertedStats.Add(convertedStat);
                }
                converted.StatsData = convertedStats;
                GameData.Instance.SavedPack.SaveData.JobDatas.Add(converted);
            }
        }
    }

    public void SetWorkloadStart(TaskData taskData, VassalDataInGame vassal)
    {
        if (taskData == null) return;
        float workload = 0;
        UserBuilding userBuilding = _userBuildingList.GetBuilding(taskData.BuildingObjectId);
        if (taskData.TypeJobAction == TypeJobAction.DESTROY)
            workload = _buildingUpgradeInfos.GetTimeCost(taskData.BuildingId, userBuilding.buildingLevel);
        else
            workload = _buildingUpgradeInfos.GetTimeCost(taskData.BuildingId, userBuilding.buildingLevel + 1);
        float workloadProduct = _productFormulaList.GetTimeCost(taskData.CurrentProductId);
        if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
            workload = workloadProduct;
        vassal.WorkloadStart = (int)(workload - taskData.Workload);
        GameData.Instance.RequestSaveGame();
    }
    public void TriggerWorkloadDecrease(TaskData taskData)
    {
        if (taskData.Workload < 1 || taskData.Man == 0) return;
        var duration = GameData.Instance.SavedPack.SaveData.TimeWorkloadOneWorker / taskData.Man;
        TriggerTimer timer = new TriggerTimer();
        timer.AddTaskTimer(duration, taskData, null, (builldingObjectId) =>
        {
            var index = GameData.Instance.SavedPack.SaveData.ListTask.FindIndex(t => t.BuildingObjectId == builldingObjectId);
            if (GameData.Instance.SavedPack.SaveData.ListTask[index] != null)
            {
                GameData.Instance.SavedPack.SaveData.ListTask[index].WorkloadDone++;
                TriggerWorkloadDecrease(GameData.Instance.SavedPack.SaveData.ListTask[index]);
            } 
        });
    }

    public void TriggerDoneWorkload(TaskData taskData, VassalDataInGame vassal)
    {
        if (taskData == null || vassal == null) return;
   
        UserBuilding userBuilding = _userBuildingList.GetBuilding(taskData.BuildingObjectId);
        int level = (int)TypeJobActionBuilding.CONTRUCTION;
        if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
            level = (int)TypeJobActionBuilding.UPGRADE;
        float workload = _buildingUpgradeInfos.GetTimeCost(taskData.BuildingId, level);
        float workloadProduct = _productFormulaList.GetTimeCost(taskData.CurrentProductId);
        if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
            workload = workloadProduct;

        int amountOfWorkload = (int)((taskData.WorkloadDone - vassal.WorkloadStart));
        if(amountOfWorkload > 0)
        {
            UpdateValue(amountOfWorkload, userBuilding.buildingLevel, taskData.BuildingObjectId);
        }
    }

    public int WorkloadsDone(TaskData taskData)
    {
        UserBuilding userBuilding = _userBuildingList.GetBuilding(taskData.BuildingObjectId);
        int level = (int)TypeJobActionBuilding.CONTRUCTION;
        if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
            level = (int)TypeJobActionBuilding.UPGRADE;

        float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(taskData.BuildingId, level);
        if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
            workload = VariableManager.Instance.ProductFormulaList.GetTimeCost(taskData.CurrentProductId);
        int amountOfWorkload = (int)taskData.WorkloadDone;
        return amountOfWorkload;
    }

    public int WorkloadsDoneProduction(TaskData taskData)
    {
        UserBuildingProduction userBuildingProduction = _userBuildingProductionList.GetBuildingProduction(taskData.BuildingObjectId);
        float workload = _productFormulaList.GetTimeCost(userBuildingProduction.CurrentProductId);
        int amountOfWorkload = (int)taskData.WorkloadDone;
        return amountOfWorkload;
    }

    public void UpdateValue(int amountOfWorkload,int level, int buildingObjectId)
    {
        var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
        var vassalPlayer = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.SubTaskId == taskData.TaskId);
        if (vassalPlayer == null) return;
        FinalObtainedStatEXP(amountOfWorkload, vassalPlayer.Data.idVassalTemplate, vassalPlayer.TaskName);
        JobClassEXPIncrease(amountOfWorkload, level, vassalPlayer.Data.idVassalTemplate, vassalPlayer.TaskName);
    }

    public void JobClassEXPIncrease(int amountOfWorkload,int level , int idVassalTemplate, string taskName)
    {

        int keyJobClass_1 = -1, keyJobClass_2 = -1, keyJobClass_3 = -1;
        var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == idVassalTemplate);
        var vassalTaskInfo = masterDataStore.BaseVassalTaskInfos.Find(t => t.TaskName == taskName);
        if (vassalTaskInfo != null)
        {
            if (vassalInfo.Data.Job_1 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB)
                keyJobClass_1 = masterDataStore.BaseVassalJobInfos.Find(t => t.Key == vassalInfo.Data.Job_1).JobClass;
            if (vassalInfo.Data.Job_2 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB)
                keyJobClass_2 = masterDataStore.BaseVassalJobInfos.Find(t => t.Key == vassalInfo.Data.Job_2).JobClass;
            if (vassalInfo.Data.Job_3 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB)
                keyJobClass_3 = masterDataStore.BaseVassalJobInfos.Find(t => t.Key == vassalInfo.Data.Job_3).JobClass;

            for (int i = 0; i < vassalInfo.Data.BaseDataVassalJobClassInfos.Count; i++)
            {
                if (vassalInfo.Data.BaseDataVassalJobClassInfos[i].IdJobClass == keyJobClass_1)
                {
                    vassalInfo.Data.BaseDataVassalJobClassInfos[i].ExpJobClass = (int)JobClassEXP(amountOfWorkload, vassalInfo, vassalTaskInfo);
                }
                if (vassalInfo.Data.BaseDataVassalJobClassInfos[i].IdJobClass == keyJobClass_2)
                {
                    vassalInfo.Data.BaseDataVassalJobClassInfos[i].ExpJobClass = (int)JobClassEXP(amountOfWorkload, vassalInfo, vassalTaskInfo);
                }
                if (vassalInfo.Data.BaseDataVassalJobClassInfos[i].IdJobClass == keyJobClass_3)
                {
                    vassalInfo.Data.BaseDataVassalJobClassInfos[i].ExpJobClass = (int)JobClassEXP(amountOfWorkload, vassalInfo, vassalTaskInfo);
                }
            }
            
        }
        UpdateLevelVassal(vassalInfo);
        GameData.Instance.RequestSaveGame();
    }

    public void FinalObtainedStatEXP(int amountOfWorkload,int idVassalTemplate, string taskName)
    {
        var vassaltemplate = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == idVassalTemplate);
        var vassalTaskInfo = masterDataStore.BaseVassalTaskInfos.Find(t => t.TaskName == taskName);
        var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == idVassalTemplate);
        if (vassalTaskInfo != null)
        {
            float statValue_1 = OnceObtainedStatEXP(amountOfWorkload, vassalTaskInfo.EXP_1, vassaltemplate, vassalTaskInfo.StatKey_1);
            float statValue_2 = OnceObtainedStatEXP(amountOfWorkload, vassalTaskInfo.EXP_2, vassaltemplate, vassalTaskInfo.StatKey_2);
            float statValue_3 = OnceObtainedStatEXP(amountOfWorkload, vassalTaskInfo.EXP_3, vassaltemplate, vassalTaskInfo.StatKey_3);
            float statValue_4 = OnceObtainedStatEXP(amountOfWorkload, vassalTaskInfo.EXP_4, vassaltemplate, vassalTaskInfo.StatKey_4);
            var vassalInfoStat_1 = vassalInfo.Data.BaseDataVassalStats.Find(t => t.Key == vassalTaskInfo.StatKey_1);
            vassalInfoStat_1.Value = vassalInfoStat_1.Value + statValue_1;
            var vassalInfoStat_2 = vassalInfo.Data.BaseDataVassalStats.Find(t => t.Key == vassalTaskInfo.StatKey_2);
            vassalInfoStat_2.Value = vassalInfoStat_2.Value + statValue_2;
            var vassalInfoStat_3 = vassalInfo.Data.BaseDataVassalStats.Find(t => t.Key == vassalTaskInfo.StatKey_3);
            vassalInfoStat_3.Value = vassalInfoStat_3.Value + statValue_3;
            var vassalInfoStat_4 = vassalInfo.Data.BaseDataVassalStats.Find(t => t.Key == vassalTaskInfo.StatKey_4);
            vassalInfoStat_4.Value = vassalInfoStat_4.Value + statValue_4;
        }
       
        GameData.Instance.RequestSaveGame();
    }

    private float OnceObtainedStatEXP(int amountOfWorkload,float expValue,BaseVassalTemplate vassaltemplate, int statKey)
    {
        if(amountOfWorkload < 0) return 0;
        BaseDataVassalStatValue vassalStatValue = vassaltemplate.BaseDataVassalStats.Find(t => t.Key == statKey);
        float potentialValueOfVassal = vassalStatValue.Value;
        float obtainedStatEXP = amountOfWorkload * expValue / ShareUIManager.Instance.Config.VASSAL_STAT_EXP_DIVIDE + amountOfWorkload * potentialValueOfVassal / ShareUIManager.Instance.Config.VASSAL_STAT_POTENTIAL_DIVIDE;
        float statsExp = UnityEngine.Random.Range((obtainedStatEXP * 90) / 100, (obtainedStatEXP * 110) / 100);
        if (statsExp < 0) statsExp = Mathf.Abs(statsExp);
        return statsExp;
    }

    public float JobClassEXP(int amountOfWorkload, VassalDataInGame vassal,BaseVassalTaskInfo taskInfo)
    {

        int max_exp = taskInfo.Job_Class_EXP;
        float statExp = amountOfWorkload * max_exp / ShareUIManager.Instance.Config.VASSAL_JOB_CLASS_EXP_DIVIDE;
        float jobClassStats = UnityEngine.Random.Range((statExp * 90) / 100, (statExp * 110) / 100);
        return jobClassStats;
    }

    public void UpdateLevelVassal(VassalDataInGame vassal)
    {
        for(int i = 0;i< MasterDataStore.Instance.BaseDataVassalStatsExps.Count;i++)
        {
            for (int j = 0; j < vassal.Data.BaseDataVassalJobClassInfos.Count; j++)
            {
                if (vassal.Data.BaseDataVassalJobClassInfos[j].ExpJobClass > MasterDataStore.Instance.BaseDataVassalStatsExps[i].Max_exp)
                    vassal.Data.BaseDataVassalJobClassInfos[j].LvJobClass = MasterDataStore.Instance.BaseDataVassalStatsExps[i].Lv;
            }
        }
        for(int j = 0;j< GameData.Instance.SavedPack.SaveData.VassalInfos.Count;j++)
        {
            if (vassal.Data.idVassalTemplate == GameData.Instance.SavedPack.SaveData.VassalInfos[j].Data.idVassalTemplate)
                GameData.Instance.SavedPack.SaveData.VassalInfos[j] = vassal;
        }
        GameData.Instance.RequestSaveGame();
    }
}
