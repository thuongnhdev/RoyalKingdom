using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public static class Utils 
    {
        public const string Construction = "Construction";
        public const string Production = "Production";
        public const string Craft = "Craft";
        public const string Trade = "Trade";
        public const string Harvest = "Harvest";
        public const string Conscription = "Conscription";
        public const string Training = "Training";
        public const string Propagation = "Propagation";
        public const string Fishing = "Fishing";
        public const string Building = "Building";
        public const string Upgrade = "Upgrade";
        public const string Destroy = "Destroy";

        public static string ConvertVector3ToString(Vector3 position)
        {
            var pos = string.Format("{0}/{1}/{2}", position.x, position.y, position.z);
            return pos;
        }

        public static Vector3 ConvertStringToVector3(string position)
        {
            string[] posSplit = position.Split('/');
            Vector3 pos = new Vector3(float.Parse(posSplit[0]), float.Parse(posSplit[1]), float.Parse(posSplit[2]));
            return pos;
        }

        public static string ConvertVector3ToStringWithParams(Vector3 position , string param)
        {
            var pos = string.Format("{0}/{1}/{2}/{3}", position.x, position.y, position.z,param);
            return pos;
        }


        public static int RemainingResources(List<ResourcePoco> _commonResources,UserBuilding _userBuilding)
        {
            int totalResource = 0, resourceComplete = 0;
            for (int i = 0; i < _commonResources.Count; i++)
            {
                totalResource += _commonResources[i].itemCount;
            }
            for (int i = 0; i < _userBuilding.constructionMaterial.Count; i++)
            {
                resourceComplete += _userBuilding.constructionMaterial[i].itemCount;
            }
            return (totalResource - resourceComplete);
        }

        public static int RemainingResourcesProduction(List<ResourcePoco> _commonResources, UserBuildingProduction _userBuildingProduction)
        {
            int totalResource = 0, resourceComplete = 0;
            for (int i = 0; i < _commonResources.Count; i++)
            {
                totalResource += _commonResources[i].itemCount;
            }
            for (int i = 0; i < _userBuildingProduction.currentMaterials.Count; i++)
            {
                resourceComplete += _userBuildingProduction.currentMaterials[i].itemCount;
            }
            return (totalResource - resourceComplete);
        }

        public static bool IsCheckDistance(Vector3 playerPos , Vector3 targetPos , float distance = 2.0f)
        {
            bool isPosX = false , isPosZ = false;
            if (playerPos.x > (targetPos.x - distance) && playerPos.x < (targetPos.x + distance)) isPosX = true;
            if ((playerPos.z > (targetPos.z - distance) && playerPos.z < (targetPos.z + distance))) isPosZ = true;
            if (isPosX && isPosZ)
                return true;
            return false;
        }

        public static IEnumerator MoveFromTo(Transform transform,Vector3 target, float speed, Action onComplete)
        {
            Vector3 b = target;
            Vector3 a = transform.position;
            float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
            float t = 0;
            while (t <= 1.0f)
            {
                t += step; // Goes from 0 to 1, incrementing by step each time
                transform.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
                yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
            }
            transform.position = b;
            if (IsCheckDistance(transform.position, a, 0))
                onComplete?.Invoke();
        }

        public static Vector3 ConvertPositionDoor(Vector3 positionBuilding, RotationDoors rotation , float size)
        {
            Vector3 positionDoor = positionBuilding;
            switch(rotation)
            {
                case RotationDoors.Left:
                    float posXLeft = positionBuilding.x - (size * ShareUIManager.Instance.Config.DURATION_PIXEL_TITLE_BUILDING_ROTATION);
                    positionDoor = new Vector3(posXLeft, positionBuilding.y, positionBuilding.z);
                    break;
                case RotationDoors.Right:
                    float posXRight = positionBuilding.x + (size * ShareUIManager.Instance.Config.DURATION_PIXEL_TITLE_BUILDING_ROTATION);
                    positionDoor = new Vector3(posXRight, positionBuilding.y, positionBuilding.z);
                    break;
                case RotationDoors.Top:
                    float posZTop = positionBuilding.z - (size * ShareUIManager.Instance.Config.DURATION_PIXEL_TITLE_BUILDING_ROTATION);
                    positionDoor = new Vector3(positionBuilding.x, positionBuilding.y, posZTop);
                    break;
                case RotationDoors.Bottom:
                    float posZBottom = positionBuilding.z + (size * ShareUIManager.Instance.Config.DURATION_PIXEL_TITLE_BUILDING_ROTATION);
                    positionDoor = new Vector3(positionBuilding.x, positionBuilding.y, posZBottom);
                    break;
            }
            return positionDoor;
        }

        public static Vector3 GetInDoorPosition(Vector3 doorPosition)
        {
            var posDoorHouse = new Vector3(doorPosition.x, 0, doorPosition.z);
            var posInDoor = posDoorHouse - Vector3.forward * 1.0f;
            return posInDoor;
        }

        public static Vector3 GetOutDoorPosition(Vector3 doorPosition)
        {
            var posDoorHouse = new Vector3(doorPosition.x, 0, doorPosition.z);
            var posOutDoor = posDoorHouse + Vector3.forward * 1.0f;
            return posOutDoor;
        }

        public static TilingTransform GetTilingTransform(int buildingObjectId,UserBuildingList userBuildingList,BuildingObjectFinder buildingFinder)
        {
            UserBuilding userBuilding = userBuildingList.GetBuilding(buildingObjectId);
            var objBuilding = buildingFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            return tilingTransform;
        }

        public static BuildingComponentGetter GetBuildingComponentGetter(int buildingObjectId, UserBuildingList userBuildingList, BuildingObjectFinder buildingFinder)
        {
            UserBuilding userBuilding = userBuildingList.GetBuilding(buildingObjectId);
            var objBuilding = buildingFinder.GetBuildingObjectByLocation(userBuilding.locationTileId);
            BuildingComponentGetter buildingGetter = objBuilding.GetComponent<BuildingComponentGetter>();
            return buildingGetter;
        }

        public static BuildingOperation GetBuildingOperation(int locationTileId,BuildingObjectFinder buildingFinder)
        {
            var objBuilding = buildingFinder.GetBuildingObjectByLocation(locationTileId);
            if (objBuilding == null) return null;
            TilingTransform tilingTransform = objBuilding.GetComponent<TilingTransform>();
            BuildingComponentGetter buildingGetter = objBuilding.GetComponent<BuildingComponentGetter>();
            return buildingGetter.Operation;
        }

        public static Quaternion GetRotation(Vector3 posInDoorBuilding, CharacterTaskData player)
        {
            Vector3 targetDirection = posInDoorBuilding - player.CharacterBehaviour.transform.position;
            Vector3 newDirection = Vector3.RotateTowards(player.CharacterBehaviour.transform.forward, targetDirection, 1, 0.0f);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            var rotation = Quaternion.LookRotation(newDirection);
            return rotation;
        }

        public static List<PositionWorkerBuilding> SetPositionWorkerBuilding(Vector3 positionBuilding, float size,float duration)
        {
            Dictionary<int, PositionItem> posDic = ConvertPositionFence(positionBuilding, size, duration);
            List<PositionWorkerBuilding> positionWorkerBuildings = new List<PositionWorkerBuilding>();
            for (int i = 0; i < posDic.Count; i++)
            {
                PositionWorkerBuilding item = new PositionWorkerBuilding(i, posDic[i].Position, false, posDic[i].RotationY);
                positionWorkerBuildings.Add(item);
            }
            return positionWorkerBuildings;
        }

        public static PositionWorkerBuilding GetPositionWorkerFance(List<PositionWorkerBuilding> positionWorkerBuildings)
        {
            Vector3 pos = Vector3.zero;
            for(var i =0;i < positionWorkerBuildings.Count;i++)
            {
                if (positionWorkerBuildings[i].IsEmpty == false)
                {
                    pos = positionWorkerBuildings[i].Position;
                    positionWorkerBuildings[i].IsEmpty = true;
                    return positionWorkerBuildings[i];
                }
            }
            if(pos == Vector3.zero)
            {
                for (var i = 0; i < positionWorkerBuildings.Count; i++)
                {
                    positionWorkerBuildings[i].IsEmpty = false;
                    pos = positionWorkerBuildings[0].Position;
                    positionWorkerBuildings[0].IsEmpty = true;
                }
            }
            return positionWorkerBuildings[0];
        }

        public static Dictionary<int, PositionItem> ConvertPositionFence(Vector3 positionBuilding, float size,float duration)
        {
            float sizeFence = size / duration;
            float pointMinX = positionBuilding.x - sizeFence;
            float pointMaxX = positionBuilding.x + sizeFence;
            float pointMinZ = positionBuilding.z - sizeFence;
            float pointMaxZ = positionBuilding.z + sizeFence;
            float durationX = (pointMaxX - pointMinX) / 4;
            float durationZ = (pointMaxZ - pointMinZ) / 4;

            List<PositionItem> posDicionary = new List<PositionItem>();
            posDicionary.Add(new PositionItem(new Vector3(pointMaxX, positionBuilding.y, pointMaxZ - durationZ), -90));
            posDicionary.Add(new PositionItem(new Vector3(pointMaxX, positionBuilding.y, posDicionary[0].Position.z - durationZ), -90));
            posDicionary.Add(new PositionItem(new Vector3(pointMaxX, positionBuilding.y, posDicionary[1].Position.z - durationZ), -90));
            posDicionary.Add(new PositionItem(new Vector3(pointMinX, positionBuilding.y, pointMaxZ - durationZ), 90));
            posDicionary.Add(new PositionItem(new Vector3(pointMinX, positionBuilding.y, posDicionary[0].Position.z - durationZ), 90));
            posDicionary.Add(new PositionItem(new Vector3(pointMinX, positionBuilding.y, posDicionary[1].Position.z - durationZ), 90));

            posDicionary.Add(new PositionItem(new Vector3(pointMaxX - durationX, positionBuilding.y, pointMaxZ), 180));
            posDicionary.Add(new PositionItem(new Vector3(posDicionary[6].Position.x - durationX, positionBuilding.y, pointMaxZ), 180));
            posDicionary.Add(new PositionItem(new Vector3(posDicionary[7].Position.x - durationX, positionBuilding.y, pointMaxZ), 180));
            posDicionary.Add(new PositionItem(new Vector3(pointMaxX - durationX, positionBuilding.y, pointMinZ), 0));
            posDicionary.Add(new PositionItem(new Vector3(posDicionary[6].Position.x - durationX, positionBuilding.y, pointMinZ), 0)); 
            posDicionary.Add(new PositionItem(new Vector3(posDicionary[7].Position.x - durationX, positionBuilding.y, pointMinZ), 0));

            Dictionary<int, PositionItem> itemDic = new Dictionary<int, PositionItem>();
         
            for (int i = 0; i < posDicionary.Count; i++)
            {
                PositionItem temp = posDicionary[i];
                int randomIndex = UnityEngine.Random.Range(i, posDicionary.Count);
                posDicionary[i] = posDicionary[randomIndex];
                posDicionary[randomIndex] = temp;
            }
            for (int i = 0; i < posDicionary.Count; i++)
            {
                itemDic.Add(i, posDicionary[i]);
            }
            return itemDic;
        }

        public static float GetRotationY(int index)
        {
            float rotationY = 0;
            switch(index)
            {
                case 0:
                case 1:
                case 2:
                    rotationY = -90;
                    break;
                case 3:
                case 4:
                case 5:
                    rotationY = 90;
                    break;
                case 6:
                case 7:
                case 8:
                    rotationY = 180;
                    break;
                case 9:
                case 10:
                case 11:
                    rotationY = 0;
                    break;
            }
            return rotationY;
        }

    
        public static BuildingMain GetWorkerHouse()
        {
            int workerHouse = TownTypeTransform.Instance.TownBaseBuildingSOList.BuildingList.Find(t => t.id == 10103).id;
            BuildingMain WorkerHouse = GameData.Instance.SavedPack.SaveData.BuildingMains.Find(t => t.BuildingId == workerHouse);
            return WorkerHouse;
        }

        public static BuildingMain GetStoreHouse(ItemType itemType)
        {
            var itemTypes = TownTypeTransform.Instance.ItemTypeList.GetItemTypeDescription(itemType);
            var buildingMain = GameData.Instance.SavedPack.SaveData.BuildingMains;
            BuildingMain storeHouse = buildingMain.Find(t => t.BuildingId == itemTypes.warehouseId);
            return storeHouse;
        }

        public static BuildingMain GetVassalHouse()
        {
            int cityHall = TownTypeTransform.Instance.TownBaseBuildingSOList.BuildingList.Find(t => t.name == "City Hall").id;
            BuildingMain VassalHouse = GameData.Instance.SavedPack.SaveData.BuildingMains.Find(t => t.BuildingId == cityHall);
            return VassalHouse;
        }

        public static List<ResourcePoco> GetResourcePocoFromServer(Fbs.AddTaskDataResponse data)
        {
            List<ResourcePoco> common = new List<ResourcePoco>();

            for (int j = 0; j < data.CommonResourceLength; j++)
            {
                var commonResouces = data.CommonResource(j).Value;
                ResourcePoco resource = new ResourcePoco()
                {
                    itemId = commonResouces.IdItem,
                    itemCount = commonResouces.Count
                };

                common.Add(resource);
            }
            return common;
        }

        public static List<PositionWorkerBuilding> GetPositionWorkerBuilding(Fbs.AddTaskDataResponse data)
        {
            List<PositionWorkerBuilding> common = new List<PositionWorkerBuilding>();

            for (int j = 0; j < data.WorkerPossitionLength; j++)
            {
                var commonResouces = data.WorkerPossition(j).Value;
                PositionWorkerBuilding resource = new PositionWorkerBuilding(
                    commonResouces.Index,
                    CreateFbWorkerVector(commonResouces.Position),
                    commonResouces.IsEmpty,
                    commonResouces.RotationY
                );

                common.Add(resource);
            }
            return common;
        }

        public static List<PositionWorkerBuilding> GetPositionVassalBuilding(Fbs.AddTaskDataResponse data)
        {
            List<PositionWorkerBuilding> common = new List<PositionWorkerBuilding>();

            for (int j = 0; j < data.VassalPossitionLength; j++)
            {
                var commonResouces = data.VassalPossition(j).Value;
                PositionWorkerBuilding resource = new PositionWorkerBuilding(
                    commonResouces.Index,
                    CreateFbWorkerVector(commonResouces.Position),
                    commonResouces.IsEmpty,
                    commonResouces.RotationY
                );

                common.Add(resource);
            }
            return common;
        }

        private static Vector3 CreateFbWorkerVector(Fbs.Vector3? position)
        {
            UnityEngine.Vector3 offset = new Vector3(
                            position.Value.X,
                            position.Value.Y,
                            position.Value.Z);
            return offset;
        }

        public static TypeJobAction GetTypeJobByName(string name)
        {
            TypeJobAction type = TypeJobAction.CONTRUCTION;
            switch (name)
            {
                case Conscription:
                    type = TypeJobAction.CONTRUCTION;
                    break;
                case Building:
                    type = TypeJobAction.BUILDING;
                    break;
                case Destroy:
                    type = TypeJobAction.DESTROY;
                    break;
                case Upgrade:
                    type = TypeJobAction.UPGRADE;
                    break;
                case Production:
                    type = TypeJobAction.PRODUCE;
                    break;
            }
            return type;
        }

        private static int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    
        public static int GetTimeDataDay(float timeD)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeD);
            var totalDay = time.TotalDays;
            var year = totalDay / 365;
            var month = totalDay / 30;
            var day = totalDay - ((year * 365) - (month * 30));

            return (int)day;
        }

        public static int GetTimeDataHour(float timeD)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeD);
            return time.Hours;
        }

        public static int GetTimeDataMinutes(float timeD)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeD);
            return time.Minutes;
        }

        public static int GetTimeDataSecond(float timeD)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeD);
            return time.Seconds;
        }

        public static TypeJobAction GetTypeJob(TypeJobAction type)
        {
            switch(type)
            {
                case TypeJobAction.CONTRUCTION:
                case TypeJobAction.UPGRADE:
                case TypeJobAction.DESTROY:
                    return TypeJobAction.BUILDING;
            }
            return type;
        }

        public static bool IsEmptyResouce(TaskData taskData, UserItemStorage _userItemStorage)
        {
            bool isEmpty = true;
            for (int i = 0; i < taskData.CommonResources.Count; i++)
            {
                int itemId = taskData.CommonResources[i].itemId;
                var itemInStore = _userItemStorage.GetItemCount(itemId);
                if (itemInStore > 0)
                    return false;
            }
            return isEmpty;
        }

        public static ResourcePocoTask GetItemIncreaseType(List<ResourcePoco> resourcePocosCurrent, List<ResourcePoco> resourcePocosTotal)
        {
            if (resourcePocosCurrent == null || resourcePocosTotal == null) return null;
            ResourcePocoTask resourcePocoTask = new ResourcePocoTask();
            for (int i = 0; i < resourcePocosTotal.Count; i++)
            {
                var itemCurrent = resourcePocosCurrent.Find(t => t.itemId == resourcePocosTotal[i].itemId);
                if (itemCurrent.itemCount < resourcePocosTotal[i].itemCount)
                {
                    int count = 0;
                    if (resourcePocosTotal[i].itemCount > 0)
                        count = resourcePocosTotal[i].itemCount - itemCurrent.itemCount;
                    if (count > 0)
                    {
                        resourcePocoTask.ResourcePoco = resourcePocosTotal[i];
                        resourcePocoTask.IndexResourcePoco = i;
                        resourcePocoTask.ItemType = TownTypeTransform.Instance.ItemList.GetItemType(resourcePocosTotal[i].itemId);
                        return resourcePocoTask;
                    }
                }

            }
            return resourcePocoTask;
        }

        public static ResourcePocoTask GetItemDecreaseType(List<ResourcePoco> resourcePocosCurrent, List<ResourcePoco> resourcePocosTotal)
        {
            if (resourcePocosCurrent == null || resourcePocosTotal == null) return null;
            ResourcePocoTask resourcePocoTask = new ResourcePocoTask();
            for (int i = 0; i < resourcePocosTotal.Count; i++)
            {
                var itemCurrent = resourcePocosCurrent.Find(t => t.itemId == resourcePocosTotal[i].itemId);
                if (itemCurrent.itemCount > 0 )
                {
                    if (resourcePocosTotal[i].itemCount > 0)
                    {
                        resourcePocoTask.ResourcePoco = resourcePocosTotal[i];
                        resourcePocoTask.IndexResourcePoco = i;
                        resourcePocoTask.ItemType = TownTypeTransform.Instance.ItemList.GetItemType(resourcePocosTotal[i].itemId);
                        return resourcePocoTask;
                    }
                }

            }
            return resourcePocoTask;
        }

        public static bool IsCheckWorkerSameBuilding(int buildingObjectId, int remainingResources)
        {
            int count = 0;
            var characterTaskData = CharacterManager.Instance.GetCharacterTaskData();
            foreach (var player in characterTaskData)
            {
                if (player.Type == Character.CharacterList.AssistantWorker && player.StatusAction == StatusAction.CarriBox
                    && player.BuildingObjectId == buildingObjectId)
                {
                    count++;
                }
            }

            foreach (var player in characterTaskData)
            {
                if (player.Type == Character.CharacterList.AssistantWorker && player.StatusAction == StatusAction.CarriBox
                    && player.BuildingObjectId == buildingObjectId && count > remainingResources)
                {
                    return true;
                }
            }
            return false;
        }

        public static int IsBringResource(List<ResourcePoco> commonResources)
        {
            int count = 0;
            for (int i = 0; i < commonResources.Count; i++)
            {
                ResourcePoco res = commonResources[i];
                count += res.itemCount;
            }
            return count;
        }

        public static List<int> GenerateRandom(int count, int min, int max)
        {
            if (max <= min || count < 0 ||
                (count > max - min && max - min > 0))
            {
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                                                      " (" + ((Int64)max - (Int64)min) + " values), or count " + count + " is illegal");
            }

            HashSet<int> candidates = new HashSet<int>();
            System.Random rnd = new System.Random();
            for (int top = max - count; top < max; top++)
            {
                if (!candidates.Add(rnd.Next(min, top + 1)))
                {
                    candidates.Add(top);
                }
            }

            List<int> result = candidates.ToList();

            for (int i = result.Count - 1; i > 0; i--)
            {
                int k = rnd.Next(i + 1);
                int tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }
    }

    public enum RotationDoors
    {
        Left = 1,
        Right,
        Top,
        Bottom,
    }

    [System.Serializable]
    public class PositionItem
    {
        public Vector3 Position;
        public float RotationY;
        public PositionItem(Vector3 position,float rotationY)
        {
            this.Position = position;
            this.RotationY = rotationY;
        }
    }
}