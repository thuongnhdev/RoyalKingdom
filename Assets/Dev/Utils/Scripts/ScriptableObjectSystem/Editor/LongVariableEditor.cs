using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LongVariable))]
public class LongVariableEditor : Editor
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

        var myTarget = (LongVariable)target;
        myTarget.Value = _value.intValue;

        serializedObject.ApplyModifiedProperties();
    }
}
