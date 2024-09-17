using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringVariableList", menuName = "ScriptableObjectSystem/StringVariableList")]
public class StringVariableList : BaseListVariable<string>
{
    protected override bool Compare(string item1, string item2)
    {
        return item1 == item2;
    }
}
