using System;
using System.Collections;
using System.Collections.Generic;
using DataCore;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using CoreData.UniFlow;
using System.Globalization;
using Cysharp.Threading.Tasks;

namespace CoreData.UniFlow
{
    public class GameTimer : MonoSingleton<GameTimer>
    {
        private const string REFRESH_REAL_TIMER = "{0}:{1} UTC, {2} {3}";
        private const string REFRESH_GAME_TIMER = "{0}:{1}, {2} {3}, {4} Uniflow";

        private TimerData _timerData;
        private float _timeDelta = 0.2f;
        private DateTime _openAppDate;
        private long _currentTimeCount = -1;

        [SerializeField]
        private GameEvent _onUpdateSalary;

        [SerializeField]
        private TextMeshProUGUI tmpTimerSeason;

        [SerializeField]
        private TextMeshProUGUI tmpTimerTopMain;

        [SerializeField]
        private TextMeshProUGUI tmpTimerPopupWeather;
        public TimerData TimerData
        {
            get
            {
                return _timerData;
            }
        }

        private DateTime _timeEnd;

        public DateTime TimeEnd
        {
            get
            {
                return _timeEnd;
            }
        }

        private TimerPointData _timeCurrent;

        public TimerPointData TimeCurrent
        {
            get
            {
                return _timeCurrent;
            }
        }

        private void FixedUpdate()
        {
            if (GameData.Instance == null) return;
            if (GameData.Instance.SavedPack == null) return;
            if (GameData.Instance.SavedPack.SaveData == null) return;
            _countUpdate++;
            _currentTimeCount += (long)GameData.Instance.SavedPack.SaveData.Ratio;
            if (_currentTimeCount > 0 && _countUpdate == 100)
            {
                _countUpdate = 0;
                FormatTime(_currentTimeCount);
            }
        }

        private int _countUpdate = 0;
        public void FormatTime(long totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.TotalTime = totalSeconds;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day = time.Days;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Hour = time.Hours;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Minutes = time.Minutes;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Second = time.Seconds;

            DateTime now = DateTime.UtcNow;
            string timeTxt = string.Format(REFRESH_GAME_TIMER, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Hour.ToString("00"), 
                GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Minutes.ToString("00"), 
                GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day.ToString(),
                 SwicthNameMonth(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month),
                 (GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year).ToString());
            tmpTimerPopupWeather.text = timeTxt;
            tmpTimerSeason.text = SwicthNameSeason(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month);
            tmpTimerTopMain.text = string.Format(REFRESH_REAL_TIMER, now.Hour.ToString("00"), now.Minute.ToString("00"), now.Day.ToString(), SwicthNameMonth(now.Month), now.Year.ToString());
            //tmpTimerPopulation.text = timeTxt;


            // Population growth and decline over time.
            //PopulationGrowth(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month);

            // check salary day
            //SalaryTimer(GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month, GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day);
        }

        public void PopulationGrowth(int year ,int month)
        {
            var time = PlayerPrefs.GetString("TimerOnUpdatePopulation");
            string[] timeList = time.Split('/');
            if (timeList.Length < 2) return;
            int value;
            if (int.TryParse(timeList[0], out value) || int.TryParse(timeList[1], out value)) // or float.TryParse, or double.TryParse etc
            {
                Debug.Log("The value is " + value);
            }
            else
            {
                Debug.Log("Not a valid integer");
                return;
            }
            if (month == MasterDataStore.Instance.GetNaturalChangeTime() || month >= (MasterDataStore.Instance.GetNaturalChangeTime() + MasterDataStore.Instance.GetNaturalChangeMonth()))
            {
                if (year != int.Parse(timeList[0]) || month != int.Parse(timeList[1]))
                {
                    PopulationManager.Instance.TotalBirth();
                    PopulationManager.Instance.BabyBoyBirth();
                    PopulationManager.Instance.NaturalDeath();
                    var monthCache = GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month;
                    var timeCurrent = string.Format("{0}/{1}", year, monthCache);
                    PlayerPrefs.SetString("TimerOnUpdatePopulation", timeCurrent);
                    PlayerPrefs.Save();
                }

            }
        }

