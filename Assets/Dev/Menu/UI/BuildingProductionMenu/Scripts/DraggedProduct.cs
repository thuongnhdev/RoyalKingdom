using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggedProduct : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private IntegerVariable _draggedProductOldIndex;
    [SerializeField]
    private Vector3Variable _draggedPointScreenPos;
    [SerializeField]
    private ItemAssetList _itemAssets;
    [SerializeField]
    private UserBuildingProductionList _productions;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _draggedProductNewIndex;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _requestChangeProductQueueOrder;

    [Header("Configs")]
    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private RectTransform _parentPanel;
    [SerializeField]
    private ItemIconAndGrade _iconAndGrade;
    [SerializeField]
    private RectTransform _firstQueueProduct;
    [SerializeField]
    private RectTransform _secondQueueProduct;

    [Header("Inspec")]
    [SerializeField]
    private float _perItemWidth;
    [SerializeField]
    private float _leftBound;
    [SerializeField]
    private float _topBound;
    [SerializeField]
    private float _botBound;
    public void Active()
    {
        int draggedProduct = _productions.GetProductAtQueueIndex(_selectedBuildingObjId.Value, _draggedProductOldIndex.Value);
        if (draggedProduct == 0)
        {
            return;
        }

        _iconAndGrade.SetUp(_itemAssets.GetItemSprite(draggedProduct), _itemAssets.GetItemGradeSprite(draggedProduct));
    }

    public void RequestChangeQueueOrder()
    {
        Vector3 screenPos = _parentPanel.InverseTransformPoint(transform.position); /*_uiCamera.WorldToScreenPoint(transform.position);*/
        if (screenPos.y < _botBound || _topBound < screenPos.y)
        {
            return;
        }


        int newIndex = Mathf.CeilToInt((screenPos.x - _leftBound) / _perItemWidth) + 1; // UI Queue starts from 1, 0 is current product
        newIndex = Mathf.Clamp(newIndex, 1, 9);

        if (newIndex == _draggedProductOldIndex.Value)
        {
            return;
        }

        _draggedProductNewIndex.Value = newIndex;

        _requestChangeProductQueueOrder.Raise();
    }

    private void MoveDraggedProduct(Vector3 screenPos)
    {
        Vector3 itemImagePos = _uiCamera.ScreenToWorldPoint(screenPos);
        itemImagePos.z = transform.position.z;

        transform.position = itemImagePos;
    }

    private void CalculateSpacingAndLeftBound()
    {
        Vector3 firstScreenPos = _parentPanel.InverseTransformPoint(_firstQueueProduct.position); /*_uiCamera.WorldToScreenPoint(_firstQueueProduct.position);*/
        Vector3 secondScreenPos = _parentPanel.InverseTransformPoint(_secondQueueProduct.position); /*_uiCamera.WorldToScreenPoint(_secondQueueProduct.position);*/

        float spacing = secondScreenPos.x - firstScreenPos.x - _firstQueueProduct.rect.width;
        _leftBound = firstScreenPos.x;
        _topBound = firstScreenPos.y + _firstQueueProduct.rect.yMax * 2f;
        _botBound = firstScreenPos.y + _firstQueueProduct.rect.yMin * 2f;

        _perItemWidth = _firstQueueProduct.rect.width + spacing;
    }

    private void OnEnable()
    {
        CalculateSpacingAndLeftBound();
        _draggedPointScreenPos.OnValueChange += MoveDraggedProduct;
    }

    

    private void OnDisable()
    {
        _draggedPointScreenPos.OnValueChange -= MoveDraggedProduct;
    }
}
