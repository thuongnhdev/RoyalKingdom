using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldMapDataExporter))]
public class WorldMapDataExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (WorldMapDataExporter)target;
        if (GUILayout.Button("Export Land Data"))
        {
            myTarget.ExportLandsData();
        }
    }
}
