using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenMaterialProperty : MonoBehaviour
{
    [SerializeField]
    private Renderer _renderer;
    [SerializeField]
    private Ease _tweenType;
    [SerializeField]
    private float _duration = 0.5f;

    [Header("Inspec")]
    [SerializeField]
    private string _targetProps;

    public void SetTarget(string propName)
    {
        _targetProps = propName;
    }

    public void SetInitPropValue(float init)
    {
        _renderer.material.SetFloat(_targetProps, init);
    }

    public void SetFloat(float value)
    {
        _renderer.material.SetFloat(_targetProps, value);
    }

    public void TweenFloat(float to)
    {
        float current = _renderer.material.GetFloat(_targetProps);
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(mpb);

        DOTween.To(() => current, value => current = value, to, _duration).SetEase(_tweenType).onUpdate = () =>
        {
            mpb.SetFloat(_targetProps, current);
            _renderer.SetPropertyBlock(mpb);
        };
    }
}
