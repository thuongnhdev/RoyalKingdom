using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : MonoSingleton<PopulationManager>
{
    [SerializeField]
    private GameEvent _onUpdateSalary;

    [SerializeField]
    private GameEvent _onUpdateNaturalDeath;

    [SerializeField]
    private GameEvent _onUpdatePopulationTotal;

    [SerializeField]
    private GameEvent _onUpdatePopulationbabyBoy;

    [SerializeField]
    private GameEvent _onUpdateWorkerStatusBoard;

    [SerializeField]
    private UserItemStorage _userItemStorage;

    private MasterDataStore _masterDataStore;

    private readonly int PopulationGrade = 1;
    private void OnEnable()
    {
        _onUpdateSalary.Subcribe(OnUpdateSalary);
    }

    private void OnDisable()
    {
        _onUpdateSalary.Unsubcribe(OnUpdateSalary);
    }

    public void TotalBirth()
    {
        _masterDataStore = MasterDataStore.Instance;

        int totalBaby = 0;
        for (int i = 0; i < _masterDataStore.GetPopulationAgeInfos().Count; i++)
        {
            var item = _masterDataStore.GetPopulationAgeInfos()[i];
            var populationItem = _masterDataStore.BaseDataAgeMortalitys[i];
            var populationAge = (item.FemaleCount * populationItem.FertiltyProb / 10000 * (1 + PopulationGrade / 100));
            totalBaby += (int)populationAge;
        }
        _onUpdatePopulationTotal.Raise(totalBaby);
    }

    public void BabyBoyBirth()
    {
        _masterDataStore = MasterDataStore.Instance;

        int babyBoy = 0;
        for (int i = 0; i < _masterDataStore.GetPopulationAgeInfos().Count; i++)
        {
            var item = _masterDataStore.GetPopulationAgeInfos()[i];
            var populationItem = _masterDataStore.BaseDataAgeMortalitys[i];
            var populationMaleBirthRate = _masterDataStore.GetMaleBirthRate();
            var populationAge = (item.FemaleCount* populationItem.FertiltyProb * (1 + PopulationGrade / 100 )) *(populationMaleBirthRate / 10000);
            babyBoy += (int)populationAge;
        }
        _onUpdatePopulationbabyBoy.Raise(babyBoy);
    }

    public void NaturalDeath()
    {
        _masterDataStore = MasterDataStore.Instance;
        int totalNaturalDeath = 0;
        for (int i = 0; i < _masterDataStore.GetPopulationAgeInfos().Count; i++)
        {
            var item = _masterDataStore.GetPopulationAgeInfos()[i];
            var populationItem = _masterDataStore.BaseDataAgeMortalitys[i];
            var populationMaleBirthRate = _masterDataStore.GetMaleBirthRate();
            var populationAge = item.MaleCount * (populationItem.BaseDeathProb / 10000) + item.FemaleCount * (populationItem.BaseDeathProb / 10000);
            totalNaturalDeath += (int)populationAge;
        }
        _onUpdateNaturalDeath.Raise(totalNaturalDeath);
    }

    private void OnUpdateSalary(object[] eventParam)
    {
        float gold = _userItemStorage.GetItemCount(990099);
        gold -= (GameData.Instance.SavedPack.SaveData.WorkerGoldSalaryInMonth * 24 * 30);
        var tenGold = new ResourcePoco
        {
            itemId = 990099,
            itemCount = (int)gold
        };
        _userItemStorage.Sub(tenGold);

        float food = _userItemStorage.GetItemCount(100001);
        food -= (GameData.Instance.SavedPack.SaveData.WorkerFoodSalaryInMonth * 24 * 30);
        var tenFood = new ResourcePoco
        {
            itemId = 100001,
            itemCount = (int)food
        };
        _userItemStorage.Sub(tenFood);

        GameData.Instance.SavedPack.SaveData.GoldTotal -= GameData.Instance.SavedPack.SaveData.WorkerGoldSalaryInMonth;
        GameData.Instance.SavedPack.SaveData.FoodTotal -= GameData.Instance.SavedPack.SaveData.WorkerFoodSalaryInMonth;
        GameData.Instance.SavedPack.SaveData.WorkerGoldSalaryInMonth = GameData.Instance.SavedPack.SaveData.WorkerGoldSalary;
        GameData.Instance.SavedPack.SaveData.WorkerFoodSalaryInMonth = GameData.Instance.SavedPack.SaveData.WorkerFoodSalary;
        GameData.Instance.RequestSaveGame();
    }

    public void SubItemResource(int idItem)
    {
        float itemCount = _userItemStorage.GetItemCount(idItem);
        itemCount = 1;
        var tenItem = new ResourcePoco
        {
            itemId = idItem,
            itemCount = (int)itemCount
        };
        _userItemStorage.Sub(tenItem);
    }

    public void AddItemResource(int idItem , RandomOption randomOption)
    {
        float itemCount = _userItemStorage.GetItemCount(idItem);
        itemCount = 1;
        var tenItem = new ResourcePoco
        {
            itemId = idItem,
            itemCount = (int)itemCount
        };
        _userItemStorage.Add(tenItem, randomOption);
    }

    public List<TaskAssignmentTemp> TaskAssignmentTemps = new List<TaskAssignmentTemp>();
    public void WorkerDivision()
    {
        float totalPercent = 0;
        TaskAssignmentTemps.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            totalPercent += GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent;
        }
        int totalWorker = 0;
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            float updatePercent = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent * GameData.Instance.SavedPack.SaveData.WorkerNumber;
            int workerPriority = (int)(updatePercent / totalPercent);
            totalWorker += workerPriority;
        }
        CheckWorkerAssign(totalWorker);

        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            float updatePercent = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent * GameData.Instance.SavedPack.SaveData.WorkerNumber;
            int workerPriority = (int)(updatePercent / totalPercent);
            WorkerAssignment countJobPriority = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i];
            if (countJobPriority.DataAssignments.Count > 0)
            {
                int worker = (int)(workerPriority / countJobPriority.DataAssignments.Count);
                for (var j = 0; j < GameData.Instance.SavedPack.SaveData.ListTask.Count; j++)
                {
                    var taskData = GameData.Instance.SavedPack.SaveData.ListTask[j];

                    var item = TaskAssignmentTemps.Find(t => t.TaskData.TaskId == taskData.TaskId);
                    if (item != null)
                        worker += item.Worker;
                    if (taskData.Priority == GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Priority)
                    {
                        if (taskData.StatusFillResource == StatusFillResource.CompleteFill)
                        {
                            int man = worker - taskData.Man;
                            GameManager.Instance.UpdateWorking(taskData, man, worker, (count) =>
                            {
                                //taskData.TimeJob = TimeWorkLoad.TimeJob(worker, taskData.Workload);
                            });
                            GameData.Instance.SavedPack.SaveData.ListTask[j].Man = worker;
                        }
                        else if (taskData.StatusFillResource == StatusFillResource.None)
                        {
                            CharacterManager.Instance.OnBeginFillResource(taskData, StatusCache.NoCache, taskData.TypeJobAction);
                        }
                    }
                    
                }
            }
        }

        _onUpdateWorkerStatusBoard?.Raise();
    }

    public void CheckWorkerAssign(int totalWorker)
    {
        int numberSub = GameData.Instance.SavedPack.SaveData.WorkerNumber - totalWorker;
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            WorkerAssignment countJobPriority = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i];
            if(GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent > 0)
            {
                var workloadList = SortTaskData(countJobPriority);
                for (int j = 0; j < workloadList.Count; j++)
                {
                    int worker = 1;
                    var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.TaskId == workloadList[j].TaskId);
                    if (taskData != null)
                    {
                        var item = TaskAssignmentTemps.Find(t => t.TaskData.TaskId == taskData.TaskId);
                        if (item == null)
                        {
                            if (numberSub > 0)
                                TaskAssignmentTemps.Add(new TaskAssignmentTemp(taskData, worker));
                        }
                        else
                        {
                            var index = TaskAssignmentTemps.FindIndex(t => t.TaskData.TaskId == taskData.TaskId);
                            TaskAssignmentTemps[index].Worker += worker;
                        }
                        totalWorker++;
                        numberSub -= worker;
                    }
                }
            }
        }
        //if (numberSub > 0)
        //    CheckWorkerAssign(totalWorker);
    }

    private List<DataAssignment> SortTaskData(WorkerAssignment workerAssignment)
    {
        List<DataAssignment> workloadList = workerAssignment.DataAssignments;
        workloadList.Sort(

    (o1, o2) =>
    {
        int taskId1 = o1.TaskId;
        int taskId2 = o2.TaskId;
        var taskData1 = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.TaskId == taskId1);
        var taskData2 = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.TaskId == taskId2);
        if (taskData1 == null || taskData2 == null) return 0;
        if (taskData1.Workload > taskData2.Workload)
        {
            return 1;
        }
        else if (taskData1.Workload == taskData2.Workload)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

);
        return workloadList;
    }

}
