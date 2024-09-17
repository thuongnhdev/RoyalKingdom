using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBezierDataExporter : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private WorldMapBezierRoadPathList _paths;
    [SerializeField]
    private WorldMapRoadMeshList _roadMeshes;
    [SerializeField]
    private TableCityViewModel _cities;

    [Header("Inspec")]
    [SerializeField]
    private PathCreator[] _pathCreators;
    private PathCreator[] PathCreators
    {
        get
        {
            return GetComponentsInChildren<PathCreator>();
        }
    }

    public void CollectCitiesOnPath()
    {
        _paths.ClearPath();
        _roadMeshes.ClearMeshes();
        _cities.ResetAllCrossBezierPaths();
        for (int i = 0; i < PathCreators.Length; i++)
        {
            var pathCreator = PathCreators[i];
            var bezierPath = pathCreator.bezierPath;
            for (int j = 0; j < bezierPath.NumPoints; j++)
            {
                if (j % 3 != 0) // exclude control points
                {
                    continue;
                }
                Vector3 point = bezierPath.GetPoint(j);
                Vector3 worldPoint = pathCreator.transform.TransformPoint(point);
                var hits = Physics.OverlapSphere(worldPoint, 0.05f);
                AddCitiesToPath(pathCreator, hits);
            }

            AddRoadMesh(pathCreator);

            int numPoint = bezierPath.NumPoints;
            List<Vector3> bezierPoints = new();
            for (int j = 0; j < numPoint; j++)
            {
                if (j % 3 != 0) // exclude control points
                {
                    continue;
                }
                bezierPoints.Add(bezierPath.GetPoint(j));
            }
            var path = _paths.GetPath(pathCreator.GetInstanceID());
            path.controlSpacing = pathCreator.bezierPath.AutoControlLength;
            path.maxAngleError = pathCreator.EditorData.vertexPathMaxAngleError;
            path.minVertextDst = pathCreator.EditorData.vertexPathMinVertexSpacing;
            path.pathLength = pathCreator.path.length;
            path.bezierPoints = bezierPoints.ToArray();
            path.center = pathCreator.transform.position;
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(_paths);
        UnityEditor.EditorUtility.SetDirty(_roadMeshes);
        UnityEditor.EditorUtility.SetDirty(_cities);
#endif

    }
    private void AddCitiesToPath(PathCreator creator, Collider[] cityCandidates)
    {
        for (int i = 0; i < cityCandidates.Length; i++)
        {
            var candidate = cityCandidates[i];
            if (candidate is not BoxCollider)
            {
                continue;
            }
            var vertexPath = creator.path;

            float time = vertexPath.GetClosestTimeOnPath(candidate.transform.position);
            float distance = vertexPath.GetClosestDistanceAlongPath(candidate.transform.position);
            int pathId = creator.GetInstanceID();

            var city = candidate.GetComponent<CityMapModel>();
            _paths.AddCityToPath(pathId, new(city.cityData.id, distance, time), creator.name);

            _cities.AddCrossBezierPaths(city.cityData.id, creator.GetInstanceID());
        }
    }
    private void AddRoadMesh(PathCreator creator)
    {
        var meshFilter = creator.GetComponentInChildren<MeshFilter>();
        if (meshFilter == null)
        {
            return;
        }

        _roadMeshes.AddMesh(meshFilter.sharedMesh);
    }
}
