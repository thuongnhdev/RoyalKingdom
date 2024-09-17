using System.Collections;
using System.Collections.Generic;
using CoreData.UniFlow.Common;
using Pathfinding;
using UnityEngine;
using TaskData = CoreData.UniFlow.Common.TaskData;
using Vector3 = UnityEngine.Vector3;

public class PoterManager : MonoSingleton<PoterManager>
{
    [SerializeField]
    private AIPath _aiPath;

    public void AddPoter(int index, TaskData taskData)
    {
        PoterDataRequestBody data = new PoterDataRequestBody();
        data.TaskId = taskData.TaskId;
        data.BuildingPlayerId = taskData.TaskId;
        data.Priority = taskData.Priority;
        data.IdJob = taskData.IdJob;
        data.CommonResource = taskData.CommonResources;
        data.PosterPositions = CalculatorPositionList(taskData);
        _ = APIManager.Instance.RequestPoterData(data);
    }

    
    public void RemovePoter()
    {

    }

    public void UpdatePoter()
    {

    }

    public List<PosterPosition> CalculatorPositionList(TaskData taskData)
    {
        int indexPosition = 0;
        List<PosterPosition> positions = new List<PosterPosition>();
        var posHouse = Utils.GetWorkerHouse();
        if (taskData.CommonResources.Count > 0)
        {
            for (int i = 0; i < taskData.CommonResources.Count; i++)
            {
                ResourcePocoTask resourcePocoTask = new ResourcePocoTask();

                resourcePocoTask.ResourcePoco = taskData.CommonResources[i];
                resourcePocoTask.ItemType = TownTypeTransform.Instance.ItemList.GetItemType(taskData.CommonResources[i].itemId);

                BuildingMain storeHouse = Utils.GetStoreHouse(resourcePocoTask.ItemType);
                var buildingOperation = Utils.GetBuildingOperation(storeHouse.LocationTileId, VariableManager.Instance.BuildingObjectFinder);
                Vector3 posDoorStoreHouse = buildingOperation.GetDoorPosition();
                posDoorStoreHouse = new Vector3(posDoorStoreHouse.x, 0, posDoorStoreHouse.z);
                Vector3 posOutDoor = Utils.GetOutDoorPosition(posDoorStoreHouse);

                PosterPosition posWareHouse = new PosterPosition();
                indexPosition++;
                posWareHouse.Index = indexPosition;
                posWareHouse.Position = Utils.ConvertVector3ToString(posHouse.Position);
    
                Path pathWareHouse = GetPath(posHouse.Position, posOutDoor);
                float timeMoveWareHouse = pathWareHouse.duration / _aiPath.maxSpeed;

                posWareHouse.TimeMove = timeMoveWareHouse;
                posWareHouse.TimeAnimation = 1.5f;
                posWareHouse.TypeHouseMove = new TypeHouseMove();
                posWareHouse.TypeHouseMove.BuildingHouse = 0;
                posWareHouse.TypeHouseMove.WareHouse = 1;
                posWareHouse.TypeHouseMove.StoreHouse = 0;
                posWareHouse.TypeHouseMove.BackHouse = 0;

                positions.Add(posWareHouse);

                PosterPosition posStore = new PosterPosition();
                indexPosition++;
                posStore.Index = indexPosition;
                posStore.Position = Utils.ConvertVector3ToString(posOutDoor);
                Vector3 posBuilding = Utils.ConvertStringToVector3(taskData.Position);
                Path pathStore = GetPath(posOutDoor, posBuilding);
                float timeMoveStore = pathStore.duration / _aiPath.maxSpeed;

                posStore.TimeMove = timeMoveStore;
                posStore.TimeAnimation = 1.5f;
                posStore.TypeHouseMove = new TypeHouseMove();
                posStore.TypeHouseMove.BuildingHouse = 0;
                posStore.TypeHouseMove.WareHouse = 0;
                posStore.TypeHouseMove.StoreHouse = 1;
                posStore.TypeHouseMove.BackHouse = 0;

                positions.Add(posStore);


                PosterPosition posTaskData = new PosterPosition();
                indexPosition++;
                posTaskData.Index = indexPosition;
                posTaskData.Position = taskData.Position;
                Vector3 positionTaskData = Utils.ConvertStringToVector3(taskData.Position);
                Path pathBackWareHouse = GetPath(positionTaskData, posHouse.Position);
                float timeMoveBackWareHouse = pathBackWareHouse.duration / _aiPath.maxSpeed;

                posTaskData.TimeMove = timeMoveBackWareHouse;
                posTaskData.TimeAnimation = 1.5f;
                posTaskData.TypeHouseMove = new TypeHouseMove();
                posTaskData.TypeHouseMove.BuildingHouse = 1;
                posTaskData.TypeHouseMove.WareHouse = 0;
                posTaskData.TypeHouseMove.StoreHouse = 0;
                posTaskData.TypeHouseMove.BackHouse = 0;

                positions.Add(posTaskData);

            }
        }
       
        return positions;
    }

    [SerializeField]
    private Seeker seeker;
    public Path GetPath(Vector3 begin, Vector3 end)
    {
        return seeker.StartPath(begin, end);
    }

    public float OnPathComplete(Path path)
    {
        if (!path.error)
        {
            Debug.Log(path.GetTotalLength());
            return path.GetTotalLength();
        }

        return 0;
    }
}
