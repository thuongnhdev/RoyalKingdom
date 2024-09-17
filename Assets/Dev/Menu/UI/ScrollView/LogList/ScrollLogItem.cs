using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollLogItem : BaseCustomCellView
{
    [Header("Config")]
    [SerializeField]
    private TMP_Text _logDateTimeText;
    [SerializeField]
    private TMP_Text _logText;

    public override void SetUp(object data)
    {
        string logData = (string)data;
        if (string.IsNullOrEmpty(logData))
        {
            return;
        }

        string[] logChunks = logData.Split("$$");
        _logDateTimeText.text = logChunks[0];
        _logText.text = logChunks[1];
    }
}
