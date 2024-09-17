using System.Collections.Generic;
using UnityEngine;
using Google.FlatBuffers;
using Town.Tile;
using Pathfinding;
using System;
using Cysharp.Threading;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using TMPro;
using CoreData.UniFlow.Commander;
using CoreData.UniFlow.Common;
using DG.Tweening;
using System.Threading.Tasks;

namespace CoreData.UniFlow.Common
{
    public class CharacterBehaviour : MonoBehaviour
    {
        #region Private fields
        [SerializeField]
        private AIPath _aiPath;
        
        [SerializeField]
        private Animator _model;

        private CharacterTaskData _characterTaskData;
        public CharacterTaskData CharacterTaskData
        {
            get { return _characterTaskData; }
            set { _characterTaskData = value; }
        }

        [SerializeField]
        private GameEvent _onSpeedWorkedTest;
        [SerializeField]
        public GameObject ObjMaterialCarribox;
        [SerializeField]
        private GameEvent _onStartFindMaterial;
        [SerializeField]
        private GameEvent _onStartTimeBuilding;
        [SerializeField]
        private UserItemStorage _userItemStorage;
        [SerializeField]
        private GameEvent _onBuildingUpdateInfo;
        [SerializeField]
        private GameEvent _onUpdateCharacterInfo;
        [SerializeField]
        private UserBuildingList _userBuildingList;
        [SerializeField]
        private TownBaseBuildingSOList _buildingSO;
        [SerializeField]
        private GameEvent _onRaiseCompleteFillResource;
        [SerializeField]
        private GameEvent _onBuildingProductionUpdateInfo;
        [SerializeField]
        private GameEvent _onRaiseCompleteFillResourceProduction;
        [SerializeField]
        private UserBuildingProductionList _userBuildingProductionList;

        // event
        public void OnRaiseCompleteFillResource(TaskData taskData) {
            _onRaiseCompleteFillResource?.Raise(taskData);
        }

        public void OnRaiseCompleteFillResourceProduction(TaskData taskData)
        {
            _onRaiseCompleteFillResourceProduction?.Raise(taskData);
        }

        public void OnBuildingProductionUpdateInfo()
        {
            _onBuildingProductionUpdateInfo?.Raise(_userBuildingProduction);
        }

        private UserBuilding _userBuilding;
        private List<ResourcePoco> _commonResources;
        private CharacterAnimation _characterAnimation;
        private CancellationTokenSource _moveTrackToken;
        private UserBuildingProduction _userBuildingProduction;
        private UniTaskCancelableAsyncEnumerable<AsyncUnit> _moveTrackLoop;

        // variable
        public float GetWalkSpeed() => _walkSpeed;
        public UserBuilding UserBuilding => _userBuilding;
        public UserItemStorage UserItemStorage => _userItemStorage;
        public UserBuildingList UserBuildingList => _userBuildingList;
        public void CancelMoveTrackToken() => _moveTrackToken.Cancel();
        public List<ResourcePoco> GetCommonResources() => _commonResources;
        public TownBaseBuildingSOList TownBaseBuildingSOList => _buildingSO;
        public void IsAiPathFinding(bool isAiPath) => _aiPath.enabled = isAiPath;
        public UserBuildingProduction UserBuildingProduction => _userBuildingProduction;
        public void InitMoveTrackToken() => _moveTrackToken = new CancellationTokenSource();
        public UserBuildingProductionList UserBuildingProductionList => _userBuildingProductionList;
        public List<ResourcePoco> SetCommonResources(List<ResourcePoco> commonResources) => _commonResources = commonResources;
        #endregion
        #region System methods
        private float _walkSpeed = 30f;
        private bool _isAction = false;
        private bool _isMove = false;
        private bool _isAutoMove = true;

        public bool IsMove
        {
            get { return _isMove; }
            set { _isMove = value; }
        }

        public bool IsAction
        {
            get { return _isAction; }
            set { _isAction = value; }
        }

        public bool IsAutoMove
        {
            get { return _isAutoMove; }
            set { _isAutoMove = value; }
        }

        private void OnEnable()
        {
            _moveTrackToken = new CancellationTokenSource();
            _onSpeedWorkedTest.Subcribe(OnSpeedWorkerTest);
        }

        private void OnDisable()
        {
            _onSpeedWorkedTest.Unsubcribe(OnSpeedWorkerTest);
        }

        private void OnSpeedWorkerTest(object[] eventParam)
        {
            _aiPath.maxSpeed = ShareUIManager.Instance.Config.DEFAULT_SPEED_WORKER;
        }

