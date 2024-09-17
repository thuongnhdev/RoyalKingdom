using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WorldMainMenu : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _camToLandDirection;
    [SerializeField]
    private FloatVariable _camToLandDistance;
    [SerializeField]
    private BoolVariable _myLandVisible;
    [SerializeField]
    private UserProfile _userProfile;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _requestFlyToMyLand;

    [Header("Configs")]
    [SerializeField]
    private TMP_Text _myLandText;
    [SerializeField]
    private Transform _compassTrans;
    [SerializeField]
    private Image _compassArrow;

    public void RequestFlyToMyLand()
    {
        if (_myLandVisible.Value)
        {
            return;
        }

        _requestFlyToMyLand.Raise();
    }

    private void UpdateCompass(Vector3 direction)
    {
        float compassAngle = Vector2.SignedAngle(Vector2.up, direction);
        _compassTrans.rotation = Quaternion.Euler(0f, 0f, compassAngle);

        if (_myLandVisible.Value)
        {
            _myLandText.text = "My Land";
            return;
        }
        _myLandText.text = $"{_camToLandDistance.Value:#.#}Km";
    }

    private void UpdateArrowVisible(bool myLandVisible)
    {
        float fade = !myLandVisible ? 1f : 0f;
        _compassArrow.DOFade(fade, 0.3f).SetEase(Ease.OutQuad);
    }

    private void OnEnable()
    {
        UpdateCompass(_camToLandDirection.Value);
        UpdateArrowVisible(_myLandVisible.Value);
        _camToLandDirection.OnValueChange += UpdateCompass;
        _myLandVisible.OnValueChange += UpdateArrowVisible;
    }

    private void OnDisable()
    {
        _camToLandDirection.OnValueChange -= UpdateCompass;
        _myLandVisible.OnValueChange -= UpdateArrowVisible;
    }
}
