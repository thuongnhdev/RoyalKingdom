using Cysharp.Threading.Tasks;
using Fbs;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserBuilding
{
    public string editorBuildingName;
    public int buildingObjectId;
    public int buildingId;
    public int locationTileId;
    public int buildingLevel;
    [SerializeField]
    private int _vassalIncharge;
    public int vassalInCharge
    {
        get
        {
            return _vassalIncharge;
        }
        set
        {
            if (_vassalIncharge == value)
            {
                return;
            }
            _vassalIncharge = value;
            TriggerChangeVassal().Forget();
        }
    }
    public float rotation;
    public float constructedTime;
    public float destructedTime;
    public float currentContructionRate;
    public float currentDestructionRate;
    public BuildingStatus status;
    public List<ResourcePoco> constructionMaterial = new();
    public List<ResourcePoco> refundMaterial = new();
    [SerializeField]
    private List<ResourcePoco> _consumedResources = new();
    public List<ResourcePoco> ConsumeResources => _consumedResources;

    private bool _justChangeVassal = false; // this flag is down in end of frame which it is raised

    public UserBuilding()
    {

    }

    public UserBuilding(BuildingTownMap data)
    {
        buildingObjectId = data.BuildingPlayerId;
        buildingId = data.BuildingTemplateId;
        locationTileId = data.Location;
        buildingLevel = data.Level;
        vassalInCharge = data.VassalInCharge;
        rotation = data.Rotation;
        constructedTime = data.ConstructedTime;
        destructedTime = data.DestructedTime;
        currentContructionRate = data.CurrentConstructionRate;
        currentDestructionRate = data.CurrentDestructionRate;
        status = (BuildingStatus)data.Status;

        int matCount = data.ItemMaterialLength;
        List<ResourcePoco> materials = new(matCount);
        for (int i = 0; i < matCount; i++)
        {
            int itemId = data.ItemMaterial(i);
            int itemCount = data.CountItemMaterial(i);
            if (itemId <= 0 || itemCount <= 0)
            {
                continue;
            }

            materials.Add(new() { itemId = itemId, itemCount = itemCount });
        }
        constructionMaterial = materials;
    }

    public void SetData(UserBuilding data)
    {
        buildingObjectId = data.buildingObjectId;
        buildingId = data.buildingId;
        locationTileId = data.locationTileId;
        buildingLevel = data.buildingLevel;
        vassalInCharge = data.vassalInCharge;
        rotation = data.rotation;
        constructedTime = data.constructedTime;
        destructedTime = data.destructedTime;
        currentContructionRate = data.currentContructionRate;
        currentDestructionRate = data.currentDestructionRate;
        status = data.status;
        constructionMaterial = data.constructionMaterial;
        refundMaterial = data.refundMaterial;
    }

    public bool IsJustChangedVassal()
    {
        return _justChangeVassal;
    }

    public void AddConsumedResource(List<ResourcePoco> resources)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            var addResource = resources[i];
            var consumed = _consumedResources.Find(resource => resource.itemId == addResource.itemId);
            if (ResourcePoco.IsZero(consumed))
            {
                consumed.itemId = addResource.itemId;
                _consumedResources.Add(consumed);
            }
            consumed.itemCount += addResource.itemCount;
        }
    }

    public void ConsumeConstructionResource()
    {
        AddConsumedResource(constructionMaterial);
        constructionMaterial.Clear();
    }

    private async UniTaskVoid TriggerChangeVassal()
    {
        _justChangeVassal = true;
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        _justChangeVassal = false;
    }
}
