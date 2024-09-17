using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    [System.Serializable]
    public class TaskData
    {
        public int Man;
        public int IdJob;
        public int Model;
        public float Size;
        public string Name;
        public int Priority;
        public float TimeJob;
        public int TaskId;
        public int BuildingId;
        public string Position;
        public string RotationDoor;
        public float Workload;
        public int IdVassal;
        public float SpeedRate;
        public long TimeBegin;
        public int BuildingObjectId;
        public int CurrentProductId;
        public float WorkloadDone;
        public TypeJobAction TypeJobAction;
        public List<ResourcePoco> CommonResources;
        public StatusFillResource StatusFillResource;
        public List<PositionWorkerBuilding> PositionWorkerBuildings;
        public List<PositionWorkerBuilding> PositionVassalBuildings;
        public TaskData(int taskId,int idJob,string name ,int buildingId, int buildingObjectId, string position,string rotationDoor, List<ResourcePoco> CommonResources,
            StatusFillResource statusFillResource, int man,int model, TypeJobAction typeJobAction, long timeBegin,float timeJob,float workload, float size, 
            int priority,float speedRate, List<PositionWorkerBuilding> positionWorkerBuildings, 
            List<PositionWorkerBuilding> positionVassalBuildings,int currentProductId,int idVassal,float workloadDone)
        {
            this.Man = man;
            this.Name = name;
            this.Size = size;
            this.IdJob = idJob;
            this.Model = model;
            this.TaskId = taskId;
            this.TimeJob = timeJob;
            this.Workload = workload;
            this.Priority = priority;
            this.Position = position;
            this.IdVassal = idVassal;
            this.TimeBegin = timeBegin;
            this.SpeedRate = speedRate;
            this.BuildingId = buildingId;
            this.WorkloadDone = workloadDone;
            this.RotationDoor = rotationDoor;
            this.TypeJobAction = typeJobAction;
            this.CommonResources = CommonResources;
            this.BuildingObjectId = buildingObjectId;
            this.CurrentProductId = currentProductId;
            this.StatusFillResource = statusFillResource;
            this.PositionWorkerBuildings = positionWorkerBuildings;
            this.PositionVassalBuildings = positionVassalBuildings;
        }
    }

    [System.Serializable]
    public enum StatusCache
    {
        Cache = 1,
        NoCache,
    }

    [System.Serializable]
    public enum TypeJobAction
    {
        PRODUCE = 1,
        BUILDING,
        RESEARCH,
        PURCHASE,
        ITEM,
        FARM,
        HEAL,
        REPAIR,
        TRAIN,
        POPULATION,
        MANAGETROOP,
        CONTRUCTION,
        UPGRADE,
        DESTROY,
        FINISHITEM,
        None
    }

    [System.Serializable]
    public enum TypeJobActionBuilding
    {
        CONTRUCTION = 1,
        UPGRADE,
        DESTROY,
        None
    }

    [System.Serializable]
    public class BuildingTimeData
    {
        public int BuildingObjectId;
        public int StatusFinish;
        public string TimeBegin;
        public string TimeEnd;
        public string TimeDuration;
        public string Position;
        public TilingTransform TilingTransform;
        public BuildingTimeData(int buildingObjectId, int finish, string timebegin, string timeEnd, string timeDuration, string position)
        {
            this.BuildingObjectId = buildingObjectId;
            this.StatusFinish = finish;
            this.TimeBegin = timebegin;
            this.TimeEnd = timeEnd;
            this.TimeDuration = timeDuration;
            this.Position = position;
        }
    }

    [System.Serializable]
    public class NameWorkload
    {
        public static string Building = "Building";
        public static string Upgrade = "Upgrade";
        public static string Production = "Production";
        public static string Destroy = "Destroy";
    }

    [System.Serializable]
    public class BuildingMain
    {
        public int LocationId;
        public int BuildingId;
        public int LocationTileId;
        public int BuildingObj;
        public Vector3 Position;
        public Vector3 Size;
        public BuildingMain(int locationId, int buildingId, Vector3 position,Vector3 size,int locationTileId,int buildingObj)
        {
            this.LocationId = locationId;
            this.BuildingId = buildingId;
            this.LocationTileId = locationTileId;
            this.BuildingObj = buildingObj;
            this.Position = position;
            this.Size = size;
        }
    }

    [System.Serializable]
    public enum StatusFillResource
    {
        BeginFill = 1,
        CompleteFill = 2,
        None = 3
    }

    [System.Serializable]
    public class WorkerAssignment
    {
        public int Index;
        public int Priority;
        public float Percent;
        public List<DataAssignment> DataAssignments;
        public WorkerAssignment(int index,int priority , float percent, List<DataAssignment> dataAssignments)
        {
            this.Index = index;
            this.Priority = priority;
            this.Percent = percent;
            this.DataAssignments = dataAssignments;
        }
    }


    [System.Serializable]
    public class MilitaryAssignment
    {
        public int Index;
        public int Priority;
        public float Percent;
        public List<DataAssignment> DataAssignments;
        public MilitaryAssignment(int index, int priority, float percent, List<DataAssignment> dataAssignments)
        {
            this.Index = index;
            this.Priority = priority;
            this.Percent = percent;
            this.DataAssignments = dataAssignments;
        }
    }

    [System.Serializable]
    public class DataAssignment
    {
        public int TaskId;
        public string Name;
        public int Worker;
        public TypeJobAction TypeJobAction;
        public DataAssignment(int taskId, string name,int worker, TypeJobAction typeJobAction)
        {
            this.TaskId = taskId;
            this.Name = name;
            this.Worker = worker;
            this.TypeJobAction = typeJobAction;
        }
    }

    [System.Serializable]
    public class PositionWorkerBuilding
    {
        public int Index;
        public Vector3 Position;
        public bool IsEmpty;
        public float RotationY;
        public PositionWorkerBuilding(int index, Vector3 position, bool isEmpty, float rotationY)
        {
            this.Index = index;
            this.Position = position;
            this.IsEmpty = isEmpty;
            this.RotationY = rotationY;
        }
    }

    [System.Serializable]
    public class TaskAssignmentTemp
    {
        public TaskData TaskData;
        public int Worker;
        public TaskAssignmentTemp(TaskData taskData,int worker)
        {
            this.TaskData = taskData;
            this.Worker = worker;
        }
    }

    [System.Serializable]
    public class PosterPosition
    {
        public int Index;
        public string Position;
        public float TimeMove;
        public float TimeAnimation;
        public TypeHouseMove TypeHouseMove;
        public PosterPosition(int index,string position,float timeMove,float timeAnimation,TypeHouseMove typeHouseMove)
        {
            this.Index = index;
            this.Position = position;
            this.TimeMove = timeMove;
            this.TimeAnimation = timeAnimation;
            this.TypeHouseMove = typeHouseMove;
        }
    }


    [System.Serializable]
    public class TypeHouseMove
    {
        public int WareHouse;
        public int StoreHouse;
        public int BuildingHouse;
        public int BackHouse;
        public TypeHouseMove(int wareHouse,int storeHouse,int buildingHouse,int backHouse)
        {
            this.WareHouse = wareHouse;
            this.StoreHouse = storeHouse;
            this.BuildingHouse = buildingHouse;
            this.BackHouse = backHouse;
        }
    }
}