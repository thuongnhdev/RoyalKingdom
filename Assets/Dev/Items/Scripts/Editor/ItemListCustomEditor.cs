using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemList))]
public class ItemListCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (ItemList)target;
        if (GUILayout.Button("Sort"))
        {
            Sort(myTarget.Items);
            EditorUtility.SetDirty(myTarget);
        }

        DrawDefaultInspector();
    }

    private void Sort(List<ItemPoco> items)
    {
        items.Sort(delegate (ItemPoco item1, ItemPoco item2)
        {
            return string.Compare(item1.name, item2.name);
        });

    }
}
