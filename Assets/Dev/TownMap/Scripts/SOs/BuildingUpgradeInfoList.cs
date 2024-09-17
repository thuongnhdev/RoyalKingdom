using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Fbs;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BuildingUpgradeInfoList", menuName = "Uniflow/Building/Building Upgrade Info List")]
public class BuildingUpgradeInfoList : ScriptableObject
#if UNITY_EDITOR
    , IExcel2SO
#endif
{
    [SerializeField]
    private List<BuildingUpgradeInfo> _upgradeInfoList;
    private Dictionary<string, BuildingUpgradeInfo> _upgradeInfoDict = new();
    private Dictionary<string, BuildingUpgradeInfo> UpgradeInfoDict
    {
        get
        {
            if (_upgradeInfoDict.Count != _upgradeInfoList.Count)
            {
                _upgradeInfoDict.Clear();

                for (int i = 0; i < _upgradeInfoList.Count; i++)
                {
                    _sb.Clear();
                    var condition = _upgradeInfoList[i];
                    string key = _sb.Append(condition.buildingId).Append(condition.level).ToString();

                    _upgradeInfoDict[key] = condition;
                }
            }

            return _upgradeInfoDict;
        }
    }

    private readonly StringBuilder _sb = new();

    public void Init(ApiGetLowData data)
    {
        int buildingCount = data.DataBuildingLevelLength;
        if (buildingCount == 0)
        {
            return;
        }

        _upgradeInfoDict.Clear();
        _upgradeInfoList.Clear();
        for (int i = 0; i < buildingCount; i++)
        {
            var upgradeData = data.DataBuildingLevel(i).Value;
            List<ResourcePoco> cost = new();
            cost.Add(new() { itemId = upgradeData.ReqItemKey1, itemCount = upgradeData.ReqItemvalue1 });
            cost.Add(new() { itemId = upgradeData.ReqItemKey2, itemCount = upgradeData.ReqItemvalue2 });
            cost.Add(new() { itemId = upgradeData.ReqItemKey3, itemCount = upgradeData.ReqItemvalue3 });
            cost.Add(new() { itemId = upgradeData.ReqItemKey4, itemCount = upgradeData.ReqItemvalue4 });
            cost.Add(new() { itemId = upgradeData.ReqItemKey5, itemCount = upgradeData.ReqItemvalue5 });

            List<BuildingCondition> conditions = new();

            string conditionStr1 = $"{upgradeData.ReqContent1}_{upgradeData.ReqCondition1}";
            Enum.TryParse(conditionStr1, out BuildingConditionType condition1);
            List<int> conditionValues1 = new();
            conditionValues1.Add(upgradeData.ReqKey1);
            conditionValues1.Add(upgradeData.ReqValue1);
            if (condition1 != BuildingConditionType.None)
            {
                conditions.Add(new() { conditionType = condition1, conditionValues = conditionValues1 });
            }

            string conditionStr2 = $"{upgradeData.ReqContent2}_{upgradeData.ReqCondition2}";
            Enum.TryParse(conditionStr2, out BuildingConditionType condition2);
            List<int> conditionValues2 = new();
            conditionValues2.Add(upgradeData.ReqKey2);
            conditionValues2.Add(upgradeData.ReqValue2);
            if (condition2 != BuildingConditionType.None)
            {
                conditions.Add(new() { conditionType = condition2, conditionValues = conditionValues2 });
            }

            var updradeInfo = new BuildingUpgradeInfo
            {
                buildingId = upgradeData.IdBuildingTemplate,
                level = upgradeData.Level,
                name = upgradeData.Name,
                timeCost = upgradeData.ReqWorkload,
                cost = ItemHelper.TrimResources(cost),
                conditions = conditions,
                capacity = upgradeData.MaxCapacity,
                buff1 = upgradeData.BuffID1,
                buffValue1 = upgradeData.BuffValue1,
                buff2 = upgradeData.BuffID2,
                buffValue2 = upgradeData.BuffValue2,
                buff3 = upgradeData.BuffID3,
                buffValue3 = upgradeData.BuffValue3,
            };

            _upgradeInfoList.Add(updradeInfo);
        }
    }

    public BuildingUpgradeInfo GetBuildingUpgradeInfo(int buildingId, int level, bool friendlyWarning = false)
    {
        _sb.Clear();
        string key = _sb.Append(buildingId).Append(level).ToString();
        UpgradeInfoDict.TryGetValue(key, out var upgradeInfo);

        if (upgradeInfo == null)
        {
            if (friendlyWarning)
            {
                Logger.Log($"Safe log: Invalid buildingId [{buildingId}] or buildingLevel [{level}]", Color.green);
            }
            else
            {
                Logger.LogError($"Invalid buildingId [{buildingId}] or buildingLevel [{level}]");
            }
        }

        return upgradeInfo;
    }

    public List<BuildingCondition> GetBuildingUpgradeConditions(int buildingId, int level)
    {
        var upgradeInfo = GetBuildingUpgradeInfo(buildingId, level);
        if (upgradeInfo == null)
        {
            return null;
        }

        return upgradeInfo.conditions;
    }

    public List<ResourcePoco> GetUpgradeCost(int buildingId, int level)
    {
        var upgradeInfo = GetBuildingUpgradeInfo(buildingId, level);
        if (upgradeInfo == null)
        {
            return new();
        }

        return new (upgradeInfo.cost);
    } 

    public float GetTimeCost(int buildingId, int level)
    {
        var upgradeInfo = GetBuildingUpgradeInfo(buildingId, level);
        if (upgradeInfo == null)
        {
            return 0f;
        }

        return upgradeInfo.timeCost;
    }

    public int GetCapacity(int buildingId, int level)
    {
        var building = GetBuildingUpgradeInfo(buildingId, level, true);
        if (building == null)
        {
            return 0;
        }

        return building.capacity;
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

#if UNITY_EDITOR
    public void FromExcelToSO(List<List<ExcelGenericRow>> parsedSheets)
    {
        var sheet = parsedSheets[1];

        if (sheet.Count == 0)
        {
            return;
        }

        _upgradeInfoList.Clear();
        for (int i = 0; i < sheet.Count; i++)
        {
            var upgradeInfo = new BuildingUpgradeInfo();
            var row = sheet[i];

            ParseBasicInfo(row, upgradeInfo);
            ParseConditions(row, upgradeInfo);
            ParseCostInfo(row, upgradeInfo);

            _upgradeInfoList.Add(upgradeInfo);
        }

        EditorUtility.SetDirty(this);
    }

    private void ParseBasicInfo(ExcelGenericRow row, BuildingUpgradeInfo upgradeInfo)
    {
        row.GetValue("BuildingKey", out int buildingId);
        row.GetValue("Level", out int level);
        row.GetValue("Name", out string name);
        upgradeInfo.buildingId = buildingId;
        upgradeInfo.level = level;
        upgradeInfo.name = name;
    }

    private void ParseConditions(ExcelGenericRow row, BuildingUpgradeInfo upgradeInfo)
    {
        row.GetValue("Req_Content_1", out string req1_1);
        row.GetValue("Req_Condition_1", out string req1_2);
        Enum.TryParse($"{req1_1}_{req1_2}", out BuildingConditionType conditionType1);
        if (conditionType1 != BuildingConditionType.None)
        {
            BuildingCondition condition1 = new();
            condition1.conditionType = conditionType1;
            row.GetValue("Req_Key_1", out int condition1Key);
            row.GetValue("Req_Value_1", out int condition1Value);
            condition1.conditionValues.Add(condition1Key);
            condition1.conditionValues.Add(condition1Value);

            upgradeInfo.conditions.Add(condition1);
        }

        row.GetValue("Req_Content_2", out string req2_1);
        row.GetValue("Req_Condition_2", out string req2_2);
        Enum.TryParse($"{req2_1}_{req2_2}", out BuildingConditionType conditionType2);
        if (conditionType2 != BuildingConditionType.None)
        {
            BuildingCondition condition2 = new();
            condition2.conditionType = conditionType2;
            row.GetValue("Req_Key_2", out int condition2Key);
            row.GetValue("Req_Value_2", out int condition2Value);
            condition2.conditionValues.Add(condition2Key);
            condition2.conditionValues.Add(condition2Value);

            upgradeInfo.conditions.Add(condition2);
        }
    }

    private void ParseCostInfo(ExcelGenericRow row, BuildingUpgradeInfo upgradeInfo)
    {
        row.GetValue("Req_Time", out int timeCost);
        upgradeInfo.timeCost = timeCost;

        string itemKeyFormat = "Req_Item_Key_{0}";
        string itemValueFormat = "Req_Itemvalue_{0}";
        for (int reqItemColumn = 1; reqItemColumn <= 5; reqItemColumn++) // There are 5 item columns in excel sheet
        {
            row.GetValue(string.Format(itemKeyFormat, reqItemColumn), out int itemId);
            row.GetValue(string.Format(itemValueFormat, reqItemColumn), out int itemCount);

            if (itemId == 0 || itemCount == 0)
            {
                continue;
            }

            var cost = new ResourcePoco
            {
                itemId = itemId,
                itemCount = itemCount
            };

            upgradeInfo.cost.Add(cost);
        }
    }
#endif
}

[System.Serializable]
public class BuildingUpgradeInfo
{
    public string name;
    public int buildingId;
    public int level;
    public float timeCost;
    public List<ResourcePoco> cost = new();
    public List<BuildingCondition> conditions = new();
    public int capacity;
    public string buff1;
    public int buffValue1;
    public string buff2;
    public int buffValue2;
    public string buff3;
    public int buffValue3;
}
