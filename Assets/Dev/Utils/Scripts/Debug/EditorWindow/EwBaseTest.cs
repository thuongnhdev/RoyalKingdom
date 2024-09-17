#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EwBaseTest : EditorWindow
{
    #region Singleton
    private static EwBaseTest _instance;
    public static EwBaseTest Instance
    {
        get { return GetWindow<EwBaseTest>(); }
    }
    #endregion


    // Add menu named "My Window" to the Window menu
    [MenuItem("Uniflow/Common")]
    static void Init()
    {        
        EwBaseTest window = (EwBaseTest)EditorWindow.GetWindow(typeof(EwBaseTest));
        window.Show();
    }

    void OnGUI()
    {
        //GUILayout.Label("UID : " + StatesGlobal.UID_PLAYER.ToString(), EditorStyles.boldLabel);        
        //ReadOnlyTextField("BattleID", StatesBattle.BATTLE_ID);
        //myString = EditorGUILayout.TextField("Text Field", myString);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Temp"))
        {            
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        Application.runInBackground = EditorGUILayout.Toggle("runInBackground", Application.runInBackground);
        //GUILayout.BeginHorizontal();
        //if (EditorApplication.isPaused)
        //{
        //    if (GUILayout.Button("RESUME"))
        //    {
        //        PacketManager.Instance.OnPauseForEditor(false);
        //        EditorApplication.isPaused = false;                
        //    }
        //}
        //else
        //{
        //    if (GUILayout.Button("PAUSE"))
        //    {
        //        Application.runInBackground = false;                

        //    }
        //}             
        //GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        }
        //GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
                

        

        GUILayout.Space(20);
        
        


        //IsNoNetwork = EditorGUILayout.BeginToggleGroup("Disconnect Network", IsNoNetwork);
        //StatesGlobal.DEBUG_NO_NETWORK = IsNoNetwork;
        //EditorGUILayout.EndToggleGroup();



        //if(toolbarIndex == 2)
        //{
        //    groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //    myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //    myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //    EditorGUILayout.EndToggleGroup();
        //}
    }
    void ReadOnlyTextField(string label, string text)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
        EditorGUILayout.EndHorizontal();
    }
}

#endif