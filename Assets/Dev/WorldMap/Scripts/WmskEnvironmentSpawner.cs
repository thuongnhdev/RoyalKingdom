using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class WmskEnvironmentSpawner : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField]
    private GameObject _cityPrefab;

    private WMSK _map;

    public void SpawnCities()
    {
        City[] cities = _map.cities;
        Debug.Log(cities[0].unity2DLocation);
    }

    private void OnEnable()
    {
        _map = WMSK.instance;
    }
}
