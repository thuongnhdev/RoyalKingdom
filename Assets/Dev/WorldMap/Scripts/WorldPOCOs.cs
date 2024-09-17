using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class KingdomPoco
{
    public string name;
    public long kingdomId;
    public int kingdomGeoId;
    public int masterLand;
    public float tax;
    public List<int> members = new();
}

[System.Serializable]
public class CityPoco
{
    public string cityName;
    public int id;
    [JsonIgnore]
    public int geoId;
    [JsonIgnore]
    public Vector3 position;
    public string landName;
    public int landId;
    public List<int> neighbors = new();
}
