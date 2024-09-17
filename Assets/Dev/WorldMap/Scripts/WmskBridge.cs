using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using WorldMapStrategyKit;

public class WmskBridge : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private IntegerVariable _zoomLevel;
    [SerializeField]
    private IntegerVariable _selectedKingdom;
    [SerializeField]
    private IntegerVariable _selectedLand;
    [SerializeField]
    private IntegerVariable _selectedCity;

    [Header("Reference - Read/Write")]
    [SerializeField]
    private LandStaticInfoList _landsStaticInfo;

    [Header("Reference - Read")]
    [SerializeField]
    private TextAsset _defaultKingdomsData;
    [SerializeField]
    private TextAsset _defaultLandsData;
    [SerializeField]
    private TextAsset _defaultCitiesData;
    [SerializeField]
    private KingdomList _kingdoms;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onLoadedGeoData;
    [SerializeField]
    private GameEvent _onSelectedAKingdom;
    [SerializeField]
    private GameEvent _onSelectedALand;
    [SerializeField]
    private GameEvent _onSelectedACity;

    [Header("Config")]
    [SerializeField]
    private bool _displayWorldMap = true;
    [SerializeField]
    private string _kingdomDataPath = "Kingdom.txt";
    [SerializeField]
    private string _landDataPath = "Lands.txt";
    [SerializeField]
    private string _citiesDataPath = "Cities.txt";
    [SerializeField]
    private WMSK _map;
    [SerializeField]
    private List<float> _zoomLevelConfigs;

    private float _wmskZoomLevel = 0f;

    private List<Action<bool>> _setGeoDisplayActions = new();

    public bool hideMapBackground;

    public void RequestExtractALandFromLandPool(string landName, string kingdomName = "")
    {
        int landIndex = _map.GetProvinceIndex("Pool", landName);
        // Server Logic here
        int kingdomIndex;
        if (string.IsNullOrEmpty(kingdomName))
        {
            kingdomIndex = _map.GetCountryIndex(kingdomName);
        }
        else
        {
            kingdomIndex = _map.ProvinceToCountry(_map.provinces[landIndex], landName);
        }

        _map.CountryTransferProvince(kingdomIndex, landIndex, true);
        SaveGeoData();
    }

    public void TransferALand(long toKingdom, int landId)
    {
        var kingdom = _kingdoms.GetKingdom(toKingdom);
        int winKingdomIndex = _map.GetCountryIndex(kingdom.kingdomGeoId);
        //int landIndex = _landsStaticInfo.GetLandIndexByLandId(landId);

        //var landGeo = _map.provinces[landIndex];
        //_map.CountryTransferProvinceRegion(winKingdomIndex, landGeo.mainRegion, true);
    }

    public void LoadGeoData()
    {
        string kingdomsData = TextFileIO.ReadText(_kingdomDataPath);
        string landsData = TextFileIO.ReadText(_landDataPath);
        string citiesData = TextFileIO.ReadText(_citiesDataPath);

        if (string.IsNullOrEmpty(kingdomsData) || string.IsNullOrEmpty(landsData))
        {
            kingdomsData = _defaultKingdomsData.text;
            landsData = _defaultLandsData.text;
            citiesData = _defaultCitiesData.text;
        }

        _map.SetCountryGeoData(kingdomsData);
        _map.SetProvincesGeoData(landsData);
        _map.SetCityGeoData(citiesData);

        SetKingdomDataFromSOToMap();
        InitLandDict();

        UpdateWmskElements(_zoomLevel.Value);

        _map.gameObject.SetActive(true);
        _map.Redraw(true);

        FrameDelay_SetUpMapVisualElements().Forget();
        NextFrame_ShowOrHideMap().Forget();

    }
    private async UniTaskVoid FrameDelay_SetUpMapVisualElements()
    {
        await UniTask.DelayFrame(2); // TODO RK find reason of region color only display if colorizing from 2nd frame
        ShowLandsInPool();
        PreCreateLandLabels().Forget();
        PreCreateCity().Forget();

        _onLoadedGeoData.Raise();
    }
    private async UniTaskVoid NextFrame_ShowOrHideMap()
    {
        await UniTask.NextFrame();
        _map.gameObject.SetActive(_displayWorldMap);
    }

    public void SaveGeoData()
    {
        TextFileIO.WriteText(_kingdomDataPath, _map.GetCountryGeoData());
        TextFileIO.WriteText(_landDataPath, _map.GetProvinceGeoData());
    }

    private void CalculateZoomLevel()
    {
        float currentWmskZoomLevel = _map.GetZoomLevel();
        if (Mathf.Abs(currentWmskZoomLevel - _wmskZoomLevel) < float.Epsilon)
        {
            return;
        }

        _wmskZoomLevel = currentWmskZoomLevel;
        for (int i = 0; i < _zoomLevelConfigs.Count; i++)
        {
            if (_zoomLevelConfigs[i] <= _wmskZoomLevel)
            {
                _zoomLevel.Value = i + 1;
                break;
            }
        }
    }

    //      | KingdomName | LandBorder | LandName | CityDot |
    // 1    |      x      |     x      |          |         | 0b1100
    // 2    |      x      |     x      |          |         | 0b1100
    // 3    |      x      |     x      |          |    x    | 0b1101
    // 4    |             |     x      |    x     |    x    | 0b0111
    // 5    |             |     x      |    x     |    x    | 0b0111
    // 6    |             |     x      |    x     |    x    | 0b0111
    // 7    |             |     x      |    x     |    x    | 0b0111
    private int CalculateGeoElementsBit(int zoomLevel)
    {
        switch (zoomLevel)
        {
            case 1: return 0b1100;
            case 2: return 0b1100;
            case 3: return 0b1101;
            case 4: return 0b0111;
            case 5: return 0b0111;
            case 6: return 0b0111;
            case 7: return 0b0111;
            default: return 0;
        }
    }
    private void UpdateWmskElements(int zoomLevel)
    {
        int showBitValue = CalculateGeoElementsBit(zoomLevel);
        for (int i = _setGeoDisplayActions.Count - 1; 0 <= i; i--)
        {
            bool show = showBitValue % 2 != 0;
            _setGeoDisplayActions[i].Invoke(show);
            showBitValue >>= 1;
        }
    }
    private void CreateSetGeoDisplayActions()
    {
        _setGeoDisplayActions.Add(ShowKingdomNames);
        _setGeoDisplayActions.Add(ShowLands);
        _setGeoDisplayActions.Add(ShowLandNames);
        _setGeoDisplayActions.Add(ShowCities);
    }

    private void ShowKingdomNames(bool active)
    {
        _map.showCountryNames = active;
    }
    private void ShowLands(bool active)
    {
        _map.showProvinces = active;
        _map.drawAllProvinces = active;
    }

    private void ShowLandNames(bool active)
    {
        _map.showProvinceNames = active;
    }

    private void ShowCities(bool active)
    {
        _map.showCities = active;
    }

    private void ShowLandsInPool()
    {
        var countries = _map.countries;
        for (int i = 0; i < countries.Length; i++)
        {
            var country = countries[i];
            if (country.name == "Pool")
            {
                country.hidden = false;
                country.labelVisible = false;
                break;
            }
        }
    }

    private void OnClickedKingdom(int countryIndex, int regionIndex, int buttonIndex)
    {
        var selectedKingdom = _map.countries[countryIndex];
        _selectedKingdom.Value = selectedKingdom.uniqueId;
        _onSelectedAKingdom.Raise();
    }

    private void OnClickedProvince(int provinceIndex, int regionIndex, int buttonIndex)
    {
        var selectedProvince = _map.provinces[provinceIndex];
        _selectedLand.Value = selectedProvince.uniqueId;
        _onSelectedALand.Raise();
    }

    public void OnClickedCity(int cityIndex, int buttonIndex)
    {
        var selecledCity = _map.cities[cityIndex];
        _selectedCity.Value = selecledCity.uniqueId;
        _onSelectedACity.Raise();
        //MilitaryManager.Instance.OnClickCity(selecledCity);
    }

    private async UniTaskVoid PreCreateCity()
    {
        await UniTask.NextFrame();
        _map.showCities = true;
        City[] cities = _map.cities;
        for (int i = 0; i < cities.Length; i++)
        {
            cities[i].isVisible = true;
        }
        _map.DrawCities();
        await UniTask.NextFrame();
        _map.showCities = false;
    }

    private async UniTaskVoid PreCreateLandLabels()
    {
        await UniTask.NextFrame();
        _map.showProvinceNames = true;
        Country[] countries = _map.countries;
        for (int i = 0; i < countries.Length; i++)
        {
            _map.DrawProvinceLabels(countries[i]);
        }
        await UniTask.NextFrame();
        _map.showProvinceNames = false;
    }

    private void SetKingdomDataFromSOToMap()
    {
        /*
        _map.countries[0].isPool = true;
        List<KingdomPoco> kingdoms = _kingdoms.Kingdoms;
        for (int i = 0; i < kingdoms.Count; i++)
        {
            var kingdom = kingdoms[i];
            var kingdomGeoDtata = new Country(kingdom.name, "none", _map.GetUniqueId(new(_map.countries)));
            kingdomGeoDtata.regions.Add(new Region(kingdomGeoDtata, 0));
            kingdomGeoDtata.mainRegionIndex = 0;
            kingdomGeoDtata.provinces = new Province[0];

            int kingdomIndex = _map.CountryAdd(kingdomGeoDtata);
            kingdom.kingdomGeoId = kingdomGeoDtata.uniqueId;

            List<int> members = kingdom.members;
            for (int j = 0; j < members.Count; j++)
            {
                int member = members[j];
                //int landGeoId = _landsStaticInfo.GetLandGeoId(member);
                //int landIndex = _map.GetProvinceIndex(landGeoId);
                var land = _map.GetProvince(landIndex);
                if (land == null)
                {
                    continue;
                }
                _map.ReadProvincePackedString(land);

                _map.CountryTransferProvinceRegion(kingdomIndex, land.mainRegion, false);
            }
            kingdomGeoDtata.regions.RemoveAt(0);
        }

        */
    }

    private void InitLandDict()
    {
        _landsStaticInfo.InitLandIndex(_map.provinces);
    }

    private void Awake()
    {
        _map.dontLoadGeodataAtStart = true;
    }

    private void OnEnable()
    {
        _map.OnCountryClick += OnClickedKingdom;
        _map.OnProvinceClick += OnClickedProvince;
        _map.OnCityClick += OnClickedCity;

        _map.enableCountryHighlight = false;
        _map.provinceLabelsVisibility = PROVINCE_LABELS_VISIBILITY.Scripting;

        CreateSetGeoDisplayActions();
        CalculateZoomLevel();

        _zoomLevel.OnValueChange += UpdateWmskElements;
    }

    private void Update()
    {
        CalculateZoomLevel();
        if (hideMapBackground) {
            _map.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnDisable()
    {
        _map.OnCountryClick -= OnClickedKingdom;
        _map.OnProvinceClick -= OnClickedProvince;
        _map.OnCityClick -= OnClickedCity;

        _zoomLevel.OnValueChange -= UpdateWmskElements;
    }
}
