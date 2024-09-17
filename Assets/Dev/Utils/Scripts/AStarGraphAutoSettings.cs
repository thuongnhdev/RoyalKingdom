using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGraphAutoSettings : MonoBehaviour
{
    [SerializeField]
    private AstarPath _aStarPath;
    public AstarPath AStarPath => _aStarPath;
    [SerializeField]
    private int _xGraphCount;
    [SerializeField]
    private int _yGraphCount;

    public void GenerateGridGraphSetup()
    {
        var graphs = _aStarPath.data.graphs;
        if (graphs == null || graphs.Length == 0)
        {
            Debug.LogWarning("Please add a pattern graph");
            return;
        }
        var patternGraph = (GridGraph)graphs[0];
        int xNodeCount = patternGraph.width;
        int yNodeCount = patternGraph.depth;
        float nodeSize = patternGraph.nodeSize;
        float nodeOffset = nodeSize * (xNodeCount - 1);
        Vector3 initCenter = patternGraph.center;
        LayerMask obstacle = patternGraph.collision.mask;

        List<GridGraph> newGraphs = new();
        for (int i = 0; i < _xGraphCount; i++)
        {
            for (int j = 0; j < _yGraphCount; j++)
            {
                GridGraph newGraph;
                if (i == 0 && j == 0)
                {
                    newGraph = patternGraph;
                }
                else
                {
                    newGraph = new GridGraph();
                }

                newGraph.SetDimensions(xNodeCount, yNodeCount, nodeSize);
                newGraph.center = new Vector3(initCenter.x + j * nodeOffset, initCenter.y, initCenter.z - i * nodeOffset);
                newGraph.collision.mask = obstacle;

                newGraphs.Add(newGraph);
            }
        }

        _aStarPath.data.graphs = newGraphs.ToArray();
    }

    public void ClearExceptPattern()
    {
        _aStarPath.data.graphs = new NavGraph[] { _aStarPath.data.graphs[0] };
    }
}
