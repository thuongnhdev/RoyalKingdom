using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UserBuildingProductionList))]
public class UserBuildingProductionListEditor : Editor
{
    private GameEvent _requestUpdateBuildingProductionEvent;
    private string _appliedBuildingObjId;

    public override void OnInspectorGUI()
    {
        _appliedBuildingObjId = EditorGUILayout.TextField("target building object Id", _appliedBuildingObjId);
        if (GUILayout.Button($"Apply Changes to [{_appliedBuildingObjId}]"))
        {
            var myTarget = (UserBuildingProductionList)target;
            int.TryParse(_appliedBuildingObjId, out int buildingObjId);
            if (buildingObjId == 0)
            {
                return;
            }

            var production = myTarget.GetBuildingProduction(buildingObjId);
            if (production == null)
            {
                return;
            }

            _requestUpdateBuildingProductionEvent.Raise(production);
        }

        DrawDefaultInspector();
    }

    private void OnEnable()
    {
        _requestUpdateBuildingProductionEvent = AssetDatabase.LoadAssetAtPath<GameEvent>("Assets\\Dev\\TownMap\\SOs\\BuildingProduction\\Events\\Evt_BuildingProduction_RequestUpdateProduction.asset");
    }
}
