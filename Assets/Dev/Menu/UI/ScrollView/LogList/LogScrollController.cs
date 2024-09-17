using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogScrollController : BaseScrollController
{
    public List<string> sampleLogs = new();

    public void SetUpDisplayData(string[] logs)
    {
        _dataList.Clear();
        for (int i = 0; i < logs.Length; i++)
        {
            _dataList.Add(logs[i]);
        }

        ReloadScrollView();
    }

    public void RemoveAfter_DisplaySampleData()
    {
        _dataList.Clear();
        for (int i = 0; i < sampleLogs.Count; i++)
        {
            _dataList.Add(sampleLogs[i]);
        }

        ReloadScrollView();
    }
}
