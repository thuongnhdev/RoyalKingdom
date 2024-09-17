using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LongVar", menuName = "ScriptableObjectSystem/LongVariable")]
public class LongVariable : BaseScriptableObjectVariable<long>
{
    private long _change;

    protected override bool IsSetNewValue(long value)
    {
        _change = value - _value;
        return value != _value;
    }

    public long LastChange
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
