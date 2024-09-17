using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreData.UniFlow.Common;
using System;
using CoreData.UniFlow;

public class TownTypeTransform : MonoSingleton<TownTypeTransform>
{
    [SerializeField]
    private GameEvent _onTypeHouse;

    [SerializeField]
    public ItemList ItemList;

    [SerializeField]
    public ItemTypeList ItemTypeList;

    [SerializeField]
    private GameEvent _onUpdatePositionHouse;

    public TownBaseBuildingSOList TownBaseBuildingSOList;

    private List<DataIdBuilding> DataIdBuildings = new List<DataIdBuilding>();

    private void OnEnable()
    {
        _onTypeHouse.Subcribe(ConvertIdBuilding);
    }

    private void OnDisable()
    {
        _onTypeHouse.Unsubcribe(ConvertIdBuilding);
    }

    public void ConvertIdBuilding(object[] eventParam)
    {
        if (eventParam.Length == 0)
        {
            return;
        }

     
        DataIdBuilding dataIdBuilding = new DataIdBuilding();
        dataIdBuilding.LocationId = (int)eventParam[0];
        dataIdBuilding.BuildingId = (int)eventParam[1];
        dataIdBuilding.TransformBuilding = (Transform)eventParam[2];
        dataIdBuilding.LocationTileId = (int)eventParam[3];
        DataIdBuildings.Add(dataIdBuilding);
        
    }

    public void SetDataBuilding()
    {
        for(var i =0;i< DataIdBuildings.Count;i++)
        {
            var house = GameData.Instance.SavedPack.SaveData.BuildingMains.Find(t => t.BuildingId == DataIdBuildings[i].BuildingId);
            TilingTransform tilingTransform = DataIdBuildings[i].TransformBuilding.gameObject.GetComponent<TilingTransform>();
            BuildingComponentGetter buildingGetter = DataIdBuildings[i].TransformBuilding.gameObject.GetComponent<BuildingComponentGetter>();
            var size = tilingTransform.GetBoxCollider().transform.localScale;

            var buildingObj = VariableManager.Instance.UserBuildingList.GetAllBuildingsOfId(DataIdBuildings[i].BuildingId);
            BuildingMain building = new BuildingMain(DataIdBuildings[i].LocationId, DataIdBuildings[i].BuildingId, transform.localPosition, size, DataIdBuildings[i].LocationTileId, buildingObj[0]);
            building.BuildingId = DataIdBuildings[i].BuildingId;
            building.LocationId = DataIdBuildings[i].LocationId;
            building.Position = DataIdBuildings[i].TransformBuilding.localPosition;
            building.LocationTileId = DataIdBuildings[i].LocationTileId;

            building.BuildingObj = buildingObj[0];
            building.Size = size;
            GameData.Instance.SavedPack.SaveData.BuildingMains.Add(building);
            GameData.Instance.RequestSaveGame();
        }
     
    }

}
[System.Serializable]
public class DataIdBuilding
{
    public int LocationId;
    public int BuildingId;
    public int LocationTileId;
    public Transform TransformBuilding;
    public void SetData(DataIdBuilding data)
    {
        LocationId = data.LocationId;
        BuildingId = data.BuildingId;
        TransformBuilding = data.TransformBuilding;
        LocationTileId = data.LocationTileId;
    }
}

[System.Serializable]
public class ResourcePocoTask
{
    public ItemType ItemType;
    public int IndexResourcePoco;
    public ResourcePoco ResourcePoco;
    public void SetData(ResourcePocoTask data)
    {
        ItemType = data.ItemType;
        ResourcePoco = data.ResourcePoco;
        IndexResourcePoco = data.IndexResourcePoco;
    }
}