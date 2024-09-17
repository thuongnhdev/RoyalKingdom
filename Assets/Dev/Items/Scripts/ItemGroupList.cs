using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGroupList", menuName = "Uniflow/Resource/ItemGroupList")]
public class ItemGroupList : ScriptableObject
{
    [SerializeField]
    private List<ItemGroup> _itemGroups;

    public void Init(ApiGetLowData data)
    {
        int itemGroupCount = data.ItemGroupLength;
        _itemGroups.Clear();
        for (int i = 0; i < itemGroupCount; i++)
        {
            var groupData = data.ItemGroup(i).Value;
            _itemGroups.Add(new() { groupId = groupData.IdItemGroup, groupName = groupData.Name });
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public string GetGroupName(int itemId)
    {
        int groupId = ItemHelper.GetGroupItemId(itemId);
        for (int i = 0; i < _itemGroups.Count; i++)
        {
            var group = _itemGroups[i];
            if (group.groupId == groupId)
            {
                return group.groupName;
            }
        }

        Logger.Log($"No group for item with id [{itemId}]", Color.yellow);
        return "";
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
