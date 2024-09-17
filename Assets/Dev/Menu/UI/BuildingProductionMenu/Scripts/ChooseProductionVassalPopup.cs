using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseProductionVassalPopup : BaseScrollController
{
    [Header("Reference - Read/Write")]
    [SerializeField]
    private IntegerVariable _considerVassal;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private UserBuildingList _buildings;
    [SerializeField]
    private UserVassalSOList _userVassals;

    public void SetUp()
    {
        _dataList.Clear();
        int[] vassals = _userVassals.GetAllVassalIds();
        for (int i = 0; i < vassals.Length; i++)
        {
            _dataList.Add(vassals[i]);
        }

        ReloadScrollView();
    }

    public void RevertConsiderVassal()
    {
        _considerVassal.Value = _buildings.GetVassalInCharge(_selectedBuildingObjId.Value);
    }
}
