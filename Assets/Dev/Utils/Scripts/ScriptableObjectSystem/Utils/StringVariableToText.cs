using UnityEngine;
using TMPro;

public class StringVariableToText : MonoBehaviour
{
    [SerializeField]
    private StringVariable _stringVariable;
    [SerializeField]
    private TMP_Text _textMesh;
    [SerializeField]
    private bool _continueUpdate = true;

    public void SetText(string text)
    {
        _textMesh.text = text;
    }

    public void SetText(StringVariable text)
    {
        _textMesh.text = text.Value;
    }

    private void OnEnable()
    {
        _textMesh.text = _stringVariable.Value;
        _stringVariable.OnValueChange += UpdateStringValue;
    }

    private void OnDisable()
    {
        _stringVariable.OnValueChange -= UpdateStringValue;
    }

    private void UpdateStringValue(string newValue)
    {
        if (!_continueUpdate)
        {
            return;
        }

        if (newValue.Equals(_textMesh.text))
        {
            return;
        }

        _textMesh.text = newValue;
    }
}
