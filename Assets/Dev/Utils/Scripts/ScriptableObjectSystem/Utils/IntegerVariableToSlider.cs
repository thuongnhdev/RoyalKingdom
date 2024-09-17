using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntegerVariableToSlider : MonoBehaviour
{
    [SerializeField]
    private IntegerVariable _integerVar;
    [SerializeField]
    private Slider _slider;

    private void Start()
    {
        _integerVar.OnValueChange += UpdateSliderValue;
        _slider.value = _integerVar.Value;
    }

    private void OnDestroy()
    {
        _integerVar.OnValueChange -= UpdateSliderValue;
    }

    private void UpdateSliderValue(int newValue)
    {
        _slider.value = newValue;
    }
}
