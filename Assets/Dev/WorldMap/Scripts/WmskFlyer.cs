using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using WorldMapStrategyKit;

public class WmskFlyer : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private UserProfile _userProfile;
    [SerializeField]
    private LandStaticInfoList _landsStaticInfos;

    [Header("Reference - Write")]
    [SerializeField]
    private FloatVariable _distanceFromCamToLand;
    [SerializeField]
    private Vector3Variable _directionFromCamToLand;
    [SerializeField]
    private BoolVariable _myLandVisible;

    [Header("Configs - Scene Ref")]
    [SerializeField]
    private Camera _uiCam;

    private WMSK _map;
    private Camera _currentCamera;
    private int _userLandGeoId;
    private int _myLandIndex = -1;

    // TODO RK Remove after demo
    public void RemoveAfterDemo_FlyToFrance()
    {
        _map.allowUserZoom = false;
        _map.allowUserDrag = false;
        _map.FlyToProvince(50, 2f, 0.03f);
        Delay_AllowZoom().Forget();
    }

    public void FlyToMyLand()
    {
        /*
        if (_myLandIndex == -1)
        {
            int myLandId = _landsStaticInfos.GetLandGeoId(_userProfile.landId);
            _myLandIndex = _map.GetProvinceIndex(myLandId);
        }
        _map.allowUserZoom = false;
        _map.allowUserDrag = false;
        _map.FlyToProvince(_myLandIndex, 1f, _map.lastKnownZoomLevel);
        Delay_AllowZoom().Forget();
        */
    }

    public void CalculateCamAndLand()
    {
        Perframe_CalculateFromCameraToMyLandDirection().Forget();
    }

    private CancellationTokenSource _camAndLandToken;
    private async UniTaskVoid Perframe_CalculateFromCameraToMyLandDirection()
    {
        await UniTask.NextFrame();

        _camAndLandToken?.Cancel();
        _camAndLandToken = new();
        int landIndex = _map.GetProvinceIndex(_userLandGeoId);
        if (landIndex == -1)
        {
            return;
        }

        Vector2 landPos = _map.GetProvinceCenter(landIndex);
        Vector3 landWorldPos = _map.transform.TransformPoint(landPos);
        Vector2 camPos;
        Vector2 distanceVector;
        int frameCount = 0;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_camAndLandToken.Token))
        {
            frameCount++;
            if (frameCount % 10 != 0)
            {
                continue;
            }

            camPos = _currentCamera.transform.position;
            camPos = _map.transform.InverseTransformPoint(camPos);

            _directionFromCamToLand.Value = landPos - camPos;
            distanceVector.x = _directionFromCamToLand.Value.x * 40000f;
            distanceVector.y = _directionFromCamToLand.Value.y * 20000f;
            _distanceFromCamToLand.Value = distanceVector.magnitude;

            Vector3 landScreenPoint = _currentCamera.WorldToViewportPoint(landWorldPos);
            _myLandVisible.Value = (0f <= landScreenPoint.x && landScreenPoint.x <= 1f) &&
                                   (0f <= landScreenPoint.y && landScreenPoint.y <= 1f);
        }
    }

    private async UniTaskVoid Delay_AllowZoom()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(2f));
        _map.allowUserZoom = true;
        _map.allowUserDrag = true;
    }

    private void OnEnable()
    {
        /*
        _map = WMSK.instance;
        _currentCamera = _map.currentCamera;
        _userLandGeoId = _landsStaticInfos.GetLandGeoId(_userProfile.landId);

        Perframe_CalculateFromCameraToMyLandDirection().Forget();
        */
    }

    private void OnDisable()
    {
        _camAndLandToken?.Cancel();
    }
}
