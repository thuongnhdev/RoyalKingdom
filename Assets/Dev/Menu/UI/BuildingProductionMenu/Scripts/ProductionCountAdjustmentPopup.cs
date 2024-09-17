using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionCountAdjustmentPopup : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _starAdjustItem;
    [SerializeField]
    private IntegerVariable _starAdjustItemCount;
    [SerializeField]
    private Vector3Variable _starAdjustPopupPos;
    [SerializeField]
    private ProductionMaterialsStar _materialStars;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private IntegerVariable _selectedProductId;
    [SerializeField]
    private ProductFormulaList _productFormula;
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private UserBuildingProductionList _userProductions;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private WarehousesCapacity _warehouseCapacity;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _openStarAdjustmentPopup;
    [SerializeField]
    private GameEvent _queueAsManyAsProductsPossible;

    [Header("Configs")]
    [SerializeField]
    private Button _produceButton;
    [SerializeField]
    private Button _produceAllButton;
    [SerializeField]
    private Image _gradeImage;
    [SerializeField]
    private Image _productImage;
    [SerializeField]
    private GameObject _warehouseFullBlock;
    [SerializeField]
    private Button _buildButton;
    [SerializeField]
    private Button _upgradeButton;
    [SerializeField]
    private Transform _costGroup;
    [SerializeField]
    private TMP_Text _noCostText;
    [SerializeField]
    private GameObject _itemIconAndLabelTemplate;
    [SerializeField]
    private TMP_Text _productCountText;

    private int _productCount = 0;
    private List<GameObject> _costObjs = new();

    public void QueueAllPossible()
    {
        _queueAsManyAsProductsPossible.Raise(_productCount);
    }

    public void BuildNewWarehouse()
    {
        int warehouse = _warehouseCapacity.GetWarehouseForItem(_selectedProductId.Value);
        BuildingCommunicator.Instance.OpenBcmAndFocusBuilding(warehouse);
    }

    public void UpgradeWarehouse()
    {
        int warehouse = _warehouseCapacity.GetWarehouseForItem(_selectedProductId.Value);
        int targetUpgradeWarehouse = _userBuildings.GetLowestLevelIdleBuildingOfId(warehouse);
        BuildingCommunicator.Instance.FocusThenOpenUpgradePopup(targetUpgradeWarehouse);
    }

    public void SetUp()
    {
        if (_selectedProductId.Value == 0)
        {
            return;
        }

        _materialStars.Reset();

        _gradeImage.overrideSprite = _itemAssets.GetItemGradeSprite(_selectedProductId.Value);
        _productImage.overrideSprite = _itemAssets.GetItemSprite(_selectedProductId.Value);

        CalculatePossibleProductCount();
        SetUpCostInfo();
        SetUpFullWarehouseBlock();
    }

    public void SetStarAdjustItemAndCount(int itemId, int count)
    {
        _starAdjustItem.Value = itemId;
        _starAdjustItemCount.Value = count;
    }

    public void SetStarAdjustPopupPosThenOpen(Transform costItemTrans)
    {

        _starAdjustPopupPos.Value = costItemTrans.position;
        _openStarAdjustmentPopup.Raise();
    }

    public void CalculatePossibleProductCount()
    {
        if (_selectedBuildingObjId.Value == 0)
        {
            return;
        }

        _productCount = _userProductions.GetAvailableSlotCount(_selectedBuildingObjId.Value);
        _productCountText.text = $"x{_productCount}";

        bool enoughQueueSlot = _productCount != 0;
        if (_selectedProductId.Value == 0)
        { 
            return;
        }
        bool enoughtWarehouseSlot = _warehouseCapacity.CanAddItem(_selectedProductId.Value, 1);
        SetProduceButtonsActive(enoughQueueSlot, enoughtWarehouseSlot);
    }

    public void SetUpFullWarehouseBlock()
    {
        int itemId = _selectedProductId.Value;
        if (itemId == 0)
        {
            return;
        }

        bool canAddItem = _warehouseCapacity.CanAddItem(itemId, 1);

        _buildButton.gameObject.SetActive(!canAddItem);
        _upgradeButton.gameObject.SetActive(!canAddItem);
        _warehouseFullBlock.SetActive(!canAddItem);

        int warehouse = _warehouseCapacity.GetWarehouseForItem(itemId);
        int lowestLevelWarehouse = _userBuildings.GetLowestLevelIdleBuildingOfId(warehouse);
        _upgradeButton.interactable = lowestLevelWarehouse != 0;
        _buildButton.interactable = _userBuildings.CanAddBuilding(warehouse);
    }

    private void SetProduceButtonsActive(bool enoughQueueSlot, bool enoughWarehouseSlot)
    {
        _produceButton.interactable = enoughQueueSlot;
        _produceAllButton.interactable = enoughQueueSlot;
        _produceButton.gameObject.SetActive(enoughWarehouseSlot);
        _produceAllButton.gameObject.SetActive(enoughWarehouseSlot);
    }

    private void SetUpCostInfo()
    {
        ClearCostInfo();

        List<ResourcePoco> perItemCost = _productFormula.GetResourceCost(_selectedProductId.Value);
        _noCostText.enabled = perItemCost.Count == 0;

        for (int i = 0; i < perItemCost.Count; i++)
        {
            var requiredItem = perItemCost[i];
            int requiredCount = requiredItem.itemCount;
            int userOwn = _userItems.GetItemCount(requiredItem.itemId);

            var costObj = Instantiate(_itemIconAndLabelTemplate, _costGroup);
            costObj.SetActive(true);
            costObj.GetComponent<ResourceIconAndLabel>().SetUp(requiredItem.itemId, ItemHelper.FormatItemCostColor(userOwn, requiredCount));
            costObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetStarAdjustItemAndCount(requiredItem.itemId, requiredCount);
                SetStarAdjustPopupPosThenOpen(costObj.transform);
            });

            _costObjs.Add(costObj);
        }
    }

    private void RefreshCost()
    {
        List<ResourcePoco> perItemCost = _productFormula.GetResourceCost(_selectedProductId.Value);
        List<ResourcePoco> totalCost = ItemHelper.MultiplyResources(perItemCost, _productCount);
        for (int i = 0; i < totalCost.Count; i++)
        {
            var requiredItem = totalCost[i];
            int requiredCount = requiredItem.itemCount;
            int userOwn = _userItems.GetItemCount(requiredItem.itemId);

            var costObj = _costObjs[i];
            costObj.GetComponent<ResourceIconAndLabel>().SetUp(requiredItem.itemId, ItemHelper.FormatItemCostColor(userOwn, requiredCount));

            _costObjs.Add(costObj);
        }
    }

    private void ClearCostInfo()
    {
        for (int i = 0; i < _costObjs.Count; i++)
        {
            var removeCostObj = _costObjs[^1];
            _costObjs.RemoveAt(_costObjs.Count - 1);
            Destroy(removeCostObj);
        }
    }
}
