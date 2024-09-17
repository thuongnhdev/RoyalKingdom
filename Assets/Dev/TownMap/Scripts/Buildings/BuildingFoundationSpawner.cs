using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFoundationSpawner : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private IntegerVariable _selectedBuildingId;
    [SerializeField]
    private IntegerVariable _foundationCenterTileId;

    [Header("Config")]
    [SerializeField]
    private GameObject _foundationObj;
    [SerializeField]
    private GameObject _roadFoundationObj;
    [SerializeField]
    private Transform _buildingHolder;
    
    public void In_SpawnBuildingFoundation()
    {
        UniTask_SpawnBuildingFoundation().Forget();
    }

    private async UniTaskVoid UniTask_SpawnBuildingFoundation()
    {
        await UniTask.DelayFrame(1); // 1 frame delay for data is set.

        _foundationObj.SetActive(true);
        _roadFoundationObj.SetActive(false);

        BuildingFoundationSetter foundationSetter = _foundationObj.GetComponent<BuildingFoundationSetter>();
        if (_selectedBuildingId.Value == 30101) // road id TODO refactor this when more road types are defined;
        {
            _foundationObj.SetActive(false);
            _roadFoundationObj.SetActive(true);
            foundationSetter = _roadFoundationObj.GetComponent<BuildingFoundationSetter>();
        }

        foundationSetter.SetUp(_selectedBuildingId.Value, _foundationCenterTileId.Value);

    }
}
