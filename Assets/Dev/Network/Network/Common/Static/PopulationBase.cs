using CoreData.UniFlow.Common;
using Fbs;
using System.Collections.Generic;
using CoreData.UniFlow;
using Google.FlatBuffers;
using UnityEngine;
using TaskData = CoreData.UniFlow.Common.TaskData;

namespace _0.PersonalWork.Harry.Scripts.Common.Static
{
    public class PopulationBase
    {
        public int ApiResultCode;
        public long Uid;
        public int Citizen;
        public int Rice;
        public int Wheat;
        public int Iron;
        public int PineWood;
        public int Glass;
        public int Stone;
        public int Granite;
        public int Gold;
        public int IdLand;
        public int IdKingdom;
        public int Dimond;
        public int ScrollToken;
        public LoginVassalInfo[] _loginVassalInfo;
        public LoginInfoPopulation[] _loginInfoPopulation;

        public PopulationBase(Fbs.ApiLoginResult apiLoginResult)
        {
            ApiResultCode = apiLoginResult.ApiResultCode;
            Uid = apiLoginResult.Uid;
            IdLand = apiLoginResult.IdLand;
            IdKingdom = apiLoginResult.IdKingdom;
            _loginInfoPopulation = new LoginInfoPopulation[Citizen];

            int constCount = apiLoginResult.LoginVassalInfoLength;
            List<BaseLoginVassalInfo> convertedConsts = new();
            for (int i = 0; i < constCount; i++)
            {
                var fbConstData = apiLoginResult.LoginVassalInfo(i).Value;
                var convertedConst = new BaseLoginVassalInfo();
                convertedConst.idVassalPlayer = fbConstData.IdVassalPlayer;
                convertedConst.idVassalTemplate = fbConstData.IdVassalTemplate;
                convertedConst.Job_1 = fbConstData.Job1;
                convertedConst.Job_2 = fbConstData.Job2;
                convertedConst.Job_3 = fbConstData.Job3;
                convertedConst.Status = fbConstData.Status;
                convertedConst.Level = fbConstData.Level;
                convertedConst.Loyalty = fbConstData.Loyalty;
                int statsCount = fbConstData.VassalStatsInfoPlayerLength;
                List<BaseDataVassalStatValue> convertedStats = new();
                for (int j = 0; j < statsCount; j++)
                {
                    var fbStatsData = fbConstData.VassalStatsInfoPlayer(j).Value;
                    var convertedStat = new BaseDataVassalStatValue()
                    {
                        Key = fbStatsData.KeyStat,
                        Value = fbStatsData.ValueStat
                    };
                    convertedStats.Add(convertedStat);
                }
                convertedConst.BaseDataVassalStats = convertedStats;

                int jobCount = fbConstData.VassalJobClassInfoPlayerLength;
                List<BaseDataVassalJobClassInfo> convertedJobs = new();
                for (int j = 0; j < jobCount; j++)
                {
                    var fbJobData = fbConstData.VassalJobClassInfoPlayer(j).Value;
                    var convertedJob = new BaseDataVassalJobClassInfo()
                    {
                        IdJobClass = fbJobData.IdJobClass,
                        ExpJobClass= fbJobData.ExpJobClass,
                        LvJobClass = fbJobData.LvJobClass
                    };
                    convertedJobs.Add(convertedJob);
                }
                convertedConst.BaseDataVassalJobClassInfos = convertedJobs;

                convertedConsts.Add(convertedConst);

            }

            MasterDataStore.Instance.BaseLoginVassalInfos = convertedConsts;

            int populationCount = apiLoginResult.LoginInfoPopulationLength;
            List<BaseLoginInfoPopulationCitzen> convertedPopulation = new();
            for (int i = 0; i < populationCount; i++)
            {
                var fbConstData = apiLoginResult.LoginInfoPopulation(i).Value;
                var convertedConst = new BaseLoginInfoPopulationCitzen()
                {
                    CitizenJobId = fbConstData.IdCitizenJob,
                    Age = fbConstData.Age,
                    Gender = fbConstData.Gender,
                    Count = fbConstData.Count
                };
                convertedPopulation.Add(convertedConst);
            }

            MasterDataStore.Instance.BaseLoginInfoPopulations = convertedPopulation;

            int troopCount = apiLoginResult.LoginPlayerTroopsLength;
            List<BasePlayerTroopInfo> convertedTroop = new();
            for (int i = 0; i < troopCount; i++)
            {
                var fbConstData = apiLoginResult.LoginPlayerTroops(i).Value;
                var convertedConst = new BasePlayerTroopInfo()
                {
                    IdTroop = fbConstData.IdTroop,
                    Vassal_1 = fbConstData.Vassal1,
                    Vassal_2 = fbConstData.Vassal2,
                    Vassal_3 = fbConstData.Vassal3,
                    Attack_1 = fbConstData.Attack1,
                    Attack_2 = fbConstData.Attack2,
                    Attack_3 = fbConstData.Attack3,
                    Attack_4 = fbConstData.Attack4,
                    Attack_5 = fbConstData.Attack5,
                    Pawns = fbConstData.Pawn,
                    Value_Infantry = fbConstData.ValueInfantry, // bộ binh
                    Value_Archer = fbConstData.ValueArcher,   // cung thủ
                    Value_Cavalry = fbConstData.ValueCavalry, // kỵ binh
                    Status = fbConstData.Status,
                    Buff_1 = fbConstData.Buff1,
                    Buff_2 = fbConstData.Buff2,
                    Buff_3 = fbConstData.Buff3,
                    Buff_4 = fbConstData.Buff4,
                    IdType = fbConstData.IdType
                };
                convertedTroop.Add(convertedConst);
            }

            MasterDataStoreGlobal.Instance.BasePlayerTroopInfos = convertedTroop;

            int taskCount = apiLoginResult.LoginWorkerTaskLength;
            List<TaskData> convertedTask = new();
            for (int i = 0; i < taskCount; i++)
            {
                var fbConstData = apiLoginResult.LoginWorkerTask(i).Value;

                int commonCount = fbConstData.CommonResourceLength;
                List<ResourcePoco> convertedCommon = new();
                for (int j = 0; j < commonCount; j++)
                {
                    var fbCommonData = fbConstData.CommonResource(j).Value;
                    var data = new ResourcePoco()
                    {
                        itemCount = fbCommonData.Count,
                        itemId = fbCommonData.IdItem
                    };
                    convertedCommon.Add(data);
                }

                int workerPositionCount = fbConstData.WorkerPossitionLength;
                List<PositionWorkerBuilding> positionWorker = new();
                for (int j = 0; j < workerPositionCount; j++)
                {
                    var fbCommonData = fbConstData.WorkerPossition(j).Value;
                    var pos = CreateFbWorkerVector(fbCommonData.Position);
                    var data = new PositionWorkerBuilding(
                        fbCommonData.Index,
                        pos,
                        fbCommonData.IsEmpty,
                        fbCommonData.RotationY
                    );
                    positionWorker.Add(data);
                }

                int vassalPositionCount = fbConstData.VassalPossitionLength;
                List<PositionWorkerBuilding> positionVassal = new();
                for (int j = 0; j < vassalPositionCount; j++)
                {
                    var fbCommonData = fbConstData.VassalPossition(j).Value;
                    var pos = CreateFbWorkerVector(fbCommonData.Position);
                    var data = new PositionWorkerBuilding(
                        fbCommonData.Index,
                        pos,
                        fbCommonData.IsEmpty,
                        fbCommonData.RotationY
                    );
                    positionVassal.Add(data);
                }

                TypeJobAction typeJob = Utils.GetTypeJobByName(fbConstData.Name);
                var convertedConst = new TaskData(
                    fbConstData.TaskId,
                    fbConstData.IdJob,
                    fbConstData.Name,
                    fbConstData.BuildingId,
                    fbConstData.BuildingPlayerId,
                    fbConstData.Position,
                    fbConstData.RotationDoor,
                    convertedCommon,
                    (StatusFillResource)fbConstData.StatusFillResource,
                    fbConstData.Worker,
                    fbConstData.Model,
                    typeJob,
                    fbConstData.TimeBegin,
                    fbConstData.Timetask,
                    fbConstData.Workload,
                    fbConstData.Size,
                    fbConstData.Priority,
                    fbConstData.SpeedRate,
                    positionWorker,
                    positionVassal,
                    fbConstData.ProductId,
                    fbConstData.IdVassal,
                    fbConstData.WorkloadDone
                );
                convertedTask.Add(convertedConst);
            }
            
            GameData.Instance.SavedPack.SaveData.ListTask.Clear();
            for (int i = 0; i < convertedTask.Count; i++)
            {
                TaskData taskData = convertedTask[i];
                taskData.WorkloadDone = 10;
                GameData.Instance.SavedPack.SaveData.ListTask.Add(taskData);
            }
            GameData.Instance.saveGameData();
            MasterDataStoreGlobal.Instance.BasePlayerTroopInfos = convertedTroop;

            DataManager.Instance.OnInitDataMaster();
        }
        
        public UnityEngine.Vector3 CreateFbWorkerVector(Fbs.Vector3? position)
        {
            UnityEngine.Vector3 offset = new UnityEngine.Vector3(position.Value.X, position.Value.Y, position.Value.Z);
            return offset;
        }
    }
}