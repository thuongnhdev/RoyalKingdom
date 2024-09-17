using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Vector3Variable))]
public class Vector3VariableEditor : Editor
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

        var myTarget = (Vector3Variable)target;
        myTarget.Value = _value.vector3Value;

        serializedObject.ApplyModifiedProperties();
    }
}
