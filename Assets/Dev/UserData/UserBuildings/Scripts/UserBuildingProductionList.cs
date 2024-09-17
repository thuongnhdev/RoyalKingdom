using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "UserBuildingProduction", menuName = "Uniflow/User/BuildingProduction")]
public class UserBuildingProductionList : ScriptableObject
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserBuildingList _userBuildings;

    [Header("Inspec")]
    [SerializeField]
    private List<UserBuildingProduction> _userBuildingProductionList;
    public List<UserBuildingProduction> BuildingProductionList => _userBuildingProductionList;
    // key = buildingObjectId
    Dictionary<int, UserBuildingProduction> _productionDict = new();
    Dictionary<int, UserBuildingProduction> ProductionDict
    {
        get
        {
            if (_productionDict.Count != _userBuildingProductionList.Count)
            {
                _productionDict.Clear();

                for (int i = 0; i < _userBuildingProductionList.Count; i++)
                {
                    var production = _userBuildingProductionList[i];
                    _productionDict[production.buildingObjectId] = production;
                }
            }

            return _productionDict;
        }
    }

    public void Init(ResponeGetProductioBuildingInfo serverData)
    {
        _userBuildingProductionList.Clear();
        _productionDict.Clear();

        int productionCount = serverData.ProductioBuildingInfoLength;
        for (int i = 0; i < productionCount; i++)
        {
            var productionData = serverData.ProductioBuildingInfo(i).Value;
            UserBuildingProduction production = new(productionData);
            production.buildingId = _userBuildings.GetBuildingId(production.buildingObjectId);
            _userBuildingProductionList.Add(production);
            _productionDict.Add(production.buildingObjectId, production);
        }
    }

    public int GetCurrentProductId(int buildingObjId)
    {
        var production = GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return 0;
        }

        return production.CurrentProductId;
    }

    public int GetPriority(int buildingObjId)
    {
        var production = GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return 1;
        }

        return production.priority;
    }

    public UserBuildingProduction GetBuildingProduction(int buildingObjectId)
    {
        ProductionDict.TryGetValue(buildingObjectId, out var production);
        if (production == null)
        {
            Logger.Log($"<color=green>Building [{buildingObjectId}] is not a Production Building</color>");
        }

        return production;
    }

    public void UpdateBuildingProduction(UserBuildingProduction productionInfo)
    {
        if (!ProductionDict.ContainsKey(productionInfo.buildingObjectId))
        {
            _userBuildingProductionList.Add(productionInfo);
        }

        _productionDict[productionInfo.buildingObjectId] = productionInfo;
    }

    public int GetAvailableSlotCount(int buildingObjId)
    {
        var production = GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return 0;
        }

        return 10 - production.itemQueue.Count;
    }

    public int GetProductAtQueueIndex(int buildingObjId, int index)
    {
        var production = GetBuildingProduction(buildingObjId);
        if (production == null || production.itemQueue.Count == 0)
        {
            return 0;
        }

        return production.itemQueue[index].productId;
    }

    public float GetCurrentProductRate(int buildingObjId)
    {
        var production = GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return 0f;
        }

        return production.productRate;
    }

    public bool GetQueueRepeat(int buildingObjId)
    {
        var production = GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return false;
        }

        return production.queueRepeat;
    }

    public bool TryTakeACompleteProduct(int buildingObjId, out UserBuildingProduction.CompleteProduct product)
    {
        product = default;
        var production = GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return false; 
        }

        var completeProducts = production.completedProducts;
        if (completeProducts.Count == 0)
        {
            return false;
        }

        product = completeProducts[0];
        completeProducts.RemoveAt(0);

        return true;
    }

    private void EditorOnly_SaveUserBuildingProduction()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

    private void OnDisable()
    {
        EditorOnly_SaveUserBuildingProduction();
    }

}

