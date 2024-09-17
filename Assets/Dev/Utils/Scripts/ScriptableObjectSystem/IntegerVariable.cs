using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IntegerVar", menuName = "ScriptableObjectSystem/IntegerVariable")]
public class IntegerVariable : BaseScriptableObjectVariable<int>
{
    private int _change;

    protected override bool IsSetNewValue(int value)
    {
        _change = value - _value;
        return value != _value;
    }

    public int LastChange
    {
        get
        {
            return _change;
        }
    }

    public void Increase(int increase)
    {
        Value += increase;
    }

}
