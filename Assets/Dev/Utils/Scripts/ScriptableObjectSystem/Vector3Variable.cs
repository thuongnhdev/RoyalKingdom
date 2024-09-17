using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector3Var", menuName = "ScriptableObjectSystem/Vector3")]
public class Vector3Variable : BaseScriptableObjectVariable<Vector3>
{
    protected override bool IsSetNewValue(Vector3 value)
    {
        return !_value.Equals(value);
    }
}
