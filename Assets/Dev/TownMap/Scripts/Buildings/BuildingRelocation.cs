using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuildingRelocation : DraggableTilingMovement
{
    [Header("Reference - Read")]
    [Header("Child fields")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingSO;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onBuildingPosChanged;
    [SerializeField]
    private UnityEvent _onBackToOriginalPos;

    [Header("Configs")]
    [SerializeField]
    private BuildingComponentGetter _compGetter;

    [Header("Inspec")]
    [SerializeField]
    private bool _relocatable = false;

    public bool Relocatable
    {
        get
        {
            return _relocatable;
        }
        set
        {
            _relocatable = value;
        }
    }
    [SerializeField]
    private int _locationTileId = -1;

    public void BackToOriginPos()
    {
        _tileTransform.MoveTo(_locationTileId);
        _onBackToOriginalPos.Invoke();
    }

    private void NofityBuildingPosChanged(int buildingId, int oldTileId, int newTileId)
    {
        _onBuildingPosChanged.Raise(buildingId, oldTileId, newTileId);
    }

    #region EditorOnly
#if UNITY_EDITOR
    protected override void ActionOnEndOfMove(Vector3 userPointedPos)
    {
        int newLocationCenterTileId = _tileTransform.GetCurrentTileId();

        if (newLocationCenterTileId == _locationTileId)
        {
            return;
        }

        var locationTileInfo = _userTownMapSO.GetTileInfo(_locationTileId);

        if (!ValidatePosition(newLocationCenterTileId))
        {
            BackToOriginPos();
            return;
        }

        _locationTileId = newLocationCenterTileId;
        NofityBuildingPosChanged(locationTileInfo.baseBuildingValue, locationTileInfo.tileId, newLocationCenterTileId);
    }

    protected override bool ValidatePosition(int buildingTileId)
    {
        int mapX = _userTownMapSO.xSize;
        int mapY = _userTownMapSO.ySize;

        var locationTileInfo = _userTownMapSO.GetTileInfo(_locationTileId);
        List<int> buildingOriginTiles = TemporaryRemoveRelocatedBuildingFromMap(locationTileInfo);

        IntegerVariable selectedBuildingObjId = AssetDatabase.LoadAssetAtPath<IntegerVariable>("Assets\\Dev\\TownMap\\SOs\\Var_Town_SelectedBuildingObjId.asset");
        bool isValid = TownMapHelper.EditorOnly_CanRelocateBuildingToTile(_userTownMapSO.TileDict, mapX, mapY, selectedBuildingObjId.Value, buildingTileId);

        RevertRelocationTiles(buildingOriginTiles);

        return isValid;
    }

    private void RevertRelocationTiles(List<int> buildingOldTiles)
    {
        Dictionary<int, TilePoco> tileDict = _userTownMapSO.TileDict;
        for (int i = 0; i < buildingOldTiles.Count; i++)
        {
            int tileId = buildingOldTiles[i];
            if (tileId == -1)
            {
                continue;
            }

            if (!tileDict.ContainsKey(tileId))
            {
                tileDict[tileId] = new TilePoco(tileId);
            }

        }
    }

    private List<int> TemporaryRemoveRelocatedBuildingFromMap(TilePoco locationTile)
    {
        Dictionary<int, TilePoco> tileDict = _userTownMapSO.TileDict;
        Vector2 buildingSize = TownMapHelper.EditorOnly_GetBaseBuildingSize(_compGetter.Operation.BuildingId);

        List<int> buildingTiles = TownMapHelper.TopLeftGetAllTilesInArea(_userTownMapSO.xSize, _userTownMapSO.ySize, buildingSize, locationTile.baseBuildingRootTile);
        for (int i = 0; i < buildingTiles.Count; i++)
        {
            int tileId = buildingTiles[i];
            if (!tileDict.ContainsKey(tileId))
            {
                tileDict[tileId] = new TilePoco(tileId);
            }
        }

        return buildingTiles;
    }
#endif
    #endregion
}
