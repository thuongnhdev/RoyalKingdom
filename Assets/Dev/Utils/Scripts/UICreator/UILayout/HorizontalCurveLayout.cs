using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HorizontalCurveLayout : MonoBehaviour
{
    [SerializeField]
    private bool _ignoreInactiveChild = true;

    private HorizontalLayoutGroup _unityHorLayout;
    private bool _isCalculating = false;

    public void Refresh()
    {
        if (!isActiveAndEnabled)
        { 
            return;
        }

        CheckBeforeRefresh();
    }

    private void CheckBeforeRefresh()
    {
        if (_isCalculating)
        {
            return;
        }
        _isCalculating = true;

        CalculateLayout().Forget();
    }

    private async UniTaskVoid CalculateLayout()
    {
        _unityHorLayout.enabled = true;
        await UniTask.NextFrame();
        if (this == null)
        {
            return;
        }

        List<Vector3> horChildPosList = new();
        List<Transform> activeChildren = new();
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);

            if (child.GetComponent<CustomLayoutElement>() == null)
            {
                var element = child.gameObject.AddComponent<CustomLayoutElement>();
                element.SetParent(this);
            }

            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            horChildPosList.Add(child.localPosition);
            activeChildren.Add(child);
        }

        var rectTrans = gameObject.GetComponent<RectTransform>();
        var childRectTrans = transform.GetChild(0).GetComponent<RectTransform>();
        float deltaHeightWithChild = rectTrans.rect.height - childRectTrans.rect.height;

        _unityHorLayout.enabled = false;
        await UniTask.NextFrame();

        SetElementsHeight(horChildPosList, activeChildren, deltaHeightWithChild);

        _isCalculating = false;
    }

    private void SetElementsHeight(List<Vector3> horChildPosList, List<Transform> activeChildren, float deltaHeightWithChild)
    {
        if (horChildPosList.Count == 0)
        {
            return;
        }

        int childCount = transform.childCount;
        int halfActiveChildCount = horChildPosList.Count / 2;
        int childCountForLayoutCalculate = _ignoreInactiveChild ? halfActiveChildCount : childCount / 2;
        float deltaHeightBetween2CloseChilds = deltaHeightWithChild / childCountForLayoutCalculate;

        bool isEvenCount = activeChildren.Count % 2 == 0;
        for (int i = 0; i < activeChildren.Count; i++)
        {
            int childPosIdx = Mathf.Abs(halfActiveChildCount - i);
            if (halfActiveChildCount <= i && isEvenCount)
            {
                childPosIdx++;
            }

            var child = activeChildren[i];
            child.localPosition += childPosIdx * deltaHeightBetween2CloseChilds * Vector3.up;
        }
    }

    private void Awake()
    {
        _unityHorLayout = GetComponent<HorizontalLayoutGroup>();
    }

    private void OnEnable()
    {
        Refresh();
    }
}