        public void Reset(bool isSpeedNormal = true)
        {
            IsMove = true;
            IsAutoMove = false;
            if (ObjMaterialCarribox != null) ObjMaterialCarribox.SetActive(false);
            CharacterTaskData.StatusAction = StatusAction.None;
            _aiPath.maxSpeed = ShareUIManager.Instance.Config.SLOW_SPEED_WORKER;
        }

        public void InitData(CharacterTaskData characterTaskData)
        {
            _characterTaskData = characterTaskData;
            _model.gameObject.SetActive(true);
            _characterAnimation = new CharacterAnimation();
            _characterAnimation.InitAnimator(_model);
            if (ObjMaterialCarribox != null) ObjMaterialCarribox.SetActive(false);
            IsAction = false;
            if (_characterTaskData.Type == Character.CharacterList.Vassar) { IsAutoMove = true; MoveAutoFreedom(); }
        }

        public void InitMission(int buildingObjectId, List<ResourcePoco> cost , ResourceFinder resourceFinder, float timebuild)
        {
            _commonResources = cost;
            _userBuilding = _userBuildingList.GetBuilding(buildingObjectId);
            _userBuilding.currentContructionRate = timebuild;
            if (_userBuilding.constructionMaterial == null)
                _userBuilding.constructionMaterial = new List<ResourcePoco>();
        }

        public void InitMissionProduction(int buildingObjectId, List<ResourcePoco> cost, ResourceFinder resourceFinder, float timebuild)
        {
            _commonResources = cost;
            _userBuildingProduction = _userBuildingProductionList.GetBuildingProduction(buildingObjectId);
            _userBuildingProduction.productRate = timebuild;
            if (_userBuildingProduction.currentMaterials == null)
                _userBuildingProduction.currentMaterials = new List<ResourcePoco>();
        }

        public async void MoveAutoFreedom()
        {
            if (!IsAutoMove || this == null || _aiPath == null) return;
            IsMove = true;
            Vector3 target = Vector3.one;
            _characterAnimation.StartAnimation(StatusAction.Walk,(isComplete)=> {
                var posCurrent = this.transform.localPosition;
                var posDistance = ConfigManager.DistanceMoveVassar;
                var posXRandom = UnityEngine.Random.Range((posCurrent.x - posDistance), (posCurrent.x + posDistance));
                var posZRandom = UnityEngine.Random.Range((posCurrent.z - posDistance), (posCurrent.z + posDistance));
                if (posXRandom < -24.5f || posXRandom > 24.5f) posXRandom = UnityEngine.Random.Range(-24.5f, 24.5f);
                if (posZRandom < -24.5f || posZRandom > 24.5f) posZRandom = UnityEngine.Random.Range(-24.5f, 24.5f);

                target = new Vector3(posXRandom, 0, posZRandom);
                _characterTaskData.StatusAction = StatusAction.None;
                _aiPath.destination = target;
                InitMoveTrackToken();
                _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);
            });
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            await foreach (var _ in _moveTrackLoop)
            {
                if (this == null || _aiPath == null) return;
                if (Utils.IsCheckDistance(this.transform.localPosition, target) && _aiPath.remainingDistance < 0.2f)
                {
                    _characterAnimation.StartAnimation(StatusAction.Idle);
                    await Task.Delay(TimeSpan.FromSeconds(1.5f));
                    MoveAutoFreedom();

                    break;
                }
            }
        }

