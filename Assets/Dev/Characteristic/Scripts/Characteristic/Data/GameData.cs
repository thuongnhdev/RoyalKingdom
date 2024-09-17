#define SAVE_BY_BINARY

using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Assets.Dev.Tutorial.Scripts;
using CoreData.UniFlow.Common;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreData.UniFlow
{
    public class GameData
    {
        #region singleton pattern
        public static GameData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameData(false);
                }
                return _instance;
            }
        }

        public static event Action<bool> OnMusicMute = delegate { };

        private static GameData _instance;

        public PlayerSavePack SavedPack
        {
            get
            {
                if (savedPack == null)
                {
                    savedPack = new PlayerSavePack();
                }
                return savedPack;
            }
        }

        private PlayerSavePack savedPack;

        private bool isDirty;
        private bool isRequestSave;
        private bool useThreadSave;
        private bool isThreadSaving;

        public enum DailyRewardType
        {
            RESET,
            DISABLE,
            NORMAL
        }

        public enum LoadType
        {
            LOCAL,
            ONLINE,
            NONE
        }
        public GameData(bool useThreadSave = true)
        {
            _instance = this;

            this.useThreadSave = useThreadSave;
            this.isThreadSaving = false;
            this.isDirty = false;
            this.isRequestSave = false;
            ThreadPool.SetMaxThreads(1, 1);
        }

        ~GameData()
        {
#if TRANSFER_DATA
            apiManager.OnRequestUserInforSuccess -= OnRequestUserInforSuccess;
            apiManager.OnRequestUserInforFailed -= OnRequestUserInforFailed;
            apiManager.OnUpdateUserInforSuccess -= OnUpdateUserInforSuccess;
            apiManager.OnUpdateUserInforFailed -= OnUpdateUserInforFailed;
#endif
        }


        #endregion
        public void saveGameData()
        {
            if (isThreadSaving)
            {
                isDirty = true;
                return;
            }

#if IGNORE_SAVEGAME
            isDirty = false;
            isRequestSave = false;
            return;
#endif

            if (useThreadSave)
                ThreadPool.QueueUserWorkItem(ThreadSave);
            else
                ThreadSave(null);

            isRequestSave = false;
        }

        public void LoadGameData(LoadType type)
        {
            switch (type)
            {
                case LoadType.LOCAL:
                    LoadLocalConfig();
                    break;
                case LoadType.ONLINE:
                    var idUser = GameData.Instance.GetUserId();
                    if (IsUserLogin())
                        LoadOnlineConfig(idUser);
                    else
                        LoadLocalConfig();
                    break;
            }
        }

        public bool IsUserLogin()
        {
            var idUser = GameData.Instance.GetUserId();
            return !string.IsNullOrEmpty(idUser);
        }

        private void LoadLocalConfig()
        {
            savedPack = LoadLocalDataFromFile();
            if (savedPack == null)
            {
                savedPack = new PlayerSavePack();
            }
        }


        private PlayerSavePack LoadLocalDataFromFile()
        {
            if (LoadSaveData.Exit(ConfigManager.FileNameGameData))
            {
                try
                {
                    var data = LoadSaveData.Load<string>(ConfigManager.FileNameGameData);
                    savedPack = JsonUtility.FromJson<PlayerSavePack>(data);
                    return savedPack;
                }
                catch (Exception ex)
                {
                    Debug.Log($"Failed LoadLocalConfig. Error: {ex.Message}");
                }
            }
            return null;
        }

        public void LoadOnlineConfig(string idUser, bool forced = false, Action completed = null)
        {
            if (string.IsNullOrEmpty(idUser)) return;
    
        }

        public void OnMarkDirty()
        {
            isDirty = true;
        }

        public void SaveIfDirty()
        {
            if (isDirty)
            {
                isRequestSave = true;
            }
        }

        public void SyncUserProfileOnApplicationInBackground(bool shouldSaveData = true)
        {

            if (shouldSaveData)
            {
               Debug.Log($"SyncUserProfileOnApplicationInBackground. isDirty: {isDirty}");
                if (isDirty)
                {
                    saveGameData();
                    ThreadSaveOnline();
                    isDirty = false;
                }
            }
        }

        public void OnApplicationQuit()
        {
            SyncUserProfileOnApplicationInBackground();
        }
        public void OnLateUpdate()
        {
            if (isRequestSave)
            {
                saveGameData();
            }
        }
 
        public string GetUserId()
        {
            var idUser = PlayerPrefs.GetString(ConfigManager.IdUser, String.Empty);
            return idUser;
        }

        public List<TaskData> GetListAction()
        {
            return savedPack.SaveData.ListTask;
        }

        public TaskData GetLoadAction(TaskData taskData)
        {
            var itask = savedPack.SaveData.ListTask.Find(i => i.BuildingObjectId == taskData.BuildingObjectId && i.Position == taskData.Position);
            return itask;
        }

        public void SetLoadAction(TaskData taskData)
        {
            //_ = APIManager.Instance.RequestAddTaskData(taskData.BuildingObjectId, taskData.BuildingId, taskData.Man, (int)taskData.Size,
            //    taskData.Name, taskData.Priority, taskData.SpeedRate, taskData.Position);
            savedPack.SaveData.ListTask.Add(taskData);
            RequestSaveGame();
        }

        public void AddJobWorkerAssignment(int priority ,int taskId,int worker,string name, TypeJobAction typeJobAction)
        {
            DataAssignment data = new DataAssignment(taskId, name, worker, typeJobAction);
            var itemAssignments = savedPack.SaveData.ListWorkerAssignments.Find(t => t.Priority == priority);
            if (itemAssignments == null)
                itemAssignments.DataAssignments = new List<DataAssignment>();
            itemAssignments.DataAssignments.Add(data);
            RequestSaveGame();
        }

        public void UpdateJobWorkerAssignment(int priority,int taskId, int worker)
        {
            var item = savedPack.SaveData.ListWorkerAssignments.Find(t => t.Priority == priority);
            DataAssignment data = item.DataAssignments.Find(t => t.TaskId == taskId);
            if (data != null)
                data.Worker = worker;
            RequestSaveGame();
        }

        public void RemoveJobWorkerAssignment(int taskId)
        {
            for(var i = 0; i< savedPack.SaveData.ListWorkerAssignments.Count;i++)
            {
                for(var j = 0; j < savedPack.SaveData.ListWorkerAssignments[i].DataAssignments.Count;j++)
                {
                    if (savedPack.SaveData.ListWorkerAssignments[i].DataAssignments[j].TaskId == taskId)
                        savedPack.SaveData.ListWorkerAssignments[i].DataAssignments.RemoveAt(j);
                }
            }
            RequestSaveGame();
        }

        void ThreadSave(object stateInfo)
        {
            if (savedPack == null) return;
            isThreadSaving = true;
            savedPack.VERSION = DateTime.Now.Ticks;
            var dataSave = JsonUtility.ToJson(savedPack);
            if (string.IsNullOrEmpty(dataSave)) return;
            LoadSaveData.Save(ConfigManager.FileNameGameData, dataSave);
            isThreadSaving = false;
            PlayerPrefs.SetString(ConfigManager.VersionConfig, savedPack.VERSION.ToString());
            PlayerPrefs.Save();
        }

        void ThreadSaveOnline()
        {
            // save on firebase
            var idUser = GameData.Instance.GetUserId();
            Debug.Log($"ThreadSaveOnline idUser: {idUser}");
            if (!string.IsNullOrEmpty(idUser))
            {
                var nameDataBase = ConfigManager.FileNameGameData;
                PlayerSavePack playerSavePack = savedPack;
                var dataSave = JsonUtility.ToJson(playerSavePack);
            }
        }

        public void EventSettingSound(bool isMute)
        {
            OnMusicMute?.Invoke(isMute);
        }

        public void RequestSaveGame()
        {
            isRequestSave = true;
            OnMarkDirty();
        }

        public void ClearDataLocal()
        {
            TutorialData.Instance.Reset();
            PlayerPrefs.DeleteAll();
            // check if file exists
            if (!LoadSaveData.Exit(ConfigManager.FileNameGameData))
            {
                Debug.Log("File not exist");
            }
            else
            {

                LoadSaveData.DeleteAllData();

            }
        }
        
