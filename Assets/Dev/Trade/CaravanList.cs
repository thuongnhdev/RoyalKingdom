using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CaravanList", menuName = "Uniflow/Trading/CaravanList")]
public class CaravanList : ScriptableObject
{
    [SerializeField]
    private List<Caravan> _caranvans;

    public void Init()
    {

    }

    public Caravan GetCaravan(int id)
    {
        for (int i = 0; i < _caranvans.Count; i++)
        {
            var caravan = _caranvans[i];
            if (caravan.id == id)
            {
                return caravan;
            }
        }

        Logger.LogError($"Invalid CaravanId [{id}]");
        return null;
    }

    public Caravan FindCaravanAtLand(int landId)
    {
        for (int i = 0; i < _caranvans.Count; i++)
        {
            var caravan = _caranvans[i];
            if (caravan.locationLand == landId)
            {
                return caravan;
            }
        }

        return null;
    }

    public List<int> GetAllItemPacks()
    {
        List<int> packs = new();
        for (int i = 0; i < _caranvans.Count; i++)
        {
            var caravan = _caranvans[i];
            List<int> packsPerCaravan = caravan.packs;
            for (int j = 0; j < packsPerCaravan.Count; j++)
            {
                packs.Add(packsPerCaravan[j]);
            }
        }

        return packs;
    }

    public List<int> GetItemPack(int caravanId)
    {
        var caravan = GetCaravan(caravanId);
        if (caravan == null)
        {
            return new(0);
        }

        return caravan.packs;
    }

    public List<ResourcePoco> GetStorage(int caravanId)
    {
        var caravan = GetCaravan(caravanId);
        if (caravan == null)
        {
            return null;
        }

        return caravan.storages;
    }

    public List<ItemWithRandomOption> GetStorageRandomOptions(int caravanId)
    {
        var caravan = GetCaravan(caravanId);
        if (caravan == null)
        {
            return null;
        }

        return caravan.storageRandomOptions;
    }

    public int GetItemCount(int caranvanId, int itemId)
    {
        var storage = GetStorage(caranvanId);
        if (storage == null || storage.Count == 0)
        {
            return 0;
        }

        int itemCount = 0;
        for (int i = 0; i < storage.Count; i++)
        {
            var item = storage[i];
            if (item.itemId == itemId)
            {
                itemCount += item.itemCount;
            }
        }

        return itemCount;
    }

    public List<RandomOption> GetOptionsOfItem(int caravanId, int itemId)
    {
        var allItemsWithOption = GetStorageRandomOptions(caravanId);
        if (allItemsWithOption == null)
        {
            return null;
        }

        for (int i = 0; i < allItemsWithOption.Count; i++)
        {
            var itemWithOption = allItemsWithOption[i];
            if (itemWithOption.itemId == itemId)
            {
                return itemWithOption.randomOptions;
            }
        }

        return null;
    }

    public int GetItemCountWithOption(int caravanId, int itemId, RandomOption option)
    {
        var itemOptions = GetOptionsOfItem(caravanId, itemId);
        
        if (option.Equals(RandomOption.NoAddition))
        {
            int totalItem = GetItemCount(caravanId, itemId);
            if (itemOptions == null)
            {
                return totalItem;
            }

            return totalItem - itemOptions.Count;
        }

        if (itemOptions == null || itemOptions.Count == 0)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < itemOptions.Count; i++)
        {
            if (itemOptions[i].Equals(option))
            {
                count++;
            }
        }

        return count;
    }

    public void SubItem(int caravanId, ResourcePoco item, RandomOption option = default)
    {
        var storage = GetStorage(caravanId);
        for (int i = 0; i < storage.Count; i++)
        {
            ResourcePoco storageItem = storage[i];
            if (storageItem.itemId != item.itemId)
            {
                continue;
            }

            storageItem.itemCount -= item.itemCount;
            storage[i] = storageItem;
            break;
        }

        if (option.Equals(RandomOption.NoAddition))
        {
            return;
        }

        var optionStorage = GetStorageRandomOptions(caravanId);
        for (int i = 0; i < optionStorage.Count; i++)
        {
            var optionsItem = optionStorage[i];
            if (optionsItem.itemId != item.itemId)
            {
                continue;
            }

            var options = optionsItem.randomOptions;
            for (int j = 0; j < item.itemCount; j++)
            {
                options.Remove(option);
            }

            break;
        }
    }

    public void AddItem(int caravanId, ResourcePoco item, RandomOption option = default)
    {
        var storage = GetStorage(caravanId);
        for (int i = 0; i < storage.Count; i++)
        {
            ResourcePoco storageItem = storage[i];
            if (storageItem.itemId != item.itemId)
            {
                continue;
            }

            storageItem.itemCount += item.itemCount;
            storage[i] = storageItem;
            break;
        }

        if (option.Equals(RandomOption.NoAddition))
        {
            return;
        }

        var optionStorage = GetStorageRandomOptions(caravanId);
        for (int i = 0; i < optionStorage.Count; i++)
        {
            var optionsItem = optionStorage[i];
            if (optionsItem.itemId != item.itemId)
            {
                continue;
            }

            var options = optionsItem.randomOptions;
            for (int j = 0; j < item.itemCount; j++)
            {
                options.Add(option);
            }

            break;
        }
    }
}
