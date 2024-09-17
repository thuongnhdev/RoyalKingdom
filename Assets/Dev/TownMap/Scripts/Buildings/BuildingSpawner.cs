using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingSOList;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssetsList;
    [SerializeField]
    private TownMapSO _userTownMap;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private IntegerVariable _selectedBuildingId;
    [SerializeField]
    private IntegerVariable _newBuildingCenterTileId;

    [Header("Events out")]
    [SerializeField]
    private UnityEvent _onSpawnedAllInitBuildings;

    [Header("Reference - Config")]
    [SerializeField]
    private GameObject _baseBuildingPrefab;
    [SerializeField]
    private GameObject _roadPrefab;
    [SerializeField]
    private Transform _buildingsHolder;
    [SerializeField]
    private BuildingObjectFinder _buildingObjFinder;
    [SerializeField]
    private GameEvent _onTypeHouse;
    [SerializeField]
    private GameEvent _onUpdatePositionResourcce;
    public GameObject TopLeftSpawnBuilding(int buildingId, int buildingTopLeftTileId)
    {
        int mapX = _userTownMap.xSize;
        int mapY = _userTownMap.ySize;

        Vector2 buildingSize = _buildingSOList.GetBuildingSize(buildingId);
        int centerTileId = TownMapHelper.FromTopLeftToCenterTile(mapX, mapY, buildingSize, buildingTopLeftTileId);
        Vector3 buildingLocalPosition = TownMapHelper.GetBuildingPosition(mapX, mapY, centerTileId, buildingSize);

        var building = SpawnAndSetUpBuilding(buildingId, buildingSize);
        building.transform.localPosition = buildingLocalPosition;

        _buildingObjFinder.AddBuildingByLocation(buildingTopLeftTileId, building);
        if (_onTypeHouse != null) _onTypeHouse.Raise(centerTileId, buildingId, building.transform, buildingTopLeftTileId);
        if (_onUpdatePositionResourcce != null) _onUpdatePositionResourcce.Raise(centerTileId, buildingId, building.transform);
        return building;
    }

    public GameObject CenterSpawnBuilding(int buildingId, int buildingCenterTileId)
    {
        int mapX = _userTownMap.xSize;
        int mapY = _userTownMap.ySize;

        Vector2 buildingSize = _buildingSOList.GetBuildingSize(buildingId);
        Vector3 buildingLocalPosition = TownMapHelper.GetBuildingPosition(mapX, mapY, buildingCenterTileId, buildingSize);

        var building = SpawnAndSetUpBuilding(buildingId, buildingSize);
        building.transform.localPosition = buildingLocalPosition;

        int topLeftTileId = TownMapHelper.FromCenterToTopLeftTile(mapX, mapY, buildingSize, buildingCenterTileId);
        _buildingObjFinder.AddBuildingByLocation(topLeftTileId, building);

        return building;
    }

    public void In_SpawnNewBuilding()
    {
        CenterSpawnBuilding(_selectedBuildingId.Value, _newBuildingCenterTileId.Value);
    }

    public void In_SpawnInitBuildings()
    {
        List<UserBuilding> buildings = _userBuildings.BuildingList;
        for (int i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            if (building.status == BuildingStatus.None)
            {
                continue;
            }

            TopLeftSpawnBuilding(building.buildingId, building.locationTileId);
        }
        NextFrame_NotifySpawnedAllInitBuildings().Forget();
    }

    private GameObject SpawnAndSetUpBuilding(int buildingId, Vector2 size)
    {
        if (buildingId == 30101)
        {
            return SpawnRoad();
        }

        GameObject building = Instantiate(_baseBuildingPrefab, _buildingsHolder);
        building.name = _buildingSOList.GetBuildingName(buildingId);
        GameObject buildingModelPrefab = _buildingAssetsList.GetBuildingModel(buildingId);
        if (buildingModelPrefab != null)
        {
            SpawnModel(building, buildingModelPrefab, size);
        }

        return building;
    }
    private void SpawnModel(GameObject buildingObject, GameObject buildingModelPrefab, Vector2 size)
    {
        var compGetter = buildingObject.GetComponent<BuildingComponentGetter>();
        var buildingModel = Instantiate(buildingModelPrefab, compGetter.BuildingModelHolder);

        buildingModel.transform.localPosition = Vector3.zero;
        buildingModel.transform.rotation = Quaternion.identity;
        Vector3 modelScale = buildingModel.transform.localScale;
        modelScale.x *= size.x;
        modelScale.y *= size.x;
        modelScale.z *= size.y;
        buildingModel.transform.localScale = modelScale;
    }

    private GameObject SpawnRoad()
    {
        return Instantiate(_roadPrefab, _buildingsHolder);
    }

    private async UniTaskVoid NextFrame_NotifySpawnedAllInitBuildings()
    {
        await UniTask.NextFrame();
        _onSpawnedAllInitBuildings.Invoke();
    }
}
