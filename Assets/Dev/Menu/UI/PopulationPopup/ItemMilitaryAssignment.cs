using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataCore;
using DG.Tweening;
using TMPro;
using CoreData.UniFlow.Common;
using CoreData.UniFlow;

public class ItemMilitaryAssignment : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _tmpPercent;

    [SerializeField] public Scrollbar _scrollbar;
    [SerializeField] public Image _imgFill;
    [SerializeField] public Image _imgIcon;
    [SerializeField] public Sprite[] _spriteIcon;

    private int _index;
    public int _priority;
    private float _percentAfter;
    private float _percentbefore;
    private Action _onComplete;

    [SerializeField]
    private Scrollbar _militaryScrollBar;

    [SerializeField]
    private GameEvent _onConfirmApplyMilitary;

    private UIPopulation _uiPopulation;
    public void InitData(UIPopulation uiPopulation, int index, int man, MilitaryAssignment militaryAssignment, Action onComplete)
    {
        _index = index;
        _imgIcon.sprite = _spriteIcon[_index];
        _priority = militaryAssignment.Priority;
        _onComplete = onComplete;
        _uiPopulation = uiPopulation;
        _tmpPercent.text = string.Format("{0} %", militaryAssignment.Percent.ToString());
        _scrollbar.value = militaryAssignment.Percent / 100;
        if (_imgFill != null) _imgFill.fillAmount = _scrollbar.value;
        UpdateStatus();
        UpdateUI();
        _percentbefore = militaryAssignment.Percent;
        _percentAfter = militaryAssignment.Percent;
        _militaryScrollBar.value = _scrollbar.value;
    }

    private void OnEnable()
    {
        _onConfirmApplyMilitary.Subcribe(OnConfirmApplyMilitary);
        _militaryScrollBar.onValueChanged.AddListener((float val) => ScrollbarWorkerCallback(val));
    }

    private void OnDisable()
    {
        _onConfirmApplyMilitary.Unsubcribe(OnConfirmApplyMilitary);
        _militaryScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarWorkerCallback(val));
    }

    void ScrollbarWorkerCallback(float value)
    {
        if (!UIManagerTown.Instance.UIPopulation._parentItemMilitaryAssign.gameObject.activeInHierarchy)
            return;
        float percent = (float)Math.Round(value * 100);
        if (percent == _uiPopulation.itemDefaulPercent[_index].WorkerAssignment.Percent)
            return;
        percent = (float)Math.Round(0.25 * 100);
        _imgFill.fillAmount = 0.25f;
        _scrollbar.value = 0.25f;
        _tmpPercent.text = string.Format("{0} %", percent.ToString());
        UIManagerTown.Instance.ShowUINotification();
        return;
        //if (!UIManagerTown.Instance.UIPopulation._parentItemMilitaryAssign.gameObject.activeInHierarchy)
        //    return;
        //float percent = (float)Math.Round(value * 100);
        //if (percent == _uiPopulation.itemDefaulPercent[_index].WorkerAssignment.Percent)
        //    return;
        //_imgFill.fillAmount = value;
        //_scrollbar.value = value;
        //_tmpPercent.text = string.Format("{0} %", percent.ToString());
        //_percentAfter = percent;
        //UIManagerTown.Instance.UIPopulation.PercentLastScroll = _percentAfter;
        //UIManagerTown.Instance.UIPopulation.PriorityLastScroll = _priority;
        //_uiPopulation.CalculatorPercent(_index, _priority, percent);
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

    private void OnConfirmApplyMilitary(object[] eventParam)
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

    private void UpdateUI()
    {

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
        return;
        OnBeginScroll();
        float value = _militaryScrollBar.value;
        if (value >= 1) return;
        value += 0.01f;
        _militaryScrollBar.value = value;
    }

    public void OnClickDecrease()
    {
        return;
        OnBeginScroll();
        float value = _militaryScrollBar.value;
        if (value <= 0) return;
        value -= 0.01f;
        _militaryScrollBar.value = value;
    }
}
