using CoreData.UniFlow.Commander;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using static CoreData.UniFlow.Common.Character;

namespace CoreData.UniFlow.Common
{
    public class CharacterManager : MonoSingleton<CharacterManager>
    {
        #region Private fields
        [SerializeField]
        private GameObject _vassarPrefab_genghiskhan;

        [SerializeField]
        private GameObject _vassarPrefab_attila;

        [SerializeField]
        private GameObject _vassarPrefab_jeannedarc;

        [SerializeField]
        private GameObject _vassarPrefab_richardi;

        [SerializeField]
        private GameObject _vassarPrefab_rurik;

        [SerializeField]
        private GameObject _vassarPrefab_saladin;

        [SerializeField]
        private GameObject _vassarPrefab_xiangyu;

        [SerializeField]
        private GameObject _workerPrefab;

        [SerializeField]
        private GameObject _workerAssistantPrefab;

        [SerializeField]
        private Transform _modelParent;

        private MasterDataStore _masterDataStore;

        [SerializeField]
        private GameEvent _onStartFindMaterial;
        
        [SerializeField]
        private GameEvent _onUpdatePositionHouse;

        [SerializeField]
        private GameEvent _onUpdateCharacterInfo;
        
        [SerializeField]
        private GameEvent _onBuildingProductionUpdateInfo;

        [SerializeField]
        private GameEvent _onUpdateManagerTask;
        
        [Header("Reference - Read")]

        [SerializeField]
        private Transform _parentBuilding;

        [SerializeField]
        private ResourceFinder _resourceFinder;

        [SerializeField]
        private UserItemStorage _userItemStorage;

        [Header("Child fields")]
        [SerializeField]
        private TownBaseBuildingSOList _buildingSO;

        [SerializeField]
        private UserBuildingList _userBuildingList;

        [SerializeField]
        private UserBuildingProductionList _userBuildingProductionList;

        // variable
        public UserItemStorage UserItemStorage => _userItemStorage;
        public UserBuildingList UserBuildingList => _userBuildingList;
        public UserBuildingProductionList UserBuildingProductionList => _userBuildingProductionList;

        #endregion
        #region System methods

        private ObjectPoolCharacter _poolVassalCharacter_genghiskhan;

        private ObjectPoolCharacter _poolVassalCharacter_attila;

        private ObjectPoolCharacter _poolVassalCharacter_jeannedarc;

        private ObjectPoolCharacter _poolVassalCharacter_richardi;

        private ObjectPoolCharacter _poolVassalCharacter_rurik;

        private ObjectPoolCharacter _poolVassalCharacter_saladin;

        private ObjectPoolCharacter _poolVassalCharacter_xiangyu;

        private ObjectPoolCharacter _poolWorkerCharacter;

        private ObjectPoolCharacter _poolAssistantWorkerCharacter;

        public Transform GetParentBuilding() { return _parentBuilding; }

        private List<CharacterTaskData> _characterTaskData = new List<CharacterTaskData>();

        public ObjectPoolCharacter GetPoolWorkerCharacter() { return _poolWorkerCharacter; }

