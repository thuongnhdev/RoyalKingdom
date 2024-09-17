using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BuildingFoundationSetter : MonoBehaviour
{
    [Header("Reference - Write")]
    [SerializeField]
    private BoolVariable _foundationIsOpening;

    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _foundTopLeftTile;
    [SerializeField]
    private BoolVariable _isValidPosition;
    [SerializeField]
    private TownBaseBuildingSOList _buildingListSO;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssetsList;
    [SerializeField]
    private TownMapSO _townMap;

    [Header("Config")]
    [SerializeField]
    private bool _isRoad = false;
    [SerializeField]
    private LayerMask _buildingModelLayer;
    [SerializeField]
    private BuildingFoundationMove _foundationMove;
    [SerializeField]
    private MeshRenderer _foundationRenderer;
    [SerializeField]
    private Transform _interactiveObject;
    [SerializeField]
    private Canvas _foundationUI;
    [SerializeField]
    private Transform _buildingModelHolder;
    [SerializeField]
    private TilingTransform _tilingTransform;
    [SerializeField]
    private Color _modelTintColor;
    [SerializeField]
    private Color _invalidPositionColor;
    [SerializeField]
    private float _tintHalfPeriod = 1f;

    private GameObject _buildingModelObj;
    private Vector4 _bufferVector;
    private bool _isTweeningModelColor = false;

    public void SetUp(int buildingId, int foundationCenterTileId)
    {
        Vector2 size = _buildingListSO.GetBuildingSize(buildingId);
        _foundationMove.SetUnitSize(size);
        _foundationMove.MoveTo(foundationCenterTileId);

        if (_isRoad)
        {
            return;
        }

        _interactiveObject.localScale = new Vector3(size.x, _interactiveObject.localScale.y, size.y);
        _bufferVector.x = size.x;
        _bufferVector.y = size.y;
        _foundationRenderer.sharedMaterial.SetVector("_Tiling", _bufferVector);
        SetUIPositionAndEventCam(new Vector3(0f, -size.y / 3f, 0f));
        SetUpBuildingModel(buildingId);
        SetUpRotation(buildingId);
    }

    public void Deactive()
    {
        if (TutorialTracker.NeedTutorial)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    private void SetUpBuildingModel(int buildingId)
    {
        GameObject buildingModel = _buildingAssetsList.GetBuildingModel(buildingId);

        if (buildingModel == null)
        {
            return;
        }

        if (_buildingModelObj != null)
        {
            Destroy(_buildingModelObj);
        }

        _buildingModelObj = Instantiate(buildingModel, _buildingModelHolder);

        _buildingModelObj.transform.localPosition = Vector3.zero;
        Vector2 buildingSize = _buildingListSO.GetBuildingSize(buildingId);
        Vector3 objectSize = _buildingModelObj.transform.localScale;
        _buildingModelObj.transform.localScale = new Vector3(
            buildingSize.x * objectSize.x, 
            buildingSize.x * objectSize.y, 
            buildingSize.y * objectSize.z);
        _buildingModelObj.transform.localRotation = Quaternion.identity;

        _buildingModelObj.SetLayerRecursively((int)Mathf.Log(_buildingModelLayer, 2));

        TweenBuildingModelColor(_buildingModelObj).Forget();
    }
    private async UniTaskVoid TweenBuildingModelColor(GameObject buildingModel)
    {
        if (_isRoad || buildingModel == null)
        {
            return;
        }

        MeshRenderer[] renderers = buildingModel.GetComponentsInChildren<MeshRenderer>();
        List<Color> originColors = new();
        for (int i = 0; i < renderers.Length; i++)
        {
            originColors.Add(renderers[i].material.color);
        }

        Color currrentTintColor = Color.white;
        int j = 0;
        while (_isTweeningModelColor)
        {
            Color desTintColor = j % 2 == 0 ? _modelTintColor : Color.white;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (buildingModel == null)
                {
                    break;
                }

                var mat = renderers[i].material;
                Color originColor = originColors[i];
                DOTween.To(() => currrentTintColor, value => currrentTintColor = value, desTintColor, _tintHalfPeriod)
                    .SetEase(Ease.InOutQuad).OnUpdate(() =>
                    {
                        Color finalColor = originColor * currrentTintColor;
                        if (!_isValidPosition.Value)
                        {
                            finalColor *= _invalidPositionColor;
                        }
                        mat.color = finalColor;
                    });
            }

            j++;
            await UniTask.Delay(System.TimeSpan.FromSeconds(_tintHalfPeriod));
        }
    }

    private void SetUpRotation(int buildingId)
    {
        if (_isRoad)
        {
            return;
        }

        Vector2 foundationSize = _buildingListSO.GetBuildingSize(buildingId);
        bool squaredSize = Mathf.Abs(foundationSize.x - foundationSize.y) < float.Epsilon;

        List<Vector2> potentialDirs = new(4);
        int mapX = _townMap.xSize;
        int mapY = _townMap.ySize;
        List<int> neighbors = TownMapHelper.GetNeighborTilesOfArea(mapX, mapY, _foundTopLeftTile.Value, foundationSize);
        List<int> foundationArea = TownMapHelper.TopLeftGetAllTilesInArea(mapX, mapY, foundationSize, _foundTopLeftTile.Value);
        for (int i = 0; i < neighbors.Count; i++)
        {
            var roadTile = _townMap.GetTileInfo(neighbors[i]);
            if (roadTile.baseBuildingValue != 30101) // Not roads 
            {
                continue;
            }

            Vector2 direction = TownMapHelper.GetDirectionOfAreaAndNeighborTile(_townMap.xSize, _townMap.ySize, foundationArea, roadTile.tileId);
            if (potentialDirs.Contains(direction))
            {
                continue;
            }
            if (!squaredSize && (direction == Vector2.right || direction == Vector2.left))
            {
                continue;
            }

            potentialDirs.Add(direction);
        }

        if (potentialDirs.Count == 0)
        {
            _tilingTransform.RotateTo(_buildingAssetsList.GetDefaultRotation(buildingId));
            return;
        }

        _tilingTransform.RotateToDirection(potentialDirs[0]); // TODO RK Find short direction so road and door is closest.
    }

    private void SetUIPositionAndEventCam(Vector3 position)
    {
        _foundationUI.transform.GetChild(0).localPosition = position;
        _foundationUI.worldCamera = Camera.main;
    }

    private void OnEnable()
    {
        _foundationIsOpening.Value = true;
        _isTweeningModelColor = true;
    }

    private void OnDisable()
    {
        _foundationIsOpening.Value = false;
        _isTweeningModelColor = false;
        Destroy(_buildingModelObj);
    }
}
