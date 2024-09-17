using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CoreData.UniFlow;
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

public class UIMilitaryHouse : BasePanel
{
    private BasePanel previousPanel;

    [SerializeField]
    private TextMeshProUGUI tmpInfantry;

    [SerializeField]
    private TextMeshProUGUI tmpArchers;

    [SerializeField]
    private TextMeshProUGUI tmpCavalry;

    [SerializeField]
    private TextMeshProUGUI tmpGold;

    [SerializeField]
    private TextMeshProUGUI tmpFood;

    [SerializeField]
    private TextMeshProUGUI tmpGoldHour;

    [SerializeField]
    private TextMeshProUGUI tmpFoodHour;

    [SerializeField]
    private TextMeshProUGUI tmpNormal;
    [SerializeField]
    private TextMeshProUGUI tmpNormalTile;
    [SerializeField]
    private TextMeshProUGUI tmpNormalRaise;


    [SerializeField]
    private GameObject panelTime;

    [SerializeField]
    private TextMeshProUGUI tmpTime;

    [SerializeField]
    private GameObject panelConfirm;

    [SerializeField]
    private Image imgFillFoodScroll;

    [SerializeField]
    private Slider _foodScrollBar;

    [SerializeField]
    private Image imgFillGoldScroll;

    [SerializeField]
    private Slider _goldScrollBar;

    [SerializeField]
    private FloatVariable _onUpdateGoldVariable;

    [SerializeField]
    private FloatVariable _onUpdateFoodVariable;

    [SerializeField]
    private UserItemStorage _userItemStorage;

    private MasterDataStore _masterDataStore;

