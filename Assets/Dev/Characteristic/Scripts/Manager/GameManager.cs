using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Assets.Dev.Tutorial.Scripts;
using Fbs;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace CoreData.UniFlow.Common
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("Task")]
        [SerializeField]
        private GameEvent _onAddTaskData;

        [SerializeField]
        private GameEvent _onStartTaskData;

        [Header("Building")]
        [SerializeField]
        private Vector3Variable _newBuildingLocation;

        [SerializeField]
        private IntegerVariable _selectedBuildingObjId;

        [SerializeField]
        private GameEvent _onUpdatePositionResourcce;

        [SerializeField]
        private GameEvent _onBuidingChangedToDestructed;

        [SerializeField]
        private GameEvent _onConfirmedNewBuilding;

        [SerializeField]
        private IntegerVariable _newBuildingTopLeftTileId;

        [SerializeField]
        private GameEvent _onClickedOpenWorkerPanel;

        [SerializeField]
        private GameEvent _onClickedOpenMilitaryPanel;

        [SerializeField]
        private GameEvent _onOpenChooseVassalPopup;

        [SerializeField]
        private GameEvent _onDestroyedBuilding;

        [SerializeField]
        private GameEvent _onStartDestroyedProgress;

        [SerializeField]
        private GameEvent _onCanceledUpgrade;

        [Header("Worker")]
        [SerializeField]
        private GameEvent _onUpdateVassalChoice;

        [SerializeField]
        private GameEvent _onChangedHumanResource;

        [SerializeField]
        private GameEvent _onChangedToWaitForUpgradeResource;

        [SerializeField]
        private GameEvent _onBuildingUpdateInfo;

        [SerializeField]
        private GameEvent _onBuildingProductionUpdateInfo;

        [SerializeField]
        private GameEvent _onUpdateWorkerInProgress;

        [SerializeField]
        private GameEvent _onRaiseCompleteFillResource;

        [SerializeField]
        private GameEvent _onRaiseCompleteFillResourceProduction;

        [SerializeField]
        private GameEvent _onUpdateNumberWorker;

        [SerializeField]
        private IntegerVariable _varBPMPriority;

        [Header("Production")]
        [SerializeField]
        private GameEvent _onStartNewProduction;

        [SerializeField]
        private GameEvent _onCanceledCurrentProduct;

        [SerializeField]
        private GameEvent _onUpdateVassalStatusBoard;

        [SerializeField]
        private GameEvent _onUpdateProductionStatusBoard;

        private int _buildingId;
        private int _buildingObjId;
        private int _onBPMPriority;

        public void SetBuildingId(int value)
        {
            _buildingId = value;
        }
        public void SetBuildingObjId(int value)
        {
            _buildingObjId = value;
        }

        public int GetOnBPMPriority()
        {
            return _onBPMPriority;
        }

        public void SetOnBPMPriority(int priority)
        {
            _onBPMPriority = priority;
        }

        public void Start()
        {
            _ = GameManager.Instance.StartGame();

            // GetTime 
            _ = APIManager.Instance.GetTime();
        }

        private void OnEnable()
        {
            _selectedBuildingObjId.OnValueChange += SelectedBuildingObjId;

            _onUpdateVassalChoice.Subcribe(OnUpdateAvatarVassal);
            _onChangedHumanResource.Subcribe(OnChangeHumanResource);
            _onOpenChooseVassalPopup.Subcribe(OpenChooseVassalPopup);
            _onClickedOpenWorkerPanel.Subcribe(SelectBuildingObjectId);
            _onUpdatePositionResourcce.Subcribe(OnUpdatePositionResource);
            _onClickedOpenMilitaryPanel.Subcribe(OnClickedOpenMilitaryPanel);

            // Task
            _onAddTaskData.Subcribe(OnAddTaskData);
            _onStartTaskData.Subcribe(OnStartTaskData);

            // Contruction
            _onConfirmedNewBuilding.Subcribe(ConfirmedNewBuilding);
            _onRaiseCompleteFillResource.Subcribe(OnRaiseCompleteFillResource);

            // Production
            _onStartNewProduction.Subcribe(OnStartNewProduction);
            _onRaiseCompleteFillResourceProduction.Subcribe(OnRaiseCompleteFillResourceProduction);

            // Destroy
            _onCanceledUpgrade.Subcribe(OnCanceledUpgrade);
            _onDestroyedBuilding.Subcribe(OnDestroyedBuilding);
            _onStartDestroyedProgress.Subcribe(OnStartDestroyedProgress);
            _onCanceledCurrentProduct.Subcribe(OnCanceledCurrentProduct);
            _onBuidingChangedToDestructed.Subcribe(OnBuidingChangedToDestructed);
            _onChangedToWaitForUpgradeResource.Subcribe(OnChangedToWaitForUpgradeResource);

            // Update
            _varBPMPriority.OnValueChange += SetOnBPMPriority;
            _onUpdateNumberWorker.Subcribe(OnUpdateNumberWorker);
            _onUpdateWorkerInProgress.Subcribe(OnUpdateWorkerInProgress);
        }

        private void OnDisable()
        {
            _selectedBuildingObjId.OnValueChange -= SelectedBuildingObjId;

            _onUpdateVassalChoice.Unsubcribe(OnUpdateAvatarVassal);
            _onChangedHumanResource.Unsubcribe(OnChangeHumanResource);
            _onOpenChooseVassalPopup.Unsubcribe(OpenChooseVassalPopup);
            _onClickedOpenWorkerPanel.Unsubcribe(SelectBuildingObjectId);
            _onUpdatePositionResourcce.Unsubcribe(OnUpdatePositionResource);
            _onClickedOpenMilitaryPanel.Unsubcribe(OnClickedOpenMilitaryPanel);

            // Task
            _onAddTaskData.Unsubcribe(OnAddTaskData);
            _onStartTaskData.Unsubcribe(OnStartTaskData);

            // Contruction
            _onConfirmedNewBuilding.Unsubcribe(ConfirmedNewBuilding);
            _onRaiseCompleteFillResource.Unsubcribe(OnRaiseCompleteFillResource);

            // Production
            _onStartNewProduction.Subcribe(OnStartNewProduction);
            _onRaiseCompleteFillResourceProduction.Subcribe(OnRaiseCompleteFillResourceProduction);

            // Destroy
            _onCanceledUpgrade.Unsubcribe(OnCanceledUpgrade);
            _onDestroyedBuilding.Unsubcribe(OnDestroyedBuilding);
            _onStartDestroyedProgress.Unsubcribe(OnStartDestroyedProgress);
            _onCanceledCurrentProduct.Unsubcribe(OnCanceledCurrentProduct);
            _onBuidingChangedToDestructed.Unsubcribe(OnBuidingChangedToDestructed);
            _onChangedToWaitForUpgradeResource.Unsubcribe(OnChangedToWaitForUpgradeResource);

            // Update
            _varBPMPriority.OnValueChange -= SetOnBPMPriority;
            _onUpdateNumberWorker.Unsubcribe(OnUpdateNumberWorker);
            _onUpdateWorkerInProgress.Unsubcribe(OnUpdateWorkerInProgress);
        }

        public void OnAddTaskData(object[] eventParam)
        {
            ResponseAddTaskData dataResponse = (ResponseAddTaskData)eventParam[0];
            for (int i = 0; i < dataResponse.AddTaskDataResponseLength; i++)
            {
                var data = dataResponse.AddTaskDataResponse(i).Value;
                List<ResourcePoco> commonResouce = Utils.GetResourcePocoFromServer(data);
                TypeJobAction typeJob = Utils.GetTypeJobByName(data.Name);
                var timeBegin = 0;
                List<PositionWorkerBuilding> positionWorkerBuilding = Utils.GetPositionWorkerBuilding(data);
                List<PositionWorkerBuilding> vassal = Utils.GetPositionVassalBuilding(data);
                var taskData = new TaskData(data.IdTaskData, data.IdJob, data.Name, data.BuildingId, data.BuildingObjectId,
                    data.Position, data.RotationDoor, commonResouce,
                    (StatusFillResource)data.StatusFillResource, data.Worker, data.Model, typeJob, timeBegin, 0,
                    data.Workload, data.Size, data.Priority, 0,
                    positionWorkerBuilding, vassal, data.ProductId, 0, 0);
                GameData.Instance.SavedPack.SaveData.ListTask.Add(taskData);
            }

            var dataBegin = dataResponse.AddTaskDataResponse(0).Value;
            var taskBegin = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.TaskId == dataBegin.IdTaskData);
            ResponseJobByName(taskBegin.Name, taskBegin);
        }

        public static void ResponseJobByName(string name,TaskData taskData)
        {
            switch (name)
            {
                case Utils.Construction:
                    ActionContruction.Instance.OnResponseAddTaskDataContruction(taskData);
                    break;
                case Utils.Production:
                    ActionProduction.Instance.OnResponseAddTaskDataProduction(taskData);
                    break;
                case Utils.Destroy:
                    ActionDestroy.Instance.OnResponseAddTaskDataDestroy(taskData);
                    break;
                case Utils.Upgrade:
                    ActionContruction.Instance.OnResponseAddTaskDataContruction(taskData);
                    break;
                case Utils.Fishing:
                    break;
            }
         
        }

        public void OnStartTaskData(object[] eventParam)
        {
            ResponseStartTaskData data = (ResponseStartTaskData)eventParam[0];

            switch (data.Name)
            {
                case Utils.Construction:
                    GameManager.Instance.OnStartBuild(data.BuildingObjectId);
                    break;
                case Utils.Production:
                    ActionProduction.Instance.OnStartProduction(data.BuildingObjectId);
                    break;
                case Utils.Destroy:
                    GameManager.Instance.OnStartBuild(data.BuildingObjectId);
                    break;
                case Utils.Upgrade:
                    GameManager.Instance.OnStartBuild(data.BuildingObjectId);
                    break;
                case Utils.Fishing:
                    break;
            }

        }

        public void ConfirmedNewBuilding(object[] eventParam)
        {
            var buildingId = (int)eventParam[0];
            SetBuildingId(buildingId);
            ActionContruction.Instance.ConfirmedNewBuilding(eventParam);
        }

        public void OnRaiseCompleteFillResource(object[] eventParam)
        {
            ActionContruction.Instance.OnRaiseCompleteFillResource(eventParam);
        }

        public void OnStartNewProduction(object[] eventParam)
        {
            ActionProduction.Instance.OnStartNewProduction(eventParam);
        }

        public void OnRaiseCompleteFillResourceProduction(object[] eventParam)
        {
            ActionProduction.Instance.OnRaiseCompleteFillResourceProduction(eventParam);
        }

        public void OnCanceledUpgrade(object[] eventParam)
        {
            ActionDestroy.Instance.OnCanceledUpgrade(eventParam);
        }

        public void OnDestroyedBuilding(object[] eventParam)
        {
            ActionDestroy.Instance.OnDestroyedBuilding(eventParam);
        }

        public void OnStartDestroyedProgress(object[] eventParam)
        {
            ActionDestroy.Instance.OnStartDestroyedProgress(eventParam);
        }

        public void OnCanceledCurrentProduct(object[] eventParam)
        {
            ActionDestroy.Instance.OnCanceledCurrentProduct(eventParam);
        }

        public void OnBuidingChangedToDestructed(object[] eventParam)
        {
            ActionDestroy.Instance.OnBuidingChangedToDestructed(eventParam);
        }

        public void OnChangedToWaitForUpgradeResource(object[] eventParam)
        {
            ActionDestroy.Instance.OnChangedToWaitForUpgradeResource(eventParam);
        }

        public void OnUpdateNumberWorker(object[] eventParam)
        {
            PopulationManager.Instance.WorkerDivision();
        }

        public void OnUpdateWorkerInProgress(object[] eventParam)
        {
            PopulationManager.Instance.WorkerDivision();
        }

        public async void OnStartBuildCache(TaskData taskData)
        {
            ActionContruction.Instance.OnStartBuildCache(taskData);

            await Task.Delay(TimeSpan.FromMilliseconds(4000));
            var vassal =
                GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t =>
                    t.Data.idVassalTemplate == taskData.IdVassal);
            if (vassal != null)
            {
                VassalChoiceData data = new VassalChoiceData(taskData.BuildingObjectId,
                    vassal.Data.idVassalPlayer,
                    1,
                    0,
                    0,
                    taskData.TaskId);
                _onUpdateVassalChoice.Raise(data);
            }
        }

        public async void WorkerStartProductionCache(TaskData taskData)
        {
            ActionProduction.Instance.WorkerStartProductionCache(taskData);

            await Task.Delay(TimeSpan.FromMilliseconds(4000));
            var vassal =
                GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t =>
                    t.Data.idVassalTemplate == taskData.IdVassal);
            if (vassal != null)
            {
                VassalChoiceData data = new VassalChoiceData(taskData.BuildingObjectId,
                    vassal.Data.idVassalPlayer,
                    1,
                    0,
                    0,
                    taskData.TaskId);
                _onUpdateVassalChoice.Raise(data);
            }
        }

        public void OpenChooseVassalPopup(object[] eventParam)
        {
            _buildingObjId = _selectedBuildingObjId.Value;
            ActionVassal.Instance.OpenChooseVassalPopup(_buildingObjId);
        }

        public void OnUpdateAvatarVassal(object[] eventParam)
        {
            VassalChoiceData reponse = (VassalChoiceData)eventParam[0];
            if (reponse.IsChoose == 1)
            {
                var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalPlayer == reponse.IdVassal);
                if (vassal != null)
                {
                    vassal.IsWorking = true;
                    var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == reponse.BuildingPlayerId);
                }
            }
            else if(reponse.IsRemove == 1)
            {
                var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalPlayer == reponse.IdVassal);
                if (vassal != null) vassal.IsWorking = false;
            }
       
            ActionVassal.Instance.OnUpdateAvatarVassal(eventParam);
        }

        public void OnUpdatePositionResource(object[] eventParam)
        {
            ActionVassal.Instance.OnUpdatePositionResource(eventParam);
        }

        public void OnCheckVassalAssignt(TaskData taskData)
        {
            ActionVassal.Instance.OnCheckVassalAssignt(taskData);
        }

        public void UpdateAvaterInBuilding(int buildingObjectId, int idVassal)
        {
            ActionVassal.Instance.UpdateAvaterInBuilding(buildingObjectId, idVassal);
        }

        public void GetWorker(int priority, TaskData taskData, Action<int> onComplete)
        {
            ActionVassal.Instance.GetWorker(priority, taskData, onComplete);
        }

        public void OnStartBuild(int idBuildingObject)
        {
            ActionVassal.Instance.OnStartBuild(idBuildingObject);
        }


        public void OnUpdateProductionStatusBoard()
        {
            _onUpdateProductionStatusBoard?.Raise();
        }

        public void OnBuildingUpdateInfo(UserBuilding userBuilding)
        {
            _onBuildingUpdateInfo?.Raise(userBuilding);
        }

        public void OnUpdateVassalStatusBoard(object[] eventParam)
        {
            _onUpdateVassalStatusBoard?.Raise(eventParam);
        }

        public IntegerVariable NewBuildingTopLeftTileId => _newBuildingTopLeftTileId;

        public void OnBuildingProductionUpdateInfo(UserBuildingProduction userBuildingProduction)
        {
            _onBuildingProductionUpdateInfo?.Raise(userBuildingProduction);
        }

        public async UniTaskVoid StartGame()
        {
            await UniTask.DelayFrame(1);
            JobManager.Instance.InitData();
            TownTypeTransform.Instance.SetDataBuilding();
            UIManagerTown.Instance.ShowUIGamePlay();
            TaskManager.Instance.InitTask();
            UIManagerTown.Instance.Init();
            UIManagerTown.Instance.ShowUIGamePlay();
    
            //Init Data
            if (CharacterManager.Instance != null)
                _ = CharacterManager.Instance.Init();

            // Check building cache
            await UniTask.DelayFrame(4);
            TaskManager.Instance.NextTask(VariableManager.Instance.UserBuildingList, VariableManager.Instance.UserItemStorage);

        }

        private void SelectBuildingObjectId(object[] eventParam)
        {
            if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
            {
                UIManagerTown.Instance.ShowUIPopulation(() => { },UIPopulation.TypePanel.WorkerInfo);
            }
            else
            {
                UIManagerTown.Instance.ShowUIWorkerHouse();
            }
        }

        private void OnClickedOpenMilitaryPanel(object[] eventParam)
        {
            if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
            {
                UIManagerTown.Instance.ShowUIPopulation(() => { }, UIPopulation.TypePanel.MilitaryInfo);
            }
        }
        private void SelectedBuildingObjId(int buildingId)
        {
            _buildingObjId = buildingId;
        }

        public async UniTask logout()
        {
            _ = APIManager.Instance.RequestUpdateTaskData(GameData.Instance.SavedPack.SaveData.ListTask);
            var resultLogout = await RequestDispatcher.Instance.SendGetRequest<ApiLoginResult>("http://10.10.60.233:8081/common/logout");
        }

        public void OnApplicationQuit()
        {
            logout();
            GameData.Instance.SavedPack.SaveData.WorkersWorking = 0;
            GameData.Instance.saveGameData();
        }

        public void UpdateWorking(TaskData data, int manWorker, int worker, Action<int> OnComplete)
        {
            TaskData taskData = data;
            if (taskData.Man == 0 && worker > 0)
            {
                taskData.Man = worker;
                JobManager.Instance.TriggerWorkloadDecrease(taskData);
            }
   
            int manModel = taskData.Man + manWorker;
            taskData.Man = worker;
            float newSpeed = TimeWorkLoad.SpeedRate(taskData.Man);
            taskData.SpeedRate = newSpeed;
            taskData.TimeJob = TimeWorkLoad.TimeJob(taskData.Man, taskData);
            int modelCurrent = MasterDataStore.Instance.GetCountWorker(manModel);
            int ModelRaise = modelCurrent - taskData.Model;
            taskData.Model = modelCurrent;
          
            UserBuilding userBuilding = VariableManager.Instance.UserBuildingList.GetBuilding(taskData.BuildingObjectId);
            UserBuildingProduction userBuildingProduction = VariableManager.Instance.UserBuildingProductionList.GetBuildingProduction(taskData.BuildingObjectId);

            int workloadsDone = JobManager.Instance.WorkloadsDone(taskData);

            if (workloadsDone < 1) workloadsDone = 0;
            if (taskData.TypeJobAction == TypeJobAction.CONTRUCTION)
            {
                userBuilding.currentContructionRate = taskData.SpeedRate;
                userBuilding.status = BuildingStatus.OnConstruction;
                GameManager.Instance.OnBuildingUpdateInfo(userBuilding);
            }
            else if (taskData.TypeJobAction == TypeJobAction.UPGRADE)
            {
                userBuilding.currentContructionRate = taskData.SpeedRate;
                userBuilding.status = BuildingStatus.Upgrading;
                GameManager.Instance.OnBuildingUpdateInfo(userBuilding);
            }
            else if (taskData.TypeJobAction == TypeJobAction.PRODUCE)
            {
                userBuildingProduction.productRate = taskData.SpeedRate;
                GameManager.Instance.OnBuildingProductionUpdateInfo(userBuildingProduction);
            }
            else if (taskData.TypeJobAction == TypeJobAction.DESTROY)
            {
                userBuilding.currentDestructionRate = taskData.SpeedRate;
                userBuilding.status = BuildingStatus.OnDestruction;
                GameManager.Instance.OnBuildingUpdateInfo(userBuilding);
            }
         
            if (ModelRaise == 0) return;
            if (ModelRaise > 0)
                CharacterManager.Instance.Worker(ModelRaise, taskData, StatusCache.NoCache);
            else if (ModelRaise < 0)
            {
                CharacterManager.Instance.WorkerComeBack(taskData, (ModelRaise * -1), worker);
                GameData.Instance.SavedPack.SaveData.WorkersWorking -= manWorker;
            }

            OnComplete?.Invoke(worker);
        }

        public void OnChangeHumanResource(object[] eventParam)
        {
            if (eventParam.Length == 0)
            {
                return;
            }
            int buildingObjectId = (int)eventParam[0];
            UserBuildingProduction userBuildingProduction = VariableManager.Instance.UserBuildingProductionList.GetBuildingProduction(buildingObjectId);
            userBuildingProduction.priority = GameManager.Instance.GetOnBPMPriority();
            var taskData = GameData.Instance.SavedPack.SaveData.ListTask.Find(t => t.BuildingObjectId == buildingObjectId);
            if (taskData != null)
            {
                if (taskData.Priority != userBuildingProduction.priority)
                {
                    taskData.Priority = userBuildingProduction.priority;
                    GameData.Instance.RequestSaveGame();
                    GameData.Instance.RemoveJobWorkerAssignment(taskData.TaskId);
                    GameData.Instance.AddJobWorkerAssignment(userBuildingProduction.priority, taskData.TaskId, taskData.Man, taskData.Name, taskData.TypeJobAction);
                    PopulationManager.Instance.WorkerDivision();
                }

            }
            GameManager.Instance.OnBuildingProductionUpdateInfo(userBuildingProduction);
        }
    }
}