        public List<CharacterTaskData> GetCharacterTaskData() { return _characterTaskData; }
        public async UniTaskVoid Init()
        {
            await UniTask.DelayFrame(2);

            _masterDataStore = MasterDataStore.Instance;
            Preallocation vassal_1 = new Preallocation();
            vassal_1.gameObject = _vassarPrefab_genghiskhan;
            vassal_1.count = 1;
            vassal_1.expandable = true;
            vassal_1.parent = _modelParent;
            vassal_1.IdVassalModel = 1;
            vassal_1.CType = Character.CharacterList.Vassar;
            vassal_1.Name = 1.ToString();

            _poolVassalCharacter_genghiskhan = ObjectPoolCharacter.Instance.Init(vassal_1, _characterTaskData);

            Preallocation vassal_2 = new Preallocation();
            vassal_2.gameObject = _vassarPrefab_richardi;
            vassal_2.count = 1;
            vassal_2.expandable = true;
            vassal_2.parent = _modelParent;
            vassal_2.IdVassalModel = 2;
            vassal_2.CType = Character.CharacterList.Vassar;
            vassal_2.Name = 2.ToString();

            _poolVassalCharacter_richardi = ObjectPoolCharacter.Instance.Init(vassal_2, _characterTaskData);

            Preallocation vassal_3 = new Preallocation();
            vassal_3.gameObject = _vassarPrefab_jeannedarc;
            vassal_3.count = 1;
            vassal_3.expandable = true;
            vassal_3.parent = _modelParent;
            vassal_3.IdVassalModel = 3;
            vassal_3.CType = Character.CharacterList.Vassar;
            vassal_3.Name = 3.ToString();

            _poolVassalCharacter_jeannedarc = ObjectPoolCharacter.Instance.Init(vassal_3, _characterTaskData);

            Preallocation vassal_4 = new Preallocation();
            vassal_4.gameObject = _vassarPrefab_saladin;
            vassal_4.count = 1;
            vassal_4.expandable = true;
            vassal_4.parent = _modelParent;
            vassal_4.IdVassalModel = 4;
            vassal_4.CType = Character.CharacterList.Vassar;
            vassal_4.Name = 4.ToString();

            _poolVassalCharacter_saladin = ObjectPoolCharacter.Instance.Init(vassal_4, _characterTaskData);

            Preallocation vassal_5 = new Preallocation();
            vassal_5.gameObject = _vassarPrefab_attila;
            vassal_5.count = 1;
            vassal_5.expandable = true;
            vassal_5.parent = _modelParent;
            vassal_5.IdVassalModel = 5;
            vassal_5.CType = Character.CharacterList.Vassar;
            vassal_5.Name = 5.ToString();

            _poolVassalCharacter_attila = ObjectPoolCharacter.Instance.Init(vassal_5, _characterTaskData);

            Preallocation vassal_6 = new Preallocation();
            vassal_6.gameObject = _vassarPrefab_rurik;
            vassal_6.count = 1;
            vassal_6.expandable = true;
            vassal_6.parent = _modelParent;
            vassal_6.IdVassalModel = 6;
            vassal_6.CType = Character.CharacterList.Vassar;
            vassal_6.Name = 6.ToString();

            _poolVassalCharacter_rurik = ObjectPoolCharacter.Instance.Init(vassal_6, _characterTaskData);

            Preallocation vassal_7 = new Preallocation();
            vassal_7.gameObject = _vassarPrefab_xiangyu;
            vassal_7.count = 1;
            vassal_7.expandable = true;
            vassal_7.parent = _modelParent;
            vassal_7.IdVassalModel = 7;
            vassal_7.CType = Character.CharacterList.Vassar;
            vassal_7.Name = 7.ToString();

            _poolVassalCharacter_xiangyu = ObjectPoolCharacter.Instance.Init(vassal_7, _characterTaskData);

            Preallocation worker = new Preallocation();
            worker.gameObject = _workerPrefab;
            worker.count = 5;
            worker.expandable = true;
            worker.parent = _modelParent;
            worker.CType = Character.CharacterList.Farmer;
            worker.Name = "Worker";
            _poolWorkerCharacter = ObjectPoolCharacter.Instance.Init(worker, _characterTaskData);

            Preallocation AassistantWorker = new Preallocation();
            AassistantWorker.gameObject = _workerAssistantPrefab;
            AassistantWorker.count = 5;
            AassistantWorker.expandable = true;
            AassistantWorker.parent = _modelParent;
            AassistantWorker.CType = Character.CharacterList.AssistantWorker;
            AassistantWorker.Name = "Porter";
            _poolAssistantWorkerCharacter = ObjectPoolCharacter.Instance.Init(AassistantWorker, _characterTaskData);

            await UniTask.DelayFrame(10);
            var posHouse = Utils.GetWorkerHouse();
            if (posHouse == null) return;
            var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
            Vector3 posDoorHouse = buildingOperation.GetDoorPosition();
            for (var i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
            {
                var info = GameData.Instance.SavedPack.SaveData.VassalInfos[i];
                var dataVassal = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == info.Data.idVassalTemplate);
                CharacterTaskData vassalData = GetVassar(info.Data.idVassalTemplate, Character.CharacterList.Vassar);
                vassalData.CharacterBehaviour.gameObject.name = dataVassal.LastName;

                vassalData.CharacterBehaviour.transform.localPosition = Utils.GetInDoorPosition(posDoorHouse);
                buildingOperation.OpenDoor(0);
                vassalData.CharacterBehaviour.MoveCenter(Vector3.zero, 0f, Utils.GetOutDoorPosition(posDoorHouse), (dataStore, rotation) =>
                {
                    buildingOperation.CloseDoor(0);
                    vassalData.CharacterBehaviour.SetAIStop(false);
                    vassalData.CharacterBehaviour.IsAutoMove = true;
                    vassalData.CharacterBehaviour.MoveAutoFreedom();
                }, false);
            }
            if (GameData.Instance != null)
                GameData.Instance.RequestSaveGame();
        }
     
