using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class IntegerVariableToSprite : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _textMesh;
    [SerializeField]
    private IntegerVariable _intVariable;
    [SerializeField]
    private bool _continueUpdate = true;

    private void OnEnable()
    {
        UpdateValue(_intVariable.Value);

        if (_continueUpdate)
        {
            _intVariable.OnValueChange += UpdateValue;
        }
    }

    private void OnDisable()
    {
        if (_continueUpdate)
        {
            _intVariable.OnValueChange -= UpdateValue;
        }
    }

    private void UpdateValue(int newValue)
    {
        var sb = new StringBuilder();

        int length = newValue.ToString().Length;
        int digit;
        Stack<string> buffer = new Stack<string>();

        for (int i = 0; i < length; i++)
        {
            digit = newValue % 10;
            newValue /= 10;
            buffer.Push($"<sprite={digit}>");
        }

        while (buffer.Count > 0)
        {
            sb.Append(buffer.Pop());   
        }

        _textMesh.text = sb.ToString();
    }
}
