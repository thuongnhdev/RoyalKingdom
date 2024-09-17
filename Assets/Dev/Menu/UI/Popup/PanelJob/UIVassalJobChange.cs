using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CoreData.UniFlow;
using UnityEngine.UI;
using CoreData.UniFlow.Common;

public class UIVassalJobChange : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private GameObject _btnFadeConfirm;

    [SerializeField]
    private Button _imgFadeConfirm;

    [SerializeField]
    private Sprite[] _sprJob;

    [SerializeField]
    private Image[] _imgJobCurrent;

    [SerializeField]
    private GameObject[] _objImageSelect;

    [SerializeField]
    private Image _imgJobChange;

    private JobData jobData;
    private int idJobChange;
    private int indexJobSelectChange;
    private VassalDataInGame vassalDataInGame;
    Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame, JobData> onComplete;

    public override void SetData(object[] data)
    {
        base.SetData(data);
        idJobChange = (int)data[0];
        jobData = (JobData)data[1];
        vassalDataInGame = (VassalDataInGame)data[2];
        onComplete = (Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame, JobData>)data[3];
    }

    public override void Open()
    {
        base.Open();
        indexJobSelectChange = -1;

        int keyJob_1 = vassalDataInGame.Data.Job_1;
        int keyJob_2 = vassalDataInGame.Data.Job_2;
        int keyJob_3 = vassalDataInGame.Data.Job_3;

        _imgJobCurrent[0].sprite = _sprJob[keyJob_1 - 1];
        _imgJobCurrent[1].sprite = _sprJob[keyJob_2 - 1];
        _imgJobCurrent[2].sprite = _sprJob[keyJob_3 - 1];

        _imgJobChange.sprite = _sprJob[jobData.JobClass - 1];

        _btnFadeConfirm.SetActive(true);
        _imgFadeConfirm.enabled = false;
    }

    private void Reset(int index)
    {
        indexJobSelectChange = index;
        for (int i = 0;i< _objImageSelect.Length;i++)
        {
            _objImageSelect[i].SetActive(false);
        }
        _btnFadeConfirm.SetActive(false);
        _imgFadeConfirm.enabled = true;
        _objImageSelect[indexJobSelectChange].SetActive(true);

        // check show register and change Job
        var jobD = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobData.Key);
        var itemJobOfVassal = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobD.Key);
        var itemJobClassOfVassal = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == itemJobOfVassal.JobClass);
        int jobClassOfVas = itemJobClassOfVassal.Key - 1;
        var jobOfVassal = vassalDataInGame.Data.BaseDataVassalJobClassInfos.Find(t => t.IdJobClass == jobClassOfVas);
        if (jobOfVassal.LvJobClass < itemJobOfVassal.Req_JobClass_Lv)
        {
            _imgFadeConfirm.enabled = false;
            _btnFadeConfirm.SetActive(true);
        }
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

    public void OnClickSelectJob(int index)
    {
        Reset(index);
    }

    public void OnClickConfirm()
    {
        if (indexJobSelectChange == -1) return;
        int _keyChoice = 0;
        switch (indexJobSelectChange)
        {
            case 0:
                _keyChoice = vassalDataInGame.Data.Job_1;
                break;
            case 1:
                _keyChoice = vassalDataInGame.Data.Job_2;
                break;
            case 2:
                _keyChoice = vassalDataInGame.Data.Job_3;
                break;
        }
       
        var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.Job_1 == _keyChoice);
        if (vassal != null) vassal.Data.Job_1 = idJobChange;
        var vassal_1 = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.Job_2 == _keyChoice);
        if (vassal_1 != null) vassal_1.Data.Job_2 = idJobChange;
        var vassal_2 = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.Job_3 == _keyChoice);
        if (vassal_2 != null) vassal_2.Data.Job_3 = idJobChange;

        GameData.Instance.RequestSaveGame();
        onComplete?.Invoke(UIVassalJobDetail.TypeSelectJob.Register, vassalDataInGame, jobData);
        Close();
    }

}
