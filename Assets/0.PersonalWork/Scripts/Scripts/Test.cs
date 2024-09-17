using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.FlatBuffers;
using Town.Tile;
using Pathfinding;
using System;
using Cysharp.Threading;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using WorldMapStrategyKit;
using System.Text;

public class Test : MonoBehaviour
{
    [SerializeField]
    private CharacterController _charCon;
    [SerializeField]
    private AIPath _aiPath;
    [SerializeField]
    private Vector3Variable _destination;
    [SerializeField]
    private Vector3Variable _testVector3;

    [SerializeField]
    private UnityEvent _onStartMove;
    [SerializeField]
    private UnityEvent _onStopMove;

    [SerializeField]
    private GameEvent _testEvent;

    private CancellationTokenSource _moveTrackToken;
    private UniTaskCancelableAsyncEnumerable<AsyncUnit> _moveTrackLoop;

    public void RemoveAllButFrance()
    {
        WMSK map = WMSK.instance;

        int franceIndex = map.GetCountryIndex("France");
        for (int i = 0; i < franceIndex; i++)
        {
            map.CountryDelete(0, true);
        }
        for (int i = map.countries.Length - 1; 1 <= i; i--)
        {
            map.CountryDelete(i, true);
        }

        map.CountryCreateProvincesPool("Pool", true);

        TextFileIO.WriteText("FrancePool.txt", map.GetCountryGeoData());
        TextFileIO.WriteText("FranceProvinces.txt", map.GetProvinceGeoData());
        TextFileIO.WriteText("FranceCities.txt", map.GetCityGeoData());
        TextFileIO.WriteText("MountPoint.txt", map.GetMountPointsGeoData());
    }

    public void ExportLandsData()
    {
        WMSK map = WMSK.instance;

        StringBuilder sb = new();
        Province[] provinces = map.provinces;
        for (int i = 0; i < provinces.Length; i++)
        {
            var p = provinces[i];
            sb.Append(p.uniqueId).Append(',').AppendLine(p.name);
        }

        TextFileIO.WriteText("Lands.csv", sb.ToString());
    }

    public class Node : IDijktraNode
    {
        private int id;
        private List<(int, int)> neighbors = new();
        public int GetId()
        {
            return id;
        }

        public Node(int id, params (int, int)[] neighbors)
        {
            this.id = id;
            for (int i = 0; i < neighbors.Length; i++)
            {
                this.neighbors.Add(neighbors[i]);
            }
        }

        public List<int> GetNeighbors()
        {
            List<int> result = new();
            for (int i = 0; i < neighbors.Count; i++)
            {
                result.Add(neighbors[i].Item1);
            }

            return result;
        }

        public float GetPathLengthToNeighbor(int neighbor)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].Item1 == neighbor)
                {
                    return neighbors[i].Item2;
                }
            }

            return 0f;
        }
    }

    public void test(int index)
    {
        List<IDijktraNode> graph = new();
        graph.Add(new Node(1, (2, 4), (3, 9)));
        graph.Add(new Node(2, (1, 4), (4, 7)));
        graph.Add(new Node(3, (1, 9), (4, 1), (8, 1)));
        graph.Add(new Node(4, (2, 7), (3, 1), (5, 4), (7, 2)));
        graph.Add(new Node(5, (4, 4), (6, 6), (7, 10)));
        graph.Add(new Node(6, (5, 6)));
        graph.Add(new Node(7, (4, 2), (5, 10), (6, 1), (8, 1)));
        graph.Add(new Node(8, (3, 1), (7, 1)));
        Dijkstra.CalculatePath(graph, graph[0], graph[index], out var path);
        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(path[i]);
        }

    }    

    private async void MoveToTarget(Vector3 destination)
    {
        _aiPath.destination = destination;

        _onStartMove.Invoke();

        _moveTrackToken.Cancel();
        _moveTrackToken = new CancellationTokenSource();

        await UniTask.NextFrame();

        _moveTrackLoop = UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_moveTrackToken.Token);

        await foreach (var _ in _moveTrackLoop)
        {
            if (_aiPath.remainingDistance < 0.2f)
            {
                _onStopMove.Invoke();
                _moveTrackToken.Cancel();
                break;
            }
        }
    }

    private void TestEvent(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        var test = (int)args[0];
        Debug.Log(test);
    }

    private void ChangePos(Vector3 newValue)
    {
        transform.localPosition = newValue;
    }

    private void OnEnable()
    {
        _testEvent.Subcribe(TestEvent);
    }

    private void OnDisable()
    {
        _testEvent.Unsubcribe(TestEvent);
    }
}
