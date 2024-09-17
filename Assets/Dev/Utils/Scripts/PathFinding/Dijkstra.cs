using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra
{
    private class PathRecord
    {
        public int nodeId;
        public float pathLength;
        public int previousNode;
    }

    private static Dictionary<int, PathRecord> _recordTable = new();
    private static HashSet<int> _visited = new();
    private static HashSet<int> _unvisited = new();

    public static bool CalculatePath(List<IDijktraNode> graph, IDijktraNode source, IDijktraNode destination, out List<int> path)
    {
        _recordTable.Clear();
        _visited.Clear();
        _unvisited.Clear();
        Dictionary<int, IDijktraNode> nodeMap = new();
        for (int i = 0; i < graph.Count; i++)
        {
            nodeMap.Add(graph[i].GetId(), graph[i]);
            _unvisited.Add(graph[i].GetId());
        }

        path = new();
        path.Add(destination.GetId());
        if (source.GetId() == destination.GetId())
        {
            return true;
        }

        _recordTable.Add(source.GetId(), new PathRecord() { nodeId = source.GetId(), pathLength = 0, previousNode = 0 });

        List<int> neighbors = source.GetNeighbors();
        if (neighbors.Count == 0)
        {
            return false;
        }

        Stack<int> trace = new();
        trace.Push(source.GetId());
        while (_unvisited.Count > 0)
        {
            if (trace.Count == 0)
            {
                break;
            }

            IDijktraNode currentNode = nodeMap[trace.Pop()];
            _visited.Add(currentNode.GetId());
            _unvisited.Remove(currentNode.GetId());

            neighbors = currentNode.GetNeighbors();
            for (int i = 0; i < neighbors.Count; i++)
            {
                int neighbor = neighbors[i];
                if (_visited.Contains(neighbor))
                {
                    continue;
                }

                trace.Push(nodeMap[neighbor].GetId());
                _recordTable.TryGetValue(currentNode.GetId(), out var currentRecord);
                if (currentNode == null)
                {
                    currentRecord = new() { nodeId = currentNode.GetId(), pathLength = 0, previousNode = 0 };
                }    
                float lengthFromSource = currentRecord.pathLength + currentNode.GetPathLengthToNeighbor(neighbor);

                _recordTable.TryGetValue(neighbor, out var record);
                if (record == null)
                {
                    record = new() { nodeId = neighbor, pathLength = lengthFromSource, previousNode = currentNode.GetId() };
                    _recordTable.Add(record.nodeId, record);
                    continue;
                }
                if (lengthFromSource < record.pathLength)
                {
                    record.pathLength = lengthFromSource;
                    record.previousNode = currentNode.GetId();
                }
            }
        }

        bool pathFound = _visited.Contains(destination.GetId());
        if (pathFound)
        {
            int previousNode = _recordTable[destination.GetId()].previousNode;
            path.Add(previousNode);
            int sourceNode = source.GetId();
            while (previousNode != sourceNode)
            {
                previousNode = _recordTable[previousNode].previousNode;
                path.Add(previousNode);
            }

            path.Reverse();
        }

        return pathFound;
    }
}

public interface IDijktraNode
{
    int GetId();
    float GetPathLengthToNeighbor(int neighbor);
    List<int> GetNeighbors();
}
