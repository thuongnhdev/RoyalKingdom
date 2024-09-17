using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFinder : MonoBehaviour
{
    // key = locationTileId, value = resource objects in this tile
    private Dictionary<int, HashSet<GameObject>> _resourceDict = new();

    public List<GameObject> GetResourceInTile(int tileId)
    {
        _resourceDict.TryGetValue(tileId, out var resources);
        if (resources == null)
        {
            resources = new HashSet<GameObject>();
        }

        List<GameObject> returnResource = new();
        foreach (var resource in resources)
        {
            returnResource.Add(resource);
        }

        return returnResource;
    }

    private void TrackResource(int locationTileId, GameObject resource)
    {
        _resourceDict.TryGetValue(locationTileId, out var resources);
        if (resources == null)
        {
            resources = new HashSet<GameObject>();
        }

        resources.Add(resource);
        _resourceDict[locationTileId] = resources;
    }

    private void UntrackResource(int location, GameObject resource)
    { 
        _resourceDict.TryGetValue(location, out var resources);
        if (resources == null || resources.Count == 0)
        {
            return;
        }

        resources.Remove(resource);
    }

    private void OnEnable()
    {
        ResourceObject.OnAddedToTile += TrackResource;
        ResourceObject.OnPickedFromTile += UntrackResource;
    }

    private void OnDisable()
    {
        ResourceObject.OnAddedToTile -= TrackResource;
        ResourceObject.OnPickedFromTile -= UntrackResource;
    }
}
