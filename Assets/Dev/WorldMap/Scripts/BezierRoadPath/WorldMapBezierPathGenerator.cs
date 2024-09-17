using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapBezierPathGenerator : MonoSingleton<WorldMapBezierPathGenerator>
{
    [Header("Reference - Read")]
    [SerializeField]
    private WorldMapBezierRoadPathList _paths;

    [Header("Config")]
    [SerializeField]
    private GameObject _pathTemplate;
    [SerializeField]
    private Transform _pathHolder;

    private Dictionary<int, PathCreator> _pathCreatorDict = new();

    public void GeneratePaths()
    {
        _pathCreatorDict.Clear();

        var paths = _paths.Paths;
        for (int i = 0; i < paths.Count; i++)
        {
            var path = paths[i];
            var pathObject = Instantiate(_pathTemplate, path.center, Quaternion.identity, _pathHolder);
            var pathCreator = pathObject.GetComponent<PathCreator>();
            pathCreator.bezierPath = new BezierPath(path.bezierPoints, space: PathSpace.xz);
            pathCreator.bezierPath.AutoControlLength = path.controlSpacing;
            pathCreator.EditorData.vertexPathMaxAngleError = path.maxAngleError;
            pathCreator.EditorData.vertexPathMinVertexSpacing = path.minVertextDst;

            _pathCreatorDict.Add(path.pathId, pathCreator);
        }
    }

    public PathCreator GetPathInstance(int pathId)
    {
        _pathCreatorDict.TryGetValue(pathId, out var path);
        if (path == null)
        {
            Logger.LogError($"No path instance for id [{pathId}]");
        }

        return path;
    }

    protected override void DoOnStart()
    {
        base.DoOnStart();
        GeneratePaths();
    }
}
