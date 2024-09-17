using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TownMapGeneratorForEditor))]
public class TownMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myTarget = (TownMapGeneratorForEditor)target;
        if (GUILayout.Button("Generate!"))
        {
            myTarget.Editor_GenerateMap();
        }
    }
}