        private void OnEnable()
        {
            _onUpdatePositionHouse.Subcribe(OnUpdatePositionHouse);
            _onUpdateCharacterInfo.Subcribe(OnUpdateCharacterInfo);

        }

        private void OnDisable()
        {
            _onUpdatePositionHouse.Unsubcribe(OnUpdatePositionHouse);
            _onUpdateCharacterInfo.Unsubcribe(OnUpdateCharacterInfo);
        }
        public void OnUpdateManagerTask()
        {
            _onUpdateManagerTask?.Raise();
        }
      
        public CharacterTaskData GetVassar(int idModelVassal,Character.CharacterList cType)
        {
            switch(idModelVassal)
            {
                case 1:
                    return _poolVassalCharacter_genghiskhan.SpawnVassal(idModelVassal, _characterTaskData, cType);
                case 2:
                    return _poolVassalCharacter_richardi.SpawnVassal(idModelVassal, _characterTaskData, cType);
                case 3:
                    return _poolVassalCharacter_jeannedarc.SpawnVassal(idModelVassal, _characterTaskData, cType);
                case 4:
                    return _poolVassalCharacter_saladin.SpawnVassal(idModelVassal, _characterTaskData, cType);
                case 5:
                    return _poolVassalCharacter_attila.SpawnVassal(idModelVassal, _characterTaskData, cType);
                case 6:
                    return _poolVassalCharacter_rurik.SpawnVassal(idModelVassal, _characterTaskData, cType);
                case 7:
                    return _poolVassalCharacter_xiangyu.SpawnVassal(idModelVassal, _characterTaskData, cType);
            }
            return null;
        }

        public CharacterTaskData GetCharacter(Character.CharacterList cType)
        {
            CharacterTaskData characterTaskData = _poolWorkerCharacter.Spawn(_characterTaskData, cType);
            while (characterTaskData.CharacterBehaviour == null)
            {
                characterTaskData = _poolWorkerCharacter.Spawn(_characterTaskData, cType);
            }
            if(characterTaskData.CharacterBehaviour != null && Utils.GetWorkerHouse() != null)
            {
                characterTaskData.CharacterBehaviour.transform.localPosition = Utils.GetWorkerHouse().Position;
                characterTaskData.CharacterBehaviour.Action(StatusAction.Walk);
                return characterTaskData;
            }
            return null;
        }

        public CharacterTaskData GetAssistantWorker(Character.CharacterList cType)
        {
            return _poolAssistantWorkerCharacter.Spawn(_characterTaskData, cType);
        }
        #endregion

        #region action building
        public void OnUpdatePositionHouse(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            Transform transform = (Transform)eventParam[0];
            foreach (var player in _characterTaskData)
            {
                if (player.Type == Character.CharacterList.Farmer && player.CharacterBehaviour.transform.localPosition == Vector3.zero)
                {
                    player.CharacterBehaviour.Move(Vector3.zero, transform.localPosition, (data) => { });
                }
            }
        }

        public void OnStartJob(TaskData taskData, StatusCache statusCache)
        {
            int modelCurrent = MasterDataStore.Instance.GetCountWorker(taskData.Man);
            Worker(modelCurrent, taskData, statusCache);
            GameData.Instance.SavedPack.SaveData.WorkloadWorkerHouse++;
            GameData.Instance.RequestSaveGame();
            JobManager.Instance.TriggerWorkloadDecrease(taskData);
            //var itemQuene = TriggerTimer.TimerTaskData.Find(t => t.TaskId == taskData.TaskId);
            //if (itemQuene == null)
            //{
            //    JobManager.Instance.TriggerWorkloadDecrease(taskData);
            //    TriggerTimer.TimerTaskData.Add(taskData);
            //}
        }

        public void OnBeginFillResource(TaskData taskData, StatusCache statusCache, TypeJobAction typeJobAction)
        {
            // check resouce ins tore
            if (Utils.IsEmptyResouce(taskData, _userItemStorage)) return;
            taskData.StatusFillResource = StatusFillResource.BeginFill;
            _onStartFindMaterial.Raise(taskData.BuildingObjectId);
            int porter = 3;
            for (int i = 0; i < porter; i++)
            {
                if (i < Utils.IsBringResource(taskData.CommonResources))
                    PorterBeginTask(i, taskData, statusCache, typeJobAction);
            }
            GameData.Instance.RequestSaveGame();
        }

