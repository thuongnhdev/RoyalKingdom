using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fbs;
using Vector3 = UnityEngine.Vector3;

namespace CoreData.UniFlow.Common
{
    public class FinishBuildding : MonoSingleton<FinishBuildding>
    {
        public void OnFinishBuildding(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            UIManagerTown.Instance.UIVassalChoice.Close();
            ReponseFinishBuilding response = (ReponseFinishBuilding)eventParam[0];
            var buildingObjectId = response.BuildingPlayerId;
            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData == null) return;
            GameData.Instance.RemoveJobWorkerAssignment(taskData.TaskId);
            GameData.Instance.SavedPack.SaveData.WorkloadWorkerHouse--;
            GameData.Instance.SavedPack.SaveData.WorkersWorking -= taskData.Man;

            var characterTaskData = CharacterManager.Instance.GetCharacterTaskData();
            // check user work build house with id
            foreach (var player in characterTaskData)
            {
                if (player.BuildingObjectId == buildingObjectId)
                {
                    if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                    {
                        UserBuildingProduction userBuildingProduction = CharacterManager.Instance.UserBuildingProductionList.GetBuildingProduction(buildingObjectId);
                        if (!userBuildingProduction.queueRepeat || userBuildingProduction == null)
                            CharacterFinishJob(player, taskData, buildingObjectId);
                        else
                        {
                            var vassalData = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == player.CharacterId);
                            // calculator trigger done workload 
                            JobManager.Instance.TriggerDoneWorkload(taskData, vassalData);
                        }

                    }
                    else
                    {
                        CharacterFinishJob(player, taskData, buildingObjectId);
                    }
                }
            }

            if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                FinishItemProduction.Instance.OnFinishItemProduction(buildingObjectId, TypeJobAction.ITEM);
            TaskManager.Instance.RemoveTask(buildingObjectId);
            GameData.Instance.RequestSaveGame();
            PopulationManager.Instance.WorkerDivision();
            // Check task || if have work task continue || if no go to house
            TaskManager.Instance.NextTask(CharacterManager.Instance.UserBuildingList, CharacterManager.Instance.UserItemStorage);

            CharacterManager.Instance.OnUpdateManagerTask();
        }

        public async void CharacterFinishJob(CharacterTaskData player, TaskData taskData, int buildingObjectId)
        {
            var vassalData = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == player.CharacterId);

            if (player.Type == Character.CharacterList.Vassar)
            {
                if (taskData.TypeJobAction != TypeJobAction.PRODUCE)
                {
                    player.CharacterBehaviour.Reset(true);
                    player.BuildingObjectId = -1;
                    player.StatusAction = StatusAction.None;
                    if (vassalData != null) vassalData.IsWorking = false;
                    if (!player.CharacterBehaviour.gameObject.activeInHierarchy)
                        player.CharacterBehaviour.gameObject.SetActive(true);
                    player.CharacterBehaviour.Action(StatusAction.Walk);
                    player.CharacterBehaviour.SetAIStop(false);
                    player.StatusAction = StatusAction.None;
                    player.CharacterBehaviour.IsAutoMove = true;
                    player.CharacterBehaviour.MoveAutoFreedom();
                    GameManager.Instance.UpdateAvaterInBuilding(buildingObjectId, 0);
                    // calculator trigger done workload 
                    JobManager.Instance.TriggerDoneWorkload(taskData, vassalData);
                }
                else
                {
                    JobManager.Instance.TriggerDoneWorkload(taskData, vassalData);
                    taskData.Workload = 0;
                    var index = GameData.Instance.SavedPack.SaveData.VassalInfos.FindIndex(t => t.Data.idVassalTemplate == player.CharacterId);
                    GameData.Instance.SavedPack.SaveData.VassalInfos[index].WorkloadStart = 0;
                }


            }
            else if (player.Type == Character.CharacterList.Farmer)
            {
                player.CharacterBehaviour.Reset(true);
                player.BuildingObjectId = -1;
                player.StatusAction = StatusAction.None;
                if (vassalData != null) vassalData.IsWorking = false;

                if (!player.CharacterBehaviour.gameObject.activeInHierarchy)
                    player.CharacterBehaviour.gameObject.SetActive(true);
                await Task.Delay(TimeSpan.FromMilliseconds(MasterDataStore.Instance.GetWorkerComeBackHouseTimeOut()));

                player.CharacterBehaviour.Action(StatusAction.Finish);
                await Task.Delay(TimeSpan.FromMilliseconds(MasterDataStore.Instance.GetWorkerComeBackHouseTimeOut()));
                player.CharacterBehaviour.Action(StatusAction.Walk);
                var posHouse = Utils.GetWorkerHouse();
                var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperation.GetDoorPosition());
                player.CharacterBehaviour.MoveCenter(Vector3.zero, 0.1f, posOutDoorHouse, (dataStore, rotation) =>
                {
                    buildingOperation.OpenDoor(0);
                    player.CharacterBehaviour.Action(StatusAction.Walk);
                    Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperation.GetDoorPosition());
                    // Calculate a rotation a step closer to the target and applies rotation to this object
                    player.CharacterBehaviour.transform.rotation = Utils.GetRotation(posInDoorHouse, player);

                    player.CharacterBehaviour.MoveOutDoor(Vector3.zero, 0, posInDoorHouse, (dataStore, rotation) =>
                    {
                        buildingOperation.CloseDoor(0);
                        player.CharacterBehaviour.IsMove = false;
                        player.CharacterBehaviour.Action(StatusAction.Idle);
                        CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(player);
                    });
                });
            }
            else if (player.Type == Character.CharacterList.AssistantWorker)
            {
                player.CharacterBehaviour.Reset(true);
                player.BuildingObjectId = -1;
                player.StatusAction = StatusAction.None;
                if (vassalData != null) vassalData.IsWorking = false;
                if (!player.CharacterBehaviour.gameObject.activeInHierarchy)
                    player.CharacterBehaviour.gameObject.SetActive(true);
                await Task.Delay(TimeSpan.FromMilliseconds(MasterDataStore.Instance.GetWorkerComeBackHouseTimeOut()));

                StatusAction statusAction = StatusAction.Walk;
                statusAction = StatusAction.CarriBox;
                player.CharacterBehaviour.Action(statusAction);
                var workerHouse = Utils.GetWorkerHouse();
                Vector3 posOutDoorHouse = Utils.ConvertPositionDoor(workerHouse.Position, RotationDoors.Left, workerHouse.Size.x);
                player.CharacterBehaviour.MoveComback(Vector3.zero, 5.0f, posOutDoorHouse, async (dataStore, rotation) =>
                {
                    player.CharacterBehaviour.IsMove = false;
                    player.CharacterBehaviour.Action(StatusAction.Idle);
                    CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(player);
                });
            }
        }
    }
}
