using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadBezierDataExporter))]
public class RoadBezierDataExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Export SO data"))
        {
            var myTarget = (RoadBezierDataExporter)target;
            myTarget.CollectCitiesOnPath();
        }
        DrawDefaultInspector();
    }
}
