using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Dev.Tutorial.Scripts;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using Fbs;
using Google.FlatBuffers;
using UnityEngine;
using UnityEngine.Networking;
using Action = System.Action;
using TaskData = CoreData.UniFlow.Common.TaskData;

public class APIManager : MonoSingleton<APIManager>
{
    [Header("Reference - Read")]
    [SerializeField]
    private ApiList _apis;

    [SerializeField]
    private InternalAccountList _interalAccounts;

    [SerializeField]
    private UserBuildingList _userBuildingList;

    [SerializeField]
    private GameEvent _onUpdateFillResource;

    [SerializeField]
    private GameEvent _onUpdateVassalChoice;

    [SerializeField]
    private GameEvent _onUpdateNumberWorker;

    [SerializeField]
    private GameEvent _onUpdateMilitaryWorker;

    [SerializeField]
    private GameEvent _onLogChangeWorker;

    [SerializeField]
    private GameEvent _onPopulationChangeDetail;

    [SerializeField]
    private GameEvent _onBuildingUpdateInfo;

    [SerializeField]
    private GameEvent _onStartFindMaterial;

    [SerializeField]
    private GameEvent _onUpdateSalary;

    [SerializeField]
    private GameEvent _onStartBuildingCache;

    [SerializeField]
    private GameEvent _onUpdateWorkerInProgress;

    [SerializeField]
    private GameEvent _onAddTaskData;

    [SerializeField]
    private GameEvent _onRemoveTaskData;

    [SerializeField]
    private GameEvent _onStartTaskData;

    [SerializeField]
    private GameEvent _onUpdateTaskData;

    [SerializeField]
    private GameEvent _onNextTaskData;

    [SerializeField]
    private GameEvent _onFinishBuilding;

    [SerializeField]
    private GameEvent _onPoterData;

    [SerializeField]
    private GameEvent _onFinishItem;

    [SerializeField]
    private GameEvent _onFinishUpgrade;

    [SerializeField]
    private GameEvent _onFinishDestroy;

    [SerializeField]
    private GameEvent _onTimeGame;

    [SerializeField]
    private TableWorldMapPlayer _worldPlayerData;
    [SerializeField]
    private WorldMapTroopService troopService;

