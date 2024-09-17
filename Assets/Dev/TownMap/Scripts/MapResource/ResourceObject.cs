using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [Header("Events out")]
    [SerializeField]
    private GameEvent _onRemoveResourceFromTile;

    [Header("Inspec")]
    [SerializeField]
    private int _locationTileId;
    [SerializeField]
    private ResourcePoco _resourceValue;
    public ResourcePoco ResourceValue => _resourceValue;

    public int LocationTileId => _locationTileId;

    public delegate void OnActiveDeactiveDel(int location, GameObject resource);
    public static event OnActiveDeactiveDel OnAddedToTile;
    public static event OnActiveDeactiveDel OnPickedFromTile;

    public void SetUp(int location, int itemId, int itemCount)
    {
        _locationTileId = location;
        _resourceValue.itemId = itemId;
        _resourceValue.itemCount = itemCount;

        OnAddedToTile?.Invoke(location, gameObject);
    }

    public void PickFromTile()
    {
        RemoveThisResourceFromTile();
        OnPickedFromTile?.Invoke(_locationTileId, gameObject);

        gameObject.SetActive(false);
    }

    private void RemoveThisResourceFromTile()
    {
        List<ResourcePoco> resources = new();
        resources.Add(_resourceValue);
        _onRemoveResourceFromTile.Raise(resources, _locationTileId);
    }
}
