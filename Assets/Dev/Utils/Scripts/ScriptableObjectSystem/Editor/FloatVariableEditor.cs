using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatVariable))]
[CanEditMultipleObjects]
public class FloatVariableEditor : Editor
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

        var myTarget = (FloatVariable)target;
        myTarget.Value = _value.floatValue;

        serializedObject.ApplyModifiedProperties();
    }
}
