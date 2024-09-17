using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CoreData.UniFlow;
using static UIVassalJobDetail;
using CoreData.UniFlow.Common;
using System.Linq;
using Cysharp.Threading.Tasks;

public class UIVassalInfomation : BasePanel
{
 
    [SerializeField]
    private Image[] _imgIconStatTop;

    [SerializeField]
    private TextMeshProUGUI[] _tmpStatTop;

    [SerializeField]
    private TextMeshProUGUI[] _tmpStatPointTop;

    [SerializeField]
    private Image[] _ProgressStatTop;

    [SerializeField]
    private TextMeshProUGUI _tmpJobClassLevel;

    [SerializeField]
    private TextMeshProUGUI _tmpPointJobClassLevel;

    [SerializeField]
    private Image _progressJobClassLevel;

    [SerializeField]
    private Image _imgCharacter;

    [SerializeField]
    private RectTransform[] _posJob;

    [SerializeField]
    private ItemJobInformation[] _itemJobInformation;

    private MasterDataStore _masterDataStore;

    [SerializeField]
    private RectTransform parentRectTransform;

    [SerializeField]
    private GameObject itemJobPrefab;

    [SerializeField]
    private RectTransform parentJobRectTransform;
    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private TextMeshProUGUI _tmpVassalSize;

    [SerializeField]
    private TextMeshProUGUI _tmpName;

    [SerializeField]
    private TextMeshProUGUI _tmpLandlord;

    [SerializeField]
    private TextMeshProUGUI _tmpReligion;

    [SerializeField]
    private TextMeshProUGUI _tmpGovernment;

    [SerializeField]
    private TextMeshProUGUI _tmpHomeTown;

    [SerializeField]
    private TextMeshProUGUI _tmpStatus;

    [SerializeField]
    private Sprite[] spriteAvatarVassal;

    [SerializeField]
    private GameObject[] vassalModel;

    [SerializeField]
    private Animator[] vassalAnimator;

    [SerializeField]
    private CharacterAnimation[] vassalAnimation;
 
    [SerializeField]
    private GameObject[] vassalModel_tooltip;

    [Header("Tooltip")]
    [SerializeField]
    private GameObject _tooltipMood;

    [SerializeField]
    private GameObject _tooltipHeath;

    [SerializeField]
    private GameObject _tooltipStatus;

    [SerializeField]
    private GameObject _tooltipOther;

    [SerializeField]
    private GameObject _tooltipOtherJob;

    [SerializeField]
    private GameObject _panelAvatar;

    [SerializeField]
    private GameObject _panelInfo;

    [SerializeField]
    private GameObject _panelCharacter;

    [SerializeField]
    private Sprite[] spriteIconJob;

    [SerializeField]
    private GameEvent _onUpdateJobVassal;

    [Header("Salary")]
    [SerializeField]
    private Toggle _toggleSalary;

    [SerializeField]
    private GameObject _objCheckSalary;

    [SerializeField]
    private Image _imgFillSalaryScroll;

    [SerializeField]
    private Scrollbar _salaryScrollBar;

    [SerializeField]
    private TextMeshProUGUI _tmpSalary;

    [Header("TooltipStatus")]
    [SerializeField]
    private TextMeshProUGUI _tmpTooltipStatusStatus;

    [Header("Salary")]
    [SerializeField]
    private TextMeshProUGUI _tmpVassalNameTooltip;

    [SerializeField]
    private TextMeshProUGUI _tmpVassalGenderTooltip;

    [SerializeField]
    private TextMeshProUGUI _tmpVassalBirthDayTooltip;

    [Header("StatTop")]
    [SerializeField]
    private Sprite[] _sprStatIcon;

    [SerializeField]
    private Image[] _imgProgressStat;

    [SerializeField]
    private TextMeshProUGUI[] _tmpProgressValueStat_Name;

    [SerializeField]
    private TextMeshProUGUI[] _tmpProgressValueStat_Value;

    [SerializeField]
    private TextMeshProUGUI[] _tmpProgressValueStat;

    [SerializeField]
    private Image _imgProgressValueStat_Loyalty;

    [SerializeField]
    private TextMeshProUGUI _tmpProgressValueStat_Loyalty;

    [SerializeField]
    private TextMeshProUGUI _tmpProgressValueJobClassLevel;

    [Header("StatTop")]
    [SerializeField]
    private GameObject _prefabItemLog;

    [SerializeField]
    private RectTransform _parentItemLog;

    [SerializeField]
    private GameObject _panelHistoryLog;

