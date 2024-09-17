using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatVar", menuName = "ScriptableObjectSystem/FloatVariable")]
public class FloatVariable : BaseScriptableObjectVariable<float>
{
    private float _change;

    protected override bool IsSetNewValue(float value)
    {
        _change = value - _value;
        return value != _value;
    }

    public float LastChange
    {
        get
        {
            return _change;
        }
    }
}
