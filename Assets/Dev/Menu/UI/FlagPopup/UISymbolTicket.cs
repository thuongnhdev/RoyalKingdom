using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreData.UniFlow;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;

public class UISymbolTicket : BasePanel
{

    private BasePanel previousPanel;

    private MasterDataStore _masterDataStore;

    [SerializeField]
    private TextMeshProUGUI _tmpSymbolTicket;

    private Action<bool> _onComplete;
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }


    public override void SetData(object[] data)
    {
        base.SetData(data);
        _onComplete = (Action<bool>)data[0];
    }

    public override void Open()
    {
        base.Open();

        _masterDataStore = MasterDataStore.Instance;

    }

    public override void Close()
    {
        base.Close();
        if (previousPanel != null)
        {
            previousPanel.Open();
            previousPanel = null;
        }
    }

    public void OnClickCancel()
    {
        Close();
        _onComplete?.Invoke(false);
    }

    public void OnClickConfirm()
    {
        Close();
        _onComplete?.Invoke(true);
    }

}
