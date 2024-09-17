using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using CoreData.UniFlow;
using CoreData.UniFlow.Common;

[System.Serializable]
public class PlayerSavePack
{
    public long VERSION;

    public PlayerSaveData SaveData;

    public PlayerSavePack()
    {
        VERSION = 1;
        SaveData = new PlayerSaveData();
}


}

[System.Serializable]
public class PlayerSaveData
{
    #region worker and population
    public float WorkerGoldSalary;
    public float WorkerFoodSalary;
    public float WorkerGoldSalaryInMonth;
    public float WorkerFoodSalaryInMonth;

    public float WorkerLastPaymentSalary;
    public float WorkerNextPaymentSalary;

    public float MilitaryGoldSalary;
    public float MilitaryFoodSalary;
    public float MilitaryGoldSalaryInMonth;
    public float MilitaryFoodSalaryInMonth;

    public int WorkerNumber;
    public int MilitaryNumber;
    public float FillWorker;
    public float FillMilitary;

    public float WorkerFillGoldBar;
    public float WorkerFillFoodBar;
    public float WorkerFillFire;

    public float MilitaryFillGoldBar;
    public float MilitaryFillFoodBar;

    public float GoldTotal;
    public float FoodTotal;
    public float PerWorkload;
    public int WorkloadWorkerHouse;
    public int RestWorkerHouse;
    public int Citizen;
    public int CitizenUp;
    public int CitizenDown;
    public float TimeWorkloadOneWorker;
    public float TimeWorkloadOneVassal;
    #endregion

    #region
    public string SeasonTimer;
    public float SpeedGameTimer;
    public float Ratio;
    public TimerData TimerData;
    #endregion

    public bool IsAutoWorkerAssignment;
    public int WorkersWorking;

    #region
    public FlagData FlagData;
    public List<JobData> JobDatas;
    public List<TaskData> ListTask;
    public List<VassalDataInGame> VassalInfos;
    public List<string> ListPopulationLog;
    public List<BuildingMain> BuildingMains;
    public List<BaseDataEnumGame> ListDataEnums;
    public List<WorkerAssignment> ListWorkerAssignments;
    public List<MilitaryAssignment> ListMilitaryAssignments;
    public List<BattleStatisticsData> BattleStatisticsDataList;
    #endregion

    #region military
    public List<BaseBandDefaut> BandDatas;
    public List<TroopData> TroopDatas;
    public List<BandItemData> BandItemDatas;
    public List<WareHouseItemData> WareHouseItemDatas;
    #endregion

    public PlayerSaveData()
    {
        Citizen = 0;
        WorkerGoldSalary = 0;
        WorkerFoodSalary = 0;
        WorkerGoldSalaryInMonth = 0;
        WorkerFoodSalaryInMonth = 0;
        WorkerLastPaymentSalary = 0;
        WorkerNextPaymentSalary = 0;

        MilitaryGoldSalary = 0;
        MilitaryFoodSalary = 0;
        MilitaryGoldSalaryInMonth = 0;
        MilitaryFoodSalaryInMonth = 0;

        WorkerNumber = 0;
        MilitaryNumber = 0;
        WorkersWorking = 0;
        FillWorker = 0;
        FillMilitary = 0;
        WorkerFillFoodBar = 0;
        WorkerFillGoldBar = 0;
        MilitaryFillFoodBar = 0;
        MilitaryFillGoldBar = 0;
        PerWorkload = 0;
        GoldTotal = 0;
        FoodTotal = 0;
        CitizenUp = 1000;
        CitizenDown = 0;
        WorkloadWorkerHouse = 0;
        RestWorkerHouse = 0;
        SeasonTimer = SeasonType.NORMAL;
        SpeedGameTimer = 0;
        TimeWorkloadOneWorker = 0;
        TimeWorkloadOneVassal = 0;
        TimerData = new TimerData();
        JobDatas = new List<JobData>();
        ListTask = new List<TaskData>();
        ListPopulationLog = new List<string>();
        BuildingMains = new List<BuildingMain>();
        VassalInfos = new List<VassalDataInGame>();
        ListDataEnums = new List<BaseDataEnumGame>();
        ListWorkerAssignments = new List<WorkerAssignment>();
        ListMilitaryAssignments = new List<MilitaryAssignment>();
        BattleStatisticsDataList = new List<BattleStatisticsData>();

        BandItemDatas = new List<BandItemData>();
        WareHouseItemDatas = new List<WareHouseItemData>();
        BandDatas = new List<BaseBandDefaut>();
        TroopDatas = new List<TroopData>();

        IsAutoWorkerAssignment = false;

        FlagData = new FlagData(0, -1, -1, 0, -1, 0, -1, 0);

        for(var i = 0; i< 4;i++)
        {
            ListWorkerAssignments.Add(new WorkerAssignment(i, i + 1, 25, null));
            ListMilitaryAssignments.Add(new MilitaryAssignment(i, i + 1, 25, null));
        }
        for (var i = 0; i < 4; i++)
        {
            ListWorkerAssignments[i].DataAssignments = new List<DataAssignment>();
        }
        ListPopulationLog.Add("00:00 1 Jan,1 Uniflow| <color=#06c6eb>1000</color> incease due to immigration");
    }

