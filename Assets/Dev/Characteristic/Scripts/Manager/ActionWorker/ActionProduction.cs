using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class ActionProduction : MonoSingleton<ActionProduction>
    {
        public void OnStartNewProduction(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            int buildingObjectId = (int)eventParam[0];
            UserBuildingProduction userBuildingProduction = VariableManager.Instance.UserBuildingProductionList.GetBuildingProduction(buildingObjectId);
            StartFillProduction(userBuildingProduction).Forget();

        }

        public void OnRaiseCompleteFillResourceProduction(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            TaskData taskData = (TaskData)eventParam[0];
            _ = APIManager.Instance.RequestStartTaskData(taskData);
            
        }

        private async UniTaskVoid StartFillProduction(UserBuildingProduction userBuildingProduction)
        {
            await UniTask.DelayFrame(2);

            List<ResourcePoco> cost = VariableManager.Instance.ProductFormulaList.GetResourceCost(userBuildingProduction.CurrentProductId);

            float workload = VariableManager.Instance.ProductFormulaList.GetTimeCost(userBuildingProduction.CurrentProductId);
            string buildingName = MasterDataStore.Instance.GetBaseVassalTaskInfo(2).TaskName;

            var timeBegin = 0;
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(userBuildingProduction.buildingObjectId);
            var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            BuildingComponentGetter buildingGetter = objBuilding.GetComponent<BuildingComponentGetter>();
            var size = tilingTransform.GetBoxCollider().transform.localScale;
            Vector3 pos = tilingTransform.transform.localPosition;
            var position = Utils.ConvertVector3ToString(pos);
            var doorPosition = Utils.ConvertVector3ToString(buildingGetter.Operation.GetDoorPosition());

            var baseDataEnumGame = GameData.Instance.SavedPack.SaveData.ListDataEnums.Find(t => t.Enum == "PRODUCE");
            int priority = 1;
            if (baseDataEnumGame.PrePriority > 0)
                priority = baseDataEnumGame.PrePriority;
            userBuildingProduction.priority = priority;
            int idJob = MasterDataStore.Instance.BaseVassalTaskInfos.Find(t => t.TaskName == buildingName).Key;
            _ = APIManager.Instance.RequestAddTaskData(userBuildingProduction.buildingObjectId, 0, 0, size.x, buildingName, priority, 0, position, timeBegin,
       idJob, doorPosition, (int)StatusFillResource.None, workload, -1, userBuildingProduction.buildingId, 0, userBuildingProduction.CurrentProductId, cost,
       Utils.SetPositionWorkerBuilding(pos, size.x, 1.7f), Utils.SetPositionWorkerBuilding(pos, size.x, 1.0f));
    
        }

        public void OnResponseAddTaskDataProduction(TaskData taskData)
        {
            List<ResourcePoco> cost = taskData.CommonResources;
            UserBuildingProduction userBuildingProduction = VariableManager.Instance.UserBuildingProductionList.GetBuildingProduction(taskData.BuildingObjectId);
            userBuildingProduction.currentTaskId = taskData.TaskId;
            if (cost.Count == 0)
            {
                TaskManager.Instance.AddTask(taskData);
                object[] eventParam = new object[1];
                eventParam[0] = taskData;
                OnRaiseCompleteFillResourceProduction(eventParam);

            }
            else
            {
                CharacterManager.Instance.OnBeginFillResource(taskData, StatusCache.NoCache, taskData.TypeJobAction);
                TaskManager.Instance.AddTask(taskData);
            }

        }

        public void OnStartProduction(int buildingObjectId)
        {
            TaskData taskData =
                GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);

            int priority = taskData.Priority;
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(taskData.BuildingObjectId);
            UserBuildingProduction userBuildingProduction = VariableManager.Instance.UserBuildingProductionList.GetBuildingProduction(taskData.BuildingObjectId);
            CharacterManager.Instance.PorterComeBack(taskData);
            taskData.StatusFillResource = StatusFillResource.CompleteFill;
            GameData.Instance.AddJobWorkerAssignment(priority, taskData.TaskId, 0, taskData.Name, taskData.TypeJobAction);

            // check vassal manager production
            CharacterTaskData vassal = CharacterManager.Instance.GetCharacterTaskData().Find(t => t.BuildingObjectId == taskData.BuildingObjectId && t.Type == Character.CharacterList.Vassar);
            if (vassal != null)
            {
                object[] paramVassal = new object[2];
                paramVassal[0] = taskData.BuildingObjectId;
                paramVassal[1] = vassal.CharacterId;
                var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassal.CharacterId);
                if (vassalInfo != null) vassalInfo.IsWorking = true;
                GameManager.Instance.OnUpdateAvatarVassal(paramVassal);

            }

            WorkerStartProduction(priority, taskData, userBuilding, userBuildingProduction);
        }


        public void WorkerStartProductionCache(TaskData taskData)
        {
            if (taskData.Workload <= 0 && taskData.CurrentProductId != -1)
            {
                object[] eventParam = new object[1];
                eventParam[0] = taskData.BuildingObjectId;
                EventFinishAction.Instance.OnFinishItem(eventParam);
                return;
            }

            if (taskData.CommonResources.Count == 0)
                taskData.StatusFillResource = StatusFillResource.CompleteFill;

            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(taskData.BuildingObjectId);
            UserBuildingProduction userBuildingProduction = VariableManager.Instance.UserBuildingProductionList.GetBuildingProduction(taskData.BuildingObjectId);
            WorkerStartProduction(taskData.Priority, taskData, userBuilding, userBuildingProduction);
            taskData.Model = MasterDataStore.Instance.GetCountWorker(taskData.Man);
            taskData.SpeedRate = TimeWorkLoad.SpeedRate(taskData.Man);
            taskData.TimeBegin = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.TotalTime;
            taskData.TimeJob = TimeWorkLoad.TimeJob(taskData.Man, taskData);
            GameData.Instance.UpdateJobWorkerAssignment(taskData.Priority, taskData.TaskId, taskData.Man);

            int workloadsDone = JobManager.Instance.WorkloadsDoneProduction(taskData);
            userBuildingProduction.currentProgress = workloadsDone;
            userBuilding.status = BuildingStatus.Producing;
            userBuildingProduction.productRate = taskData.SpeedRate;
            GameManager.Instance.OnBuildingUpdateInfo(userBuilding);
            GameManager.Instance.OnBuildingProductionUpdateInfo(userBuildingProduction);

           GameManager.Instance.OnStartBuild(taskData.BuildingObjectId);
        }

        public void WorkerStartProduction(int priority, TaskData taskData, UserBuilding userBuilding, UserBuildingProduction userBuildingProduction)
        {
            if (GameData.Instance.SavedPack.SaveData.WorkerNumber > 0)
            {
                // check have worker 
                GameManager.Instance.GetWorker(priority, taskData, (count) =>
                {
                    taskData.Man = count;
                    taskData.Model = MasterDataStore.Instance.GetCountWorker(count);
                    taskData.SpeedRate = TimeWorkLoad.SpeedRate(taskData.Man);
                    taskData.TimeBegin = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.TotalTime;
                    taskData.TimeJob = TimeWorkLoad.TimeJob(count, taskData);
                    GameData.Instance.UpdateJobWorkerAssignment(priority, taskData.TaskId, taskData.Man);

                    int workloadsDone = JobManager.Instance.WorkloadsDoneProduction(taskData);
                    userBuildingProduction.currentProgress = workloadsDone;
                    userBuilding.status = BuildingStatus.Producing;
                    userBuildingProduction.productRate = taskData.SpeedRate;
                    GameManager.Instance.OnBuildingUpdateInfo(userBuilding);
                    GameManager.Instance.OnBuildingProductionUpdateInfo(userBuildingProduction);

                    GameManager.Instance.OnStartBuild(taskData.BuildingObjectId);

                    GameManager.Instance.OnUpdateProductionStatusBoard();
                });
            }
            else
            {
                UIManagerTown.Instance.ShowUIPopulation(() => {
                    if (GameData.Instance.SavedPack.SaveData.WorkerNumber > 0)
                        WorkerStartProduction(priority, taskData, userBuilding, userBuildingProduction);
                });
            }


        }
    }
}