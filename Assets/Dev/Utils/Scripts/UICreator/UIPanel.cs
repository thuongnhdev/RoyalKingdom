using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UIPanel : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private List<UIElement> _elements;
    [SerializeField]
    private Image _clickBlocker;
    public bool HasClickBlocker => _clickBlocker != null;
    [SerializeField]
    private bool _showFromStart;
    [SerializeField]
    private bool _refreshWhenReopen = false;

    [Header("UnityEvents")]
    [SerializeField]
    private UnityEvent _onRefresh;
    [SerializeField]
    private UnityEvent _onStartShow;
    [SerializeField]
    private UnityEvent _onAllElementsShown;
    [SerializeField]
    private UnityEvent _onStartHide;
    [SerializeField]
    private UnityEvent _onAllElementsHided;
    [field: SerializeField]
    public PanelStatus Status { get; private set; }

    private UIController _uiController;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private GraphicRaycaster _rayCaster;

    [SerializeField]
    private int _showedElements = 0;

    [System.Serializable]
    public enum PanelStatus
    {
        IsClosing = 0b1,
        Closed = 0b10,
        IsOpening = 0b100,
        Opened = 0b1000,
    }

    private UIPanel _showNextAfterThisHide;
    private Color _clickBlockerColor;

    public bool ShowFromStart
    {
        get
        {
            return _showFromStart;
        }
        set
        {
            _showFromStart = value;
        }
    }
    public void Init(UIController controller)
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rayCaster = GetComponent<GraphicRaycaster>();
        _canvas.enabled = false;
        _canvasGroup.interactable = false;
        _rayCaster.enabled = false;

        _uiController = controller;

        if (!ValidateElements())
        {
            return;
        }

        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Init(this);
            var clickBlocker = _elements[i].GetComponent<Image>();
            if (clickBlocker == null)
            {
                continue;
            }
            _clickBlocker = clickBlocker;
            _clickBlockerColor = clickBlocker.color;
        }

        TrackGlobalClickBlocker();
    }

    public UIElement GetElement(int index)
    {
        if (_elements.Count == 0)
        {
            return null;
        }

        return _elements[index % _elements.Count];
    }

    public void Open()
    {
        if (Status == PanelStatus.Opened || Status == PanelStatus.IsOpening)
        {
            if (_refreshWhenReopen)
            {
                _onRefresh.Invoke();
            }
            return;
        }

        if (!ValidateElements())
        {
            return;
        }

        // Handle case: a Panel is asked to open while it is closing
        if (Status == PanelStatus.IsClosing)
        {
            ForcePanelToClosedState();
        }

        _canvas.enabled = true;
        _rayCaster.enabled = true;
        transform.SetAsLastSibling();

        Status = PanelStatus.IsOpening;

        _onStartShow.Invoke();
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Show();
        }

        _uiController.PushToStack(this);
        ActiveClickBlocker(true);
    }

    public void Close()
    {
        if (Status == PanelStatus.Closed || Status == PanelStatus.IsClosing)
        {
            return;
        }

        if (!ValidateElements())
        {
            return;
        }

        // Handle case: a Panel is asked to close while it is opening
        if (Status == PanelStatus.IsOpening)
        {
            ForcePanelToOpenedState();
        }

        Status = PanelStatus.IsClosing;
        _onStartHide.Invoke();
        // panel is on top, pop it.
        // if it is not on top, just close and it will be pop later
        if (_uiController.IsOnTop(this))
        {
            _uiController.PopFromStack();
        }

        _canvasGroup.interactable = false;
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].Hide();
        }

        ActiveClickBlocker(false);
    }

    public void CloseAndOpenOther(UIPanel other)
    {
        Close();

        _showNextAfterThisHide = other;
    }

    public void CloseAllButInitPanels()
    {
        _uiController.CloseAllButInitPanels();
    }

    public void NotifyElementShowed()
    {
        _showedElements++;
        if (_showedElements >= _elements.Count)
        {
            _showedElements = _elements.Count;
            FinishOpenProcess();
        }
    }

    private void FinishOpenProcess()
    {
        _canvasGroup.interactable = true;
        _onAllElementsShown?.Invoke();
        Status = PanelStatus.Opened;
    }

    public void NotifyElementHided()
    {
        _showedElements--;
        if (_showedElements > 0)
        {
            return;
        }

        _showedElements = 0;
        FinishCloseProcess();

        if (_showNextAfterThisHide == null)
        {
            return;
        }

        _showNextAfterThisHide.Open();
        _showNextAfterThisHide = null;
    }

    private void FinishCloseProcess()
    {
        _canvas.enabled = false;
        _rayCaster.enabled = false;
        _onAllElementsHided?.Invoke();
        Status = PanelStatus.Closed;
        transform.SetAsFirstSibling();
    }

    public void SetInteractable(bool interactable)
    {
        _canvasGroup.interactable = interactable;
    }

    public void SetMoveToFront()
    {
        transform.SetAsLastSibling();
    }

    public void ActiveClickBlocker(bool active)
    {
        if (_clickBlocker == null)
        {
            return;
        }
        _clickBlockerColor.a = active ? 0.83f : 0f;
        _clickBlocker.color = _clickBlockerColor;

        if (!_uiController.IsGlobalUI)
        {
            return;
        }
        if (active)
        {
            _uiController.NotifyGlobalClickerBlockerActive();
            return;
        }
        if (_uiController.ShowingPanelCount != 0)
        {
            return;
        }

        _uiController.NotifyAllGlobalClickBlockerInactive();
    }

    private void ForcePanelToOpenedState()
    {
        _showedElements = _elements.Count;
        _canvas.enabled = true;
        _rayCaster.enabled = true;
        transform.SetAsLastSibling();
        Status = PanelStatus.Opened;

        FinishOpenProcess();
    }

    private void ForcePanelToClosedState()
    {
        _showedElements = 0;
        _canvasGroup.interactable = false;

        FinishCloseProcess();
    }

    private bool ValidateElements()
    {
        if (_uiController == null)
        {
            return false;
        }

        if (_elements.Count == 0)
        {
            Debug.LogWarning($"No element found in {gameObject.name}", this);
            return false;
        }

        return true;
    }

    private void TrackGlobalClickBlocker()
    {
        if (_uiController.IsGlobalUI)
        {
            return;
        }

        UIController.OnGlobalClickerBlockerActive += AdaptWithActivatedGlobalClickerBlocker;
        UIController.OnAllGlobalClickerBlockersInactive += ApdaptWithInactiveGlobalClickerBlocker;
    }

    private void OnDisable()
    {
        if (_uiController == null || _uiController.IsGlobalUI)
        {
            return;
        }

        UIController.OnGlobalClickerBlockerActive -= AdaptWithActivatedGlobalClickerBlocker;
        UIController.OnAllGlobalClickerBlockersInactive -= ApdaptWithInactiveGlobalClickerBlocker;
    }

    private void AdaptWithActivatedGlobalClickerBlocker()
    {
        if (!_uiController.IsOnTop(this))
        {
            return;
        }

        ActiveClickBlocker(false);
    }

    private void ApdaptWithInactiveGlobalClickerBlocker()
    {
        if (!_uiController.IsOnTop(this))
        {
            return;
        }

        ActiveClickBlocker(true);
    }
}
