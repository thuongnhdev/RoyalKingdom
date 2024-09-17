using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    [SerializeField]
    private float _showDelay;
    [SerializeField]
    private float _hideDelay;

    private UIPanel _parentPanel;

    private UITransition _transition;

    public void Init(UIPanel container)
    {
        _parentPanel = container;

        _transition = GetComponent<UITransition>();

        if (_transition == null)
        {
            transform.localScale = Vector3.zero;
            return;
        }
        _transition.Init(container);
    }

    public void Show()
    {
        Async_Show().Forget();
    }

    public void Hide()
    {
        Async_Hide().Forget();
    }

    private async UniTaskVoid Async_Show()
    {
        _transition?.PreShowSetup();

        await UniTask.Delay(System.TimeSpan.FromSeconds(_showDelay));
        if (_transition == null)
        {
            transform.localScale = Vector3.one;
            _parentPanel.NotifyElementShowed();
            return;
        }

        _transition.ShowTransition().Forget();
    }

    private async UniTaskVoid Async_Hide()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(_hideDelay));
        if (_transition == null)
        {
            transform.localScale = Vector3.zero;
            _parentPanel.NotifyElementHided();
            return;
        }

        _transition.HideTransition().Forget();
    }
}
