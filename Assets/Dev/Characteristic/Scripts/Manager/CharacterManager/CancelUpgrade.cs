using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class CancelUpgrade : MonoSingleton<CancelUpgrade>
    {
        public void OnCollectResourceCanceledUpgrade(Vector3 sizetilingTransform, Vector3 posTilingTransform, List<ResourcePoco> _commonResources, UserBuilding userBuilding, int buildingObjectId)
        {
            int porter = 3;
            for (int i = 0; i < porter; i++)
            {
                if (i < Utils.IsBringResource(_commonResources))
                {
                    CharacterTaskData porterAnim = CharacterManager.Instance.GetAssistantWorker(Character.CharacterList.AssistantWorker);
                    porterAnim.CharacterBehaviour.Action(StatusAction.CarriBox);
                    porterAnim.CharacterBehaviour.transform.localPosition = Utils.GetWorkerHouse().Position;
                    DelayPorterCanceledUpgrade(i, porterAnim, sizetilingTransform, posTilingTransform, _commonResources, userBuilding, buildingObjectId);
                }
                else
                {
                    var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
                    if (taskData == null) return;
                    GameData.Instance.RemoveJobWorkerAssignment(taskData.TaskId);
                    TaskManager.Instance.RemoveTask(buildingObjectId);
                    GameData.Instance.RequestSaveGame();
                }
            }
        }

        public async void DelayPorterCanceledUpgrade(int index, CharacterTaskData Assistan, Vector3 sizetilingTransform,
           Vector3 posTilingTransform, List<ResourcePoco> _commonResources, UserBuilding userBuilding, int buildingObjectId)
        {
            float timeDelay = MasterDataStore.Instance.GetPopulationWorkerTimeComeOut() * index;
            await Task.Delay(TimeSpan.FromMilliseconds(timeDelay));
            PorterCollectCanceledUpgrade(index, Assistan, sizetilingTransform, posTilingTransform, _commonResources, userBuilding, buildingObjectId);
        }

        private void PorterCollectCanceledUpgrade(int index, CharacterTaskData Assistan, Vector3 sizetilingTransform,
          Vector3 posTilingTransform, List<ResourcePoco> _commonResources, UserBuilding userBuilding, int buildingObjectId)
        {
            if (!ItemHelper.IsSmallerOrEquals(userBuilding.refundMaterial))
            {
                if (Assistan != null)
                {
                    Assistan.CharacterBehaviour.Reset(false);
                    Assistan.StatusAction = StatusAction.CarriBox;
                    Assistan.BuildingObjectId = buildingObjectId;
                    Assistan.CharacterBehaviour.InitMoveTrackToken();
                    var size = sizetilingTransform;
                    Assistan.CharacterBehaviour.SetCommonResources(_commonResources);
                    Vector3 posDoorHouse = Utils.ConvertPositionDoor(posTilingTransform, RotationDoors.Bottom, size.x);
                    Assistan.CharacterBehaviour.MoveCenter(Vector3.zero, 2f, posTilingTransform, (dataStore, rotation) =>
                    {
                        Assistan.CharacterBehaviour.Action(StatusAction.CarriBoxHigh);
                        if (Assistan.CharacterBehaviour.ObjMaterialCarribox != null) Assistan.CharacterBehaviour.ObjMaterialCarribox.SetActive(true);

                        var commonResources = Assistan.CharacterBehaviour.GetCommonResources();
                        ResourcePocoTask resourcePocoTask = Utils.GetItemDecreaseType(userBuilding.refundMaterial, commonResources);
                        if (resourcePocoTask == null) return;
                        Assistan.CharacterBehaviour.DecreaseResource(userBuilding.refundMaterial, userBuilding);
                        BuildingMain storeHouse = Utils.GetStoreHouse(resourcePocoTask.ItemType);

                        if (storeHouse == null)
                        {
                            return;
                        }
                        var buildingOperationStore = Utils.GetBuildingOperation(storeHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                        Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperationStore.GetDoorPosition());
                        Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperationStore.GetDoorPosition());
                        Assistan.CharacterBehaviour.MoveCenter(Vector3.zero, 1.5f, posOutDoorHouse, (dataStore, rotation) =>
                        {
                            var workerHouse = Utils.GetWorkerHouse();
                            Vector3 posDoorHouse = Utils.ConvertPositionDoor(workerHouse.Position, RotationDoors.Left, workerHouse.Size.x);
                            if (Assistan.CharacterBehaviour.ObjMaterialCarribox != null) Assistan.CharacterBehaviour.ObjMaterialCarribox.SetActive(false);
                            Assistan.CharacterBehaviour.Action(StatusAction.CarriBox);
                            List<ResourcePoco> countAmount = commonResources.FindAll(t => t.itemCount > 0);
                            if (countAmount.Count == 0)
                            {
                                Assistan.CharacterBehaviour.MoveCenter(Vector3.zero, 1.5f, posDoorHouse, (dataStore, rotation) =>
                                {
                                    VassalCancel(buildingObjectId);
                                    Assistan.BuildingObjectId = -1;
                                    GameData.Instance.SavedPack.SaveData.WorkersWorking--;
                                    GameData.Instance.RequestSaveGame();
                                    Assistan.StatusAction = StatusAction.None;
                                    Assistan.CharacterBehaviour.IsMove = false;
                                    Assistan.CharacterBehaviour.Action(StatusAction.Idle);
                                    Assistan.CharacterBehaviour.gameObject.SetActive(false);
                                    var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
                                    if (taskData == null) return;
                                    GameData.Instance.RemoveJobWorkerAssignment(taskData.TaskId);
                                    TaskManager.Instance.RemoveTask(buildingObjectId);
                                    GameData.Instance.RequestSaveGame();
                                });
                            }
                            else
                            {
                                if (!Utils.IsCheckWorkerSameBuilding(buildingObjectId, Utils.RemainingResources(_commonResources, userBuilding)))
                                {
                                    Assistan.CharacterBehaviour.Move(size, posTilingTransform, (dataStore) =>
                                    {
                                        PorterCollectCanceledUpgrade(index, Assistan, sizetilingTransform, posTilingTransform, _commonResources, userBuilding, buildingObjectId);
                                    });
                                }
                                else
                                {
                                    Assistan.BuildingObjectId = -1;
                                    Assistan.CharacterBehaviour.MoveCenter(Vector3.zero, 1.5f, posDoorHouse, (dataStore, rotation) =>
                                    {
                                        VassalCancel(buildingObjectId);
                                        GameData.Instance.SavedPack.SaveData.WorkersWorking--;
                                        GameData.Instance.RequestSaveGame();
                                        Assistan.StatusAction = StatusAction.None;
                                        Assistan.CharacterBehaviour.IsMove = false;
                                        Assistan.CharacterBehaviour.Action(StatusAction.Idle);
                                        Assistan.CharacterBehaviour.gameObject.SetActive(false);
                                    });
                                }
                            }
                            PopulationManager.Instance.AddItemResource(resourcePocoTask.ResourcePoco.itemId, new RandomOption());

                        });
                    });

                }
            }
        }

        public void VassalCancel(int buildingObjectId)
        {
            var characterTaskData = CharacterManager.Instance.GetCharacterTaskData();
            foreach (var player in characterTaskData)
            {
                if (player.BuildingObjectId == buildingObjectId && player.StatusAction == StatusAction.Work00)
                {
                    player.CharacterBehaviour.Action(StatusAction.Walk);
                    player.CharacterBehaviour.Reset(true);
                    player.BuildingObjectId = -1;
                    player.StatusAction = StatusAction.None;
                    var vassalData = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == player.CharacterId);
                    if (vassalData != null) vassalData.IsWorking = false;

                    if (player.Type == Character.CharacterList.Vassar)
                    {
                        player.CharacterBehaviour.SetAIStop(false);
                        player.StatusAction = StatusAction.None;
                        player.CharacterBehaviour.IsAutoMove = true;
                        player.CharacterBehaviour.MoveAutoFreedom();
                        GameManager.Instance.UpdateAvaterInBuilding(buildingObjectId, 0);
                    }
                }
            }
        }
    }
}
