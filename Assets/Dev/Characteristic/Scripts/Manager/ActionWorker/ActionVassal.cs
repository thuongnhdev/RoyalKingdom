using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Fbs;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CoreData.UniFlow.Common
{
    public class ActionVassal : MonoSingleton<ActionVassal>
    {
        public void OpenChooseVassalPopup(int buildingObjectId)
        {
            UIManagerTown.Instance.ShowUIVassarChoice(buildingObjectId);
        }

        public void OnUpdateAvatarVassal(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            VassalChoiceData reponse = (VassalChoiceData)eventParam[0];
            var data = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalPlayer == reponse.IdVassal);
       
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(reponse.BuildingPlayerId);
            if (reponse.IsChoose == 1)
                UpdateAvaterInBuilding(reponse.BuildingPlayerId, data.Data.idVassalTemplate);
            if(reponse.IsRemove == 1)
                UpdateAvaterInBuilding(reponse.BuildingPlayerId, 0);

            var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            var size = new Vector3(tilingTransform.GetBoxCollider().transform.localScale.x + 2,
                tilingTransform.GetBoxCollider().transform.localScale.y + 2,
                tilingTransform.GetBoxCollider().transform.localScale.z + 2);


            if (reponse.IsChoose == 1)
            {
                CharacterManager.Instance.Move(tilingTransform.transform.localPosition, size, reponse.BuildingPlayerId, data.Data.idVassalTemplate);
                MasterDataStore.Instance.ChangeVassalInTask(data.Data.idVassalTemplate, reponse.BuildingPlayerId);
            }

            if (reponse.IsRemove == 1)
            {
                CharacterManager.Instance.Move(tilingTransform.transform.localPosition, size, reponse.BuildingPlayerId, 0);
                MasterDataStore.Instance.ChangeVassalInTask(0, reponse.BuildingPlayerId);
            }
            GameManager.Instance.OnUpdateVassalStatusBoard(eventParam);
        }

        public void UpdateAvaterInBuilding(int buildingObjectId, int idVassal)
        {
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(buildingObjectId);
            userBuilding.vassalInCharge = idVassal;
            GameManager.Instance.OnBuildingUpdateInfo(userBuilding);
        }

        public void OnUpdatePositionResource(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }

            int locationId = (int)eventParam[0];
            int buildingId = (int)eventParam[1];
            Transform transform = (Transform)eventParam[2];
            if (buildingId == 2 || buildingId == 3)
            {
                List<ResourcePoco> constructionCost = VariableManager.Instance.BuildingUpgradeInfoList.GetUpgradeCost(buildingId, 1);
                VariableManager.Instance.ResourceSpawner.SpawnResource(constructionCost, locationId);
            }
        }

        public void OnStartBuild(int idBuildingObject)
        {
            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == idBuildingObject);
            CharacterManager.Instance.OnStartJob(taskData, StatusCache.NoCache);
        }

        public void OnCheckVassalAssignt(TaskData taskData)
        {
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(taskData.BuildingObjectId);
            VassalDataInGame player = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == userBuilding.vassalInCharge);
            if (player != null)
            {
                object[] eventParam = new object[2];
                eventParam[0] = taskData.BuildingObjectId;
                eventParam[1] = player.Data.idVassalTemplate;
                var objBuilding = VariableManager.Instance.BuildingObjectFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
                TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
                var size = new Vector3(tilingTransform.GetBoxCollider().transform.localScale.x + 2,
                    tilingTransform.GetBoxCollider().transform.localScale.y + 2,
                    tilingTransform.GetBoxCollider().transform.localScale.z + 2);
                CharacterManager.Instance.MoveBuildCache(tilingTransform.transform.localPosition, size, taskData.BuildingObjectId, player.Data.idVassalTemplate);

                MasterDataStore.Instance.ChangeVassalInTask(player.Data.idVassalTemplate, taskData.BuildingObjectId);
                GameManager.Instance.OnUpdateVassalStatusBoard(eventParam);
            }
        }

        public void GetWorker(int priority, TaskData taskData, Action<int> onComplete)
        {
            int totalWorker = 0;
            float totalPercent = 0;
            PopulationManager.Instance.TaskAssignmentTemps.Clear();
            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
            {
                totalPercent += GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent;
            }

            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
            {
                float updatePercent = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent * GameData.Instance.SavedPack.SaveData.WorkerNumber;
                int workerPriority = (int)(updatePercent / totalPercent);
                totalWorker += workerPriority;
            }
            PopulationManager.Instance.CheckWorkerAssign(totalWorker);

            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
            {
                float percent = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Percent * GameData.Instance.SavedPack.SaveData.WorkerNumber;
                int workerPriority = (int)(percent / totalPercent);
                totalWorker += workerPriority;
                WorkerAssignment countJobPriority = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i];
                if (priority == GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i].Priority && countJobPriority.DataAssignments.Count > 0)
                {
                    int worker = (int)(workerPriority / countJobPriority.DataAssignments.Count);
                    RedistributingWorkers(priority, worker, taskData, onComplete);
                }
            }
        }

        private void RedistributingWorkers(int priority, int worker, TaskData taskData, Action<int> OnComplete)
        {
            int workerTask = worker;
            Action<int> onComplete = OnComplete;
            for (var i = 0; i < GameData.Instance.SavedPack.SaveData.ListTask.Count; i++)
            {
                if (GameData.Instance.SavedPack.SaveData.ListTask[i].Priority == priority &&
                    GameData.Instance.SavedPack.SaveData.ListTask[i].StatusFillResource == StatusFillResource.CompleteFill)
                {
                    worker = workerTask;
                    var item = PopulationManager.Instance.TaskAssignmentTemps.Find(t => t.TaskData.TaskId == GameData.Instance.SavedPack.SaveData.ListTask[i].TaskId);
                    if (item != null)
                    {
                        worker = worker + item.Worker;
                    }
                    int man = worker - GameData.Instance.SavedPack.SaveData.ListTask[i].Man;
                    if (man < 0 && GameData.Instance.SavedPack.SaveData.ListTask[i].Man > 0)
                    {
                        GameManager.Instance.UpdateWorking(GameData.Instance.SavedPack.SaveData.ListTask[i], man, worker, null);
                        GameData.Instance.SavedPack.SaveData.ListTask[i].Man = worker;
                    }
                    else if (taskData.BuildingObjectId == GameData.Instance.SavedPack.SaveData.ListTask[i].BuildingObjectId && man >= 0)
                        onComplete?.Invoke(worker);
                }
            }
            GameData.Instance.RequestSaveGame();
        }
    }


    [System.Serializable]
    public class VassalChoiceData
    {
        public int BuildingPlayerId;
        public int IdVassal;
        public int IsChoose;
        public int IsRemove;
        public float WorkloadStart;
        public int IdTask;

        public VassalChoiceData(int buildingPlayerId = 0,
            int idVassal = 0,
            int isChoose = 0,
            int isRemove = 0,
            float workloadStart = 0.0f,
            int idTask = 0)
        {
            BuildingPlayerId = buildingPlayerId;
            IdVassal = idVassal;
            IsChoose = isChoose;
            IsRemove = isRemove;
            WorkloadStart = workloadStart;
            IdTask = idTask;
        }
    }
}
