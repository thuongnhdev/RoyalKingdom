using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVassalChoiceForProduction : UIVassalChoice
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _considerVassal;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    public void Prod_OnClickConfirm()
    {
        // Temporary selected vassal is assigned to ConsiderVassal when click to Vassal Item
        Close();
    }

    public void Prod_OnClickCancel()
    {
        _considerVassal.Value = _userBuildingList.GetVassalInCharge(_selectedBuildingObjId.Value);
        Close();
    }
}