        public void SalaryTimer(int month ,int day)
        {
            var time = PlayerPrefs.GetString("TimerSalary");
            string[] timeList = time.Split('/');
            if (timeList.Length < 2) return;
            if (day == MasterDataStore.Instance.GetNaturalChangeTime())
            {
                if (month != int.Parse(timeList[0]) || day != int.Parse(timeList[1]))
                {
                    _onUpdateSalary.Raise();
                    var timeCurrent = string.Format("{0}/{1}", month, day);
                    PlayerPrefs.SetString("TimerSalary", timeCurrent);
                    PlayerPrefs.Save();
                }

            }        }

        private string SwicthNameSeason(int month)
        {
            string strMonth = "Spring";
            switch (month)
            {
                case 4:
                case 5:
                case 6:
                    strMonth = "Summer";
                    break;
                case 7:
                case 8:
                case 9:
                    strMonth = "Autumn";
                    break;
                case 10:
                case 11:
                case 12:
                    strMonth = "Winter";
                    break;
                    
            }
            return strMonth;
        }

        public string SwicthNameMonth(int month)
        {
            string strMonth = "Jan";
            switch(month)
            {
                case 2:
                    strMonth = "Feb";
                    break;
                case 3:
                    strMonth = "Mar";
                    break;
                case 4:
                    strMonth = "Apr";
                    break;
                case 5:
                    strMonth = "May";
                    break;
                case 6:
                    strMonth = "Jun";
                    break;
                case 7:
                    strMonth = "Jul";
                    break;
                case 8:
                    strMonth = "Aug";
                    break;
                case 9:
                    strMonth = "Sep";
                    break;
                case 10:
                    strMonth = "Oct";
                    break;
                case 11:
                    strMonth = "Nov";
                    break;
                case 12:
                    strMonth = "Dec";
                    break;
            }
            return strMonth;
        }

