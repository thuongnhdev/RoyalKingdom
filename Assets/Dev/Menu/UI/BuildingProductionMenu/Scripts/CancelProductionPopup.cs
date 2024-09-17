using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CancelProductionPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private UserBuildingProductionList _productions;
    [SerializeField]
    private ProductFormulaList _formulas;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjectId;
    [SerializeField]
    private FloatVariable _productionProgressForUI;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _askCancelCurrentProduction;
    [SerializeField]
    private UnityEvent _openSelf;

    [Header("Config")]
    [SerializeField]
    private GameObject _itemIconAndLabelPrefab;
    [SerializeField]
    private RectTransform _refundInfoLayoutGroup;
    [SerializeField]
    private TMP_Text _remainTimeText;

    private List<GameObject> _itemUiObjs = new();

    public void SetUpInfo()
    {
        var production = _productions.GetBuildingProduction(_selectedBuildingObjectId.Value);
        if (production == null)
        {
            return;
        }

        float timeCost = _formulas.GetTimeCost(production.CurrentProductId);
        PerFrame_UpdateRemainTime(timeCost).Forget();

        List<ResourcePoco> refundResources = production.currentMaterials;

        for (int i = 0; i < refundResources.Count; i++)
        {
            var itemUiObj = Instantiate(_itemIconAndLabelPrefab, _refundInfoLayoutGroup);
            itemUiObj.GetComponent<ResourceIconAndLabel>().SetUp(refundResources[i].itemId, $"{refundResources[i].itemCount}/{refundResources[i].itemCount}");
            itemUiObj.SetActive(true);

            _itemUiObjs.Add(itemUiObj);
        }
    }

    public void OpenSelfOrGoToNextStep()
    {
        int product = _productions.GetCurrentProductId(_selectedBuildingObjectId.Value);
        if (product == 0)
        {
            return;
        }
        var cost = _formulas.GetResourceCost(product);
        if (cost.Count == 0)
        {
            _askCancelCurrentProduction.Raise();
            return;
        }

        _openSelf.Invoke();
    }

    public void In_ResetRefundInfo()
    {
        for (int i = _itemUiObjs.Count - 1; 0 <= i; i--)
        {
            Destroy(_itemUiObjs[i]);
            _itemUiObjs.RemoveAt(_itemUiObjs.Count - 1);
        }

        _updateToken?.Cancel();
    }

    private CancellationTokenSource _updateToken;
    private async UniTaskVoid PerFrame_UpdateRemainTime(float timeCost)
    {
        _updateToken?.Cancel();
        _updateToken = new();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            _remainTimeText.text = TimeUtils.FormatTime(timeCost - _productionProgressForUI.Value);
        }
    }
}
