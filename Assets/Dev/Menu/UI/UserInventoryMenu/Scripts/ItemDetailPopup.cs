using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ItemDetailPopup : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _cameraFocusTile;
    [SerializeField]
    private IntegerVariable _bcmSelectedBuildingId;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private IntegerVariable _selectedProductId;
    [SerializeField]
    private IntegerVariable _selectedCellId;

    [Header("Reference - Read")]
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private ItemTypeList _itemTypes;
    [SerializeField]
    private ProductFormulaList _itemFormulas;
    [SerializeField]
    private PrefixList _prefixes;
    [SerializeField]
    private UserProfile _passport;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _displayItemDetail;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _askOpenBcmAndFocusSelectedBuilding;
    [SerializeField]
    private UnityEvent _onCannotProduceItemAtThisLand;
    [SerializeField]
    private UnityEvent _howToMakePopupSequence;
    [SerializeField]
    private UnityEvent _howToMakeBuildingRequiredBuilding;

    [Header("Config")]
    [SerializeField]
    private InventoryItemCell _itemImage;
    [SerializeField]
    private Button _tradeButton;
    [SerializeField]
    private TMP_Text _itemName;
    [SerializeField]
    private Transform _buffGroup;
    [SerializeField]
    private TMP_Text _buffTextTemplate;
    [SerializeField]
    private TMP_Text _itemDes;
    [SerializeField]
    private Transform _costGroup;
    [SerializeField]
    private TMP_Text _noCostText;
    [SerializeField]
    private TMP_Text _cannotProduceText;
    [SerializeField]
    private GameObject _itemIconAndTextPrefab;
    [SerializeField]
    private TMP_Text _workload;
    [SerializeField]
    private GameObject[] _showHideContents;
    [SerializeField]
    private GameObject[] _costShowHideContent;

    private int _itemId;
    private List<GameObject> _itemsIconAndText = new();
    private List<GameObject> _buffTexts = new();

    public void HowToMake()
    {
        int producingBuilding = _itemFormulas.GetProducingBuilding(_itemId);
        if (producingBuilding == 0)
        {
            return;
        }

        if (!CanUserLandProduceItem())
        {
            _onCannotProduceItemAtThisLand.Invoke();
            return;
        }

        List<int> userProducingBuildings = _userBuildings.GetAllBuildingsOfId(producingBuilding);
        if (userProducingBuildings.Count == 0)
        {
            DirectToRequiredBuildingInMenu(producingBuilding);
            return;
        }

        int focusedBuilding = GetFocusedBuilding(userProducingBuildings);
        if (focusedBuilding == 0)
        {
            DirectToRequiredBuildingInMenu(producingBuilding);
            return;
        }

        FocusProducingBuilding(focusedBuilding);
    }

    private void SetUp(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        ActiveContent(true);
        var selectedCell = (InventoryScrollRow.CellData)args[0];

        _itemId = selectedCell.itemId;
        _itemImage.SetUp(selectedCell);
        _itemImage.SetItemCount(_userItems.GetItemCount(_itemId, new() { star = selectedCell.star, prefix = selectedCell.prefix }));
        _itemName.text = _items.GetItemName(_itemId);

        SetUpTradeButton();
        DisplayBuff(selectedCell.prefix);

        var itemCost = _itemFormulas.GetFormula(selectedCell.itemId);
        _workload.text = itemCost.timeCost.ToString();

        _selectedCellId.Value = selectedCell.cellId;
        bool canProduce = CanUserLandProduceItem();
        ActiveCostContent(canProduce);
        if (!canProduce)
        {
            RemoveAllCostInfo();
            return;
        }

        List<ResourcePoco> materials = itemCost.material;
        DisplayCost(materials);
    }

    public void ActiveContent(bool active)
    {
        for (int i = 0; i < _showHideContents.Length; i++)
        {
            _showHideContents[i].SetActive(active);
        }
    }

    public void ActiveCostContent(bool active)
    {
        _cannotProduceText.enabled = !active;
        for (int i = 0; i < _costShowHideContent.Length; i++)
        {
            _costShowHideContent[i].SetActive(active);
        }
    }

    private bool CanUserLandProduceItem()
    {
        ItemType itemType = _items.GetItemType(_itemId);
        if (itemType == ItemType.TREASURY)
        {
            return false;
        }

        var land = _landsStaticInfos.GetLand(_passport.landId);
        var condition = _itemFormulas.GetLandCondition(_itemId);
        return ItemHelper.CanProduceItemAtLand(land, condition);
    }

    private void FocusProducingBuilding(int focusedBuilding)
    {
        _cameraFocusTile.Value = _userBuildings.GetBuildingLocation(focusedBuilding);

        BuildingStatus focusedBuildingStatus = _userBuildings.GetBuildingStatus(focusedBuilding);
        bool canBuildingProduce = BuildingHelper.CanBuildingProduce(focusedBuildingStatus);
        if (!canBuildingProduce)
        {
            return;
        }

        _selectedBuildingObjId.Value = focusedBuilding;
        _selectedProductId.Value = _itemId;
        _howToMakePopupSequence.Invoke();
    }

    private int GetFocusedBuilding(List<int> userProducingBuildings)
    {
        int focusedBuilding = 0;
        for (int i = 0; i < userProducingBuildings.Count; i++)
        {
            var userBuildingObjId = userProducingBuildings[i];
            BuildingStatus status = _userBuildings.GetBuildingStatus(userBuildingObjId);
            if (status != BuildingStatus.Destructed)
            {
                focusedBuilding = userBuildingObjId;
            }

            bool canBuildingProduce = BuildingHelper.CanBuildingProduce(status);
            if (canBuildingProduce)
            {
                break;
            }
        }

        return focusedBuilding;
    }

    private void DirectToRequiredBuildingInMenu(int buildingId)
    {
        _howToMakeBuildingRequiredBuilding.Invoke();
        _bcmSelectedBuildingId.Value = buildingId;
        _askOpenBcmAndFocusSelectedBuilding.Raise();
    }

    private void SetUpTradeButton()
    {
        ItemType type = _items.GetItemType(_itemId);
        _tradeButton.interactable = _itemTypes.IsTradable(type);
    }

    private void DisplayBuff(int prefixId)
    {
        if (_buffTextTemplate == null)
        {
            return;
        }

        _buffTextTemplate.text = "NO BUFF";
        if (prefixId == 0)
        {
            return;
        }

        var prefix = _prefixes.GetPrefix(prefixId);
        // TODO Update logic when there are multiple buffs in 1 prefix
        _buffTextTemplate.enabled = true;
        _buffTextTemplate.text = $"{prefix.buff} <color=green>+{prefix.buffValue}</color>";
    }

    private void DisplayCost(List<ResourcePoco> materials)
    {
        RemoveAllCostInfo();

        _noCostText.enabled = materials.Count == 0;
        for (int i = 0; i < materials.Count; i++)
        {
            var mat = materials[i];
            var costObj = Instantiate(_itemIconAndTextPrefab, _costGroup);
            int userOwn = _userItems.GetItemCount(mat.itemId);

            costObj.GetComponent<ResourceIconAndLabel>().SetUp(mat.itemId, ItemHelper.FormatItemCostColor(userOwn, mat.itemCount));
            costObj.SetActive(true);
            _itemsIconAndText.Add(costObj);
        }
    }

    private void RemoveAllCostInfo()
    {
        for (int i = _itemsIconAndText.Count - 1; i >= 0; i--)
        {
            var item = _itemsIconAndText[i];
            _itemsIconAndText.RemoveAt(_itemsIconAndText.Count - 1);
            Destroy(item);
        }
    }

    private void OnEnable()
    {
        _displayItemDetail.Subcribe(SetUp);
    }

    private void OnDisable()
    {
        _displayItemDetail.Unsubcribe(SetUp);
    }
}
