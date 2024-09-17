using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TilingTransform : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownMapSO _userTownMapSO;

    [Header("Config")]
    [SerializeField]
    private Transform _refTransform;
    [SerializeField]
    private Transform _refRotationTransform;
    [SerializeField]
    private BoxCollider _boxCollider;

    [Header("Inspec")]
    [SerializeField]
    private bool _isEvenXSize = false;
    [SerializeField]
    private bool _isEvenYSize = false;
    [SerializeField]
    private bool _isSquareSize = false;

    public delegate void OnChangedTileDel(int newTileId);
    public event OnChangedTileDel OnChangedTile;
    
    public void MoveBy(Vector2 direction, float xOffset = 1f, float yOffset = 1f)
    {
        if (direction == Vector2.zero)
        {
            return;
        }

        Vector3 newPos = new Vector3
        {
            x = _refTransform.position.x + direction.x * xOffset,
            y = 0f,
            z = _refTransform.position.z + direction.y * yOffset
        };

        _refTransform.position = newPos;

        int newTileId = TownMapHelper.GetTileIdAtPosition(_userTownMapSO.xSize, _userTownMapSO.ySize, _refTransform.localPosition, xOffset, yOffset);

        OnChangedTile?.Invoke(newTileId);
    }

    public void MoveTo(int destinationTileId, float xOffset = 1f, float yOffset = 1f)
    {
        if (destinationTileId == -1 || destinationTileId == GetCurrentTileId())
        {
            return;
        }

        Vector3 tilePos = TownMapHelper.GetTileLocalPosition(_userTownMapSO.xSize, _userTownMapSO.ySize, destinationTileId, xOffset, yOffset);

        if (_isEvenXSize)
        {
            tilePos.x -= xOffset / 2f;
        }

        if (_isEvenYSize)
        {
            tilePos.z += yOffset / 2f;
        }

        _refTransform.localPosition = tilePos;

        OnChangedTile?.Invoke(destinationTileId);
    }

    public void Rotate(bool clockWise = true)
    {
        float degree = _isSquareSize ? 90f : 180f;

        if (!clockWise)
        {
            degree *= -1f;
        }

        _refRotationTransform.Rotate(0f, degree, 0f);
    }

    public void RotateTo(float rotation)
    {
        if (!_isSquareSize)
        {
            rotation = Mathf.Abs(rotation) < 90f ? 0f : 180f;
        }

        _refRotationTransform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

    public void RotateToDirection(Vector2 direction)
    {
        float rotation = -90f;
        if (direction == Vector2.up)
        {
            rotation = 0f;
        }
        if (direction == Vector2.right)
        {
            rotation = 90f;
        }
        if (direction == Vector2.down)
        {
            rotation = 180f;
        }

        RotateTo(rotation);
    }

    public void RotateToOrigin()
    {
        _refRotationTransform.rotation = Quaternion.identity;
    }

    public int GetCurrentTileId()
    {
        return TownMapHelper.GetTileIdAtPosition(_userTownMapSO.xSize, _userTownMapSO.ySize, _refTransform.localPosition);
    }

    public Vector3 GetLocalPosition()
    {
        return _refTransform.localPosition;
    }

    public float GetRotation()
    {
        return _refRotationTransform.rotation.eulerAngles.y;
    }

    public BoxCollider GetBoxCollider()
    {
        return _boxCollider;
    }
    public void SetObjectSize(int xSize, int ySize)
    {
        _isEvenXSize = xSize % 2 == 0;
        _isEvenYSize = ySize % 2 == 0;

        _isSquareSize = xSize == ySize;
    }
}
