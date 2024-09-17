using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TimerData 
{
    public TimeData TimeCurent;
    public TimeData EndTime;
}

[System.Serializable]
public struct TimeData
{
    public int IdSeasonTime;
    public float SpeedScaleRation;
    public int Year;
    public int Month;
    public int Day;
    public int Hour;
    public int Minutes;
    public int Second;
    public int Ratio;
    public string KingdomEra;
    public long TotalTime;
}

[System.Serializable]
public class TimerPointData
{
    public int Year;
    public int Month;
    public int Day;
    public TimerPointData(int year , int month , int day)
    {
        this.Year = year;
        this.Month = month;
        this.Day = day;
    }
}

[System.Serializable]
public class SeasonTime
{
    public int IdSeasonTime;
    public float SpeedScaleRation;
    public int Ratio;
    public int Year;
    public int Month;
    public int Day;
    public int Hour;
    public int Minute;
    public int Second;
    public string KingdomEra;
    public long TotalTime;

    public SeasonTime(int idSeasonTime, float speedScaleRation,int ratio, int year, int month, int day,int hour, int minutes, int second, string kingdomEra,long totalTime)
    {
        IdSeasonTime = idSeasonTime;
        SpeedScaleRation = speedScaleRation;
        Ratio = ratio;
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minutes = minutes;
        Second = second;
        KingdomEra = kingdomEra;
        TotalTime = totalTime;
    }

    public int Minutes { get; }
}