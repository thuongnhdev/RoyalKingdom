using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScreenPositionConverter : MonoSingleton<ObjectScreenPositionConverter>
{
    [Header("Configs")]
    [SerializeField]
    private BuildingConstructionMenu _buildingConstructionMenu;
    [SerializeField]
    private BuildingItemProductionMenu _productionMenu;
    [SerializeField]
    private UIBand _bandMenu;

    private Camera mainCam => CameraGetter.Instance.MainCamera;

    public Vector2 MapPositionToCanvasPosition(GameObject go, Canvas canvas)
    {
        return CanvasUtils.WorldPositionToCanvasPosition(go.transform.position, canvas, mainCam);
    }

    public Vector2 MapPositionToCanvasPosition(Vector3 position, Canvas canvas)
    {
        return CanvasUtils.WorldPositionToCanvasPosition(position, canvas, mainCam);
    }

    public Vector2 MapBuildingPositionToCanvasPosition(int buildingObjId, Canvas canvas)
    {
        GameObject building = BuildingObjectFinder.Instance.GetBuildingByBuildingObjectId(buildingObjId);
        if (building == null)
        {
            return Vector2.zero;
        }

        return MapPositionToCanvasPosition(building, canvas);
    }

    public Vector2 MapCommand1ButtonPositionToCanvasPosition(int buildingObjId, Canvas canvas)
    {
        GameObject building = BuildingObjectFinder.Instance.GetBuildingByBuildingObjectId(buildingObjId);
        BuildingComponentGetter compGetter = building.GetComponent<BuildingComponentGetter>();
        return MapPositionToCanvasPosition(compGetter.BuildingCommand1Button, canvas);
    }

    public Vector2 MapCommand2ButtonPositionToCanvasPosition(int buildingObjId, Canvas canvas)
    {
        GameObject building = BuildingObjectFinder.Instance.GetBuildingByBuildingObjectId(buildingObjId);
        BuildingComponentGetter compGetter = building.GetComponent<BuildingComponentGetter>();
        return MapPositionToCanvasPosition(compGetter.BuildingCommand2Button, canvas);
    }

    public RectTransform BCM_GetBuildingPosition(int buildingId)
    {
        return _buildingConstructionMenu.GetBuildingCellPosition(buildingId);
    }

    public RectTransform BPM_GetItemPosition(int itemId)
    {
        return _productionMenu.GetItemPosition(itemId);
    }

    public RectTransform BPM_GetItemPositionByIndex(int index)
    {
        return _productionMenu.GetItemPositionByIndex(index);
    }

    public RectTransform BandUI_GetBandRect(int bandIndex)
    {
        return _bandMenu.GetBandCellPosition(bandIndex);
    }
}
