using System;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using CoreData.UniFlow;
using Fbs;

public class UIGameplayController : BasePanel
{
    private const string LOG_POPULATION_UP = "{0}:{1} {2} {3},{4} Uniflow| <color=#06c6eb>{5}</color> increase due to immigration";
    private const string LOG_POPULATION_DOWN = "{0}:{1} {2} {3},{4} Uniflow| <color=#c94e58>{5}</color> deaths due to epidemic";

    private MasterDataStore _masterDataStore;

    [SerializeField]
    private GameEvent _onUpdateNumberWorker;

    [SerializeField]
    private GameEvent _onOpenMilitaryHouse;

    [SerializeField]
    private GameEvent _onUpdatePopulationTotal;

    [SerializeField]
    private GameEvent _onUpdateNaturalDeath;

    [SerializeField]
    private GameTimer _gameTimer;

    [SerializeField] private TextMeshProUGUI txtPopulationNumber;

    private void OnEnable()
    {
        _onUpdateNumberWorker.Subcribe(OnUpdateNumberWorker);
        _onUpdatePopulationTotal.Subcribe(OnIncreasePopulation);
        _onUpdateNaturalDeath.Subcribe(OnDecreasePopulation);
        _onOpenMilitaryHouse.Subcribe(OnOpenMilitaryHouse);
    }

    private void OnDisable()
    {
        _onUpdateNumberWorker.Unsubcribe(OnUpdateNumberWorker);
        _onUpdatePopulationTotal.Unsubcribe(OnIncreasePopulation);
        _onUpdateNaturalDeath.Unsubcribe(OnDecreasePopulation);
        _onOpenMilitaryHouse.Unsubcribe(OnOpenMilitaryHouse);
    }

    private void OnUpdateNumberWorker(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        ResponseNumberWorker reponse = (ResponseNumberWorker)eventParam[0];
        txtPopulationNumber.text = reponse.Worker.ToString();
    }

    public override void Init()
    {
        _masterDataStore = MasterDataStore.Instance;
    }

    public override void Open()
    {
        base.Open();
        if (GameData.Instance == null) return;
       
        if (txtPopulationNumber != null && _masterDataStore != null && GameData.Instance.SavedPack.SaveData.Citizen == 0)
        {
            for (int i = 0; i < _masterDataStore.BaseConsts.Count; i++)
            {
                if (_masterDataStore.BaseConsts[i].NameConst == "POPULATION_START_VALUE")
                    GameData.Instance.SavedPack.SaveData.Citizen = (int)_masterDataStore.BaseConsts[i].Value;
            }

        }
        int workerCount = MasterDataStore.Instance.GetCitiZenTypeCount(MasterDataStore.CitiZenType.Worker);
        if (workerCount > 0 && GameData.Instance.SavedPack.SaveData.WorkerNumber == 0)
            GameData.Instance.SavedPack.SaveData.WorkerNumber = workerCount;

        if (GameData.Instance.SavedPack.SaveData.WorkerNumber == 0)
            txtPopulationNumber.text = "0";
        else
            txtPopulationNumber.text = GameData.Instance.SavedPack.SaveData.WorkerNumber.ToString();

        _gameTimer.enabled = true;
    }

    private void OnIncreasePopulation(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        int count = (int)eventParam[0];
        TimeData timeData = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent;
        var log = string.Format(LOG_POPULATION_UP, timeData.Hour.ToString("00"), timeData.Minutes.ToString("00"),
            timeData.Day, GameTimer.Instance.SwicthNameMonth(timeData.Month), timeData.Year + 1, count);
        GameData.Instance.SavedPack.SaveData.ListPopulationLog.Add(log);
        GameData.Instance.SavedPack.SaveData.CitizenUp = count;
        GameData.Instance.SavedPack.SaveData.Citizen += count;
        GameData.Instance.RequestSaveGame();
    }

    private void OnDecreasePopulation(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

        int count = (int)eventParam[0];
        TimeData timeData = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent;
        var log = string.Format(LOG_POPULATION_DOWN, timeData.Hour.ToString("00"), timeData.Minutes.ToString("00"),
            timeData.Day, GameTimer.Instance.SwicthNameMonth(timeData.Month), timeData.Year + 1, count);
        GameData.Instance.SavedPack.SaveData.ListPopulationLog.Add(log);
        GameData.Instance.SavedPack.SaveData.CitizenDown = count;
        GameData.Instance.SavedPack.SaveData.Citizen -= count;
        GameData.Instance.RequestSaveGame();
    }

    private void OnOpenMilitaryHouse(object[] eventParam)
    {
        UIManagerTown.Instance.ShowUIMilitaryHouse();
    }

    public bool IsSpecialResolution()
    {
        return ((float)Screen.height / (float)Screen.width) >= 1.8f;
    }

    public override void Close()
    {
        base.Close();
    }

    public override void OnCloseManual()
    {
        base.OnCloseManual();
    }

    public void ShowUIPopulation()
    {
        UIManagerTown.Instance.ShowUIPopulation();
    }

    public void ShowUIVassalInformation()
    {
        UIManagerTown.Instance.ShowUIVassarInformation();
    }

    public void ShowUIStatusBoard()
    {
        UIManagerTown.Instance.ShowUIStatusBoard();
    }

    public void ShowUI()
    {
        
    }

    public void HideUI()
    {
       
    }

    public void OnButtonBackPress()
    {
    }
}
