using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIVassalJobDetail;

public class ItemJobInformation : MonoBehaviour
{
    [SerializeField]
    private Image _imgIconEnable;

    [SerializeField]
    private Image _imgIconJob;

    [SerializeField]
    private GameObject panelEnable;

    [SerializeField]
    private GameObject panelDisable;

    public int Index;
    public JobData JobData;
    MasterDataStore masterDataStore;
    private Action<int, int> _onClickDetail;
    private Action<int, int> _onClickSelect;
    public void InitData(Sprite sprJob,VassalDataInGame vassal,int index, JobData jobData, Action<int, int> onClickDetail, Action<int, int> onClickSelect)
    {
        Index = index;
        JobData = jobData;
        _imgIconJob.sprite = sprJob;
        panelEnable.SetActive(false);
        panelDisable.SetActive(true);
        _onClickDetail = onClickDetail;
        _onClickSelect = onClickSelect;
        if (jobData == null)
            return;
        if (jobData.Key == vassal.Data.Job_1)
        {
            panelEnable.SetActive(true);
            panelDisable.SetActive(false);
        }
        if (jobData.Key == vassal.Data.Job_2)
        {
            panelEnable.SetActive(true);
            panelDisable.SetActive(false);
        }
        if (jobData.Key == vassal.Data.Job_3)
        {
            panelEnable.SetActive(true);
            panelDisable.SetActive(false);
        }
    }

    public void OnClickOpenJob()
    {
        int level = 1;
        if (JobData != null)
            level = JobData.Level;
        if (panelEnable.activeInHierarchy)
        {
            _onClickSelect?.Invoke(Index, level);
        }
        else
        {
            _onClickDetail?.Invoke(Index, level);
        }
       
    }

    public void OnReset(int index, bool isSelect)
    {

    }
    public void OnEventSelectJob(TypeSelectJob typeSelectJob)
    {
    }
}
