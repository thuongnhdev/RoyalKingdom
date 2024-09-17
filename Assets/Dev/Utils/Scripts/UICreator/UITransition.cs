using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(UIElement))]
public class UITransition : MonoBehaviour
{
    [SerializeField]
    private TransitionType _transitionType;

    [SerializeField]
    private float _showDelay = 0f;
    [SerializeField]
    private Vector3 _showFrom;
    [SerializeField]
    private Vector3 _showTo;
    [SerializeField]
    private float _showTransitionTime;
    [SerializeField]
    private Ease _showEaseType;
 
    [SerializeField]
    private float _hideDelay = 0f;
    [SerializeField]
    private Vector3 _hideFrom;
    [SerializeField]
    private Vector3 _hideTo;
    [SerializeField]
    private float _hideTransitionTime;
    [SerializeField]
    private Ease _hideEaseType;

    [SerializeField]
    private UnityEvent _onStartShow;
    [SerializeField]
    private UnityEvent _onFinishShow;
    [SerializeField]
    private UnityEvent _onStartHide;
    [SerializeField]
    private UnityEvent _onFinishHide;

    private UIPanel _parentPanel;
    private CanvasGroup _canvasGroup;
    private Animator _animator;

    public void Init(UIPanel parent)
    {
        _parentPanel = parent;
        _canvasGroup = GetComponent<CanvasGroup>();
        _animator = GetComponent<Animator>();
    }

    public void PreShowSetup()
    {
        switch (_transitionType)
        {
            case TransitionType.Fade:
                {
                    if (_canvasGroup == null)
                    {
                        Debug.LogWarning("In order to use Fade Transition, add CanvasGroup Component to this object", this);
                        return;
                    }

                    _canvasGroup.alpha = _showFrom.x;
                    break;
                }
            case TransitionType.Move:
                {
                    transform.localPosition = _showFrom;
                    break;
                }
            case TransitionType.Zoom:
                {
                    transform.localScale = _showFrom;
                    break;
                }
            case TransitionType.Animation:
                {
                    break;
                }
            default:
                {
                    return;
                }
        }
    }

    public async UniTaskVoid ShowTransition()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_showDelay));
        _onStartShow.Invoke();

        switch (_transitionType)
        {
            case TransitionType.Fade:
                {
                    if (_canvasGroup == null)
                    {
                        Debug.LogWarning("In order to use Fade Transition, add CanvasGroup Component to this object", this);
                        return;
                    }

                    _canvasGroup.DOKill();
                    _canvasGroup.DOFade(_showTo.x, _showTransitionTime).SetEase(_showEaseType).onComplete = NotifyFinishShow;
                    break;
                }
            case TransitionType.Move:
                {
                    transform.DOKill();
                    transform.DOLocalMove(_showTo, _showTransitionTime).SetEase(_showEaseType).onComplete = NotifyFinishShow;
                    break;
                }
            case TransitionType.Zoom:
                {
                    transform.DOKill();
                    transform.DOScale(_showTo.x, _showTransitionTime).SetEase(_showEaseType).onComplete = NotifyFinishShow;
                    break;
                }
            case TransitionType.Animation:
                {
                    if (_animator == null)
                    {
                        Debug.LogWarning("In order to use Animation Transition, add Animator Component to this object", this);
                        return;
                    }

                    _animator.Play("Open");

                    await UniTask.DelayFrame(1);
                    float clipTime = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                    await UniTask.Delay(TimeSpan.FromSeconds(clipTime));

                    NotifyFinishShow();
                    break;
                }
            default:
                {
                    NotifyFinishShow();
                    return;
                }
        }
    }

    public async UniTaskVoid HideTransition()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_hideDelay));
        _onStartHide.Invoke();
        
        switch (_transitionType)
        {
            case TransitionType.Fade:
                {
                    if (_canvasGroup == null)
                    {
                        Debug.LogWarning("In order to use Fade Transition, add CanvasGroup Component to this object", this);
                        return;
                    }

                    _canvasGroup.DOKill();
                    _canvasGroup.DOFade(_hideTo.x, _hideTransitionTime).SetEase(_hideEaseType).onComplete = NotifyFinishHide;
                    break;
                }
            case TransitionType.Move:
                {
                    transform.DOKill();
                    transform.DOLocalMove(_hideTo, _hideTransitionTime).SetEase(_hideEaseType).onComplete = NotifyFinishHide;
                    break;
                }
            case TransitionType.Zoom:
                {
                    transform.DOKill();
                    transform.DOScale(_hideTo, _hideTransitionTime).SetEase(_hideEaseType).onComplete = NotifyFinishHide;
                    break;
                }
            case TransitionType.Animation:
                {
                    if (_animator == null)
                    {
                        Debug.LogWarning("In order to use Animation Transition, add Animator Component to this object", this);
                        return;
                    }

                    _animator.Play("Close");

                    await UniTask.DelayFrame(1);
                    float clipTime = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                    await UniTask.Delay(TimeSpan.FromSeconds(clipTime));

                    NotifyFinishHide();
                    break;
                }
            default:
                {
                    NotifyFinishHide();
                    return;
                }
        }
    }

    private void NotifyFinishShow()
    {
        _onFinishShow.Invoke();
        _parentPanel.NotifyElementShowed();
    }

    private void NotifyFinishHide()
    {
        _onFinishHide.Invoke();
        _parentPanel.NotifyElementHided();
    }
}

public enum TransitionType
{
    None = 0,
    Move = 1,
    Zoom = 2,
    Fade = 3,
    Animation = 4
}
