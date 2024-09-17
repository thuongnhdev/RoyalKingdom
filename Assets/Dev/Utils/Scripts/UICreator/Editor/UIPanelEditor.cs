using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIPanel))]
public class UIPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var uiTarget = (UIPanel)target;
        if (GUILayout.Button("Open"))
        {
            uiTarget.Open();
        }

        if (GUILayout.Button("Close"))
        {
            uiTarget.Close();
        }

        DrawDefaultInspector();
    }
}
