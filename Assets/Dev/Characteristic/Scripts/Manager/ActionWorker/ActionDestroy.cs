using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class ActionDestroy : MonoSingleton<ActionDestroy>
    {
        public void OnDestroyedBuilding(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            int buildingObjectId = (int)eventParam[0];
            OnCanceledUpgrade(eventParam);
        }

        public void OnStartDestroyedProgress(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            var buildingObjectId = (int)eventParam[0];
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(buildingObjectId);
            InitDestroyTask(userBuilding.buildingId, userBuilding.locationTileId, userBuilding.buildingLevel, TypeJobAction.DESTROY).Forget();
        }

        public void OnCanceledCurrentProduct(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            int buildingObjectId = (int)eventParam[0];
            OnCanceledUpgrade(eventParam);
        }

        public void OnCanceledUpgrade(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            int buildingObjectId = (int)eventParam[0];
            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData == null) return;
            CharacterManager.Instance.PorterComeBack(taskData);

            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(buildingObjectId);
            var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            var posTiling = tilingTransform.transform.localPosition;
            var sizeTiling = tilingTransform.GetBoxCollider().transform.localScale;
            List<ResourcePoco> _commonResources = userBuilding.refundMaterial;
            if (taskData != null)
            {
                if (taskData.StatusFillResource == StatusFillResource.BeginFill)
                    CharacterManager.Instance.PorterComeBack(taskData);
            }
            CancelUpgrade.Instance.OnCollectResourceCanceledUpgrade(sizeTiling, posTiling, _commonResources, userBuilding, buildingObjectId);

        }

        public void OnBuidingChangedToDestructed(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            int buildingObjectId = (int)eventParam[0];
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(buildingObjectId);
            var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            var posTiling = tilingTransform.transform.localPosition;
            var sizeTiling = tilingTransform.GetBoxCollider().transform.localScale;
            List<ResourcePoco> _commonResources = userBuilding.refundMaterial;
            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData != null)
            {
                if (taskData.StatusFillResource == StatusFillResource.BeginFill)
                    CharacterManager.Instance.PorterComeBack(taskData);
            }
            EventFinishAction.Instance.OnFinishDestroy(eventParam);
            CollectResource.Instance.OnStartCollectResource(sizeTiling, posTiling, _commonResources, userBuilding, buildingObjectId);
        }

        public void OnChangedToWaitForUpgradeResource(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            var buildingObjectId = (int)eventParam[0];
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(buildingObjectId);
            InitBuildingTask(userBuilding.buildingId, userBuilding.locationTileId, userBuilding.buildingLevel + 1, TypeJobAction.UPGRADE).Forget();

        }


        private async UniTaskVoid InitBuildingTask(int buildingId, int locationId, int level, TypeJobAction typeJobAction)
        {
            int currentBuildingCount = VariableManager.Instance.BuildingObjectFinder.BuildingCount;
            if (typeJobAction == TypeJobAction.CONTRUCTION) await UniTask.WaitUntil(() => currentBuildingCount != VariableManager.Instance.BuildingObjectFinder.BuildingCount); // Wait for data response from server
            await UniTask.DelayFrame(2); // first frame for instanciating building object, second frame for this object Operation data is filled

            var buildingInfo = VariableManager.Instance.TownBaseBuildingSOList.GetBaseBuilding(buildingId);
            var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(locationId);
            if (objBuilding == null) return;
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            BuildingComponentGetter buildingGetter = objBuilding.GetComponent<BuildingComponentGetter>();
            var size = tilingTransform.GetBoxCollider().transform.localScale;
            List<ResourcePoco> cost = VariableManager.Instance.BuildingUpgradeInfoList.GetUpgradeCost(buildingId, level);

            float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(buildingId, level);
            string buildingName = MasterDataStore.Instance.GetBaseVassalTaskInfo(1).TaskName;
            int buildingObjId = buildingGetter.Operation.BuildingObjId;
            GameManager.Instance.SetBuildingObjId(buildingObjId);
            if (typeJobAction == TypeJobAction.UPGRADE)
                buildingName = NameWorkload.Upgrade;
            var timeBegin = 0;
            var pos = tilingTransform.transform.localPosition;
            var position = Utils.ConvertVector3ToString(pos);
            var doorPosition = Utils.ConvertVector3ToString(buildingGetter.Operation.GetDoorPosition());
    
            var baseDataEnumGame = GameData.Instance.SavedPack.SaveData.ListDataEnums.Find(t => t.Enum == "BUILDING");
            int priority = 1;
            if (baseDataEnumGame.PrePriority > 0)
                priority = baseDataEnumGame.PrePriority;
            int idJob = MasterDataStore.Instance.BaseVassalTaskInfoSubs.Find(t => t.TaskName == "CancelUpgrade").Key;
            _ = APIManager.Instance.RequestAddTaskData(buildingObjId, 0, 0, size.x, buildingName, priority, 0, position,
                timeBegin,
                idJob, doorPosition, (int)StatusFillResource.None, workload, -1, buildingId, 0, 0, cost,
                Utils.SetPositionWorkerBuilding(pos, size.x, 1.7f), Utils.SetPositionWorkerBuilding(pos, size.x, 1.0f));
            
        }

        public void OnResponseAddTaskDataChangedToWaitForUpgrade(TaskData taskData)
        {
            CharacterManager.Instance.OnBeginFillResource(taskData, StatusCache.NoCache, taskData.TypeJobAction);
            TaskManager.Instance.AddTask(taskData);
        }
        private async UniTaskVoid InitDestroyTask(int buildingId, int locationId, int level, TypeJobAction typeJobAction)
        {
            await UniTask.DelayFrame(2); // first frame for instanciating building object, second frame for this object Operation data is filled
            var buildingInfo = VariableManager.Instance.TownBaseBuildingSOList.GetBaseBuilding(buildingId);
            var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(locationId);
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            BuildingComponentGetter buildingGetter = objBuilding.GetComponent<BuildingComponentGetter>();
            var size = tilingTransform.GetBoxCollider().transform.localScale;
            List<ResourcePoco> cost = VariableManager.Instance.BuildingUpgradeInfoList.GetUpgradeCost(buildingId, level);

            float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(buildingId, level);
            string buildingName = MasterDataStore.Instance.GetBaseVassalTaskInfo(1).TaskName;
            var buildingObjId = buildingGetter.Operation.BuildingObjId;
            GameManager.Instance.SetBuildingObjId(buildingObjId);
            if (typeJobAction == TypeJobAction.UPGRADE)
                buildingName = NameWorkload.Upgrade;
            var timeBegin = 0;
            var pos = tilingTransform.transform.localPosition;
            var position = Utils.ConvertVector3ToString(pos);
            var doorPosition = Utils.ConvertVector3ToString(buildingGetter.Operation.GetDoorPosition());

            var baseDataEnumGame = GameData.Instance.SavedPack.SaveData.ListDataEnums.Find(t => t.Enum == "BUILDING");
            int priority = 1;
            if (baseDataEnumGame.PrePriority > 0)
                priority = baseDataEnumGame.PrePriority;
            int idJob = MasterDataStore.Instance.BaseVassalTaskInfoSubs.Find(t => t.TaskName == "Destroy").Key;
            _ = APIManager.Instance.RequestAddTaskData(buildingObjId, 0, 0, size.x, buildingName, priority, 0, position,
                timeBegin,
                idJob, doorPosition, (int)StatusFillResource.None, workload, -1, buildingId, 0, 0, cost,
                Utils.SetPositionWorkerBuilding(pos, size.x, 1.7f), Utils.SetPositionWorkerBuilding(pos, size.x, 1.0f));
      
        }

        public void OnResponseAddTaskDataDestroy(TaskData taskData)
        {
            TaskManager.Instance.AddTask(taskData);

            object[] eventParam = new object[1];
            eventParam[0] = taskData;
            OnRaiseCompleteFillResource(eventParam);
        }

        public void OnRaiseCompleteFillResource(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            TaskData taskData = (TaskData)eventParam[0];
            int priority = taskData.Priority;
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(taskData.BuildingObjectId);
            CharacterManager.Instance.PorterComeBack(taskData);
            taskData.StatusFillResource = StatusFillResource.CompleteFill;
            GameData.Instance.AddJobWorkerAssignment(priority, taskData.TaskId, 0, taskData.Name, taskData.TypeJobAction);
            // check have worker 
            if (GameData.Instance.SavedPack.SaveData.WorkerNumber > 0)
            {
                WorkerStartBuilding(priority, taskData, userBuilding);
            }
            else
            {
                UIManagerTown.Instance.ShowUIPopulation(() => {
                    if (GameData.Instance.SavedPack.SaveData.WorkerNumber > 0)
                        WorkerStartBuilding(priority, taskData, userBuilding);
                });
            }
            GameData.Instance.RequestSaveGame();
        }

        private void WorkerStartBuilding(int priority, TaskData task, UserBuilding userBuilding)
        {
            // check have worker 
            GameManager.Instance.GetWorker(priority, task, (count) =>
            {
                var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.TaskId == task.TaskId);
                taskData.Man = count;
                taskData.Model = MasterDataStore.Instance.GetCountWorker(count);
                taskData.SpeedRate = TimeWorkLoad.SpeedRate(taskData.Man);
                taskData.TimeBegin = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.TotalTime;
                taskData.TimeJob = TimeWorkLoad.TimeJob(count, taskData);
                GameData.Instance.UpdateJobWorkerAssignment(taskData.Priority, taskData.TaskId, taskData.Man);
                if (taskData.TypeJobAction == TypeJobAction.CONTRUCTION)
                    userBuilding.status = BuildingStatus.OnConstruction;
                else if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
                    userBuilding.status = BuildingStatus.Upgrading;

                int workloadsDone = JobManager.Instance.WorkloadsDone(taskData);
                if (taskData.TypeJobAction == TypeJobAction.DESTROY)
                {
                    userBuilding.currentDestructionRate = taskData.SpeedRate;
                    userBuilding.status = BuildingStatus.OnDestruction;
                    userBuilding.destructedTime = workloadsDone;
                }
                else
                {
                    userBuilding.constructedTime = workloadsDone;
                    userBuilding.currentContructionRate = taskData.SpeedRate;
                }

                GameManager.Instance.OnBuildingUpdateInfo(userBuilding);

                _ = APIManager.Instance.RequestStartTaskData(taskData);
                GameManager.Instance.OnStartBuild(taskData.BuildingObjectId);

            });
        }
    }
}