    public async UniTaskVoid RequestFillResource(int buildingObjectId, int idItem, int itemCount)
    {
        var requestBody = UserFillResourceHelper.CreateFBAddFillResourceRequestBody(new AddFillResourceRequestBody()
        {
            BuildingPlayerId = buildingObjectId,
            IdItem = idItem,
            ItemCount = itemCount
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseUpdateMaterial>(_apis.FillResource, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseFillResource(response);
    }

    private void ResponseFillResource(ResponseUpdateMaterial reponse)
    {
        var idItem = reponse.IdItem;
        var itemCount = reponse.ItemCount;
        var buildingPlayerId = reponse.BuildingPlayerId;
        _onUpdateFillResource.Raise(buildingPlayerId, idItem, itemCount);
        UserBuilding _userBuilding = _userBuildingList.GetBuilding(buildingPlayerId);
        _onBuildingUpdateInfo.Raise(_userBuilding);
        _onStartFindMaterial.Raise(buildingPlayerId);
    }

    public async UniTaskVoid RequestVassalChoice(int buildingPlayerId, int idVassal,int isChoose, int isRemove,int idTask)
    {
        var requestBody = UserVassalChoiceHelper.CreateFBAddVassalChoiceRequestBody(new AddVassalChoiceRequestBody()
        {
            BuildingPlayerId = buildingPlayerId,
            IdVassal = idVassal,
            IsChoose = isChoose,
            IsRemove = isRemove,
            IdTask = idTask
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseVassalChoice>(_apis.VassalChoice, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseVassalChoice(response);
    }

    private void ResponseVassalChoice(ResponseVassalChoice reponse)
    {
        VassalChoiceData data = new VassalChoiceData(reponse.BuildingPlayerId,
            reponse.IdVassal,
            reponse.IsChoose,
            reponse.IsRemove,
            reponse.WorkloadStart,
            reponse.IdTask);
        _onUpdateVassalChoice.Raise(data);
    }

    public async UniTaskVoid RequestChangeVassalTask(int idTask, int workload,int idVassalPlayer)
    {
        var requestBody = RequestChangeVassalTaskHelper.CreateFBChangeVassalTaskRequestBody(new AddChangeVassalTaskRequestBody()
        {
            IdTask = idTask,
            Workload = workload,
            IdVassalPlayer = idVassalPlayer
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeChangeVassalTask>(_apis.ChangeVassalTask, requestBody);
        if (response.Apiresult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponeChangeVassalTask(response);
    }

    private void ResponeChangeVassalTask(ResponeChangeVassalTask reponse)
    {
        var apiresult = reponse.Apiresult;
    }

    public async UniTaskVoid RequestNumberWorker(int worker, int totalCitizen, float priority_1, float priority_2, float priority_3, float priority_4)
    {
        var requestBody = UserNumberWorkerHelper.CreateFBAddNumberWorkerRequestBody(new AddNumberWorkerRequestBody()
        {
            Worker = worker,
            TotalCitizen = totalCitizen,
            Priority1 = priority_1,
            Priority2 = priority_2,
            Priority3 = priority_3,
            Priority4 = priority_4
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseNumberWorker>(_apis.NumberWorker, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseNumberWorker(response);
    }

    private void ResponseNumberWorker(ResponseNumberWorker reponse)
    {
        var worker = reponse.Worker;
        var totalCitizen = reponse.TotalCitizen;
        var priority_1 = reponse.Priority1;
        var priority_2 = reponse.Priority2;
        var priority_3 = reponse.Prioriry3;
        var priority_4 = reponse.Priority4;
        _onUpdateNumberWorker.Raise(reponse);
    }

    public async UniTaskVoid RequestNumberMilitary(int military, int totalCitizen, int archer, int cavalry, int infantry, int military4)
    {
        var requestBody = UserNumberMilitaryHelper.CreateFBAddNumberMilitaryRequestBody(new AddNumberMilitaryRequestBody()
        {
            Military = military,
            TotalCitizen = totalCitizen,
            Archer = archer,
            Cavalry = cavalry,
            Infantry = infantry,
            Military4 = military4
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseNumberMilitary>(_apis.NumberMilitary, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseNumberMilitary(response);
    }

    private void ResponseNumberMilitary(ResponseNumberMilitary reponse)
    {
        var military = reponse.Military;
        var totalCitizen = reponse.TotalCitizen;
        //_onUpdateMilitaryWorker.Raise(military, totalCitizen);
    }

    public async UniTaskVoid RequestLogChangeWorker()
    {
        var requestBody = UserLogChangeWorkerHelper.CreateFBAddLogWorkerRequestBody();

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponePopulationLog>(_apis.PopulationLog, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponePopulationLog(response);
    }

    private void ResponePopulationLog(ResponePopulationLog response)
    {
        int count = response.ResponseLogChangeWorkerLength;
        List<ResponseLogChangeWorker> initLogs = new();
        for (int i = 0; i < count; i++)
        {
            var detailData = response.ResponseLogChangeWorker(i).Value;
            ResponseLogChangeWorker data = new()
            {
                Time = detailData.Time,
                Msg = detailData.Msg
            };

            initLogs.Add(data);
        }
        MasterDataStore.Instance.ResponseLogChangeWorkers = initLogs;
        _onLogChangeWorker.Raise();
    }

    public async UniTaskVoid RequestPopulationChangeDetail()
    {
        var requestBody = UserPopulationChangeDetailHelper.CreateFBAddPopulationChangeDeatailRequestBody();

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponsePopulationChangeDetail>(_apis.PopulationChangeDetail, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponsePopulationChangeDetail(response);
    }

    private void ResponsePopulationChangeDetail(ResponsePopulationChangeDetail response)
    {
        int countColum = response.PopulationChangeDetailColumLength;
        List<PopulationChangeDetailColum> initColums = new();
        for (int i = 0; i < countColum; i++)
        {
            var detailData = response.PopulationChangeDetailColum(i).Value;
            PopulationChangeDetailColum data = new()
            {
                DateTime = detailData.DateTime,
                Increase = detailData.Increase,
                Decrease = detailData.Decrease
            };

            initColums.Add(data);
        }

        int countTable = response.PopulationChangeDetailTableLength;
        List<PopulationChangeDetailTable> initTables = new();
        for (int i = 0; i < countTable; i++)
        {
            var detailData = response.PopulationChangeDetailTable(i).Value;
            PopulationChangeDetailTable data = new()
            {
                DateTime = detailData.DateTime,
                Total = detailData.Total,
                Citizen = detailData.Citizen,
                Worker = detailData.Worker,
                Military = detailData.Military
            };

            initTables.Add(data);
        }
        _onPopulationChangeDetail.Raise();
    }

    public async UniTaskVoid RequestSalary(float gold, float food, float goldSalaryInMonth, float foodSalaryInMonth, float perworkLoad, int mood)
    {
        var requestBody = UserSalaryHelper.CreateFBAddSalaryRequestBody(new AddSalaryRequestBody()
        {
            Gold = gold,
            Food = food,
            GoldSalaryInMonth = goldSalaryInMonth,
            FoodSalaryInMonth = foodSalaryInMonth,
            PerworkLoad = perworkLoad,
            Mood = mood
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseSalary>(_apis.SalaryWorker, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseSalary(response);
    }

    private void ResponseSalary(ResponseSalary response)
    {
        var gold = response.Gold;
        var food = response.Food;
        var goldSalaryInMonth = response.GoldSalaryInMonth;
        var foodSalaryInMonth = response.FoodSalaryInMonth;
        var perworkLoad = response.PerworkLoad;
        var mood = response.Mood;
        _onUpdateSalary.Raise(gold, food, goldSalaryInMonth, foodSalaryInMonth, perworkLoad, mood);
    }

    public async UniTaskVoid RequestStartBuildingCache(int buildingPlayerId)
    {
        var requestBody = UserStartBuildingCacheHelper.CreateFBAddVassalChoiceRequestBody(new AddStartBuildingCacheRequestBody()
        {
            BuildingPlayerId = buildingPlayerId
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseUpdateMaterial>(_apis.FillResource, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseStartBuildingCache(response);
    }

    private void ResponseStartBuildingCache(ResponseUpdateMaterial response)
    {
        var BuildingPlayerId = response.BuildingPlayerId;
        _onStartBuildingCache.Raise();
    }

    public async UniTaskVoid RequestUpdateWorkerInProgress(int priority, float percent)
    {
        var requestBody = UserUpdateWorkerInProgressHelper.CreateFBAddUpdateWorkerInProgressRequestBody(new AddUpdateWorkerInProgressRequestBody()
        {
            Priority = priority,
            Percent = percent
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseUpdateWorkerInProgress>(_apis.UpdateWorkerInProgress, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseUpdateWorkerInProgress(response);
    }

    private void ResponseUpdateWorkerInProgress(ResponseUpdateWorkerInProgress response)
    {
        var priority = response.Prioriry;
        var percent = response.Percent;
        _onUpdateWorkerInProgress.Raise(priority, percent);
    }

    public async UniTaskVoid RequestAddTaskData(
    int buildingObjectId,
    int worker,
    int model,
    float size,
    string name,
    int priority,
    float timetask,
    string position,
    long timeBegin,
    int idJob,
    string rotationDoor,
    int statusFillResource,
    float workload,
    int idVassal,
    int buildingId,
    float speedRate,
    int productId,
    List<ResourcePoco> commonResource,
    List<PositionWorkerBuilding> workerPossition,
    List<PositionWorkerBuilding> vassalPossition)
    {
        var requestBody = UserAddTaskDataHelper.CreateFBAddTaskDataRequestBody(new AddTaskDataRequestBody()
        {
            BuildingPlayerId = buildingObjectId,
            Worker = worker,
            Model = model,
            Size = size,
            Name = name,
            Priority = priority,
            Timetask = timetask,
            Position = position,
            TimeBegin = timeBegin,
            IdJob = idJob,
            RotationDoor = rotationDoor,
            StatusFillResource = statusFillResource,
            Workload = workload,
            IdVassal = idVassal,
            BuildingId = buildingId,
            SpeedRate = speedRate,
            ProductId = productId,
            CommonResource = commonResource,
            WorkerPossition = workerPossition,
            VassalPossition = vassalPossition
        }); ;

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseAddTaskData>(_apis.AddTaskData, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseAddTaskData(response);
    }

    private void ResponseAddTaskData(ResponseAddTaskData response)
    {
        _onAddTaskData.Raise(response);
    }

    public async UniTaskVoid RequestUpdateTaskData(List<TaskData> taskDatas)
    {
        var requestBody = UpdateTaskDataHelper.CreateFbUpdateTaskDataRequestBody(new UpdateTaskDataRequestBody()
        {
            TaskDataRequestBody = taskDatas
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseUpdateTaskData>(_apis.UpdateTaskData, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseUpdateTaskData(response);
    }

    private void ResponseUpdateTaskData(ResponseUpdateTaskData response)
    {
        var taskId = response.TaskDatasLength;
        _onUpdateTaskData.Raise(response);
    }

    public async UniTaskVoid RequestRemoveTaskData(int buildingPlayerId, int taskId)
    {
        var requestBody = UserRemoveTaskDataHelper.CreateFBAddRemoveTaskDataRequestBody(new AddRemoveTaskDataRequestBody()
        {
            BuildingPlayerId = buildingPlayerId,
            TaskId = taskId
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseRemoveTaskData>(_apis.RemoveTaskData, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseRemoveTaskData(response);
    }

    private void ResponseRemoveTaskData(ResponseRemoveTaskData response)
    {
        var taskId = response.TaskId;
        var buildingPlayerId = response.BuildingPlayerId;
        _onRemoveTaskData.Raise(taskId, buildingPlayerId);
    }

    public async UniTaskVoid RequestStartTaskData(TaskData taskData)
    {
        var timeB = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent;
        var timeBegin = timeB.TotalTime;
        var requestBody = UserStartTaskDataHelper.CreateFBAddStartTaskDataRequestBody(new AddNextTaskDataRequestBody()
        {
            IdTaskData = taskData.TaskId,
            BuildingObjectId = taskData.BuildingObjectId,
            Worker = taskData.Man,
            Model = taskData.Model,
            Size = taskData.Size,
            Name = taskData.Name,
            Priority = taskData.Priority,
            Timetask = taskData.TimeJob,
            Position = taskData.Position,
            TimeBegin = timeBegin,
            IdJob = taskData.IdJob,
            RotationDoor = taskData.RotationDoor,
            StatusFillResource = (int)taskData.StatusFillResource,
            Workload = taskData.Workload,
            IdVassal = taskData.IdVassal,
            BuildingId = taskData.BuildingId,
            SpeedRate = taskData.SpeedRate,
            ProductId = taskData.CurrentProductId,
            CommonResource = taskData.CommonResources,
            WorkerPossition = taskData.PositionWorkerBuildings,
            VassalPossition = taskData.PositionVassalBuildings

        }); 

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseStartTaskData>(_apis.StartTaskData, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseStartTaskData(response);
    }

    private void ResponseStartTaskData(ResponseStartTaskData response)
    {
        var taskId = response.IdTaskData;
        var buildingPlayerId = response.BuildingObjectId;
        _onStartTaskData.Raise(response);
    }


    public async UniTaskVoid RequestFinishBuilding(TaskData taskDatas)
    {
        var requestBody = UserFinishBuildingHelper.CreateFbFinishBuildingRequestBody(new FinishBuildingRequestBody()
        {
            TaskId = taskDatas.TaskId,
            BuildingPlayerId = taskDatas.BuildingObjectId,
            Worker = taskDatas.Man,
            Model = taskDatas.Model,
            Size = taskDatas.Size,
            Name = taskDatas.Name,
            Priority = taskDatas.Priority,
            Timetask = taskDatas.TimeJob,
            Position = taskDatas.Position,
            TimeBegin = taskDatas.TimeBegin,
            IdJob = taskDatas.IdJob,
            RotationDoor = taskDatas.RotationDoor,
            StatusFillResource = (int)taskDatas.StatusFillResource,
            Workload = taskDatas.Workload,
            IdVassal = taskDatas.IdVassal,
            BuildingId = taskDatas.BuildingId,
            SpeedRate = taskDatas.SpeedRate,
            ProductId = taskDatas.CurrentProductId,
            CommonResource = taskDatas.CommonResources,
            WorkerPossition = taskDatas.PositionWorkerBuildings,
            VassalPossition = taskDatas.PositionVassalBuildings
        });
        
        var response = await RequestDispatcher.Instance.SendPostRequest<ReponseFinishBuilding>(_apis.FinishBuilding, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ReponseFinishBuilding(response);
    }

    private void ReponseFinishBuilding(ReponseFinishBuilding response)
    {
        var taskId = response.TaskId;
        _onFinishBuilding.Raise(response);
    }

    public async UniTaskVoid RequestFinishDestroy(TaskData taskDatas)
    {
        var requestBody = UserFinishDestroyHelper.CreateFbFinishDestroyRequestBody(new FinishDestroyRequestBody()
        {
            TaskId = taskDatas.TaskId,
            BuildingPlayerId = taskDatas.BuildingObjectId,
            Worker = taskDatas.Man,
            Model = taskDatas.Model,
            Size = taskDatas.Size,
            Name = taskDatas.Name,
            Priority = taskDatas.Priority,
            Timetask = taskDatas.TimeJob,
            Position = taskDatas.Position,
            TimeBegin = taskDatas.TimeBegin,
            IdJob = taskDatas.IdJob,
            RotationDoor = taskDatas.RotationDoor,
            StatusFillResource = (int)taskDatas.StatusFillResource,
            Workload = taskDatas.Workload,
            IdVassal = taskDatas.IdVassal,
            BuildingId = taskDatas.BuildingId,
            SpeedRate = taskDatas.SpeedRate,
            ProductId = taskDatas.CurrentProductId,
            CommonResource = taskDatas.CommonResources,
            WorkerPossition = taskDatas.PositionWorkerBuildings,
            VassalPossition = taskDatas.PositionVassalBuildings
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ReponseFinishDestroy>(_apis.FinishDestroy, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ReponseFinishDestroy(response);
    }

    private void ReponseFinishDestroy(ReponseFinishDestroy response)
    {
        var taskId = response.TaskId;
        _onFinishDestroy.Raise(response);
    }

    public async UniTaskVoid RequestFinishItem(TaskData taskDatas)
    {
        var requestBody = UserFinishItemHelper.CreateFbFinishItemRequestBody(new FinishItemRequestBody()
        {
            TaskId = taskDatas.TaskId,
            BuildingPlayerId = taskDatas.BuildingObjectId,
            Worker = taskDatas.Man,
            Model = taskDatas.Model,
            Size = taskDatas.Size,
            Name = taskDatas.Name,
            Priority = taskDatas.Priority,
            Timetask = taskDatas.TimeJob,
            Position = taskDatas.Position,
            TimeBegin = taskDatas.TimeBegin,
            IdJob = taskDatas.IdJob,
            RotationDoor = taskDatas.RotationDoor,
            StatusFillResource = (int)taskDatas.StatusFillResource,
            Workload = taskDatas.Workload,
            IdVassal = taskDatas.IdVassal,
            BuildingId = taskDatas.BuildingId,
            SpeedRate = taskDatas.SpeedRate,
            ProductId = taskDatas.CurrentProductId,
            CommonResource = taskDatas.CommonResources,
            WorkerPossition = taskDatas.PositionWorkerBuildings,
            VassalPossition = taskDatas.PositionVassalBuildings
        });
        var response = await RequestDispatcher.Instance.SendPostRequest<ReponseFinishItem>(_apis.FinishItem, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ReponseFinishItem(response);
    }

    private void ReponseFinishItem(ReponseFinishItem response)
    {
        var taskId = response.TaskId;
        _onFinishItem.Raise(response);
    }

    public async UniTaskVoid RequestFinishUpgrade(TaskData taskDatas)
    {
        var requestBody = UserFinishUpgradeHelper.CreateFbFinishUpgradeRequestBody(new FinishUpgradeRequestBody()
        {
            TaskId = taskDatas.TaskId,
            BuildingPlayerId = taskDatas.BuildingObjectId,
            Worker = taskDatas.Man,
            Model = taskDatas.Model,
            Size = taskDatas.Size,
            Name = taskDatas.Name,
            Priority = taskDatas.Priority,
            Timetask = taskDatas.TimeJob,
            Position = taskDatas.Position,
            TimeBegin = taskDatas.TimeBegin,
            IdJob = taskDatas.IdJob,
            RotationDoor = taskDatas.RotationDoor,
            StatusFillResource = (int)taskDatas.StatusFillResource,
            Workload = taskDatas.Workload,
            IdVassal = taskDatas.IdVassal,
            BuildingId = taskDatas.BuildingId,
            SpeedRate = taskDatas.SpeedRate,
            ProductId = taskDatas.CurrentProductId,
            CommonResource = taskDatas.CommonResources,
            WorkerPossition = taskDatas.PositionWorkerBuildings,
            VassalPossition = taskDatas.PositionVassalBuildings
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ReponseFinishUpgrade>(_apis.FinishUpgrade, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ReponseFinishUpgrade(response);
    }

    private void ReponseFinishUpgrade(ReponseFinishUpgrade response)
    {
        var taskId = response.TaskId;
        _onFinishUpgrade.Raise(response);
    }



    public async UniTaskVoid RequestPoterData(PoterDataRequestBody poterDataRequestBodys)
    {
        var requestBody = PoterDataHelper.CreateFbpoterDataRequestBody(new PoterDataRequestBody()
        {
           TaskId  = poterDataRequestBodys.TaskId,
           BuildingPlayerId = poterDataRequestBodys.BuildingPlayerId,
           Priority = poterDataRequestBodys.Priority,
           IdJob = poterDataRequestBodys.IdJob,
           CommonResource = poterDataRequestBodys.CommonResource,
           PosterPositions = poterDataRequestBodys.PosterPositions
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ReponsePoterData>(_apis.PoterData, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ReponsePoterData(response);
    }

    private void ReponsePoterData(ReponsePoterData response)
    {
        var apiResult = response.ApiResult;
        _onPoterData.Raise(response);
    }


    public async UniTaskVoid RequestTimeGame()
    {
        var requestBody = UserTimeGameHelper.CreateFBTimeGameRequestBody();

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponseTimeGame>(_apis.TimeGame, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseTimeGame(response);
    }

    private void ResponseTimeGame(ResponseTimeGame response)
    {
        var hour = response.Hour;
        var minutes = response.Minutes;
        var month = response.Month;
        var nameMonth = response.NameMonth;
        var nameSeason = response.NameSeason;
        var year = response.Year;
        _onTimeGame.Raise(hour, minutes, month, nameMonth, nameSeason, year);
    }

    public async UniTaskVoid RequestLeaveWorldMap(long uid)
    {
        var requestBody = LeaveWorldMapHelper.CreateLeaveWorldMapRequestBody(new AddleaveWorldMapRequestBody()
        {
            Uid = uid
        });

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeRemoveWorldMap>(_apis.LeaveWorldMap, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseLeaveWorldMap(response);
    }

    private void ResponseLeaveWorldMap(ResponeRemoveWorldMap response)
    {
        var ApiResult = response.ApiResult;
    }

    public async UniTaskVoid RequestJoinWorldMap()
    {
        var requestBody = JoinWorldMapHelper.CreateJoinWorldMapHelperRequestBody();

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeJoinWorldMap>(_apis.JoinWorldMap, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS) {
            return;
        }
        ResponeJoinWorldMap(response);
    }

    private void ResponeJoinWorldMap(ResponeJoinWorldMap response)
    {
        var ApiResult = response.ApiResult;
        // if Tutorial
        if (TutorialData.Instance.TutorialTracker.IsNeedTutorial())
            ResponeJoinWorldMapTutorial(response);
        else
            ResponeJoinWorldMapNormal(response);
    }

    private void ResponeJoinWorldMapTutorial(ResponeJoinWorldMap response)
    {
        var ApiResult = response.ApiResult;
        
        int count = response.PlayerWorldMapLength;
        Debug.Log(" ---- " + count);
        troopService.Clear();
        _worldPlayerData.Clear();

        List<PlayerInWorldMap> initPlayer = new();
        for (int i = 0; i < count; i++)
        {
            var detailData = response.PlayerWorldMap(i).Value;
            if (_worldPlayerData.Exists(detailData.Uid))
            {
                Debug.Log("Dup Player ==== " + detailData.Uid + " Skip");
                continue;
            }



            WorldMapPlayer p = new()
            {
                Uid = detailData.Uid,
                IdLand = detailData.IdLand,
                PlayerName = detailData.PlayerName,
                Position = detailData.Possition,
                Status = detailData.Status,
            };

            _worldPlayerData.Set(p);

            long[] troops = new long[detailData.DataPlayerTroopsLength];

            for (int j = 0; j < troops.Length; j++)
            {
                var troopData = detailData.DataPlayerTroops(j).Value;
                if (troopService.troopRepo.Exists(troopData.IdTroop))
                {
                    var dupTroop = troopService.troopRepo.GetByKey(troopData.IdTroop);
                    Debug.Log("Server send dup============= " + troopData.IdTroop + " " + p.Uid + " " + dupTroop.idPlayer);
                }
                WorldMapTroop troop = new WorldMapTroop()
                {
                    IdTroop = troopData.IdTroop,
                    Vassal_1 = troopData.Vassal1,
                    Vassal_2 = troopData.Vassal2,
                    Vassal_3 = troopData.Vassal3,

                    Buff_1 = troopData.Buff1,
                    Buff_2 = troopData.Buff2,
                    Buff_3 = troopData.Buff3,
                    Buff_4 = troopData.Buff4,

                    Attack_1 = troopData.Attack1,
                    Attack_2 = troopData.Attack2,
                    Attack_3 = troopData.Attack3,
                    Attack_4 = troopData.Attack4,
                    Attack_5 = troopData.Attack5,

                    Value_Archer = troopData.ValueArcher,
                    Value_Cavalry = troopData.ValueCavalry,
                    Value_Infantry = troopData.ValueInfantry,

                    Pawns = troopData.Pawn,
                    Status = troopData.Status,
                    IdType = troopData.IdType,

                    idPlayer = p.Uid,
                    Position = troopData.Possition,
                    TargetPosition = troopData.TargetCity,
                    StartTime = troopData.TimeStartMove
                };

                troops[j] = troop.IdTroop;
                troopService.Save(troop);
            }

            p.Troops = troops;
        }
        
    }

    private void ResponeJoinWorldMapNormal(ResponeJoinWorldMap response)
    {
        var ApiResult = response.ApiResult;

        int count = response.PlayerWorldMapLength;
        Debug.Log(" ---- " + count);
        troopService.Clear();
        _worldPlayerData.Clear();

        List<PlayerInWorldMap> initPlayer = new();
        for (int i = 0; i < count; i++)
        {
            var detailData = response.PlayerWorldMap(i).Value;
            if (_worldPlayerData.Exists(detailData.Uid))
            {
                Debug.Log("Dup Player ==== " + detailData.Uid + " Skip");
                continue;
            }



            WorldMapPlayer p = new()
            {
                Uid = detailData.Uid,
                IdLand = detailData.IdLand,
                PlayerName = detailData.PlayerName,
                Position = detailData.Possition,
                Status = detailData.Status,
            };

            _worldPlayerData.Set(p);

            long[] troops = new long[detailData.DataPlayerTroopsLength];

            for (int j = 0; j < troops.Length; j++)
            {
                var troopData = detailData.DataPlayerTroops(j).Value;
                if (troopService.troopRepo.Exists(troopData.IdTroop))
                {
                    var dupTroop = troopService.troopRepo.GetByKey(troopData.IdTroop);
                    Debug.Log("Server send dup============= " + troopData.IdTroop + " " + p.Uid + " " + dupTroop.idPlayer);
                }
                WorldMapTroop troop = new WorldMapTroop()
                {
                    IdTroop = troopData.IdTroop,
                    Vassal_1 = troopData.Vassal1,
                    Vassal_2 = troopData.Vassal2,
                    Vassal_3 = troopData.Vassal3,

                    Buff_1 = troopData.Buff1,
                    Buff_2 = troopData.Buff2,
                    Buff_3 = troopData.Buff3,
                    Buff_4 = troopData.Buff4,

                    Attack_1 = troopData.Attack1,
                    Attack_2 = troopData.Attack2,
                    Attack_3 = troopData.Attack3,
                    Attack_4 = troopData.Attack4,
                    Attack_5 = troopData.Attack5,

                    Value_Archer = troopData.ValueArcher,
                    Value_Cavalry = troopData.ValueCavalry,
                    Value_Infantry = troopData.ValueInfantry,

                    Pawns = troopData.Pawn,
                    Status = troopData.Status,
                    IdType = troopData.IdType,

                    idPlayer = p.Uid,
                    Position = troopData.Possition,
                    TargetPosition = troopData.TargetCity,
                    StartTime = troopData.TimeStartMove
                };

                troops[j] = troop.IdTroop;
                troopService.Save(troop);
            }

            p.Troops = troops;
        }

    }

    public async UniTaskVoid SetAttackTroopRequest(int playerAttack, int troopAttack , int playerEnemy , int troopEnemy, int idLand)
    {
        FlatBufferBuilder builder = new(1024);
        var offset = RequestStartBattleWar.CreateRequestStartBattleWar(
                            builder,
                            Fb.WorldMap.BATTLE_START,
                            playerAttack,
                            troopAttack,
                            playerEnemy,
                            troopEnemy,
                            idLand);
        builder.Finish(offset.Value);

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeStartBalltleWar>(_apis.StartBattle, builder.SizedByteArray());
        if (response.ApiResult != (int)ApiResultCode.SUCCESS) {
            return;
        }
        ResponeStartBalltleWar(response);
    }

    private void ResponeStartBalltleWar(ResponeStartBalltleWar response)
    {
        var ApiResult = response.ApiResult;
        //var player = response.Competitor;
        //var timeMatch = response.TimeMatch;
    }

    public async UniTaskVoid SendRequestMoveInWorldMap(int[] troops , int city)
    {
        FlatBufferBuilder builder = new(1024);
        var offset = RequestMoveInWorldMap.CreateRequestMoveInWorldMap(
                            builder,
                            Fb.WorldMap.PLAYER_MOVE,
                            StatesGlobal.UID_PLAYER,
                            RequestMoveInWorldMap.CreateIdTroopsVector(builder, troops),
                            city
            );
        builder.Finish(offset.Value);

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeMoveInWorldMap>(_apis.MoveWorldMap, builder.SizedByteArray());
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponeMoveInWorldMap(response);
    }

    private void ResponeMoveInWorldMap(ResponeMoveInWorldMap response)
    {
        //Do nothing we receive info from socket
    }
    
    public async UniTaskVoid RequestPlayerTroopInfo(List<TroopData> troopDatas)
    {
        List<PlayerTroopInfoBody> datas = new List<PlayerTroopInfoBody>();
        for (int i = 0; i < troopDatas.Count; i++)
        {
            PlayerTroopInfoBody data = new PlayerTroopInfoBody();
            data.IdTroop = troopDatas[i].IdTroop;
            data.Vassal_1 = troopDatas[i].Vassal.IdVassal_1;
            data.Vassal_2 = troopDatas[i].Vassal.IdVassal_2;
            data.Vassal_3 = troopDatas[i].Vassal.IdVassal_3;
            data.Attack_1 = troopDatas[i].Vassal.Percent;
            data.Attack_2 = troopDatas[i].Vassal.Strong;
            data.Attack_3 = troopDatas[i].Vassal.Fire;
            data.Attack_4 = troopDatas[i].Vassal.Attack;
            data.Attack_5 = troopDatas[i].Vassal.Balance;

            data.Value_Infantry = troopDatas[i].Band[0].Proficiency_1;
            data.Value_Cavalry = troopDatas[i].Band[0].Proficiency_2;
            data.Value_Archer = troopDatas[i].Band[0].Proficiency_3;
            data.Pawns = troopDatas[i].Pawns;
            data.Status = troopDatas[i].Buff.IsActive;
            data.Buff_1 = troopDatas[i].Buff.Buff_1;
            data.Buff_2 = troopDatas[i].Buff.Buff_2;
            data.Buff_3 = troopDatas[i].Buff.Buff_3;
            data.Buff_4 = troopDatas[i].Buff.Buff_4;
            data.idType = troopDatas[i].IdType;
            datas.Add(data);
        }
        var requestBody = PlayerTroopInfoHelper.CreateFBPlayerTroopInfoRequestBody(datas);

        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeUpdateTroops>(_apis.UpdateTroops, requestBody);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponsePlayerTroopInfo(response);
    }

    private void ResponsePlayerTroopInfo(ResponeUpdateTroops response)
    {
        ResponeUpdateTroops data = response;
    }

    public async UniTask GetTime()
    {
        var response = await RequestDispatcher.Instance.SendGetRequest<ResponseGetSeasonTime>(_apis.TimeInGame);
        if (response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            return;
        }
        ResponseGetSeasonTime(response);
    }

    private void ResponseGetSeasonTime(ResponseGetSeasonTime response)
    {
        SeasonTime data = new SeasonTime(response.Time.Value.IdSeasonTime,
            response.Time.Value.SpeedScaleRation,
            response.Time.Value.Ratio,
            response.Time.Value.Year,
            response.Time.Value.Month,
            response.Time.Value.Day,
            response.Time.Value.Hour,
            response.Time.Value.Minutes,
            response.Time.Value.Second,
            response.Time.Value.KingdomEra,
            response.Time.Value.TotalTime);
            GameData.Instance.SavedPack.SaveData.SetTimer(data);
    }

}
public class PopulationChangeDetailColum
{
    public long DateTime;
    public int Increase;
    public int Decrease;
}

public class PopulationChangeDetailTable
{
    public long DateTime;
    public int Total;
    public int Citizen;
    public int Worker;
    public int Military;
}

public class ResponseLogChangeWorker
{
    public long Time;
    public string Msg;
}