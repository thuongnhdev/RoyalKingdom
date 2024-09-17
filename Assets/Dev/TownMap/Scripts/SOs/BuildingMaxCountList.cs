using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingMaxCountList", menuName = "Uniflow/Building/BuildingMaxCountList")]
public class BuildingMaxCountList : ScriptableObject
{
    [SerializeField]
    private List<BuildingMaxCountInfo> _buildingMaxCountInfos;
    private Dictionary<int, List<BuildingMaxCountInfo>> _buildingMaxCountDict = new();
    private Dictionary<int, List<BuildingMaxCountInfo>> BuildingMaxCountDict
    {
        get
        {
            if (_buildingMaxCountInfos.Count != _buildingMaxCountDict.Count)
            {
                _buildingMaxCountDict.Clear();
                for (int i = 0; i < _buildingMaxCountInfos.Count; i++)
                {
                    var building = _buildingMaxCountInfos[i];
                    _buildingMaxCountDict.TryGetValue(building.buildingId, out var dictValue);
                    if (dictValue == null)
                    {
                        dictValue = new();
                        _buildingMaxCountDict.Add(building.buildingId, dictValue);
                    }
                    dictValue.Add(building);
                }
            }

            return _buildingMaxCountDict;
        }
    }
    public void Init(ApiGetLowData serverData)
    {
        int infoCount = serverData.DataBuildingMaxCountLength;
        _buildingMaxCountInfos.Clear();
        _buildingMaxCountDict.Clear();

        for (int i = 0; i < infoCount; i++)
        {
            var maxCountData = serverData.DataBuildingMaxCount(i).Value;
            List<BuildingCondition> conditions = new();
            maxCountData = ExtractConditions(maxCountData, conditions);
            _buildingMaxCountInfos.Add(new()
            {
                buildingId = maxCountData.BuildingKey,
                buildingName = maxCountData.Name,
                countLevel = maxCountData.CountLevel,
                maxCount = maxCountData.MaxCount,
                conditions = conditions
            });
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public List<BuildingMaxCountInfo> GetMaxCountInfo(int buildingId)
    {
        BuildingMaxCountDict.TryGetValue(buildingId, out var building);
        return building;
    }

    private BuildingMaxCount ExtractConditions(BuildingMaxCount maxCountData, List<BuildingCondition> conditions)
    {
        if (maxCountData.ReqContent1 != "" && maxCountData.ReqCondition1 != "")
        {
            BuildingCondition condition1 = new();
            Enum.TryParse($"{maxCountData.ReqContent1}_{maxCountData.ReqCondition1}", out BuildingConditionType conditionType1);
            condition1.conditionType = conditionType1;
            condition1.conditionValues.Add(maxCountData.ReqKey1);
            condition1.conditionValues.Add(maxCountData.ReqValue1);

            conditions.Add(condition1);
        }
        if (maxCountData.ReqContent2 != "" && maxCountData.ReqCondition2 != "")
        {
            BuildingCondition condition2 = new();
            Enum.TryParse($"{maxCountData.ReqContent2}_{maxCountData.ReqCondition2}", out BuildingConditionType conditionType2);
            condition2.conditionType = conditionType2;
            condition2.conditionValues.Add(maxCountData.ReqKey2);
            condition2.conditionValues.Add(maxCountData.ReqValue2);

            conditions.Add(condition2);
        }

        return maxCountData;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}

[System.Serializable]
public class BuildingMaxCountInfo
{
    public string buildingName;
    public int buildingId;
    public int countLevel;
    public int maxCount;
    public List<BuildingCondition> conditions; 
}
