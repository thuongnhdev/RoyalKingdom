using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoPopup : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildingInfos;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssets;
    [SerializeField]
    private UserBuildingList _userBuildings;
    [SerializeField]
    private IntegerVariable _selectedBuildingObjId;

    [Header("Config")]
    [SerializeField]
    private Image _buildingImage;
    [SerializeField]
    private TMP_Text _buildingNameText;
    [SerializeField]
    private TMP_Text _buildingDesText;
    [SerializeField]
    private TMP_Text _buildingLevelText;
    [SerializeField]
    private Button _nextButton;
    [SerializeField]
    private Button _prevButton;

    [Header("Inspec")]
    [SerializeField]
    private int _showLevel;

    public void In_SetUpContent()
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        if (userBuilding == null)
        {
            return;
        }

        var building = _buildingInfos.GetBaseBuilding(userBuilding.buildingId);
        if (building == null)
        {
            return;
        }

        _buildingNameText.text = building.name;
        _buildingDesText.text = building.description;
        _buildingImage.sprite = _buildingAssets.GetBuildingSprite(building.id);

        ShowInfoOfLevel(userBuilding.buildingLevel);
    }

    public void In_ShowNextLevelInfo()
    {
        ShowInfoOfLevel(_showLevel + 1);
    }

    public void In_ShowPrevLevelInfo()
    {
        ShowInfoOfLevel(_showLevel - 1);
    }

    private void ShowInfoOfLevel(int level)
    {
        var userBuilding = _userBuildings.GetBuilding(_selectedBuildingObjId.Value);
        var building = _buildingInfos.GetBaseBuilding(userBuilding.buildingId);

        _showLevel = Mathf.Clamp(level, 1, building.maxLevel);

        _nextButton.interactable = _showLevel < building.maxLevel;
        _prevButton.interactable = 1 < _showLevel;

        string buildinglevelText = _showLevel == building.maxLevel ? "Max" : _showLevel.ToString();
        _buildingLevelText.text = $"Level {buildinglevelText}";
    }
}
