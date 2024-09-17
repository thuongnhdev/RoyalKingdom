using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAreaScanner : MonoBehaviour
{
    [SerializeField]
    private AstarPath _aStar;

    [Header("Reference - Read")]
    [SerializeField]
    private TownMapSO _townMap;

    [Header("Events in")]
    [SerializeField]
    private GameEvent _onAskedRescanMap;

    public void ScanAll()
    {
        DelayScan().Forget();
    }
    private async UniTaskVoid DelayScan()
    {
        await UniTask.NextFrame();
        _aStar.Scan();
    }

    private void HandleRescanEvent(object[] args)
    {
        if (args.Length == 0)
        {
            return;
        }

        int topLeftTile = (int)args[0];
        Vector2 size = (Vector2)args[1];
        Vector3 topLeftPosition = TownMapHelper.GetTileLocalPosition(_townMap.xSize, _townMap.ySize, topLeftTile);

        Vector3 scenCenter = new Vector3(topLeftPosition.x + size.x / 2f, 0f, topLeftPosition.z - size.y / 2f);
        Vector3 scanBounds = new Vector3(size.x, 1f, size.y);

        Bounds bounds = new Bounds(scenCenter, scanBounds);
        _aStar.UpdateGraphs(bounds);
    }

    private void OnEnable()
    {
        _onAskedRescanMap.Subcribe(HandleRescanEvent);
    }

    private void OnDisable()
    {
        _onAskedRescanMap.Subcribe(HandleRescanEvent);
    }
}
