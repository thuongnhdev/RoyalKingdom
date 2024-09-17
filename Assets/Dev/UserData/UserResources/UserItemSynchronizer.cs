using Cysharp.Threading.Tasks;
using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserItemSynchronizer : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private UserItemStorage _userItems;
    [SerializeField]
    private WarehousesCapacity _warehouses;

    [Header("Reference - Read")]
    [SerializeField]
    private ApiList _apis;
    [SerializeField]
    private ItemTypeList _itemTypes;
    [SerializeField]
    private ItemList _items;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onAskedForStorageReservation;
    [SerializeField]
    private GameEvent _onWarehouseCapacityChanged;
    [SerializeField]
    private GameEvent _onInventoryChanged;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onOutOfWarehouseCapacity;


    private async UniTaskVoid ServerSync_FetchInventory()
    {
        byte[] body = UserItemNetworkHelper.CreateGetInventoryRequestBody();

        var response = await RequestDispatcher.Instance.SendPostRequest<GetInfoInventory>(_apis.GetInventory, body);
        if (response.ByteBuffer == null || response.ApiResult != (int)ApiResultCode.SUCCESS)
        {
            Logger.LogError("Failed to get Inventory");
            return;
        }

        _userItems.RefreshItems(response);
    }

    private void ReserveStorage(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int itemId = (int)args[0];
        int count = (int)args[1];
        bool canReserve = _userItems.TryReserve(itemId, count);
        if (!canReserve)
        {
            Logger.LogWarning($"Failed to reserver item [{itemId}] count [{count}]");
        }
    }

    private void CheckCapacityWhenWarehouseChanged(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int warehouseId = (int)args[0];
        int capacity = _warehouses.GetCapacity(warehouseId);
        int loadedAndReserved = _userItems.GetCountBasedOnContainWarehouse(warehouseId, true);

        if (capacity <= loadedAndReserved)
        {
            _onOutOfWarehouseCapacity.Raise(warehouseId);
        }
    }

    private void CheckCapacityWhenInventoryChanged(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int itemId = (int)args[0];
        ItemType type = _items.GetItemType(itemId);
        int warehouseId = _itemTypes.GetContainedWarehouse(type);
        int capacity = _warehouses.GetCapacity(warehouseId);
        int loadedAndReserved = _userItems.GetCountBasedOnContainWarehouse(warehouseId, true);

        if (capacity <= loadedAndReserved)
        {
            _onOutOfWarehouseCapacity.Raise(warehouseId);
        }
    }

    private void OnEnable()
    {
        ServerSync_FetchInventory().Forget();
        _onAskedForStorageReservation.Subcribe(ReserveStorage);
        _onWarehouseCapacityChanged.Subcribe(CheckCapacityWhenWarehouseChanged);
        _onInventoryChanged.Subcribe(CheckCapacityWhenInventoryChanged);
    }

    private void OnDisable()
    {
        _onAskedForStorageReservation.Unsubcribe(ReserveStorage);
        _onWarehouseCapacityChanged.Unsubcribe(CheckCapacityWhenWarehouseChanged);
        _onInventoryChanged.Unsubcribe(CheckCapacityWhenInventoryChanged);
    }
}