#if UNITY_EDITOR
        [MenuItem("Uniflow/ClearSave")]
        public static void DeleteFile()
        {
            PlayerPrefs.DeleteAll();
            // check if file exists
            if (!LoadSaveData.Exit(ConfigManager.FileNameGameData))
            {
                Debug.Log("File not exist");
            }
            else
            {

                LoadSaveData.DeleteAllData();

            }
        }

        [MenuItem("Uniflow/Timer/NextSixMonth")]
        public static void NextSixMonth()
        {
            var time = PlayerPrefs.GetString("TimerOnUpdatePopulation");
            string[] timeList = time.Split('/');
            int year = int.Parse(timeList[0]);
            int month = int.Parse(timeList[1]);
            if (month == 1) month = 7;
            else if(month == 7)
            {
                month = 1;
                year += 1;
            }
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Year = year;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month = month;
            GameData.Instance.RequestSaveGame();
            GameTimer.Instance.PopulationGrowth(year,month);

        }

        [MenuItem("Uniflow/Timer/SalaryDay")]
        public static void SalaryDay()
        {
            
            int day = 1;
            var time = PlayerPrefs.GetString("TimerSalary");
            string[] timeList = time.Split('/');
            int month = int.Parse(timeList[0]);
            month++;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Month = month;
            GameData.Instance.SavedPack.SaveData.TimerData.TimeCurent.Day = day;
            GameData.Instance.RequestSaveGame();
            GameTimer.Instance.SalaryTimer(month,day);

        }
#endif
    }
}