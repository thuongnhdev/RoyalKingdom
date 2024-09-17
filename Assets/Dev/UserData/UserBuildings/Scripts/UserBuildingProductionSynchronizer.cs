using Cysharp.Threading.Tasks;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBuildingProductionSynchronizer : MonoBehaviour
{
    [SerializeField]
    private ApiList _apis;
    [Header("Reference - Read/Write")]
    [SerializeField]
    private UserBuildingProductionList _userBuildingProductions;
    [SerializeField]
    private UserItemStorage _userItems;

    [Header("Reference - Read Data")]
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private BuildingProductDescriptionList _productionDes;
    [SerializeField]
    private ProductFormulaList _itemFormulas;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private UserVassalSOList _userVassals;
    [SerializeField]
    private PrefixList _prefixes;
    [SerializeField]
    private WarehousesCapacity _warehousesCapacity;

    [Header("Reference - Read Variables")]
    [SerializeField]
    private IntegerVariable _selectedBuildingId;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;
    [SerializeField]
    private IntegerVariable _selectedProductId;
    [SerializeField]
    private IntegerVariable _priority;
    [SerializeField]
    private IntegerVariable _assignedWorkerNumber;
    [SerializeField]
    private IntegerVariable _removedProductQueueIndex;
    [SerializeField]
    private IntegerVariable _oldProductQueueIndex;
    [SerializeField]
    private IntegerVariable _newProductQueueIndex;
    [SerializeField]
    private ProductionMaterialsStar _materialsStarConfig;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onQueueAsManyAsProductsPossible;
    [SerializeField]
    private GameEvent _onABuildingDataAdded;
    [SerializeField]
    private GameEvent _onABuildingDataDestroyed;
    [SerializeField]
    private GameEvent _onRequestCompleteItem;
    [SerializeField]
    private GameEvent _onRequestCompleteUnitTraining;
    [SerializeField]
    private GameEvent _onRequestCompleteTechResearch;
    [SerializeField]
    private GameEvent _onRequestUpdateProduction;
    [SerializeField]
    private GameEvent _onChangedVassal;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onStartedNewProduction;
    [SerializeField]
    private GameEvent _onRequestSwitchBuildingStatus;
    [SerializeField]
    private GameEvent _onItemQueueFull;
    [SerializeField]
    private GameEvent _onUserBuildingProductionDataChanged;
    [SerializeField]
    private GameEvent _onCompletedItem;
    [SerializeField]
    private GameEvent _onCanceledCurrentProduct;
    [SerializeField]
    private GameEvent _askForStorageReservation;

    public void FetchAllProductionsInfo()
    {
        Async_FetchBuildingProductions().Forget();
    }

    public void CheckStuckingAndResume()
    {
        List<UserBuildingProduction> productions = _userBuildingProductions.BuildingProductionList;
        Dictionary<ItemType, int> warehouseCapacityDict = new(); // key = itemType, value = availableSlot

        for (int i = 0; i < productions.Count; i++)
        {
            var production = productions[i];
            int currentProduct = production.CurrentProductId;
            if (currentProduct == 0)
            {
                continue;
            }

            if (production.currentProgress < _itemFormulas.GetTimeCost(currentProduct))
            {
                continue;
            }

            ItemType type = _items.GetItemType(currentProduct);
            if (!warehouseCapacityDict.ContainsKey(type))
            {
                int available = _warehousesCapacity.GetSlotAvailableForItem(currentProduct);
                warehouseCapacityDict.Add(type, available);
            }

            if (warehouseCapacityDict[type] - 1 < 0)
            {
                continue;
            }

            warehouseCapacityDict[type] -= 1;
            HandleCompletedProduct(production.buildingObjectId);
        }
    }

    private async UniTaskVoid Async_FetchBuildingProductions()
    {
        var requestBody = UserBuildingNetworkHelper.CreateFBGetUserBuildingBody(-1); // All buildings
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeGetProductioBuildingInfo>(_apis.GetProductionInfo, requestBody);
        if (response.ByteBuffer == null || response.ApiResultcode != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to fetch Building Productions!");
            return;
        }

        _userBuildingProductions.Init(response);
        Logger.Log("Fetched Building Productions!");
    }

    public void QueueAProduct()
    {
        if (_selectedProductId.Value == 0)
        {
            return;
        }

        var production = _userBuildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }

        var itemQueue = production.itemQueue;
        if (itemQueue.Count == 10)
        {
            _onItemQueueFull.Raise();
            return;
        }

        UserBuildingProduction.QueuedProduct product = new(_selectedProductId.Value, _materialsStarConfig.materialsStars);
        ServerSync_QueueAProduct(production, product).Forget();
    }
    private async UniTaskVoid ServerSync_QueueAProduct(UserBuildingProduction production, UserBuildingProduction.QueuedProduct product)
    {
        var requestBody = UserBuildingNetworkHelper.CreateQueueAProductRequestBody(production.buildingObjectId, product);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeQueueProduct>(_apis.QueueAProduct, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to queue a product!");
            return;
        }

        LocalSync_QueueAProduct(production, product);
        Logger.Log("Successful to queue a product!");
    }
    private void LocalSync_QueueAProduct(UserBuildingProduction production, UserBuildingProduction.QueuedProduct product)
    {
        var itemQueue = production.itemQueue;
        itemQueue.Add(product);

        ConsiderBuildingNextStatus(production);

        if (itemQueue.Count == 1)
        {
            _onStartedNewProduction.Raise(production.buildingObjectId);
        }

        _onUserBuildingProductionDataChanged.Raise(production.buildingObjectId);
    }

    public async UniTaskVoid QueueProducts(int productCount)
    {
        if (_selectedProductId.Value == 0)
        {
            return;
        }

        var production = _userBuildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }

        UserBuildingProduction.QueuedProduct product = new(_selectedProductId.Value, _materialsStarConfig.materialsStars);
        await ServerSync_QueueProducts(production, product);
        LocalSync_QueueProducts(production, product, productCount);
    }
    private async UniTask ServerSync_QueueProducts(UserBuildingProduction production, UserBuildingProduction.QueuedProduct product)
    {
        var requestBody = UserBuildingNetworkHelper.CreateQueueAProductRequestBody(production.buildingObjectId, product);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeChooseManyItem>(_apis.QueueAsManyAsProductsPossible, requestBody);
        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to queue many products!");
            return;
        }
        Logger.Log("Successful to queue many products!");
    }
    private void LocalSync_QueueProducts(UserBuildingProduction production, UserBuildingProduction.QueuedProduct product, int productCount)
    {
        var itemQueue = production.itemQueue;
        bool startProduction = itemQueue.Count == 0;

        for (int i = 0; i < productCount; i++)
        {
            if (itemQueue.Count == 10)
            {
                break;
            }
            itemQueue.Add(product);
        }

        ConsiderBuildingNextStatus(production);
        _onUserBuildingProductionDataChanged.Raise(production.buildingObjectId);
        if (startProduction)
        {
            _onStartedNewProduction.Raise(production.buildingObjectId);
        }
    }

    public void RemoveAQueuedItem()
    {
        int removedIndex = _removedProductQueueIndex.Value;
        if (removedIndex == -1)
        {
            return;
        }

        var production = _userBuildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }

        ServerSync_RemoveAQueuedProduct(production.buildingObjectId, removedIndex).Forget();
        production.itemQueue.RemoveAt(removedIndex);

        ConsiderBuildingNextStatus(production);
        _onUserBuildingProductionDataChanged.Raise(production.buildingObjectId);
    }
    private async UniTaskVoid ServerSync_RemoveAQueuedProduct(int buildingOjbId, int queueIndex)
    {
        byte[] body = UserBuildingNetworkHelper.CreateRemoveQueuedItemRequestBody(buildingOjbId, queueIndex);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeRemoveQueue>(_apis.RemoveAQueueProduct, body);

        if (response.ByteBuffer == null)
        {
            return;
        }

        Logger.Log("Request Remove A Queued Product Successfully!");
    }    

    public void CancelCurrentProduction()
    {
        var production = _userBuildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }

        var currentMat = production.currentMaterials;
        if (!_userItems.TryReserve(currentMat))
        {
            return;
        }

        ServerSync_CancelCurrentProduction(production).Forget();
    }
    private async UniTaskVoid ServerSync_CancelCurrentProduction(UserBuildingProduction production)
    {
        var itemQueue = production.itemQueue;
        if (itemQueue.Count == 0)
        {
            return;
        }

        byte[] body = UserBuildingNetworkHelper.CreateCancelCurrentProductionRequestBody(production.buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeCancelProduct>(_apis.CancelCurrentProduct, body, blockUi: true);
        if (response.ByteBuffer == null)
        {
            return;
        }

        production.currentProgress = 0f;
        itemQueue.RemoveAt(0);
        RefundProductMaterial(production);

        ConsiderBuildingNextStatus(production);
        _onUserBuildingProductionDataChanged.Raise(_selectedBuildingObjId.Value);
        _onCanceledCurrentProduct.Raise(_selectedBuildingObjId.Value);
        Logger.Log("Cancel current production successfully!");
    }

    public void UpdateQueueRepeat()
    {
        var production = _userBuildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }

        production.queueRepeat = !production.queueRepeat;
        ServerSync_UpdateQueueRepeat(production.buildingObjectId, production.queueRepeat).Forget();
    }
    private async UniTaskVoid ServerSync_UpdateQueueRepeat(int buildingObjId, bool repeat)
    {
        var requestbody = UserBuildingNetworkHelper.CreateRepeatQueueRequestBody(buildingObjId, repeat);

        var rd = RequestDispatcher.Instance;
        rd.SetThrottleForRequest(_apis.QueueRepeat, 2f);
        var response = await rd.SendPostRequest<ResponeRepeatQueue>(_apis.QueueRepeat, requestbody);
        
        if (response.ByteBuffer == null)
        {
            return;
        }

        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to Update Queue Repeat!");
            return;
        }

        Logger.Log("Successful to Update Queue Repeat");
    }

    public void ChangeQueueOrder()
    {
        var production = _userBuildingProductions.GetBuildingProduction(_selectedBuildingObjId.Value);
        if (production == null)
        {
            return;
        }

        ServerSync_ChangeQueueOrder(production).Forget();

        var queue = production.itemQueue;

        int oldIndex = _oldProductQueueIndex.Value;
        int newIndex = Mathf.Clamp(_newProductQueueIndex.Value, 1, queue.Count);

        var movedProduct = queue[oldIndex];

        if (newIndex < oldIndex)
        {
            queue.RemoveAt(oldIndex);
            queue.Insert(newIndex, movedProduct);
        }
        else
        {
            queue.Insert(newIndex, movedProduct);
            queue.RemoveAt(oldIndex);
        }
        _onUserBuildingProductionDataChanged.Raise(production.buildingObjectId);
    }
    private async UniTaskVoid ServerSync_ChangeQueueOrder(UserBuildingProduction production)
    {
        var requestBody = UserBuildingNetworkHelper.CreateChangeQueueOrderRequestBody(production.buildingObjectId, production.itemQueue);

        var rd = RequestDispatcher.Instance;
        rd.SetThrottleForRequest(_apis.ChangeQueueOrder, 2f);
        var response = await rd.SendPostRequest<ResponeChangeQueueIndex>(_apis.ChangeQueueOrder, requestBody);
        if (response.ByteBuffer == null)
        {
            return;
        }

        if (response.ApiResultCode != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to Change Product Queue Order");
            return;
        }

        Logger.Log("Successful to Change Product Queue Order");
    }

    private void QueueAsManyAsProductsPossible(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }
        int productCount = (int)args[0];
        QueueProducts(productCount).Forget();
    }

    private void UpdateBuildingProduction(object[] args)
    {
        // TODO server logic here
        
        if (args.Length == 0)
        {
            return;
        }

        var newProductionInfo = (UserBuildingProduction)args[0];
        if (newProductionInfo == null)
        {
            return;
        }

        _userBuildingProductions.UpdateBuildingProduction(newProductionInfo);

        _onUserBuildingProductionDataChanged.Raise(newProductionInfo.buildingObjectId);
    }

    private void HandleCompletedProduct(params object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)args[0];
        var production = _userBuildingProductions.GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return;
        }

        int currentProduct = production.CurrentProductId;
        bool warehouseAvaiable = _warehousesCapacity.CanAddItem(currentProduct, 1);
        if (!warehouseAvaiable)
        {
            return;
        }

        ServerSync_CompleteAProduct(production).Forget();
    }
    private async UniTaskVoid ServerSync_CompleteAProduct(UserBuildingProduction production)
    {
        byte[] body = UserBuildingNetworkHelper.CreateCompleteProductRequestBody(production.buildingObjectId);
        var response = await RequestDispatcher.Instance.SendPostRequest<ResponeCompleteProduct>(_apis.CompleteAProduction, body);
        if (response.ByteBuffer == null)
        {
            return;
        }

        int completeProduct = production.CurrentProductId;
        var completeRandomOption = RemoveAfterServer_CalculateAp(production);

        RearrangeQueue(production);

        production.considerPrefix = true;
        production.completedProducts.Add(new() { productId = completeProduct, randomOptions = completeRandomOption });

        production.currentTaskId = -1;
        production.currentProgress = 0f;
        production.currentMaterials.Clear();
        production.currentMatStar.Clear();

        _onCompletedItem.Raise(production.buildingObjectId);
        _askForStorageReservation?.Raise(completeProduct, 1);

        ConsiderBuildingNextStatus(production);

        _onUserBuildingProductionDataChanged.Raise(production.buildingObjectId);

        Logger.Log("Request Complete Product Successfully!");
    }

    private void RearrangeQueue(UserBuildingProduction production)
    {
        var itemQueue = production.itemQueue;

        var completedProduct = itemQueue[0];
        if (production.queueRepeat)
        {
            itemQueue.Add(completedProduct);
        }

        itemQueue.RemoveAt(0);

        if (itemQueue.Count != 0)
        {
            _onStartedNewProduction.Raise(production.buildingObjectId);
        }
    }

    private RandomOption RemoveAfterServer_CalculateAp(UserBuildingProduction production)
    {
        RandomOption ap = new();

        bool prefixAppear = RemoveAfterServer_IsPrefixAppear(production);
        if (prefixAppear)
        {
            int prefix = RemoveAfterServer_CalculatePrefix(_userBuildings.GetVassalInCharge(production.buildingObjectId));
            ap.prefix = prefix;
        }

        List<ItemWithStar> materialsStar = production.currentMatStar;
        if (materialsStar == null || materialsStar.Count == 0)
        {
            return ap;
        }

        int star = RemoveAfterServer_CalculateStar(materialsStar);
        ap.star = star;

        return ap;
    }

    private bool RemoveAfterServer_IsPrefixAppear(UserBuildingProduction production)
    {
        if (!production.considerPrefix)
        {
            return false;
        }

        bool prefixable = _items.IsPrefixable(production.CurrentProductId);
        if (!prefixable)
        {
            return false;
        }

        float lucky = _userVassals.GetLucky(_userBuildings.GetVassalInCharge(production.buildingObjectId));

        return Random.Range(0f, 1f) < lucky;
    }

    private int RemoveAfterServer_CalculateStar(List<ItemWithStar> itemsWithStar) 
    {
        float[][] starTable = {new float[] { 0.94f, 0.04f, 0.018f, 0.002f },
                               new float[] { 0f, 0.94f, 0.04f, 0.02f },
                               new float[] { 0f, 0f, 0.94f, 0.06f },
                               new float[] { 0f, 0f, 0f, 1f }};

        int[] starsCount = new int[4];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < itemsWithStar.Count; j++)
            {
                int starCount = itemsWithStar[j].stars[i];
                starsCount[i] += starCount;
            }
        }

        float[] starsRatio = new float[4];
        for (int i = 0; i < starsCount.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                starsRatio[j] += starTable[i][j] * starsCount[i];
            }
        }

        int star = MathUtils.RandomChooseFrom(starsRatio);
        if (star == -1)
        {
            return 0;
        }

        return star;
    }

    private int RemoveAfterServer_CalculatePrefix(int vassal)
    {
        return _userVassals.GetRandomPrefixBasedOnStats(vassal);
    }

    private void AddABuildingProduction(params object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        // TODO Server logic here

        int buildingObjectId = (int)args[0];


        if (!_productionDes.HasProduct(_selectedBuildingId.Value))
        {
            return;
        }

        _userBuildingProductions.UpdateBuildingProduction(new UserBuildingProduction
        {
            buildingObjectId = buildingObjectId,
            buildingId = _selectedBuildingId.Value,
        });
    }

    private void RemoveABuildingProduction(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        // TODO Server logic here

        int buildingObjectId = (int)args[0];

        var buildingProduction = _userBuildingProductions.GetBuildingProduction(buildingObjectId);
        if (buildingProduction == null)
        {
            return;
        }

        buildingProduction.buildingObjectId = 0;

        _userBuildingProductions.UpdateBuildingProduction(buildingProduction);
    }

    private void ConsiderBuildingNextStatus(UserBuildingProduction production)
    {
        var itemQueue = production.itemQueue;

        if (itemQueue.Count == 0)
        {
            _onRequestSwitchBuildingStatus.Raise(production.buildingObjectId, BuildingStatus.Idle);
            return;
        }

        List<ResourcePoco> nextProductCost = _itemFormulas.GetResourceCost(itemQueue[0].productId);
        if (!ItemHelper.IsGreaterOrEquals(production.currentMaterials, nextProductCost))
        {
            _onRequestSwitchBuildingStatus.Raise(production.buildingObjectId, BuildingStatus.WaitingForProductResource);
            return;
        }

        _onRequestSwitchBuildingStatus.Raise(production.buildingObjectId, BuildingStatus.Producing);
    }

    private void RefundProductMaterial(UserBuildingProduction production)
    {
        List<ResourcePoco> currentMat = production.currentMaterials;
        List<ItemWithStar> currentMatWithStar = production.currentMatStar;
        for (int i = 0; i < currentMat.Count; i++)
        {
            _userItems.Add(currentMat[i]);
        }
        for (int i = 0; i < currentMatWithStar.Count; i++)
        {
            var itemWithStar = currentMatWithStar[i];
            var stars = itemWithStar.stars;
            for (int j = 0; j < stars.Length; j++)
            {
                if (stars[j] <= 0)
                {
                    continue;
                }

                _userItems.AddOptionForItem(itemWithStar.itemId, stars[j], new() { prefix = 0, star = j });
            }
        }

        currentMat.Clear();
        currentMatWithStar.Clear();
    }

    private void TerminatePrefixIfChangeVassalOnProducing(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int buildingObjId = (int)args[0];
        var building = _userBuildings.GetBuilding(buildingObjId);
        if (building.status != BuildingStatus.Producing &&
            building.status != BuildingStatus.WaitingForProductResource)
        {
            return;
        }

        var production = _userBuildingProductions.GetBuildingProduction(buildingObjId);
        if (production == null)
        {
            return;
        }

        production.considerPrefix = false;
        Logger.Log($"Product {production.CurrentProductId} of building {production.buildingObjectId} will not be added Prefix because of vassal changing");
    }

    private void OnEnable()
    {
        _onChangedVassal.Subcribe(TerminatePrefixIfChangeVassalOnProducing);

        _onQueueAsManyAsProductsPossible.Subcribe(QueueAsManyAsProductsPossible);

        _onABuildingDataAdded.Subcribe(AddABuildingProduction);
        _onABuildingDataDestroyed.Subcribe(RemoveABuildingProduction);

        _onRequestCompleteItem.Subcribe(HandleCompletedProduct);
        _onRequestCompleteUnitTraining.Subcribe(HandleCompletedProduct);
        _onRequestCompleteTechResearch.Subcribe(HandleCompletedProduct);

        _onRequestUpdateProduction.Subcribe(UpdateBuildingProduction);
    }

    private void OnDisable()
    {
        _onChangedVassal.Unsubcribe(TerminatePrefixIfChangeVassalOnProducing);

        _onQueueAsManyAsProductsPossible.Unsubcribe(QueueAsManyAsProductsPossible);

        _onABuildingDataAdded.Unsubcribe(AddABuildingProduction);
        _onABuildingDataDestroyed.Unsubcribe(RemoveABuildingProduction);

        _onRequestCompleteItem.Unsubcribe(HandleCompletedProduct);
        _onRequestCompleteUnitTraining.Unsubcribe(HandleCompletedProduct);
        _onRequestCompleteTechResearch.Unsubcribe(HandleCompletedProduct);

        _onRequestUpdateProduction.Unsubcribe(UpdateBuildingProduction);
    }
}
