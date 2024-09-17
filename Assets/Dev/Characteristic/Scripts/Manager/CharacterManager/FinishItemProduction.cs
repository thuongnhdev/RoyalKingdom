using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class FinishItemProduction : MonoSingleton<FinishItemProduction>
    {
        public void OnFinishItemProduction(int buildingObjectId, TypeJobAction type)
        {
            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            UserBuilding userBuilding = CharacterManager.Instance.UserBuildingList.GetBuilding(buildingObjectId);
            List<ResourcePoco> cost = VariableManager.Instance.BuildingUpgradeInfoList.GetUpgradeCost(taskData.BuildingId, userBuilding.buildingLevel + 1);
            taskData.CommonResources = cost;
            int porter = 3;
            for (int i = 0; i < porter; i++)
            {
                PortHarvestResource(i, taskData, StatusCache.NoCache, type);
            }
        }

        public async void PortHarvestResource(int index, TaskData taskData, StatusCache statusCache, TypeJobAction typeJobAction)
        {
            float timeDelay = MasterDataStore.Instance.GetPopulationWorkerTimeComeOut() * index;
            await Task.Delay(TimeSpan.FromMilliseconds(timeDelay));
            CharacterTaskData farmer = CharacterManager.Instance.GetAssistantWorker(Character.CharacterList.AssistantWorker);
            farmer.CharacterBehaviour.gameObject.SetActive(false);
            farmer.CharacterBehaviour.Reset(false);
            farmer.CharacterBehaviour.Action(StatusAction.CarriBox);
            farmer.BuildingObjectId = taskData.BuildingObjectId;
         
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(taskData.BuildingObjectId);
            var buildingOperation = Utils.GetBuildingOperation(userBuilding.locationTileId, VariableManager.Instance.BuildingObjectFinder);

            var posWorker = Utils.GetWorkerHouse();
            var buildingOperationWorker = Utils.GetBuildingOperation(posWorker.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
            Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperationWorker.GetDoorPosition());
            Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperationWorker.GetDoorPosition());

            farmer.CharacterBehaviour.transform.localPosition = posWorker.Position;
            farmer.CharacterBehaviour.gameObject.SetActive(true);
            buildingOperation.OpenDoor(0);
       
 
            farmer.CharacterBehaviour.InitMissionProduction(taskData.BuildingObjectId, taskData.CommonResources, VariableManager.Instance.ResourceFinder, taskData.SpeedRate);
            farmer.CharacterBehaviour.MoveCenter(Vector3.zero, 1.5f, posOutDoorHouse, (dataStore, rotation) =>
            {
                buildingOperation.CloseDoor(0);
                farmer.CharacterBehaviour.StartMaterial(index, taskData, statusCache, typeJobAction);
            });
        }
    }
}
