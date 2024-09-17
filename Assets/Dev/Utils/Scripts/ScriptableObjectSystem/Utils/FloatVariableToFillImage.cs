using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FloatVariableToFillImage : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _refFloat;
    [SerializeField]
    private float _maxValue;
    [SerializeField] [Tooltip("unit per sec")]
    private float _autoReduceSpeed;
    [SerializeField]
    private Image _targetImage;

    private CancellationTokenSource _updateToken;

    public async UniTaskVoid AutoReduceFromRefValue()
    {

        float currentValue = _refFloat.Value;

        _updateToken.Cancel();
        _updateToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            if (currentValue <= 0f)
            {
                UpdateImageFill(0f);

                _updateToken.Cancel();
                return;
            }

            currentValue -= _autoReduceSpeed * Time.deltaTime;
            UpdateImageFill(currentValue);
        }
    }

    private void OnEnable()
    {
        _refFloat.OnValueChange += UpdateImageFill;
    }

    private void OnDisable()
    {
        _refFloat.OnValueChange -= UpdateImageFill;
    }

    private void UpdateImageFill(float value)
    {
        _targetImage.fillAmount = value / _maxValue;
    }
}
