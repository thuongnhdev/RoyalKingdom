using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class WmLandObjectMover : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TableCityViewModel _cities;
    [SerializeField]
    private WorldMapBezierRoadPathList _paths;
    [SerializeField]
    private UserProfile _profile;

    [SerializeField]
    private UnityEvent _onReachedTargetCity;
    [SerializeField]
    private UnityEvent _onStopMoving;

    [Header("Configs")]
    [SerializeField]
    private Transform _movedObject;
    [SerializeField]
    private float _moveSpeed;
    public float MoveSpeed
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }
    [SerializeField]
    private float _rotateSpeed = 720f;
    public float RotateSpeed
    {
        get
        {
            return _rotateSpeed;
        }
        set
        {
            _rotateSpeed = value;
        }
    }

    [Header("Inspec")]
    [SerializeField]
    private int _currentCity;
    public int CurrentCity
    {
        get
        {
            return _currentCity;
        }
        set
        {
            _currentCity = value;
        }
    }

    private Quaternion _targetRotation;

    public void MoveTo(int toCity,Action onReachedTargetCity)
    {
        if (toCity == _currentCity)
        {
            Logger.LogWarning("toCity and fromCity are same, ignore move command");
            return;
        }
        if (Mathf.Abs(_moveSpeed) <= float.Epsilon)
        {
            Logger.LogWarning($"object [{name}] has moveSpeed = 0, ignore move command");
            return;
        }

        List<BezierPathInstruction> instructions = _paths.GetBezierPath(_currentCity, toCity, _profile.id);
        if (instructions == null || instructions.Count == 0)
        {
            return;
        }

        Async_MoveFollowingInstructions(instructions, toCity, onReachedTargetCity).Forget();
    }


    public void MoveToTarget(int toCity, Action onReachedTargetCity,float duration)
    {
        if (toCity == _currentCity)
        {
            Logger.LogWarning("toCity and fromCity are same, ignore move command");
            return;
        }
        if (Mathf.Abs(_moveSpeed) <= float.Epsilon)
        {
            Logger.LogWarning($"object [{name}] has moveSpeed = 0, ignore move command");
            return;
        }

        List<BezierPathInstruction> instructions = _paths.GetBezierPath(_currentCity, toCity, _profile.id);
        if (instructions == null || instructions.Count == 0)
        {
            return;
        }

        Async_MoveFollowingInstructionsTarget(instructions, toCity, onReachedTargetCity,duration).Forget();
    }

    private async UniTaskVoid Async_MoveFollowingInstructionsTarget(List<BezierPathInstruction> instructions, int toCity, Action onReachedTargetCity,float duration)
    {
        for (int i = 0; i < instructions.Count; i++)
        {
            await PerFrame_MoveFollowingInstructionTarget(instructions[i], duration);
        }

        _currentCity = toCity;
        onReachedTargetCity?.Invoke();
        _onReachedTargetCity.Invoke();
    }

    private async UniTask PerFrame_MoveFollowingInstructionTarget(BezierPathInstruction instruction,float duration)
    {
        _movingToken?.Cancel();
        _movingToken = new();

        var pathInstance = WorldMapBezierPathGenerator.Instance.GetPathInstance(instruction.pathId);
        if (pathInstance == null)
        {
            return;
        }
        var path = pathInstance.path;
        float direction = Mathf.Sign(instruction.toDistance - instruction.fromDistance);

        float velocity = _moveSpeed * direction;
        float currentDistance = instruction.fromDistance;
        (Vector3, Quaternion) posAndRot = CalculatePositionAndRotationi(path, currentDistance, direction);
        _movedObject.position = posAndRot.Item1;
        PerFrame_RotateToTarget().Forget();

        await UniTask.NextFrame();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_movingToken.Token))
        {
            float deltaMove = velocity * Time.deltaTime;
            if (Mathf.Abs(instruction.toDistance - currentDistance) <= duration)
            {
                StopMoving();
                break;
            }
            currentDistance += deltaMove;
            posAndRot = CalculatePositionAndRotationi(path, currentDistance, direction);
            _movedObject.position = posAndRot.Item1;
        }
    }

    public void Move(int fromCity, int toCity,Action onReachedTargetCity)
    {
        _currentCity = fromCity;
        MoveTo(toCity, onReachedTargetCity);
    }

    public void MoveTarget(int fromCity, int toCity, Action onReachedTargetCity,float duration)
    {
        _currentCity = fromCity;
        MoveToTarget(toCity, onReachedTargetCity, duration);
    }

    public void StopMoving()
    {
        _movingToken?.Cancel();
        _rotateToken?.Cancel();
        _onStopMoving.Invoke();
    }

    private async UniTaskVoid Async_MoveFollowingInstructions(List<BezierPathInstruction> instructions, int toCity,Action onReachedTargetCity)
    {
        for (int i = 0; i < instructions.Count; i++)
        {
            await PerFrame_MoveFollowingInstruction(instructions[i]);
        }

        _currentCity = toCity;
        onReachedTargetCity?.Invoke();
        _onReachedTargetCity.Invoke();
    }

    private CancellationTokenSource _movingToken;
    private async UniTask PerFrame_MoveFollowingInstruction(BezierPathInstruction instruction)
    {
        _movingToken?.Cancel();
        _movingToken = new();

        var pathInstance = WorldMapBezierPathGenerator.Instance.GetPathInstance(instruction.pathId);
        if (pathInstance == null)
        {
            return;
        }
        var path = pathInstance.path;
        float direction = Mathf.Sign(instruction.toDistance - instruction.fromDistance);

        float velocity = _moveSpeed * direction;
        float currentDistance = instruction.fromDistance;
        (Vector3, Quaternion) posAndRot = CalculatePositionAndRotationi(path, currentDistance, direction);
        _movedObject.position = posAndRot.Item1;
        PerFrame_RotateToTarget().Forget();

        await UniTask.NextFrame();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_movingToken.Token))
        {
            float deltaMove = velocity * Time.deltaTime;
            if (Mathf.Abs(instruction.toDistance - currentDistance) <= Mathf.Abs(deltaMove))
            {
                StopMoving();
                break;
            }
            currentDistance += deltaMove;
            posAndRot = CalculatePositionAndRotationi(path, currentDistance, direction);
            _movedObject.position = posAndRot.Item1;
        }
    }

    private (Vector3, Quaternion) CalculatePositionAndRotationi(VertexPath path, float distance, float direction)
    {
        Vector3 position = path.GetPointAtDistance(distance);
        Quaternion rotation = path.GetRotationAtDistance(distance);

        Vector3 euler = rotation.eulerAngles;
        if (direction < 0f)
        {
            euler.y += 180f;
        }
        euler.z = 0f; // The rotation has z angle = -90 that does not match our model settings.
        rotation = Quaternion.Euler(euler);

        _targetRotation = rotation;
        return (position, rotation);
    }

    private CancellationTokenSource _rotateToken;
    private async UniTaskVoid PerFrame_RotateToTarget()
    {
        _rotateToken?.Cancel();
        _rotateToken = new();
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_rotateToken.Token))
        {
            _movedObject.rotation = Quaternion.RotateTowards(_movedObject.rotation, _targetRotation, _rotateSpeed * Time.deltaTime);
        }
    }
}
