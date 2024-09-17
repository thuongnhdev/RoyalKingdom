using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityList", menuName = "Uniflow/World/CityList")]
public class CityList : ScriptableObject
{
    [SerializeField]
    private LandStaticInfoList _lands;
    [SerializeField]
    private List<CityPoco> _cities;
    private Dictionary<int, CityPoco> _cityDict = new();

    public List<CityPoco> Cities => _cities;
    public Dictionary<int, CityPoco> CityDict
    {
        get
        {
            if (_cities.Count != _cityDict.Count) {
                Reindex();
            }

            return _cityDict;
        }
    }

    void Reindex()
    {
        _cityDict.Clear();
        for (int i = 0; i < _cities.Count; i++)
        {
            _cityDict.Add(_cities[i].id, _cities[i]);
        }
    }
    public void Init(List<CityPoco> newList) {
        _cities = newList;
    }

    public void Init(ApiGetLowData serverData)
    {
        _cities.Clear();
        _cityDict.Clear();

        int cityCount = serverData.CityWorldMapLength;
        for (int i = 0; i < cityCount; i++)
        {
            var cityData = serverData.CityWorldMap(i).Value;
            var city = new CityPoco()
            {
                id = cityData.Id,
                geoId = (int)cityData.IdGround,
                cityName = cityData.Name,
                landId = cityData.IdLand
            };

            _cities.Add(city);
            _cityDict.Add(city.id, city);
            var land = _lands.GetLand(city.landId);
            if (land == null)
            {
                continue;
            }

            land.cities.Add(city.id);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public List<CityPoco> FindBy_idLand(int id) {
        List<CityPoco> rs = new();
        for (int i = 0; i < _cities.Count; i++)
        {
            if (_cities[i].landId == id)
                rs.Add(_cities[i]);
        }
        return rs;
    }

    public CityPoco GetCity(int cityId)
    {
        CityDict.TryGetValue(cityId, out var city);
        if (city == null) {
            //return null;
            //Logger.LogError($"Invalid cityId [{cityId}]");
        }

        return city;
    }

    public string GetCityName(int cityId)
    {
        var city = GetCity(cityId);
        if (city == null)
        {
            return "";
        }

        return city.cityName;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
