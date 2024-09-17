using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityContextMenu : MonoBehaviour
{
    public IntegerVariable selectedCity;
    public GameEvent attackEvent;
    public CityList cities;
    public TableCityViewModel cities_LocalData;
    public ApiList api;

    CityPoco cityPoco;
    CityViewModel cityModel;


    private void OnEnable() {
        selectedCity.OnValueChange += OnCitySelected;
    }

    private void OnDisable() {
        selectedCity.OnValueChange -= OnCitySelected;
    }

    public void OnCitySelected(int notUsed) {
        cityPoco = cities.GetCity(selectedCity.Value);
        cityModel = cities_LocalData.GetByKey(selectedCity.Value);
        UpdateView();
    }

    public void UpdateView() { 
    }

    public void Move() {
        attackEvent.Raise();
    }
}

