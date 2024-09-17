using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwapToggle : Toggle
{
    [SerializeField]
    private Sprite _enableSprite;
    [SerializeField]
    private Sprite _disableSprite;
    [SerializeField]
    private Image _targetImage;

    protected override void OnEnable()
    {
        base.OnEnable();
        SwapSprite(isOn);
        onValueChanged.AddListener(SwapSprite);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onValueChanged.RemoveListener(SwapSprite);
    }

    public void SwapSprite(bool value)
    {
        _targetImage.sprite = value ? _enableSprite : _disableSprite;
    }
}
