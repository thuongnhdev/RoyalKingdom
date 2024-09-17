using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionConditionIconAndLabel : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private TownBaseBuildingSOList _buildings;
    [SerializeField]
    private TownBuildingAssetsInfoList _buildingAssets;

    [Header("Config")]
    [SerializeField]
    private Image _conditionIcon;
    [SerializeField]
    private TMP_Text _conditionLabel;

    public void SetUp(BuildingCondition condition)
    {
        GetConditionSpriteAndLabel(condition.conditionType, condition.conditionValues, out Sprite conditionSprite, out string conditionLabel);
        _conditionIcon.sprite = conditionSprite;
        _conditionLabel.text = conditionLabel;
    }

    private void GetConditionSpriteAndLabel(BuildingConditionType conditionType, List<int> conditionValue, 
                                            out Sprite conditionSprite, out string conditionLabel)
    {
        switch (conditionType)
        {
            case BuildingConditionType.BUILDING_LEVEL:
                {
                    int buildingId = conditionValue[0];
                    conditionSprite = _buildingAssets.GetBuildingSprite(buildingId);
                    int buildingLevel = conditionValue[1];

                    conditionLabel = $"Lv{buildingLevel} {_buildings.GetBuildingName(buildingId)}";
                    break;
                }
            // TODO Add Trade sprite when defined
            case BuildingConditionType.TRADE_VALUE:
                {
                    conditionSprite = null;
                    conditionLabel = "";
                    break;
                }
            default:
                {
                    conditionSprite = null;
                    conditionLabel = "";
                    break;
                }
        }
    }
}
