using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingPolisher : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private BuildingComponentGetter _comGetter;
    [SerializeField]
    private bool _isBuilding = true;

    [Header("Upgrade")]
    [SerializeField]
    private ParticleSystem _upgradeVfx;

    [Header("Construction")]
    [SerializeField]
    private IntervalAction _intervalShake;
    [SerializeField]
    private GameObject _constructionFences;

    [Header("WaitForCons")]
    [SerializeField]
    private Transform _buildingModelHolder;
    
    private MeshRenderer[] _buildingMeshes;
    private MeshRenderer[] BuildingMeshes
    {
        get
        {
            if (_buildingMeshes == null)
            {
                _buildingMeshes = _buildingModelHolder.GetComponentsInChildren<MeshRenderer>(true);
            }

            return _buildingMeshes;
        }
    }

    public void ActiveConstructionEffect()
    {
        BuildingStatus status = _comGetter.Operation.Status;
        if (status == BuildingStatus.Upgrading)
        {
            _upgradeVfx.Play();
            return;
        }

        if (status == BuildingStatus.OnConstruction)
        {
            _intervalShake.StartIntervalAction();
            return;
        }

        _upgradeVfx.Stop();
        _intervalShake.StopIntervalAction();
    }

    public void ShowConstructingBuildingBasedOnResourceFilled(List<ResourcePoco> current, List<ResourcePoco> cost)
    {
        int costCount = ItemHelper.CountResource(cost);
        if (costCount == 0)
        {
            SetBuildingAppearance(1f);
            return;
        }

        SetBuildingAppearance(ItemHelper.CountResource(current) / (float)costCount);
    }

    public void SetBuildingAppearance(float percent)
    {
        Color bufferColor;
        for (int i = 0; i < BuildingMeshes.Length; i++)
        {
            MeshRenderer renderer = BuildingMeshes[i];
            var mat = renderer.material;

            if (1f <= percent)
            {
                mat.ToOpaqueAlphaClip(0.5f);
                bufferColor = mat.GetColor("_BaseColor");
                bufferColor.a = 1f;
                renderer.material.SetColor("_BaseColor", bufferColor);
                continue;
            }

            mat.ToTransparent();
            bufferColor = mat.GetColor("_BaseColor");
            bufferColor.a = MathUtils.Remap(0f, 1f, 0.2f, 1f, percent);
            renderer.material.SetColor("_BaseColor", bufferColor);
        }
    }

    public void SetUpAppearanceFollowingStatus()
    {
        BuildingStatus status = _comGetter.Operation.Status;
        if (status == BuildingStatus.OnConstruction || status == BuildingStatus.Upgrading)
        {
            _constructionFences.SetActive(true);
            return;
        }

        if (!_constructionFences.activeSelf)
        {
            return;
        }

        _constructionFences.transform.GetChild(1).GetComponent<DoTweenAnimation>().DoTween();
    }
}
