using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Text;
#endif

[CreateAssetMenu(fileName = "BuildingAssetsInfoList", menuName = "Uniflow/Building/BuildingAssetsInfoList")]
public class TownBuildingAssetsInfoList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private List<TownBuidingAssetsInfo> _buildingAssetsInfos;
    private Dictionary<int, TownBuidingAssetsInfo> _assetsMap = new();
    private Dictionary<int, TownBuidingAssetsInfo> AssetsMap
    {
        get
        {
            if (_buildingAssetsInfos.Count != _assetsMap.Count)
            {
                _assetsMap.Clear();

                for (int i = 0; i < _buildingAssetsInfos.Count; i++)
                {
                    var assetInfo = _buildingAssetsInfos[i];
                    _assetsMap[assetInfo.buildingId] = assetInfo;
                }
            }

            return _assetsMap;
        }
    }

    public TownBuidingAssetsInfo GetBuildingAssetsInfo(int buildingId)
    {
        AssetsMap.TryGetValue(buildingId, out var assets);
        if (assets == null)
        {
            Debug.LogWarning($"No Sprite for buildingId [{buildingId}]");
        }

        return assets;
    }

    public float GetDefaultRotation(int buildingId)
    {
        var building = GetBuildingAssetsInfo(buildingId);
        if (building == null)
        {
            return 180f;
        }

        return building.defaultRotation;
    }

    public Sprite GetBuildingSprite(int buildingId)
    {
        var assets = GetBuildingAssetsInfo(buildingId);
        if (assets == null)
        {
            return null;
        }

        return assets.buildingSprite;
    }

    public Sprite GetBuildingGreyscaleSprite(int buildingId)
    {
        var assets = GetBuildingAssetsInfo(buildingId);
        if (assets == null)
        {
            return null;
        }

        return assets.greyscaleSprite;
    }

    public GameObject GetBuildingModel(int buildingId)
    {
        var assets = GetBuildingAssetsInfo(buildingId);
        if (assets == null)
        {
            return null;
        }

        return assets.buildingModel;
    }

#if UNITY_EDITOR
    public void AutoAssignBuildingSprite()
    {
        FillMissingBuildingInfos();
        var guid = AssetDatabase.FindAssets("t:Sprite", new[] { $"Assets/Art/UI_Sprite/Building/ConstructionMenu" });
        if (guid.Length == 0)
        {
            Logger.LogWarning($"No Sprite found, Check direction in script of {name}");
            return;
        }

        for (int i = 0; i < _buildingAssetsInfos.Count; i++)
        {
            var assets = _buildingAssetsInfos[i];
            string trimAssetName = assets.name.Replace(" ", string.Empty).ToLower();
            for (int j = 0; j < guid.Length; j++)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(guid[j]);
                Sprite buildingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (!buildingSprite.name.ToLower().Contains(trimAssetName))
                {
                    continue;
                }

                if (buildingSprite.name.ToLower().Contains("greyscale"))
                {
                    assets.greyscaleSprite = buildingSprite;
                    continue;
                }

                assets.buildingSprite = buildingSprite;
            }
        }

        EditorUtility.SetDirty(this);
    }

    public void AutoAssignBuildingModel()
    {
        FillMissingBuildingInfos();

        for (int i = 0; i < _buildingAssetsInfos.Count; i++)
        {
            var assets = _buildingAssetsInfos[i];
            var guid = AssetDatabase.FindAssets("t:prefab", new[] { $"Assets/Art/BG/3DModel/Town/Building/{assets.name}" });
            if (guid.Length != 1)
            {
                Logger.LogWarning($"No prefab or more than 1 prefab found for [{assets.name}]");
                continue;
            }

            string modelPath = AssetDatabase.GUIDToAssetPath(guid[0]);
            assets.buildingModel = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        }

        EditorUtility.SetDirty(this);
    }

    public void FillMissingBuildingInfos()
    {
        var buildings = _buildings.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            AssetsMap.TryGetValue(building.id, out var buildingAsset);
            if (buildingAsset != null)
            {
                buildingAsset.name = building.name;
                continue;
            }

            buildingAsset = new() { buildingId = building.id, name = building.name };
            _assetsMap.Add(buildingAsset.buildingId, buildingAsset);
            _buildingAssetsInfos.Add(buildingAsset);
        }

        EditorUtility.SetDirty(this);
    }

    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        var buildingSheet = parsedSheets[0];
        for (int i = 0; i < buildingSheet.Count; i++)
        {
            var row = buildingSheet[i];
            row.GetValue("Key", out int buildingId);
            row.GetValue("Name", out string buildingName);

            var buildingAssets = GetBuildingAssetsInfo(buildingId);
            if (buildingAssets == null)
            {
                buildingAssets = new TownBuidingAssetsInfo();
                buildingAssets.buildingId = buildingId;
                _buildingAssetsInfos.Add(buildingAssets);
            }
            buildingAssets.name = buildingName;

            _assetsMap[buildingId] = buildingAssets;
        }

        EditorUtility.SetDirty(this);
    }
#endif
}

[System.Serializable]
public class TownBuidingAssetsInfo
{
    public string name;
    public int buildingId;
    public float defaultRotation = 180f;
    public Sprite buildingSprite;
    public Sprite greyscaleSprite;
    public GameObject buildingModel;
}
