using System.Collections;
using System.Collections.Generic;


public class ItemHelper
{
    public static int GetGroupItemId(int itemId)
    {
        return itemId / 10000 * 10000;
    }

    public static bool IsGroupItemId(int itemId)
    {
        if (itemId <= 0)
        {
            return false;
        }

        return itemId % 10000 == 0;
    }

    public static bool IsCurrencyItem(int itemId)
    {
        return itemId / 10000 * 10000 == 990000;
    }

    public static bool IsResourcesEmpty(List<ResourcePoco> resources)
    {
        if (resources == null || resources.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < resources.Count; i++)
        {
            if (!ResourcePoco.IsZero(resources[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static int CountResource(List<ResourcePoco> resources)
    {
        if (resources == null || resources.Count == 0)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < resources.Count; i++)
        {
            var resource = resources[i];
            if (ResourcePoco.IsZero(resource))
            {
                continue;
            }

            count += resource.itemCount;
        }

        return count;
    }

    public static bool CompareResources(List<ResourcePoco> a, List<ResourcePoco> b)
    {
        if (IsResourcesEmpty(a) && IsResourcesEmpty(b))
        {
            return true;
        }

        a = TrimResources(a);
        b = TrimResources(b);

        if (a.Count != b.Count)
        {
            return false;
        }

        Dictionary<int, int> itemDict = new();
        for (int i = 0; i < a.Count; i++)
        {
            itemDict[a[i].itemId] = a[i].itemCount;
        }

        for (int i = 0; i < b.Count; i++)
        {
            var bRes = b[i];
            itemDict.TryGetValue(bRes.itemId, out int aResCount);

            if (aResCount == 0 || aResCount != bRes.itemCount)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// a is considered as greater than or equals b if a has all b's item type with greater or equal number
    /// </summary>
    public static bool IsGreaterOrEquals(List<ResourcePoco> a, List<ResourcePoco> b)
    {
        if (IsResourcesEmpty(b))
        {
            return true;
        }

        if (IsResourcesEmpty(a))
        {
            return false;
        }

        Dictionary<int, int> aItemDict = new();
        for (int i = 0; i < a.Count; i++)
        {
            aItemDict[a[i].itemId] = a[i].itemCount;
        }

        for (int i = 0; i < b.Count; i++)
        {
            var bResource = b[i];
            aItemDict.TryGetValue(bResource.itemId, out int aCount);
            if (aCount < bResource.itemCount)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsSmallerOrEquals(List<ResourcePoco> a)
    {
        if (IsResourcesEmpty(a))
        {
            return true;
        }
        for(int i = 0;i< a.Count;i++)
        {
            if (a[i].itemCount > 0)
                return false;
        }
        return true;
    }

    public static bool IsGreaterOrEqualsFinish(List<ResourcePoco> a)
    {
        if (IsResourcesEmpty(a))
        {
            return true;
        }

        for (int i = 0; i < a.Count; i++)
        {
            var bResource = a[i];
            if (bResource.itemCount > 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Remove resources that have invalid Id or 0 count
    /// </summary>
    public static List<ResourcePoco> TrimResources(List<ResourcePoco> a)
    {
        List<ResourcePoco> trimmed = new();
        if (a == null || a.Count == 0)
        {
            return trimmed;
        }

        for (int i = 0; i < a.Count; i++)
        {
            var resource = a[i];
            if (resource.itemId == 0 || resource.itemCount <= 0)
            {
                continue;
            }

            trimmed.Add(resource);
        }

        return trimmed;
    }

    public static List<ResourcePoco> AddResources(List<ResourcePoco> currents, List<ResourcePoco> adds)
    {
        if (adds == null || adds.Count == 0)
        {
            return new List<ResourcePoco>(currents);
        }

        if (currents == null || currents.Count == 0)
        {
            return new List<ResourcePoco>(adds);
        }

        Dictionary<int, ResourcePoco> resourceDict = new();

        for (int i = 0; i < currents.Count; i++)
        {
            resourceDict[currents[i].itemId] = currents[i];
        }

        for (int i = 0; i < adds.Count; i++)
        {
            var add = adds[i];
            resourceDict.TryGetValue(add.itemId, out var current);
            if (ResourcePoco.IsZero(current))
            {
                resourceDict[add.itemId] = add;
                continue;
            }

            current.itemCount += add.itemCount;
            resourceDict[current.itemId] = current;
        }

        List<ResourcePoco> result = new();
        foreach (var resource in resourceDict)
        {
            result.Add(resource.Value);
        }

        return result;
    }

    public static List<ResourcePoco> AddResource(List<ResourcePoco> currents, ResourcePoco add)
    {
        if (ResourcePoco.IsZero(add))
        {
            return new List<ResourcePoco>(currents);
        }

        var result = new List<ResourcePoco>();
        if (currents == null)
        {
            result.Add(add);
            return result;
        }

        result.AddRange(currents);
        for (int i = 0; i < result.Count; i++)
        {
            var current = result[i];
            if (current.itemId == add.itemId)
            {
                current.itemCount += add.itemCount;

                result[i] = current;
                return result;
            }
        }

        result.Add(add);
        return result;
    }

    public static List<ResourcePoco> SubResources(List<ResourcePoco> currents, List<ResourcePoco> subs)
    {
        if (IsResourcesEmpty(subs) || IsResourcesEmpty(currents))
        {
            return new List<ResourcePoco>(currents);
        }

        Dictionary<int, ResourcePoco> resourceDict = new();

        for (int i = 0; i < currents.Count; i++)
        {
            resourceDict[currents[i].itemId] = currents[i];
        }

        for (int i = 0; i < subs.Count; i++)
        {
            var sub = subs[i];
            resourceDict.TryGetValue(sub.itemId, out var current);
            current.itemCount = current.itemCount < sub.itemCount ? 0 : current.itemCount - sub.itemCount;

            resourceDict[current.itemId] = current;
        }

        List<ResourcePoco> result = new();
        foreach (var resource in resourceDict)
        {
            var resourceValue = resource.Value;
            if (ResourcePoco.IsZero(resourceValue))
            {
                continue;
            }
            result.Add(resourceValue);
        }

        return result;
    }

    public static List<ResourcePoco> MultiplyResources(List<ResourcePoco> currents, float multiplier)
    {
        List<ResourcePoco> result = new();

        if (multiplier == 0f || IsResourcesEmpty(currents))
        {
            return result;
        }

        result.AddRange(currents);
        if (multiplier == 1f )
        {
            return result;
        }

        for (int i = 0; i < result.Count; i++)
        {
            var current = result[i];
            current.itemCount = (int)(current.itemCount * multiplier);

            result[i] = current;
        }

        return result;
    }

    public static bool CanProduceItemAtLand(LandStaticInfo land, (List<int> terrains, int religion, int culture) condition)
    {
        bool validTerrain = condition.terrains == null || condition.terrains.Count == 0 || condition.terrains.Contains(land.terrainId);
        bool validReligion = condition.religion == 0 || condition.religion == land.religion;
        bool validCulture = condition.culture == 0 || condition.culture == land.culture;

        return validTerrain && validReligion && validCulture;
    }

    public static string FormatItemCostColor(int current, int destination)
    {
        string costText = $"{current}/{destination}";
        if (current < destination)
        {
            costText = $"<color=\"red\">{costText}";
        }

        return costText;
    }

}
