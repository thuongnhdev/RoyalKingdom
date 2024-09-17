using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ItemAssetList", menuName = "Uniflow/Resource/ItemAssetList")]
public class ItemAssetList : ScriptableObject
{
    [Header("Reference - Read")]
    [SerializeField]
    private ItemList _itemList;
    [SerializeField]
    private StarAndGradeAssetList _starAndGradeIcons;
    [SerializeField]
    private List<ItemAssetPoco> _itemsAssets;
    public List<ItemAssetPoco> ItemsAssets => _itemsAssets;
    [SerializeField]
    private List<ItemAssetPoco> _itemGroupAssets;
    private Dictionary<int, ItemAssetPoco> _itemsDict = new();
    private Dictionary<int, ItemAssetPoco> ItemDict
    {
        get
        {
            if (_itemsAssets.Count != _itemsDict.Count)
            {
                _itemsDict.Clear();

                for (int i = 0; i < _itemsAssets.Count; i++)
                {
                    var item = _itemsAssets[i];
                    _itemsDict[item.itemId] = item;
                }
            }

            return _itemsDict;
        }
    }


    public ItemAssetPoco GetItemAssets(int itemId)
    {
        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            Debug.LogError($"Invalid ItemId [{itemId}]");
        }

        return item;
    }

    public Sprite GetItemSprite(int itemId)
    {
        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            return GetItemGroupSprite(itemId);
        }

        return item.sprite;
    }

    public Sprite GetItemGradeSprite(int itemId)
    {
        if (ItemHelper.IsGroupItemId(itemId))
        {
            return _starAndGradeIcons.GetItemGroupGrade();
        }

        return _starAndGradeIcons.GetGradeIcon(_itemList.GetItemGrade(itemId));
    }

    public Sprite GetItemGradeWithStarSlot(int itemId)
    {
        if (ItemHelper.IsGroupItemId(itemId))
        {
            return _starAndGradeIcons.GetItemGroupGrade();
        }

        return _starAndGradeIcons.GetGradeWithSlotIcon(_itemList.GetItemGrade(itemId));
    }

    public Sprite GetItemStarBG(int itemId)
    {
        return _starAndGradeIcons.GetStarBG(_itemList.GetItemGrade(itemId));
    }

    public GameObject GetItemPrefab(int itemId)
    {
        ItemDict.TryGetValue(itemId, out var item);
        if (item == null)
        {
            return null;
        }

        return item.prefab;
    }

    private Sprite GetItemGroupSprite(int itemId)
    {
        for (int i = 0; i < _itemGroupAssets.Count; i++)
        {
            var itemAsset = _itemGroupAssets[i];
            if (itemAsset.itemId == itemId)
            {
                return itemAsset.sprite;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isUpdating)
        {
            return;
        }

        List<ItemPoco> items = _itemList.Items;

        if (items.Count == _itemsAssets.Count)
        {
            return;
        }

        _itemsAssets.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            int itemId = item.itemId;

            ItemDict.TryGetValue(itemId, out var itemAsset);
            if (itemAsset == null)
            {
                itemAsset = new ItemAssetPoco();
                _itemsAssets.Add(itemAsset);
                _itemsDict.Add(item.itemId, itemAsset);
            }

            itemAsset.itemId = item.itemId;
            itemAsset.editorName = item.name;
        }

        EditorUtility.SetDirty(this);
    }
#endif
}

[System.Serializable]
public class ItemAssetPoco
{
    public string editorName;
    public int itemId;
    public Sprite sprite;
    public GameObject prefab;
}
