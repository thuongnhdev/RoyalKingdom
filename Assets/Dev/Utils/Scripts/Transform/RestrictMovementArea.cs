using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RestrictMovementArea : MonoBehaviour
{
    [SerializeField]
    private Vector3 _restrictArea;
    [SerializeField]
    private Transform _restrictAreaCenter;

    private CancellationTokenSource _moveToken;
    private float _xMax;
    private float _xMin;
    private float _yMax;
    private float _yMin;
    private float _zMax;
    private float _zMin;

    private async UniTaskVoid StartRestrict()
    {
        _moveToken?.Cancel();
        _moveToken = new();

        Vector3 inversePos;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveToken.Token))
        {
            inversePos = _restrictAreaCenter.InverseTransformPoint(transform.position);

            if (IsInRestrictArea(inversePos))
            {
                continue;
            }
            inversePos.x = Mathf.Clamp(inversePos.x, _xMin, _xMax);
            inversePos.y = Mathf.Clamp(inversePos.y, _yMin, _yMax);
            inversePos.z = Mathf.Clamp(inversePos.z, _zMin, _zMax);

            transform.position = _restrictAreaCenter.TransformPoint(inversePos);
        }
    }

    private bool IsInRestrictArea(Vector3 checkPos)
    {
        return _xMin <= checkPos.x && checkPos.x <= _xMax
            && _yMin <= checkPos.y && checkPos.y <= _yMax
            && _zMin <= checkPos.z && checkPos.z <= _zMax;
    }

    private void CalculateBoundary()
    {
        _xMax = _restrictArea.x / 2f;
        _xMin = - _restrictArea.x / 2f;
        _yMax = _restrictArea.y / 2f;
        _yMax = -_restrictArea.y / 2f;
        _zMax = _restrictArea.z / 2f;
        _zMin = -_restrictArea.z / 2f;
    }

    private void OnEnable()
    {
        CalculateBoundary();
        StartRestrict().Forget();
    }

    private void OnDisable()
    {
        _moveToken?.Cancel();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_restrictAreaCenter == null)
        {
            return;
        }

        Gizmos.DrawWireCube(_restrictAreaCenter.position, _restrictArea);
    }
}