        public void Init()
        {
            if (MasterDataStore.Instance == null) return;
            if (MasterDataStore.Instance.BaseDataSeasonTimes == null) return;
            _timerData = GameData.Instance.SavedPack.SaveData.TimerData;
            _currentTimeCount = _timerData.TimeCurent.TotalTime;


            var timePopulation = string.Format("{0}/{1}", _timerData.TimeCurent.Year, _timerData.TimeCurent.Month);
            PlayerPrefs.SetString("TimerOnUpdatePopulation", timePopulation);

            var timeSalary = string.Format("{0}/{1}", _timerData.TimeCurent.Month, _timerData.TimeCurent.Day);
            PlayerPrefs.SetString("TimerSalary", timeSalary);
            PlayerPrefs.Save();

            //          if (_timerData == null)
            //          {
            //              _timerData = GameData.Instance.SavedPack.SaveData.TimerData;
            //              if (GameData.Instance.SavedPack.SaveData.TimerData.EndTime.Day == 0)
            //              {
            //                  long ticks = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0,
            //new CultureInfo("en-US", false).Calendar).Ticks;
            //                  DateTime startTime = new DateTime(ticks);
            //                  _timerData.TimeCurent.Year = 0;
            //                  _timerData.TimeCurent.Month = startTime.Month;
            //                  _timerData.TimeCurent.Day = startTime.Day;
            //                  _timerData.TimeCurent.Hour = startTime.Hour;
            //                  _timerData.TimeCurent.Minutes = startTime.Minute;
            //                  _timerData.TimeCurent.Second = startTime.Second;

            //                  double seasonPeriod_GameTime = 0, seasonPeriod_RealTime = 0, seasonRestPeriod_RealTime = 0;
            //                  for (int i = 0; i < MasterDataStore.Instance.BaseDataSeasonTimes.Count; i++)
            //                  {
            //                      if (MasterDataStore.Instance.BaseDataSeasonTimes[i].SeasonType == GameData.Instance.SavedPack.SaveData.SeasonTimer)
            //                      {
            //                          seasonPeriod_GameTime = MasterDataStore.Instance.BaseDataSeasonTimes[i].SeasonPeriod_GameTime;
            //                          seasonPeriod_RealTime = MasterDataStore.Instance.BaseDataSeasonTimes[i].SeasonPeriod_RealTime;
            //                          seasonRestPeriod_RealTime = MasterDataStore.Instance.BaseDataSeasonTimes[i].SeasonRestPeriod_RealTime;
            //                      }
            //                  }

            //                  var speedTimer = seasonPeriod_GameTime / seasonPeriod_RealTime;
            //                  GameData.Instance.SavedPack.SaveData.SpeedGameTimer = (int)speedTimer;
            //                  seasonPeriod_RealTime = (seasonPeriod_RealTime * speedTimer);
            //                  DateTime timeEnd = startTime.AddDays(seasonPeriod_RealTime);
            //                  _timerData.EndTime.Year = timeEnd.Year;
            //                  _timerData.EndTime.Month = timeEnd.Month;
            //                  _timerData.EndTime.Day = timeEnd.Day;
            //                  _timerData.EndTime.Hour = timeEnd.Hour;
            //                  _timerData.EndTime.Minutes = timeEnd.Minute;
            //                  _timerData.EndTime.Second = timeEnd.Second;

            //                  _timeCurrent = new TimerPointData(_timerData.TimeCurent.Year, _timerData.TimeCurent.Month, _timerData.TimeCurent.Day);
            //                  _currentTimeCount = (_timerData.TimeCurent.Second + (_timerData.TimeCurent.Minutes * 60) + (_timerData.TimeCurent.Hour * 3600) + (_timerData.TimeCurent.Day * 24 * 3600));
            //                  GameData.Instance.SavedPack.SaveData.TimerData = _timerData;
            //                  GameData.Instance.RequestSaveGame();

            //                  _openAppDate = System.DateTime.Now;
            //                  var timeCurrent = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            //                  PlayerPrefs.SetString("TimerBeginOpenApp", timeCurrent);


            //                  var timePopulation = string.Format("{0}/{1}", _timerData.TimeCurent.Year, _timerData.TimeCurent.Month);
            //                  PlayerPrefs.SetString("TimerOnUpdatePopulation", timePopulation);

            //                  var timeSalary = string.Format("{0}/{1}", _timerData.TimeCurent.Month, _timerData.TimeCurent.Day);
            //                  PlayerPrefs.SetString("TimerSalary", timeSalary);
            //                  PlayerPrefs.Save();

            //              }
            //              else
            //              {
            //                  var timeOpenApp = PlayerPrefs.GetString("TimerBeginOpenApp", "0");
            //                  if (string.Compare(timeOpenApp, "0") == 0) return;
            //                  _openAppDate = DateTime.ParseExact(timeOpenApp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            //                  var timeBegin = PlayerPrefs.GetString("TimerOnApplicationQuit", "0");
            //                  if (string.Compare(timeBegin, "0") == 0) return;
            //                  DateTime oDate = DateTime.ParseExact(timeBegin, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            //                  System.TimeSpan diff1 = DateTime.Now.Subtract(oDate);
            //                  float totalAdd = (diff1.Seconds + (diff1.Minutes * 60) + (diff1.Hours * 3600) + ((int)diff1.Days * 24 * 3600));
            //                  totalAdd = totalAdd * GameData.Instance.SavedPack.SaveData.SpeedGameTimer;
            //                  float timerBefore = (_timerData.TimeCurent.Second + (_timerData.TimeCurent.Minutes * 60) + (_timerData.TimeCurent.Hour * 3600) + (_timerData.TimeCurent.Day * 24 * 3600));
            //                  _currentTimeCount = totalAdd + timerBefore;
            //                  _timeCurrent = new TimerPointData(_timerData.TimeCurent.Year, _timerData.TimeCurent.Month, _timerData.TimeCurent.Day);
            //              }
            //              _timeDelta = Time.deltaTime * GameData.Instance.SavedPack.SaveData.SpeedGameTimer;


            //          }
        }

        private void OnApplicationQuit()
        {
            var timeCurrent = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            PlayerPrefs.SetString("TimerOnApplicationQuit",timeCurrent);
            PlayerPrefs.Save();
        }
    }

}
