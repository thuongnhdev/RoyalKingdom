using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScrollController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField]
    protected float _cellViewSize = 100f;
    [SerializeField]
    protected BaseCustomCellView _cellViewPrefab;
    [SerializeField]
    private EnhancedScroller _scroller;

    protected List<object> _dataList = new List<object>();

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = scroller.GetCellView(_cellViewPrefab) as BaseCustomCellView;
        cellView.SetUp(_dataList[dataIndex]);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return _cellViewSize;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _dataList.Count;
    }

    public virtual void AdjustItemPerRow(int itemPerRow)
    {

    }

    protected void ReloadScrollView()
    {
        _scroller.ReloadData();
    }

    protected virtual void DoOnStart()
    {

    }

    private void Start()
    {
        DoOnStart();
        _scroller.Delegate = this;
    }
}
