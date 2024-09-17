using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapRuntimeRoadGenerator : MonoBehaviour
{
    [SerializeField]
    private WorldMapBezierRoadPathList _paths;
    [SerializeField]
    private WorldMapRoadMeshList _roadMeshes;
    [SerializeField]
    private GameObject _runtimeRoadTemplate;

    public void GenerateRoad()
    {
        List<RoadMeshData> roadMeshes = _roadMeshes.RoadMeshes;
        for (int i = 0; i < roadMeshes.Count; i++)
        {
            GameObject road = Instantiate(_runtimeRoadTemplate, Vector3.zero, Quaternion.identity);
            RoadMeshData meshData = roadMeshes[i];
            Mesh mesh = new();
            mesh.vertices = meshData.vertices;
            mesh.tangents = meshData.tangents;
            mesh.triangles = meshData.tris;
            mesh.uv = meshData.uv;
            mesh.bounds = meshData.bound;

            road.GetComponent<MeshFilter>().mesh = mesh;

            // Use this code if you need adjust texture tiling based on road length;
            //float pathLength = _paths.Paths[i].pathLength;
            //road.GetComponent<MeshRenderer>().material.mainTextureScale = new(1f, pathLength);
        }
    }

    private void Start()
    {
        GenerateRoad();
    }
}
