using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WorldMapStrategyKit;

public class WmskRegionColorFiller : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;
    [SerializeField]
    private KingdomList _kingdoms;
    [SerializeField]
    private IntegerVariable _selectedKingdom;
    [SerializeField]
    private IntegerVariable _selectedLand;
    [SerializeField]
    private IntegerVariable _zoomLevel;

    private WMSK _map;

    private HashSet<GameObject> _kingdomColorObjs = new();
    private HashSet<GameObject> _landColorObjs = new();
    private GameObject _lastHighlightObject;
    private int _lastHighlightedLandGeoId;
    private bool _activeKingdomColor = false;

    public void HighLightLand()
    {
        _highlightRegionToken?.Cancel();
        RevertALandColorToOrigin(_lastHighlightedLandGeoId);

        int landIndex = _map.GetProvinceIndex(_selectedLand.Value);
        _lastHighlightedLandGeoId = _selectedLand.Value;
        _lastHighlightObject = _map.ToggleProvinceRegionSurfaceHighlight(landIndex, 0, new Color(0f, 0f, 0f, 0.5f));
        PerFrame_HighlightSelectedRegion().Forget();
    }

    public void ColorAllKingdoms()
    {
        List<KingdomPoco> kingdoms = _kingdoms.Kingdoms;
        for (int i = 0; i < kingdoms.Count; i++)
        {
            var kingdom = kingdoms[i];
            ColorAKingdom(kingdom.kingdomId);
        }
    }

    public void RefreshAllKingdomsColor()
    {
        NextFrame_RefreshKingdomsColor().Forget();
    }
    private async UniTaskVoid NextFrame_RefreshKingdomsColor()
    {
        await UniTask.NextFrame();
        // WMSK caches kingdom surfaces. So if we want to refresh Kingdom, make sure to destroy old color objects
        // and clear surfaces cached like following
        foreach (var color in _kingdomColorObjs)
        {
            Destroy(color);
        }
        _kingdomColorObjs.Clear();
        _map.HideCountrySurfaces();
        // End of destroy color object and clear surfaces cache
        await UniTask.NextFrame();
        ColorAllKingdoms();
    }

    public void ColorAllLands()
    {
        List<LandStaticInfo> lands = _landsStaticInfos.Lands;
        for (int i = 0; i < lands.Count; i++)
        {
            var land = lands[i];
            ColorALand(land.landGeoId);
        }

        ShowLandOrKingdomColor(_zoomLevel.Value);
    }

    public void ColorALand(int landGeoId)
    {
        /*
        Color landColor = _landsStaticInfos.GetLandColor(landGeoId);
        int landIndex = _map.GetProvinceIndex(landGeoId);
        GameObject colorObject = _map.ToggleProvinceRegionSurfaceHighlight(landIndex, 0, landColor);
        if (_landColorObjs.Contains(colorObject))
        {
            return;
        }
        _landColorObjs.Add(colorObject);
        */
    }

    public void ColorAKingdom(long kingdomId)
    {
        var kingdom = _kingdoms.GetKingdom(kingdomId);
        if (kingdom == null || kingdom.members.Count == 0)
        {
            return;
        }

        int kingdomIndex = _map.GetCountryIndex(kingdom.name);
        var masterLand = _landsStaticInfos.GetLand(kingdom.masterLand);
        if (masterLand == null)
        {
            return;
        }

        var kingdomGeo = _map.countries[kingdomIndex];
        int regionCount = kingdomGeo.regions.Count;
        for (int i = 0; i < regionCount; i++)
        {
            GameObject colorObj = _map.ToggleCountryRegionSurfaceHighlight(kingdomIndex, i, masterLand.color, true);
            
            if (_kingdomColorObjs.Contains(colorObj))
            {
                continue;
            }
            _kingdomColorObjs.Add(colorObj);
        }
    }

    public void RevertALandColorToOrigin(int landGeoId)
    {
        ColorALand(landGeoId);
    }

    public void RevertKingdomColorToOrigin(int kingdomId)
    {

    }

    private CancellationTokenSource _highlightRegionToken;
    private async UniTaskVoid PerFrame_HighlightSelectedRegion()
    {
        _highlightRegionToken?.Cancel();
        _highlightRegionToken = new();

        var landHighlightSharedMat = _lastHighlightObject.GetComponent<Renderer>().sharedMaterial;
        float colorRatio;
        Color red = Color.red;
        red.a = 0.5f;
        Color green = Color.green;
        green.a = 0.5f;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_highlightRegionToken.Token))
        {
            colorRatio = Mathf.Sin(4f * Time.realtimeSinceStartup);
            colorRatio = MathUtils.Remap(-1f, 1f, 0f, 1f, colorRatio);
            landHighlightSharedMat.color = Color.Lerp(red, green, colorRatio);
        }
    }

    private void ShowLandOrKingdomColor(int zoomLevel)
    {
        bool activeKingdomColor = zoomLevel < 3;
        if (activeKingdomColor == _activeKingdomColor)
        {
            return;
        }
        _activeKingdomColor = activeKingdomColor;
        ActiveKingdomsColor(activeKingdomColor);
        ActiveLandsColor(!activeKingdomColor);
    }

    private void ActiveKingdomsColor(bool active)
    {
        foreach (var color in _kingdomColorObjs)
        {
            if (color == null)
            {
                continue;
            }

            color.SetActive(active);
        }
    }

    private void ActiveLandsColor(bool active)
    {
        foreach (GameObject color in _landColorObjs)
        {
            if (color == null)
            {
                continue;
            }

            color.SetActive(active);
        }
    }

    private void OnEnable()
    {
        _map = WMSK.instance;
        _zoomLevel.OnValueChange += ShowLandOrKingdomColor;
    }

    private void OnDisable()
    {
        _highlightRegionToken?.Cancel();
        _zoomLevel.OnValueChange -= ShowLandOrKingdomColor;
    }
}
