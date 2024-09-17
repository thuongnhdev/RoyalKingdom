using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WorldMapStrategyKit;

public class WorldmapSceneUIPanel : MonoBehaviour
{
    [Header("Reference - Configs")]
    [SerializeField]
    protected GameObjectAnimator _goa;
    [SerializeField]
    protected Canvas _canvas;

    [SerializeField]
    protected UnityEvent _onOpen;
    [SerializeField]
    protected UnityEvent _onClose;

    protected bool _isOpening = false;

    public virtual void Open()
    {
        if (_isOpening)
        {
            return;
        }

        _isOpening = true;
        _onOpen.Invoke();
    }

    public virtual void Close()
    {
        if (!_isOpening)
        {
            return;
        }

        _isOpening = false;
        _onClose.Invoke();
    }

    protected virtual void DoOnEnable()
    {

    }

    protected virtual void DoOnDisable()
    {

    }

    private void OnEnable()
    {
        _goa.OnVisibleChange += UpdateVisible;
        DoOnEnable();
    }

    private void OnDisable()
    {
        _goa.OnVisibleChange -= UpdateVisible;
        DoOnDisable();
    }

    private void UpdateVisible(GameObjectAnimator anim)
    {
        _canvas.enabled = anim.isVisibleInViewport;
    }
}
