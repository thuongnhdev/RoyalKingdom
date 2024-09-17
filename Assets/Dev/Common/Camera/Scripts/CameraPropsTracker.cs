using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraPropsTracker : MonoBehaviour
{
    [Header("Reference - Read/Write")]
    [SerializeField]
    private FloatVariable _trackedCameraSize;

    [Header("Config")]
    [SerializeField]
    private Camera _trackedCamera;
    [SerializeField]
    private bool _readValue;

    private CancellationTokenSource _updateToken;


    private async UniTaskVoid TrackCamSize()
    {
        if (_updateToken != null)
        {
            _updateToken.Cancel();
        }

        _updateToken = new CancellationTokenSource();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            if (Mathf.Abs(_trackedCamera.orthographicSize - _trackedCameraSize.Value) <= float.Epsilon)
            {
                continue;
            }
            _trackedCameraSize.Value = _trackedCamera.orthographicSize;
        }
    }

    private void UpdateCamSize(float newCamSize)
    {
        _trackedCamera.orthographicSize = newCamSize;
    }

    private void OnEnable()
    {
        if (_readValue)
        {
            UpdateCamSize(_trackedCameraSize.Value);
            _trackedCameraSize.OnValueChange += UpdateCamSize;
            return;
        }

        TrackCamSize().Forget();
    }

    private void OnDisable()
    {
        _trackedCameraSize.OnValueChange -= UpdateCamSize;

        if (_updateToken != null)
        {
            _updateToken.Cancel();
        }
    }
}
