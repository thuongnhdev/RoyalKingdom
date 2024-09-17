using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatVariableToSlider : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _float;
    [SerializeField]
    private Slider _slider;

    private void Start()
    {
        _float.OnValueChange += UpdateSliderValue;
        _slider.value = _float.Value;
    }

    private void OnDestroy()
    {
        _float.OnValueChange -= UpdateSliderValue;
    }

    private void UpdateSliderValue(float newValue)
    {
        _slider.value = newValue;
    }
}
