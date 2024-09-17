using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataCore;
using TMPro;
using System;
using UnityEngine.UI;
using CoreData.UniFlow;
using ChartAndGraph;
using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;

public class UIPopulation : BasePanel
{
    private const string UPDATE_LOG_WORKER = "{0}:{1} {2} {3},{4} Uniflow| <color=#06c6eb>{5}</color> worker change";
    private const string UPDATE_LOG_MILITARY = "{0}:{1} {2} {3},{4} Uniflow| <color=#06c6eb>{5}</color> military change";

    private Action onComplete;
    private TypePanel _typePanelOpen = TypePanel.Population;

    [SerializeField]
    private Material[] __materialsList;

    [SerializeField]
    private GameObject _panelCurrentPopulation;

    [SerializeField]
    private TextMeshProUGUI _tmpValueFood;

    [SerializeField]
    private TextMeshProUGUI _tmpValueFoodMilitary;

    [SerializeField]
    private TextMeshProUGUI _tmpValueGold;

    [SerializeField]
    private TextMeshProUGUI _tmpValueGoldMilitary;

    [SerializeField]
    private TextMeshProUGUI _tmpPopulationRaise;

    [SerializeField]
    private TextMeshProUGUI _tmpPopulationUp;

    [SerializeField]
    private TextMeshProUGUI _tmpPopulationDown;

    [SerializeField]
    private GameObject _btnCurrentPopulationNormal;

    [SerializeField]
    private GameObject _btnCurrentPopulationChoice;

    private MasterDataStore _masterDataStore;
    //========== Log ==============
    [SerializeField]
    private GameObject _prefabItemLog;

    [SerializeField]
    private RectTransform _parentItemLog;

    [SerializeField]
    private GameObject _btnPopulationLogNormal;

    [SerializeField]
    private GameObject _btnPopulationLogChoice;

    [SerializeField]
    private GameObject _panelPopulationLog;

    //========== Worker ==============
    [SerializeField]
    private Scrollbar _workerScrollBar;

    [SerializeField]
    private TextMeshProUGUI _tmpprogressValueWorker;

    [SerializeField]
    private Image imgFillWorkScroll;

    [SerializeField]
    private Image imgFillNoWorkScroll;

    [SerializeField]
    private GameObject _btnWorkerInfoNormal;

    [SerializeField]
    private GameObject _btnWorkerInfoChoice;

    [SerializeField]
    private GameObject _panelWorkerInfo;

    [SerializeField]
    private RectTransform _parentWorkerInfo;

    [SerializeField]
    private GameObject _prefabItemWorkerAssign;

    [SerializeField]
    private RectTransform _parentItemWorkerAssign;

    [SerializeField]
    private RectTransform[] _positionAutoWorkerAssignment;

    [SerializeField]
    private RectTransform _imageAutoWorkerAssignment;

    [SerializeField]
    private RectTransform _imageBgAutoWorkerAssignment;

    [SerializeField]
    private TextMeshProUGUI _txtFemalePercent;

    [SerializeField]
    private TextMeshProUGUI _txtMalePercent;

    [SerializeField]
    private TextMeshProUGUI _txt15Percent;

    [SerializeField]
    private TextMeshProUGUI _txt40Percent;

    [SerializeField]
    private TextMeshProUGUI _txt60Percent;

    [SerializeField]
    private TextMeshProUGUI _txt90Percent;

    [SerializeField]
    private TextMeshProUGUI _txtWorkerPercent;

    [SerializeField]
    private TextMeshProUGUI _txtMilitaryPercent;

    [SerializeField]
    private ItemLabels _itemLabelAge;

    [SerializeField]
    private ItemLabels _itemLabelGender;

    [SerializeField]
    private ItemLabels _itemLabelCitizen;

    [SerializeField]
    private TextMeshProUGUI _txtOtherPercent;

    private float _areaNoWorker;
    private int _workerScroll;
    public int PriorityLastScroll;
    public float PercentLastScroll;
    private List<ItemWorkerAssignment> ItemWorkerAssignments = new List<ItemWorkerAssignment>();

    //========== Military ==============

    [SerializeField]
    private Image imgFillMilitaryScroll;

    [SerializeField]
    private Image imgFillNoMilitaryScroll;

    [SerializeField]
    private Scrollbar _militaryScrollBar;

    [SerializeField]
    private TextMeshProUGUI _tmpprogressValueMilitary;

    [SerializeField]
    private GameObject _prefabItemMilitaryAssign;

    [SerializeField]
    public RectTransform _parentItemMilitaryAssign;

    [SerializeField]
    private RectTransform _parentMilitaryInfo;

    private float _areaNoMilitary;
    private int _militaryScroll;
    private List<ItemMilitaryAssignment> ItemMilitaryAssignments = new List<ItemMilitaryAssignment>();

    //========== Chart ==============
    [SerializeField]
    private CanvasPieChart CanvasPieChartGender;

    [SerializeField]
    private CanvasPieChart CanvasPieChartAge;

    [SerializeField]
    private CanvasPieChart CanvasPieChartCitizen;


    //========== Event ==============

    [SerializeField]
    private FloatVariable _onUpdateGoldVariable;

    [SerializeField]
    private FloatVariable _onUpdateFoodVariable;
    
    [SerializeField]
    private GameEvent _onConfirmApplyWorker;

    [SerializeField]
    private GameEvent _onUpdateWorkerInProgress;


    //========== End ==============
    public enum TYPECITIZEN
    {
        Worker = 1,
        Military = 2
    }

