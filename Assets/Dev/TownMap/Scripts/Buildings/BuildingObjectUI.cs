using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingObjectUI : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingProductionList _userProductions;
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private VassalSpriteSOList _vassalSpriteList;
    [SerializeField]
    private IntegerVariable _selectedBCMBuildingId;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private BoolVariable _foundationIsOpening;

    [SerializeField]
    private UnityEvent _onOpenUI;
    [SerializeField]
    private UnityEvent _onCloseUI;

    [Header("Config")]
    [SerializeField]
    private BuildingComponentGetter _compGetter;
    [SerializeField]
    private Canvas _buildingContextMenuCanvas;
    [SerializeField]
    private Canvas _persistentUiCanvas;

    [Header("ProgressGroup")]
    [SerializeField]
    private GameObject _progressUIGroup;
    [SerializeField]
    private Slider _progressBar;
    [SerializeField]
    private Slider _destroyProgressBar;
    [SerializeField]
    private Image _progressingIcon;
    [SerializeField]
    private GameObject _repeatIcon;
    [SerializeField]
    private TMP_Text _remainTimeText;
    [SerializeField]
    private TMP_Text _destroyRemainTimeText;

    [Header("RequireResourceGroup")]
    [SerializeField]
    private GameObject _requireResourceUIGroup;
    [SerializeField]
    private GameObject _resourceIconAndLabelPrefab;
    [SerializeField]
    private RectTransform _requireResourceLayoutGroup;

    [Header("VassalIconGroup")]
    [SerializeField]
    private GameObject _vassalIconGroup;
    [SerializeField]
    private Image _vassalIcon;

    private Slider _currentProgressBar;
    private TMP_Text _currentTimeText;
    private List<GameObject> _resourcesIconAndLabel = new();

    public void In_OpenOrCloseBasedOnObjId()
    {
        if (_selectedBuildingObjId.Value == 0)
        {
            In_CloseUI();
            return;
        }

        if (_selectedBuildingObjId.Value == _compGetter.Operation.BuildingObjId)
        {
            In_OpenUI();
            return;
        }

        In_CloseUI();
    }

    public void In_CloseUI()
    {
        if (!_buildingContextMenuCanvas.enabled)
        {
            return;
        }

        _onCloseUI.Invoke();
    }

    public void In_OpenUI()
    {
        if (_foundationIsOpening.Value)
        {
            return;
        }
        if (_buildingContextMenuCanvas.enabled)
        {
            return;
        }

        _onOpenUI.Invoke();
    }

    private ResourcePoco _bufferResource;
    public void In_RefreshResourceTextColor()
    {
        for (int i = 0; i < _resourcesIconAndLabel.Count; i++)
        {
            var resource = _resourcesIconAndLabel[i].GetComponent<ResourceIconAndLabel>();
            if (resource == null)
            {
                continue;
            }
            string currentText = resource.Label.Split('/')[0];
            string costText = resource.Label.Split('/')[1];
            int.TryParse(currentText, out int current);
            int.TryParse(costText, out int cost);
            _bufferResource.itemId = resource.ItemId;
            _bufferResource.itemCount = cost - current;
            bool canAfford = _userItems.CanAfford(_bufferResource);

            Color color = canAfford ? Color.white : Color.red;
            resource.SetLabelColor(color);
        }
    }

    public void In_DisplayFullWarehouseIfNeeded()
    {
        if (_currentProgressBar == null || _currentProgressBar.value < 1f)
        {
            return;
        }

        _currentTimeText.text = "Warehouse is full!";
    }

    public void UpdateProgress(float timeCost, float currentProgress, float progressPerSec, bool isDestroyProgress = false)
    {
        BuildingStatus status = _compGetter.Operation.Status;
        bool isProgressStatus = BuildingHelper.IsOnProgressStatus(status);
        SetActiveProgressGroup(isProgressStatus);

        if (timeCost <= currentProgress)
        {
            _currentTimeText.text = TimeUtils.FormatTime(0f);
            _currentProgressBar.value = 1f;
            return;
        }

        SetActiveRequireResourceGroup(false);

        SetActiveProgressGroup(true);
        SetProgressBarAppearance(isDestroyProgress);

        _currentProgressBar.value = currentProgress / timeCost;

        if (progressPerSec == 0f)
        {
            _currentTimeText.text = TimeUtils.FormatTime(float.PositiveInfinity);
            return;
        }

        _currentTimeText.gameObject.SetActive(true);
        _currentTimeText.text = TimeUtils.FormatTime((timeCost - currentProgress) / progressPerSec);
    }

    public void UpdateRequireResource(List<ResourcePoco> currentResources, List<ResourcePoco> cost)
    {
        SetActiveRequireResourceGroup(true);
        SetActiveProgressGroup(false);

        if (ItemHelper.IsGreaterOrEquals(currentResources, cost))
        {
            SetActiveRequireResourceGroup(false);
            RemoveAllResourceUIItem();
            return;
        }

        SpawnOrRemoveResourceUIItem(cost.Count - _resourcesIconAndLabel.Count);

        for (int i = 0; i < _resourcesIconAndLabel.Count; i++)
        {
            var resourceUIObj = _resourcesIconAndLabel[i];
            resourceUIObj.SetActive(true);

            int itemId = cost[i].itemId;
            int currentResourceValue = GetCurrentResourceValue(currentResources, itemId);

            ResourcePoco needed = cost[i];
            needed.itemCount -= currentResourceValue;
            bool affordable = _userItems.CanAfford(needed);
            var resourceFillDisplayer = resourceUIObj.GetComponent<ResourceIconAndLabel>();
            resourceFillDisplayer.SetUp(itemId, $"{currentResourceValue}/{cost[i].itemCount}");
            if (affordable)
            {
                resourceFillDisplayer.SetLabelColor(Color.white);
                continue;
            }

            resourceFillDisplayer.SetLabelColor(Color.red);
        }
    }

    public void UpdateVassalIcon(int vassalId, BuildingStatus status)
    {
        if (status != BuildingStatus.WaitForConstructingResource
            && status != BuildingStatus.OnConstruction
            && status != BuildingStatus.WaitingForUpgradeResource
            && status != BuildingStatus.Upgrading
            && status != BuildingStatus.OnDestruction
            && status != BuildingStatus.WaitingForProductResource
            && status != BuildingStatus.Producing)
        {
            SetActiveVassalGroup(false);
            return;
        }

        SetActiveVassalGroup(true);
        if (vassalId == 0)
        {
            _vassalIcon.overrideSprite = null;
            MoveVassalToPersistentGroup();
            return;
        }

        _vassalIcon.overrideSprite = _vassalSpriteList.GetSmallIcon(vassalId);
        MoveVassalGroupToContextGroup();
    }

    public void SetActive(bool active) 
    {
        SetActiveVassalGroup(active);
        SetActiveRequireResourceGroup(active);
        SetActiveProgressGroup(active);
    }

    private void OpenOrCloseBasedOnSelectedBuilding(int selectedBuildingObjId)
    {
        In_OpenOrCloseBasedOnObjId();
    }

    private int GetCurrentResourceValue(List<ResourcePoco> currentResources, int itemId)
    {
        if (currentResources == null)
        {
            return 0;
        }

        int resourceValue = 0;
        for (int i = 0; i < currentResources.Count; i++)
        {
            ResourcePoco resource = currentResources[i];
            int id = resource.itemId;
            if (ItemHelper.IsGroupItemId(itemId))
            {
                id = ItemHelper.GetGroupItemId(id);
            }

            if (id == itemId)
            {
                resourceValue += resource.itemCount;
            }
        }

        return resourceValue;
    }

    private void SpawnOrRemoveResourceUIItem(int count)
    {
        if (count == 0)
        {
            return;
        }

        if (count < 0)
        {
            count = Mathf.Abs(count);
            for (int i = count - 1; 0 <= i; i--)
            {
                var removeObj = _resourcesIconAndLabel[^1];
                Destroy(removeObj);
                _resourcesIconAndLabel.RemoveAt(_resourcesIconAndLabel.Count - 1);
            }

            return;
        }

        for (int i = 0; i < count; i++)
        {
            _resourcesIconAndLabel.Add(Instantiate(_resourceIconAndLabelPrefab, _requireResourceLayoutGroup));
        }
    }

    private void RemoveAllResourceUIItem()
    {
        for (int i = _resourcesIconAndLabel.Count - 1; 0 <= i; i--)
        {
            Destroy(_resourcesIconAndLabel[i]);
            _resourcesIconAndLabel.RemoveAt(_resourcesIconAndLabel.Count - 1);
        }
    }

    private void SetActiveProgressGroup(bool active)
    {
        _progressUIGroup.SetActive(active);
        if (!active)
        {
            return;
        }

        SetUpProgressingIcon();

    }
    private void SetUpProgressingIcon()
    {
        if (_compGetter.Operation.Status != BuildingStatus.Producing)
        {
            _progressingIcon.overrideSprite = null;
            _repeatIcon.SetActive(false);
            return;
        }

        int buildingObjId = _compGetter.Operation.BuildingObjId;
        int currentProduct = _userProductions.GetCurrentProductId(buildingObjId);

        bool repeat = _userProductions.GetQueueRepeat(buildingObjId);
        _repeatIcon.SetActive(repeat);

        if (currentProduct == 0)
        {
            _progressingIcon.overrideSprite = null;
            return;
        }

        _progressingIcon.overrideSprite = _itemAssets.GetItemSprite(currentProduct);
    }

    private void SetActiveVassalGroup(bool active)
    {
        _vassalIconGroup.SetActive(active);
    }

    private void MoveVassalGroupToContextGroup()
    {
        _vassalIconGroup.transform.SetParent(_buildingContextMenuCanvas.transform, false);
    }

    private void MoveVassalToPersistentGroup()
    {
        _vassalIconGroup.transform.SetParent(_persistentUiCanvas.transform, false);
    }

    private void SetActiveRequireResourceGroup(bool active)
    {
        _requireResourceUIGroup.SetActive(active);
    }

    private void SetProgressBarAppearance(bool isDestroyProgress)
    {
        _progressBar.gameObject.SetActive(!isDestroyProgress);
        _destroyProgressBar.gameObject.SetActive(isDestroyProgress);

        _currentProgressBar = isDestroyProgress ? _destroyProgressBar : _progressBar;
        _currentTimeText = isDestroyProgress ? _destroyRemainTimeText : _remainTimeText;
    }

    private void DisplayFollowingFoundationIsOpening(bool isOpening)
    {
        if (isOpening)
        {
            In_CloseUI();
        }

        _persistentUiCanvas.gameObject.SetActive(!isOpening);
    }

    private void OnEnable()
    {
        _buildingContextMenuCanvas.worldCamera = Camera.main;
        _persistentUiCanvas.worldCamera = Camera.main;

        _selectedBuildingObjId.OnValueChange += OpenOrCloseBasedOnSelectedBuilding;
        _foundationIsOpening.OnValueChange += DisplayFollowingFoundationIsOpening;
    }

    private void OnDisable()
    {
        _selectedBuildingObjId.OnValueChange -= OpenOrCloseBasedOnSelectedBuilding;
        _foundationIsOpening.OnValueChange -= DisplayFollowingFoundationIsOpening;
    }
}
