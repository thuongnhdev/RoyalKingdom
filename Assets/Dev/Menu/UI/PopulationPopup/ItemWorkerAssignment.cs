using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataCore;
using DG.Tweening;
using TMPro;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;

public class ItemWorkerAssignment : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _tmpIndex;
    [SerializeField] public TextMeshProUGUI _tmpPercent;

    [SerializeField] public Scrollbar _scrollbar;
    [SerializeField] public Image _imgFill;

    private int _index;
    public int _priority;
    private float _percentAfter;
    private float _percentbefore;
    private Action _onComplete;

    [SerializeField]
    private Scrollbar _workerScrollBar;

    [SerializeField]
    private GameEvent _onConfirmApplyWorker;

    private UIPopulation _uiPopulation;
    public void InitData(UIPopulation uiPopulation,int index,int man, WorkerAssignment workerAssignment, Action onComplete)
    {
        _index = index;
        _priority = workerAssignment.Priority;
        _onComplete = onComplete;
        _uiPopulation = uiPopulation;
        _tmpIndex.text = _priority.ToString();
        _tmpPercent.text = string.Format("{0} %", workerAssignment.Percent.ToString());
        _scrollbar.value = workerAssignment.Percent / 100;
        if(_imgFill != null) _imgFill.fillAmount = _scrollbar.value;
        UpdateStatus();
        _percentbefore = workerAssignment.Percent;
        _percentAfter = workerAssignment.Percent;
        _workerScrollBar.value = _scrollbar.value;
    }

    private void OnEnable()
    {
        _onConfirmApplyWorker.Subcribe(OnConfirmApplyWorker);
        _workerScrollBar.onValueChanged.AddListener((float val) => ScrollbarWorkerCallback(val));
    }

    private void OnDisable()
    {
        _onConfirmApplyWorker.Unsubcribe(OnConfirmApplyWorker);
        _workerScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarWorkerCallback(val));
    }

    void ScrollbarWorkerCallback(float value)
    {
        float percent = (float)Math.Round(value * 100);
        if (percent == _uiPopulation.itemDefaulPercent[_index].WorkerAssignment.Percent)
            return;
        _imgFill.fillAmount = value;
        _scrollbar.value = value;
        _tmpPercent.text = string.Format("{0} %", percent.ToString());
        _percentAfter = percent;
        UIManagerTown.Instance.UIPopulation.PercentLastScroll = _percentAfter;
        UIManagerTown.Instance.UIPopulation.PriorityLastScroll = _priority;
        _uiPopulation.CalculatorPercent(_index, _priority, percent);
    }

    public void OnBeginScroll()
    {
        _uiPopulation.OnBeginScroll(_index, _priority);
    }

    public void OnEndScroll()
    {
        _uiPopulation.OnEndScroll(_index, _priority);
    }

    public void UpdateUiScroll(float value)
    {
        var Percent = (float)value / 100;
        if (_imgFill != null) _imgFill.fillAmount = Percent;
        if (_scrollbar != null) _scrollbar.value = Percent;
        var valueNew = (int)value;
        if (_tmpPercent != null) _tmpPercent.text = string.Format("{0} %", valueNew.ToString());
        _percentAfter = value;
    }

    public void OnUpdateScrollWorkerAssignment(WorkerAssignment workerAssignment)
    {
        if (_priority == workerAssignment.Priority) UpdateUiScroll(workerAssignment.Percent);
    }

    private void OnConfirmApplyWorker(object[] eventParam)
    {
        if (UIManagerTown.Instance.UIPopulation.PriorityLastScroll != _priority &&
            UIManagerTown.Instance.UIPopulation.PercentLastScroll == _percentAfter)
        {
            return;
        }
        var index = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.FindIndex(t => t.Priority == _priority);
        GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[index].Percent = _percentAfter;
        GameData.Instance.RequestSaveGame();

    }

    private IEnumerator FadeOutText(float timeSpeed, Image image)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        while (image.color.a > 0.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }
    private void UpdateStatus()
    {
       
    }


  
    public void OnClickIncease()
    {
        OnBeginScroll();
        float value = _workerScrollBar.value;
        if (value >= 1) return;
        value += 0.01f;
        _workerScrollBar.value = value;
        //ScrollbarWorkerCallback(value);
    }

    public void OnClickDecrease()
    {
        OnBeginScroll();
        float value = _workerScrollBar.value;
        if (value <= 0) return;
        value -= 0.01f;
        _workerScrollBar.value = value;
        //ScrollbarWorkerCallback(value);
    }
   
}
