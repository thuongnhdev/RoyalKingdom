using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTimeScale : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _gameFieldDeltaTime;
    [SerializeField]
    private FloatVariable _gameFieldTimeScale;

    private void Update()
    {
        _gameFieldDeltaTime.Value = Time.deltaTime * _gameFieldTimeScale.Value;
    }
}
