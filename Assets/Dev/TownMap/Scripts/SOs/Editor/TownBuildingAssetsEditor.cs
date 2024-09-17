using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TownBuildingAssetsInfoList))]
public class TownBuildingAssetsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (TownBuildingAssetsInfoList)target;
        if (GUILayout.Button("Fill Missing Buildings Info"))
        {
            myTarget.FillMissingBuildingInfos();
        }
        if (GUILayout.Button("Auto Assign BuildingModel"))
        {
            myTarget.AutoAssignBuildingModel();
        }
        if (GUILayout.Button("Auto Assign BuildingSprite"))
        {
            myTarget.AutoAssignBuildingSprite();
        }

        DrawDefaultInspector();
    }
}
