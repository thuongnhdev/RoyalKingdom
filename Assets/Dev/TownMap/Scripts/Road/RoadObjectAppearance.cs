using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObjectAppearance : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownMapSO _townMap;

    [Header("Config")]
    [SerializeField]
    private TilingTransform _tilingTransform;
    [SerializeField]
    private Transform _roadHolder;
    [SerializeField]
    private List<GameObject> _roads;

    private int _officialAppearance;
    private Quaternion _officialRotation;
    private List<int> _currentNeighbors = new();

    public void AskNeighborsForGreeting()
    {
        int tileId = TownMapHelper.GetTileIdAtPosition(_townMap.xSize, _townMap.ySize, _tilingTransform.GetLocalPosition());
        if (tileId == -1)
        {
            return;
        }

        _currentNeighbors = GetRoads(TownMapHelper.GetNeighborTiles(_townMap.xSize, _townMap.ySize, tileId));

        foreach (var roadObj in GetNeighborObjs(_currentNeighbors))
        {
            roadObj.GreetNewNeighbor(tileId);
        }
    }

    public void AskNeighborsSaveGreetingAsOfficial()
    {
        foreach (var roadObj in GetNeighborObjs(_currentNeighbors))
        {
            roadObj.SaveGreetingSetupAsOfficial();
        }
    }

    public void GreetNewNeighbor(int newNeighbor)
    {
        int mapX = _townMap.xSize;
        int mapY = _townMap.ySize;
        int currentTileId = TownMapHelper.GetTileIdAtPosition(mapX, mapY, _tilingTransform.GetLocalPosition());

        List<int> currentNeighbors = TownMapHelper.GetNeighborTiles(_townMap.xSize, _townMap.ySize, currentTileId);
        List<int> currentNeighborRoads = GetRoads(currentNeighbors);
        currentNeighborRoads.Add(newNeighbor);

        SetUpRoadAppearance(currentNeighborRoads, true);
        SetUpRoadRotation(currentTileId, currentNeighborRoads, true);
    }

    public void SaveGreetingSetupAsOfficial()
    {
        for (int i = 0; i < _roads.Count; i++)
        {
            if (_roads[i].activeSelf)
            {
                SaveAppearance(i);
                break;
            }
        }
        SaveRotation(_roadHolder.rotation);
    }

    public void RevertGreeting()
    {
        DisableAllRoads();
        _roads[_officialAppearance].SetActive(true);
        _roadHolder.rotation = _officialRotation;
    }

    public void ClearNeighbors()
    {
        _currentNeighbors.Clear();
    }

    public void SelfSetUp()
    {
        SetupDelay().Forget(); ;
    }

    private async UniTaskVoid SetupDelay()
    {
        await UniTask.DelayFrame(2); // TODO RK: investigate why roads only correctly appear from 2nd frame
        int tileId = TownMapHelper.GetTileIdAtPosition(_townMap.xSize, _townMap.ySize, _tilingTransform.GetLocalPosition());
        if (tileId == -1)
        {
            return;
        }

        SetUpRoad(tileId);
    }

    private void SetUpRoadAndAskForGreeting(int currentTile)
    {
        SetUpRoad(currentTile, true);
    }

    private void SetUpRoad(int currentTile, bool askNeighborsForGreeting = false)
    {
        if (askNeighborsForGreeting)
        {
            AskNeighborRevertGreeting();
        }

        List<int> neighbors = TownMapHelper.GetNeighborTiles(_townMap.xSize, _townMap.ySize, currentTile);
        _currentNeighbors = GetRoads(neighbors);

        SetUpRoadAppearance(_currentNeighbors);
        SetUpRoadRotation(currentTile, _currentNeighbors);

        if (askNeighborsForGreeting)
        {
            AskNeighborsForGreeting();
            if (_currentNeighbors.Count == 0)
            {
                _roadHolder.gameObject.SetLayerRecursively(5); // UI
                return;
            }
            _roadHolder.gameObject.SetLayerRecursively(0); // Default
        }
    }

    private void SetUpRoadAppearance(List<int> neighbors, bool isGreeting = false)
    {
        DisableAllRoads();
        int neighborCount = neighbors.Count;
        int appearance;
        if (neighborCount != 2)
        {
            appearance = neighborCount;
            goto SET_AND_CACHE;
        }

        Vector2 neighbor1 = TownMapHelper.FromTileIdToAxis(_townMap.xSize, _townMap.ySize, neighbors[0]);
        Vector2 neighbor2 = TownMapHelper.FromTileIdToAxis(_townMap.xSize, _townMap.ySize, neighbors[1]);
        if (neighbor1.x == neighbor2.x || neighbor1.y == neighbor2.y)
        {
            appearance = 2;
            goto SET_AND_CACHE;
        }
        appearance = 5;

    SET_AND_CACHE:
        _roads[appearance].SetActive(true);
        SaveAppearance(appearance, isGreeting);
    }

    private void SetUpRoadRotation(int currentTile, List<int> neighbors, bool isGreeting = false)
    {
        int neighborCount = neighbors.Count;
        if (neighborCount == 0 || neighborCount == 4)
        {
            return;
        }

        Quaternion rotation;
        if (neighborCount == 1)
        {
            Vector2 neighborDir = TownMapHelper.GetDirectionOf2Tiles(_townMap.xSize, _townMap.ySize, currentTile, neighbors[0]);
            rotation = RotateToDirection(neighborDir);
            goto SET_AND_CACHE;
        }

        if (neighborCount == 2)
        {
            rotation = ChooseSmallerAngleMadeByCurrentAnd2Neighbors(currentTile, neighbors[0], neighbors[1]);
            goto SET_AND_CACHE;
        }

        rotation = ChooseMidAngleMadeByCurrentAnd3Neighbors(currentTile, neighbors);

    SET_AND_CACHE:
        _roadHolder.rotation = rotation;
        SaveRotation(rotation, isGreeting);
    }

    private void SaveAppearance(int appearance, bool isGreeting = false)
    {
        if (isGreeting)
        {
            return;
        }

        _officialAppearance = appearance;
    }

    private void SaveRotation(Quaternion rotation, bool isGreeting = false)
    {
        if (isGreeting)
        {
            return;
        }

        _officialRotation = rotation;
    }

    private Quaternion ChooseSmallerAngleMadeByCurrentAnd2Neighbors(int current, int tile1, int tile2)
    {
        int mapX = _townMap.xSize;
        int mapY = _townMap.ySize;

        Vector2 dir1 = TownMapHelper.GetDirectionOf2Tiles(mapX, mapY, current, tile1);
        Vector2 dir2 = TownMapHelper.GetDirectionOf2Tiles(mapX, mapY, current, tile2);
        var rot1 = RotateToDirection(dir1);
        var rot2 = RotateToDirection(dir2);

        float yAngle1 = rot1.eulerAngles.y;
        float yAngle2 = rot2.eulerAngles.y;
        if (Mathf.Abs(yAngle1 - yAngle2) > 180f)
        {
            return yAngle1 < yAngle2 ? rot2 : rot1;
        }
        return yAngle1 < yAngle2 ? rot1 : rot2;
        
    }

    private Quaternion ChooseMidAngleMadeByCurrentAnd3Neighbors(int current, List<int> neighbors)
    {
        int mapX = _townMap.xSize;
        int mapY = _townMap.ySize;
        for (int i = 0; i < 2; i++)
        {
            for (int j = i + 1; j < 3; j++)
            {
                int neighbor1 = neighbors[i];
                int neighbor2 = neighbors[j];
                Vector2 dir = TownMapHelper.GetDirectionOf2Tiles(mapX, mapY, neighbor1, neighbor2);
                if (dir.x == 0 || dir.y == 0)
                {
                    neighbors.Remove(neighbor1);
                    neighbors.Remove(neighbor2);
                    return RotateToDirection(TownMapHelper.GetDirectionOf2Tiles(mapX, mapY, current, neighbors[0]));
                }
            }
        }

        return Quaternion.identity;
    }

    private Quaternion RotateToDirection(Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            return Quaternion.identity;
        }
        if (direction == Vector2.right)
        {
            return Quaternion.Euler(0f, 90f, 0f);
        }
        if (direction == Vector2.down)
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }
        return Quaternion.Euler(0f, -90f, 0f);
    }

    private List<int> GetRoads(List<int> nominators)
    {
        List<int> roads = new();
        for (int i = 0; i < nominators.Count; i++)
        {
            var tile = _townMap.GetTileInfo(nominators[i]);
            if (tile.baseBuildingValue != 30101) // TODO refactor this if more road types come
            {
                continue;
            }

            roads.Add(tile.tileId);
        }

        return roads;
    }

    private void AskNeighborRevertGreeting()
    {
        foreach (var roadObj in GetNeighborObjs(_currentNeighbors))
        {
            roadObj.RevertGreeting();
        }
    }

    private void DisableAllRoads()
    {
        for (int i = 0; i < _roads.Count; i++)
        {
            _roads[i].SetActive(false);
        }
    }

    private IEnumerable<RoadObjectAppearance> GetNeighborObjs(List<int> neighbors)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            var roadObj = BuildingObjectFinder.Instance.GetBuildingObjectByLocation(_currentNeighbors[i]);
            if (roadObj == null)
            {
                continue;
            }

            var compGetter = roadObj.GetComponent<RoadComponentGetter>();
            if (compGetter == null)
            {
                continue;
            }

            yield return compGetter.RoadAppearance;
        }
    }

    private void OnEnable()
    {
        _tilingTransform.OnChangedTile += SetUpRoadAndAskForGreeting;
    }

    private void OnDisable()
    {
        _tilingTransform.OnChangedTile -= SetUpRoadAndAskForGreeting;
    }

}
