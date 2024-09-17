using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class PorterBringResourceProduction : MonoBehaviour
    {
        public void CountResourceProduction(TaskData taskData, StatusCache statusCache)
        {
            CharacterBehaviour _characterBehaviour = this.GetComponent<CharacterBehaviour>();
            int countCheckFullmaterial = 1;
            bool isMissMaterial = false;
            for (int i = 0; i < _characterBehaviour.GetCommonResources().Count; i++)
            {
                var itemInStore = _characterBehaviour.UserItemStorage.GetItemCount(_characterBehaviour.GetCommonResources()[i].itemId);
                if (itemInStore == 0)
                {
                    isMissMaterial = true;
                    countCheckFullmaterial++;
                }
            }
            if (!ItemHelper.IsGreaterOrEquals(_characterBehaviour.UserBuildingProduction.currentMaterials, _characterBehaviour.GetCommonResources()))
            {
                ResourcePocoTask resourcePocoTask = Utils.GetItemIncreaseType(_characterBehaviour.UserBuildingProduction.currentMaterials, _characterBehaviour.GetCommonResources());
                if (resourcePocoTask == null) return;
                BuildingMain storeHouse = Utils.GetStoreHouse(resourcePocoTask.ItemType);

                if (storeHouse == null)
                {
                    _characterBehaviour.OnResourceNoEnough(taskData);
                    return;
                }

                Vector2 buildingSize = _characterBehaviour.TownBaseBuildingSOList.GetBuildingSize(taskData.BuildingId);
                var buildingOperation = Utils.GetBuildingOperation(storeHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                Vector3 posDoorStoreHouse = buildingOperation.GetDoorPosition();
                posDoorStoreHouse = new Vector3(posDoorStoreHouse.x, 0, posDoorStoreHouse.z);
                Vector3 posOutDoor = Utils.GetOutDoorPosition(posDoorStoreHouse);
                _characterBehaviour.MoveCenter(Vector3.zero, 1.0f, posOutDoor, async (dataStore, rotation) =>
                {
                    buildingOperation.OpenDoor(0);
                    dataStore.CharacterBehaviour.Action(StatusAction.Idle02);
                    await UniTask.DelayFrame(250);
                    dataStore.CharacterBehaviour.Action(StatusAction.Idle01);
                    await UniTask.DelayFrame(250);
                    buildingOperation.CloseDoor(0);
                    dataStore.CharacterBehaviour.Action(StatusAction.CarriBoxHigh);
                    if (_characterBehaviour.ObjMaterialCarribox != null) _characterBehaviour.ObjMaterialCarribox.SetActive(true);
                    var posHouse = Utils.ConvertStringToVector3(taskData.RotationDoor);
                    var posOutDoor = Utils.GetOutDoorPosition(posHouse);
                    _characterBehaviour.MoveCenter(Vector3.zero, 0.75f, posOutDoor, async (dataStore, rotation) =>
                    {
                        var buildingComponentGetter = Utils.GetBuildingComponentGetter(taskData.BuildingObjectId, _characterBehaviour.UserBuildingList, VariableManager.Instance.BuildingObjectFinder);
                        buildingComponentGetter.Operation.OpenDoor(0);
                        await UniTask.DelayFrame(100);
                        buildingComponentGetter.Operation.CloseDoor(0);
                        if (_characterBehaviour.ObjMaterialCarribox != null) _characterBehaviour.ObjMaterialCarribox.SetActive(false);
                        dataStore.CharacterBehaviour.Action(StatusAction.CarriBox);
                        if (dataStore.CharacterBehaviour.CharacterTaskData.Type == Character.CharacterList.AssistantWorker)
                        {
                            bool isFinish = false;
                            ResourcePocoTask resourcePocoTask = Utils.GetItemIncreaseType(_characterBehaviour.UserBuildingProduction.currentMaterials, _characterBehaviour.GetCommonResources());
                            if (resourcePocoTask == null) return;
                            var item = _characterBehaviour.UserBuildingProduction.currentMaterials.Find(t => t.itemId == resourcePocoTask.ResourcePoco.itemId);
                            var indext = _characterBehaviour.UserBuildingProduction.currentMaterials.FindIndex(t => t.itemId == resourcePocoTask.ResourcePoco.itemId);
                            var itemInStore = _characterBehaviour.UserItemStorage.GetItemCount(resourcePocoTask.ResourcePoco.itemId);
                            if (itemInStore > 0)
                            {
                                if (ResourcePoco.IsZero(item))
                                {
                                    ResourcePoco newRes = new ResourcePoco();
                                    newRes.itemId = resourcePocoTask.ResourcePoco.itemId;
                                    newRes.itemCount = 1;
                                    _characterBehaviour.UserBuildingProduction.currentMaterials.Add(newRes);
                                    // sub item in inventory
                                    PopulationManager.Instance.SubItemResource(resourcePocoTask.ResourcePoco.itemId);
                                    _ = APIManager.Instance.RequestFillResource(_characterBehaviour.UserBuildingProduction.buildingObjectId, newRes.itemId, newRes.itemCount);
                                    if (resourcePocoTask.IndexResourcePoco == (_characterBehaviour.GetCommonResources().Count - countCheckFullmaterial) && newRes.itemCount == (resourcePocoTask.ResourcePoco.itemCount - 1)) isFinish = true;
                                }
                                else if (item.itemCount < resourcePocoTask.ResourcePoco.itemCount)
                                {
                                    // sub item in inventory
                                    PopulationManager.Instance.SubItemResource(resourcePocoTask.ResourcePoco.itemId);
                                    item.itemCount++;
                                    _characterBehaviour.UserBuildingProduction.currentMaterials[indext] = item;
                                    _ = APIManager.Instance.RequestFillResource(_characterBehaviour.UserBuildingProduction.buildingObjectId, resourcePocoTask.ResourcePoco.itemId, resourcePocoTask.ResourcePoco.itemCount);
                                    if (resourcePocoTask.IndexResourcePoco == (_characterBehaviour.GetCommonResources().Count - countCheckFullmaterial) && item.itemCount >= (resourcePocoTask.ResourcePoco.itemCount - 2))
                                        isFinish = true;
                                    if (resourcePocoTask.IndexResourcePoco == (_characterBehaviour.GetCommonResources().Count - countCheckFullmaterial) && item.itemCount >= resourcePocoTask.ResourcePoco.itemCount)
                                        _characterBehaviour.OnRaiseCompleteFillResourceProduction(taskData);
                                }
                            }
                            else
                            {
                                countCheckFullmaterial--;
                                isFinish = _characterBehaviour.IsCheckEmptyResource(_characterBehaviour.GetCommonResources());
                            }
                            _characterBehaviour.OnBuildingProductionUpdateInfo();
                            if (isFinish)
                            {
                                var posHouse = Utils.GetWorkerHouse();
                                var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                                Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperation.GetDoorPosition());
                                Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperation.GetDoorPosition());
                                Vector2 buildingSize = _characterBehaviour.TownBaseBuildingSOList.GetBuildingSize(taskData.BuildingId);
                                _characterBehaviour.MoveCenter(Vector3.zero, ShareUIManager.Instance.Config.DURATION_DOOR_WORKERHOUSE_PORTER, posOutDoorHouse, async (dataStore, rotation) =>
                                {
                                    buildingOperation.OpenDoor(0);
                                    _characterBehaviour.IsMove = false;
                                    _characterBehaviour.Action(StatusAction.Idle);
                                    CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(_characterBehaviour.CharacterTaskData);
                                    _characterBehaviour.CharacterTaskData.BuildingObjectId = -1;
                                    await UniTask.DelayFrame(10);
                                    buildingOperation.CloseDoor(0);
                                });
                            }
                            else
                                CountResourceProduction(taskData, StatusCache.NoCache);
                        }
                    });
                });
            }
            else
            {
                if (statusCache == StatusCache.Cache)
                {
                    Vector3 posHouse = Utils.ConvertStringToVector3(taskData.Position);
                    Vector3 posDoorHouse = Utils.GetOutDoorPosition(posHouse);
                    _characterBehaviour.Move(new Vector3(taskData.Size, 1, taskData.Size), posDoorHouse, (dataStore) =>
                    {
                        _characterBehaviour.Move(new Vector3(taskData.Size, 1, taskData.Size), posHouse, (dataStore) =>
                        {
                            if (Utils.IsCheckWorkerSameBuilding(taskData.BuildingObjectId, Utils.RemainingResourcesProduction(_characterBehaviour.GetCommonResources(),_characterBehaviour.UserBuildingProduction)))
                            {
                                var posHouse = Utils.GetWorkerHouse();
                                Vector3 posDoorHouse = Utils.GetOutDoorPosition(posHouse.Position);
                                Vector2 buildingSize = _characterBehaviour.TownBaseBuildingSOList.GetBuildingSize(taskData.BuildingId);
                                _characterBehaviour.MoveCenter(Vector3.zero, ShareUIManager.Instance.Config.DURATION_DOOR_WORKERHOUSE_PORTER, posHouse.Position, (dataStore, rotation) =>
                                {
                                    _characterBehaviour.IsMove = false;
                                    _characterBehaviour.Action(StatusAction.Idle);
                                    CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(_characterBehaviour.CharacterTaskData);
                                });
                            }
                            else
                                _characterBehaviour.OnRaiseCompleteFillResourceProduction(taskData);
                        });
                    });
                }
                else if (!isMissMaterial)
                    _characterBehaviour.OnRaiseCompleteFillResourceProduction(taskData);

            }
        }

    }
}
