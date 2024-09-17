using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueuedProduct : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private VassalSpriteSOList _vassalAssets;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _removedProductQueueIndex;
    [SerializeField]
    private IntegerVariable _draggedProductOldIndex;

    [Header("Configs")]
    [SerializeField]
    private Image _grade;
    [SerializeField]
    private Image _itemIcon;
    [SerializeField]
    private Image _prefix;
    [SerializeField]
    private Button _cancelButton;

    private int _queueIndex = -1;

    public class Data
    {
        public int queueIndex;
        public int itemId;
        public int prefix;
    }

    public void SetUp(Data data)
    {
        ActiveContent(true);

        _queueIndex = data.queueIndex;
        _itemIcon.overrideSprite = _itemAssets.GetItemSprite(data.itemId);
        _prefix.overrideSprite = _vassalAssets.GetPortrailSprite(data.prefix);
        _grade.overrideSprite = _itemAssets.GetItemGradeSprite(data.itemId);
    }

    public void RemoveThisFromQueue()
    {
        if (_queueIndex == -1)
        {
            return;
        }

        _removedProductQueueIndex.Value = _queueIndex;
    }

    public void HideContentForDragging()
    {
        _draggedProductOldIndex.Value = _queueIndex;
        ActiveContent(false);
    }

    public void ShowContentForEndDrag()
    {
        ActiveContent(true);
    }

    public void ActiveContent(bool active)
    {
        _itemIcon.enabled = active;
        _prefix.enabled = active;
        _grade.enabled = active;
        _cancelButton.gameObject.SetActive(active);
    }

}
