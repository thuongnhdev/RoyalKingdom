using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System.Linq;

public class UIVassalJobSelect : BasePanel
{
    [SerializeField]
    private TextMeshProUGUI _tmpVassalSize;

    [SerializeField]
    private Image imgJob;

    [SerializeField]
    private GameObject itemAvatarPrefab;

    [SerializeField]
    private RectTransform parentAvatarRectTransform;

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
    private RectTransform BtnStatsCancel;

    [SerializeField]
    private RectTransform BtnStatsChange;

    [SerializeField]
    private Button BtnChange;

    [SerializeField]
    private GameObject imgFadeBtnChange;

    [SerializeField]
    private Button BtnRegister;

    [SerializeField]
    private RectTransform BtnStatsRegister;

    [SerializeField]
    private GameObject imgFadeBtnRegister;

    [SerializeField]
    private RectTransform statsPositionOpen;

    [SerializeField]
    private RectTransform statsPositionClose;

    [SerializeField]
    private TextMeshProUGUI tmpJobLevel;

    [SerializeField]
    private TextMeshProUGUI tmpJobName;

    [SerializeField]
    private TextMeshProUGUI tmpJobClassName;

    [SerializeField]
    private TextMeshProUGUI tmpJobGrade;

    [SerializeField]
    private TextMeshProUGUI tmpPantience;

    [SerializeField]
    private TextMeshProUGUI tmpLucky;

    [SerializeField]
    private TextMeshProUGUI tmpWisdom;

    [SerializeField]
    private TextMeshProUGUI tmpJustice;

    [SerializeField]
    private TextMeshProUGUI tmpAcrobatPoints;

    [SerializeField]
    private TextMeshProUGUI tmpConsumeAcrobatPoints;

    [SerializeField]
    private Image imgJobClassLevelProgress;

    [SerializeField]
    private TextMeshProUGUI tmpJobClassLevel;

    [SerializeField]
    private TextMeshProUGUI tmpJobClassLevelProgress;

    [SerializeField]
    private Image imgVassalAvatar;

    [SerializeField]
    private Sprite[] spriteIconJob;

    [SerializeField]
    private Sprite[] spriteIconStats;

    [SerializeField]
    private GameObject[] objectSubLevel_1;

    [SerializeField]
    private GameObject[] objectSubLevel_2;

    [SerializeField]
    private GameObject[] objectSubLevel_3;

    [SerializeField]
    private GameObject[] objectSubLevel_4;

    [SerializeField]
    private GameObject[] objectSubLevel_5;

    [SerializeField]
    private GameObject[] objectSubLevel_6;

    [SerializeField]
    private VassalSpriteSOList _vassalSpriteSOList;

    [Header("Stat")]
    [SerializeField]
    private GameObject[] _objStatInfo;

    [SerializeField]
    private Image[] _imgIconStatInfo;

    [SerializeField]
    private TextMeshProUGUI[] _tmpNameStatInfo;

    [SerializeField]
    private TextMeshProUGUI[] _tmpLevelStatInfo;

    [SerializeField]
    private JobData jobData;
    private VassalDataInGame vassalDataInGame;
    Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame , JobData> onComplete;
    private List<ItemAvatar> itemAvatars = new List<ItemAvatar>();
    private List<ItemStats> itemStats = new List<ItemStats>();
    public override void SetData(object[] data)
    {
        base.SetData(data);
        int index = (int)data[0];
        jobData = (JobData)data[1];
        vassalDataInGame = (VassalDataInGame)data[2];
        onComplete = (Action<UIVassalJobDetail.TypeSelectJob, VassalDataInGame, JobData>)data[3];
    }

