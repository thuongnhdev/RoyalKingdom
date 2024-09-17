using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOperationCheatingPanel : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onUserBuildingObjUpdateInfo;

    [Header("Config")]
    [SerializeField]
    private TMP_Dropdown _statusDropdown;
    [SerializeField]
    private TMP_Dropdown _levelDropDown;
    [SerializeField]
    private List<Button> _applyButtons;

    private BuildingObjectFinder _buildingFinder;
    private BuildingObjectFinder BuildingFinder
    {
        get
        {
            if (_buildingFinder == null)
            {
                _buildingFinder = FindObjectOfType<BuildingObjectFinder>();
            }

            return _buildingFinder;
        }
    }

    public void InitContent()
    {
        InitStatusDropDown();
        InitLevelDropdown();
    }
    public void ApplyAll()
    {
        ApplyStatus();
        ApplyLevel();

        _applyButtons[0].interactable = false;
    }

    public void ApplyStatus()
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        Enum.TryParse(_statusDropdown.captionText.text, out BuildingStatus newStatus);

        if (userBuilding.status == newStatus)
        {
            return;
        }

        userBuilding.status = newStatus;

        _onUserBuildingObjUpdateInfo.Raise(userBuilding);

        _applyButtons[1].interactable = false;
    }

    public void ApplyLevel()
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        if (userBuilding.buildingLevel == _levelDropDown.value + 1)
        {
            return;
        }

        userBuilding.buildingLevel = _levelDropDown.value + 1;

        _onUserBuildingObjUpdateInfo.Raise(userBuilding);

        _applyButtons[2].interactable = false;
    }

    public void DestroyBuilding()
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        userBuilding.status = BuildingStatus.Destructed;
        userBuilding.refundMaterial = new();
        _onUserBuildingObjUpdateInfo.Raise(userBuilding);

        _selectedBuildingObjId.Value = 0;
    }

    public void Relocate()
    {
        var selectedBuilding = BuildingFinder.GetBuildingByBuildingObjectId(_selectedBuildingObjId.Value);
        var relocation = selectedBuilding.transform.GetChild(1).Find("Relocation").GetComponent<BuildingRelocation>();
        var draggable = selectedBuilding.transform.GetChild(0).Find("Platform").GetComponent<Draggable>();
        relocation.Relocatable = true;
        draggable.IsDraggable = true;
    }

    public void FinishRelocate()
    {
        var selectedBuilding = BuildingFinder.GetBuildingByBuildingObjectId(_selectedBuildingObjId.Value);
        var relocation = selectedBuilding.transform.GetChild(1).Find("Relocation").GetComponent<BuildingRelocation>();
        var draggable = selectedBuilding.transform.GetChild(0).Find("Platform").GetComponent<Draggable>();
        relocation.Relocatable = false;
        draggable.IsDraggable = false;

        _selectedBuildingObjId.Value = 0;
    }

    private void InitStatusDropDown()
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        var building = _buildings.GetBaseBuilding(userBuilding.buildingId);
        if (building == null)
        {
            return;
        }

        InitDropdown(userBuilding.status.ToString(), _statusDropdown, typeof(BuildingStatus));
    }
 
    private void InitLevelDropdown()
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        var building = _buildings.GetBaseBuilding(userBuilding.buildingId);
        if (building == null)
        {
            return;
        }

        int maxLevel = building.maxLevel;
        _levelDropDown.ClearOptions();

        for (int i = 0; i < maxLevel; i++)
        {
            _levelDropDown.options.Add(new TMP_Dropdown.OptionData("Level " + (i + 1).ToString()));
        }

        _levelDropDown.SetValueWithoutNotify(int.MaxValue);
        _levelDropDown.SetValueWithoutNotify(Mathf.Clamp(0, userBuilding.buildingLevel - 1, maxLevel - 1));
    }

    private void InitDropdown(string initValue, TMP_Dropdown targetDropdown, Type enumType, string prefix = "")
    {
        targetDropdown.options.Clear();

        foreach (string status in Enum.GetNames(enumType))
        {
            targetDropdown.options.Add(new TMP_Dropdown.OptionData(prefix + status));
        }

        var options = targetDropdown.options;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].text == initValue)
            {
                targetDropdown.SetValueWithoutNotify(int.MaxValue);
                targetDropdown.SetValueWithoutNotify(i);
                return;
            }
        }
    }
}
