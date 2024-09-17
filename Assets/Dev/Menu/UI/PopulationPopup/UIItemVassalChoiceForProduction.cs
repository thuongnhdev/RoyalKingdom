using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemVassalChoiceForProduction : UIItemVassalChoice
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _considerVassal;

    public void Prod_OnClickVassal()
    {
        OnClickChoice();
        _considerVassal.Value = _vassal.Data.idVassalTemplate;
    }
}
