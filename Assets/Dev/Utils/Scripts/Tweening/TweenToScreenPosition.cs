using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenToScreenPosition : MonoBehaviour
{
    [SerializeField]
    private DoTweenAnimation _moveTween;
    [SerializeField]
    private Vector3Variable _targetScreenPosition;

    private Camera _mainCam;
    private Camera MainCam
    {
        get
        {
            if (_mainCam == null)
            {
                _mainCam = Camera.main;
            }

            return _mainCam;
        }
    }
    public void Move()
    {
        Vector3 targetWorldPos = MainCam.ScreenToWorldPoint(_targetScreenPosition.Value);
        _moveTween.To = targetWorldPos;

        _moveTween.DoTween();
    }
}