    private VassalDataInGame vassalDataInGame;
    private BaseVassalTemplate _baseVassalTemplateCurrent;
    private List<ItemAvatar> itemAvatars = new List<ItemAvatar>();
    private List<ItemJobInformation> ItemJobInformations = new List<ItemJobInformation>();

    private void OnEnable()
    {
        _onUpdateJobVassal.Subcribe(OnUpdateJobVassal);
        _toggleSalary.onValueChanged.AddListener((value) =>
        {
            OnValueChangeSalary(value);
        });
        _salaryScrollBar.onValueChanged.AddListener((float val) => ScrollbarSalaryCallback(val));
    }

    private void OnDisable()
    {
        _onUpdateJobVassal.Unsubcribe(OnUpdateJobVassal);
        _salaryScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarSalaryCallback(val));
    }

    private void OnValueChangeSalary(bool value)
    {
        if(value)
        {
            var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassalDataInGame.Data.idVassalTemplate);
            if(vassalInfo != null)
            {
                vassalInfo.Salary = (int)this._salary;
                vassalInfo.PercentSalary = _imgFillSalaryScroll.fillAmount;
                GameData.Instance.RequestSaveGame();
            }
        }
    }

    void ScrollbarSalaryCallback(float salary)
    {
        _imgFillSalaryScroll.fillAmount = salary;
        _salaryScrollBar.value = salary;
        int step = MoodKey(salary, 0.167f);
        OnClickSalaryLevel(step);

    }

    public override void SetData(object[] data)
    {
        base.SetData(data);
    }

    public override void Open()
    {
        base.Open();
        foreach (Transform child in parentRectTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        vassalModel[0].SetActive(true);
        vassalAnimation[0].ActionReset(vassalAnimator[0]);
        vassalAnimation[0].ActionIdle_99(vassalAnimator[0], () => { });
        _masterDataStore = MasterDataStore.Instance;
        vassalDataInGame = GameData.Instance.SavedPack.SaveData.VassalInfos[0];
        itemAvatars.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = parentRectTransform;
            ItemAvatar itemAvatar = item.GetComponent<ItemAvatar>();

            itemAvatar.InitData(i, spriteAvatarVassal[i], GameData.Instance.SavedPack.SaveData.VassalInfos[i], (vassal) => {
                OnSelectVassalJob(vassal);
            });
            itemAvatar.transform.localPosition = Vector3.zero;
            itemAvatar.transform.localScale = Vector3.one;
            itemAvatars.Add(itemAvatar);
        }
        if (itemAvatars.Count > 0)
            itemAvatars[0].SetActiveVassal(true);
        ItemJobInformations.Clear();
        for (int i = 0; i < 3; i++)
        {
            ItemJobInformation itemJob = _itemJobInformation[i];
            int key = 0;
            var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos[0];
            JobData jobData = null;
            if (i == 0)
            {
                key = vassal.Data.Job_1;
                var job = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == vassal.Data.Job_1);
                if (job != null)
                    jobData = job;
            }
            if (i == 1)
            {
                key = vassal.Data.Job_2;
                var job = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == vassal.Data.Job_2);
                if (job != null)
                    jobData = job;
            }
            if (i == 2)
            {
                key = vassal.Data.Job_3;
                var job = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == vassal.Data.Job_3);
                if (job != null)
                    jobData = job;
            }
            if(GameData.Instance.SavedPack.SaveData.VassalInfos.Count > 0)
            {
                int indexSprJob = 0;
                if (jobData != null)
                    indexSprJob = GameData.Instance.SavedPack.SaveData.JobDatas.FindIndex(t => t.Key == jobData.Key);
                itemJob.InitData(spriteIconJob[indexSprJob], vassal, i, jobData, (a, b) =>
                {
                    OnDisableModel();
                    UIManagerTown.Instance.ShowUIVassarJobDetail(vassal);
                }, (c, d) =>
                {
                    OnDisableModel();
                    UIManagerTown.Instance.ShowUIVassarSelectJob(0, jobData, vassalDataInGame, (typeSelectJob, vassal, jobData) => { OnEventSelectJob(typeSelectJob, vassal, jobData); });
                });
            }
          
            itemJob.transform.localScale = Vector3.one;
            ItemJobInformations.Add(itemJob);
        }
        _tmpVassalSize.text = GameData.Instance.SavedPack.SaveData.VassalInfos.Count.ToString();
        var vassalParams = _masterDataStore.BaseVassalTemplates[0];
        _baseVassalTemplateCurrent = vassalParams;
        UpdateUI(vassalParams, vassalDataInGame);
    }

    public void OnSelectVassalJob(VassalDataInGame vassal)
    {
        _imgFillSalaryScroll.fillAmount = 0;
        for (var i = 0; i < itemAvatars.Count; i++)
        {
            if (itemAvatars[i].VassalData.Data.idVassalTemplate != vassal.Data.idVassalTemplate)
                itemAvatars[i].OnReset();
        }
        var vassalParams = _masterDataStore.BaseVassalTemplates.Find(t => t.Key == vassal.Data.idVassalTemplate);
        var vassalInfo = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassal.Data.idVassalTemplate);
        UpdateUI(vassalParams, vassalInfo);
        _baseVassalTemplateCurrent = vassalParams;
        OnDisableModel();
        for(int i = 0;i< GameData.Instance.SavedPack.SaveData.VassalInfos.Count;i++)
        {
            if (vassal.Data.idVassalTemplate == GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data.idVassalTemplate)
            {
                vassalModel[i].SetActive(true);
                vassalAnimation[i].ActionReset(vassalAnimator[i]);
                vassalAnimation[i].ActionIdle_99(vassalAnimator[i], () => { });
            }
        }

        UpdateJobUI(vassal);
    }

    public async void OnClickTouch()
    {
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            if (vassalModel[i].activeInHierarchy)
            {
                // Avoid any reload.
                vassalAnimation[i].ActionReset(vassalAnimator[i]);
                vassalAnimation[i].ActionTouch(vassalAnimator[i], async () =>
                {
                    await UniTask.DelayFrame(400);
                    if (i < vassalAnimator.Length && i < vassalAnimation.Length)
                        vassalAnimation[i].ActionIdle_99(vassalAnimator[i], () => { });
                });
            }
        }
    }

    bool isPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public void OnUpdateJobVassal(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        VassalDataInGame vassal = (VassalDataInGame)eventParam[0];
        if (_baseVassalTemplateCurrent.Key != vassal.Data.idVassalTemplate)
            return;
        UpdateJobUI(vassal);
    }

    private void UpdateUI(BaseVassalTemplate vassalParams, VassalDataInGame vassalInfo)
    {
        this._salary = 0;
        _imgFillSalaryScroll.fillAmount = 0;
        _objCheckSalary.SetActive(true);
        _toggleSalary.gameObject.SetActive(false);
        _salaryScrollBar.value = 0;
        _tmpName.text = vassalParams.LastName.ToString();
        _tmpVassalNameTooltip.text = vassalParams.LastName.ToString();
        _tmpVassalGenderTooltip.text = "FeMale";
        if (vassalParams.Gender == 0)
            _tmpVassalGenderTooltip.text = "Male";
        _tmpVassalBirthDayTooltip.text = vassalParams.Birth.ToString();
        _tmpLandlord.text = vassalParams.LastName.ToString();
        _tmpReligion.text = vassalParams.Religion.ToString();
        _tmpGovernment.text = vassalParams.FirstName.ToString();
        _tmpHomeTown.text = vassalParams.Hometown.ToString();
        _tmpStatus.text = vassalInfo.Data.Status.ToString();
        _tmpSalary.text = vassalInfo.Salary.ToString();
        _salaryScrollBar.value = vassalInfo.PercentSalary;
        _imgFillSalaryScroll.fillAmount = vassalInfo.PercentSalary;
        UpdateStatUI(vassalInfo);
    }

    private void UpdateStatUI(VassalDataInGame vassalInfo)
    {
        List<BaseDataVassalStatValue> newStatList = vassalInfo.Data.BaseDataVassalStats.OrderByDescending(x => x.Value)
                  .ThenBy(x => x.Value)
                  .ToList();
        List<BaseDataVassalJobClassInfo> newJobList = vassalInfo.Data.BaseDataVassalJobClassInfos.OrderByDescending(x => x.ExpJobClass)
                .ThenBy(x => x.ExpJobClass)
                .ToList();
        for (int i = 0; i < 3; i++)
        {
            var itemCurrent = _masterDataStore.GetLevelVassalStatExp(newStatList[i].Value);
            int valueStat = (int)newStatList[i].Value;
            int levelExp = 1;
            for (int j = 0; j < MasterDataStore.Instance.BaseDataVassalStatsExpIngames.Count; j++)
            {
                var item = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j];
                var itemNext = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j];
                if (j < MasterDataStore.Instance.BaseDataVassalStatsExpIngames.Count - 1)
                    itemNext = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[j + 1];

                if (valueStat >= item.TotalExpLevelUp && valueStat < itemNext.TotalExpLevelUp)
                {
                    levelExp = item.Lv;
                    break;
                }
            }

            _tmpProgressValueStat[i].text = string.Format("{0}/{1}", valueStat.ToString(), MasterDataStore.Instance.BaseDataVassalStatsExpIngames[levelExp].TotalExpLevelUp.ToString());
            int preExpDuration = valueStat - MasterDataStore.Instance.BaseDataVassalStatsExpIngames[levelExp - 1].TotalExpLevelUp;
            int durationExp = MasterDataStore.Instance.BaseDataVassalStatsExpIngames[levelExp - 1].Max_exp;
            float percentStat = (float)((float)preExpDuration / (float)durationExp);
            _imgProgressStat[i].fillAmount = percentStat;
            var itemStatName = _masterDataStore.BaseDataVassalStats.Find(t => t.Key == newStatList[i].Key);
            _tmpProgressValueStat_Name[i].text = itemStatName.StatName.ToString();
            _tmpProgressValueStat_Value[i].text = levelExp.ToString();
            _imgIconStatTop[i].sprite = _sprStatIcon[i];
        }
       
        _tmpPointJobClassLevel.text = newJobList[0].LvJobClass.ToString();
        _tmpProgressValueJobClassLevel.text = string.Format("{0}/{1}", newJobList[0].ExpJobClass, 10000);
        float percentExpJobClass = newJobList[0].ExpJobClass / 10000;
        _imgProgressValueStat_Loyalty.fillAmount = percentExpJobClass;
    }

    private void UpdateJobUI(VassalDataInGame vassal)
    {
        vassalDataInGame = vassal;
        for (int i = 0; i < ItemJobInformations.Count; i++)
        {
            if (_itemJobInformation[i] == null) break;
            ItemJobInformation itemJob = _itemJobInformation[i];
            int key = 0;
            JobData jobData = null;
            if (i == 0)
            {
                key = vassal.Data.Job_1;
                var job = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == vassal.Data.Job_1);
                if (job != null)
                    jobData = job;
            }
            if (i == 1)
            {
                key = vassal.Data.Job_2;
                var job = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == vassal.Data.Job_2);
                if (job != null)
                    jobData = job;
            }
            if (i == 2)
            {
                key = vassal.Data.Job_3;
                var job = GameData.Instance.SavedPack.SaveData.JobDatas.Find(t => t.Key == vassal.Data.Job_3);
                if (job != null)
                    jobData = job;
            }
            if (GameData.Instance.SavedPack.SaveData.VassalInfos.Count > 0)
            {
                int indexSprJob = 0;
                if (jobData != null)
                    indexSprJob = GameData.Instance.SavedPack.SaveData.JobDatas.FindIndex(t => t.Key == jobData.Key);
                if (indexSprJob >= spriteIconJob.Length) return;
                itemJob.InitData(spriteIconJob[indexSprJob], vassal, i, jobData, (a, b) =>
                {
                    OnDisableModel();
                    UIManagerTown.Instance.ShowUIVassarJobDetail(vassal);
                }, (c, d) =>
                {
                    OnDisableModel();
                    UIManagerTown.Instance.ShowUIVassarSelectJob(0, jobData, vassalDataInGame, (typeSelectJob, vassal, jobData) => { OnEventSelectJob(typeSelectJob, vassal, jobData); });
                });
            }

        }
    }

    public void OnEventSelectJob(TypeSelectJob typeSelectJob, VassalDataInGame vassal, JobData jobData)
    {
        List<int> jobList = new List<int>();
        if (vassal.Data.Job_1 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassal.Data.Job_1 != vassal.Data.Job_2 && vassal.Data.Job_1 != vassal.Data.Job_3) jobList.Add(vassal.Data.Job_1);
        if (vassal.Data.Job_2 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassal.Data.Job_2 != vassal.Data.Job_1 && vassal.Data.Job_2 != vassal.Data.Job_3) jobList.Add(vassal.Data.Job_2);
        if (vassal.Data.Job_3 != ShareUIManager.Instance.Config.DEFAUL_VALUE_OF_VASSAL_JOB && vassal.Data.Job_3 != vassal.Data.Job_1 && vassal.Data.Job_3 != vassal.Data.Job_2) jobList.Add(vassal.Data.Job_3);

        var vassalUpdate = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == vassal.Data.idVassalTemplate);

        if (typeSelectJob == TypeSelectJob.Register)
        {

            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.JobDatas.Count; i++)
            {
                var item = GameData.Instance.SavedPack.SaveData.JobDatas[i];
                if (item.Key == jobData.Key)
                {
                    if (vassalUpdate.Data.Job_1 == -1) vassalUpdate.Data.Job_1 = jobData.Key;
                    else if (vassalUpdate.Data.Job_2 == -1) vassalUpdate.Data.Job_2 = jobData.Key;
                    else if (vassalUpdate.Data.Job_3 == -1) vassalUpdate.Data.Job_3 = jobData.Key;
                }
            }
        }
        GameData.Instance.RequestSaveGame();

        _onUpdateJobVassal?.Raise(vassalUpdate);
    }

    private float _salaryBeginValue;
    public void OnBeginScrollFood()
    {
        _objCheckSalary.SetActive(false);
        _toggleSalary.isOn = false;
        _toggleSalary.gameObject.SetActive(true);
        _salaryBeginValue = _salaryScrollBar.value;
    }

    public void OnEndScrollFood()
    {
        float value = _salaryScrollBar.value;
        if (value > _salaryBeginValue)
        {
            _imgFillSalaryScroll.fillAmount = ValueScrollInCrease(value);
            _salaryScrollBar.value = ValueScrollInCrease(value);
        }
        else
        {
            _imgFillSalaryScroll.fillAmount = ValueScrollDeCrease(value);
            _salaryScrollBar.value = ValueScrollDeCrease(value);
        }

    }

    private float ValueScrollInCrease(float value)
    {
        float moodKey = 0;
        if (value > 0 && value < 0.167f) moodKey = 0.167f;
        else if (value > 0.167f && value < 0.334f) moodKey = 0.334f;
        else if (value > 0.334f && value < 0.501f) moodKey = 0.501f;
        else if (value > 0.501f && value < 0.668f) moodKey = 0.668f;
        else if (value > 0.668f && value < 0.835f) moodKey = 0.835f;
        else if (value > 0.835f) moodKey = 1;
        return moodKey;
    }

    private float ValueScrollDeCrease(float value)
    {
        float moodKey = 0;
        if (value > 0 && value < 0.167f) moodKey = 0f;
        else if (value > 0.167f && value < 0.334f) moodKey = 0.167f;
        else if (value > 0.334f && value < 0.501f) moodKey = 0.334f;
        else if (value > 0.501f && value < 0.668f) moodKey = 0.501f;
        else if (value > 0.668f && value < 0.835f) moodKey = 0.668f;
        else if (value > 0.835f) moodKey = 0.835f;
        return moodKey;
    }

    private int MoodKey(float value, float duration)
    {
        int moodKey = 0;
        if (value < (1 * duration)) moodKey = 1;
        else if (value < (2 * duration)) moodKey = 2;
        else if (value < (3 * duration)) moodKey = 3;
        else if (value < (4 * duration)) moodKey = 4;
        else if (value < (5 * duration)) moodKey = 5;
        else if (value < (6 * duration)) moodKey = 6;
        else moodKey = 7;
        return moodKey;
    }

    public void OnClickSalaryLevel(int index)
    {
        var defaultValue = _masterDataStore.BaseDataPopulationSalarys.Find(t => t.SalaryKey == 1);
        var salaryStep = _masterDataStore.BaseDataPopulationSalarySteps.Find(t => t.SalaryStepKey == index);
        if (salaryStep != null)
        {
            float defaultMinium = (float)_masterDataStore.BaseDataPopulationSalarySteps.Find(t => t.SalaryStepKey == 4).ChangeSalaryRate;
            float percent = (salaryStep.ChangeSalaryRate / defaultMinium) * 100;
            float valueSalary = (percent * CaculatorMinimum(defaultValue.DefaultValue)) / 100;
            Salary(valueSalary);
        }
    }

    public int CaculatorMinimum(int defaultValue)
    {
        var minimumWage = defaultValue * ShareUIManager.Instance.Config.PROSPERITY_COEFFICIENT * ShareUIManager.Instance.Config.ECONOMIC_FIGURE_CONEFFICIENT;
        return minimumWage;
    }

    private float _salary;
    void Salary(float salary)
    {
        _tmpSalary.text = salary.ToString();
        this._salary = salary;
        GameData.Instance.RequestSaveGame();
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnClickCloseHistory()
    {
        _panelHistoryLog.SetActive(false);
    }

    public void OnClickClose()
    {
        if(_tooltipOther.activeInHierarchy)
        {
            OnResetTooltip();
            OnDisableModelTooltip();
            _panelInfo.SetActive(true);
            _panelAvatar.SetActive(true);
            _panelCharacter.SetActive(true);
            OnDisableModel();

            for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
            {
                if (vassalDataInGame.Data.idVassalTemplate == GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data.idVassalTemplate)
                {
                    vassalModel[i].SetActive(true);
                    vassalAnimation[i].ActionReset(vassalAnimator[i]);
                    vassalAnimation[i].ActionIdle_99(vassalAnimator[i], () => { });
                }
            }
           
        }
        else
        {
            OnDisableModel();
            Close();
        }
    }

    private void OnDisableModel()
    {
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            vassalModel[i].SetActive(false);
        }
    }

    public void OnEnableModel()
    {
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            if (vassalDataInGame.Data.idVassalTemplate == GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data.idVassalTemplate)
                vassalModel[i].SetActive(true);
        }
    }

    private void OnDisableModelTooltip()
    {
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            vassalModel_tooltip[i].SetActive(false);
        }
    }

    public void OnClickDetail()
    {
        OnDisableModel();
        var vassal = GameData.Instance.SavedPack.SaveData.VassalInfos.Find(t => t.Data.idVassalTemplate == _baseVassalTemplateCurrent.Key);
        UIManagerTown.Instance.ShowUIVassarJobDetail(vassal);
    }

    public void OnClickHistory()
    {
        OnResetTooltip();
        _panelHistoryLog.SetActive(true);
        InitLog();
    }

    public void OnClickBack()
    {

    }

    public void OnClickItemTrousers()
    {

    }

    public void OnClickItemWeapon()
    {

    }

    public void OnClickItemChoker()
    {

    }

    public void OnClickItemGloves()
    {

    }

    public void OnClickItemShoes()
    {

    }

    public void OnClickItemShield()
    {

    }

    public void OnClickItemHat()
    {

    }

    public void OnClickItemArmor()
    {

    }

    public void OnClickTooltipMood()
    {
        OnResetTooltip();
        _tooltipMood.SetActive(true);
    }

    public void OnClickTooltipHealth()
    {
        OnResetTooltip();
        _tooltipHeath.SetActive(true);
    }

    public void OnClickTooltipStatus()
    {
        OnResetTooltip();
        _tooltipStatus.SetActive(true);
        _tmpTooltipStatusStatus.text = "Status: None";
        if (vassalDataInGame.IsWorking)
            _tmpTooltipStatusStatus.text = "Status: Working";
    }

    private void InitLog()
    {
        foreach (Transform child in _parentItemLog)
        {
            GameObject.Destroy(child.gameObject);
        }
        List<string> listItem = GameData.Instance.SavedPack.SaveData.ListPopulationLog;
        for (int i = 0; i < listItem.Count; i++)
        {
            GameObject item = Instantiate(_prefabItemLog, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentItemLog;
            ItemLog itemLog = item.GetComponent<ItemLog>();
            string[] time = listItem[i].Split('|');
            itemLog.InitData(time[0], time[1]);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        _parentItemLog.sizeDelta = new Vector2(_parentItemLog.sizeDelta.x, (50 * listItem.Count));
    }

    public void OnClickTooltipOther()
    {
        OnDisableModel();
        _panelInfo.SetActive(false);
        _panelAvatar.SetActive(false);
        _panelCharacter.SetActive(false);
        OnResetTooltip();
        _tooltipOther.SetActive(true);
        OnDisableModelTooltip();

        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.VassalInfos.Count; i++)
        {
            if (vassalDataInGame.Data.idVassalTemplate == GameData.Instance.SavedPack.SaveData.VassalInfos[i].Data.idVassalTemplate)
            {
                vassalModel_tooltip[i].SetActive(true);
            }
        }
    }

    public void OnClickTooltipJobOther()
    {
        OnResetTooltip();
        _tooltipOtherJob.SetActive(true);
    }

    public void OnClickBackground()
    {
        _tooltipMood.SetActive(false);
        _tooltipHeath.SetActive(false);
        _tooltipStatus.SetActive(false);
        _tooltipOtherJob.SetActive(false);
    }

    public void OnResetTooltip()
    {
        _tooltipMood.SetActive(false);
        _tooltipHeath.SetActive(false);
        _tooltipStatus.SetActive(false);
        _tooltipOther.SetActive(false);
        _tooltipOtherJob.SetActive(false);
    }

}
