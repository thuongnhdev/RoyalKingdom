using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScrollToFloat : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _affectedFloat;
    [SerializeField]
    private Vector2 _range;
    [SerializeField]
    private float _scale = 0.1f;

    [SerializeField]
    private void Update()
    {
        float scrollValue = Input.mouseScrollDelta.y * _scale;
        if (_affectedFloat.Value - scrollValue < _range.x)
        {
            _affectedFloat.Value = _range.x;
            return;
        }

        if (_affectedFloat.Value - scrollValue > _range.y)
        {
            _affectedFloat.Value = _range.y;
            return;
        }

        _affectedFloat.Value -= scrollValue;
    }
}
