using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIPanel))]
public class UITooltip : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _tooltipScreenPosition;
    [SerializeField]
    private Vector3Variable _tooltipOffset;

    [Header("Config")]
    [SerializeField]
    private Canvas _rootCanvas;
    [SerializeField]
    private UIPanel _refUiPanel;
    [SerializeField]
    private RectTransform _boundary;
    [SerializeField]
    private RectTransform[] _paddingRects;
    [SerializeField]
    private Padding _defaultPadding;

    [Header("Inspec")]
    [SerializeField]
    private Vector3[] _originPaddingPositions;
    [SerializeField]
    private Padding _padding;
    public Padding CurrentPadding => _padding;
    public enum Padding
    {
        TopLeft = 0,
        TopRight = 1,
        BottomRight = 2,
        BottomLeft = 3
    }

    private Vector2 _canvasHalfScaledSize;
    private Vector2 _screenPixelPosToPos;
    Vector2 _canvasMin;
    Vector2 _canvasMax;
    private CancellationTokenSource _updateToken;

    public void Open()
    {
        Delay_Open().Forget();
    }
    private async UniTaskVoid Delay_Open()
    {
        await UniTask.DelayFrame(2); // To avoid open and close at same frame
        _refUiPanel.Open();
        SetUp();
        TrackUserClick();
    }
    private void SetUp()
    {
        Vector3 toolTipScreenPos = _tooltipScreenPosition.Value;
        toolTipScreenPos.x *= _screenPixelPosToPos.x;
        toolTipScreenPos.y *= _screenPixelPosToPos.y;
        toolTipScreenPos.x -= _canvasHalfScaledSize.x;
        toolTipScreenPos.y -= _canvasHalfScaledSize.y;
        toolTipScreenPos.z = 0f;

        if (IsPaddingInScreenArea(_defaultPadding, toolTipScreenPos, out Vector3 considerPos))
        {
            _boundary.localPosition = considerPos;
            _padding = _defaultPadding;
            return;
        }

        for (int i = 0; i < _originPaddingPositions.Length; i++)
        {
            Padding padding = (Padding)i;
            if (IsPaddingInScreenArea(padding, toolTipScreenPos, out considerPos))
            {
                _boundary.localPosition = considerPos;
                _padding = padding;
                break;
            }
        }
    }
    private bool IsPaddingInScreenArea(Padding padding, Vector3 toolTipScreenPos, out Vector3 considerPos)
    {
        considerPos = _originPaddingPositions[(int)padding] + toolTipScreenPos;
        considerPos += CalculateOffsetForPadding(padding, _tooltipOffset.Value);

        return IsAreaInScreenArea(considerPos, _boundary.rect.size);
    }

    private bool IsAreaInScreenArea(Vector2 position, Vector2 size)
    {
        Vector2 min;
        Vector2 max;
        min.x = position.x - size.x / 2f;
        min.y = position.y + size.y / 2f;
        max.x = min.x + size.x;
        max.y = min.y - size.y;

        return _canvasMin.x < min.x && min.x < _canvasMax.x
            && _canvasMax.y < min.y && min.y < _canvasMin.y
            && _canvasMin.x < max.x && max.x < _canvasMax.x
            && _canvasMax.y < max.y && max.y < _canvasMin.y;
    }

    public void TrackUserClick()
    {
        PerFrame_TrackClick().Forget();
    }

    public void StopTrackUserClick()
    {
        _updateToken?.Cancel();
    }

    private async UniTaskVoid PerFrame_TrackClick()
    {
        await UniTask.NextFrame();

        _updateToken?.Cancel();
        _updateToken = new();

        Camera uiCam = _rootCanvas.worldCamera;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            if (!Input.GetMouseButtonDown(0))
            {
                continue;
            }

            Vector3 clickedScreenPos = Input.mousePosition;
            bool isClickInTooltipArea = RectTransformUtility.RectangleContainsScreenPoint(_boundary, clickedScreenPos, uiCam);

            if (isClickInTooltipArea)
            {
                continue;
            }

            _refUiPanel.Close();
            StopTrackUserClick();
        }
    }

    private Vector3 CalculateOffsetForPadding(Padding padding, Vector3 offsetAbsValue)
    {
        switch (padding)
        {
            case Padding.TopLeft:
                {
                    offsetAbsValue.x *= -1f;
                    break;
                }
            case Padding.BottomLeft:
                {
                    offsetAbsValue.x *= -1f;
                    offsetAbsValue.y *= -1f;
                    break;
                }
            case Padding.BottomRight:
                {
                    offsetAbsValue.y *= -1f;
                    break;
                }
        }

        return offsetAbsValue;
    }

    // Call this function after 1 first frame due to Canvas Scaler
    private void CalculateCanvasBoundAndScreenPixelToCanvasFactor()
    {
        Vector2 canvasScaledSize = _rootCanvas.GetComponent<RectTransform>().rect.size;
        Camera renderCam = _rootCanvas.worldCamera;

        _screenPixelPosToPos.x = canvasScaledSize.x / renderCam.pixelWidth;
        _screenPixelPosToPos.y = canvasScaledSize.y / renderCam.pixelHeight;

        _canvasHalfScaledSize = canvasScaledSize / 2f;
        _canvasMin = new(-_canvasHalfScaledSize.x, _canvasHalfScaledSize.y);
        _canvasMax = new(_canvasHalfScaledSize.x, -_canvasHalfScaledSize.y);
    }

    private void OnEnable()
    {
        _originPaddingPositions = new Vector3[4];
        for (int i = 0; i < _paddingRects.Length; i++)
        {
            _originPaddingPositions[i] = _paddingRects[i].localPosition;
        }
    }

    private void Start()
    {
        CalculateCanvasBoundAndScreenPixelToCanvasFactor();
    }
}
