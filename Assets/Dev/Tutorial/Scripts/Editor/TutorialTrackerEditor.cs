using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TutorialTracker))]
public class TutorialTrackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Raise Tutorial Flag"))
        {
            var myTarget = (TutorialTracker)target;
            myTarget.SetTutorialFlag(true);
        }

        if (GUILayout.Button("Negate Tutorial Flag"))
        {
            var myTarget = (TutorialTracker)target;
            myTarget.SetTutorialFlag(false);
        }

        DrawDefaultInspector();
    }
}
