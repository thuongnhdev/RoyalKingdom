using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;

public class UIVassalJobDetail : BasePanel
{
    [SerializeField]
    private TextMeshProUGUI _tmpVassalSize;

    [SerializeField]
    private GameObject itemAvatarPrefab;

    [SerializeField]
    private RectTransform parentAvatarRectTransform;

    [SerializeField]
    private GameObject itemJobPrefab;

    [SerializeField]
    private RectTransform parentJobRectTransform;

    [SerializeField]
    private GameObject itemStatsPrefab;

    [SerializeField]
    private RectTransform parentStatsRectTransform;

    [SerializeField]
    private RectTransform statsRectTransform;

    [SerializeField]
    private RectTransform BtnStatsOpen;

    [SerializeField]
    private RectTransform BtnStatsClose;

    [SerializeField]
    private RectTransform statsPositionOpen;

    [SerializeField]
    private RectTransform statsPositionClose;

    [SerializeField]
    private Sprite[] spriteIconJob;

    [SerializeField]
    private GameEvent _onUpdateJobVassal;

    [SerializeField]
    private Sprite[] spriteAvatarVassal;

    [SerializeField]
    private Sprite[] spriteIconStats;

    MasterDataStore _masterDataStore;
    private List<ItemAvatar> itemAvatars = new List<ItemAvatar>();
    private List<ItemVassalJob> itemVassalJob = new List<ItemVassalJob>();
    private List<ItemStats> itemStats = new List<ItemStats>();
    private VassalDataInGame vassalDataInGame;
    public enum TypeSelectJob
    {
        Select = 1,
        Register = 2,
        Change = 3,
        None
    }

    public override void SetData(object[] data)
    {
        base.SetData(data);
        vassalDataInGame = (VassalDataInGame)data[0];
    }

