using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemVassalJob : MonoBehaviour
{
    [SerializeField]
    private Image _imgIcon;

    [SerializeField]
    private Image _imgBgJob;

    [SerializeField]
    private GameObject _tickSelect;

    [SerializeField]
    private GameObject _imgEffectSelect;

    [SerializeField]
    private TextMeshProUGUI _tmpLevel;

    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private Sprite[] sprLevelJob;

    [SerializeField]
    private Image _imgLoad;

    [SerializeField]
    private TextMeshProUGUI _tmpPercentLoad;


    public int Index;
    public JobData JobData;
    MasterDataStore masterDataStore;
    private Action<int, JobData> _onClickDetail;
    public void InitData(VassalDataInGame vassal,int index,JobData jobData,Sprite icon, Action<int, JobData> onClickDetail)
    {
        Index = index;
        JobData = jobData;
        _onClickDetail = onClickDetail;
        _tmpLevel.text = jobData.Level.ToString();
        _tmpName.text = jobData.JobName.ToString();
        if (icon != null) _imgIcon.sprite = icon;
        _imgBgJob.sprite = sprLevelJob[jobData.JobGrade - 1];
        _tickSelect.gameObject.SetActive(false);
        if (jobData.Key == vassal.Data.Job_1 && vassal.Data.Job_1 != -1)
        {
            _tickSelect.gameObject.SetActive(true);
        }
        if (jobData.Key == vassal.Data.Job_2 && vassal.Data.Job_2 != -1)
        {
            _tickSelect.gameObject.SetActive(true);
        }
        if (jobData.Key == vassal.Data.Job_3 && vassal.Data.Job_3 != -1)
        {
            _tickSelect.gameObject.SetActive(true);
        }

        var itemJobOfVassal = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobData.Key);
        var itemJobClassOfVassal = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == itemJobOfVassal.JobClass);
        int jobClassOfVas = itemJobClassOfVassal.Key - 1;
        var jobOfVassal = vassal.Data.BaseDataVassalJobClassInfos.Find(t => t.IdJobClass == jobClassOfVas);
        float expJob = jobOfVassal.ExpJobClass;
        float duration = expJob;
        if (expJob >= 10000)
            duration = expJob - 10000;
        float percent = duration / 10000;
        _tmpPercentLoad.text = string.Format("{0}%", percent);
        _imgLoad.fillAmount = percent;
    }
    public void UpdateData(VassalDataInGame vassal, int index, JobData jobData, Sprite icon)
    {
        Index = index;
        JobData = jobData;
        _tmpLevel.text = jobData.Level.ToString();
        _tmpName.text = jobData.JobName.ToString();
        if (icon != null) _imgIcon.sprite = icon;
        _imgBgJob.sprite = sprLevelJob[jobData.JobGrade - 1];
        _tickSelect.gameObject.SetActive(false);
        if (jobData.Key == vassal.Data.Job_1)
        {
            _tickSelect.gameObject.SetActive(true);
        }
        if (jobData.Key == vassal.Data.Job_2)
        {
            _tickSelect.gameObject.SetActive(true);
        }
        if (jobData.Key == vassal.Data.Job_3)
        {
            _tickSelect.gameObject.SetActive(true);
        }
    }

    public void UpDateUI()
    {
        _imgEffectSelect.SetActive(false);
        _tickSelect.gameObject.SetActive(true);
    }

    public void OnClickOpenJob()
    {
        _imgEffectSelect.SetActive(true);
        _onClickDetail?.Invoke(Index, JobData);
    }

    public void OnReset()
    {
        _tickSelect.SetActive(false);
        _imgEffectSelect.SetActive(false);
    }

    public void OnResetHighlight()
    {
        _imgEffectSelect.SetActive(false);
    }
}
