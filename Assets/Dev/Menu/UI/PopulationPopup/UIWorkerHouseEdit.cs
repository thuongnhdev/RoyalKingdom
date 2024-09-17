using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreData.UniFlow;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;

public class UIWorkerHouseEdit : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private TextMeshProUGUI tmpWorkload;

    [SerializeField]
    private TextMeshProUGUI tmpTotal;

    [SerializeField]
    private TextMeshProUGUI tmpWorking;

    [SerializeField]
    private TextMeshProUGUI tmpRest;

    [SerializeField]
    private TextMeshProUGUI tmpGold;

    [SerializeField]
    private TextMeshProUGUI tmpGoldHour;

    [SerializeField]
    private TextMeshProUGUI tmpNormal;
    [SerializeField]
    private TextMeshProUGUI tmpNormalTile;
    [SerializeField]
    private TextMeshProUGUI tmpNormalRaise;

    [SerializeField]
    private TextMeshProUGUI tmpSteadyWorkload;

    [SerializeField]
    private GameObject panelTime;

    [SerializeField]
    private TextMeshProUGUI tmpTime;

    [SerializeField]
    private GameObject panelConfirm;

    [SerializeField]
    private Image imgFillGoldScroll;

    [SerializeField]
    private Slider _goldScrollBar;

    [SerializeField]
    private GameObject fireWorkerPopup;

    [SerializeField]
    private Slider fireWorkersSlider;

    [SerializeField]
    private TextMeshProUGUI tmpFireWorkers;

    [SerializeField]
    private GameObject TrainingUI;



    [SerializeField]
    private FloatVariable _onUpdateGoldVariable; 

    [SerializeField]
    private UserItemStorage _userItemStorage;

    private MasterDataStore _masterDataStore;

    [SerializeField]
    private TextMeshProUGUI tmpLastPaymentSalary;
    
    [SerializeField]
    private TextMeshProUGUI tmpNextPaymentSalary;


    private float _goldSalary;
    private float _workLoad;


    [SerializeField]
    private GameEvent _onUpdateNumberWorker;

    [Header("Tooltip")]
    
    [SerializeField]
    private GameObject _tooltipMood;

    [Header("WorkerMood")]
    [SerializeField]
    private TextMeshProUGUI _tmpWorkerMoodTooltip;

    [SerializeField]
    private TextMeshProUGUI _tmpNextPaymentTooltip;


    [Header("WorkerEffect")]
    [SerializeField]
    private TextMeshProUGUI _tmpWorkingSpeedTooltip;
    
    [SerializeField]
    private TextMeshProUGUI _tmpGeneralStrikeTooltip;


    private void OnEnable()
    {
        _onUpdateNumberWorker.Subcribe(OnUpdateNumberWorker);
        _goldScrollBar.onValueChanged.AddListener((float val) => ScrollbarGoldCallback(val));
    }
    private void OnDisable()
    {
        _onUpdateNumberWorker.Unsubcribe(OnUpdateNumberWorker);
        //Un-Reigister to event
        _goldScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarGoldCallback(val));
    }

    private float _goldBeginValue;

    public void OnBeginScrollGold()
    {
        _goldBeginValue = _goldScrollBar.value;
        float keyMood = ValueScrollInMedium(_goldBeginValue);
        _goldScrollBar.value = keyMood;
    }

    public void OnEndScrollGold()
    {
        float valueGold = _goldScrollBar.value;
        float keyMood = ValueScrollInMedium(valueGold);
        _goldScrollBar.value = keyMood;
    }

    private async void OnUpdateNumberWorker(object[] eventParam)
    {
        await UniTask.DelayFrame(4);
        int WorkersWorking = 0;
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListTask.Count; i++)
        {
            WorkersWorking += GameData.Instance.SavedPack.SaveData.ListTask[i].Man;
        }
        GameData.Instance.SavedPack.SaveData.WorkersWorking = WorkersWorking;
        GameData.Instance.RequestSaveGame();
        tmpWorkload.text = GameData.Instance.SavedPack.SaveData.WorkloadWorkerHouse.ToString();
        tmpTotal.text = GameData.Instance.SavedPack.SaveData.WorkerNumber.ToString();
        tmpWorking.text = GameData.Instance.SavedPack.SaveData.WorkersWorking.ToString();
        tmpRest.text = (GameData.Instance.SavedPack.SaveData.WorkerNumber - GameData.Instance.SavedPack.SaveData.WorkersWorking).ToString();
    }

    public override void SetData(object[] data)
    {
        base.SetData(data);
        previousPanel = (BasePanel)data[0];
    }

    public override void Open()
    {
        base.Open();
        float  gold = 0;
        _masterDataStore = MasterDataStore.Instance;
        gold = GameData.Instance.SavedPack.SaveData.GoldTotal;
        if (GameData.Instance.SavedPack.SaveData.GoldTotal == 0)
        {
            gold = _userItemStorage.GetItemCount(990099);
            GameData.Instance.SavedPack.SaveData.GoldTotal = gold;
        }

        tmpGold.text = gold.ToString();

        int WorkersWorking = 0;
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListTask.Count; i++)
        {
            WorkersWorking += GameData.Instance.SavedPack.SaveData.ListTask[i].Man;
        }
        GameData.Instance.SavedPack.SaveData.WorkersWorking = WorkersWorking;
        GameData.Instance.RequestSaveGame();
        tmpWorkload.text = GameData.Instance.SavedPack.SaveData.WorkloadWorkerHouse.ToString();
        tmpTotal.text = GameData.Instance.SavedPack.SaveData.WorkerNumber.ToString();
        tmpWorking.text = GameData.Instance.SavedPack.SaveData.WorkersWorking.ToString();
        tmpRest.text = (GameData.Instance.SavedPack.SaveData.WorkerNumber - GameData.Instance.SavedPack.SaveData.WorkersWorking).ToString();

        _goldScrollBar.value = GameData.Instance.SavedPack.SaveData.WorkerFillGoldBar;
        imgFillGoldScroll.fillAmount = GameData.Instance.SavedPack.SaveData.WorkerFillGoldBar;
        _goldBeginValue = _goldScrollBar.value;

        int goldGroupValue = (int)GameData.Instance.SavedPack.SaveData.WorkerGoldSalary;
        //int lastPaymentVaue = (int)GameData.Instance.SavedPack.SaveData.WorkerLastPaymentSalary;
        //int nextPaymentVaue = (int)GameData.Instance.SavedPack.SaveData.WorkerNextPaymentSalary;

        tmpGoldHour.text = goldGroupValue.ToString();
        this._workLoad = goldGroupValue;

        // Calculate last payment salary
        int lastPaymentRange = (int)GameData.Instance.SavedPack.SaveData.WorkerLastPaymentSalary;
        int lastPaymentTotalWorkers = (int)GameData.Instance.SavedPack.SaveData.WorkersWorking;
        float lastPaymentSalary = CalculateLastPaymentSalary(lastPaymentRange, lastPaymentTotalWorkers);
        tmpLastPaymentSalary.text = lastPaymentSalary.ToString();

        // Calculate next payment salary
        int WorkerCount = (int)GameData.Instance.SavedPack.SaveData.WorkerNumber;
        int TraineeWorkerCount = (int)GameData.Instance.SavedPack.SaveData.FillWorker;
        float previewSalary = this._goldSalary;
        float nextPaymentSalary = CalculateNextPaymentSalary(WorkerCount, TraineeWorkerCount, previewSalary);
        tmpNextPaymentSalary.text = nextPaymentSalary.ToString();

        ScrollbarGoldCallback(_goldScrollBar.value);

        panelConfirm.SetActive(false);
        panelTime.SetActive(true);

        int dayOfMonth = 30;
        if (DateTime.Now.Month == 2)
            dayOfMonth = 28;
        long ticks = new DateTime(DateTime.Now.Year, DateTime.Now.Month, dayOfMonth, 0, 0, 0,
new CultureInfo("en-US", false).Calendar).Ticks;
        _endTime = new DateTime(ticks);
        CountDownTime();
    }

    void ScrollbarGoldCallback(float salary)
    {
        float valueGold = salary;
        float keyMood = ValueScrollInMedium(valueGold);
        _goldScrollBar.value = keyMood;

        imgFillGoldScroll.fillAmount = keyMood;
        int step = MoodKey(keyMood, 0.167f);
        OnClickGoodLevel(step);

    }

    void GoldSalary(float salary)
    {
        // Save last payment salary
        tmpGoldHour.text = salary.ToString();
        this._goldSalary = salary;
        GameData.Instance.SavedPack.SaveData.WorkerGoldSalary = this._goldSalary;
        
        this._workLoad = this._goldSalary;
        panelTime.SetActive(false);
        panelConfirm.SetActive(true);
        MoodString(MoodKeyAverage( this._goldSalary));
        GameData.Instance.RequestSaveGame();
    }

    private float CalculateLastPaymentSalary(int lastPaymentSalaryRange, int lastPaymentTotalWorkers)
    {
        float lastPaymentSalary = lastPaymentSalaryRange * lastPaymentTotalWorkers;
        return lastPaymentSalary;
    }

    private float CalculateNextPaymentSalary(int currentWorkerCount, int TraineeWorkerCount, float previewSalary)
    {
        float totalWorkerCount = currentWorkerCount + TraineeWorkerCount;
        float nextPaymentSalary = totalWorkerCount * previewSalary;
        return nextPaymentSalary;
    }

    private void FixedUpdate()
    {
        _countUpdate++;
        if (_countUpdate == 50)
        {
            _countUpdate = 0;
            CountDownTime();
        }
    }

    private DateTime _endTime;
    private int _countUpdate = 0;
    private const string REFRESH_REAL_TIMER = "{0} {1}:{2}:{3}";
    private void CountDownTime()
    {
        DateTime startTime = DateTime.Now;
        TimeSpan span = _endTime.Subtract(startTime);
        tmpTime.text = string.Format(REFRESH_REAL_TIMER, span.Days.ToString("00"), span.Hours.ToString("00"), span.Minutes.ToString("00"), span.Seconds.ToString("00"));
    }

    private int MoodKey(float value, float duration)
    {
        int moodKey = 0;
        if (value == (1 * duration)) moodKey = 1;
        else if (value == (2 * duration)) moodKey = 2;
        else if (value == (3 * duration)) moodKey = 3;
        else if (value == (4 * duration)) moodKey = 4;
        else if (value == (5 * duration)) moodKey = 5;
        else if (value == 1) moodKey = 6;
        return moodKey;
    }

    private float ValueScrollInMedium(float value)
    {
        float moodKey = 0;
        if (value > 0 && value < 0.075f) moodKey = 0;
        else if (value > 0.075f && value < 0.25f) moodKey = 0.167f;
        else if (value > 0.25f && value < 0.42f) moodKey = 0.334f;
        else if (value > 0.42f && value < 0.588f) moodKey = 0.501f;
        else if (value > 0.588f && value < 0.76f) moodKey = 0.668f;
        else if (value > 0.76f && value < 0.925f) moodKey = 0.835f;
        else if (value > 0.925f) moodKey = 1;
        return moodKey;
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

    private float MoodKeyAverage(float gold)
    {
        float average = gold;
        return average;
    }

    private void MoodString(float average)
    {
        string stepName = "";
        //average = average * 3; // temp 
        //average = _nextPaymentSalary - _goldSalary;
        float levelValue = 0, remainder = 0, moodKey = 0;
        foreach (var item in _masterDataStore.BaseDataMoodTemplates)
        {
            if (average >= item.MinValue && average <= item.MaxValue)
            {
                levelValue = (float)item.MinValue;
                remainder = (int)(average - levelValue);
                //remainder = average;
                moodKey = (int)item.MoodKey;
                stepName = item.StepName;
                break;
            }
        }

        int  WorkersWorking = 0;
        GameData.Instance.SavedPack.SaveData.WorkersWorking = WorkersWorking;

        float generalStrike = this._workLoad / WorkersWorking;

        tmpNormal.text = stepName;
        tmpNormalTile.text = "(" + levelValue.ToString() + ")";
        tmpNormalRaise.text = remainder.ToString();

        _tmpWorkerMoodTooltip.text = levelValue + ":" + stepName;
        _tmpNextPaymentTooltip.text = "+" + remainder + ")";
        _tmpWorkingSpeedTooltip.text = GameData.Instance.SavedPack.SaveData.WorkloadWorkerHouse.ToString();
        _tmpGeneralStrikeTooltip.text = generalStrike + "%";

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

    //public void OnClickPopulation()
    //{
    //    UIManagerTown.Instance.ShowUIPopulation();
    //    Close();
    //}

    public void OnClickConfirm()
    {
        GameData.Instance.SavedPack.SaveData.WorkerGoldSalary = this._goldSalary;
        GameData.Instance.SavedPack.SaveData.WorkerFillGoldBar = _goldScrollBar.value;

        if (GameData.Instance.SavedPack.SaveData.WorkerGoldSalaryInMonth == 0)
            GameData.Instance.SavedPack.SaveData.WorkerGoldSalaryInMonth = _goldSalary;

        _ = APIManager.Instance.RequestSalary(this._goldSalary, 0, 0, 0, this._workLoad, 0);
        GameData.Instance.saveGameData();

        _onUpdateGoldVariable.Value = this._goldSalary;
        Close();
    }

    public void OnCickFireWorkers()
    {
        fireWorkersSlider.maxValue = GameData.Instance.SavedPack.SaveData.WorkersWorking;
        int fireWorkers = (int)fireWorkersSlider.value;
        tmpFireWorkers.text = "Discard " + fireWorkers + "/"+ GameData.Instance.SavedPack.SaveData.WorkersWorking + " workers?"; 
        fireWorkerPopup.SetActive(true);
        GameData.Instance.saveGameData();
    }    

    public void ConfirmFireWorkers()
    {
        GameData.Instance.SavedPack.SaveData.WorkerFillFire = fireWorkersSlider.value;

        fireWorkerPopup.SetActive(false);

    }

    public void CloseFireWorkers()
    {
        fireWorkerPopup.SetActive(false);
    }

    public void OnClickNoti()
    {
        UIManagerTown.Instance.ShowUINotification();
    }

    public int CaculatorMinimum(int defaultValue)
    {
        var minimumWage = defaultValue * ShareUIManager.Instance.Config.PROSPERITY_COEFFICIENT * ShareUIManager.Instance.Config.ECONOMIC_FIGURE_CONEFFICIENT;
        return minimumWage;
    }

    public void OnClickGoodLevel(int index)
    {
        var defaultValue = _masterDataStore.BaseDataPopulationSalarys.Find(t => t.SalaryKey == 2);
        var salaryStep = _masterDataStore.BaseDataPopulationSalarySteps[index];
        if (salaryStep != null)
        {
            float defaultMinium = (float)_masterDataStore.BaseDataPopulationSalarySteps.Find(t => t.SalaryStepKey == 4).ChangeSalaryRate;
            float percent = (salaryStep.ChangeSalaryRate / defaultMinium) * 100;
            float valueSalary = (percent * CaculatorMinimum(defaultValue.DefaultValue)) / 100;
            GoldSalary(valueSalary);
        }
    }

    public void OnClickTooltipMood()
    {
        _tooltipMood.SetActive(true);
    }

}