    public void SetTimer(SeasonTime timer)
    {
        TimerData.TimeCurent.IdSeasonTime = timer.IdSeasonTime;
        TimerData.TimeCurent.SpeedScaleRation = timer.SpeedScaleRation;
        TimerData.TimeCurent.Ratio = timer.Ratio;
        TimerData.TimeCurent.Year = timer.Year;
        TimerData.TimeCurent.Month = timer.Month;
        TimerData.TimeCurent.Day = timer.Day;
        TimerData.TimeCurent.Hour = timer.Hour;
        TimerData.TimeCurent.Minutes = timer.Minutes;
        TimerData.TimeCurent.Second = timer.Second;
        TimerData.TimeCurent.KingdomEra = timer.KingdomEra;
        TimerData.TimeCurent.TotalTime = timer.TotalTime;
        GameData.Instance.SavedPack.SaveData.SpeedGameTimer = timer.SpeedScaleRation;
        GameData.Instance.SavedPack.SaveData.TimeWorkloadOneWorker = TimeWorkLoad.TimeOneWorkloadWorker();
        GameData.Instance.SavedPack.SaveData.TimeWorkloadOneVassal = TimeWorkLoad.TimeOneWorkloadVassal();
        GameData.Instance.SavedPack.SaveData.Ratio =(float) ((float)timer.Ratio / (float)50);
        GameTimer.Instance.Init();
    }
}

[System.Serializable]
public class SeasonType
{
    public static string SLOW = "SLOW";
    public static string NORMAL = "NORMAL";
    public static string FAST = "FAST";
}

[System.Serializable]
public class BattleStatisticsData
{
    public int PointMax;
    public int PointWin;
    public int PointLose;
    public int PointDie;
    public int PointSpy;

    public BattleStatisticsData()
    {
        PointMax = 0;
        PointWin = 0;
        PointLose = 0;
        PointDie = 0;
        PointSpy = 0;
    }
}

[System.Serializable]
public class VassalDataInGame
{
    public BaseLoginVassalInfo Data;
    public bool IsWorking;
    public int KeyJob;
    public string TaskName;
    public int SubTaskId;
    public int WorkloadStart;
    public int Salary;
    public float PercentSalary;
    public VassalDataInGame(BaseLoginVassalInfo data, bool isWorking, int key,string taskName, int subTaskId, int workloadStart,int salary,float percentSalary)
    {
        this.Data = data;
        this.IsWorking = isWorking;
        this.KeyJob = key;
        this.TaskName = taskName;
        this.SubTaskId = subTaskId;
        this.WorkloadStart = workloadStart;
        this.Salary = salary;
        this.PercentSalary = percentSalary;
    }
}
