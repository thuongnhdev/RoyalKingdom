using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTooltipTrigger : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _tooltipForItem;
    [SerializeField]
    private Vector3Variable _tooltipPosition;
    [SerializeField]
    private Vector3Variable _tooltipOffset;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onOpenItemTooltip;

    [Header("Config")]
    [SerializeField]
    private bool _isScreenSpace = true;
    [SerializeField]
    private ResourceIconAndLabel _itemDisplay;
    [SerializeField]
    private Vector3 _tooltipOffsetConfig;

    public void TriggerTooltip()
    {
        _tooltipForItem.Value = _itemDisplay.ItemId;

        var camGetter = CameraGetter.Instance;
        Camera renderedCam = _isScreenSpace ? camGetter.UICamera : camGetter.MainCamera;
        _tooltipPosition.Value = renderedCam.WorldToScreenPoint(_itemDisplay.transform.position);
        _tooltipOffset.Value = _tooltipOffsetConfig;
        _onOpenItemTooltip.Raise();
    }
}
