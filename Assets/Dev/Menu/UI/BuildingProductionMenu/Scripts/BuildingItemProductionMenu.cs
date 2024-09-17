using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingItemProductionMenu : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _considerVassal;
    [SerializeField]
    private IntegerVariable _considerPriority;

    [Header("Reference - Read Master Data")]
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssets;
    [SerializeField]
    private BuildingProductDescriptionList _buildingProductionDesList;
    [SerializeField]
    private ProductFormulaList _itemFormularList;
    [SerializeField]
    private ItemAssetList _itemAssetList;
    [SerializeField]
    private VassalSOList _vassals;
    [SerializeField]
    private VassalStatsSOList _vassalStats;

    [Header("Reference - Read User Data")]
    [SerializeField]
    private UserProfile _passport;
    [SerializeField]
    private UserItemStorage _userItemStorage;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private UserBuildingProductionList _userBuildingProductionList;
    [SerializeField]
    private UserVassalSOList _userVassals;
    [SerializeField]
    private WarehousesCapacity _warehousesCapacity;

    [Header("Reference - Read Variable")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private IntegerVariable _selectedItemId;
    [SerializeField]
    private FloatVariable _currentProgress;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onUserClickQueueAnItem;
    [SerializeField]
    private GameEvent _askedUpdateQueueRepeat;
    [SerializeField]
    private GameEvent _askedUpdateHumanResource;

    [SerializeField]
    private UnityEvent _onClosePM;
    [SerializeField]
    private UnityEvent _onCloseAndGoHome;

    [Header("Configs")]
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private GameObject _resourceIconAndLabelPrefab;
    [SerializeField]
    private ProductionHRChangePopup _hrChangePopup;
    [SerializeField]
    private BuildingProductionItemScrollController _itemScroller;
    [SerializeField]
    private EnhancedScroller _scroller;

    [Header("Configs - Progress and mini Queue")]
    [SerializeField]
    private Toggle _repeatToggle;
    [SerializeField]
    private ItemIconAndGrade _currentProduct;
    [SerializeField]
    private Slider _currentProgressSlider;
    [SerializeField]
    private TMP_Text _remainTimeText;
    [SerializeField]
    private List<ItemIconAndGrade> _miniQueueProducts;
    [Header("Configs - Vassal and Worker")]
    [SerializeField]
    private VassalPotraitAndInfo _vassalDisplay;
    [SerializeField]
    private TMP_Text _buildingName;
    [SerializeField]
    private TMP_Text _workerNumberText;
    [SerializeField]
    private TMP_Text _estTimeText;
    [SerializeField]
    private Button _confirmButton;
    [SerializeField]
    private Button[] _priorityButtons;

    private float _currentTimeCost = 0f;
    private float _currentProgressPerSec = 0f;
    private int _workersWillBeAssigned = 0;

    // Inspector Call
    public void SetUpContent()
    {
        var userBuildingProduction = _userBuildingProductionList.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (userBuildingProduction == null)
        {
            return;
        }
        int buildingId = userBuildingProduction.buildingId;

        _currentProgressPerSec = userBuildingProduction.productRate;

        _buildingName.text = _buildings.GetBuildingName(buildingId);

        var queueItems = userBuildingProduction.itemQueue;
        List<int> queueItemsId = new();
        queueItems.ForEach(item => queueItemsId.Add(item.productId));
        SetUpItemMiniQueue(queueItemsId);

        _repeatToggle.isOn = _userBuildingProductionList.GetQueueRepeat(_selectedBuildingObjId.Value);

        SetUpItemList(userBuildingProduction.buildingId);

        int currentProduct = userBuildingProduction.CurrentProductId;
        _currentTimeCost = _itemFormularList.GetTimeCost(currentProduct);

        SetUpCurrentProduct(currentProduct);
        _considerVassal.Value = _userBuildings.GetVassalInCharge(_selectedBuildingObjId.Value);
        RefreshVassalInfo(true);

        _considerPriority.Value = userBuildingProduction.priority;
        ChangePriorityTab(_considerPriority.Value);

        UpdateCurrentProgress(_currentProgress.Value);
        UpdateEstimatedTime(_currentProgress.Value);

        TrackConfirmButtonInteraction(true).Forget();
    }

    // Inspector Call
    public void ResetContent()
    {
        _currentProduct.ResetDisplay();
        _currentProgressSlider.value = 0f;
        _remainTimeText.text = "\u221e";
        _selectedItemId.Value = 0;
        TrackConfirmButtonInteraction(false).Forget();
    }

    public void OpenWorkerPopup()
    {
        UIManagerTown.Instance.ShowUIPopulation();
    }

    public void RefreshRepeatToggle()
    {
        NextFrame_RefreshToggle().Forget();
    }
    private async UniTaskVoid NextFrame_RefreshToggle()
    {
        await UniTask.NextFrame(); // to ensure data is updated
        _repeatToggle.isOn = _userBuildingProductionList.GetQueueRepeat(_selectedBuildingObjId.Value);
    }

    public RectTransform GetItemPosition(int itemId)
    {
        var userBuildingProduction = _userBuildingProductionList.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (userBuildingProduction == null)
        {
            return null;
        }

        int buildingId = userBuildingProduction.buildingId;
        List<int> items = GetBuildingItems(buildingId);
        int dataIndex = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == itemId)
            {
                dataIndex = i;
                break;
            }
        }

        var itemCell = _scroller.GetCellViewAtDataIndex(dataIndex);
        return itemCell.transform.GetComponent<RectTransform>();
    }

    public RectTransform GetItemPositionByIndex(int index)
    {
        var itemCell = _scroller.GetCellViewAtDataIndex(index);
        if (itemCell == null)
        {
            return new();
        }
        return itemCell.transform.GetComponent<RectTransform>();
    }

    public void ConfirmChangeHumanResource()
    {
        if (!IsHumanResourceChanged(out bool priorityChanged))
        {
            return;
        }

        if (priorityChanged)
        {
            _askedUpdateHumanResource.Raise(_selectedBuildingObjId.Value);
        }
    }

    public void ShowHRInfoChangePopupIfNeeded(string closeOrGoHome)
    {
        if (!IsHumanResourceChanged())
        {
            CloseOrGoHome(closeOrGoHome);
            return;
        }

        _hrChangePopup.GetComponent<UIPanel>().Open();
        _hrChangePopup.SetSaveAction(() =>
        {
            ConfirmChangeHumanResource();
            CloseOrGoHome(closeOrGoHome);
        });
        _hrChangePopup.SetDiscardAction(() =>
        {
            CloseOrGoHome(closeOrGoHome);
        });
    }
    private void CloseOrGoHome(string closeOrGoHome)
    {
        if (closeOrGoHome == "close")
        {
            _onClosePM.Invoke();
            return;
        }

        _onCloseAndGoHome.Invoke();
    }

    public void RefreshVassalInfo(bool readFromBuilding)
    {
        if (_selectedBuildingObjId.Value == 0)
        {
            return;
        }

        int vassalInCharge = _considerVassal.Value;
        if (readFromBuilding)
        {
            vassalInCharge = _userBuildings.GetVassalInCharge(_selectedBuildingObjId.Value);
        }

        if (vassalInCharge == 0)
        {
            _vassalDisplay.SetActive(false);
            return;
        }
        _vassalDisplay.SetUp(vassalInCharge);
    }

    public void RefreshWorkerInfo()
    {
        SetUpWorkerInfo();
    }

    public void CheckAndSyncQueueRepeat(bool repeat)
    {
        if (_selectedBuildingObjId.Value == 0)
        {
            return;
        }

        var production = _userBuildingProductionList.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (repeat == production.queueRepeat)
        {
            return;
        }

        _askedUpdateQueueRepeat.Raise();
    }

    public void ChangePriorityTab(int priority)
    {
        for (int i = 0; i < _priorityButtons.Length; i++)
        {
            _priorityButtons[i].interactable = i + 1 != priority;
        }
        _considerPriority.Value = priority;

        SetUpWorkerInfo();
    }

    private void SetUpCurrentProduct(int currentProductId)
    {
        if (currentProductId == 0)
        {
            _currentProduct.ResetDisplay();
            return;
        }
        _currentProduct.SetUp(_itemAssetList.GetItemSprite(currentProductId), _itemAssetList.GetItemGradeSprite(currentProductId));
    }

    private void UpdateCurrentProgress(float currentProgress)
    {
        BuildingStatus status = _userBuildings.GetBuildingStatus(_selectedBuildingObjId.Value);
        if (status == BuildingStatus.WaitingForProductResource)
        {
            _remainTimeText.text = "IN CARRYING";
            _currentProgressSlider.value = 0f;
            return;
        }
        if (_currentTimeCost == 0f || status != BuildingStatus.Producing)
        {
            _remainTimeText.text = "\u221e";
            _currentProgressSlider.value = 0f;
            return;
        }

        (float current, float remain) progress = MathUtils.CurrentRatioAndRemain(currentProgress, _currentTimeCost, _currentProgressPerSec);
        _currentProgressSlider.value = progress.current;

        if (1f <= progress.current)
        {
            int currentProductId = _userBuildingProductionList.GetCurrentProductId(_selectedBuildingObjId.Value);
            if (!_warehousesCapacity.CanAddItem(currentProductId, 1))
            {
                _remainTimeText.text = "Warehouses are full";
                return;
            }
        }

        _remainTimeText.text = TimeUtils.FormatTime(progress.remain);
    }

    private void UpdateEstimatedTime(float currentProgress)
    {
        int currentProductId = _userBuildingProductionList.GetCurrentProductId(_selectedBuildingObjId.Value);
        if (currentProductId == 0)
        {
            _estTimeText.text = "No Product";
            return;
        }

        var savePack = GameData.Instance.SavedPack;
        if (savePack == null)
        {
            return;
        }
        var savedData = savePack.SaveData;
        if (savedData == null)
        {
            return;
        }
        float estimatedProductRate = _workersWillBeAssigned * (1f / savedData.TimeWorkloadOneWorker);
        var production = _userBuildingProductionList.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }
        if (_considerPriority.Value == production.priority)
        {
            estimatedProductRate = production.productRate;
        }

        (float current, float remain) progress = MathUtils.CurrentRatioAndRemain(currentProgress, _currentTimeCost, estimatedProductRate);
        _estTimeText.text = TimeUtils.FormatTime(progress.remain);
    }

    private void SetUpItemList(int buildingId)
    {
        _itemScroller.ChangeDisplayData(GetBuildingItems(buildingId));
    }

    private List<int> GetBuildingItems(int buildingId)
    {
        LandStaticInfo land = _landsStaticInfos.GetLand(_passport.landId);
        if (land == null)
        {
            return null;
        }

        List<int> buildingProducts = _buildingProductionDesList.GetBuildingProducts(buildingId);
        List<int> availableProducts = new();
        for (int i = 0; i < buildingProducts.Count; i++)
        {
            int productId = buildingProducts[i];
            (List<int>, int, int) condition = _itemFormularList.GetLandCondition(productId);
            if (!ItemHelper.CanProduceItemAtLand(land, condition))
            {
                continue;
            }

            availableProducts.Add(productId);
        }

        return availableProducts;
    }

    private void SetUpItemMiniQueue(List<int> itemQueue)
    {
        int queueCount = Mathf.Clamp(itemQueue.Count, 0, 4);

        for (int i = 0; i < _miniQueueProducts.Count; i++) 
        {
            int queueIndex = i + 1; // queue starts from second item, first item is put on CurrentItem
            if (queueIndex < queueCount)
            {
                int itemId = itemQueue[queueIndex];
                _miniQueueProducts[i].SetUp(_itemAssetList.GetItemSprite(itemId), _itemAssetList.GetItemGradeSprite(itemId));
                continue;
            }

            _miniQueueProducts[i].ResetDisplay();
        }
    }

    private void SetUpWorkerInfo()
    {
        var savePack = GameData.Instance.SavedPack;
        if (savePack == null)
        {
            return;
        }
        var savedData = savePack.SaveData;
        if (savedData == null)
        {
            return;
        }

        int totalWorker = savedData.WorkerNumber;
        var workerAssignment = savedData.ListWorkerAssignments[_considerPriority.Value - 1];
        int workerForConsiderPriority = (int)(workerAssignment.Percent * totalWorker / 100f);

        List<TaskData> currentTasks = savedData.ListTask;
        int taskCountForConsiderPriority = PrecalculateTaskCountForConsiderPriority(currentTasks);
        int workersWillBeAssigned = workerForConsiderPriority / taskCountForConsiderPriority;
        _workersWillBeAssigned = workersWillBeAssigned;
        _workerNumberText.text = $"<color=#43EF2C>{workersWillBeAssigned}</color>/{workerForConsiderPriority}";

        UpdateEstimatedTime(_currentProgress.Value);
    }

    private int PrecalculateTaskCountForConsiderPriority(List<TaskData> currentTasks)
    {
        int taskCount = 0;
        for (int i = 0; i < currentTasks.Count; i++)
        {
            if (currentTasks[i].Priority == _considerPriority.Value)
            {
                taskCount++;
            }
        }

        BuildingStatus status = _userBuildings.GetBuildingStatus(_selectedBuildingObjId.Value);
        if (status == BuildingStatus.Producing || status == BuildingStatus.WaitingForProductResource)
        {
            int currentPriority = _userBuildingProductionList.GetPriority(_selectedBuildingObjId.Value);
            if (currentPriority == _considerPriority.Value)
            {
                return Mathf.Clamp(taskCount, 1, taskCount);
            }
        }

        return taskCount + 1; // current task is not assigned, so +1 for calculating estimated time
    }

    private bool IsHumanResourceChanged(out bool priorityChanged)
    {
        priorityChanged = false;
        if (_selectedBuildingObjId.Value == 0)
        {
            return false;
        }

        int currentPriority = _userBuildingProductionList.GetPriority(_selectedBuildingObjId.Value);
        priorityChanged = currentPriority != _considerPriority.Value;

        return priorityChanged;
    }

    private bool IsHumanResourceChanged()
    {
        return IsHumanResourceChanged(out bool priorityChanged);
    }

    private CancellationTokenSource _trackConfirmButtonToken;
    private async UniTaskVoid TrackConfirmButtonInteraction(bool track)
    {
        _trackConfirmButtonToken?.Cancel();
        if (!track)
        {
            return;
        }

        _trackConfirmButtonToken = new();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_trackConfirmButtonToken.Token))
        {
            _confirmButton.interactable = IsHumanResourceChanged();
        }
    }

    private void OnEnable()
    {
        _currentProgress.OnValueChange += UpdateCurrentProgress;
        _currentProgress.OnValueChange += UpdateEstimatedTime;
    }

    private void OnDisable()
    {
        _trackConfirmButtonToken?.Cancel();
        _currentProgress.OnValueChange -= UpdateCurrentProgress;
        _currentProgress.OnValueChange -= UpdateEstimatedTime;
    }
}
