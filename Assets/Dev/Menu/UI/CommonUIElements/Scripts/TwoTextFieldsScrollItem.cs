using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TwoTextFieldsScrollItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _fieldText;
    [SerializeField]
    private TMP_Text _fieldValueText;

    public void SetUp(string fieldText, string valueText)
    {
        _fieldText.text = fieldText;
        _fieldValueText.text = valueText;
    }

    public void SetUp(string valueText)
    {
        _fieldValueText.text = valueText;
    }
}