[System.Serializable]
public class UserBuildingProduction
{
    public int buildingObjectId;
    public int buildingId;
    public float productRate;
    public int currentTaskId;
    public int priority = 1;
    public int CurrentProductId
    {
        get
        {
            if (itemQueue.Count == 0)
            {
                return 0;
            }

            return itemQueue[0].productId;
        }
    }
    public QueuedProduct CurrentProduct
    {
        get
        {
            if (itemQueue.Count == 0)
            {
                return null;
            }

            return itemQueue[0];
        }
    }
    public float currentProgress;
    public bool queueRepeat;
    public bool considerPrefix = true;
    public List<ResourcePoco> currentMaterials = new();
    public List<ItemWithStar> currentMatStar = new();

    public List<QueuedProduct> itemQueue = new();

    public List<CompleteProduct> completedProducts= new();

    [System.Serializable]
    public struct CompleteProduct
    {
        public int productId;
        public RandomOption randomOptions;
    }

    [System.Serializable]
    public class QueuedProduct
    {
        public int productId;
        public List<ItemWithStar> materialStar = new();

        public QueuedProduct(int productId, List<ItemWithStar> materialStar)
        {
            this.productId = productId;
            this.materialStar = materialStar;
        }
    }

    public UserBuildingProduction() { }
    public UserBuildingProduction(ProductioBuildingInfo serverData)
    {
        buildingObjectId = serverData.IdBuildingProductionPlayer;
        productRate = serverData.ProductRate;
        currentTaskId = serverData.IdCurrentTask;
        priority = serverData.Priority;
        currentProgress = serverData.CurrentProgress;
        queueRepeat = serverData.QueueRepeat;
        considerPrefix = serverData.CosiderRepeat;

        int currentMatCount = serverData.CurrentItemMaterialBuildingInfoLength;
        for (int i = 0; i < currentMatCount; i++)
        {
            var mat = serverData.CurrentItemMaterialBuildingInfo(i).Value;
            ResourcePoco matPoco = new() { itemId = mat.IdItem, itemCount = mat.Count };
            if (ResourcePoco.IsZero(matPoco))
            {
                continue;
            }
            currentMaterials.Add(matPoco);
        }

        int currentMatStarCount = serverData.ItemMaterialStarBuildingInfoLength;
        for (int i = 0; i < currentMatStarCount; i++)
        {
            var matStarData = serverData.ItemMaterialStarBuildingInfo(i);
            var matStar = new ItemWithStar(matStarData.Value);
            if (matStar.itemId <= 0)
            {
                continue;
            }
            currentMatStar.Add(matStar);
        }

        int queueCount = serverData.ItemQueueBuildingInfoLength;
        for (int i = 0; i < queueCount; i++)
        {
            var queuedProductData = serverData.ItemQueueBuildingInfo(i).Value;
            int matCount = queuedProductData.ItemMaterialStarQueueLength;

            List<ItemWithStar> materials = new();
            for (int j = 0; j < matCount; j++)
            {
                var matStar = new ItemWithStar(queuedProductData.ItemMaterialStarQueue(j).Value);
                if (matStar.itemId <= 0)
                {
                    continue;
                }
                materials.Add(matStar);
            }

            QueuedProduct queuedProduct = new(queuedProductData.IdProduct, materials);
            if (queuedProduct.productId <= 0)
            {
                continue;
            }
            itemQueue.Add(queuedProduct);
        }

        int completeProductCount = serverData.ItemCompleteLength;
        for (int i = 0; i < completeProductCount; i++)
        {
            var completeProductData = serverData.ItemComplete(i).Value;
            var completeOptionData = completeProductData.ItemOptionCompletePro.Value;

            CompleteProduct product = new()
            {
                productId = completeProductData.IdProduct,
                randomOptions = new() { prefix = completeOptionData.Prefix, star = completeOptionData.Star }
            };

            if (product.productId <= 0)
            {
                continue;
            }
            completedProducts.Add(product);
        }
    }
}