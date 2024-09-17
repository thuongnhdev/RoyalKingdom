using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLayoutElement : MonoBehaviour
{
    private HorizontalCurveLayout _parent;

    public void SetParent(HorizontalCurveLayout parent)
    {
        _parent = parent;
    }

    private void OnEnable()
    {
        if (_parent != null)
        {
            _parent.Refresh();
        }
    }

    private void OnDisable()
    {
        if (_parent != null)
        {
            _parent.Refresh();
        }
    }
}
