using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFontWeightSetter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _targetText;
    [SerializeField]
    private FontWeight _fontWeight;

    private void OnEnable()
    {
        if (_targetText == null)
        {
            return;
        }

        _targetText.fontWeight = _fontWeight;
    }

    private void OnValidate()
    {
        _targetText = GetComponent<TMP_Text>();
    }
}
