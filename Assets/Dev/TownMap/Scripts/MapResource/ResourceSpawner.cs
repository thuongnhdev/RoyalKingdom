using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Referece - Read")]
    [SerializeField]
    private TownMapSO _userTownMap;
    [SerializeField]
    private ItemList _itemList;

    [Header("Config")]
    [SerializeField]
    private float _spawnInterval = 0.3f;
    [SerializeField]
    private ObjectSpawner _resourcePool;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onResourceAddedToTileData;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onSpawnedResourceObjects;

    public void SpawnResource(List<ResourcePoco> resources, int location)
    {
        IntervalSpawnResources(resources, location).Forget();
    }

    public void SpawnInitResourceFollowMapData(List<ResourcePoco> resources, int location)
    {
        IntervalSpawnResources(resources, location, false).Forget();
    }

    private void SpawnResouceObjects(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }
        var resources = (List<ResourcePoco>)args[0];
        int locationTileId = (int)args[1];

        IntervalSpawnResources(resources, locationTileId).Forget();
    }

    private async UniTaskVoid IntervalSpawnResources(List<ResourcePoco> resources, int location, bool notifyDataSync = true)
    {
        Vector3 tilePos = TownMapHelper.GetTileLocalPosition(_userTownMap.xSize, _userTownMap.ySize, location);
        Vector3 poolPos = tilePos;
        poolPos.x -= _userTownMap.xOffset / 3f;
        poolPos.y = 0.15f;
        poolPos.z += _userTownMap.xOffset / 3f;

        _resourcePool.transform.localPosition = poolPos;

        for (int i = 0; i < resources.Count; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_spawnInterval));
            var resource = resources[i];
            for (int j = 0; j < resource.itemCount; j++)
            {
                var resourceObj = _resourcePool.SpawnWithRandomRangeAndReturnObject();
                var resourceComp = resourceObj.GetComponent<ResourceObject>();
                resourceComp.SetUp(location, resource.itemId, 1);
            }
        }

        if (!notifyDataSync)
        {
            return;
        }

        _onSpawnedResourceObjects.Raise(location);
    }

    private void OnEnable()
    {
        _onResourceAddedToTileData.Subcribe(SpawnResouceObjects);
    }

    private void OnDisable()
    {
        _onResourceAddedToTileData.Unsubcribe(SpawnResouceObjects);
    }
}