        public void SetAIStop(bool iStop) => _aiPath.isStopped = iStop;
        public async void Move(Vector3 size ,Vector3 position,Action<DataStore> onComplete)
        {
            SetAIStop(false);
            CancelMoveTrackToken();
            IsMove = true;
            float distance = size.x > 1.5f ? size.x : 1.5f;
            if (size != Vector3.zero && size.x > 2.5f)
                distance = size.x - 1.0f;
            _aiPath.destination = position;
            InitMoveTrackToken();
            _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);
            await foreach (var _ in _moveTrackLoop)
            {
                if (Utils.IsCheckDistance(this.transform.localPosition, position, distance) && _aiPath.remainingDistance < distance)
                {
                    SetAIStop(true);
                    onComplete?.Invoke(new DataStore(this, TypeStore.Stone));
                    break;
                }
            }
        }

        public async void MoveOutDoor(Vector3 rotation, float distance, Vector3 position,
            Action<DataStore, Vector3> onComplete)
        {
            SetAIStop(false);
            CancelMoveTrackToken();
            IsMove = true;
            if (!_aiPath.enabled) _aiPath.enabled = true;
            InitMoveTrackToken();
            _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);
            await foreach (var _ in _moveTrackLoop)
            {
                if (_aiPath.enabled && _characterTaskData.Type != Character.CharacterList.AssistantWorker) _aiPath.enabled = false;
                // Move our position a step closer to the target.
                var step = (_aiPath.maxSpeed / 2) * Time.deltaTime; // calculate distance to move
                this.transform.position = Vector3.MoveTowards(this.transform.position, position, step);
                // Check if the position of the cube and sphere are approximately equal.
                if (Vector3.Distance(this.transform.position, position) < 0.08f)
                {
                    // Swap the position of the cylinder.
                    position *= -1.0f;
                    SetAIStop(true);
                    if (!_aiPath.enabled) _aiPath.enabled = true;
                    onComplete?.Invoke(new DataStore(this, TypeStore.Stone), rotation);
                    break;
                }
            }
        }

        public async void MoveComback(Vector3 rotation, float distance, Vector3 position,
           Action<DataStore, Vector3> onComplete)
        {
            SetAIStop(false);
            CancelMoveTrackToken();
            IsMove = true;
            if (!_aiPath.enabled) _aiPath.enabled = true;
            _aiPath.destination = position;
            InitMoveTrackToken();
            _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);
            await foreach (var _ in _moveTrackLoop)
            {
                if (Utils.IsCheckDistance(this.transform.localPosition, position, distance))
                {
                    onComplete?.Invoke(new DataStore(this, TypeStore.Stone), rotation);
                    break;
                }
            }
        }

        public async void MoveCenter(Vector3 rotation,float distance ,Vector3 position, 
            Action<DataStore,Vector3> onComplete, bool isCheck = true)
        {
            SetAIStop(false);
            CancelMoveTrackToken();
            IsMove = true;

            if (_aiPath != null && this.transform != null)
            {
                if (!_aiPath.enabled) _aiPath.enabled = true;
                _aiPath.destination = position;
                InitMoveTrackToken();
                _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);
                await foreach (var _ in _moveTrackLoop)
                {
                    if (isCheck)
                    {
                        if (_characterTaskData.BuildingObjectId == -1)
                        {
                            var workerHouse = Utils.GetWorkerHouse();
                            Vector3 posOutDoorHouse = Utils.ConvertPositionDoor(workerHouse.Position,
                                RotationDoors.Left, workerHouse.Size.x);
                            MoveComback(Vector3.zero, 1.0f, posOutDoorHouse, async (dataStore, rotation) =>
                            {
                                IsMove = false;
                                Action(StatusAction.Idle);
                                CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(_characterTaskData);
                            });
                            break;
                        }

                        if (_aiPath == null && gameObject == null) return;
                        if (Utils.IsCheckDistance(this.transform.localPosition, position, distance))
                        {
                            SetAIStop(true);
                            onComplete?.Invoke(new DataStore(this, TypeStore.Stone), rotation);
                            break;
                        }
                    }
                    else
                    {
                        if (_aiPath.enabled && _characterTaskData.Type != Character.CharacterList.AssistantWorker)
                            _aiPath.enabled = false;
                        // Move our position a step closer to the target.
                        var step = (_aiPath.maxSpeed / 2) * Time.deltaTime; // calculate distance to move
                        this.transform.position = Vector3.MoveTowards(this.transform.position, position, step);
                        // Check if the position of the cube and sphere are approximately equal.
                        if (Vector3.Distance(this.transform.position, position) < 0.08f)
                        {
                            // Swap the position of the cylinder.
                            position *= -1.0f;
                            SetAIStop(true);
                            if (!_aiPath.enabled) _aiPath.enabled = true;
                            onComplete?.Invoke(new DataStore(this, TypeStore.Stone), rotation);
                            break;
                        }
                    }
                }
            }
        }

        public async void MovePositionBuild(Vector3 rotation, float distance, Vector3 position,
            Action<DataStore, Vector3> onComplete, bool isCheck = true)
        {
            SetAIStop(false);
            CancelMoveTrackToken();
            IsMove = true;
            _aiPath.destination = position;
            InitMoveTrackToken();
            _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);
            await foreach (var _ in _moveTrackLoop)
            {
                if (Vector3.Distance(this.transform.position, position) < 0.5f)
                {
                    SetAIStop(true);
                    onComplete?.Invoke(new DataStore(this, TypeStore.Stone), rotation);
                    break;
                }
            }
        }

        public void StartMaterial(int index,TaskData taskData, StatusCache statusCache, TypeJobAction typeJobAction)
        {
            switch(typeJobAction)
            {
                case TypeJobAction.ITEM:
                    resourcePocosFinishCurrent = taskData.CommonResources;
                    this.GetComponent<BringFinishResourceItem>().FinishResourceItem(index, taskData, statusCache);
                    break;
                case TypeJobAction.DESTROY:
                    resourcePocosFinishCurrent = taskData.CommonResources;
                    this.GetComponent<BringFinishResourceItem>().FinishResourceItem(index, taskData, statusCache);
                    break;
                case TypeJobAction.CONTRUCTION:
                    this.GetComponent<PorterBringResourceContruction>().CountResource(taskData, statusCache);
                    break;
                case TypeJobAction.UPGRADE:
                    this.GetComponent<PorterBringResourceUpgrade>().CountResourceUpgrade(taskData, statusCache);
                    break;
                case TypeJobAction.PRODUCE:
                    this.GetComponent<PorterBringResourceProduction>().CountResourceProduction(taskData, statusCache);
                    break;
                case TypeJobAction.FINISHITEM:
                    resourcePocosFinishCurrent = taskData.CommonResources;
                    this.GetComponent<BringFinishResourceItem>().FinishResourceItem(index, taskData, statusCache);
                    break;
            }
        }
        
        public void OnResourceNoEnough(TaskData taskData)
        {
            var posHouse = Utils.GetWorkerHouse();
            var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
            Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperation.GetDoorPosition());
            Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperation.GetDoorPosition());
            Vector2 buildingSize = _buildingSO.GetBuildingSize(taskData.BuildingId);
            MoveCenter(Vector3.zero, 5.0f, posHouse.Position, async (dataStore, ratation) =>
            {
                buildingOperation.OpenDoor(0);
                IsMove = false;
                Action(StatusAction.Idle);
                CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(_characterTaskData);
                _characterTaskData.BuildingObjectId = -1;
                await UniTask.DelayFrame(10);
                buildingOperation.CloseDoor(0);
            });
        }

        public void DecreaseResource(List<ResourcePoco> commonResources, UserBuilding userBuilding)
        {
            List<ResourcePoco> res = commonResources;
            var item = new ResourcePoco();
            for (var i = 0;i< res.Count;i++)
            {
                if (res[i].itemCount > 0)
                {
                    item = res[i];
                    break;
                }
            }
            var itemSub = userBuilding.refundMaterial.Find(t => t.itemId == item.itemId);
            var indext = userBuilding.refundMaterial.FindIndex(t => t.itemId == item.itemId);
            if (indext >= 0 && indext < userBuilding.refundMaterial.Count)
            {
                item.itemCount--;
                userBuilding.refundMaterial[indext] = item;
                _onBuildingUpdateInfo.Raise(userBuilding);
            }
        }

        public List<ResourcePoco> resourcePocosFinishCurrent = new List<ResourcePoco>();
        public ResourcePocoTask GetItemFinishType(List<ResourcePoco> resourcePocosTotal)
        {
            if (resourcePocosTotal == null) return null;
            ResourcePocoTask resourcePocoTask = new ResourcePocoTask();
            for (int i = 0; i < resourcePocosTotal.Count; i++)
            {
                var itemCurrent = resourcePocosFinishCurrent.Find(t => t.itemId == resourcePocosTotal[i].itemId);
                if(itemCurrent.itemCount > 0)
                {
                    itemCurrent.itemCount--;
                    resourcePocoTask.ResourcePoco = resourcePocosTotal[i];
                    resourcePocoTask.IndexResourcePoco = i;
                    resourcePocoTask.ItemType = TownTypeTransform.Instance.ItemList.GetItemType(resourcePocosTotal[i].itemId);
                    return resourcePocoTask;
                }
            }
            return resourcePocoTask;
        }

        public bool IsCheckEmptyResource(List<ResourcePoco> _commonResources)
        {
            bool isEmptyResource = true;
            for (int j = 0; j < _commonResources.Count; j++)
            {
                ResourcePoco resCheck = _commonResources[j];
                var itemInStoreCheck = _userItemStorage.GetItemCount(resCheck.itemId);
                if (itemInStoreCheck != 0)
                {
                    isEmptyResource = false;
                }
            }
            return isEmptyResource;
        }

        public void Action(StatusAction action, Action<bool> onComplete = null)
        {
            _characterAnimation.StartAnimation(action, (isComplete) => { onComplete?.Invoke(isComplete); });
        }

        private void OnApplicationQuit()
        {
            CancelMoveTrackToken();
        }
        #endregion
    }
}

