using CoreData.UniFlow;
using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Fbs;
using TaskData = CoreData.UniFlow.Common.TaskData;
using Vector3 = UnityEngine.Vector3;

public class TaskManager : MonoSingleton<TaskManager>
{

    public List<BuildingTimeData> ListBuildingTimeData;
    // Init Task List
    public void InitTask()
    {
        ListBuildingTimeData = new List<BuildingTimeData>();
    }

    // Add Task List
    public void AddTask(TaskData taskData)
    {
        var itask = GameData.Instance.GetLoadAction(taskData);
        if (itask == null)
            GameData.Instance.SetLoadAction(taskData);
    }

    public void AddBuildingTimeHarvest(int id, int finish, int timeDuration, Vector3 position)
    {
        TimeSpan duration = new TimeSpan(0, 0, 0, timeDuration);
        DateTime timeEnd = DateTime.Now.Add(duration);
        string pos = Utils.ConvertVector3ToString(position);
        var iBuilding = ListBuildingTimeData.Find(i => i.BuildingObjectId == id && i.Position == pos);
        if (iBuilding == null)
        {
            var buildItem = new BuildingTimeData(id, finish, DateTime.Now.ToString(), timeDuration.ToString(), timeEnd.ToString(), pos);
            ListBuildingTimeData.Add(buildItem);
            AddTaskTimer((float)duration.TotalSeconds, 0, buildItem);
        }
    }

    public void AddTaskTimer(float duration, int finish, BuildingTimeData buildItem)
    {
        buildItem.StatusFinish = finish;
        //TriggerTimer.Instance.AddTaskTimer(duration, null, buildItem);
    }

    public BuildingTimeData GetBuildingTimeHarvest()
    {
        if (ListBuildingTimeData.Count > 0)
        {
            return ListBuildingTimeData[0];
        }
        else
            return null;
    }

    // Remove Task List
    public void RemoveTask(int id)
    {
        var itask = GameData.Instance.GetListAction().Find(i => i.BuildingObjectId == id);
        if (itask == null) return;
        GameData.Instance.GetListAction().Remove(itask);
        _ = APIManager.Instance.RequestRemoveTaskData(itask.BuildingObjectId, itask.BuildingId);
        GameData.Instance.RequestSaveGame();
    }

    public void RemoveBuildingTimeHarvest(int id, Vector3 position)
    {
        string pos = Utils.ConvertVector3ToString(position);
        var iBuilding = ListBuildingTimeData.Find(i => i.BuildingObjectId == id && i.Position == pos);
        ListBuildingTimeData.Remove(iBuilding);
    }

    public bool IsCheckList()
    {
        if (GameData.Instance.GetListAction().Count > 0)
        {
            return true;
        }
        return false;
    }

    public void NextTask(UserBuildingList userBuildingList, UserItemStorage userItemStorage)
    {
        var listAction = GameData.Instance.GetListAction();

        List<TaskData> listActionCheckProduction = new List<TaskData>();
        for (int i = 0; i < listAction.Count; i++)
        {
            var task = listActionCheckProduction.Find(t => t.BuildingObjectId == listAction[i].BuildingObjectId);
            if (task == null)
                listActionCheckProduction.Add(listAction[i]);
        }

        for (int i = 0; i < listActionCheckProduction.Count; i++)
        {
            //check have pending
            if (isCheckResource(listActionCheckProduction[i].CommonResources, userBuildingList.GetBuilding(listActionCheckProduction[i].BuildingObjectId), userItemStorage))
            {
                GameManager.Instance.OnCheckVassalAssignt(listActionCheckProduction[i]);
                if (listActionCheckProduction[i].TypeJobAction == TypeJobAction.CONTRUCTION || listActionCheckProduction[i].TypeJobAction == TypeJobAction.UPGRADE || listActionCheckProduction[i].TypeJobAction == TypeJobAction.DESTROY)
                {
                    var worker = CharacterManager.Instance.GetCharacterTaskData().Find(t => t.BuildingObjectId == listActionCheckProduction[i].BuildingObjectId);
                    if (worker == null)
                    {

                        if (listActionCheckProduction[i].StatusFillResource == StatusFillResource.BeginFill)
                        {
                            CharacterManager.Instance.OnBeginFillResource(listActionCheckProduction[i], StatusCache.NoCache, listActionCheckProduction[i].TypeJobAction);
                        }
                        else
                        {
                            GameManager.Instance.OnStartBuildCache(listActionCheckProduction[i]);
                        }
                    }
                }
                else if (listActionCheckProduction[i].TypeJobAction == TypeJobAction.PRODUCE)
                {
                    var worker = CharacterManager.Instance.GetCharacterTaskData().Find(t => t.BuildingObjectId == listActionCheckProduction[i].BuildingObjectId);
                    if (worker == null)
                    {
                        if (listActionCheckProduction[i].StatusFillResource == StatusFillResource.BeginFill)
                        {
                            CharacterManager.Instance.OnBeginFillResource(listActionCheckProduction[i], StatusCache.NoCache, listActionCheckProduction[i].TypeJobAction);
                        }
                        else
                        {
                            GameManager.Instance.WorkerStartProductionCache(listActionCheckProduction[i]);
                        }
                    }
                    else
                    {
                        GameManager.Instance.WorkerStartProductionCache(listActionCheckProduction[i]);
                    }
                }
            }

        }
    }

    private bool isCheckResource(List<ResourcePoco> _commonResources, UserBuilding userBuilding, UserItemStorage userItemStorage)
    {
        for (int i = 0; i < _commonResources.Count; i++)
        {
            ResourcePoco res = _commonResources[i];
            var item = userBuilding.constructionMaterial.Find(t => t.itemId == res.itemId);
            var indext = userBuilding.constructionMaterial.FindIndex(t => t.itemId == res.itemId);
            var itemInStore = userItemStorage.GetItemCount(res.itemId);
            if (itemInStore == 0)
                return false;
        }
        return true;
    }
}
