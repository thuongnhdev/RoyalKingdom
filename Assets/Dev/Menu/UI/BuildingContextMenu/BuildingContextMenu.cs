using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingContextMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingList;
    [SerializeField]
    private BuildingCommandList _buildingCommands;
    [SerializeField]
    private UserBuildingList _userBuildingList;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Config")]
    [SerializeField][Tooltip("Check Script for button order")]
    private List<Button> _buttons;
    [SerializeField][Tooltip("Check Script for event order")]
    private List<GameEvent> _cancelButtonEvents;
    [SerializeField]
    private RectTransform _buttonLayoutGroup;
    [SerializeField]
    private Canvas _parentCanvas;
    [SerializeField]
    private Image _buttonBG;
    [SerializeField]
    private GameObject _destructedGroup;

    [Header("Inpsec")]
    [SerializeField]
    private int _selectedBuildingId;
    [SerializeField]
    private int _activeButtonCount;

    private float _bgWidthfor6Buttons;

    public void In_UpdateButtonStatus()
    {
        if (!_parentCanvas.enabled || _selectedBuildingObjId.Value == 0)
        {
            return;
        }

        _destructedGroup.SetActive(false);
        _buttonBG.gameObject.SetActive(true);
        BuildingStatus status = _userBuildingList.GetBuildingStatus(_selectedBuildingObjId.Value);
        if (status == BuildingStatus.Destructed)
        {
            _destructedGroup.SetActive(true);
            _buttonBG.gameObject.SetActive(false);
        }

        int buttonBit = CalculateButtonsBitValue(status);

        _activeButtonCount = 0;
        for (int i = _buttons.Count - 1; i >= 0 ; i--)
        {
            bool active = buttonBit % 2 == 1;
            _buttons[i].gameObject.SetActive(active);
            if (active)
            {
                _activeButtonCount++;
            }

            buttonBit >>= 1;
        }

        SetUpCommandButton();
        SetUpRotateButton();
        SetUpUpgradeButton();
        SetUpCancelButton(status);
        SetButtonBGWidth(_activeButtonCount);
    }

    public void In_RaiseCommandEvent()
    {
        _buildingList.ExecuteBuildingCommand1(_selectedBuildingId);
    }

    public void In_RaiseCommend2Event()
    {
        _buildingList.ExecuteBuildingCommand2(_selectedBuildingId);
    }

    public void In_RaiseEventForCancelButton()
    {
        BuildingStatus status = _userBuildingList.GetBuildingStatus(_selectedBuildingObjId.Value);
        int eventBit = CalculateCancelBehaviorBitValue(status);
        for (int i = _cancelButtonEvents.Count - 1; 0 <= i; i--)
        {
            if (eventBit % 2 == 1)
            {
                _cancelButtonEvents[i].Raise();
                return;
            }

            eventBit >>= 1;
        }
    }

    private void SetUpCommandButton()
    {
        var commandButton = _buttons[1];
        if (!commandButton.gameObject.activeSelf)
        {
            return;
        }

        _selectedBuildingId = _userBuildingList.GetBuildingId(_selectedBuildingObjId.Value);
        var buildingCommandKey = _buildingList.GetBuildingCommand1Key(_selectedBuildingId);
        if (buildingCommandKey == BuildingCommandKey.None)
        {
            _activeButtonCount--;
            commandButton.gameObject.SetActive(false);
            return;
        }

        var buttonIcon = commandButton.transform.GetChild(0);
        buttonIcon.GetComponent<Image>().overrideSprite = _buildingCommands.GetCommandIcon(buildingCommandKey);

        var commandButton2 = _buttons[6];
        var buildingCommandKey2 = _buildingList.GetBuildingCommand2Key(_selectedBuildingId);
        if (buildingCommandKey2 == BuildingCommandKey.None)
        {
            _activeButtonCount--;
            commandButton2.gameObject.SetActive(false);
            return;
        }

        var buttonIcon2 = commandButton2.transform.GetChild(0);
        buttonIcon2.GetComponent<Image>().overrideSprite = _buildingCommands.GetCommandIcon(buildingCommandKey2);
    }

    private void SetUpRotateButton()
    {
        var rotateButton = _buttons[3].gameObject;
        if (!rotateButton.activeSelf)
        {
            return;
        }

        _selectedBuildingId = _userBuildingList.GetBuildingId(_selectedBuildingObjId.Value);
        if (_selectedBuildingId == 30101) // TODO refactor this when more road types come
        {
            rotateButton.SetActive(false);
            _activeButtonCount--;
        }
    }

    private void SetUpUpgradeButton()
    {
        if (!_buttons[2].gameObject.activeSelf)
        {
            return;
        }

        int maxLevel = _buildingList.GetBuldingMaxLevel(_selectedBuildingId);
        int currentLevel = _userBuildingList.GetBuildingLevel(_selectedBuildingObjId.Value);

        bool active = currentLevel < maxLevel;
        _activeButtonCount -= active ? 0 : 1;
        _buttons[2].gameObject.SetActive(active);
    }

    private void SetUpCancelButton(BuildingStatus status)
    {
        var destroyButton = _buttons[4].gameObject;
        if (!destroyButton.activeSelf)
        {
            return;
        }
        int cancelBehaviorBit = CalculateCancelBehaviorBitValue(status);
        bool isDestroyBehavior = cancelBehaviorBit >> (_cancelButtonEvents.Count - 1) == 1;
        Image destroyButtonIcon = destroyButton.transform.GetChild(0).GetComponent<Image>();
        Image cancelButtonIcon = destroyButton.transform.GetChild(1).GetComponent<Image>();
        destroyButtonIcon.enabled = isDestroyBehavior;
        cancelButtonIcon.enabled = !isDestroyBehavior;
        if (!isDestroyBehavior)
        {
            return;
        }

        if (status == BuildingStatus.WaitForConstructingResource
            || status == BuildingStatus.OnConstruction)
        {
            return;
        }

        bool isBreakable = _buildingList.IsBuildingBreakable(_selectedBuildingId);
        _activeButtonCount -= isBreakable ? 0 : 1;
        destroyButton.SetActive(isBreakable);
    }

    private void SetButtonBGWidth(int buttonsCount)
    {
        Vector2 bgSizeDelta = _buttonLayoutGroup.sizeDelta;
        bgSizeDelta.x = _bgWidthfor6Buttons * buttonsCount / 6f;
        _buttonLayoutGroup.sizeDelta = bgSizeDelta;
    }

    //                   Info   |Command | Upgrade | Rotate | Cancel| Repair | 2ndCommand |
    // None                     |        |         |        |       |        |            |  0000000
    // WaitForCon          x    |        |         |    x   |   x   |        |            |  1001100
    // OnCon               x    |        |         |    x   |   x   |        |            |  1001100
    // Idle                x    |   x    |    x    |    x   |   x   |        |      x     |  1111101
    // WaitForUp           x    |        |         |    x   |   x   |        |            |  1001100
    // Upgrading           x    |        |         |    x   |   x   |        |            |  1001100
    // WaitForProd         x    |   x    |         |    x   |   x   |        |      x     |  1101101
    // Producing           x    |   x    |         |    x   |   x   |        |      x     |  1101101
    // OnDestroy           x    |        |         |        |   x   |        |            |  1000100
    // Damaged             x    |        |         |    x   |   x   |    x   |            |  1001110
    // WairForRepair       x    |        |         |    x   |   x   |        |            |  1001100
    // Repairing           x    |        |         |    x   |   x   |        |            |  1001100
    private int CalculateButtonsBitValue(BuildingStatus status)
    {
        switch (status)
        {
            case BuildingStatus.WaitForConstructingResource:    return 0b1001100;
            case BuildingStatus.OnConstruction:                 return 0b1001100;
            case BuildingStatus.Idle:                           return 0b1111101;
            case BuildingStatus.WaitingForUpgradeResource:      return 0b1001100;
            case BuildingStatus.Upgrading:                      return 0b1001100;
            case BuildingStatus.WaitingForProductResource:      return 0b1101101;
            case BuildingStatus.Producing:                      return 0b1101101;
            case BuildingStatus.OnDestruction:                  return 0b1000100;
            case BuildingStatus.Damaged:                        return 0b1001110;
            case BuildingStatus.WaitForRepairResource:          return 0b1001100;
            case BuildingStatus.Repairing:                      return 0b1001100;
            default:                                            return 0;
        }
    }

    //                  Destroy | CancelCons | CancelProd | CancelRepair | CancelDestroy |
    // None                     |            |            |              |               | 00000
    // WaitForCon          x    |            |            |              |               | 10000
    // OnCon               x    |            |            |              |               | 10000
    // Idle                x    |            |            |              |               | 10000
    // WaitForUp                |      x     |            |              |               | 01000
    // Upgrading                |      x     |            |              |               | 01000
    // WaitForProd              |            |      x     |              |               | 00100
    // Producing                |            |      x     |              |               | 00100
    // OnDestroy                |            |            |              |       x       | 00001
    // Damaged             x    |            |            |              |               | 10000
    // WairForRepair            |            |            |      x       |               | 00010
    // Repairing                |            |            |      x       |               | 00010
    private int CalculateCancelBehaviorBitValue(BuildingStatus status)
    {
        switch (status)
        {
            case BuildingStatus.WaitForConstructingResource:    return 0b10000;
            case BuildingStatus.OnConstruction:                 return 0b10000;
            case BuildingStatus.Idle:                           return 0b10000;
            case BuildingStatus.WaitingForUpgradeResource:      return 0b01000;
            case BuildingStatus.Upgrading:                      return 0b01000;
            case BuildingStatus.WaitingForProductResource:      return 0b00100;
            case BuildingStatus.Producing:                      return 0b00100;
            case BuildingStatus.OnDestruction:                  return 0b00001;
            case BuildingStatus.Damaged:                        return 0b10000;
            case BuildingStatus.WaitForRepairResource:          return 0b00010;
            case BuildingStatus.Repairing:                      return 0b00010;
            default:                                            return 0;
        }
    }

    private void OnEnable()
    {
        _bgWidthfor6Buttons = _buttonLayoutGroup.sizeDelta.x;
    }
}
