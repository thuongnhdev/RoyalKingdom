using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreData.UniFlow.Common;
using CoreData.UniFlow;
using _0.PersonalWork.Harry.Scripts.Common.Static;

public class MasterDataStore : MonoSingleton<MasterDataStore>
{

    public List<BaseConst> BaseConsts;

    public List<BaseDataEnum> BaseDataEnums;

    public List<BaseDataAgeMortality> BaseDataAgeMortalitys;

    public List<BaseDataPopulationTemplate> BaseDataPopulationTemplates;

    public List<BaseDataMoodTemplate> BaseDataMoodTemplates;

    public List<BaseDataPopulationSalary> BaseDataPopulationSalarys;

    public List<BaseDataPopulationSalaryStep> BaseDataPopulationSalarySteps;

    public List<BasePopulationMorale> BasePopulationMorales;

    public List<BaseDataVassalGrade> BaseDataVassalGrades;

    public List<BaseDataVassalJobGrade> BaseDataVassalJobGrades;

    public List<BaseDataVassalNature> BaseDataVassalNatures;

    public List<BaseDataVassalStat> BaseDataVassalStats;

    public List<BaseDataVassalStatsExp> BaseDataVassalStatsExps;

    public List<BaseDataVassalStatsExpInGame> BaseDataVassalStatsExpIngames;

    public List<BaseDataVassalJobClass> BaseDataVassalJobClasss;

    public List<BaseVassalStatus> BaseVassalStatus;

    public List<BaseVassalTaskInfo> BaseVassalTaskInfos;

    public List<BaseVassalTaskInfo> BaseVassalTaskInfoSubs;

    public List<BaseVassalJobInfo> BaseVassalJobInfos;

    public List<BaseVassalJobTreeDefaut> BaseVassalJobTreeDefauts;

    public List<BaseVassalTemplate> BaseVassalTemplates;

    public List<BaseDataSeasonDate> BaseDataSeasonDates;

    public List<BaseDataSeasonTime> BaseDataSeasonTimes;

    public PopulationBase PopulationBase;

    public List<BaseLoginVassalInfo> BaseLoginVassalInfos;

    public List<BaseLoginInfoPopulationCitzen> BaseLoginInfoPopulations;

    public List<EntityPopulationAgeInfo> EntityPopulationAgeInfos;

    public List<PopulationChangeDetailColum> PopulationChangeDetailColums;

    public List<PopulationChangeDetailTable> PopulationChangeDetailTables;

    public List<BaseDataPriority> BaseDataPrioritys;

    public List<ResponseLogChangeWorker> ResponseLogChangeWorkers;

    public List<BaseMilitaryTemplate> BaseMilitaryTemplates;

 

    [SerializeField]
    private List<VassarData> vassarDatas = new List<VassarData>();

    [SerializeField]
    private List<WorkerData> workerDatas = new List<WorkerData>();

    [SerializeField]
    private Entity_POPULATION_WORKEROBJECT _populationWorkerObject;

    public enum CitiZenType
    {
        Citizen = 0,
        Worker = 1,
        Military = 2
    }

    public List<VassarData> VassarDatas
    {
        get { return vassarDatas; }
    }

    public List<WorkerData> WorkerDatas
    {
        get { return workerDatas; }
    }

    public Entity_POPULATION_WORKEROBJECT PopulationWorkerObject
    {
        get { return _populationWorkerObject; }
    }

    public int GetCountWorker(int number)
    {
        if (number == 0) return number;
        for (int i = 0; i < _populationWorkerObject.sheets[0].list.Count; i++)
        {
            if (number >= _populationWorkerObject.sheets[0].list[i].MinWorkers && number <= _populationWorkerObject.sheets[0].list[i].MaxWorkers)
            {
                int count = (int)_populationWorkerObject.sheets[0].list[i].WorkerCount;
                return count;
            }
        }
        return 1;
    }

