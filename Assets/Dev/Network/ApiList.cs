using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Uniflow/Network/Api List", fileName = "ApiList")]
public class ApiList : ScriptableObject
{
    [SerializeField]
    private NetworkEnvironment _environment;

    [Header("Admin")]
    [SerializeField]
    private string _initData;
    public string InitData => CreateFullUrl(_initData);

    [Header("Common")]
    [SerializeField]
    private string _login;
    public string Login => CreateFullUrl(_login);
    [SerializeField]
    private string _logout;
    public string Logout => CreateFullUrl(_logout);
    [SerializeField]
    private string _chooseLand;
    public string ChooseLand => CreateFullUrl(_chooseLand);
    [SerializeField]
    private string _masterData;
    public string MasterData => CreateFullUrl(_masterData);

    [Header("Time")]
    [SerializeField]
    private string _timeGame;
    public string TimeGame => CreateFullUrl(_timeGame);

    [SerializeField]
    private string _timeInGame;
    public string TimeInGame => CreateFullUrl(_timeInGame);

    [Header("Time")]
    [SerializeField]
    private string _leaveWorldMap;
    public string LeaveWorldMap => CreateFullUrl(_leaveWorldMap);

    [Header("Population")]
    [SerializeField]
    private string _populationLog;
    public string PopulationLog => CreateFullUrl(_populationLog);

    [SerializeField]
    private string _salaryWorker;
    public string SalaryWorker => CreateFullUrl(_salaryWorker);

    [SerializeField]
    private string _vassalChoice;
    public string VassalChoice => CreateFullUrl(_vassalChoice);

    [SerializeField]
    private string _changeVassalTask;
    public string ChangeVassalTask => CreateFullUrl(_changeVassalTask);

    [Header("Worker")]
    [SerializeField]
    private string _fillresource;
    public string FillResource => CreateFullUrl(_fillresource);

    [SerializeField]
    private string _poterData;
    public string PoterData => CreateFullUrl(_poterData);

    [SerializeField]
    private string _numberWorker;
    public string NumberWorker => CreateFullUrl(_numberWorker);

    [SerializeField]
    private string _numberMilitary;
    public string NumberMilitary => CreateFullUrl(_numberMilitary);

    [SerializeField]
    private string _populationChangeDetail;
    public string PopulationChangeDetail => CreateFullUrl(_populationChangeDetail);

    [SerializeField]
    private string _updateWorkerInProgress;
    public string UpdateWorkerInProgress => CreateFullUrl(_updateWorkerInProgress);

    [Header("WorldMap")]
    [SerializeField]
    private string _joinWorldMap;
    public string JoinWorldMap => CreateFullUrl(_joinWorldMap);

    [SerializeField]
    private string _moveWorldMap;
    public string MoveWorldMap => CreateFullUrl(_moveWorldMap);

    [SerializeField]
    private string _startBattle;
    public string StartBattle => CreateFullUrl(_startBattle);

    [Header("Kingdom")]
    [SerializeField]
    private string _fetchKingdomInfo;
    public string FetchKingdomInfo => CreateFullUrl(_fetchKingdomInfo);

    [Header("Land")]
    [SerializeField]
    private string _fetchLandInfo;
    public string FetchLandInfo => CreateFullUrl(_fetchLandInfo);

    [Header("JobData")]
    [SerializeField]
    private string _addTaskData;
    public string AddTaskData => CreateFullUrl(_addTaskData);

    [SerializeField]
    private string _removeTaskData;
    public string RemoveTaskData => CreateFullUrl(_removeTaskData);

    [SerializeField]
    private string _startTaskData;
    public string StartTaskData => CreateFullUrl(_startTaskData);

    [SerializeField]
    private string _updateTaskData;
    public string UpdateTaskData => CreateFullUrl(_updateTaskData);

    [SerializeField]
    private string _finishBuilding;
    public string FinishBuilding => CreateFullUrl(_finishBuilding);

    [SerializeField]
    private string _finishItem;
    public string FinishItem => CreateFullUrl(_finishItem);

    [SerializeField]
    private string _finishUpgrade;
    public string FinishUpgrade => CreateFullUrl(_finishUpgrade);

    [SerializeField]
    private string _finishDestroy;
    public string FinishDestroy => CreateFullUrl(_finishDestroy);

    [Header("Building")]
    [SerializeField]
    private string _getUserBuildings;
    public string GetUserBuilding => CreateFullUrl(_getUserBuildings);
    [SerializeField]
    private string _rotateBuilding;
    public string RotateBuilding => CreateFullUrl(_rotateBuilding);