        public async void PorterBeginTask(int index, TaskData taskData, StatusCache statusCache, TypeJobAction typeJobAction)
        {
            float timeDelay = _masterDataStore.GetPopulationWorkerTimeComeOut() * index;
            await Task.Delay(TimeSpan.FromMilliseconds(timeDelay));
            CharacterTaskData farmer = GetAssistantWorker(Character.CharacterList.AssistantWorker);
            farmer.CharacterBehaviour.gameObject.SetActive(false);
            farmer.CharacterBehaviour.Reset(false);
            farmer.CharacterBehaviour.Action(StatusAction.CarriBox);
            farmer.BuildingObjectId = taskData.BuildingObjectId;
            if (typeJobAction == TypeJobAction.PRODUCE)
                farmer.CharacterBehaviour.InitMissionProduction(taskData.BuildingObjectId, taskData.CommonResources, _resourceFinder, taskData.SpeedRate);
            else
                farmer.CharacterBehaviour.InitMission(taskData.BuildingObjectId, taskData.CommonResources, _resourceFinder, taskData.SpeedRate);

            var posHouse = Utils.GetWorkerHouse();
            var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
            Vector3 posDoorHouse = buildingOperation.GetDoorPosition();
            farmer.CharacterBehaviour.transform.localPosition = Utils.GetOutDoorPosition(posDoorHouse);
            farmer.CharacterBehaviour.gameObject.SetActive(true);
            buildingOperation.OpenDoor(0);
            farmer.CharacterBehaviour.StartMaterial(index, taskData, statusCache, taskData.TypeJobAction);
            //farmer.CharacterBehaviour.MoveOutDoor(Vector3.zero, 0.5f, outDoor, (dataStore, rotation) =>
            // {
            //     buildingOperation.CloseDoor(0);
            //     farmer.CharacterBehaviour.StartMaterial(taskData, statusCache, taskData.TypeJobAction);
            // });

            //// Add pot into list
            //PoterManager.Instance.AddPoter(index, taskData);
        }

        public async void Worker(int workerModel, TaskData taskData, StatusCache statusCache)
        {
            if (workerModel == 0) return;
            Vector3 posHouse = Utils.ConvertStringToVector3(taskData.Position);
            List<PositionWorkerBuilding> posFanceHouse = taskData.PositionWorkerBuildings;
            for (int i = 0; i < workerModel; i++)
            {
                if (i != 0) await Task.Delay(TimeSpan.FromMilliseconds(_masterDataStore.GetPopulationWorkerTimeComeOut()));
                CharacterTaskData worker = GetCharacter(Character.CharacterList.Farmer);
                worker.BuildingObjectId = taskData.BuildingObjectId;
                PositionWorkerBuilding position = Utils.GetPositionWorkerFance(posFanceHouse);
                Vector3 rotation = new Vector3(worker.CharacterBehaviour.transform.localRotation.x, position.RotationY,
                         worker.CharacterBehaviour.transform.localRotation.z);
                var posWorkerHouse = Utils.GetWorkerHouse();

                if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                {
                    int level = (int)TypeJobActionBuilding.CONTRUCTION;
                    if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
                        level = (int)TypeJobActionBuilding.UPGRADE;
                    float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(taskData.BuildingId, level);
                    if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                        workload = VariableManager.Instance.ProductFormulaList.GetTimeCost(taskData.CurrentProductId);
                    // work in progress
                    if (taskData.Workload > 0 && taskData.Workload < workload)
                    {
                        worker.CharacterBehaviour.transform.localPosition = posHouse;
                        worker.CharacterBehaviour.IsMove = false;
                        worker.CharacterBehaviour.Action(StatusAction.Idle);
                        CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(worker);
                    }
                    else
                        MoveWorkerHouseToTarget(posWorkerHouse, taskData, worker, rotation, position);
                }
                else
                {
                    int level = (int)TypeJobActionBuilding.CONTRUCTION;
                    if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
                        level = (int)TypeJobActionBuilding.UPGRADE;
                    float workload = VariableManager.Instance.BuildingUpgradeInfoList.GetTimeCost(taskData.BuildingId, level);
                    if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                        workload = VariableManager.Instance.ProductFormulaList.GetTimeCost(taskData.CurrentProductId);
                    // work in progress
                    if (taskData.WorkloadDone > 9 && taskData.WorkloadDone < workload)
                    {
                        SetWorkerPositionAtWork(posWorkerHouse, taskData, worker, rotation, position);
                    }
                    else
                        MoveWorkerHouseToTarget(posWorkerHouse, taskData, worker, rotation, position);
                }
            }
        }

