using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TownMapSO))]
public class TownMapSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (TownMapSO)target;

        if (GUILayout.Button("Apply to Game"))
        {
            myTarget.EditorOnly_ApplyDataToSoUsedInGame();
        }

        DrawDefaultInspector();
    }
}
