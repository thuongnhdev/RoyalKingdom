using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3VariableToPosition : MonoBehaviour
{
    [SerializeField]
    private Vector3Variable _targetVariable;

    public void SetTarget(Vector3Variable target)
    {
        if (_targetVariable != null)
        {
            _targetVariable.OnValueChange -= UpdatePosition;
        }

        _targetVariable = target;
        _targetVariable.OnValueChange += UpdatePosition;
    }

    private void OnEnable()
    {
        transform.position = _targetVariable.Value;
        _targetVariable.OnValueChange += UpdatePosition;
    }

    private void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private void OnDisable()
    {
        _targetVariable.OnValueChange -= UpdatePosition;
    }

#if UNITY_EDITOR
    private void Update()
    {
        UpdatePosition(_targetVariable.Value);
    }
#endif
}
