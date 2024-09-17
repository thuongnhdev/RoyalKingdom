using Fbs;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class LandStaticInfo
{
    public string landName;
    public int id;
    public int landGeoId;
    public int maxCity;
    [JsonIgnore]
    public List<int> cities = new(3);
    public int maxTown;
    public bool purchasable;
    public int regionalProduct;
    public int terrainId;
    public int religion;
    public int culture;
    public int race;
    public int populationGrade;
    [Newtonsoft.Json.JsonIgnore]
    public Color color;

    public LandStaticInfo() { }

    public LandStaticInfo(Land serverData)
    {
        landName = serverData.LandName;
        id = serverData.Key;
        landGeoId = serverData.GroundId;
        maxCity = serverData.MaxCity;
        maxTown = 150; // TODO RK parse data from server
        purchasable = serverData.Purchasable;
        regionalProduct = serverData.RegionalProductItem;
        terrainId = serverData.TerrainKEY;
        religion = serverData.Religion;
        culture = serverData.Culture;
        race = serverData.Race;
        populationGrade = serverData.PopulationGrade;
    }
}

[System.Serializable]
public class LandDynamicInfo
{
    public string landName;
    public int id;
    public int hp;
    public long owner;
    public string ownerName;
    public long kingdom;
    public long population;
    public int prosperity;
    public int reputation;
    public float taxRate;
    public int idParent;
    public List<long> members = new();
}

[System.Serializable]
public struct LandBattleInfo
{
    public string landName;
    public int id;
    public int win;
    public int lose;
    public int buildingRazed;
    public int razedBuilding;
    public int kills;
    public int totalLostTroops;
    public int totalTroops;
}

[System.Serializable]
public class LandTax
{
    public int id;
    public int taxedItem;
    public int buildingForItem;
    public float changeRateCooldown;
    public float minRate;
    public float maxRate;
    public TaxTarget target;
}

public enum LandAuthority
{
    LandOwner = 1,
    LandAgent = 2,
    CityOwner = 3,
    HardPlayer = 4,
    TownOwner = 5
}

public enum TaxTarget
{
    OURLAND = 0,
    OTHERLAND = 1
}

[System.Serializable]
public class TownInLandInfo
{
    public string townName;
    public long id;
    public int population;
    public int prosperity;
    public int reputation;
    public int totalTroops;
    public long gatheredResource;
    public int consecutiveLogin;
    public int login;
    public int builtBuildings;
}
