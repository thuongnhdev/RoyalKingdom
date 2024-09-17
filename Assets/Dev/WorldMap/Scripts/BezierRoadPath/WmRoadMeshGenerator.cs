using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modified from RoadMeshCreator. Only generate TopRoad mesh
/// </summary>
public class WmRoadMeshGenerator : PathSceneTool
{
    [Header("Road settings")]
    [SerializeField]
    private float roadWidth = .4f;
    [SerializeField]
    private bool flattenSurface;

    [Header("Material settings")]
    [SerializeField]
    private Material roadMaterial;
    [SerializeField]
    private float textureTiling = 1;

    [SerializeField]
    private Transform meshHolder;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    protected override void PathUpdated()
    {
        if (pathCreator == null)
        {
            return;
        }

        AssignMeshComponents();
        AssignMaterials();
        CreateRoadMesh();
    }

    void CreateRoadMesh()
    {
        Vector3[] verts = new Vector3[path.NumPoints * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
        int[] roadTriangles = new int[numTris * 3];

        int vertIndex = 0;
        int triIndex = 0;

        // Vertices for the top of the road are layed out:
        // 0  1
        // 2  3
        // and so on... So the triangle map 0,2,1 for example, defines a triangle from top left to bottom left to bottom right.
        int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

        bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
            Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

            // Find position to left and right of current path vertex
            Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
            Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

            // Add top of road vertices
            verts[vertIndex + 0] = vertSideA;
            verts[vertIndex + 1] = vertSideB;

            // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
            uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
            uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;

            // Set triangle indices
            if (i < path.NumPoints - 1 || path.isClosedLoop)
            {
                for (int j = 0; j < triangleMap.Length; j++)
                {
                    roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                }
            }

            vertIndex += 2;
            triIndex += 6;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 1;
        mesh.SetTriangles(roadTriangles, 0);
        mesh.RecalculateBounds();
    }

    // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
    void AssignMeshComponents()
    {
        meshHolder.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        meshHolder.localScale = Vector3.one;

        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.GetComponent<MeshFilter>())
        {
            meshHolder.gameObject.AddComponent<MeshFilter>();
        }
        if (!meshHolder.GetComponent<MeshRenderer>())
        {
            meshHolder.gameObject.AddComponent<MeshRenderer>();
        }

        meshRenderer = meshHolder.GetComponent<MeshRenderer>();
        meshFilter = meshHolder.GetComponent<MeshFilter>();
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        meshFilter.sharedMesh = mesh;
    }

    void AssignMaterials()
    {
        if (roadMaterial != null)
        {
            meshRenderer.sharedMaterials = new Material[] { roadMaterial };
            meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
        }
    }
}
