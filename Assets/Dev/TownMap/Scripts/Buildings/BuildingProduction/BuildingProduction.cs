using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class BuildingProduction : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private ProductFormulaList _productFormulas;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Reference - Write")]
    [SerializeField]
    private FloatVariable _currentProgressUiValue;
    [SerializeField]
    private UserItemStorage _userItemStorage;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onAProductionCompleted;

    [Header("Config")]
    [SerializeField]
    private BuildingComponentGetter _compGetter;
    [SerializeField]
    private BuildingObjectUI _buildingUi;

    [Header("Inspec")]
    [SerializeField]
    private int _currentProductId;
    [SerializeField]
    private List<ResourcePoco> _currentMaterials;
    [SerializeField]
    private float _currentProgress;
    [SerializeField]
    private bool _isIdle;
    [SerializeField]
    private float _productRate;

    private UserBuildingProduction _referenceProduction; // DO NOT modify anything except current progress when producing
    private float _productControlFactor = 1f;
    private bool _isShowingProgressOnUi = false;

    public void UpdateProductionInfo(UserBuildingProduction buildingProduction)
    {
        _referenceProduction = buildingProduction;

        _currentProductId = buildingProduction.CurrentProductId;
        _productRate = buildingProduction.productRate;
        _currentProgress = buildingProduction.currentProgress;

        SetCurrentMaterials(buildingProduction.currentMaterials);
        CheckBuildingStatusAndStartProduction();
    }

    public void SuspendProducing()
    {
        _productControlFactor = 0f;
    }

    public void ResumeProducing()
    {
        _productControlFactor = 1f;
    }

    // Inspector Call
    public void CheckBuildingStatusAndStartProduction()
    {
        if (_compGetter.Operation.Status != BuildingStatus.Producing)
        {
            StopPerFrameProduce();
            return;
        }

        if (_currentProductId == 0)
        {
            return;
        }

        ProduceAnItem(_currentProductId).Forget();
    }

    private CancellationTokenSource _produceItemToken;
    private void StopPerFrameProduce()
    {
        _produceItemToken?.Cancel();
    }

    private async UniTaskVoid ProduceAnItem(int productId)
    {
        float timeCost = _productFormulas.GetTimeCost(productId);

        StopPerFrameProduce();
        _produceItemToken = new();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_produceItemToken.Token))
        {
            _currentProgress += _productRate * _productControlFactor * Time.deltaTime;
            _referenceProduction.currentProgress = _currentProgress;
            _buildingUi.UpdateProgress(timeCost, _currentProgress, _productRate);

            if (_isShowingProgressOnUi)
            {
                _currentProgressUiValue.Value = _currentProgress;
            }

            if (timeCost <= _currentProgress)
            {
                _currentProgress = timeCost;
                _onAProductionCompleted.Raise(_compGetter.Operation.BuildingObjId, productId);
                break;
            }
        }
    }

    private void SetCurrentMaterials(List<ResourcePoco> materials)
    {
        _currentMaterials = materials;

        if (_compGetter.Operation.Status != BuildingStatus.WaitingForProductResource)
        {
            return;
        }

        List<ResourcePoco> cost = _productFormulas.GetResourceCost(_currentProductId);
        UpdateUiRequireResource(cost);
    }

    private void UpdateUiRequireResource(List<ResourcePoco> cost)
    {
        _buildingUi.UpdateRequireResource(_currentMaterials, cost);
    }

    private void SetShowProgressOnUi(int buildingObjId)
    {
        if (buildingObjId != _compGetter.Operation.BuildingObjId)
        {
            _isShowingProgressOnUi = false;
            return;
        }

        _isShowingProgressOnUi = true;
    }

    private void OnEnable()
    {
        _selectedBuildingObjId.OnValueChange += SetShowProgressOnUi;
    }

    private void OnDisable()
    {
        StopPerFrameProduce();
        _selectedBuildingObjId.OnValueChange -= SetShowProgressOnUi;
    }

    private DateTime _timeLostFocus;
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

        _currentProgress += TimeUtils.HowManySecFrom(_timeLostFocus) * _productControlFactor * _productRate;
    }
}
