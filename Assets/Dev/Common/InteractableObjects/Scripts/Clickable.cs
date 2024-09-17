using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour, ISceneGameObjectClickHandler
{
    [SerializeField]
    private UnityEvent _onClicked;
    [SerializeField]
    private Vector3Variable _clickPoint;

    public void OnPointerClicked(Vector3 clickPoint)
    {
        if (_clickPoint != null)
        {
            _clickPoint.Value = clickPoint;
        }
        _onClicked.Invoke();
    }
}