    private float _foodSalary;
    private float _goldSalary;
    private void OnEnable()
    {
        _foodScrollBar.onValueChanged.AddListener((float val) => ScrollbarFoodCallback(val));
        _goldScrollBar.onValueChanged.AddListener((float val) => ScrollbarGoldCallback(val));
    }
    private void OnDisable()
    {
        //Un-Reigister to event
        _foodScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarFoodCallback(val));
        _goldScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarGoldCallback(val));
    }

    public override void SetData(object[] data)
    {
        base.SetData(data);
        previousPanel = (BasePanel)data[0];
    }

    public override void Open()
    {
        base.Open();
        float food, gold = 0;
        _masterDataStore = MasterDataStore.Instance;
        food = GameData.Instance.SavedPack.SaveData.FoodTotal;
        gold = GameData.Instance.SavedPack.SaveData.GoldTotal;
        if (GameData.Instance.SavedPack.SaveData.GoldTotal == 0)
        {
            gold = _userItemStorage.GetItemCount(990099);
            GameData.Instance.SavedPack.SaveData.GoldTotal = gold;
        }
        if (GameData.Instance.SavedPack.SaveData.FoodTotal == 0)
        {
            food = _userItemStorage.GetItemCount(100001);
            GameData.Instance.SavedPack.SaveData.FoodTotal = food;
        }

        tmpGold.text = gold.ToString();
        tmpFood.text = food.ToString();

        int militaryCount = (int)GameData.Instance.SavedPack.SaveData.MilitaryNumber / 3;
        tmpInfantry.text = militaryCount.ToString();
        tmpArchers.text = militaryCount.ToString();
        tmpCavalry.text = militaryCount.ToString();

        _foodScrollBar.value = GameData.Instance.SavedPack.SaveData.MilitaryFillFoodBar;
        _goldScrollBar.value = GameData.Instance.SavedPack.SaveData.MilitaryFillGoldBar;
        imgFillFoodScroll.fillAmount = GameData.Instance.SavedPack.SaveData.MilitaryFillFoodBar;
        imgFillGoldScroll.fillAmount = GameData.Instance.SavedPack.SaveData.MilitaryFillGoldBar;
        _foodBeginValue = _foodScrollBar.value;
        _goldBeginValue = _goldScrollBar.value;

        int foodGroupValue = (int)GameData.Instance.SavedPack.SaveData.WorkerFoodSalary;
        int goldGroupValue = (int)GameData.Instance.SavedPack.SaveData.WorkerGoldSalary;
        tmpFoodHour.text = foodGroupValue.ToString();
        tmpGoldHour.text = goldGroupValue.ToString();

        ScrollbarFoodCallback(_foodScrollBar.value);
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
    void ScrollbarFoodCallback(float salary)
    {
        float valueFood = salary;
        float keyMood = ValueScrollInMedium(valueFood);
        _foodScrollBar.value = keyMood;
        imgFillFoodScroll.fillAmount = keyMood;
        int step = MoodKey(keyMood, 0.167f);
        OnClickFoodLevel(step);
    }
    private float _foodBeginValue;
    public void OnBeginScrollFood()
    {
        _foodBeginValue = _foodScrollBar.value;
        float keyMood = ValueScrollInMedium(_foodBeginValue);
        _foodScrollBar.value = keyMood;
    }

    public void OnEndScrollFood()
    {
        float value = _foodScrollBar.value;
        float keyMood = ValueScrollInMedium(value);
        _foodScrollBar.value = keyMood;
    }

    private float _goldBeginValue;
    void ScrollbarGoldCallback(float salary)
    {
        float valueGold = salary;
        float keyMood = ValueScrollInMedium(valueGold);
        _goldScrollBar.value = keyMood;
        imgFillGoldScroll.fillAmount = keyMood;
        int step = MoodKey(keyMood, 0.167f);
        OnClickGoodLevel(step);
    }
   
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
    void FoodSalary(float salary)
    {
        tmpFoodHour.text = salary.ToString();
        this._foodSalary = salary;
        //_onUpdateFoodVariable.Value = this._foodSalary;

        panelTime.SetActive(false);
        panelConfirm.SetActive(true);
        MoodString(MoodKeyAverage(this._foodSalary, this._goldSalary));
        GameData.Instance.RequestSaveGame();
    }

    void GoldSalary(float salary)
    {
        tmpGoldHour.text = salary.ToString();
        this._goldSalary = salary;
        //_onUpdateGoldVariable.Value = this._goldSalary;

        panelTime.SetActive(false);
        panelConfirm.SetActive(true);
        MoodString(MoodKeyAverage(this._foodSalary, this._goldSalary));
        GameData.Instance.RequestSaveGame();
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

    private float MoodKeyAverage(float food, float gold)
    {
        float average = (float)((food + gold) / 2);
        return average;
    }
    private void MoodString(float average)
    {
        string stepName = "";
        average = average * 3; // temp 
        float levelValue = 0, remainder = 0, moodKey = 0;
        foreach (var item in _masterDataStore.BaseDataMoodTemplates)
        {
            if (average >= item.MinValue && average <= item.MaxValue)
            {
                levelValue = (float)item.MinValue;
                remainder = (int)(average - levelValue);
                moodKey = (int)item.MoodKey;
                stepName = item.StepName;
                break;
            }
        }

        tmpNormal.text = stepName;
        tmpNormalTile.text = "(" + levelValue.ToString() + ")";
        tmpNormalRaise.text = "(" + remainder + ")";
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

    public void OnClickPopulation()
    {
        UIManagerTown.Instance.ShowUIPopulation();
        Close();
    }

    public void OnClickOpenBand()
    {
        UIManagerTown.Instance.ShowUITroop(()=> { });
        Close();
    }

    public void OnClickConfirm()
    {
        GameData.Instance.SavedPack.SaveData.MilitaryGoldSalary = this._goldSalary;
        GameData.Instance.SavedPack.SaveData.MilitaryFoodSalary = this._foodSalary;

        GameData.Instance.SavedPack.SaveData.MilitaryFillFoodBar = _foodScrollBar.value;
        GameData.Instance.SavedPack.SaveData.MilitaryFillGoldBar = _goldScrollBar.value;

        if (GameData.Instance.SavedPack.SaveData.MilitaryGoldSalaryInMonth == 0)
            GameData.Instance.SavedPack.SaveData.MilitaryGoldSalaryInMonth = _goldSalary;

        if (GameData.Instance.SavedPack.SaveData.MilitaryFoodSalaryInMonth == 0)
            GameData.Instance.SavedPack.SaveData.MilitaryFoodSalaryInMonth = _foodSalary;

        _ = APIManager.Instance.RequestSalary(this._goldSalary, this._foodSalary, 0, 0, 0, 0);
        GameData.Instance.saveGameData();
        _onUpdateFoodVariable.Value = this._foodSalary;
        _onUpdateGoldVariable.Value = this._goldSalary;
        Close();
    }

    public void OnClickAutoGold()
    {

    }

    public void OnClickAutoFood()
    {

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

    public void OnClickFoodLevel(int index)
    {
        var defaultValue = _masterDataStore.BaseDataPopulationSalarys.Find(t => t.SalaryKey == 1);
        var salaryStep = _masterDataStore.BaseDataPopulationSalarySteps[index];
        if (salaryStep != null)
        {
            float defaultMinium = (float)_masterDataStore.BaseDataPopulationSalarySteps.Find(t => t.SalaryStepKey == 4).ChangeSalaryRate;
            float percent = (salaryStep.ChangeSalaryRate / defaultMinium) * 100;
            float valueSalary = (percent * CaculatorMinimum(defaultValue.DefaultValue)) / 100;
            FoodSalary(valueSalary);
        }
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

}
