using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionProgressAndQueuePopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _currentProgress;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private ProductFormulaList _formulas;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private UserBuildingProductionList _buildingProductions;
    [SerializeField]
    private ItemAssetList _itemAssets;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _askUpdateQueueRepeat;

    [Header("Configs")]
    [SerializeField]
    private ItemIconAndGrade _currentItem;
    [SerializeField]
    private TMP_Text _remainTime;
    [SerializeField]
    private Slider _currentProgressSlider;
    [SerializeField]
    private Toggle _repeatToggle;
    [SerializeField]
    private List<QueuedProduct> _queuedProductObjects;

    private int _currentProduct;
    
    public void SetUp()
    {
        var buildingProduction = _buildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (buildingProduction == null)
        {
            return;
        }

        _repeatToggle.isOn = _buildingProductions.GetQueueRepeat(_selectedBuildingObjId.Value);

        _currentProduct = buildingProduction.CurrentProductId;
        if (_currentProduct != 0)
        {
            _currentItem.SetUp(_itemAssets.GetItemSprite(_currentProduct), _itemAssets.GetItemGradeSprite(_currentProduct));
        }
        else
        {
            _currentItem.ResetDisplay();
        }

        SetUpCurrentProgress(_currentProgress.Value);
        SetUpProductQueue(buildingProduction);
    }

    public void CheckAndSyncQueueRepeat(bool repeat)
    {
        if (_selectedBuildingObjId.Value == 0)
        {
            return;
        }

        var production = _buildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (repeat == production.queueRepeat)
        {
            return;
        }

        _askUpdateQueueRepeat.Raise();
    }

    public void RefreshRepeatToggle()
    {
        NextFrame_RefreshToggle().Forget();
    }

    private async UniTaskVoid NextFrame_RefreshToggle()
    {
        await UniTask.NextFrame(); // to ensure data is updated
        _repeatToggle.isOn = _buildingProductions.GetQueueRepeat(_selectedBuildingObjId.Value);
    }

    private void SetUpCurrentProgress(float currentProgress)
    {
        int buildingObjId = _selectedBuildingObjId.Value;
        if (buildingObjId == 0 || _currentProduct == 0)
        {
            _remainTime.text = "\u221e";
            _currentProgressSlider.value = 0f;
            return;
        }
        BuildingStatus status = _userBuildings.GetBuildingStatus(buildingObjId);
        if (status == BuildingStatus.WaitingForProductResource)
        {
            _remainTime.text = "IN CARRYING";
            _currentProgressSlider.value = 0f;
            return;
        }

        float workload = _formulas.GetTimeCost(_currentProduct);
        float productRate = _buildingProductions.GetCurrentProductRate(buildingObjId);

        (float current, float remain) progress = MathUtils.CurrentRatioAndRemain(_currentProgress.Value, workload, productRate);
        _currentProgressSlider.value = progress.current;
        string timeText = TimeUtils.FormatTime(progress.remain);
        _remainTime.text = timeText;
    }

    private void SetUpProductQueue(UserBuildingProduction production)
    {
        var queuedProducts = production.itemQueue;
        for (int i = 0; i < _queuedProductObjects.Count; i++)
        {
            int queueIndex = i + 1; // The first item is put on CurrentItem, queue starts from second item (if any)
            var queueProductObj = _queuedProductObjects[i];
            if (queueIndex < queuedProducts.Count)
            {
                // TODO add ap option
                queueProductObj.SetUp(new()
                {
                    queueIndex = queueIndex,
                    itemId = queuedProducts[queueIndex].productId
                });
                continue;
            }

            queueProductObj.ActiveContent(false);
        }
    }

    private void OnEnable()
    {
        _currentProgress.OnValueChange += SetUpCurrentProgress;
    }

    private void OnDisable()
    {
        _currentProgress.OnValueChange -= SetUpCurrentProgress;
    }
}
