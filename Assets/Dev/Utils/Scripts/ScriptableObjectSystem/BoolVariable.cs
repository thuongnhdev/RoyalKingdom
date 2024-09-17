using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoolVar", menuName = "ScriptableObjectSystem/BoolVariable")]
public class BoolVariable : BaseScriptableObjectVariable<bool>
{
    public void Revert()
    {
        Value = !Value;
    }

    protected override bool IsSetNewValue(bool value)
    {
        return value != _value;
    }
}