        // Worker go to harvest
        public void OnUpdateCharacterInfo(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            CharacterTaskData characterTaskData = (CharacterTaskData)eventParam[0];
            var characterItem = _characterTaskData.Find(t => t.CharacterId == characterTaskData.CharacterId);
            if (characterItem != null)
                characterItem = characterTaskData;
        }


        //===================================== move ============================

        private void MoveWorkerHouseToTarget(BuildingMain posWorkerHouse, TaskData taskData, CharacterTaskData worker, Vector3 rotation, PositionWorkerBuilding position)
        {
            if (posWorkerHouse == null) return;
            if (taskData.TypeJobAction == TypeJobAction.CONTRUCTION || taskData.TypeJobAction == TypeJobAction.UPGRADE || taskData.TypeJobAction == TypeJobAction.DESTROY)
            {
                var buildingOperation = Utils.GetBuildingOperation(posWorkerHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                Vector3 posDoorHouse = buildingOperation.GetDoorPosition();
                worker.CharacterBehaviour.transform.localPosition = Utils.GetInDoorPosition(posDoorHouse);
                buildingOperation.OpenDoor(0);
                worker.CharacterBehaviour.transform.rotation = Utils.GetRotation(Utils.GetOutDoorPosition(posDoorHouse), worker);
                worker.CharacterBehaviour.MoveOutDoor(rotation, 0.75f, Utils.GetOutDoorPosition(posDoorHouse), (dataStore, rotation) =>
                {
                    buildingOperation.CloseDoor(0);
                    worker.CharacterBehaviour.MovePositionBuild(rotation, 0.5f, position.Position, (dataStore, rotation) =>
                    {
                        worker.CharacterBehaviour.transform.eulerAngles = rotation;
                        int randomAnim = UnityEngine.Random.Range(0, 2);
                        switch (randomAnim)
                        {
                            case 0:
                            case 2:
                                worker.StatusAction = StatusAction.Work00;
                                break;
                            case 1:
                                worker.StatusAction = StatusAction.Work01;
                                break;
                        }
                        worker.CharacterBehaviour.Action(worker.StatusAction);
                        ChangePositionBuilding(taskData, worker);
                    }, true);
                });
            }
            else
            {
                var buildingOperation = Utils.GetBuildingOperation(posWorkerHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                Vector3 posDoorHouse = buildingOperation.GetDoorPosition();
                worker.CharacterBehaviour.transform.localPosition = Utils.GetInDoorPosition(posDoorHouse);
                buildingOperation.OpenDoor(0);
                worker.CharacterBehaviour.MoveOutDoor(Vector3.zero, 1.5f, Utils.GetOutDoorPosition(posDoorHouse), (dataStore, rotation) =>
                {
                    Vector3 posDoorHouse = Utils.ConvertStringToVector3(taskData.RotationDoor);
                    Vector3 posInDoorBuilding = Utils.GetInDoorPosition(posDoorHouse);
                    Vector3 posOutDoorBuilding = Utils.GetOutDoorPosition(posDoorHouse);
                    worker.CharacterBehaviour.MoveCenter(Vector3.zero, 1.5f, posDoorHouse, async (dataStore, rotation) =>
                    {
                        // Calculate a rotation a step closer to the target and applies rotation to this object
                        worker.CharacterBehaviour.transform.rotation = Utils.GetRotation(posInDoorBuilding, worker);
                        worker.CharacterBehaviour.MoveOutDoor(Vector3.zero, 0.75f, posInDoorBuilding, (dataStore, rotation) =>
                        {
                            worker.CharacterBehaviour.IsMove = false;
                            worker.CharacterBehaviour.Action(StatusAction.Idle);
                            CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(worker);

                        });
                    });
                });
            }
        }

        public void SetWorkerPositionAtWork(BuildingMain posWorkerHouse, TaskData taskData, CharacterTaskData worker, Vector3 rotation, PositionWorkerBuilding position)
        {
            var buildingOperation = Utils.GetBuildingOperation(posWorkerHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
            Vector3 posDoorHouse = buildingOperation.GetDoorPosition();
            worker.CharacterBehaviour.transform.localPosition = position.Position;

            worker.CharacterBehaviour.transform.rotation = Utils.GetRotation(Utils.GetOutDoorPosition(posDoorHouse), worker);
            worker.CharacterBehaviour.transform.eulerAngles = rotation;
            int randomAnim = UnityEngine.Random.Range(0, 2);
            switch (randomAnim)
            {
                case 0:
                case 2:
                    worker.StatusAction = StatusAction.Work00;
                    break;
                case 1:
                    worker.StatusAction = StatusAction.Work01;
                    break;
            }
            worker.CharacterBehaviour.Action(worker.StatusAction);
            ChangePositionBuilding(taskData, worker);
        }

        public void Move(Vector3 postionTarget, Vector3 size, int buildingObjectId, int characterID)
        {
            foreach (var player in _characterTaskData)
            {
                if (player.BuildingObjectId == buildingObjectId && player.Type == Character.CharacterList.Vassar)
                {
                    player.CharacterBehaviour.Reset(true);
                    player.CharacterBehaviour.gameObject.SetActive(true);
                    player.BuildingObjectId = -1;
                    player.StatusAction = StatusAction.None;
                    var vassalData = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == player.CharacterId);
                    if (vassalData != null) vassalData.IsWorking = false;

                    player.CharacterBehaviour.Action(StatusAction.Walk);
                    player.CharacterBehaviour.SetAIStop(false);
                    player.StatusAction = StatusAction.None;
                    player.CharacterBehaviour.IsAutoMove = true;
                    player.CharacterBehaviour.MoveAutoFreedom();
                }
            }

            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            CharacterTaskData vassal = _characterTaskData.Find(t => t.CharacterId == characterID);
            if (vassal != null)
            {
                vassal.BuildingObjectId = buildingObjectId;
                StatusAction status = StatusAction.Manager00;
                int randomActionManager = UnityEngine.Random.Range(0, 1);
                if (randomActionManager == 1)
                    status = StatusAction.Manager01;
                vassal.StatusAction = status;
                vassal.StatusAction = StatusAction.Manager00;
                vassal.CharacterBehaviour.Action(StatusAction.Walk);
                if (taskData == null) return;
                vassal.CharacterBehaviour.IsAutoMove = false;
                switch (taskData.TypeJobAction)
                {
                    case TypeJobAction.DESTROY:
                    case TypeJobAction.CONTRUCTION:
                    case TypeJobAction.UPGRADE:
                        {
                            vassal.CharacterBehaviour.Move(size, postionTarget, (dataStore) =>
                            {
                                vassal.CharacterBehaviour.Action(status);
                                //ChangePositionVassal(taskData, vassal);
                            });
                        }
                        break;
                    case TypeJobAction.PRODUCE:
                        Vector3 posDoorBuilding = Utils.GetOutDoorPosition(postionTarget);
                        vassal.CharacterBehaviour.MoveCenter(Vector3.zero, ((float)taskData.Size / 3), posDoorBuilding, (dataStore, rotation) =>
                        {
                            vassal.CharacterBehaviour.MoveCenter(Vector3.zero, 0, postionTarget, (dataStore, rotation) =>
                            {
                                vassal.CharacterBehaviour.IsMove = false;
                                vassal.CharacterBehaviour.Action(StatusAction.Idle);
                                CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(vassal);
                            }, false);
                        });
                        break;
                }

            }
        }

        public async void MoveBuildCache(Vector3 postionTarget, Vector3 size, int buildingObjectId, int characterID)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(4000));
            foreach (var player in _characterTaskData)
            {
                if (player.CharacterId == characterID && player.Type == Character.CharacterList.Vassar)
                {
                    player.CharacterBehaviour.Reset(true);
                    player.CharacterBehaviour.gameObject.SetActive(true);
                    player.BuildingObjectId = -1;
                    player.StatusAction = StatusAction.None;
                    var vassalD = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == player.CharacterId);
                    if (vassalD != null) vassalD.IsWorking = false;

                    player.CharacterBehaviour.Action(StatusAction.Walk);
                    player.CharacterBehaviour.SetAIStop(false);
                    player.CharacterBehaviour.IsAutoMove = true;
                    player.CharacterBehaviour.MoveAutoFreedom();
                }
            }

            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            CharacterTaskData vassal = _characterTaskData.Find(t => t.CharacterId == characterID);
            var vassalData = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == characterID);
            if (vassal != null)
            {
                vassalData.IsWorking = true;
                vassal.BuildingObjectId = buildingObjectId;
                StatusAction status = StatusAction.Manager00;
                int randomActionManager = UnityEngine.Random.Range(0, 1);
                if (randomActionManager == 1)
                    status = StatusAction.Manager01;
                vassal.StatusAction = status;
                vassal.StatusAction = StatusAction.Manager00;
                vassal.CharacterBehaviour.Action(StatusAction.Walk);
                if (taskData == null) return;
                vassal.CharacterBehaviour.IsAutoMove = false;
                switch (taskData.TypeJobAction)
                {
                    case TypeJobAction.DESTROY:
                        {
                            vassal.CharacterBehaviour.Move(new Vector3(1.2f, 1.2f, 1.2f), postionTarget, (dataStore) =>
                            {
                                vassal.CharacterBehaviour.Action(status);
                                //ChangePositionVassal(taskData, vassal);
                            });
                        }
                        break;
                    case TypeJobAction.CONTRUCTION:
                    case TypeJobAction.UPGRADE:
                        {
                            vassal.CharacterBehaviour.Move(size, postionTarget, (dataStore) =>
                            {
                                vassal.CharacterBehaviour.Action(status);
                                //ChangePositionVassal(taskData, vassal);
                            });
                        }
                        break;
                    case TypeJobAction.PRODUCE:
                        Vector3 posDoorBuilding = Utils.GetOutDoorPosition(postionTarget);
                        vassal.CharacterBehaviour.MoveCenter(Vector3.zero, ((float)taskData.Size / 3), posDoorBuilding, (dataStore, rotation) =>
                        {
                            vassal.CharacterBehaviour.MoveCenter(Vector3.zero, 0, postionTarget, (dataStore, rotation) =>
                            {
                                vassal.CharacterBehaviour.IsMove = false;
                                vassal.CharacterBehaviour.Action(StatusAction.Idle);
                                CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(vassal);
                            }, false);
                        });
                        break;
                }

            }
        }

        //=======================================================================

        //===================================== Change position =================
        public async void ChangePositionBuilding(TaskData taskData, CharacterTaskData worker)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10000));
            if (worker.BuildingObjectId == -1) return;
            if (worker.StatusAction == StatusAction.Work00 || worker.StatusAction == StatusAction.Work01)
            {
                List<PositionWorkerBuilding> posFanceHouse = taskData.PositionWorkerBuildings;
                worker.StatusAction = StatusAction.Walk;
                worker.CharacterBehaviour.Action(worker.StatusAction);

                PositionWorkerBuilding position = Utils.GetPositionWorkerFance(posFanceHouse);
                if (worker == null || worker.CharacterBehaviour == null) return;
                Vector3 rotation = new Vector3(worker.CharacterBehaviour.transform.localRotation.x, position.RotationY,
                        worker.CharacterBehaviour.transform.localRotation.z);
                worker.CharacterBehaviour.MovePositionBuild(rotation, 0.5f, position.Position, (dataStore, rotation) =>
                {
                    worker.CharacterBehaviour.transform.eulerAngles = rotation;
                    int randomAnim = UnityEngine.Random.Range(0, 2);
                    switch (randomAnim)
                    {
                        case 0:
                        case 2:
                            worker.StatusAction = StatusAction.Work00;
                            break;
                        case 1:
                            worker.StatusAction = StatusAction.Work01;
                            break;
                    }
                    worker.CharacterBehaviour.Action(worker.StatusAction);
                    ChangePositionBuilding(taskData, worker);
                }, true);
            }

        }

        public async void ChangePositionVassal(TaskData taskData, CharacterTaskData worker)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10000));
            if (worker.StatusAction != StatusAction.Building || worker.BuildingObjectId == -1) return;
            List<PositionWorkerBuilding> posFanceHouse = taskData.PositionVassalBuildings;
            worker.StatusAction = StatusAction.Walk;
            worker.CharacterBehaviour.Action(worker.StatusAction);

            PositionWorkerBuilding position = Utils.GetPositionWorkerFance(posFanceHouse);
            Vector3 rotation = new Vector3(worker.CharacterBehaviour.transform.localRotation.x, position.RotationY,
                    worker.CharacterBehaviour.transform.localRotation.z);
            worker.CharacterBehaviour.MoveCenter(rotation, 0.5f, position.Position, (dataStore, rotation) =>
            {
                worker.CharacterBehaviour.transform.localEulerAngles = rotation;
                worker.StatusAction = StatusAction.Building;
                ChangePositionVassal(taskData, worker);
            }, true);
        }
        //=======================================================================

        //===================================== Come back =================
        public void PorterComeBack(TaskData taskData)
        {
            List<CharacterTaskData> taskList = CharacterManager.Instance.GetCharacterTaskData().FindAll(t => t.BuildingObjectId == taskData.BuildingObjectId && t.Type == CharacterList.AssistantWorker);
            for (int i = 0; i < taskList.Count; i++)
            {
                CharacterTaskData player = taskList[i];
                var posHouse = Utils.GetWorkerHouse();
                player.CharacterBehaviour.Action(StatusAction.Walk);
                var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperation.GetDoorPosition());
                Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperation.GetDoorPosition());
                Vector2 buildingSize = _buildingSO.GetBuildingSize(taskData.BuildingId);
                player.CharacterBehaviour.MoveCenter(Vector3.zero, 1.5f, posOutDoorHouse, async (dataStore, rotation) =>
                {
                    buildingOperation.OpenDoor(0);
                    player.CharacterBehaviour.IsMove = false;
                    player.CharacterBehaviour.Action(StatusAction.Idle);
                    CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(player);
                    player.BuildingObjectId = -1;
                    await UniTask.DelayFrame(10);
                    buildingOperation.CloseDoor(0);
                });
            }
        }

        public void WorkerComeBack(TaskData taskData, int workerModel, int worker)
        {
            List<CharacterTaskData> taskList = CharacterManager.Instance.GetCharacterTaskData().FindAll(t => t.BuildingObjectId == taskData.BuildingObjectId && t.Type == CharacterList.Farmer);

            for (int i = 0; i < workerModel; i++)
            {
                if (i >= taskList.Count) return;
                CharacterTaskData player = taskList[i];
                if (!player.CharacterBehaviour.gameObject.activeInHierarchy)
                    player.CharacterBehaviour.gameObject.SetActive(true);
                player.CharacterBehaviour.Reset(true);
                player.BuildingObjectId = -1;
                player.StatusAction = StatusAction.None;
                player.CharacterBehaviour.Action(StatusAction.Walk, (isComplete) =>
                {
                    if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
                    {
                        var productionDoorHouse = Utils.ConvertStringToVector3(taskData.RotationDoor);
                        var posInDoorProduction = Utils.GetInDoorPosition(productionDoorHouse);
                        var posOutDoorProduction = Utils.GetOutDoorPosition(productionDoorHouse);
                        player.CharacterBehaviour.transform.localPosition = posInDoorProduction;
                        player.CharacterBehaviour.MoveOutDoor(Vector3.zero, 0.75f, posOutDoorProduction, (dataStore, rotation) =>
                        {
                            var posHouse = Utils.GetWorkerHouse();
                            var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                            Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperation.GetDoorPosition());
                            Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperation.GetDoorPosition());
                            player.CharacterBehaviour.MoveCenter(Vector3.zero, 0.75f, posOutDoorHouse, async (dataStore, rotation) =>
                            {
                                // Calculate a rotation a step closer to the target and applies rotation to this object
                                player.CharacterBehaviour.transform.rotation = Utils.GetRotation(posInDoorHouse, player);
                                player.CharacterBehaviour.MoveOutDoor(Vector3.zero, 0, posInDoorHouse, (dataStore, rotation) =>
                                {
                                    player.CharacterBehaviour.IsMove = false;
                                    player.CharacterBehaviour.Action(StatusAction.Idle);
                                    CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(player);
                                });
                            });
                        });
                    }
                    else
                    {
                        var posHouse = Utils.GetWorkerHouse();
                        var buildingOperation = Utils.GetBuildingOperation(posHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                        Vector3 posInDoorHouse = Utils.GetInDoorPosition(buildingOperation.GetDoorPosition());
                        Vector3 posOutDoorHouse = Utils.GetOutDoorPosition(buildingOperation.GetDoorPosition());
                        player.CharacterBehaviour.MoveCenter(Vector3.zero, ShareUIManager.Instance.Config.DURATION_DOOR_WORKERHOUSE_PORTER, posOutDoorHouse, async (dataStore, rotation) =>
                        {
                            // Calculate a rotation a step closer to the target and applies rotation to this object
                            player.CharacterBehaviour.transform.rotation = Utils.GetRotation(posInDoorHouse, player);
                            player.CharacterBehaviour.MoveOutDoor(Vector3.zero, 0, posInDoorHouse, (dataStore, rotation) =>
                            {
                                player.CharacterBehaviour.IsMove = false;
                                player.CharacterBehaviour.Action(StatusAction.Idle);
                                CharacterManager.Instance.GetPoolWorkerCharacter().DisableObject(player);
                            });
                        });
                    }
                });
            }
        }
        //=======================================================================

        #endregion
    }
}
