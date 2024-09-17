using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoolVariable))]
public class BoolVariableEditor : Editor
{
    private SerializedProperty _value;

    private void OnEnable()
    {
        _value = serializedObject.FindProperty("_value");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.PropertyField(_value);

        var myTarget = (BoolVariable)target;
        myTarget.Value = _value.boolValue;

        serializedObject.ApplyModifiedProperties();
    }
}
