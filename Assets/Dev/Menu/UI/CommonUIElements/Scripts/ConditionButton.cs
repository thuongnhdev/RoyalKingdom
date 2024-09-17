using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConditionButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _conditionText;
    [SerializeField]
    private Button _button;

    public void SetUp(string condition, UnityAction buttonAction = null)
    {
        _conditionText.text = condition;
        
        if (buttonAction == null)
        {
            return;
        }
        _button.onClick.AddListener(buttonAction);
    }
}