    public override void Open()
    {
        base.Open();
        foreach (Transform child in parentAvatarRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in parentStatsRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }

        itemStats.Clear();
        itemAvatars.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            GameObject item = Instantiate(itemAvatarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentAvatarRectTransform;
            ItemAvatar itemAvatar = item.GetComponent<ItemAvatar>();

            itemAvatar.InitData(i, null, GameData.Instance.SavedPack.SaveData.VassalInfos[i], (vassal) => {
                vassalDataInGame = vassal;
                var vassalParams = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == vassal.Data.idVassalTemplate);
                var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassal.Data.idVassalTemplate);
                UpdateUI(vassalParams, vassalInfo);
                OnShowStats(vassal);
            });
            itemAvatar.transform.localPosition = Vector3.zero;
            itemAvatar.transform.localScale = Vector3.one;
            itemAvatars.Add(itemAvatar);

        }

        for (int i = 0; i < vassalDataInGame.Data.BaseDataVassalStats.Count; i++)
        {
            GameObject item = Instantiate(itemStatsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentStatsRectTransform;
            ItemStats itemStat = item.GetComponent<ItemStats>();

            itemStat.InitData(i, jobData, vassalDataInGame.Data.BaseDataVassalStats[i], spriteIconStats[i]);
            itemStat.transform.localPosition = Vector3.zero;
            itemStat.transform.localScale = Vector3.one;
            itemStats.Add(itemStat);
        }
        ActiveSubJobInStree();

        BtnStatsChange.gameObject.SetActive(false);
        BtnStatsRegister.gameObject.SetActive(true);
        if (vassalDataInGame.Data.Job_1 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassalDataInGame.Data.Job_2 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB
            && vassalDataInGame.Data.Job_3 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB)
        {
            BtnStatsChange.gameObject.SetActive(true);
            BtnStatsRegister.gameObject.SetActive(false);
        }

        if (itemAvatars.Count > 0)
        {
            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
            {
                if (GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data.idVassalTemplate == vassalDataInGame.Data.idVassalTemplate)
                    itemAvatars[i].SetActiveVassal(true);
            }
        }
        imgJob.sprite = spriteIconJob[jobData.Key - 1];
        _tmpVassalSize.text = GameData.Instance.SavedPack.SaveData.VassalInfos.Count.ToString();

        var vassalParams = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == vassalDataInGame.Data.idVassalTemplate);
        var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassalDataInGame.Data.idVassalTemplate);
        UpdateUI(vassalParams, vassalInfo);

    }

    private void UpdateUI(BaseVassalTemplate vassalParams, VassalDataInGame vassalInfo)
    {
        var jobD = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobData.Key);
        var jobClass = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.JobClass == jobD.JobClass);
        var vassalJobGrade = MasterDataStore.Instance.BaseDataVassalJobGrades.Find(t => t.Key == jobD.JobGrade);
        tmpJobLevel.text = jobD.Key.ToString();
        tmpJobGrade.text = GetGradeName(jobD.JobGrade).ToString();
        tmpJobClassName.text = jobD.JobName.ToString();
        tmpJobName.text = jobClass.JobName;
        tmpJobClassLevel.text = vassalInfo.Data.BaseDataVassalJobClassInfos.Find(t => t.IdJobClass == jobData.JobGrade).LvJobClass.ToString();

        List<BaseDataVassalJobClassInfo> newJobList = vassalInfo.Data.BaseDataVassalJobClassInfos.OrderByDescending(x => x.ExpJobClass)
             .ThenBy(x => x.ExpJobClass)
             .ToList();
     ;
        tmpJobClassLevelProgress.text = string.Format("{0}/{1}", newJobList[0].ExpJobClass, 10000);
        float percentExpJobClass = newJobList[0].ExpJobClass / 10000;
        imgJobClassLevelProgress.fillAmount = percentExpJobClass;

        tmpPantience.text = vassalInfo.Data.BaseDataVassalStats[11].Value.ToString();
        tmpLucky.text = vassalInfo.Data.BaseDataVassalStats[15].Value.ToString();
        tmpWisdom.text = vassalInfo.Data.BaseDataVassalStats[3].Value.ToString();
        tmpConsumeAcrobatPoints.text = vassalJobGrade.Consume.ToString();
        tmpAcrobatPoints.text = vassalJobGrade.Consume.ToString();

        var vassalJobInfo = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobData.Key);
        imgVassalAvatar.sprite = _vassalSpriteSOList.GetSmallIcon(vassalInfo.Data.idVassalTemplate);
        BtnChange.enabled = true;
        BtnRegister.enabled = true;
        imgFadeBtnChange.SetActive(false);
        imgFadeBtnRegister.SetActive(false);
        // check show register and change Job
        var itemJobOfVassal = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobD.Key);
        var itemJobClassOfVassal = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == itemJobOfVassal.JobClass);
        int jobClassOfVas = itemJobClassOfVassal.Key - 1;
        var jobOfVassal = vassalInfo.Data.BaseDataVassalJobClassInfos.Find(t => t.IdJobClass == jobClassOfVas);
        if (jobOfVassal.LvJobClass < itemJobOfVassal.Req_JobClass_Lv)
        {
            if (vassalDataInGame.Data.Job_1 != -1 && vassalDataInGame.Data.Job_2 != -1
    && vassalDataInGame.Data.Job_3 != -1)
            {
                BtnStatsChange.gameObject.SetActive(true);
                BtnStatsRegister.gameObject.SetActive(false);
                imgFadeBtnChange.SetActive(true);
                imgFadeBtnRegister.SetActive(true);
                BtnChange.enabled = false;
                BtnRegister.enabled = false;
            }
            else
            {
                BtnStatsChange.gameObject.SetActive(false);
                BtnStatsRegister.gameObject.SetActive(true);
                imgFadeBtnChange.SetActive(true);
                imgFadeBtnRegister.SetActive(true);
                BtnChange.enabled = false;
                BtnRegister.enabled = false;
            }
        }
        else
        {
            if (vassalDataInGame.Data.Job_1 != -1 && vassalDataInGame.Data.Job_2 != -1
         && vassalDataInGame.Data.Job_3 != -1)
            {
                BtnStatsChange.gameObject.SetActive(true);
                BtnStatsRegister.gameObject.SetActive(false);
                imgFadeBtnChange.SetActive(false);
                BtnChange.enabled = true;
            }
            else
            {
                BtnStatsChange.gameObject.SetActive(false);
                BtnStatsRegister.gameObject.SetActive(true);
                imgFadeBtnRegister.SetActive(false);
                BtnRegister.enabled = true;
            }
        }
        OnResetStatObj();
        if (jobData != null)
        {
            var jobInfoBase = MasterDataStore.Instance.BaseVassalJobInfos.Find(t => t.Key == jobData.Key);
            int add_StatKey = jobInfoBase.Add_StatKey_1;
            var itemStat_1 = vassalDataInGame.Data.BaseDataVassalStats.Find(t => t.Key == add_StatKey);
            if (itemStat_1 != null)
            {
                _objStatInfo[0].SetActive(true);
                _imgIconStatInfo[0].sprite = spriteIconStats[itemStat_1.Key];
                var nameStats = MasterDataStore.Instance.BaseDataVassalStats.Find(t => t.Key == add_StatKey).StatName;
                _tmpNameStatInfo[0].text = nameStats.ToString();
                int level = 1;
                for (int i = 0; i < MasterDataStore.Instance.BaseDataVassalStatsExps.Count - 1; i++)
                {
                    if (itemStat_1.Value > MasterDataStore.Instance.BaseDataVassalStatsExps[i].Max_exp && itemStat_1.Value < MasterDataStore.Instance.BaseDataVassalStatsExps[i + 1].Max_exp)
                        level = MasterDataStore.Instance.BaseDataVassalStatsExps[i].Lv;
                }
                _tmpLevelStatInfo[0].text = level.ToString();
            }

            int add_StatKey_2 = jobInfoBase.Add_StatKey_2;
            var itemStat_2 = vassalDataInGame.Data.BaseDataVassalStats.Find(t => t.Key == add_StatKey_2);
            if (itemStat_2 != null)
            {
                _objStatInfo[1].SetActive(true);
                _imgIconStatInfo[1].sprite = spriteIconStats[itemStat_2.Key];
                var nameStats = MasterDataStore.Instance.BaseDataVassalStats.Find(t => t.Key == add_StatKey_2).StatName;
                _tmpNameStatInfo[1].text = nameStats.ToString();
                int level = 1;
                for (int i = 0; i < MasterDataStore.Instance.BaseDataVassalStatsExps.Count - 1; i++)
                {
                    if (itemStat_2.Value > MasterDataStore.Instance.BaseDataVassalStatsExps[i].Max_exp && itemStat_2.Value < MasterDataStore.Instance.BaseDataVassalStatsExps[i + 1].Max_exp)
                        level = MasterDataStore.Instance.BaseDataVassalStatsExps[i].Lv;
                }
                _tmpLevelStatInfo[1].text = level.ToString();
            }

            int add_StatKey_3 = jobInfoBase.Add_StatKey_3;
            var itemStat_3 = vassalDataInGame.Data.BaseDataVassalStats.Find(t => t.Key == add_StatKey_3);
            if (itemStat_3 != null)
            {
                _objStatInfo[2].SetActive(true);
                _imgIconStatInfo[2].sprite = spriteIconStats[itemStat_3.Key];
                var nameStats = MasterDataStore.Instance.BaseDataVassalStats.Find(t => t.Key == add_StatKey_3).StatName;
                _tmpNameStatInfo[2].text = nameStats.ToString();
                int level = 1;
                for (int i = 0; i < MasterDataStore.Instance.BaseDataVassalStatsExps.Count - 1; i++)
                {
                    if (itemStat_3.Value > MasterDataStore.Instance.BaseDataVassalStatsExps[i].Max_exp && itemStat_3.Value < MasterDataStore.Instance.BaseDataVassalStatsExps[i + 1].Max_exp)
                        level = MasterDataStore.Instance.BaseDataVassalStatsExps[i].Lv;
                }
                _tmpLevelStatInfo[2].text = level.ToString();
            }

            int add_StatKey_4 = jobInfoBase.Add_StatKey_4;
            var itemStat_4 = vassalDataInGame.Data.BaseDataVassalStats.Find(t => t.Key == add_StatKey_4);
            if (itemStat_4 != null)
            {
                _objStatInfo[3].SetActive(true);
                _imgIconStatInfo[3].sprite = spriteIconStats[itemStat_4.Key];
                var nameStats = MasterDataStore.Instance.BaseDataVassalStats.Find(t => t.Key == add_StatKey_4).StatName;
                _tmpNameStatInfo[3].text = nameStats.ToString();
                int level = 1;
                for (int i = 0; i < MasterDataStore.Instance.BaseDataVassalStatsExps.Count - 1; i++)
                {
                    if (itemStat_4.Value > MasterDataStore.Instance.BaseDataVassalStatsExps[i].Max_exp && itemStat_4.Value < MasterDataStore.Instance.BaseDataVassalStatsExps[i + 1].Max_exp)
                        level = MasterDataStore.Instance.BaseDataVassalStatsExps[i].Lv;
                }
                _tmpLevelStatInfo[3].text = level.ToString();
            }
        }

        foreach (Transform child in parentStatsRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }


        itemStats.Clear();
        for (int i = 0; i < vassalDataInGame.Data.BaseDataVassalStats.Count; i++)
        {
            GameObject item = Instantiate(itemStatsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentStatsRectTransform;
            ItemStats itemStat = item.GetComponent<ItemStats>();

            itemStat.InitData(i, jobData, vassalDataInGame.Data.BaseDataVassalStats[i], spriteIconStats[i]);
            itemStat.transform.localPosition = Vector3.zero;
            itemStat.transform.localScale = Vector3.one;
            itemStats.Add(itemStat);
        }
    }

    private void OnResetStatObj()
    {
        for(int i = 0;i < _objStatInfo.Length;i++)
        {
            _objStatInfo[i].SetActive(false);
        }
    }

    private string GetGradeName(int key)
    {
        var item = MasterDataStore.Instance.BaseDataVassalJobGrades.Find(t => t.Key == key);
        return item.GradeName;
    }

    private void ActiveSubJobInStree()
    {
        int keyJob = jobData.Key;
        Dictionary<int,List<BaseVassalJobInfo>> dics = new Dictionary<int,List<BaseVassalJobInfo>>();
        for (int i = 0; i < MasterDataStore.Instance.BaseVassalJobInfos.Count; i++)
        {
            var item = MasterDataStore.Instance.BaseVassalJobInfos[i];
            if (item.JobClass == keyJob)
            {
                if (!dics.ContainsKey(item.JobGrade))
                {
                    List<BaseVassalJobInfo> listI = new List<BaseVassalJobInfo>();
                    listI.Add(item);
                    dics.Add(item.JobGrade, listI);
                }
                else
                {
                    List<BaseVassalJobInfo> listI = dics[item.JobGrade];
                    listI.Add(item);
                    dics[item.JobGrade] = listI;
                }
            }
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject[] objList = new GameObject[7];
            if (i == 0)
                objList = objectSubLevel_1;
            if (i == 1)
                objList = objectSubLevel_2;
            if (i == 2)
                objList = objectSubLevel_3;
            if (i == 3)
                objList = objectSubLevel_4;
            if (i == 4)
                objList = objectSubLevel_5;
            if (i == 5)
                objList = objectSubLevel_6;
            for (int j = 0; j < 7; j++)
            {
                objList[j].SetActive(false);
            }
        }

        for (int i = 0; i < 7; i++)
        {
            GameObject[] objList = new GameObject[7];
            if (i == 1)
                objList = objectSubLevel_1;
            if (i == 2)
                objList = objectSubLevel_2;
            if (i == 3)
                objList = objectSubLevel_3;
            if (i == 4)
                objList = objectSubLevel_4;
            if (i == 5)
                objList = objectSubLevel_5;
            if (i == 6)
                objList = objectSubLevel_6;
            if (dics.ContainsKey(i))
            {
                for (int j = 0; j < dics[i].Count; j++)
                {
                    objList[j].SetActive(true);
                    ItemButtonJobInTree item = objList[j].GetComponent<ItemButtonJobInTree>();
                    var data = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == dics[i][j].Key);
                    item.InitData(dics[i][j].Key, data, vassalDataInGame, spriteIconJob[data.JobClass - 1], (typeSelectJob, vassal, job) =>
                      {
                          jobData = job;
                          var vassalParams = MasterDataStore.Instance.BaseVassalTemplates.Find(t => t.Key == vassalDataInGame.Data.idVassalTemplate);
                          var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassalDataInGame.Data.idVassalTemplate);
                          UpdateUI(vassalParams, vassalInfo);
                          
                      });
                }
            }
        }
    }

    private void OnShowStats(VassalDataInGame vassal)
    {
        for (var i = 0; i < itemAvatars.Count; i++)
        {
            if (itemAvatars[i].VassalData.Data.idVassalTemplate != vassal.Data.idVassalTemplate)
                itemAvatars[i].OnReset();
        }
        for (int i = 0; i < vassal.Data.BaseDataVassalStats.Count; i++)
        {
            itemStats[i].UpdateUI(vassal.Data.BaseDataVassalStats[i]);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnClickClose()
    {
        if (UIManagerTown.Instance.UIVassalInfomation.isActiveAndEnabled && !UIManagerTown.Instance.UIVassalJobDetail.isActiveAndEnabled)
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

    public void OnClickCancel()
    {
        OnClickClose();
    }

    public void OnClickChange()
    {
        UIManagerTown.Instance.ShowUIVassalJobChange(jobData.Key, jobData, vassalDataInGame, onComplete);
    }

    public void OnClickSelectJob(int id)
    {
        onComplete?.Invoke(UIVassalJobDetail.TypeSelectJob.Select, vassalDataInGame, jobData);
    }

    public void OnClickRegister()
    {
        onComplete?.Invoke(UIVassalJobDetail.TypeSelectJob.Register, vassalDataInGame , jobData);
        OnClickClose();
    }
}
