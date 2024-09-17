using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class BringFinishResourceItem : MonoBehaviour
    {
        public void FinishResourceItem(int index,TaskData taskData, StatusCache statusCache)
        {
            CharacterBehaviour _characterBehaviour = this.GetComponent<CharacterBehaviour>();
            Vector2 buildingSize = _characterBehaviour.TownBaseBuildingSOList.GetBuildingSize(taskData.BuildingId);
            var posHouse = Utils.ConvertStringToVector3(taskData.RotationDoor);
            var posOutDoor = Utils.GetOutDoorPosition(posHouse);

            UserBuildingProduction userBuildingProduction = _characterBehaviour.UserBuildingProductionList.GetBuildingProduction(taskData.BuildingObjectId);
            if (!ItemHelper.IsGreaterOrEqualsFinish(_characterBehaviour.resourcePocosFinishCurrent))
            {
                _characterBehaviour.MoveCenter(Vector3.zero, 1.5f, posOutDoor, async (dataStore, rotation) =>
                {
                    var buildingComponentGetter = Utils.GetBuildingComponentGetter(taskData.BuildingObjectId, _characterBehaviour.UserBuildingList, VariableManager.Instance.BuildingObjectFinder);
                    buildingComponentGetter.Operation.OpenDoor(0);
                    dataStore.CharacterBehaviour.Action(StatusAction.Idle02);
                    await UniTask.DelayFrame(250);
                    dataStore.CharacterBehaviour.Action(StatusAction.Idle01);
                    await UniTask.DelayFrame(250);
                    buildingComponentGetter.Operation.CloseDoor(0);
                    dataStore.CharacterBehaviour.Action(StatusAction.CarriBoxHigh);

                    ResourcePocoTask resourcePocoTask = _characterBehaviour.GetItemFinishType(taskData.CommonResources);
                    if (resourcePocoTask == null) return;
                    BuildingMain storeHouse = Utils.GetStoreHouse(resourcePocoTask.ItemType);
                    var buildingOperationStore = Utils.GetBuildingOperation(storeHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                    Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperationStore.GetDoorPosition());
                    Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperationStore.GetDoorPosition());
                    Vector2 buildingSize = _characterBehaviour.TownBaseBuildingSOList.GetBuildingSize(taskData.BuildingId);
                    if (_characterBehaviour.ObjMaterialCarribox != null) _characterBehaviour.ObjMaterialCarribox.SetActive(true);
                    _characterBehaviour.MoveCenter(Vector3.zero, 1.5f, posOutDoorHouse, async (dataStore, rotation) =>
                    {
                        buildingOperationStore.OpenDoor(0);
                        dataStore.CharacterBehaviour.Action(StatusAction.Idle02);
                        await UniTask.DelayFrame(250);
                        dataStore.CharacterBehaviour.Action(StatusAction.Idle01);
                        await UniTask.DelayFrame(250);
                        buildingOperationStore.CloseDoor(0);
                        dataStore.CharacterBehaviour.Action(StatusAction.CarriBox);
                        if (_characterBehaviour.ObjMaterialCarribox != null) _characterBehaviour.ObjMaterialCarribox.SetActive(false);

                        var posWorker = Utils.GetWorkerHouse();
                        var buildingOperationWorker = Utils.GetBuildingOperation(posWorker.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                        Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperationWorker.GetDoorPosition());
                        Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperationWorker.GetDoorPosition());
                        _characterBehaviour.MoveCenter(Vector3.zero, 1.5f, posOutDoorHouse, async (dataStore, rotation) =>
                        {
                            buildingOperationWorker.OpenDoor(0);
                            _characterBehaviour.IsMove = false;
                            _characterBehaviour.Action(StatusAction.Idle);
                            CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(_characterBehaviour.CharacterTaskData);
                            _characterBehaviour.CharacterTaskData.BuildingObjectId = -1;
                            await UniTask.DelayFrame(10);
                            buildingOperationWorker.CloseDoor(0);
                            if(index == 2)
                            {
                                TaskManager.Instance.RemoveTask(taskData.BuildingObjectId);
                                GameData.Instance.RequestSaveGame();
                                PopulationManager.Instance.WorkerDivision();
                                // Check task || if have work task continue || if no go to house
                                TaskManager.Instance.NextTask(CharacterManager.Instance.UserBuildingList, CharacterManager.Instance.UserItemStorage);

                                CharacterManager.Instance.OnUpdateManagerTask();
                            }
                        });
                    });
                });
            }
            

            var completedProduction = userBuildingProduction.completedProducts;
            for (int i = 0; i < completedProduction.Count; i++)
            {
                PopulationManager.Instance.AddItemResource(completedProduction[i].productId, completedProduction[i].randomOptions);
                userBuildingProduction.completedProducts.RemoveAt(i);
            }
            
        }
    }
}