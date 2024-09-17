using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplePools : MonoSingleton<MultiplePools>
{
    // key = object InstanceId
    Dictionary<int, PoolInMultiplePool> _pools = new Dictionary<int, PoolInMultiplePool>();

    public GameObject SpawnGameObject(GameObject go, int poolSize = 5)
    {
        _pools.TryGetValue(go.GetInstanceID(), out var pool);

        if (pool == null)
        {
            pool = ScriptableObject.CreateInstance<PoolInMultiplePool>();
            pool.Init(go, poolSize);

            _pools[go.GetInstanceID()] = pool;
        }

        return pool.SpawnObject();
    }
}

public class PoolInMultiplePool : ScriptableObject
{
    private GameObject _rootObject;
    private int _limit;

    private Queue<GameObject> _available = new Queue<GameObject>();

    public void Init(GameObject go, int limit)
    {
        _rootObject = go;
        _limit = limit;
    }

    public GameObject SpawnObject()
    {
        GameObject go;
        if (_available.Count == 0)
        {
            go = Instantiate(_rootObject);
            var objectInPool = go.AddComponent<ObjectInMultiplePoolsPool>();
            objectInPool.AssignParrentPool(this);
            go.SetActive(true);
            return go;
        }

        go = _available.Dequeue();

        if (go == null || go.activeSelf)
        {
            go = SpawnObject();
        }

        go.SetActive(true);

        return go;
    }

    public void BackToPool(GameObject go)
    {
        if (_available.Count + 1 > _limit)
        {
            Destroy(go);
            return;
        }

        go.SetActive(false);
        go.transform.SetPositionAndRotation(_rootObject.transform.position, Quaternion.identity);
        _available.Enqueue(go);
    }
}
