using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCommunicator : MonoSingleton<BuildingCommunicator>
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _selectedBcmBuildingId;
    [SerializeField]
    private IntegerVariable _cameraFocusTile;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _openBuildingUpgradePopupCommand;
    [SerializeField]
    private GameEvent _openBcmCommandAndFocusBuilding;

    public void FocusBuilding(int buildingObjectId)
    {
        if (buildingObjectId == 0)
        {
            return;
        }

        _cameraFocusTile.Value = _userBuildings.GetBuildingLocation(buildingObjectId);
    }

    public void FocusThenOpenUpgradePopup(int buildingObjectId)
    {
        FocusBuilding(buildingObjectId);
        _selectedBuildingObjId.Value = buildingObjectId;
        _openBuildingUpgradePopupCommand.Raise();
    }

    public void OpenBcmAndFocusBuilding(int buildingId)
    {
        _selectedBcmBuildingId.Value = buildingId;
        _openBcmCommandAndFocusBuilding.Raise();
    }
}
