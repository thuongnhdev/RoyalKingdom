using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
[CanEditMultipleObjects]
public class GameEventEditor : Editor
{
    const string QUALIFIED_NAME = "{0}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var myTarget = (GameEvent)target;

        if (GUILayout.Button("Raise"))
        {
            myTarget.Raise();
        }

        if (GUILayout.Button("Raise With Params"))
        {
            RaiseWithParam(myTarget);
        }
    }

    private void RaiseWithParam(GameEvent myTarget)
    {
        List<GameEvent.EditorEventParam> args = myTarget._eventParams;
        object[] paramObjects = new object[args.Count];
        for (int i = 0; i < args.Count; i++)
        {
            var param = args[i];
            object paramObj;
            if (param.valueOrJson.StartsWith("{") && param.valueOrJson.EndsWith("}"))
            {
                paramObj = JsonParse(param.valueOrJson, param.className);
                paramObjects[i] = paramObj;
            }
            else
            {
                paramObj = PrimitiveParse(param.valueOrJson, param.className);
            }

            paramObjects[i] = paramObj;
        }

        myTarget.Raise(paramObjects);
    }

    private object JsonParse(string jsonString, string className)
    {
        string classFullName = string.Format(QUALIFIED_NAME, className);
        Type type = Type.GetType(classFullName);
        if (type == null)
        {
            Debug.Log($"Invalid class Name {classFullName}");
            return null;
        }

        return JsonUtility.FromJson(jsonString, type);
    }

    private object PrimitiveParse(string valueString, string typeName)
    {
        typeName = typeName.ToLower();
        if (typeName == "int")
        {
            int.TryParse(valueString, out int result);
            return result;
        }
        if (typeName == "float")
        {
            float.TryParse(valueString, out float result);
            return result;
        }
        if (typeName == "long")
        {
            long.TryParse(valueString, out long result);
            return result;
        }

        return valueString;
    }
}
