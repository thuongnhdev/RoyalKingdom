using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToDirection : MonoBehaviour
{
    [SerializeField]
    private Vector3Variable _direction;
    [SerializeField]
    private Transform _target;

    private void RotateTo(Vector3 direction)
    {
        _target.rotation = Quaternion.LookRotation(direction);
    }

    private void OnEnable()
    {
        _direction.OnValueChange += RotateTo;
    }

    private void OnDisable()
    {
        _direction.OnValueChange -= RotateTo;
    }
}
