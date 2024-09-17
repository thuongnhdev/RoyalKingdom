using Fbs;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct ResourcePoco
{
    public const int GOLD = 990099;
    public const int DIAMOND = 990100;
    public const int STOKEN = 990101;

    public static bool IsZero(ResourcePoco resource)
    {
        return resource.itemId <= 0 || resource.itemCount <= 0;
    }

    public int itemId;
    public int itemCount;
}


[System.Serializable]
public class ItemPoco
{
    public string name;
    public int itemId;
    public ItemType itemType;
    public float price;
    public ItemGrade grade;
    public bool prefixable;
}

[System.Serializable]
public class ItemTypePoco
{
    public ItemType type;
    public int order;
    public bool taxable;
    public bool usable;
    public bool disassemble;
    public bool discardable;
    public bool tradable;
    public bool superPosition;
    public int maxValue;
    public bool suppliable;
    public int warehouseId;
}

[System.Serializable]
public struct ItemGroup
{
    public string groupName;
    public int groupId;
}

[System.Serializable]
public struct RandomOption : IEquatable<RandomOption>
{
    public int prefix;
    public int star;

    public static RandomOption NoAddition
    {
        get
        {
            return new RandomOption { prefix = 0, star = 0 };
        }
    }

    public bool IsNoAddition()
    {
        return prefix == 0 && star == 0;
    }

    public bool Equals(RandomOption compareWith)
    {
        return prefix == compareWith.prefix && star == compareWith.star;
    }
}

[System.Serializable]
public class ItemWithRandomOption
{
    public int itemId;
    public List<RandomOption> randomOptions = new();
}

[System.Serializable]
public class ItemWithStar
{
    public int itemId;
    public int[] stars;

    public ItemWithStar() { }
    public ItemWithStar(ItemMaterialStar serverData)
    {
        itemId = serverData.IdItem;
        stars = new int[4];
        stars[0] = serverData.MaterialStarQueue.Value.ItemStar0;
        stars[1] = serverData.MaterialStarQueue.Value.ItemStar1;
        stars[2] = serverData.MaterialStarQueue.Value.ItemStar2;
        stars[3] = serverData.MaterialStarQueue.Value.ItemStar3;
    }
}

public enum ItemGrade
{
    None = 0,
    Common = 1,
    Uncommon = 2,
    Advanced = 3,
    Rare = 4,
    Unique = 5,
    Legendary = 6
}

public enum ItemType
{
    None = 0,
    RESOURCE = 1,
    MATERIAL = 2,
    FOOD = 3,
    PRODUCT = 4,
    ARTWORK = 5,
    EQUIPMENT = 6,
    CARAVAN = 7,
    LIVESTOCK = 8,
    TREASURY = 9,
    TOKEN = 10,
    PACKAGE = 11
}

[System.Serializable]
public struct Prefix
{
    public int id;
    public string name;
    public int relatedStat;
    public float productionRate;
    public Buff buff;
    public int buffType;
    public float buffValue;
}

public enum Buff
{
    None = -1,
    Damage = 0,
    Time = 1,
    Happy = 2,
    Defense = 3,
    MoveSpeed = 4,
    Health = 5,
    Charm = 6
}
