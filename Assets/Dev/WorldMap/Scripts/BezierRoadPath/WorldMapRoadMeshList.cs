using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMapRoadMeshList", menuName = "Uniflow/World/WorldMapRoadMeshList")]
public class WorldMapRoadMeshList : ScriptableObject
{
    [SerializeField]
    private List<RoadMeshData> _meshes = new();
    public List<RoadMeshData> RoadMeshes => new(_meshes);

    public void ClearMeshes()
    {
        _meshes.Clear();
    }

    public void AddMesh(Mesh mesh)
    {
        _meshes.Add(new()
        {
            vertices = mesh.vertices,
            tangents = mesh.tangents,
            uv = mesh.uv,
            tris = mesh.triangles,
            bound = mesh.bounds
        });
    }
}

[System.Serializable]
public class RoadMeshData
{
    public Vector3[] vertices;
    public Vector4[] tangents;
    public Vector2[] uv;
    public int[] tris;
    public Bounds bound;
}
