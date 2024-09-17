using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemAssetList))]
public class ItemAssetListCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myTarget = (ItemAssetList)target;
        if (GUILayout.Button("Auto Assign Sprite"))
        {
            AutoAssign(myTarget.ItemsAssets);
            EditorUtility.SetDirty(myTarget);
        }
        DrawDefaultInspector();    
    }

    private void AutoAssign(List<ItemAssetPoco> items)
    {
        string[] itemsGuId = AssetDatabase.FindAssets("t:sprite", new string[] { "Assets\\Art\\UI_Sprite\\Item" });
        Dictionary<string, ItemAssetPoco> itemDict = new();
        for (int i = 0; i < items.Count; i++)
        {
            itemDict[items[i].editorName.ToLower()] = items[i];
        }

        for (int i = 0; i < itemsGuId.Length; i++)
        {
            string guId = itemsGuId[i];
            Sprite itemSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guId));

            if (itemSprite == null)
            {
                return;
            }

            string itemName = itemSprite.name.Split("_")[^1].ToLower();
            itemDict.TryGetValue(itemName, out var item);
            if (item == null)
            {
                Debug.LogWarning($"Sprite [{itemSprite.name}] cannot be assigned", itemSprite);
                continue;
            }

            item.sprite = itemSprite;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].sprite == null)
            {
                Debug.LogWarning($"Item [{items[i].editorName}] does not have sprite");
            }
        }
    }
}
