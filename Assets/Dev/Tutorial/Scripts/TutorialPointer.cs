using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPointer : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Configs")]
    [SerializeField]
    private Vector2 _offset;
    [SerializeField]
    private RectTransform _pointerContainer;
    [SerializeField]
    private Image _pointImage;
    [SerializeField]
    private Animator _animator;

    [Header("Inspec")]
    [SerializeField]
    private Canvas _rootCanvas;

    public void PointToPosition(Vector3 worldPosition)
    {
        Vector2 pointedPos = ObjectScreenPositionConverter.Instance.MapPositionToCanvasPosition(worldPosition, _rootCanvas);
        _pointerContainer.localPosition = pointedPos + _offset;
    }

    public void PointToUiTarget(RectTransform target)
    {
        if (target == null)
        {
            return;
        }

        Vector3 pos = target.position;
        pos.z = _pointerContainer.position.z;

        _pointerContainer.pivot = target.pivot;
        _pointerContainer.position = pos + new Vector3(_offset.x, _offset.y, 0f);
        ActivePointImage(true);
    }

    public void PointToUiTargetDistance(RectTransform target,float x, float y)
    {
        if (target == null)
        {
            return;
        }

        Vector3 pos = target.position;
        pos.z = _pointerContainer.position.z;

        _pointerContainer.pivot = target.pivot;
        _pointerContainer.position = pos + new Vector3(_offset.x + x, _offset.y +y, 0f);
        ActivePointImage(true);
    }

    public void PointToBuilding(int buildingId)
    {
        List<int> buildings = _userBuildings.GetAllBuildingsOfId(buildingId);
        if (buildings.Count == 0)
        {
            return;
        }

        Vector2 pointedPos = ObjectScreenPositionConverter.Instance.MapBuildingPositionToCanvasPosition(buildings[0], _rootCanvas);
        _pointerContainer.localPosition = pointedPos + _offset;
        ActivePointImage(true);
    }

    public void PointToBuidlingCommand1Button(int buildingId)
    {
        List<int> buildings = _userBuildings.GetAllBuildingsOfId(buildingId);
        if (buildings.Count == 0)
        {
            return;
        }

        Vector2 pointedPos = ObjectScreenPositionConverter.Instance.MapCommand1ButtonPositionToCanvasPosition(buildings[0], _rootCanvas);
        _pointerContainer.transform.localPosition = pointedPos + _offset;
        ActivePointImage(true);
    }

    public void PointToBuildingCommand2Button(int buildingId)
    {
        List<int> buildings = _userBuildings.GetAllBuildingsOfId(buildingId);
        if (buildings.Count == 0)
        {
            return;
        }

        Vector2 pointedPos = ObjectScreenPositionConverter.Instance.MapCommand2ButtonPositionToCanvasPosition(buildings[0], _rootCanvas);
        _pointerContainer.transform.localPosition = pointedPos + _offset;
        ActivePointImage(true);
    }

    public void BCM_HighlightBuilding(int buildingId)
    {
        RectTransform buildingCellPos = ObjectScreenPositionConverter.Instance.BCM_GetBuildingPosition(buildingId);
        PointToUiTarget(buildingCellPos);
    }

    public void BPM_HighlightItem(int itemId)
    {
        RectTransform buildingCellPos = ObjectScreenPositionConverter.Instance.BPM_GetItemPosition(itemId);
        PointToUiTarget(buildingCellPos);
    }

    public void BPM_HighlightItemByIndex(int index)
    {
        RectTransform buildingCellPos = ObjectScreenPositionConverter.Instance.BPM_GetItemPositionByIndex(index);
        PointToUiTarget(buildingCellPos);
    }

    public void BandMenu_HighlightBandCell(int index)
    {
        RectTransform bandCell = ObjectScreenPositionConverter.Instance.BandUI_GetBandRect(index);
        PointToUiTarget(bandCell);
    }

    public void ActivePointImage(bool active)
    {
        _pointImage.enabled = active;
        _animator.speed = active ? 1f : 0f;
    }

    private void OnEnable()
    {
        _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        _animator.speed = 0f;
    }

    private void OnValidate()
    {
        if (_pointImage != null)
        {
            return;
        }

        _pointImage = GetComponent<Image>();
    }
}
