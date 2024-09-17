using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserBuildingList))]
public class UserBuildingListEditor : Editor
{
    private TownBaseBuildingSOList _buildings;
    private GameEvent _requestUpdateBuildingEvent;
    private string _appliedBuildingObjId;
    public override void OnInspectorGUI()
    {
        _appliedBuildingObjId = EditorGUILayout.TextField("target building object Id", _appliedBuildingObjId);
        if (GUILayout.Button("Apply Change"))
        {
            ApplyChange(_appliedBuildingObjId);
        }
        if (GUILayout.Button("Display BuildingName"))
        {
            DisplayBuildingName();
        }

        DrawDefaultInspector();
    }

    private void ApplyChange(string buildingObjectId)
    {
        int.TryParse(buildingObjectId, out int id);
        var userBuildings = (UserBuildingList)target;
        var userbuilding = userBuildings.GetBuilding(id);
        if (userBuildings == null)
        {
            return;
        }

        _requestUpdateBuildingEvent.Raise(userbuilding);
    }

    private void DisplayBuildingName()
    {
        if (_buildings == null)
        {
            Debug.Log("Master data building cannot be loaded");
            return;
        }

        var userBuildings = (UserBuildingList)target;
        var buildings = userBuildings.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            building.editorBuildingName = _buildings.GetBuildingName(building.buildingId);
        }
    }

    private void OnEnable()
    {
        _buildings = AssetDatabase.LoadAssetAtPath<TownBaseBuildingSOList>("Assets\\Dev\\TownMap\\SOs\\TownBuildingList.asset");
        _requestUpdateBuildingEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets\\Dev\\TownMap\\SOs\\Events\\BuildingEvents\\Evt_TownMap_OnRequestUpdateBuildingData.asset");
    }
}