    [Header("Building Construction")]
    [SerializeField]
    private string _addNewBuilding;
    public string AddNewBuilding => CreateFullUrl(_addNewBuilding);
    [SerializeField]
    private string _startBuildingConstruction;
    public string StartBuildingConstruction => CreateFullUrl(_startBuildingConstruction);
    [SerializeField]
    private string _cancelBuildingConstruction;
    public string CancelBuildingConstruction => CreateFullUrl(_cancelBuildingConstruction);
    [SerializeField]
    private string _completeConstruction;
    public string CompleteConstruction => CreateFullUrl(_completeConstruction);

    [Header("Building Upgrade")]
    [SerializeField]
    private string _upgradeBuilding;
    public string UpgradeBuilding => CreateFullUrl(_upgradeBuilding);
    [SerializeField]
    private string _startUpgradingBuilding;
    public string StartUpgradingBuilding => CreateFullUrl(_startUpgradingBuilding);
    [SerializeField]
    private string _completeUpgrade;
    public string CompleteUpgrade => CreateFullUrl(_completeUpgrade);
    [SerializeField]
    private string _cancelUpgradeBuilding;
    public string CancelUpgradeBuilding => CreateFullUrl(_cancelUpgradeBuilding);

    [Header("Building Destroy")]
    [SerializeField]
    private string _startDestroyBuilding;
    public string StartDestroyBuilding => CreateFullUrl(_startDestroyBuilding);
    [SerializeField]
    private string _cancelDestroyingBuilding;
    public string CancelDestroyingBuilding => CreateFullUrl(_cancelDestroyingBuilding);
    [SerializeField]
    private string _finishDestroyBuildingProgress;
    public string FinishDestroyBuildingProgress => CreateFullUrl(_finishDestroyBuildingProgress);
    [SerializeField]
    private string _completelyRemoveBuilding;
    public string CompletelyRemoveBuilding => CreateFullUrl(_completelyRemoveBuilding);

    [Header("Building Production")]
    [SerializeField]
    private string _getProductionInfo;
    public string GetProductionInfo => CreateFullUrl(_getProductionInfo);
    [SerializeField]
    private string _queueAProduct;
    public string QueueAProduct => CreateFullUrl(_queueAProduct);
    [SerializeField]
    private string _queueAsManyAsProductsPossible;
    public string QueueAsManyAsProductsPossible => CreateFullUrl(_queueAsManyAsProductsPossible);
    [SerializeField]
    private string _removeAQueuedProduct;
    public string RemoveAQueueProduct => CreateFullUrl(_removeAQueuedProduct);
    [SerializeField]
    private string _cancelCurrentProduct;
    public string CancelCurrentProduct => CreateFullUrl(_cancelCurrentProduct);
    [SerializeField]
    private string _queueRepeat;
    public string QueueRepeat => CreateFullUrl(_queueRepeat);
    [SerializeField]
    private string _changeQueueOrder;
    public string ChangeQueueOrder => CreateFullUrl(_changeQueueOrder);
    [SerializeField]
    private string _queueAllSlots;
    public string QueueAllShots => CreateFullUrl(_queueAllSlots);
    [SerializeField]
    private string _updateBuildingProduction;
    public string UpdateBuildingProduction => CreateFullUrl(_updateBuildingProduction);
    [SerializeField]
    private string _completeAProduction;
    public string CompleteAProduction => CreateFullUrl(_completeAProduction);

    [Header("Inventory")]
    [SerializeField]
    private string _getInventory;
    public string GetInventory => CreateFullUrl(_getInventory);

    [Header("Troop")]
    [SerializeField]
    private string _updateTroops;
    public string UpdateTroops => CreateFullUrl(_updateTroops);

    [Header("Trade")]
    [SerializeField]
    private string _sellAPack;
    public string SellAPack => CreateFullUrl(_sellAPack);
    [SerializeField]
    private string _buyAPack;
    public string BuyAPack => CreateFullUrl(_buyAPack);
    [SerializeField]
    private string _getLandCaravans;
    public string GetLandCaravans => CreateFullUrl(_getLandCaravans);
    [SerializeField]
    private string _getPlayerCaravans;
    public string GetPlayCaravans => CreateFullUrl(_getPlayerCaravans);

    private string CreateFullUrl(string api)
    {
        return _environment.GetApiHostAndPort() + api;
    }
}