    public void Init()
    {
        try
        {
            for(int i = 0;i< BaseLoginVassalInfos.Count;i++)
            {
                VassalDataInGame data = new VassalDataInGame(BaseLoginVassalInfos[i], false, -1, "empty", -1, 0, 0, 0);
                var vassalData = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalPlayer == BaseLoginVassalInfos[i].idVassalPlayer && t.Data.idVassalTemplate == BaseLoginVassalInfos[i].idVassalTemplate);
                if (vassalData == null)
                {
                    data.Data.Job_1 = -1;
                    data.Data.Job_2 = -1;
                    data.Data.Job_3 = -1;
                    GameData.Instance.SavedPack.SaveData.VassalInfos.Add(data);
                }
            }

            for(int i = 0;i< 10;i++)
            {
                var itemEnum = GameData.Instance.SavedPack.SaveData.ListDataEnums.Find(t => t.Id == BaseDataEnums[i].Id);
                if(itemEnum == null)
                {
                    BaseDataEnumGame baseDataEnumGame = new BaseDataEnumGame();
                    baseDataEnumGame.Id = BaseDataEnums[i].Id;
                    baseDataEnumGame.Memo = BaseDataEnums[i].Memo;
                    baseDataEnumGame.Enum = BaseDataEnums[i].Enum;
                    baseDataEnumGame.IsPin = false;
                    baseDataEnumGame.PrePriority = -1;
                    GameData.Instance.SavedPack.SaveData.ListDataEnums.Add(baseDataEnumGame);
                }
            }
            GameData.Instance.RequestSaveGame();
        }
        catch (System.Exception ex)
        {
           Debug.Log($"MasterDataStore Init. Error: {ex.Message}");
        }
    }

    public BaseLoginVassalInfo GetVassarByID(int id)
    {
        BaseLoginVassalInfo vassar = null;
        var VassalDataInGame = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == id);
        if (VassalDataInGame != null) return VassalDataInGame.Data;

        return vassar;
    }

    public void SetPopulationAgeInfos()
    {
        EntityPopulationAgeInfos.Clear();
        for (int i = 0; i < BaseLoginInfoPopulations.Count; i++)
        {
            var item = EntityPopulationAgeInfos.Find(t => t.Age == BaseLoginInfoPopulations[i].Age);
            if (item == null)
            {
                EntityPopulationAgeInfo ageItem = new EntityPopulationAgeInfo();
                ageItem.Age = BaseLoginInfoPopulations[i].Age;
                if (BaseLoginInfoPopulations[i].Gender == 0)
                    ageItem.MaleCount = BaseLoginInfoPopulations[i].Count;
                else
                    ageItem.FemaleCount = BaseLoginInfoPopulations[i].Count;
        
                EntityPopulationAgeInfos.Add(ageItem);
            }
            else
            {
                var indext = EntityPopulationAgeInfos.FindIndex(t => t.Age == BaseLoginInfoPopulations[i].Age);
                if (BaseLoginInfoPopulations[i].Gender == 0)
                    item.MaleCount += BaseLoginInfoPopulations[i].Count;
                else
                    item.FemaleCount += BaseLoginInfoPopulations[i].Count;
                EntityPopulationAgeInfos[indext] = item;
            }
        }
    }

    public int GetCitiZenTypeCount(CitiZenType citiZenType)
    {
        int count = 0;
        for (int i = 0; i < BaseLoginInfoPopulations.Count; i++)
        {
           if(BaseLoginInfoPopulations[i].CitizenJobId == (int)citiZenType)
            {
                count = count + BaseLoginInfoPopulations[i].Count;
            }
        }
        return count;
    }

    public List<EntityPopulationAgeInfo> GetPopulationAgeInfos()
    {
        if (EntityPopulationAgeInfos.Count == 0)
            SetPopulationAgeInfos();
        return EntityPopulationAgeInfos;
    }
    public int GetMaleBirthRate()
    {
        var item = BaseConsts.Find(t => t.NameConst == "POPULATION_MALE_BIRTH_RATE");
        if (item != null)
            return (int)item.Value;
        return 0;
    }

    public int GetNaturalChangeMonth()
    {
        var item = BaseConsts.Find(t => t.NameConst == "POPULATION_NATURAL_CHANGE_MONTH");
        if (item != null)
            return (int)item.Value;
        return 0;
    }

    public int GetNaturalChangeTime()
    {
        var item = BaseConsts.Find(t => t.NameConst == "POPULATION_NATURAL_CHANGE_TIME");
        if (item != null)
            return (int)item.Value;
        return 0;
    }

    public int GetPopulationWorkerTimeComeOut()
    {
        var item = BaseConsts.Find(t => t.NameConst == "POPULATION_WORKER_TIME_COMEOUT");
        if (item != null)
            return (int)item.Value;
        return 0;
    }

    public int GetWorkerComeBackHouseTimeOut()
    {
        var item = BaseConsts.Find(t => t.NameConst == "POPULATION_WORKER_COMEBACK_TIME_COMEOUT");
        if (item != null)
            return (int)item.Value;
        return 0;
    }

    public BaseVassalTaskInfo GetBaseVassalTaskInfo(int key)
    {
        var item = BaseVassalTaskInfos.Find(t => t.Key == key);
        if (item != null)
            return item;
        return null;
    }

    public void ChangeVassalInTask(int idVassalPlayer , int buildingPlayerId)
    {

        var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingPlayerId);
        var indexTaskData = GameData.Instance.SavedPack.SaveData.ListTask.FindIndex(t => t.BuildingObjectId == buildingPlayerId);
        var vassalData = MasterDataStore.Instance.GetVassarByID(idVassalPlayer);
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            var data = GameData.Instance.SavedPack.SaveData.VassalInfos[i];
            if (data.SubTaskId == taskData.TaskId)
            {
                // calculator trigger done workload 
                JobManager.Instance.TriggerDoneWorkload(taskData, data);
                var taskInfo = MasterDataStore.Instance.BaseVassalTaskInfos[0];
                data.KeyJob = -1;
                data.IsWorking = false;
                data.TaskName = "empty";
                data.SubTaskId = -1;
                data.WorkloadStart = 0;
                GameData.Instance.SavedPack.SaveData.VassalInfos[i] = data;
            }
        }
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            var data = GameData.Instance.SavedPack.SaveData.VassalInfos[i];
            if (data.KeyJob == -1 && data.Data.idVassalTemplate == idVassalPlayer)
            {
                var taskInfo = MasterDataStore.Instance.BaseVassalTaskInfos[0];
                data.KeyJob = taskInfo.Key;
                data.IsWorking = true;
                data.Data = vassalData;
                if (taskData != null)
                    data.TaskName = taskData.Name;
                else
                    data.TaskName = "";
                data.SubTaskId = taskData.TaskId;
                GameData.Instance.SavedPack.SaveData.ListTask[indexTaskData].IdVassal = data.Data.idVassalTemplate;
                GameData.Instance.SavedPack.SaveData.VassalInfos[i] = data;
                JobManager.Instance.SetWorkloadStart(taskData, data);
            }
        }

        GameData.Instance.RequestSaveGame();
    }

    public BaseDataVassalStatsExp GetLevelVassalStatExp(float expStat)
    {
        for(int i = 0; i< BaseDataVassalStatsExps.Count;i++)
        {
            if (expStat < BaseDataVassalStatsExps[i].Max_exp)
                return BaseDataVassalStatsExps[i];
        }
        return BaseDataVassalStatsExps[0];
    }

    public void SetBaseDataVassalStatsExpsIngame()
    {
        BaseDataVassalStatsExpIngames = new List<BaseDataVassalStatsExpInGame>();
        for (int i = 0; i < BaseDataVassalStatsExps.Count; i++)
        {
            BaseDataVassalStatsExpInGame data = new BaseDataVassalStatsExpInGame();
            data.Lv = BaseDataVassalStatsExps[i].Lv;
            data.Max_exp = BaseDataVassalStatsExps[i].Max_exp;
            for(int j = 0;j < i;j++)
            {
                data.TotalExpLevelUp += BaseDataVassalStatsExps[j].Max_exp;
            }
            BaseDataVassalStatsExpIngames.Add(data);
        }
    }
}
