using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TownMapHelper
{
    private static (int x, int y)[] _directions = new (int, int)[8]{(1, 0), (-1, 0), (0, 1), (0, -1),
                                                                    (1, 1), (-1, 1), (1, -1), (-1, -1)};

    public static bool IsTileOccupiable(Dictionary<int, TilePoco> mapDict, int tileId)
    {
        if (tileId == -1)
        {
            return false;
        }
        mapDict.TryGetValue(tileId, out var tile);

        if (tile == null)
        {
            return true;
        }

        if (!tile.buildable)
        {
            return false;
        }

        if (tile.baseBuildingValue != 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Map coordinate has (0, 0) at topleft.
    /// </summary>
    public static int GetTileIdBaseOnCurrentTile(int mapX, int mapY, int currentTileId, int horMove, int verMove)
    {
        if (currentTileId == -1)
        {
            return -1;
        }

        Vector2 currentTileAxis = FromTileIdToAxis(mapX, mapY, currentTileId);
        Vector2 newAxis = new Vector2(currentTileAxis.x + horMove, currentTileAxis.y + verMove);

        if (newAxis.x < 0 || mapX <= newAxis.x ||
            newAxis.y < 0 || mapY <= newAxis.y)
        {
            return -1;
        }

        return FromAxisToTileId(mapX, newAxis);
    }

    public static List<int> GetNeighborTiles(int mapX, int mapY, int currentTile, bool eightDimension = false)
    {
        List<int> neighbors = new();

        int dimension = eightDimension ? 8 : 4;
        for (int i = 0; i < dimension; i++)
        {
            (int x, int y) = _directions[i];
            int neighbor = GetTileIdBaseOnCurrentTile(mapX, mapY, currentTile, x, y);
            if (neighbor == -1)
            {
                continue;
            }

            neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public static List<int> GetNeighborTilesOfArea(int mapX, int mapY, int topLeft, Vector2 areaSize, bool eightDimension = false)
    {
        List<int> neighbors = new();
        int xArea = (int)areaSize.x;
        int yArea = (int)areaSize.y;

        int aboveTopLeft = GetTileIdBaseOnCurrentTile(mapX, mapY, topLeft, 0, -1);
        int rightTopRight = GetTileIdBaseOnCurrentTile(mapX, mapY, topLeft, xArea, 0);
        int underBottomRight = GetTileIdBaseOnCurrentTile(mapX, mapY, topLeft, xArea - 1, yArea);
        int leftBottomLeft = GetTileIdBaseOnCurrentTile(mapX, mapY, topLeft, -1, yArea - 1);

        int xSearch = xArea;
        int ySearch = yArea;
        if (eightDimension)
        {
            xSearch++;
            ySearch++;
        }

        for (int i = 0; i <= xSearch; i++)
        {
            int aboveNeighbor = GetTileIdBaseOnCurrentTile(mapX, mapY, aboveTopLeft, i, 0);
            int underNeighbor = GetTileIdBaseOnCurrentTile(mapX, mapY, underBottomRight, -i, 0);
            if (aboveNeighbor != -1)
            {
                neighbors.Add(aboveNeighbor);
            }
            if (underNeighbor != -1)
            {
                neighbors.Add(underNeighbor);
            }
        }

        for (int i = 0; i <= ySearch; i++)
        {
            int rightNeighbor = GetTileIdBaseOnCurrentTile(mapX, mapY, rightTopRight, 0, i);
            int leftNeighbor = GetTileIdBaseOnCurrentTile(mapX, mapY, leftBottomLeft, 0, -i);
            if (rightNeighbor != -1)
            {
                neighbors.Add(rightNeighbor);
            }
            if (leftNeighbor != -1)
            {
                neighbors.Add(leftNeighbor);
            }
        }

        return neighbors;
    }

    public static Vector2 FromTileIdToAxis(int mapX, int mapY, int tileId)
    {
        int x = tileId % mapX;
        int y = tileId / mapX;

        return new Vector2(x, y);
    }

    public static int FromAxisToTileId(int mapX, Vector2 axis)
    {
        return (int)(axis.x + axis.y * mapX);
    }

    public static int FromAxisToTileId(int mapX, int mapY, int xAxis, int yAxis)
    {
        if (!IsValidAxis(mapX, mapY, xAxis, yAxis))
        {
            return -1;
        }
        return xAxis + yAxis * mapX;
    }

    public static int FromTopLeftToCenterTile(int mapX, int mapY, Vector2 area, int topLeftTile)
    {
        int horMove = (int)(area.x) / 2;
        int verMove = (int)(area.y) / 2;

        return GetTileIdBaseOnCurrentTile(mapX, mapY, topLeftTile, horMove, verMove);
    }

    public static int FromCenterToTopLeftTile(int mapX, int mapY, Vector2 area, int centerTile)
    {
        int horMove = (int)(area.x) / 2;
        int verMove = (int)(area.y) / 2;

        return GetTileIdBaseOnCurrentTile(mapX, mapY, centerTile, -horMove, -verMove);
    }

    /// <summary>
    /// direction of tile1 ------> tile2
    /// </summary>
    public static Vector2 GetDirectionOf2Tiles(int mapX, int mapY, int tileId1, int tileId2, bool fourDirOnly = false)
    {
        Vector2 tile1Axis = FromTileIdToAxis(mapX, mapY, tileId1);
        Vector2 tile2Axis = FromTileIdToAxis(mapX, mapY, tileId2);

        Vector2 direction = new()
        {
            x = tile2Axis.x - tile1Axis.x,
            y = tile1Axis.y - tile2Axis.y // our tile coordinate system has (0, 0) tile at top-left
        };

        float x = direction.x;
        float y = direction.y;
        if (fourDirOnly)
        {
            if (Mathf.Abs(x) < Mathf.Abs(y))
            {
                direction.x = 0f;
                direction.y = Mathf.Sign(y);
            }
            else
            {
                direction.x = Mathf.Sign(x);
                direction.y = 0f;
            }
        }

        return direction;
    }

    /// <summary>
    /// Four-direction only
    /// </summary>
    public static Vector2 GetDirectionOfAreaAndNeighborTile(int mapX, int mapY, List<int> area, int neighbor)
    {
        Vector2 direction = Vector2.up; // non-rotated building has default direction as Up due to coordination system which has (0;0) at top left
        for (int i = 0; i < area.Count; i++)
        {
            direction = GetDirectionOf2Tiles(mapX, mapY, area[i], neighbor);
            if (direction.sqrMagnitude - 1f <= float.Epsilon)
            {
                break;
            }
        }

        return direction;
    }

    public static int GetTileIdAtPosition(int mapX, int mapY, Vector3 localPosition, float xOffset = 1f, float yOffset = 1f)
    {
        Vector2 axis = GetTileAxisAtPosition(mapX, mapY, localPosition, xOffset, yOffset);

        if (!IsValidAxis(mapX, mapY, axis))
        {
            return -1;
        }

        return FromAxisToTileId(mapX, axis);
    }

    public static Vector2 GetTileAxisAtPosition(int mapX, int mapY, Vector3 localPosition, float xOffset = 1f, float yOffset = 1f)
    {
        Vector3 mapOriginPoint = GetTileZeroLocalPosition(mapX, mapY, xOffset, yOffset) + new Vector3(-0.5f, 0f, 0.5f);

        return new Vector2
        {
            x = Mathf.FloorToInt((localPosition.x - mapOriginPoint.x) / xOffset),
            y = Mathf.FloorToInt((mapOriginPoint.z - localPosition.z) / yOffset)
        };
    }

    public static Vector3 GetTileZeroLocalPosition(int mapX, int mapY, float xOffset = 1f, float yOffset = 1f)
    {
        return new Vector3
        {
            x = -mapX / 2f + xOffset / 2f,
            y = 0f,
            z = mapY / 2f - yOffset / 2f
        };
    }

    public static Vector3 GetTileLocalPosition(int mapX, int mapY, int tileId, float xOffset = 1f, float yOffset = 1f)
    {
        Vector2 tileAxis = FromTileIdToAxis(mapX, mapY, tileId);
        Vector3 tileZeroPos = GetTileZeroLocalPosition(mapX, mapY);

        return new Vector3
        {
            x = tileZeroPos.x + tileAxis.x * xOffset,
            y = 0f,
            z = tileZeroPos.z - tileAxis.y * yOffset
        };
    }

    public static bool IsValidAxis(int mapX, int mapY, int xAxis, int yAxis)
    {
        return (0 <= xAxis && xAxis < mapX) && (0 <= yAxis && yAxis < mapY);
    }

    public static bool IsValidAxis(int mapX, int mapY, Vector2 axis)
    {
        return IsValidAxis(mapX, mapY, (int)axis.x, (int)axis.y);
    }

    /// <summary>
    /// The input tile will be considered as top-left tile of building
    /// </summary>
    public static bool TopLeftCanBuildBuildingOnTile(Dictionary<int, TilePoco> mapDict, int mapX, int mapY, Vector2 buildingSize, int topLeftTileXAxis, int topLeftTileYAxis)
    {
        // Go right then Go down
        for (int i = 0; i < buildingSize.y; i++)
        {
            for (int j = 0; j < buildingSize.x; j++)
            {
                int neighborTileId = FromAxisToTileId(mapX, mapY, topLeftTileXAxis + j, topLeftTileYAxis + i);

                mapDict.TryGetValue(neighborTileId, out var tile);
                if (tile == null)
                {
                    tile = new TilePoco(neighborTileId);
                }

                if (!ItemHelper.IsResourcesEmpty(tile.containedResources))
                {
                    return false;
                }

                if (!IsTileOccupiable(mapDict, neighborTileId))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool TopLeftCanBuildBuildingOnTile(Dictionary<int, TilePoco> mapDict, int mapX, int mapY, Vector2 buildingSize, int tileId)
    {
        if (tileId == -1)
        {
            return false;
        }

        Vector2 tileAxis = FromTileIdToAxis(mapX, mapY, tileId);
        return TopLeftCanBuildBuildingOnTile(mapDict, mapX, mapY, buildingSize, (int)tileAxis.x, (int)tileAxis.y);
    }

    /// <summary>
    /// input tile is considered as center tile of building
    /// </summary>
    public static bool CenterCanBuildBuildingOnTile(Dictionary<int, TilePoco> mapDict, int mapX, int mapY, Vector2 buildingSize, int centerTileId)
    {
        Vector2 tileAxis = FromTileIdToAxis(mapX, mapY, centerTileId);
        return CenterCanBuildBuildingOnTile(mapDict, mapX, mapY, buildingSize, (int)tileAxis.x, (int)tileAxis.y);
    }

    /// <summary>
    /// input tile is considered as center tile of building
    /// </summary>
    public static bool CenterCanBuildBuildingOnTile(Dictionary<int, TilePoco> mapDict, int mapX, int mapY, Vector2 buildingSize, int centerTileXAxis, int centerTileYAxis)
    {
        int topLeftX = centerTileXAxis - (int)buildingSize.x / 2;
        int topLeftY = centerTileYAxis - (int)buildingSize.y / 2;

        return TopLeftCanBuildBuildingOnTile(mapDict, mapX, mapY, buildingSize, topLeftX, topLeftY);
    }

    /// <summary>
    /// init tile is top-left tile of area
    /// </summary>
    public static List<int> TopLeftGetAllTilesInArea(int mapX, int mapY,  Vector2 area, int initTileId)
    {
        Vector2 initTileAxis = FromTileIdToAxis(mapX, mapY, initTileId);
        return TopLeftGetAllTilesInArea(mapX, mapY, area, initTileAxis);
    }

    public static List<int> TopLeftGetAllTilesInArea(int mapX, int mapY, Vector2 area, Vector2 initTileAxis)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < area.y; i++)
        {
            for (int j = 0; j < area.x; j++)
            {
                int tileId = FromAxisToTileId(mapX, mapY, (int)(initTileAxis.x + j), (int)(initTileAxis.y + i));

                if (tileId == -1)
                {
                    continue;
                }

                result.Add(tileId);
            }
        }

        return result;
    }

    public static List<int> GetAllTilesOfBuildingOccupancy(Dictionary<int, TilePoco> mapDict, int mapX, int mapY, Vector2 buildingSize, TilePoco selectedTile)
    {
        if (selectedTile == null)
        {
            return new List<int>();
        }

        return TopLeftGetAllTilesInArea(mapX, mapY, buildingSize, selectedTile.baseBuildingRootTile);
    }

    public static List<int> GetTileRectAreaDefineBy2Tiles(int mapX, int mapY, int fromTile, int toTile)
    {
        Vector2 fromAxis = FromTileIdToAxis(mapX, mapY, fromTile);
        Vector2 toAxis = FromTileIdToAxis(mapX, mapY, toTile);

        Vector2 initTile = new Vector2
        {
            x = Mathf.Min(fromAxis.x, toAxis.x),
            y = Mathf.Min(fromAxis.y, toAxis.y)
        };

        Vector2 areaSize = new Vector2
        {
            x = Mathf.Abs(fromAxis.x - toAxis.x) + 1,
            y = Mathf.Abs(fromAxis.y - toAxis.y) + 1
        };

        return TopLeftGetAllTilesInArea(mapX, mapY, areaSize, initTile);
    }

    public static Vector3 GetBuildingPosition(int mapX, int mapY, int centerTileId, Vector2 buildingSize, float xOffset = 1f, float yOffset = 1f)
    {
        Vector3 buildingPos = GetTileLocalPosition(mapX, mapY, centerTileId);

        if ((int)(buildingSize.x) % 2 == 0)
        {
            buildingPos.x -= xOffset / 2f;
        }

        if ((int)(buildingSize.y) % 2 == 0)
        {
            buildingPos.z += yOffset / 2f;
        }

        return buildingPos;
    }

    /// <summary>
    /// Find by right, down, left, up then repeat (clockwise).
    /// The result is tileId that top-left tile of building should be set into
    /// </summary>
    public static int FindLocationForBuildingAroundTile(Dictionary<int, TilePoco> tileDict, int mapX, int mapY, Vector2 buildingSize, int initTileId)
    {
        if (initTileId == -1)
        {
            return -1;
        }

        if (TopLeftCanBuildBuildingOnTile(tileDict, mapX, mapY, buildingSize, initTileId))
        {
            return initTileId;
        }

        (int x, int y)[] findDirections = { (1, 0), (0, 1), (-1, 0), (0, -1) };
        Vector2 initAxis = FromTileIdToAxis(mapX, mapY, initTileId);
        int moveStep = 1;
        int moveTime = 0;

        (int x, int y) currentAxis = ((int)initAxis.x, (int)initAxis.y);
        while (true)
        {

            if (Mathf.Abs(currentAxis.x - (int)initAxis.x) > mapX ||
                Mathf.Abs(currentAxis.y - (int)initAxis.y) > mapY)
            {
                return -1;
            }

            for (int count = 0; count < 2; count++) // increase step after 2 times of loop
            {
                (int x, int y) direction = findDirections[moveTime % 4];
                for (int i = 0; i < moveStep; i++)
                {
                    currentAxis.x += direction.x;
                    currentAxis.y += direction.y;

                    int id = FromAxisToTileId(mapX, mapY, currentAxis.x, currentAxis.y);

                    if (TopLeftCanBuildBuildingOnTile(tileDict, mapX, mapY, buildingSize, currentAxis.x, currentAxis.y))
                    {
                        return FromAxisToTileId(mapX, mapY, currentAxis.x, currentAxis.y);
                    }

                }
                moveTime++;
            }

            moveStep++;
        }
    }

    #region EditorOnly
#if UNITY_EDITOR

    private static TownBaseBuildingSOList _townBaseBuildingSO;
    private static TownBaseBuildingSOList TownBaseBuildingSO
    {
        get
        {
            if (_townBaseBuildingSO == null)
            {
                _townBaseBuildingSO = AssetDatabase.LoadAssetAtPath<TownBaseBuildingSOList>("Assets\\Dev\\TownMap\\SOs\\TownBuildingList.asset");
            }

            return _townBaseBuildingSO;
        }
    }
    private static UserBuildingList _userBuilings;
    private static UserBuildingList UserBuildings
    {
        get
        {
            if (_userBuilings == null)
            {
                _userBuilings = AssetDatabase.LoadAssetAtPath<UserBuildingList>("Assets\\Dev\\UserData\\UserBuildings\\SOs\\UserBuildingList.asset");
            }

            return _userBuilings;
        }
    }

    public static Vector2 EditorOnly_GetBaseBuildingSize(int baseBuildingId)
    {
        var building = TownBaseBuildingSO.GetBaseBuilding(baseBuildingId);
        if (building == null)
        {
            return Vector2.zero;
        }

        return building.size;
    }

    public static bool EditorOnly_CanRelocateBuildingToTile(Dictionary<int, TilePoco> mapDict, int mapX, int mapY, int buildingObjId, int centerTile)
    {
        var userBuilding = UserBuildings.GetBuilding(buildingObjId);
        Vector2 buildingSize = TownBaseBuildingSO.GetBuildingSize(userBuilding.buildingId);
        int topLeftTile = FromCenterToTopLeftTile(mapX, mapY, buildingSize, centerTile);
        Vector2 tileAxis = FromTileIdToAxis(mapX, mapY, topLeftTile);

        int buidingOriginTopLeftTile = FromCenterToTopLeftTile(mapX, mapY, buildingSize, userBuilding.locationTileId);
        List<int> originArea = TopLeftGetAllTilesInArea(mapX, mapY, buildingSize, buidingOriginTopLeftTile);

        for (int i = 0; i < buildingSize.y; i++)
        {
            for (int j = 0; j < buildingSize.x; j++)
            {
                int neighborTileId = FromAxisToTileId(mapX, mapY, (int)(tileAxis.x) + j, (int)(tileAxis.y) + i);

                mapDict.TryGetValue(neighborTileId, out var tile);
                if (tile == null)
                {
                    tile = new TilePoco(neighborTileId);
                }

                if (originArea.Contains(tile.tileId))
                {
                    continue;
                }

                if (!ItemHelper.IsResourcesEmpty(tile.containedResources))
                {
                    return false;
                }

                if (!IsTileOccupiable(mapDict, neighborTileId))
                {
                    return false;
                }
            }
        }

        return true;
    }
#endif
    #endregion
}
