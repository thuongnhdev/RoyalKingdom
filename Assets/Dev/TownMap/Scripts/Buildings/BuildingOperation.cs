using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildingOperation : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingList;
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;

    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onBuildingObjSelected;
    [SerializeField]
    private GameEvent _onFinishConstruction;
    [SerializeField]
    private GameEvent _onFinishUpgrade;
    [SerializeField]
    private GameEvent _onFinishDestruction;
    [SerializeField]
    private GameEvent _onRequestSwitchBuildingStatus;
    [SerializeField]
    private GameEvent _onAskedRescanMap;

    [SerializeField]
    private UnityEvent _onSwitchStatus;

    [Header("Config")]
    [SerializeField]
    private Transform _doorPos;
    [SerializeField]
    private Collider _buildingPlatform;
    [SerializeField]
    private TilingTransform _tilingTransform;
    [SerializeField]
    private BuildingObjectUI _buildingUI;
    [SerializeField]
    private BuildingPolisher _polisher;
    [SerializeField]
    private GameObject _buildingModel;
    [SerializeField]
    private Animator _buildingAnimator;
    private Animator BuildingAnimator
    {
        get
        {
            if (_buildingAnimator == null)
            {
                var child = _buildingModel.transform.GetChild(0);
                if (!child.gameObject.activeSelf)
                {
                    child = _buildingModel.transform.GetChild(1);
                }

                _buildingAnimator = child.GetComponent<Animator>();
            }

            return _buildingAnimator;
        }
    }

    [Header("Inspec")]
    [SerializeField]
    private int _buildingObjId;
    public int BuildingObjId => _buildingObjId;
    [SerializeField]
    private int _buildingId;
    public int BuildingId => _buildingId;
    [SerializeField]
    private int _level;
    [SerializeField]
    private BuildingStatus _status;
    public BuildingStatus Status => _status;
    [SerializeField]
    private float _constructionProgress;
    [SerializeField]
    private float _destructionProgress;
    [SerializeField]
    private float _currentConstructionRate;
    [SerializeField]
    private float _currentDestructionRate;
    [SerializeField]
    private List<ResourcePoco> _constructionMat;
    [SerializeField]
    private List<ResourcePoco> _productionMat;

    private DateTime _timeLostFocus;
    private UserBuilding _referenceBuilding; // DO NOT modify anything except current constructedTime and destructedTime

    // Inspector call
    public void SelectBuilding()
    {
        if (_selectedBuildingObjId.Value == _buildingObjId)
        {
            return;
        }

        SelectBuildingWithoutNotify();
        _onBuildingObjSelected.Raise();
    }

    // Inspector call (vassal icon)
    public void SelectBuildingWithoutNotify()
    {
        _selectedBuildingObjId.Value = _buildingObjId;
    }

    public void UpdateInfo(UserBuilding userBuildingInfo)
    {
        _referenceBuilding = userBuildingInfo;

        _buildingObjId = userBuildingInfo.buildingObjectId;
        _buildingId = userBuildingInfo.buildingId;
        SetUpPlatform(_buildingId);

        _level = userBuildingInfo.buildingLevel;
        SwitchStatus(userBuildingInfo.status);
        DetermineWalkThrough();

        _constructionProgress = userBuildingInfo.constructedTime;
        _destructionProgress = userBuildingInfo.destructedTime;

        _currentConstructionRate = userBuildingInfo.currentContructionRate;
        _currentDestructionRate = userBuildingInfo.currentDestructionRate;

        _constructionMat = userBuildingInfo.constructionMaterial;

        UpdateBuildingOperation();
        _buildingUI.UpdateVassalIcon(userBuildingInfo.vassalInCharge, userBuildingInfo.status);

#if UNITY_EDITOR
        EditorOnly_DisplayBuildlingName();
#endif
    }

    public Vector3 GetDoorPosition()
    {
        if (_doorPos == null)
        {
            return transform.position;
        }

        var mr = _doorPos.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = _doorPos.GetComponentInChildren<MeshRenderer>();
        }

        return _doorPos != null ? mr.bounds.center : transform.position;
    }

    public void OpenDoor(Vector3 doorPos)
    {
        List<Vector2> doorsDirection = _buildingList.GetBuildingDoorsDirection(_buildingId);
        int doorIndex = BuildingHelper.FromDoorPosToDoorIndex(doorPos, _tilingTransform.GetLocalPosition(), _tilingTransform.GetRotation(), doorsDirection);

        OpenDoor(doorIndex);
    }
    public void OpenDoor(int doorIndex)
    {
        if (doorIndex == -1)
        {
            return;
        }

        if (BuildingAnimator == null)
        {
            Logger.LogWarning("Animator is not assigned to Building!");
            return;
        }

        BuildingAnimator.Play($"OpenDoor {doorIndex}");
    }

    public void CloseDoor(Vector3 doorPos)
    {
        List<Vector2> doorsDirection = _buildingList.GetBuildingDoorsDirection(_buildingId);
        int doorIndex = BuildingHelper.FromDoorPosToDoorIndex(doorPos, _tilingTransform.GetLocalPosition(), _tilingTransform.GetRotation(), doorsDirection);

        CloseDoor(doorIndex);
    }
    public void CloseDoor(int doorIndex)
    {
        if (doorIndex == -1)
        {
            return;
        }

        if (BuildingAnimator == null)
        {
            Logger.LogWarning("Animator is not assigned to Building!");
            return;
        }

        BuildingAnimator.Play($"CloseDoor {doorIndex}");
    }

    public List<Vector3> GetDoorPositions()
    {
        List<Vector2> doorDirections = _buildingList.GetBuildingDoorsDirection(_buildingId);
        List<Vector3> doorsPosition = new List<Vector3>();

        if (doorDirections == null || doorDirections.Count == 0)
        {
            return doorsPosition;
        }

        for (int i = 0; i < doorDirections.Count; i++)
        {
            doorsPosition.Add(BuildingHelper.GetDoorPosition(doorDirections[i], _tilingTransform.GetLocalPosition(), _tilingTransform.GetRotation()));
        }

        return doorsPosition;
    }

    private void SwitchStatus(BuildingStatus status)
    {
        if (status == _status)
        {
            return;
        }

        StopAllProgresses();
        _buildingUI.SetActive(false);

        _status = status;
        _onSwitchStatus.Invoke();
    }

    private void Construct()
    {
        var buildingInfo = _buildingList.GetBaseBuilding(_buildingId);
        if (buildingInfo == null)
        {
            return;
        }

        _status = BuildingStatus.OnConstruction;

        PerFrame_Construct(_buildingUpgradeInfos.GetTimeCost(_buildingId, 1)).Forget();
    }

    private CancellationTokenSource _constructionToken;
    private async UniTaskVoid PerFrame_Construct(float timeCost)
    {
        _constructionToken?.Cancel();
        _constructionToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_constructionToken.Token))
        {
            _referenceBuilding.constructedTime = _constructionProgress;
            if (_constructionProgress >= timeCost)
            {
                _buildingUI.UpdateProgress(timeCost, _constructionProgress, _currentConstructionRate);

                _constructionProgress = 0f;

                _onFinishConstruction.Raise(_buildingObjId);
                _constructionToken.Cancel();
                break;
            }

            float progressPerFrame = _currentConstructionRate * Time.deltaTime;
            _constructionProgress += progressPerFrame;

            _buildingUI.UpdateProgress(timeCost, _constructionProgress, _currentConstructionRate);
        }
    }

    private void Upgrade()
    {
        var buildingInfo = _buildingList.GetBaseBuilding(_buildingId);
        if (buildingInfo == null)
        {
            return;
        }

        _status = BuildingStatus.Upgrading;

        PerFrame_Upgrade(_buildingUpgradeInfos.GetTimeCost(_buildingId, _level + 1)).Forget();
    }

    private CancellationTokenSource _upgradeToken;
    private async UniTaskVoid PerFrame_Upgrade(float timeCost)
    {
        _upgradeToken?.Cancel();
        _upgradeToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_upgradeToken.Token))
        {
            _referenceBuilding.constructedTime = _constructionProgress;
            if (_constructionProgress >= timeCost)
            {
                _constructionProgress = 0f;

                _buildingUI.UpdateProgress(timeCost, _constructionProgress, _currentConstructionRate);

                _onFinishUpgrade.Raise(_buildingObjId);
                _upgradeToken.Cancel();
                break;
            }

            float progressPerFrame = _currentConstructionRate * Time.deltaTime;
            _constructionProgress += progressPerFrame;

            _buildingUI.UpdateProgress(timeCost, _constructionProgress, _currentConstructionRate);
        }
    }

    private void DestroyBuilding()
    {
        var buildingInfo = _buildingList.GetBaseBuilding(_buildingId);
        if (buildingInfo == null)
        {
            return;
        }

        _status = BuildingStatus.OnDestruction;

        PerFrame_Destroy(_buildingUpgradeInfos.GetTimeCost(_buildingId, _level) * buildingInfo.destructionTimeFactor).Forget();
    }

    private CancellationTokenSource _destroyToken;
    private async UniTaskVoid PerFrame_Destroy(float timeCost)
    {
        StopAllProgresses();

        _destroyToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_destroyToken.Token))
        {
            _referenceBuilding.destructedTime = _destructionProgress;
            if (_destructionProgress >= timeCost)
            {
                _buildingUI.UpdateProgress(timeCost, _destructionProgress, _currentDestructionRate, true);
                _destructionProgress = 0f;

                _onFinishDestruction.Raise(_buildingObjId);
                _destroyToken.Cancel();
                break;
            }

            float progressPerFrame = _currentDestructionRate * Time.deltaTime;
            _destructionProgress += progressPerFrame;

            _buildingUI.UpdateProgress(timeCost, _destructionProgress, _currentDestructionRate, true);
        }
    }

    private void StopAllProgresses()
    {
        _constructionToken?.Cancel();
        _upgradeToken?.Cancel();
        _destroyToken?.Cancel();
    }

    private void UpdateBuildingOperation()
    {
        float buildingAppearPercent = 1f;
        switch (_status)
        {
            case BuildingStatus.WaitForConstructingResource:
                {
                    List<ResourcePoco> constructionCost = _buildingUpgradeInfos.GetUpgradeCost(_buildingId, 1);
                    _buildingUI.UpdateRequireResource(_constructionMat, constructionCost);
                    _polisher.ShowConstructingBuildingBasedOnResourceFilled(_constructionMat, constructionCost);

                    return;
                }
            case BuildingStatus.Idle:
                {
                    break;
                }
            case BuildingStatus.OnConstruction:
                {
                    Construct();
                    break;
                }
            case BuildingStatus.WaitingForUpgradeResource:
                {
                    List<ResourcePoco> constructionCost = _buildingUpgradeInfos.GetUpgradeCost(_buildingId, _level + 1);
                    _buildingUI.UpdateRequireResource(_constructionMat, constructionCost);
                    break;
                }
            case BuildingStatus.Upgrading:
                {
                    Upgrade();
                    break;
                }
            case BuildingStatus.OnDestruction:
                {
                    DestroyBuilding();
                    break;
                }
            default:
                {
                    break;
                }
        }

        _polisher.SetBuildingAppearance(buildingAppearPercent);
    }

    private void AddProgressWhenFocus()
    {
        float timePass = TimeUtils.HowManySecFrom(_timeLostFocus);

        if (_status == BuildingStatus.OnConstruction)
        {
            _constructionProgress += timePass * _currentConstructionRate;
            return;
        }

        _destructionProgress += timePass * _currentDestructionRate;
    }

    private void SetUpPlatform(int buildingId)
    {
        Vector2 buildingSize = _buildingList.GetBuildingSize(buildingId);
        Vector3 platformSize = Vector3.one;
        platformSize.y = _buildingPlatform.transform.localScale.y;
        platformSize.x = buildingSize.x - 0.5f;
        platformSize.z = buildingSize.y - 0.5f;

        _buildingPlatform.transform.localScale = platformSize;
    }

    private void SetUpModelSize(int buildingId)
    {
        Vector2 buildingSize = _buildingList.GetBuildingSize(buildingId);
        Vector3 modelSize = Vector3.one;
        modelSize.x = buildingSize.x - 0.5f;
        modelSize.y = buildingSize.x;
        modelSize.z = buildingSize.y - 0.5f;

        _buildingModel.transform.localScale = modelSize;
    }

    private void DetermineWalkThrough()
    {
        var platformObj = _buildingPlatform.gameObject;
        List<Vector2> doors = _buildingList.GetBuildingDoorsDirection(_buildingId);
        if (doors == null || doors.Count == 0)
        {
            platformObj.layer = LayerMask.NameToLayer("Walkable");
            return;
        }

        if (_status == BuildingStatus.WaitForConstructingResource)
        {
            platformObj.layer = LayerMask.NameToLayer("Walkable");
            return;
        }

        platformObj.layer = LayerMask.NameToLayer("Unwalkable");
        _onAskedRescanMap.Raise(_referenceBuilding.locationTileId, _buildingList.GetBuildingSize(_buildingId));
    }

    private void OnDisable()
    {
        StopAllProgresses();
    }

    private void OnApplicationFocus(bool focus)
    {
#if UNITY_EDITOR
        return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
        if (!focus)
#pragma warning restore CS0162 // Unreachable code detected
        {
            _timeLostFocus = DateTime.Now;
            return;
        }

        AddProgressWhenFocus();
    }

#if UNITY_EDITOR
    public void EditorOnly_RefreshOperation()
    {
        var userbuildings = UnityEditor.AssetDatabase.LoadAssetAtPath<UserBuildingList>("Assets\\Dev\\UserData\\UserBuildings\\SOs\\UserBuildingList.asset");
        var userbuilding = userbuildings.GetBuilding(_buildingObjId);
        if (userbuilding == null)
        {
            return;
        }

        UpdateInfo(userbuilding);
    }

    public void EditorOnly_DisplayBuildlingName()
    {
        TMP_Text buildingName = transform.GetChild(0).GetComponent<TMP_Text>();
        buildingName.text = _buildingList.GetBuildingName(_buildingId);
    }
#endif
}
