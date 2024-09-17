using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBlocker : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingList _userBuilding;

    [Header("Configs")]
    [SerializeField]
    private float _highlightTime = 0.2f;
    [SerializeField]
    private Ease _ease;
    [SerializeField]
    private Ease _hideEase;
    [SerializeField]
    private Ease _showFadeEase;
    [SerializeField][Tooltip("RectTransform of UI object that needs to be highlighted, it's optional")]
    private RectTransform _spotLightRect;
    [SerializeField]
    private RectTransform _left;
    [SerializeField]
    private RectTransform _right;
    [SerializeField]
    private RectTransform _top;
    [SerializeField]
    private RectTransform _bot;
    [SerializeField]
    private RectTransform _highlight;
    [SerializeField]
    private Vector2 _hightlightOffset;
    [SerializeField]
    private Image _fullScreenBlocker;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Inspec")]
    [SerializeField]
    private Canvas _rootCanvas;

    private Vector2 _originalHighlightSize;

    public void HighlighToPosition(Vector3 worldPosition)
    {
        Vector2 pointedPos = ObjectScreenPositionConverter.Instance.MapPositionToCanvasPosition(worldPosition, _rootCanvas);
        _highlight.localPosition = pointedPos + new Vector2(_hightlightOffset.x, _hightlightOffset.y);
        TweenHighlighter();
    }
    public void HighlightUiElement(RectTransform target)
    {
        _spotLightRect = target;
        SetUpSpotlightRect();
        TweenHighlighter();
    }

    public void HighlightBuilding(int buildingId)
    {
        List<int> candidates = _userBuilding.GetAllBuildingsOfId(buildingId);
        if (candidates.Count == 0)
        {
            return;
        }

        Vector2 buildingScreenPos = ObjectScreenPositionConverter.Instance.MapBuildingPositionToCanvasPosition(candidates[0], _rootCanvas);
        _highlight.localPosition = buildingScreenPos + new Vector2(_hightlightOffset.x, _hightlightOffset.y);
        TweenHighlighter();
    }

    public void HighlightBuildingCommand1Button(int buildingId)
    {
        List<int> candidates = _userBuilding.GetAllBuildingsOfId(buildingId);
        if (candidates.Count == 0)
        {
            return;
        }

        Vector2 buildingScreenPos = ObjectScreenPositionConverter.Instance.MapCommand1ButtonPositionToCanvasPosition(candidates[0], _rootCanvas);
        _highlight.localPosition = buildingScreenPos + new Vector2(_hightlightOffset.x, _hightlightOffset.y);
        TweenHighlighter();
    }

    public void HightlightBuildingCommand2Button(int buildingId)
    {
        List<int> candidates = _userBuilding.GetAllBuildingsOfId(buildingId);
        if (candidates.Count == 0)
        {
            return;
        }

        Vector2 buildingScreenPos = ObjectScreenPositionConverter.Instance.MapCommand2ButtonPositionToCanvasPosition(candidates[0], _rootCanvas);
        _highlight.localPosition = buildingScreenPos + new Vector2(_hightlightOffset.x, _hightlightOffset.y);
        TweenHighlighter();
    }

    public void BCM_HighlightBuilding(int buildingId)
    {
        _spotLightRect = ObjectScreenPositionConverter.Instance.BCM_GetBuildingPosition(buildingId);
        SetUpSpotlightRect();
        TweenHighlighter();
    }

    public void BPM_HighlightItem(int itemId)
    {
        _spotLightRect = ObjectScreenPositionConverter.Instance.BPM_GetItemPosition(itemId);
        SetUpSpotlightRect();
        TweenHighlighter();
    }

    public void BPM_HighlightItemByIndex(int index)
    {
        _spotLightRect = ObjectScreenPositionConverter.Instance.BPM_GetItemPositionByIndex(index);
        SetUpSpotlightRect();
        TweenHighlighter();
    }

    public void BandMenu_HighlightBandCell(int index)
    {
        _spotLightRect = ObjectScreenPositionConverter.Instance.BandUI_GetBandRect(index);
        SetUpSpotlightRect();
        TweenHighlighter();
    }

    public void RunStopHighlightTween(bool disableAfterHide)
    {
        _fullScreenBlocker.raycastTarget = true;
        _canvasGroup.DOFade(0f, _highlightTime).SetEase(_hideEase);
        _highlight.DOSizeDelta(_rootCanvas.GetComponent<RectTransform>().rect.size, _highlightTime).SetEase(_ease).OnUpdate(() =>
        {
            AdjustBlockerFollowingHighlight();
        }).OnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = !disableAfterHide;
        });
    }

    private void TweenHighlighter()
    {
        PreHighlightSetUp();

        AdjustBlockerFollowingHighlight();
        _fullScreenBlocker.raycastTarget = true;

        _canvasGroup.DOFade(1f, _highlightTime).SetEase(_showFadeEase);
        _highlight.sizeDelta = _rootCanvas.GetComponent<RectTransform>().rect.size * 2f;
        _highlight.DOSizeDelta(_originalHighlightSize, _highlightTime).SetEase(_ease)
            .OnUpdate(() =>
        {
            AdjustBlockerFollowingHighlight();
        }).OnStepComplete(() =>
        {
            _fullScreenBlocker.raycastTarget = false;
        });
    }

    private void AdjustBlockerFollowingHighlight()
    {
        Rect highlightRect = _highlight.rect;
        if (_spotLightRect != null)
        {
            _highlight.position = _spotLightRect.position + new Vector3(_hightlightOffset.x, _hightlightOffset.y, 0f);
        }
        Vector3 highLightPos = _highlight.localPosition;

        float leftSizeDelta = highLightPos.x - (highlightRect.width * _highlight.pivot.x) - _left.localPosition.x;
        float rightSizeDelta = _right.localPosition.x - (highLightPos.x + (highlightRect.width * (1f - _highlight.pivot.x)));
        _left.sizeDelta = new(leftSizeDelta, _left.sizeDelta.y);
        _right.sizeDelta = new(rightSizeDelta, _right.sizeDelta.y);

        float topSizeDelta = _top.localPosition.y - (highLightPos.y + highlightRect.height * (1f - _highlight.pivot.y));
        float botSizeDelta = highLightPos.y - highlightRect.height * _highlight.pivot.y - _bot.localPosition.y;
        _top.sizeDelta = new(_top.sizeDelta.x, topSizeDelta);
        _bot.sizeDelta = new(_bot.sizeDelta.x, botSizeDelta);
    }

    private void PreHighlightSetUp()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = true;
    }

    private void SetUpSpotlightRect()
    {
        if (_spotLightRect == null)
        {
            return;
        }

        Vector3 pos = _spotLightRect.position;
        pos.z = _highlight.position.z;
        _highlight.pivot = _spotLightRect.pivot;
        _highlight.position = pos;
    }

    private void OnEnable()
    {
        _originalHighlightSize = _highlight.sizeDelta;
        AdjustBlockerFollowingHighlight();
        _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        _canvasGroup.alpha = 0f;
    }

    private void OnDrawGizmos()
    {
        AdjustBlockerFollowingHighlight();
    }
}