    public override void Open()
    {
        base.Open();
        foreach (Transform child in parentAvatarRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in parentJobRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in parentStatsRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        _masterDataStore = MasterDataStore.Instance;
        itemStats.Clear();
        itemAvatars.Clear();
        itemVassalJob.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            GameObject item = Instantiate(itemAvatarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentAvatarRectTransform;
            ItemAvatar itemAvatar = item.GetComponent<ItemAvatar>();

            itemAvatar.InitData(i, spriteAvatarVassal[i], GameData.Instance.SavedPack.SaveData.VassalInfos[i], (vassal) => {
                OnSelectVassalJob(vassal);
            });
            itemAvatar.transform.localPosition = Vector3.zero;
            itemAvatar.transform.localScale = Vector3.one;
            itemAvatars.Add(itemAvatar);
        }

        for (int i = 0; i < 12; i++)
        {
            GameObject item = Instantiate(itemJobPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentJobRectTransform;
            ItemVassalJob itemJob = item.GetComponent<ItemVassalJob>();

            itemJob.InitData(vassalDataInGame, i, GameData.Instance.SavedPack.SaveData.JobDatas[i], spriteIconJob[i], (a, jobData) => {
                OnClickOpenJobDetail(a, jobData);
                OnClickCloseStats();
                UIManagerTown.Instance.ShowUIVassarSelectJob(a, jobData, vassalDataInGame, (typeSelectJob, vassal, jobData) => { OnEventSelectJob(typeSelectJob, vassal, jobData); });
            });
            itemJob.transform.localPosition = Vector3.zero;
            itemJob.transform.localScale = Vector3.one;
            itemVassalJob.Add(itemJob);
        }

        for (int i = 0; i < vassalDataInGame.Data.BaseDataVassalStats.Count; i++)
        {
            GameObject item = Instantiate(itemStatsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentStatsRectTransform;
            ItemStats itemStat = item.GetComponent<ItemStats>();

            itemStat.InitData(i, null, vassalDataInGame.Data.BaseDataVassalStats[i], spriteIconStats[i]);
            itemStat.transform.localPosition = Vector3.zero;
            itemStat.transform.localScale = Vector3.one;
            itemStats.Add(itemStat);
        }
        if (itemAvatars.Count > 0)
        {
            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
            {
                if (GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data.idVassalTemplate == vassalDataInGame.Data.idVassalTemplate)
                    itemAvatars[i].SetActiveVassal(true);
            }
        }
        _tmpVassalSize.text = GameData.Instance.SavedPack.SaveData.VassalInfos.Count.ToString();
        OnClickCloseStats();
        for (int i = 0; i < vassalDataInGame.Data.BaseDataVassalStats.Count; i++)
        {
            itemStats[i].UpdateUI(vassalDataInGame.Data.BaseDataVassalStats[i]);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnClickClose()
    {
        if (UIManagerTown.Instance.UIVassalInfomation.isActiveAndEnabled)
            UIManagerTown.Instance.UIVassalInfomation.OnEnableModel();
       Close();
    }

    public void OnClickOpenStats()
    {
        BtnStatsClose.gameObject.SetActive(true);
        BtnStatsOpen.gameObject.SetActive(false);
        statsRectTransform.DOLocalMoveX(statsPositionOpen.localPosition.x, 0.5f).SetEase(Ease.Linear);
    }

    public void OnClickCloseStats()
    {
        BtnStatsClose.gameObject.SetActive(false);
        BtnStatsOpen.gameObject.SetActive(true);
        statsRectTransform.DOLocalMoveX(statsPositionClose.localPosition.x, 0.5f).SetEase(Ease.Linear);
    }

    public void OnSelectVassalJob(VassalDataInGame vassal)
    {
        for(var i = 0;i< itemAvatars.Count;i++)
        {
            if (itemAvatars[i].VassalData.Data.idVassalTemplate != vassal.Data.idVassalTemplate)
                itemAvatars[i].OnReset();
        }
        for (int i = 0; i < vassal.Data.BaseDataVassalStats.Count; i++)
        {
            itemStats[i].UpdateUI(vassal.Data.BaseDataVassalStats[i]);
        }
        for (int i = 0; i < itemVassalJob.Count; i++)
        {
            itemVassalJob[i].InitData(vassal, i, GameData.Instance.SavedPack.SaveData.JobDatas[i], spriteIconJob[i], (a, jobData) => {
                OnClickOpenJobDetail(a, jobData);
                UIManagerTown.Instance.ShowUIVassarSelectJob(a, jobData, vassal, (typeSelectJob, vassal, jobData) => { OnEventSelectJob(typeSelectJob, vassal, jobData); });
            });
        }
        vassalDataInGame = vassal;
    }

    private void OnClickOpenJobDetail(int index, JobData jobData)
    {
        for (int i = 0; i < itemVassalJob.Count; i++)
        {
            if (itemVassalJob[i].JobData.Key != jobData.Key)
                itemVassalJob[i].OnResetHighlight();
        }
    }

    public void OnEventSelectJob(TypeSelectJob typeSelectJob , VassalDataInGame vassal , JobData jobData)
    {
        List<int> jobList = new List<int>();
        if (vassal.Data.Job_1 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassal.Data.Job_1 != vassal.Data.Job_2 && vassal.Data.Job_1 != vassal.Data.Job_3) jobList.Add(vassal.Data.Job_1);
        if (vassal.Data.Job_2 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassal.Data.Job_2 != vassal.Data.Job_1 && vassal.Data.Job_2 != vassal.Data.Job_3) jobList.Add(vassal.Data.Job_2);
        if (vassal.Data.Job_3 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassal.Data.Job_3 != vassal.Data.Job_1 && vassal.Data.Job_3 != vassal.Data.Job_2) jobList.Add(vassal.Data.Job_3);

        var vassalUpdate = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassal.Data.idVassalTemplate);
   
        if(typeSelectJob == TypeSelectJob.Register)
        {
            
            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.JobDatas.Count; i++)
            {
                var item = GameData.Instance.SavedPack.SaveData.JobDatas[i];
                if (item.Key == jobData.Key)
                {
                    if (vassalUpdate.Data.Job_1 == -1 && vassalUpdate.Data.Job_2  != jobData.Key && vassalUpdate.Data.Job_3 != jobData.Key) vassalUpdate.Data.Job_1 = jobData.Key;
                    else if (vassalUpdate.Data.Job_2 == -1 && vassalUpdate.Data.Job_1 != jobData.Key && vassalUpdate.Data.Job_3 != jobData.Key) vassalUpdate.Data.Job_2 = jobData.Key;
                    else if (vassalUpdate.Data.Job_3 == -1 && vassalUpdate.Data.Job_2 != jobData.Key && vassalUpdate.Data.Job_1 != jobData.Key) vassalUpdate.Data.Job_3 = jobData.Key;
                }
            }
            for (int i = 0; i < itemVassalJob.Count; i++)
            {
                var item = itemVassalJob[i];
                item.OnReset();
                if (item.JobData.Key == vassal.Data.Job_1 || item.JobData.Key == vassal.Data.Job_2 ||
                    item.JobData.Key == vassal.Data.Job_3)
                {
                    item.UpDateUI();
                }
            }
        }
        GameData.Instance.RequestSaveGame();

        _onUpdateJobVassal?.Raise(vassalUpdate);
    }
}
