using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationStringList))]
public class LocalizationStringListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Clear all tables"))
        {
            var myTarget = (LocalizationStringList)target;
            myTarget.Editor_ClearAllTablesAndSharedTables();
        }
    }
}