    private void OnEnable()
    {
        _onUpdateFoodVariable.OnValueChange += OnUpdateFoodVariable;
        _onUpdateGoldVariable.OnValueChange += OnUpdateGoldVariable;
        _workerScrollBar.onValueChanged.AddListener((float val) => ScrollbarWorkerCallback(val));
        _militaryScrollBar.onValueChanged.AddListener((float val) => ScrollbarMilitaryCallback(val));
    }
    private void OnDisable()
    {
        _onUpdateFoodVariable.OnValueChange -= OnUpdateFoodVariable;
        _onUpdateGoldVariable.OnValueChange -= OnUpdateGoldVariable;
        //Un-Reigister to event
        _workerScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarWorkerCallback(val));
        _militaryScrollBar.onValueChanged.RemoveListener((float val) => ScrollbarMilitaryCallback(val));
    }

    private void SetNoWorkerArea()
    {
        float  workerAge = 0;
        for (int i = 0; i < _masterDataStore.GetPopulationAgeInfos().Count; i++)
        {
            if(_masterDataStore.GetPopulationAgeInfos()[i].Age > 14 && _masterDataStore.GetPopulationAgeInfos()[i].Age < 41)
            {
                float total = (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount; 
                workerAge += total;
            }
        }
        _areaNoWorker = 1 - ((workerAge) / GameData.Instance.SavedPack.SaveData.Citizen);
        imgFillNoWorkScroll.fillAmount = _areaNoWorker;
    }

    private void SetNoMilitaryArea()
    {
        float militaryAge = 0;
        for (int i = 0; i < _masterDataStore.GetPopulationAgeInfos().Count; i++)
        {
            if (_masterDataStore.GetPopulationAgeInfos()[i].Age > 14 && _masterDataStore.GetPopulationAgeInfos()[i].Age < 41)
            {
                float total = (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount;
                militaryAge += total;
            }
        }
        _areaNoMilitary = 1 - ((militaryAge) / GameData.Instance.SavedPack.SaveData.Citizen);
        imgFillNoMilitaryScroll.fillAmount = _areaNoMilitary;
    }

    private void OnUpdateFoodVariable(float value)
    {
        _tmpValueFood.text = value.ToString();
        _tmpValueFoodMilitary.text = value.ToString();
    }

    private void OnUpdateGoldVariable(float value)
    {
        _tmpValueGold.text = value.ToString();
        _tmpValueGoldMilitary.text = value.ToString();
    }

    void ScrollbarWorkerCallback(float newValue)
    {
        if (newValue > (1 - _areaNoWorker))
            newValue = (1 - _areaNoWorker);
        _workerScrollBar.value = newValue;
        imgFillWorkScroll.fillAmount = newValue;
        if (_tmpprogressValueWorker != null && _masterDataStore != null)
        {
            float workerNew = newValue * GameData.Instance.SavedPack.SaveData.Citizen;
            int worker = (int)(workerNew);
            _workerScroll = worker;
            _tmpprogressValueWorker.text = string.Format("<color=#00FF00>{0}</color>/{1}", worker, GameData.Instance.SavedPack.SaveData.Citizen);
        }
    }

    void ScrollbarMilitaryCallback(float newValue)
    {
        if (newValue > (1 - _areaNoMilitary))
            newValue = (1 - _areaNoMilitary);
        _militaryScrollBar.value = newValue;
        imgFillMilitaryScroll.fillAmount = newValue;
        if (_tmpprogressValueMilitary != null && _masterDataStore != null)
        {
            float militaryNew = newValue * GameData.Instance.SavedPack.SaveData.Citizen;
            int military = (int)(militaryNew);
            _militaryScroll = military;
            _tmpprogressValueMilitary.text = string.Format("<color=#00FF00>{0}</color>/{1}", military, GameData.Instance.SavedPack.SaveData.Citizen);
        }
    }

    public override void SetData(object[] data)
    {
        base.SetData(data);
        onComplete = (Action)data[0];
        _typePanelOpen = (TypePanel)data[1];
    }

    public enum TypePanel
    {
        Population = 1,
        WorkerInfo = 2,
        MilitaryInfo = 3,
        Log = 4
    }

    public override void Open()
    {
        base.Open();

        _masterDataStore = MasterDataStore.Instance;
        CanvasPieChartAge.gameObject.SetActive(true);
        CanvasPieChartGender.gameObject.SetActive(true);
        CanvasPieChartCitizen.gameObject.SetActive(true);
        if (_typePanelOpen == TypePanel.Population)
        {
            OnClickCurrentPopulation();
        }
        else if (_typePanelOpen == TypePanel.WorkerInfo)
        {
            OnClickWorkerInfo();
            OnClickChangeModeWorker();
        }
        else if (_typePanelOpen == TypePanel.MilitaryInfo)
        {
            OnClickWorkerInfo();
            OnClickChangeModeMilitary();
        }
        else
        {
            OnClickPopulationLog();
        }
        int workerCount = MasterDataStore.Instance.GetCitiZenTypeCount(MasterDataStore.CitiZenType.Worker);
        int militaryCount = MasterDataStore.Instance.GetCitiZenTypeCount(MasterDataStore.CitiZenType.Military);
        if (workerCount > 0)
            GameData.Instance.SavedPack.SaveData.WorkerNumber = workerCount;
        if (militaryCount > 0)
            GameData.Instance.SavedPack.SaveData.MilitaryNumber = militaryCount;

        GameData.Instance.SavedPack.SaveData.FillWorker = (float)GameData.Instance.SavedPack.SaveData.WorkerNumber / 1000;
        GameData.Instance.SavedPack.SaveData.FillMilitary = (float)GameData.Instance.SavedPack.SaveData.MilitaryNumber / 1000;
        _workerScroll = GameData.Instance.SavedPack.SaveData.WorkerNumber;
        _workerScrollBar.value = GameData.Instance.SavedPack.SaveData.FillWorker;
        imgFillWorkScroll.fillAmount = GameData.Instance.SavedPack.SaveData.FillWorker;

        _militaryScroll = GameData.Instance.SavedPack.SaveData.MilitaryNumber;
        _militaryScrollBar.value = GameData.Instance.SavedPack.SaveData.FillMilitary;
        imgFillMilitaryScroll.fillAmount = GameData.Instance.SavedPack.SaveData.FillMilitary;

        _tmpValueGold.text = GameData.Instance.SavedPack.SaveData.WorkerGoldSalary.ToString();
        _tmpValueFood.text = GameData.Instance.SavedPack.SaveData.WorkerFoodSalary.ToString();
        if (GameData.Instance.SavedPack.SaveData.WorkerGoldSalary == 0)
            OnClickGoodLevel(0, TYPECITIZEN.Worker);
        if (GameData.Instance.SavedPack.SaveData.WorkerFoodSalary == 0)
            OnClickFoodLevel(0, TYPECITIZEN.Worker);

        _tmpValueGoldMilitary.text = GameData.Instance.SavedPack.SaveData.MilitaryGoldSalary.ToString();
        _tmpValueFoodMilitary.text = GameData.Instance.SavedPack.SaveData.MilitaryFoodSalary.ToString();
        if (GameData.Instance.SavedPack.SaveData.MilitaryGoldSalary == 0)
            OnClickGoodLevel(0, TYPECITIZEN.Military);
        if (GameData.Instance.SavedPack.SaveData.MilitaryFoodSalary == 0)
            OnClickFoodLevel(0, TYPECITIZEN.Military);

        _tmpPopulationUp.text = GameData.Instance.SavedPack.SaveData.CitizenUp.ToString();
        _tmpPopulationDown.text = GameData.Instance.SavedPack.SaveData.CitizenDown.ToString();
      
        if (_tmpprogressValueWorker != null && _masterDataStore != null)
        {
            _tmpprogressValueWorker.text = string.Format("<color=#01D7FE>{0}</color>/{1}", GameData.Instance.SavedPack.SaveData.WorkerNumber, GameData.Instance.SavedPack.SaveData.Citizen);
            _tmpPopulationRaise.text = "(0)";

        }
        if (_tmpprogressValueMilitary != null && _masterDataStore != null)
        {
            _tmpprogressValueMilitary.text = string.Format("<color=#01D7FE>{0}</color>/{1}", GameData.Instance.SavedPack.SaveData.MilitaryNumber, GameData.Instance.SavedPack.SaveData.Citizen);
        }
        itemDefaulPercent.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            var item = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i];
            itemDefaulPercent.Add(new ItemPercentDefaul(new WorkerAssignment(i, item.Priority, item.Percent, item.DataAssignments), i));
        }

        SetNoWorkerArea();
        SetNoMilitaryArea();

        OnBeginScroll(0, 1);
        // Init chart
        InitChart();

        // Init Item Log
        _ = APIManager.Instance.RequestLogChangeWorker();
        _ = APIManager.Instance.RequestPopulationChangeDetail();
        InitLog();

        InitMilitaryAssignment();
        InitWorkerAssignment();

        ResetItemLabel();
    }

    public void OnClickFoodLevel(int index, TYPECITIZEN type)
    {
        var defaultValue = _masterDataStore.BaseDataPopulationSalarys.Find(t => t.SalaryKey == 1);
        var salaryStep = _masterDataStore.BaseDataPopulationSalarySteps[index];
        if (salaryStep != null)
        {
            float defaultMinium = (float)_masterDataStore.BaseDataPopulationSalarySteps.Find(t => t.SalaryStepKey == 4).ChangeSalaryRate;
            float percent = (salaryStep.ChangeSalaryRate / defaultMinium) * 100;
            float valueSalary = (percent * CaculatorMinimum(defaultValue.DefaultValue)) / 100;
            FoodSalary(valueSalary,type);
        }
    }

    public void OnClickGoodLevel(int index, TYPECITIZEN type)
    {
        var defaultValue = _masterDataStore.BaseDataPopulationSalarys.Find(t => t.SalaryKey == 2);
        var salaryStep = _masterDataStore.BaseDataPopulationSalarySteps[index];
        if (salaryStep != null)
        {
            float defaultMinium = (float)_masterDataStore.BaseDataPopulationSalarySteps.Find(t => t.SalaryStepKey == 4).ChangeSalaryRate;
            float percent = (salaryStep.ChangeSalaryRate / defaultMinium) * 100;
            float valueSalary = (percent * CaculatorMinimum(defaultValue.DefaultValue)) / 100;
            GoldSalary(valueSalary,type);
        }
    }

    void FoodSalary(float salary, TYPECITIZEN type)
    {
        if (type == TYPECITIZEN.Worker)
            _tmpValueFood.text = salary.ToString();
        else
            _tmpValueFoodMilitary.text = salary.ToString();
        GameData.Instance.RequestSaveGame();
    }

    void GoldSalary(float salary, TYPECITIZEN type)
    {
        if (type == TYPECITIZEN.Worker)
            _tmpValueGold.text = salary.ToString();
        else
            _tmpValueGoldMilitary.text = salary.ToString();
        GameData.Instance.RequestSaveGame();
    }

    public int CaculatorMinimum(int defaultValue)
    {
        var minimumWage = defaultValue * ShareUIManager.Instance.Config.PROSPERITY_COEFFICIENT * ShareUIManager.Instance.Config.ECONOMIC_FIGURE_CONEFFICIENT;
        return minimumWage;
    }

    private void InitChart()
    {
        float female = 0, male = 0, children = 0, Young = 0, middleAge = 0, Age = 0 , worker = 0, military = 0,citizen = 0;
        for (int i = 0; i < _masterDataStore.GetPopulationAgeInfos().Count; i++)
        {
            female += (float)_masterDataStore.GetPopulationAgeInfos()[i].FemaleCount;
            male += (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount;

            if (_masterDataStore.GetPopulationAgeInfos()[i].Age < 14)
            {
                float total = (float)_masterDataStore.GetPopulationAgeInfos()[i].FemaleCount + (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount; 
                children += total;
            }
            else if(_masterDataStore.GetPopulationAgeInfos()[i].Age > 14 && _masterDataStore.GetPopulationAgeInfos()[i].Age < 41)
            {
                float total = (float)_masterDataStore.GetPopulationAgeInfos()[i].FemaleCount + (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount; 
                Young += total;
            }
            else if (_masterDataStore.GetPopulationAgeInfos()[i].Age > 40 && _masterDataStore.GetPopulationAgeInfos()[i].Age < 61)
            {
                float total = (float)_masterDataStore.GetPopulationAgeInfos()[i].FemaleCount + (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount; 
                middleAge += total;
            }
            else if (_masterDataStore.GetPopulationAgeInfos()[i].Age > 61)
            {
                float total = (float)_masterDataStore.GetPopulationAgeInfos()[i].FemaleCount + (float)_masterDataStore.GetPopulationAgeInfos()[i].MaleCount; 
                Age += total;
            }
        }

        for (int i = 0; i < _masterDataStore.BaseDataPopulationTemplates.Count; i++)
        {
            if (_masterDataStore.BaseDataPopulationTemplates[i].CitizenJob == 1)
                citizen = (float)_masterDataStore.BaseDataPopulationTemplates[i].TotalRate;
            else if (_masterDataStore.BaseDataPopulationTemplates[i].CitizenJob == 2)
                worker = (float)_masterDataStore.BaseDataPopulationTemplates[i].TotalRate;
            else if (_masterDataStore.BaseDataPopulationTemplates[i].CitizenJob == 3)
                military = (float)_masterDataStore.BaseDataPopulationTemplates[i].TotalRate;

        }
        float totalGender = male + female;
        float totalAge = children + Young + middleAge + Age;

        if (!CanvasPieChartAge.DataSource.HasCategory("15"))
            CanvasPieChartAge.DataSource.AddCategory("15", __materialsList[0]);
        if (!CanvasPieChartAge.DataSource.HasCategory("40"))
            CanvasPieChartAge.DataSource.AddCategory("40", __materialsList[1]);
        if (!CanvasPieChartAge.DataSource.HasCategory("60"))
            CanvasPieChartAge.DataSource.AddCategory("60", __materialsList[2]);
        if (!CanvasPieChartAge.DataSource.HasCategory("90"))
            CanvasPieChartAge.DataSource.AddCategory("90", __materialsList[3]);
        CanvasPieChartAge.DataSource.SetValue("15", (children / totalAge) * 100);
        CanvasPieChartAge.DataSource.SetValue("40", (Young / totalAge) * 100);
        CanvasPieChartAge.DataSource.SetValue("60", (middleAge / totalAge) * 100);
        CanvasPieChartAge.DataSource.SetValue("90", ((totalAge - (children + middleAge + Young)) / totalAge) * 100);
        CanvasPieChartAge.DataSource.SetCateogryParams("15", 0.923f, 1, 0);
        CanvasPieChartAge.DataSource.SetCateogryParams("60", 0.923f, 1, 0);

        if (!CanvasPieChartGender.DataSource.HasCategory("Male"))
            CanvasPieChartGender.DataSource.AddCategory("Male", __materialsList[0]);
        if (!CanvasPieChartGender.DataSource.HasCategory("FeMale"))
            CanvasPieChartGender.DataSource.AddCategory("FeMale", __materialsList[1]);
        CanvasPieChartGender.DataSource.SetValue("Male", (male / totalGender) * 100);
        CanvasPieChartGender.DataSource.SetValue("FeMale", (female / totalGender) * 100);

        if (!CanvasPieChartCitizen.DataSource.HasCategory("Worker"))
            CanvasPieChartCitizen.DataSource.AddCategory("Worker", __materialsList[0]);
        if (!CanvasPieChartCitizen.DataSource.HasCategory("Military"))
            CanvasPieChartCitizen.DataSource.AddCategory("Military", __materialsList[1]);
        if (!CanvasPieChartCitizen.DataSource.HasCategory("Other"))
            CanvasPieChartCitizen.DataSource.AddCategory("Other", __materialsList[2]);
        CanvasPieChartCitizen.DataSource.SetValue("Worker", (worker / citizen) * 100);
        CanvasPieChartCitizen.DataSource.SetValue("Military", (military / citizen) * 100);
        CanvasPieChartCitizen.DataSource.SetValue("Other", ((citizen - (worker + military)) / citizen) * 100);
        CanvasPieChartCitizen.DataSource.SetCateogryParams("Other", 0.98f, 1, 0);

        int femalePercent = (int)((female / totalGender) * 100);
        _txtFemalePercent.text = string.Format("{0}<color=#304370>%</color>", femalePercent);
        _txtMalePercent.text = string.Format("{0}<color=#304370>%</color>", (100 - femalePercent));

        int Percent_15 = (int)((children / totalAge) * 100);
        int Percent_40 = (int)((Young / totalAge) * 100);
        int Percent_60 = (int)((middleAge / totalAge) * 100);
        _txt15Percent.text = string.Format("{0}<color=#304370>%</color>", Percent_15);
        _txt40Percent.text = string.Format("{0}<color=#304370>%</color>", Percent_40);
        _txt60Percent.text = string.Format("{0}<color=#304370>%</color>", Percent_60);
        _txt90Percent.text = string.Format("{0}<color=#304370>%</color>", 100 - (Percent_15 + Percent_40 + Percent_60));

        int percent_worker = (int)((worker / citizen) * 100);
        int percent_military = (int)((military / citizen) * 100);
        _txtWorkerPercent.text = string.Format("{0}<color=#304370>%</color>", percent_worker);
        _txtMilitaryPercent.text = string.Format("{0}<color=#304370>%</color>", percent_military);
        _txtOtherPercent.text = string.Format("{0}<color=#304370>%</color>", 100 - (percent_worker + percent_military));
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
            itemLog.InitData(time[0] , time[1]);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
        _parentItemLog.sizeDelta = new Vector2(_parentItemLog.sizeDelta.x, (50 * listItem.Count));
    }

    public void InitWorkerAssignment()
    {
        foreach (Transform child in _parentItemWorkerAssign)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemWorkerAssignments.Clear();
        List<WorkerAssignment> listItem = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments;
        for (int  i = 0;i< listItem.Count; i++)
        {
            GameObject item = Instantiate(_prefabItemWorkerAssign, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentItemWorkerAssign;
            ItemWorkerAssignment itemWorkerAssignment = item.GetComponent<ItemWorkerAssignment>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            int worker = (int)((listItem[i].Percent * GameData.Instance.SavedPack.SaveData.WorkerNumber) / 100);
            itemWorkerAssignment.InitData(this, i, worker, listItem[i], () => { });
            ItemWorkerAssignments.Add(itemWorkerAssignment);
        }
    }

    public void InitMilitaryAssignment()
    {
        foreach (Transform child in _parentItemMilitaryAssign)
        {
            GameObject.Destroy(child.gameObject);
        }
        ItemMilitaryAssignments.Clear();
        List<MilitaryAssignment> listItem = GameData.Instance.SavedPack.SaveData.ListMilitaryAssignments;
        for (int i = 0; i < listItem.Count; i++)
        {
            GameObject item = Instantiate(_prefabItemMilitaryAssign, new Vector3(0, 0, 0), Quaternion.identity);
            item.transform.parent = _parentItemMilitaryAssign;
            ItemMilitaryAssignment itemMilitaryAssignment = item.GetComponent<ItemMilitaryAssignment>();
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            int military = (int)(listItem[i].Percent);
            itemMilitaryAssignment.InitData(this, i, military, listItem[i], () => { });
            ItemMilitaryAssignments.Add(itemMilitaryAssignment);
        }
    }


    public override void Close()
    {
        base.Close();
    }

    public void OnClickCurrentPopulation()
    {

        ResetPanel();
        _btnCurrentPopulationChoice.gameObject.SetActive(true);
        _btnWorkerInfoNormal.gameObject.SetActive(true);
        _btnPopulationLogNormal.gameObject.SetActive(true);
        _panelCurrentPopulation.SetActive(true);

        itemDefaulPercent.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            var item = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i];
            itemDefaulPercent.Add(new ItemPercentDefaul(new WorkerAssignment(i, item.Priority, item.Percent, item.DataAssignments), i));
        }

        SetNoWorkerArea();
        SetNoMilitaryArea();

        OnBeginScroll(0, 1);
        InitMilitaryAssignment();
        InitWorkerAssignment();

    }

    public void OnClickWorkerInfo()
    {
        ResetPanel();
        _btnWorkerInfoChoice.gameObject.SetActive(true);
        _btnPopulationLogNormal.gameObject.SetActive(true);
        _btnCurrentPopulationNormal.gameObject.SetActive(true);
        _panelWorkerInfo.SetActive(true);
        OnClickChangeModeWorker();
    }

    public void OnClickPopulationLog()
    {
        ResetPanel();
        _btnPopulationLogChoice.gameObject.SetActive(true);
        _btnWorkerInfoNormal.gameObject.SetActive(true);
        _btnCurrentPopulationNormal.gameObject.SetActive(true);
        _panelPopulationLog.SetActive(true);
        InitLog();

        itemDefaulPercent.Clear();
        for (int i = 0; i < GameData.Instance.SavedPack.SaveData.ListWorkerAssignments.Count; i++)
        {
            var item = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i];
            itemDefaulPercent.Add(new ItemPercentDefaul(new WorkerAssignment(i, item.Priority, item.Percent, item.DataAssignments), i));
        }

        SetNoWorkerArea();
        SetNoMilitaryArea();

        OnBeginScroll(0, 1);
        InitMilitaryAssignment();
        InitWorkerAssignment();
    }

    private void ResetPanel()
    {
        _panelWorkerInfo.SetActive(false);
        _panelPopulationLog.SetActive(false);
        _panelCurrentPopulation.SetActive(false);
        _parentMilitaryInfo.gameObject.SetActive(false);

        _btnWorkerInfoNormal.gameObject.SetActive(false);
        _btnWorkerInfoChoice.gameObject.SetActive(false);
        _btnPopulationLogChoice.gameObject.SetActive(false);
        _btnPopulationLogNormal.gameObject.SetActive(false);
        _btnCurrentPopulationNormal.gameObject.SetActive(false);
        _btnCurrentPopulationChoice.gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        CanvasPieChartAge.gameObject.SetActive(false);
        CanvasPieChartGender.gameObject.SetActive(false);
        CanvasPieChartCitizen.gameObject.SetActive(false);
        onComplete?.Invoke();
        Close();
    }

    public void OnClickDetail()
    {
        //UIManager.Instance.ShowUIPopulationDetail();
        UIManagerTown.Instance.ShowUINotification();
    }

    public void OnClickChangeWorker()
    {
        int workerBefore = GameData.Instance.SavedPack.SaveData.WorkerNumber;
        GameData.Instance.SavedPack.SaveData.WorkerNumber = _workerScroll;
        GameData.Instance.SavedPack.SaveData.FillWorker = (float)(GameData.Instance.SavedPack.SaveData.WorkerNumber) / (float)1000;

        UpdateWorkerAssign();

        float priority_1 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[0].Percent;
        float priority_2 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[1].Percent;
        float priority_3 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[2].Percent;
        float priority_4 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[3].Percent;
        _ = APIManager.Instance.RequestNumberWorker(_workerScroll, 1000, priority_1, priority_2, priority_3,
            priority_4);
        GameData.Instance.RequestSaveGame();
        if (_tmpprogressValueMilitary != null && _masterDataStore != null)
        {
            _tmpprogressValueWorker.text = string.Format("<color=#01D7FE>{0}</color>/{1}", GameData.Instance.SavedPack.SaveData.WorkerNumber, GameData.Instance.SavedPack.SaveData.Citizen);
        }
        string timeTxt = string.Format(UPDATE_LOG_WORKER, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Hour.ToString("00"),
              GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Minutes.ToString("00"),
              GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day.ToString(),
               GameTimer.Instance.SwicthNameMonth(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month),
               ((GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year) + 1).ToString(), GameData.Instance.SavedPack.SaveData.WorkerNumber.ToString());
        GameData.Instance.SavedPack.SaveData.ListPopulationLog.Add(timeTxt);
    }

    public void OnApplyChange()
    {
        int workerBefore = GameData.Instance.SavedPack.SaveData.WorkerNumber;
        GameData.Instance.SavedPack.SaveData.WorkerNumber = _workerScroll;
        GameData.Instance.SavedPack.SaveData.FillWorker = (float)(GameData.Instance.SavedPack.SaveData.WorkerNumber) / (float)1000;

        UpdateWorkerAssign();
        float priority_1 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[0].Percent;
        float priority_2 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[1].Percent;
        float priority_3 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[2].Percent;
        float priority_4 = GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[3].Percent;
        _ = APIManager.Instance.RequestNumberWorker(_workerScroll, 1000, priority_1, priority_2, priority_3,
            priority_4);
        GameData.Instance.RequestSaveGame();
        onComplete?.Invoke();
        Close();
    }

    public void OnClickChangeMilitary()
    {
        int militaryBefore = GameData.Instance.SavedPack.SaveData.MilitaryNumber;
        GameData.Instance.SavedPack.SaveData.MilitaryNumber = _militaryScroll;
        GameData.Instance.SavedPack.SaveData.FillMilitary = (float)(GameData.Instance.SavedPack.SaveData.MilitaryNumber) / (float)1000;
        if (_tmpprogressValueMilitary != null && _masterDataStore != null)
        {
            _tmpprogressValueMilitary.text = string.Format("<color=#01D7FE>{0}</color>/{1}", GameData.Instance.SavedPack.SaveData.MilitaryNumber, GameData.Instance.SavedPack.SaveData.Citizen);
        }
        _ = APIManager.Instance.RequestNumberMilitary(_militaryScroll, 1000, 25, 25, 25, 25);
        string timeTxt = string.Format(UPDATE_LOG_MILITARY, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Hour.ToString("00"),
      GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Minutes.ToString("00"),
      GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day.ToString(),
       GameTimer.Instance.SwicthNameMonth(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month),
       ((GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year) + 1).ToString(), GameData.Instance.SavedPack.SaveData.MilitaryNumber.ToString());
        GameData.Instance.SavedPack.SaveData.ListPopulationLog.Add(timeTxt);
        GameData.Instance.RequestSaveGame();
    }

    public void OnClickConfirmMilitary()
    {
        int militaryBefore = GameData.Instance.SavedPack.SaveData.MilitaryNumber;
        GameData.Instance.SavedPack.SaveData.MilitaryNumber = _militaryScroll;
        GameData.Instance.SavedPack.SaveData.FillMilitary = (float)(GameData.Instance.SavedPack.SaveData.MilitaryNumber) / (float)1000;

        _ = APIManager.Instance.RequestNumberMilitary(_militaryScroll, 1000, 25, 25, 25, 25);
        GameData.Instance.RequestSaveGame();
        onComplete?.Invoke();
        Close();
        string timeTxt = string.Format(UPDATE_LOG_MILITARY, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Hour.ToString("00"),
GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Minutes.ToString("00"),
GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day.ToString(),
GameTimer.Instance.SwicthNameMonth(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month),
((GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year) + 1).ToString(), GameData.Instance.SavedPack.SaveData.MilitaryNumber.ToString());
        GameData.Instance.SavedPack.SaveData.ListPopulationLog.Add(timeTxt);
    }

    private void UpdateWorkerAssign()
    {
        List<WorkerAssignment> listItem = new List<WorkerAssignment>();
        for (int i = 0; i < itemDefaulPercent.Count; i++)
        {
            listItem.Add(itemDefaulPercent[i].WorkerAssignment);
        }
        for (int i = 0; i < listItem.Count; i++)
        {
            int worker = (int)((listItem[i].Percent * GameData.Instance.SavedPack.SaveData.WorkerNumber) / 100);
            ItemWorkerAssignments[i].InitData(this, i, worker, listItem[i], () => { });
        }
    }

    public void OnClickConfirm()
    {
        for (int i = 0; i < itemDefaulPercent.Count; i++)
        {
            GameData.Instance.SavedPack.SaveData.ListWorkerAssignments[i] = itemDefaulPercent[i].WorkerAssignment;
        }
        _onConfirmApplyWorker?.Raise();

        //CalculatorPercent();
        GameData.Instance.RequestSaveGame();
        OnApplyChange();
        _onUpdateWorkerInProgress?.Raise();
        string timeTxt = string.Format(UPDATE_LOG_WORKER, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Hour.ToString("00"),
GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Minutes.ToString("00"),
GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day.ToString(),
GameTimer.Instance.SwicthNameMonth(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month),
((GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year) + 1).ToString(), GameData.Instance.SavedPack.SaveData.WorkerNumber.ToString());
        GameData.Instance.SavedPack.SaveData.ListPopulationLog.Add(timeTxt);

    }

    private List<ItemTemp> indexCount = new List<ItemTemp>();
    public List<ItemPercentDefaul> itemDefaulPercent = new List<ItemPercentDefaul>();
    private float totalPercentCurrent = 0;
    public void OnBeginScroll(int index, int priority)
    {
        indexCount.Clear();
        for (int i = 0; i < itemDefaulPercent.Count; i++)
        {
            indexCount.Add(new ItemTemp(itemDefaulPercent[i].WorkerAssignment, false, i));
        }
        totalPercentCurrent = 100;
    }

    public void OnEndScroll(int index, int priority)
    {
        for(int  i = 0; i< indexCount.Count;i++)
        {
            ItemWorkerAssignments[i].OnUpdateScrollWorkerAssignment(indexCount[i].WorkerAssignment);
        }
        indexCount.Clear();
        totalPercentCurrent = 100;
    }

    public void CalculatorPercent(int index,int priority , float percent)
    {
        int totalPercent = (int) (totalPercentCurrent + percent), percentOver = 0, percentMiss = 0;
   
        for (int i = 0; i < itemDefaulPercent.Count; i++)
        {
            if (itemDefaulPercent[i].WorkerAssignment.Priority == priority)
                totalPercent -= (int)itemDefaulPercent[i].WorkerAssignment.Percent;
        }
        if (totalPercent > 100)
        {
            indexCount[index].IsSelect = true;
            percentOver = (int )totalPercent - 100;
            CountUp(percentOver, priority, index, percent);
        }
        else
        {
            indexCount[index].IsSelect = true;
            percentMiss = 100 - totalPercent;
            CountDown(percentMiss, priority, index, percent);
        }
    }

    private void CountUp(int percentOver , int priority , int index, float percent)
    {
        indexCount[index].WorkerAssignment.Percent = percent;
        itemDefaulPercent[index].WorkerAssignment.Percent = percent;
        for (int i = indexCount.Count - 1; i >= 0; i--)
        {
    
            if (indexCount[i].WorkerAssignment.Priority != priority && percentOver > 0 && !indexCount[i].IsSelect)
            {
                percentOver--;
                indexCount[i].IsSelect = true;
                var listCount = indexCount.FindAll(t => t.WorkerAssignment.Priority != priority);
                ItemTemp max = listCount[0];
                int indexMax = 0;
                for (int t = 1; t < listCount.Count; t++)
                {
                    if (listCount[t].WorkerAssignment.Percent > max.WorkerAssignment.Percent)
                    {
                        max = listCount[t];
                        indexMax = t;
                    }
                }
                if (max.WorkerAssignment.Priority != priority)
                    indexCount[max.Index].WorkerAssignment.Percent--;
                else
                    indexCount[i].WorkerAssignment.Percent--;

            }
            if (i < ItemWorkerAssignments.Count && i < indexCount.Count)
            {
                ItemWorkerAssignments[i].OnUpdateScrollWorkerAssignment(indexCount[i].WorkerAssignment);
                itemDefaulPercent[i].WorkerAssignment.Percent = indexCount[i].WorkerAssignment.Percent;
            }
        }
        var items = indexCount.Find(t => t.IsSelect == false);
        if (items == null)
        {
            for (int j = 0; j < indexCount.Count; j++)
            {
                if (indexCount[j].WorkerAssignment.Priority != priority)
                    indexCount[j].IsSelect = false;
            }
        }
        for (int i = 0; i < indexCount.Count; i++)
        {
            if (i < ItemWorkerAssignments.Count)
                ItemWorkerAssignments[i].OnUpdateScrollWorkerAssignment(indexCount[i].WorkerAssignment);
        }
        if (percentOver > 0)
            CountUp(percentOver, priority, index, percent);
    }

    private void CountDown(float percentMiss, int priority, int index, float percent)
    {
        indexCount[index].WorkerAssignment.Percent = percent;
        itemDefaulPercent[index].WorkerAssignment.Percent = percent;
        for (int i = 0; i< indexCount.Count;i++)
        {
            if (indexCount[i].WorkerAssignment.Priority != priority && percentMiss > 0 && !indexCount[i].IsSelect)
            {
                indexCount[i].IsSelect = true;
                percentMiss--;
                var listCount = indexCount.FindAll(t => t.WorkerAssignment.Priority != priority);
                ItemTemp min = listCount[0];
                int indexMax = 0;
                for (int t = 1; t < listCount.Count; t++)
                {
                    if (min.WorkerAssignment.Percent > listCount[t].WorkerAssignment.Percent)
                    {
                        min = listCount[t];
                        indexMax = t;
                    }
                }
                if (min.WorkerAssignment.Priority == priority)
                    indexCount[i].WorkerAssignment.Percent++;
                else
                    indexCount[min.Index].WorkerAssignment.Percent++;
            }
            if(i < ItemWorkerAssignments.Count && i < indexCount.Count)
            {
                ItemWorkerAssignments[i].OnUpdateScrollWorkerAssignment(indexCount[i].WorkerAssignment);
                itemDefaulPercent[i].WorkerAssignment.Percent = indexCount[i].WorkerAssignment.Percent;
            }
        }
        var items = indexCount.Find(t => t.IsSelect == false);
        if (items == null)
        {
            for (int j = 0; j < indexCount.Count; j++)
            {
                if (indexCount[j].WorkerAssignment.Priority != priority)
                    indexCount[j].IsSelect = false;
            }
        }
        for (int i = 0; i < indexCount.Count; i++)
        {
            if (i < ItemWorkerAssignments.Count)
                ItemWorkerAssignments[i].OnUpdateScrollWorkerAssignment(indexCount[i].WorkerAssignment);
        }
        if (percentMiss > 0)
            CountDown(percentMiss, priority, index, percent);
    }

    public void OnClickAutoWorkerAssignment()
    {
        //bool IsAuto = GameData.Instance.SavedPack.SaveData.IsAutoWorkerAssignment;
        //if(IsAuto)
        //{
        //    _imageBgAutoWorkerAssignment.gameObject.SetActive(false);
        //    _imageAutoWorkerAssignment.transform.localPosition = _positionAutoWorkerAssignment[0].localPosition;
        //}
        //else
        //{
        //    _imageBgAutoWorkerAssignment.gameObject.SetActive(true);
        //    _imageAutoWorkerAssignment.transform.localPosition = _positionAutoWorkerAssignment[1].localPosition;
        //}
        //GameData.Instance.SavedPack.SaveData.IsAutoWorkerAssignment = !IsAuto;
        //GameData.Instance.RequestSaveGame();
        UIManagerTown.Instance.ShowUINotification();
    }

    public void OnClickNoti()
    {
        UIManagerTown.Instance.ShowUINotification();
    }

    public void OnClickDecrease()
    {
        float value = _workerScrollBar.value;
        if (value <= 0) return;
        value -= 0.001f;
        _workerScrollBar.value = value;
        ScrollbarWorkerCallback(value);
    }

    public void OnClickIncrease()
    {
        float value = _workerScrollBar.value;
        if (value >= 1000) return;
        value += 0.001f;
        _workerScrollBar.value = value;
        ScrollbarWorkerCallback(value);
    }

    public void OnClickDecreaseMilitary()
    {
        float value = _militaryScrollBar.value;
        if (value <= 0) return;
        value -= 0.001f;
        _militaryScrollBar.value = value;
        ScrollbarMilitaryCallback(value);
    }

    public void OnClickIncreaseMilitary()
    {
        float value = _militaryScrollBar.value;
        if (value >= 1000) return;
        value += 0.001f;
        _militaryScrollBar.value = value;
        ScrollbarMilitaryCallback(value);
    }

    public void OnClickChangeSalary()
    {
        //UIManagerTown.Instance.ShowUIWorkerHouse();
        UIManagerTown.Instance.ShowUIWorkerHouseEdit();
    }

    public void OnClickChangeSalaryMilitary()
    {
        UIManagerTown.Instance.ShowUIMilitaryHouse();
    }
    public void OnClickChangeModeWorker()
    {
        _parentMilitaryInfo.gameObject.SetActive(false);
        _parentWorkerInfo.gameObject.SetActive(true);

        _workerScroll = GameData.Instance.SavedPack.SaveData.WorkerNumber;
        _workerScrollBar.value = GameData.Instance.SavedPack.SaveData.FillWorker;
        imgFillWorkScroll.fillAmount = GameData.Instance.SavedPack.SaveData.FillWorker;
        _tmpValueGold.text = GameData.Instance.SavedPack.SaveData.WorkerGoldSalary.ToString();
        _tmpValueFood.text = GameData.Instance.SavedPack.SaveData.WorkerFoodSalary.ToString();
        if (GameData.Instance.SavedPack.SaveData.WorkerGoldSalary == 0)
            OnClickGoodLevel(0, TYPECITIZEN.Worker);
        if (GameData.Instance.SavedPack.SaveData.WorkerFoodSalary == 0)
            OnClickFoodLevel(0, TYPECITIZEN.Worker);

        if (_tmpprogressValueWorker != null && _masterDataStore != null)
        {
            _tmpprogressValueWorker.text = string.Format("<color=#01D7FE>{0}</color>/{1}", GameData.Instance.SavedPack.SaveData.WorkerNumber, GameData.Instance.SavedPack.SaveData.Citizen);
            _tmpPopulationRaise.text = "(0)";

        }
    }

    public void OnClickChangeModeMilitary()
    {
        _parentMilitaryInfo.gameObject.SetActive(true);
        _parentWorkerInfo.gameObject.SetActive(false);
        _militaryScroll = GameData.Instance.SavedPack.SaveData.MilitaryNumber;
        _militaryScrollBar.value = GameData.Instance.SavedPack.SaveData.FillMilitary;
        imgFillMilitaryScroll.fillAmount = GameData.Instance.SavedPack.SaveData.FillMilitary;
        _tmpValueGoldMilitary.text = GameData.Instance.SavedPack.SaveData.MilitaryGoldSalary.ToString();
        _tmpValueFoodMilitary.text = GameData.Instance.SavedPack.SaveData.MilitaryFoodSalary.ToString();

        if (GameData.Instance.SavedPack.SaveData.MilitaryGoldSalary == 0)
            OnClickGoodLevel(0, TYPECITIZEN.Military);
        if (GameData.Instance.SavedPack.SaveData.MilitaryFoodSalary == 0)
            OnClickFoodLevel(0, TYPECITIZEN.Military);

        if (_tmpprogressValueMilitary != null && _masterDataStore != null)
        {
            _tmpprogressValueMilitary.text = string.Format("<color=#01D7FE>{0}</color>/{1}", GameData.Instance.SavedPack.SaveData.MilitaryNumber, GameData.Instance.SavedPack.SaveData.Citizen);
        }
    }

    public void ResetItemLabel()
    {
        _itemLabelAge.enabled = false;
        _itemLabelCitizen.enabled = false;
        _itemLabelGender.enabled = false;
    }

    public async void OnClickPieAge()
    {
        _itemLabelAge.enabled = true;
        await UniTask.DelayFrame(1000);
        _itemLabelAge.enabled = false;
    }

    public async void OnClickPieGender()
    {
        _itemLabelGender.enabled = true;
        await UniTask.DelayFrame(1000);
        _itemLabelGender.enabled = false;
    }

    public async void OnClickPieCitizen()
    {
        _itemLabelCitizen.enabled = true;
        await UniTask.DelayFrame(1000);
        _itemLabelCitizen.enabled = false;
    }
}

[Serializable]
public class ItemTemp
{
    public int Index;
    public WorkerAssignment WorkerAssignment;
    public bool IsSelect;
    public ItemTemp(WorkerAssignment key, bool isSelect, int index)
    {
        this.Index = index;
        this.WorkerAssignment = key;
        this.IsSelect = isSelect;
    }
}

[Serializable]
public class ItemPercentDefaul
{
    public int Index;
    public WorkerAssignment WorkerAssignment;
    public ItemPercentDefaul(WorkerAssignment key,int index)
    {
        this.Index = index;
        this.WorkerAssignment = key;
    }
}