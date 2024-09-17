using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AStarGraphAutoSettings))]
public class AStarGraphAutoSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (AStarGraphAutoSettings)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Apply"))
        {
            myTarget.GenerateGridGraphSetup();
            EditorUtility.SetDirty(myTarget.AStarPath);
        }
        if (GUILayout.Button("Clear Except Pattern"))
        {
            myTarget.ClearExceptPattern();
            EditorUtility.SetDirty(myTarget.AStarPath);
        }
    }
}
