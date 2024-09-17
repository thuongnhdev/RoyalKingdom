using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonJobInTree : MonoBehaviour
{
    [SerializeField]
    public Image imgIcon;

    private JobData jobData;
    private int idJobChange;
    private int indexJobSelectChange;
    private VassalDataInGame vassalDataInGame;
    Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame, JobData> _onComplete;

    public void InitData(int id , JobData data, VassalDataInGame vassal, Sprite spr, Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame, JobData> onComplete)
    {
        idJobChange = id;
        jobData = data;
        vassalDataInGame = vassal;
        _onComplete = onComplete;
        imgIcon.sprite = spr;
    }

  
    public void OnClickJob()
    {
        if (vassalDataInGame.Data.Job_1 != -1 && vassalDataInGame.Data.Job_2 != -1
           && vassalDataInGame.Data.Job_3 != -1)
        {
            UIManagerTown.Instance.ShowUIVassalJobChange(jobData.Key, jobData, vassalDataInGame, _onComplete);
        }
        else
        {
            _onComplete?.Invoke(UIVassalJobDetail.TypeSelectJob.Register, vassalDataInGame, jobData);
        }
       
    }

  
}
