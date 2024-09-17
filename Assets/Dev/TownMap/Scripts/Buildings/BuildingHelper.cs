using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHelper
{
    public static float GetDestructionRefundRate(BuildingStatus status)
    {
        if (status == BuildingStatus.WaitForConstructingResource || status == BuildingStatus.WaitingForUpgradeResource)
        {
            return 1f;
        }

        if (status == BuildingStatus.OnConstruction || status == BuildingStatus.Upgrading)
        {
            return 0.8f;
        }

        return 0.5f;
    }

    public static float GetCancelUpgradeRefundRate(BuildingStatus status)
    {
        if (status == BuildingStatus.WaitingForUpgradeResource)
        {
            return 1f;
        }

        return 0.8f;
    }

    public static float GetDestructionTimeCostRate(BuildingStatus status)
    {
        if (status == BuildingStatus.WaitForConstructingResource ||
            status == BuildingStatus.OnConstruction)
        {
            return 0f;
        }

        return 1f;
    }

    public static bool CanBuildingProduce(BuildingStatus status)
    {
        return status == BuildingStatus.Idle ||
               status == BuildingStatus.WaitingForProductResource ||
               status == BuildingStatus.Producing;
    }

    public static bool IsOnProgressStatus(BuildingStatus status)
    {
        return status == BuildingStatus.OnConstruction ||
                                status == BuildingStatus.Upgrading ||
                                status == BuildingStatus.OnDestruction ||
                                status == BuildingStatus.Producing;
    }

    public static Vector3 GetDoorPosition(Vector3 doorDirection, Vector3 buildingPosition, float buildingRotation)
    {
        return buildingPosition + Quaternion.Euler(0f, buildingRotation, 0f) * doorDirection;
    }

    public static int FromDoorPosToDoorIndex(Vector3 doorPos, Vector3 buildingPos, float buildingRotation, List<Vector2> doorsDirection)
    {
        if (doorsDirection == null || doorsDirection.Count == 0)
        {
            return -1;
        }
        Vector2 doorDir = new Vector2(doorPos.x - buildingPos.x, doorPos.z - buildingPos.z);
        doorDir = Quaternion.Euler(0f, buildingRotation, 0f) * doorDir;

        for (int i = 0; i < doorsDirection.Count; i++)
        {
            if (Vector2.SqrMagnitude(doorDir - doorsDirection[i]) <= float.Epsilon)
            {
                return i;
            }
        }

        return -1;
    }
}
