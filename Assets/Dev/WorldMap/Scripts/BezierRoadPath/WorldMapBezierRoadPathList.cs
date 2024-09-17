using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMapRoadPathList", menuName = "Uniflow/World/WorldMapRoadPathList")]
public class WorldMapBezierRoadPathList : ScriptableObject
{
    [Header("Reference - Read")]
    [SerializeField]
    private WorldPathFindingService _pathFindingServices;
    [SerializeField]
    private TableCityViewModel _cities;

    [Header("")]
    [SerializeField]
    private List<RoadBezierPath> _paths = new();
    public List<RoadBezierPath> Paths => new(_paths);
    private Dictionary<int, RoadBezierPath> _pathDict = new();
    private Dictionary<int, RoadBezierPath> PathDict
    {
        get
        {
            if (_pathDict.Count != _paths.Count)
            {
                _pathDict.Clear();
                for (int i = 0; i < _paths.Count; i++)
                {
                    _pathDict.Add(_paths[i].pathId, _paths[i]);
                }
            }

            return _pathDict;
        }
    }

    public RoadBezierPath GetPath(int pathId)
    {
        PathDict.TryGetValue(pathId, out var path);
        if (path == null)
        {
            Logger.LogError($"Invalid path id [{pathId}]");
        }

        return path;
    }

    public void SetBezierPoints(int pathId, Vector3[] points)
    {
        var path = GetPath(pathId);
        if (path == null)
        {
            return;
        }

        path.bezierPoints = points;
    }

    public void AddCityToPath(int pathId, CityPositionOnPath city, string pathName = "No name")
    {
        PathDict.TryGetValue(pathId, out var path);
        if (path == null)
        {
            path = new(pathId, new());
            path.pathName = $"{pathName} {path.GetHashCode()}";
            _paths.Add(path);
            _pathDict.Add(pathId, path);
        }

        path.AddCity(city);
    }

    public void ClearPath()
    {
        _paths.Clear();
        _pathDict.Clear();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

    }

    public List<BezierPathInstruction> GetBezierPath(int fromCity, int toCity, long playerId)
    {
        var citiesPath = _pathFindingServices.FindPath(fromCity, toCity, playerId, true);
        if (citiesPath.Count < 2)
        {
            Logger.Log($"No valid path from city [{fromCity}] to city [{toCity}]");
            return null;
        }

        List<BezierPathInstruction> path = new();
        BezierPathInstruction firstPathSegment = FindConnectedPathBetween2Cities(citiesPath[0], citiesPath[1]);
        path.Add(firstPathSegment);
        if (citiesPath.Count == 2)
        {
            return path;
        }

        for (int i = 1; i < citiesPath.Count - 1; i++)
        {
            var currentSegment = path[^1];
            var nextSegment = FindConnectedPathBetween2Cities(citiesPath[i], citiesPath[i + 1]);
            if (currentSegment.pathId == nextSegment.pathId)
            {
                currentSegment.toDistance = nextSegment.toDistance;
                continue;
            }
            path.Add(nextSegment);
        }

        return path;
    }
    private BezierPathInstruction FindConnectedPathBetween2Cities(CityViewModel city1, CityViewModel city2)
    {
        List<int> city1CrossPaths = city1.crossBezierPaths;
        List<int> city2CrossPaths = city2.crossBezierPaths;
        for (int i = 0; i < city1CrossPaths.Count; i++)
        {
            int city1CrossPath = city1CrossPaths[i];
            for (int j = 0; j < city2CrossPaths.Count; j++)
            {
                int city2CrossPath = city2CrossPaths[j];
                if (city1CrossPath != city2CrossPath)
                {
                    continue;
                }

                var path = GetPath(city1CrossPath);
                float city1Distance = path.GetCityDistance(city1.Uid);
                float city2Distance = path.GetCityDistance(city2.Uid);
                return new() { pathId = city1CrossPath, fromDistance = city1Distance, toDistance = city2Distance };
            }
        }

        return null;
    }

    private void OnValidate()
    {
        for (int i = 0; i < _paths.Count; i++)
        {
            var cities = _paths[i].cities;
            for (int j = 0; j < cities.Count; j++)
            {
                cities[j].stringId = cities[j].cityId.ToString();
            }
        }
    }

}

[System.Serializable]
public class RoadBezierPath
{
    public string pathName;
    public int pathId;
    public Vector3 center;
    public float controlSpacing;
    public float maxAngleError;
    public float minVertextDst;
    public float pathLength;
    public Vector3[] bezierPoints;
    public List<CityPositionOnPath> cities = new();
    // key = cityId
    private Dictionary<int, CityPositionOnPath> _citiDict = new();
    private Dictionary<int, CityPositionOnPath> CityDict
    {
        get
        {
            if (_citiDict == null)
            {
                _citiDict = new();
            }
            if (_citiDict.Count != cities.Count)
            {
                _citiDict.Clear();
                for (int i = 0; i < cities.Count; i++)
                {
                    _citiDict.Add(cities[i].cityId, cities[i]);
                }
            }

            return _citiDict;
        }
    }
    
    public RoadBezierPath(int pathId, List<CityPositionOnPath> cities)
    {
        this.pathId = pathId;
        for (int i = 0; i < cities.Count; i++)
        {
            this.cities.Add(cities[i]);
            _citiDict.Add(cities[i].cityId, cities[i]);
        }
    }

    public CityPositionOnPath GetCityOnPath(int cityId)
    {
        CityDict.TryGetValue(cityId, out var city);
        if (city == null)
        {
            Logger.LogError($"Invalid cityId [{cityId}]");
        }

        return city;
    }

    public void AddCity(CityPositionOnPath city)
    {
        if (_citiDict.ContainsKey(city.cityId))
        {
            return;
        }

        cities.Add(city);
        _citiDict.Add(city.cityId, city);
    }

    public bool CanGoViaThisPath(int fromCity, int toCity)
    {
        return CityDict.ContainsKey(fromCity) && CityDict.ContainsKey(toCity);
    }

    public float GetCityDistance(int cityId)
    {
        var city = GetCityOnPath(cityId);
        if (city == null)
        {
            return 0f;
        }

        return city.pathDistance;
    }

}

[System.Serializable]
public class CityPositionOnPath
{
    public string stringId;
    public int cityId;
    /// <summary>
    /// distance from path root to city
    /// </summary>
    public float pathDistance;
    /// <summary>
    /// rootPath = 0, endOfPath = 1
    /// </summary>
    public float pathTime;
    public CityPositionOnPath(int id, float distance, float time)
    {
        cityId = id;
        pathDistance = distance;
        pathTime = time;
    }
}

[System.Serializable]
public class BezierPathInstruction
{
    public int pathId;
    public float fromDistance;
    public float toDistance;
}
