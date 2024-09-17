using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DraggableTilingMovement : MonoBehaviour
{
    [Header("Reference - Read")]
    [Header("Inherit fields")]
    [SerializeField]
    protected TownMapSO _userTownMapSO;

    [Header("Reference - Write")]
    [SerializeField]
    protected BoolVariable _isValidPosition;

    [Header("Config")]
    [SerializeField]
    protected TilingTransform _tileTransform;
    [SerializeField]
    protected Vector2 _size;

    [SerializeField]
    protected UnityEvent _onValidPosition;
    [SerializeField]
    protected UnityEvent _onInvalidPosition;

    protected int _lastPointedTileId = -1;

    /// <summary>
    /// Call this OnPointerDown of Draggable Component
    /// </summary>
    public void In_OnPointerDown(Vector3 userPointedPos)
    {
        int mapX = _userTownMapSO.xSize;
        int mapY = _userTownMapSO.ySize;
        _lastPointedTileId = TownMapHelper.GetTileIdAtPosition(mapX, mapY, userPointedPos);
    }

    /// <summary>
    /// Call this OnDrag of Draggable Component
    /// </summary>
    public void In_Drag(Vector3 userPointedPos)
    {
        // NOTE: userPointedPos is world position. Currently, world position and local position is same, so this logic is acceptable.
        // TODO Need to transform the world position to local position (root object is tile holder)

        int mapX = _userTownMapSO.xSize;
        int mapY = _userTownMapSO.ySize;
        int pointedTileId = TownMapHelper.GetTileIdAtPosition(mapX, mapY, userPointedPos);

        if (pointedTileId == -1 || pointedTileId == _lastPointedTileId)
        {
            return;
        }

        Vector2 moveDirection = TownMapHelper.GetDirectionOf2Tiles(mapX, mapY, _lastPointedTileId, pointedTileId);
        _tileTransform.MoveBy(moveDirection);

        _lastPointedTileId = pointedTileId;
    }

    /// <summary>
    /// Call this OnEndDrag of Draggable Component
    /// </summary>
    public void In_OnEndDrag(Vector3 userPointedPos)
    {
        ActionOnEndOfMove(userPointedPos);
    }

    public void MoveTo(int tileId)
    {
        _tileTransform.MoveTo(tileId);
    }

    public void SetUnitSize(Vector2 size)
    {
        _size = size;
        _tileTransform.SetObjectSize((int)(size.x), (int)(size.y));
    }

    protected virtual bool ValidatePosition(int currentTileIdPosition)
    {
        return true;
    }

    protected virtual void ActionOnEndOfMove(Vector3 userPointedPos)
    {

    }

    protected virtual void DoOnEnable()
    {

    }

    protected virtual void DoOnDisable()
    {

    }

    private void ValidatePositionAndRaiseEvent(int tileId)
    {
        _isValidPosition.Value = ValidatePosition(tileId);

        if (!_isValidPosition.Value)
        {
            _onInvalidPosition.Invoke();
            return;
        }

        _onValidPosition.Invoke();
    }

    private void OnEnable()
    {
        _tileTransform.OnChangedTile += ValidatePositionAndRaiseEvent;
        DoOnEnable();
    }

    private void OnDisable()
    {
        _tileTransform.OnChangedTile -= ValidatePositionAndRaiseEvent;
        DoOnDisable();
    }
}
