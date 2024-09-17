using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

[CreateAssetMenu(fileName = "LandList", menuName = "Uniflow/World/LandList")]
public class LandStaticInfoList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<LandStaticInfo> _lands;
    public List<LandStaticInfo> Lands => _lands;
    private Dictionary<int, LandStaticInfo> _landDict = new();
    private Dictionary<int, LandStaticInfo> LandDict
    {
        get
        {
            if (_landDict.Count != _lands.Count)
            {
                _landDict.Clear();
                for (int i = 0; i < _lands.Count; i++)
                {
                    _landDict[_lands[i].id] = _lands[i];
                }
            }

            return _landDict;
        }
    }
    // Key = landGeoId, Value = landIndex (provinceIndex of WMSK)
    private Dictionary<int, int> _landIndexDict = new();

    private Color _noDefineColor = new Color(0f, 0f, 0f, 0.5f);

    public void Init(List<LandStaticInfo> newList) {
        _lands = newList;
    }
    public void Init(ApiGetLowData serverData)
    {
        int landCount = serverData.LandInfoLength;
        if (landCount == 0)
        {
            return;
        }

        _lands.Clear();
        _landDict.Clear();
        for (int i = 0; i < landCount; i++)
        {
            var landData = serverData.LandInfo(i).Value;
            var landColorData = serverData.Colorland(i).Value;
            LandStaticInfo land = new(landData);
            ColorUtility.TryParseHtmlString(landColorData.ColorCode, out Color landColor);
            landColor.a = 0.5f;
            land.color = landColor;
            _lands.Add(land);
            _landDict.Add(land.id, land);
        }
    }

    public void InitLandIndex(Province[] lands)
    {
        _landIndexDict.Clear();
        for (int i = 0; i < lands.Length; i++)
        {
            var land = lands[i];
            _landIndexDict.Add(land.uniqueId, i);
        }
    }

    public LandStaticInfo GetLand(int landId)
    {
        LandDict.TryGetValue(landId, out var land);
        if (land == null)
        {
            Logger.LogWarning($"Land with id [{landId}] is not available");
        }

        return land;
    }

    public int GetLandMaxTown(int geoId)
    {
        var land = GetLand(geoId);
        if (land == null)
        {
            return 0;
        }

        return land.maxTown;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        _lands.Clear();
        _landDict.Clear();
        var infoSheet = parsedSheets[0];
        var colorSheet = parsedSheets[4];
        for (int i = 0; i < infoSheet.Count; i++)
        {
            var row = infoSheet[i];
            row.GetValue("Key", out int id);
            row.GetValue("GroundID", out int geoId);
            row.GetValue("MaxCity", out int maxCity);
            row.GetValue("LandName", out string landName);
            row.GetValue("Purchasable", out int purchasableInt);
            row.GetValue("RegionalProductItem", out int regionalProduct);
            row.GetValue("TerrainKEY", out int terrain);
            row.GetValue("Religion", out int religion);
            row.GetValue("Culture", out int culture);
            row.GetValue("Race", out int race);
            row.GetValue("PopulationGrade", out int popGrade);

            var land = new LandStaticInfo()
            {
                id = id,
                landGeoId = geoId,
                maxCity = maxCity,
                landName = landName,
                purchasable = purchasableInt == 1,
                regionalProduct = regionalProduct,
                terrainId = terrain,
                religion = religion,
                culture = culture,
                race = race, 
                populationGrade = popGrade,
                maxTown = 150
            };
            _lands.Add(land);
            _landDict[land.id] = land;

            var colorRow = colorSheet[i];
            colorRow.GetValue("ColorCode", out string color);
            ColorUtility.TryParseHtmlString($"#{color}", out Color landColor);
            landColor.a = 0.5f;
            land.color = landColor;
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
