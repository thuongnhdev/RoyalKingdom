using CoreData.UniFlow.Common;
using Cysharp.Threading.Tasks;
using Fbs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Localization.Tables;

public class MasterDataParser : MonoBehaviour
{
    [Header("Common")]
    [SerializeField]
    private LocalizationStringList _localizationString;

    [Header("Town Buildings")]
    [SerializeField]
    private TownBaseBuildingSOList _townBuildingList;
    [SerializeField]
    private BuildingUpgradeInfoList _buildingUpgradeInfos;
    [SerializeField]
    private BuildingMaxCountList _buildingMaxCountInfos;

    [Header("Vassal")]
    [SerializeField]
    private VassalSOList _vassals;
    [SerializeField]
    private VassalStatsSOList _vassalStats;
    [SerializeField]
    private VassalExpAndLevel _vassalExpAndLevel;

    [Header("Items/Productions")]
    [SerializeField]
    private ItemList _items;
    [SerializeField]
    private ItemGroupList _itemGroups;
    [SerializeField]
    private ItemTypeList _itemTypes;
    [SerializeField]
    private BuildingProductDescriptionList _productionDesList;
    [SerializeField]
    private ProductFormulaList _productFormulas;
    [SerializeField]
    private PrefixList _prefixes;

    [Header("World")]
    [SerializeField]
    private LandStaticInfoList _landStaticInfos;
    [SerializeField]
    private CityList _cities;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onMasterDataParsed;


    public void ParseMasterDataFromServer(ApiGetLowData fbData)
    {
        ParseLocalizationString(fbData);

        ParseBuildingMasterData(fbData);
        ParseBuildingUpgradeInfo(fbData);
        ParseBuildingMaxCountInfo(fbData);

        ParseLands(fbData);

        ParseDataEnum(fbData);

        ParseConstMasterData(fbData);

        ParseAgeMortalityMasterData(fbData);

        ParseMoodMasterData(fbData);

        ParsePopulationMasterData(fbData);

        ParsePopulationSalaryMasterData(fbData);

        ParseSeasonTimeMasterData(fbData);

        ParseSeasonDateMasterData(fbData);

        ParseDataVassalGrade(fbData);

        ParseDataVassalJobClass(fbData);

        ParseDataVassalJobGrade(fbData);

        ParseDataVassalNature(fbData);

        ParseDataVassaleStats(fbData);

        ParseDataVassalStatsLevel(fbData);

        ParseVassalStatus(fbData);

        ParseVassalTemplate(fbData);

        ParseDataPriorityTemplate(fbData);

        ParseItemMasterData(fbData);

        ParseItemGroupMasterData(fbData);

        ParseItemTypes(fbData);

        ParsePrefixMasterData(fbData);

        ParseVassalJobInfo(fbData);

        ParseVassalJobTreeDefaut(fbData);

        ParseVassalTaskInfo(fbData);

        ParsePopulationSlaryStepData(fbData);

        ParseMilitaryTemplate(fbData);

        ParseCities(fbData);

        ParseMilitaryProficiency(fbData);

        ParseMilitaryFomation(fbData);

        ParseMilitaryBranch(fbData);

        ParseMilitaryInfo(fbData);

        ParseSymbolBg(fbData);

        ParseSymbolLayout(fbData);

        ParseSymbolIconSub(fbData);

        ParseSymbolIconMain(fbData);

        ParseSymbolColor(fbData);

        ParseSymbolDisplay(fbData);

        ParsePopulationMorale(fbData);

        ParseBandDefaut(fbData);

        ParseVassalTaskInfoSub(fbData);
    }

    private void ParseLocalizationString(ApiGetLowData fbData)
    {
        _localizationString.Init(fbData);
    }

    private void ParseBuildingMasterData(ApiGetLowData fbData)
    {
        int buildingCount = fbData.BuildingTemplateLength;
        List<BaseBuildingPoco> convertedBuildings = new();
        for (int i = 0; i < buildingCount; i++)
        {
            var fbBuildingData = fbData.BuildingTemplate(i).Value;
            int.TryParse(fbBuildingData.BuildingSize, out int storedSize);
            Vector2 size = new();
            size.x = storedSize / 10;
            size.y = storedSize % 10;
            List<Vector2> doors = ExtractDoorData(fbBuildingData);

            var convertedBuilding = new BaseBuildingPoco()
            {
                id = fbBuildingData.BuildingTemplateId,
                name = fbBuildingData.NameBuilding,
                description = fbBuildingData.DescriptBuilding,
                //category = (BuildingCategory)fbBuildingData.TypeBuilding,
                size = size,
                doorsDirection = doors,
                maxLevel = fbBuildingData.MaxLevel,
                destructionTimeFactor = fbBuildingData.DesTructionTimeFactor,
                unbreakable = fbBuildingData.Unbreakable == 1,
                command1 = (BuildingCommandKey)fbBuildingData.CommandKey1,
                command2 = (BuildingCommandKey)fbBuildingData.CommandKey2
            };

            convertedBuildings.Add(convertedBuilding);
        }

        _townBuildingList.Init(convertedBuildings);
    }

    private void ParseBuildingUpgradeInfo(ApiGetLowData fbData)
    {
        _buildingUpgradeInfos.Init(fbData);
    }

    private void ParseBuildingMaxCountInfo(ApiGetLowData fbData)
    {
        _buildingMaxCountInfos.Init(fbData);
    }

    private List<Vector2> ExtractDoorData(BuildingTemplate buildingData)
    {
        List<Vector2> doors = new();
        int doorCount = buildingData.DoorLength;

        Vector2 door = new();
        for (int i = 0; i < doorCount; i++)
        {
            if (i % 2 == 0)
            {
                door.x = buildingData.Door(i);
                continue;
            }

            door.y = buildingData.Door(i);
            doors.Add(door);
        }

        return doors;
    }

    private void ParseConstMasterData(ApiGetLowData fbData)
    {
        int constCount = fbData.ConstDataTemplateLength;
        List<BaseConst> convertedConsts = new();
        for (int i = 0; i < constCount; i++)
        {
            var fbConstData = fbData.ConstDataTemplate(i).Value;
            var convertedConst = new BaseConst()
            {
                NameConst = fbConstData.NameConst,
                Parameter = fbConstData.Paramater,
                Value = fbConstData.Value,
                Description = fbConstData.Description
            };
            convertedConsts.Add(convertedConst);
        }

        MasterDataStore.Instance.BaseConsts = convertedConsts;
    }

    private void ParseDataEnum(ApiGetLowData fbData)
    {
        int enumCount = fbData.DataEnumLength;
        List<BaseDataEnum> convertedEnums = new();
        for (int i = 0; i < enumCount; i++)
        {
            var fbEnumData = fbData.DataEnum(i).Value;
            var convertedEnumt = new BaseDataEnum()
            {
                Id = fbEnumData.Id,
                Enum = fbEnumData.Enum,
                Memo = fbEnumData.Memo
            };
            convertedEnums.Add(convertedEnumt);
        }

        MasterDataStore.Instance.BaseDataEnums = convertedEnums;
    }

    private void ParseAgeMortalityMasterData(ApiGetLowData fbData)
    {
        int ageCount = fbData.DataAgeMortalityLength;
        List<BaseDataAgeMortality> convertedAges = new();
        for (int i = 0; i < ageCount; i++)
        {
            var fbAgeData = fbData.DataAgeMortality(i).Value;

            var convertedAge = new BaseDataAgeMortality()
            {
                Age = fbAgeData.Age,
                BaseDeathProb = fbAgeData.BaseDeathProb,
                FertiltyProb = fbAgeData.FertiltyProb,
                EmigrationProb = fbAgeData.EmigrationProb
            };

            convertedAges.Add(convertedAge);
        }

        MasterDataStore.Instance.BaseDataAgeMortalitys = convertedAges;
    }

    private void ParseMoodMasterData(ApiGetLowData fbData)
    {
        int moodCount = fbData.DataMoodTemplateLength;
        List<BaseDataMoodTemplate> convertedMoods = new();
        for (int i = 0; i < moodCount; i++)
        {
            var fbMoodData = fbData.DataMoodTemplate(i).Value;

            var convertedMood = new BaseDataMoodTemplate()
            {
                MoodKey = fbMoodData.MoodKey,
                StepName = fbMoodData.StepName,
                MinValue = fbMoodData.MinValue,
                MaxValue = fbMoodData.MaxValue,
                RequirementSalaryRate = fbMoodData.RequirementSalaryRate,
                IncreasePerSalary = fbMoodData.IncreasePerSalary,
                BuffID_1 = fbMoodData.BuffID1,
                BuffValue_IncreasePer_1 = fbMoodData.BuffValueIncreasePer1,
                BuffID_2 = fbMoodData.BuffID2,
                BuffValue_IncreasePer_2 = fbMoodData.BuffValueIncreasePer2,
                BuffID_3 = fbMoodData.BuffID3,
                BuffValue_IncreasePer_3 = fbMoodData.BuffValueIncreasePer3
            };

            convertedMoods.Add(convertedMood);
        }

        MasterDataStore.Instance.BaseDataMoodTemplates = convertedMoods;
    }

    private void ParsePopulationMasterData(ApiGetLowData fbData)
    {
        int populationCount = fbData.DataPopulationTemplateLength;
        List<BaseDataPopulationTemplate> convertedPopulations = new();
        for (int i = 0; i < populationCount; i++)
        {
            var fbPopulationData = fbData.DataPopulationTemplate(i).Value;

            var convertedPopulation = new BaseDataPopulationTemplate()
            {
                CitizenJob = fbPopulationData.CitizenJobId,
                Name = fbPopulationData.Name,
                TotalRate = fbPopulationData.TotalRate,
                MinAge = fbPopulationData.MinAge,
                MaxAge = fbPopulationData.MaxAge,
                Salary_1 = fbPopulationData.Salary1,
                Salary_2 = fbPopulationData.Salary2,
                DeathConstProb = fbPopulationData.DeathConstProb,
                EmigrationRate = fbPopulationData.EmigrationRate,
                Health = fbPopulationData.Health,
                Attack = fbPopulationData.Attack,
                Defense = fbPopulationData.Defense,
                MoveSpeed = fbPopulationData.MoveSpeed,
                Range = fbPopulationData.Range,
                OnlyMale = fbPopulationData.OnlyMale,
                TimeTaskRate = fbPopulationData.TimeTaskRate
            };

            convertedPopulations.Add(convertedPopulation);
        }

        MasterDataStore.Instance.BaseDataPopulationTemplates = convertedPopulations;
    }

    private void ParsePopulationSalaryMasterData(ApiGetLowData fbData)
    {
        int populationSalaryCount = fbData.DataPopulationSalaryLength;
        List<BaseDataPopulationSalary> convertedPopulationSalarys = new();
        for (int i = 0; i < populationSalaryCount; i++)
        {
            var fbSalaryData = fbData.DataPopulationSalary(i).Value;

            var convertedPopulationSalary = new BaseDataPopulationSalary()
            {
                SalaryKey = fbSalaryData.SalaryKey,
                Req_ItemType = fbSalaryData.ReqItemType,
                DefaultValue = fbSalaryData.DefautValue,
                CriteriaStep = fbSalaryData.CriteriaStep,
                DemandPerWorker = fbSalaryData.DemandPerWorker
                
            };

            convertedPopulationSalarys.Add(convertedPopulationSalary);
        }

        MasterDataStore.Instance.BaseDataPopulationSalarys = convertedPopulationSalarys;
    }

    private void ParsePopulationSlaryStepData(ApiGetLowData fbData)
    {
        int populationSalaryStepCount = fbData.DataPopulationSalaryStepLength;
        List<BaseDataPopulationSalaryStep> convertedPopulationSalarys = new();
        for (int i = 0; i < populationSalaryStepCount; i++)
        {
            var fbSalaryData = fbData.DataPopulationSalaryStep(i).Value;

            var convertedPopulationSalary = new BaseDataPopulationSalaryStep()
            {
                SalaryStepKey = fbSalaryData.SalaryStepKey,
                ChangeSalaryRate = fbSalaryData.ChangeSalaryRate,
                ChangeMood = fbSalaryData.ChangeMood,
                ChangeMorale = fbSalaryData.ChangeMorale

            };

            convertedPopulationSalarys.Add(convertedPopulationSalary);
        }

        MasterDataStore.Instance.BaseDataPopulationSalarySteps = convertedPopulationSalarys;
    }

    private void ParsePopulationMorale(ApiGetLowData fbData)
    {
        int count = fbData.DataPopulationMoraleLength;
        List<BasePopulationMorale> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataPopulationMorale(i).Value;

            var converted = new BasePopulationMorale()
            {
                MoraleKey = data.MoraleKey,
                StepName = data.StepName,
                MinValue = data.MinValue,
                MaxValue = data.MaxValue,
                RequirementSalaryRate = data.RequirementSalaryRate,
                IncreasePerSalary = data.IncreasePerSalary,
                BuffID_1 = data.BuffID1,
                BuffValue_IncreasePer_1 = data.BuffValueIncreasePer1,
                BuffID_2 = data.BuffID2,
                BuffValue_IncreasePer_2 = data.BuffValueIncreasePer2,
                BuffID_3 = data.BuffID3,
                BuffValue_IncreasePer_3 = data.BuffValueIncreasePer3
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BasePopulationMorales = converteds;
    }

    private void ParseSeasonTimeMasterData(ApiGetLowData fbData)
    {
        int seasonTimeCount = fbData.DataSeasonTimeLength;
        List<BaseDataSeasonTime> convertedSeasonTimes = new();
        for (int i = 0; i < seasonTimeCount; i++)
        {
            var fbSeasonTimeData = fbData.DataSeasonTime(i).Value;

            var convertedSeasonTime = new BaseDataSeasonTime()
            {
                SeasonType = fbSeasonTimeData.SeasonType,
                SeasonPeriod_RealTime = fbSeasonTimeData.SeasonPeriodRealTime,
                SeasonRestPeriod_RealTime = fbSeasonTimeData.SeasonRestPeriodRealTime,
                SeasonPeriod_GameTime = fbSeasonTimeData.SeasonPeriodGameTime
            };

            convertedSeasonTimes.Add(convertedSeasonTime);
        }

        MasterDataStore.Instance.BaseDataSeasonTimes = convertedSeasonTimes;
    }

    private void ParseSeasonDateMasterData(ApiGetLowData fbData)
    {
        int seasonDateCount = fbData.DataSeasonDateLength;
        List<BaseDataSeasonDate> convertedSeasonDates = new();
        for (int i = 0; i < seasonDateCount; i++)
        {
            var fbSeasonDateData = fbData.DataSeasonDate(i).Value;

            var convertedSeasonDate = new BaseDataSeasonDate()
            {
                Month = fbSeasonDateData.Month,
                MonthDay = fbSeasonDateData.MonthDay,
                MonthName = fbSeasonDateData.MonthName
            };

            convertedSeasonDates.Add(convertedSeasonDate);
        }

        MasterDataStore.Instance.BaseDataSeasonDates = convertedSeasonDates;
    }

    private void ParseDataVassalGrade(ApiGetLowData fbData)
    {
        int vassalGradeCount = fbData.DataVassalGradeLength;
        List<BaseDataVassalGrade> convertedVassalGrades = new();
        for (int i = 0; i < vassalGradeCount; i++)
        {
            var fbVassalGrade = fbData.DataVassalGrade(i).Value;

            var convertedVassalGrade = new BaseDataVassalGrade()
            {
                Key = fbVassalGrade.Key,
                GradeName = fbVassalGrade.GradeName
            };

            convertedVassalGrades.Add(convertedVassalGrade);
        }

        MasterDataStore.Instance.BaseDataVassalGrades = convertedVassalGrades;
    }

    private void ParseDataVassalJobClass(ApiGetLowData fbData)
    {
        int count = fbData.DataVassalJobClassLength;
        List<BaseDataVassalJobClass> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataVassalJobClass(i).Value;

            var converted = new BaseDataVassalJobClass()
            {
                Key = data.Key,
                JobName = data.JobName,
                JobClassName = data.JobClassName
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseDataVassalJobClasss = converteds;
    }

    private void ParseDataVassalJobGrade(ApiGetLowData fbData)
    {
        int count = fbData.DataVassalJobGradeLength;
        List<BaseDataVassalJobGrade> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataVassalJobGrade(i).Value;

            var converted = new BaseDataVassalJobGrade()
            {
                Key = data.Key,
                GradeName = data.GradeName,
                Consume = data.Consume,
                Salary = data.Salary
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseDataVassalJobGrades = converteds;
    }

    private void ParseDataVassalNature(ApiGetLowData fbData)
    {
        int count = fbData.DataVassalNatureLength;
        List<BaseDataVassalNature> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataVassalNature(i).Value;

            var converted = new BaseDataVassalNature()
            {
                Key = data.Key,
                Nature = data.Nature
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseDataVassalNatures = converteds;
    }

    private void ParseDataVassaleStats(ApiGetLowData fbData)
    {
        int count = fbData.DataVassaleStatsLength;
        List<BaseDataVassalStat> converteds = new();
        List<VassalStat> convertedVassalStats = new();

        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataVassaleStats(i).Value;

            var converted = new BaseDataVassalStat()
            {
                Key = data.Key,
                StatName = data.StatName
            };

            convertedVassalStats.Add(new(data));

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseDataVassalStats = converteds;
        _vassalStats.Init(convertedVassalStats);
    }

    private void ParseDataVassalStatsLevel(ApiGetLowData fbData)
    {
        int count = fbData.DataVassalStatsLevelLength;
        List<BaseDataVassalStatsExp> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataVassalStatsLevel(i).Value;

            var converted = new BaseDataVassalStatsExp()
            {
                Lv = data.Lv,
                Max_exp = data.MaxExp
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseDataVassalStatsExps = converteds;
        MasterDataStore.Instance.SetBaseDataVassalStatsExpsIngame();
        _vassalExpAndLevel.Init(fbData);
    }

    private void ParseVassalStatus(ApiGetLowData fbData)
    {
        int count = fbData.VassalStatusLength;
        List<BaseVassalStatus> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.VassalStatus(i).Value;

            var converted = new BaseVassalStatus()
            {
                Key = data.Key,
                Status_Name = data.StatusName,
                Workable = data.Workable
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseVassalStatus = converteds;
    }

    private void ParseVassalTaskInfo(ApiGetLowData fbData)
    {
        int count = fbData.VassalTaskInfoLength;
        List<BaseVassalTaskInfo> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.VassalTaskInfo(i).Value;

            var converted = new BaseVassalTaskInfo()
            {
                Key = data.Key,
                TaskName = data.TaskName,
                StatKey_1 = data.StatKey1,
                EXP_1 = data.EXP1,
                StatKey_2 = data.StatKey2,
                EXP_2 = data.EXP2,
                StatKey_3 = data.StatKey3,
                EXP_3 = data.EXP3,
                StatKey_4 = data.StatKey4,
                EXP_4 = data.EXP4,
                Job_Class_Key = data.JobClassKey,
                Job_Class_EXP = data.JobClassEXP,
                BuffID_1 = data.BuffID1,
                BuffStatKey_1 = data.BuffStatKey1,
                BuffValue_1 = data.BuffValue1,
                BuffID_2 = data.BuffID2,
                BuffStatKey_2 = data.BuffStatKey2,
                BuffValue_2 = data.BuffValue2,
                BuffID_3 = data.BuffID3,
                BuffStatKey_3 = data.BuffStatKey3,
                BuffValue_3 = data.BuffValue3,
                BuffID_4 = data.BuffID4,
                BuffStatKey_4 = data.BuffStatKey4,
                BuffValue_4 = data.BuffValue4
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseVassalTaskInfos = converteds;
    }

    private void ParseVassalTaskInfoSub(ApiGetLowData fbData)
    {
        int count = fbData.DataVassalTaskSubLength;
        List<BaseVassalTaskInfo> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataVassalTaskSub(i).Value;

            var converted = new BaseVassalTaskInfo()
            {
                Key = data.Key,
                TaskName = data.TaskName,
                StatKey_1 = data.StatKey1,
                EXP_1 = data.EXP1,
                StatKey_2 = data.StatKey2,
                EXP_2 = data.EXP2,
                StatKey_3 = data.StatKey3,
                EXP_3 = data.EXP3,
                StatKey_4 = data.StatKey4,
                EXP_4 = data.EXP4,
                Job_Class_Key = data.JobClassKey,
                Job_Class_EXP = data.JobClassEXP,
                BuffID_1 = data.BuffID1,
                BuffStatKey_1 = data.BuffStatKey1,
                BuffValue_1 = data.BuffValue1,
                BuffID_2 = data.BuffID2,
                BuffStatKey_2 = data.BuffStatKey2,
                BuffValue_2 = data.BuffValue2,
                BuffID_3 = data.BuffID3,
                BuffStatKey_3 = data.BuffStatKey3,
                BuffValue_3 = data.BuffValue3,
                BuffID_4 = data.BuffID4,
                BuffStatKey_4 = data.BuffStatKey4,
                BuffValue_4 = data.BuffValue4
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseVassalTaskInfoSubs = converteds;
    }

    private void ParseVassalTemplate(ApiGetLowData fbData)
    {
        int count = fbData.VassalTemplateLength;
        List<BaseVassalTemplate> converteds = new();
        List<VassalInSOList> vassalsConvertedSOData = new();

        for (int i = 0; i < count; i++)
        {
            var data = fbData.VassalTemplate(i).Value;

            var converted = new BaseVassalTemplate();
            converted.Key = data.Key;
            converted.FirstName = data.FirstName;
            converted.LastName = data.LastName;
            converted.Birth = data.Birth;
            converted.Gender = data.Gender;
            converted.Hometown = data.HomeTown;
            converted.Grade = data.Grade;
            converted.Nature = data.Nature;
            converted.Religion = data.Religion;
            converted.Loyalty = data.Loyalty;
            converted.DescriptVassal = data.DescriptVassal;
            converted.BaseStat_1 = data.BaseStat1;
            converted.BaseValue_1 = data.BaseValue1;
            converted.BaseStat_2 = data.BaseStat2;
            converted.BaseValue_2 = data.BaseValue2;
            converted.BaseStat_3 = data.BaseStat3;
            converted.BaseValue_3 = data.BaseValue3;
            converted.BaseJobClass_1 = data.BaseJobClass1;
            converted.JobClassLevel_1 = data.JobClassLevel1;
            converted.BaseJobClass_2 = data.BaseJobClass2;
            converted.JobClassLevel_2 = data.JobClassLevel2;
            converted.BaseJobClass_3 = data.BaseJobClass3;
            converted.JobClassLevel_3 = data.JobClassLevel3;
            int statsCount = data.VassalStatsInfoLength;
            List<BaseDataVassalStatValue> convertedStats = new();
            for (int j = 0; j < statsCount; j++)
            {
                var fbStatsData = data.VassalStatsInfo(j).Value;
                var convertedStat = new BaseDataVassalStatValue()
                {
                    Key = fbStatsData.KeyStat,
                    Value = fbStatsData.ValueStat
                };
                convertedStats.Add(convertedStat);
            }
            converted.BaseDataVassalStats = convertedStats;

            var vassalConvertedSOData = new VassalInSOList(data);
            vassalConvertedSOData.stats = ExtractVassalStats(data);

            converteds.Add(converted);
            vassalsConvertedSOData.Add(vassalConvertedSOData);
        }

        MasterDataStore.Instance.BaseVassalTemplates = converteds;
        _vassals.Init(vassalsConvertedSOData);
    }

    private int[] ExtractVassalStats(VassalTemplate data)
    {
        int statCount = data.VassalStatsInfoLength;
        int[] stats = new int[statCount];
        for (int i = 0; i < statCount; i++)
        {
            var statData = data.VassalStatsInfo(i).Value;
            stats[statData.KeyStat - 1] = statData.ValueStat;
        }

        return stats;
    }

    private void ParseDataPriorityTemplate(ApiGetLowData fbData)
    {
        int count = fbData.DataPriorityTemplateLength;
        List<BaseDataPriority> convertedDates = new();
        for (int i = 0; i < count; i++)
        {
            var fbPriorityData= fbData.DataPriorityTemplate(i).Value;

            var convertedDate = new BaseDataPriority()
            {
                Id = fbPriorityData.Id,
                Name = fbPriorityData.Name,
                Percent = fbPriorityData.Percent
            };

            convertedDates.Add(convertedDate);
        }

        MasterDataStore.Instance.BaseDataPrioritys = convertedDates;
    }

    private void ParseVassalJobInfo(ApiGetLowData fbData)
    {
        int count = fbData.VassalJobInfoLength;
        List<BaseVassalJobInfo> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.VassalJobInfo(i).Value;

            var converted = new BaseVassalJobInfo()
            {
                Key = data.Key,
                JobName = data.JobName,
                TextKey = data.TextKey,
                DerivedJob = data.DerivedJob,
                JobGrade = data.JobGrade,
                JobClass = data.JobClass,
                Req_JobClass_Lv = data.ReqJobClassLv,
                Add_StatKey_1 = data.AddStatKey1,
                Add_StatValue_1 = data.AddStatValue1,
                Add_StatKey_2 = data.AddStatKey2,
                Add_StatValue_2 = data.AddStatValue2,
                Add_StatKey_3 = data.AddStatKey3,
                Add_StatValue_3 = data.AddStatValue3,
                Add_StatKey_4 = data.AddStatKey4,
                Add_StatValue_4 = data.AddStatValue4,
                BuffID_1 = data.BuffID1,
                BuffValue_1 = data.BuffValue1,
                BuffID_2 = data.BuffID2,
                BuffValue_2 = data.BuffValue2
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseVassalJobInfos = converteds;
    }

    private void ParseVassalJobTreeDefaut(ApiGetLowData fbData)
    {
        int count = fbData.VassalJobTreeDefautLength;
        List<BaseVassalJobTreeDefaut> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.VassalJobTreeDefaut(i).Value;

            var converted = new BaseVassalJobTreeDefaut()
            {
                Key = data.Key,
                Level1st = data.Level1st,
                Level2nd = data.Level2nd,
                Level3rd = data.Level3rd,
                Level4th = data.Level4th,
                Level5th = data.Level5th,
                Level6th = data.Level1st
            };

            converteds.Add(converted);
        }

        MasterDataStore.Instance.BaseVassalJobTreeDefauts = converteds;
    }

    private void ParseMilitaryTemplate(ApiGetLowData fbData)
    {
        int count = fbData.DataMilitaryTemplateLength;
        List<BaseMilitaryTemplate> convertedDates = new();
        for (int i = 0; i < count; i++)
        {
            var fbMilitaryData = fbData.DataMilitaryTemplate(i).Value;

            var convertedDate = new BaseMilitaryTemplate()
            {
                idMilitaryTemplaye = fbMilitaryData.IdMilitaryTemplaye,
                NameMilitary = fbMilitaryData.NameMilitary
            };

            convertedDates.Add(convertedDate);
        }

        MasterDataStore.Instance.BaseMilitaryTemplates = convertedDates;
    }

    private void ParseCities(ApiGetLowData fbData)
    {
        int count = fbData.CityWorldMapLength;
        List<BaseCities> convertedCities = new();
        for (int i = 0; i < count; i++)
        {
            var fbCities = fbData.CityWorldMap(i).Value;

            var convertedCiti = new BaseCities()
            {
                id = fbCities.Id,
                idLand = fbCities.IdLand,
                PosX = fbCities.PosX,
                PosY = fbCities.PosY,
                Name = fbCities.Name,
                IdGround = fbCities.IdGround
            };
            convertedCities.Add(convertedCiti);
        }

        _cities.Init(fbData);
        MasterDataStoreGlobal.Instance.BaseCitiesList = convertedCities;
    }

    private void ParseLands(ApiGetLowData fbData)
    {
        _landStaticInfos.Init(fbData);
    }

    private static void ParseLandSingleton(ApiGetLowData fbData)
    {
        int count = fbData.LandInfoLength;
        List<BaseLand> convertedLands = new();
        for (int i = 0; i < count; i++)
        {
            var fbLands = fbData.LandInfo(i).Value;

            var convertedLand = new BaseLand()
            {
                Key = fbLands.Key,
                GroundId = fbLands.GroundId,
                MaxCity = fbLands.MaxCity,
                LandName = fbLands.LandName,
                LandNameTextKey = fbLands.LandNameTextKey,
                LandSprite = fbLands.LandSprite,
                Purchasable = fbLands.Purchasable,
                RegionalProductItem = fbLands.RegionalProductItem,
                TerrainKEY = fbLands.TerrainKEY,
                Religion = fbLands.Religion,
                Culture = fbLands.Culture,
                Race = fbLands.Race,
                PopulationGrade = fbLands.PopulationGrade,
                BuffID = fbLands.BuffID,
                BuffType = fbLands.BuffType,
                BuffValue = fbLands.BuffValue
            };

            convertedLands.Add(convertedLand);
        }

        MasterDataStoreGlobal.Instance.BaseLands = convertedLands;
    }

    private void ParseMilitaryProficiency(ApiGetLowData fbData)
    {
        int count = fbData.DataMilitaryProficiencyLength;
        List<BaseMilitaryProficiency> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataMilitaryProficiency(i).Value;

            var converted = new BaseMilitaryProficiency()
            {
                ProficiencyKey = data.ProficiencyKey,
                ProficiencyMAXExp = data.ProficiencyMAXExp
            };
            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseMilitaryProficiencys = converteds;
    }

    private void ParseMilitaryFomation(ApiGetLowData fbData)
    {
        int count = fbData.DataMilitaryFomationLength;
        List<BaseMilitaryFomation> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataMilitaryFomation(i).Value;

            var converted = new BaseMilitaryFomation()
            {
                FormationKey = data.FormationKey,
                Memo = data.Memo
            };
            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseMilitaryFomations = converteds;
    }

    private void ParseMilitaryBranch(ApiGetLowData fbData)
    {
        int count = fbData.DataMilitaryBranchLength;
        List<BaseMilitaryBranch> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataMilitaryBranch(i).Value;

            var converted = new BaseMilitaryBranch()
            {
                BranchKey = data.BranchKey,
                BranchName = data.BranchName,
                MilitaryKey = data.MilitaryKey,
                AvaliableCuture = data.AvaliableCuture,
                AvailableReligion = data.AvailableReligion,
                Branch_ItemKey = data.BranchItemKey,
                Branch_ItemValue = data.BranchItemValue,
                SupplyConsumption = data.SupplyConsumption,
                StatusFactor = data.StatusFactor,
                ItemHoldCountL = data.ItemHoldCountL,
                Req_Content_1 = data.ReqContent1,
                Req_Key_1 = data.ReqKey1,
                Req_Condition_1 = data.ReqCondition1,
                Req_Value_1 = data.ReqValue1,
                Req_Content_2 = data.ReqContent2,
                Req_Key_2 = data.ReqKey2,
                Req_Condition_2 = data.ReqCondition2,
                Req_Value_2 = data.ReqValue2
            };
            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseMilitaryBranchs = converteds;
    }

    private void ParseMilitaryInfo(ApiGetLowData fbData)
    {
        int count = fbData.DataMiliraryInfoLength;
        List<BaseMilitaryInfo> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataMiliraryInfo(i).Value;

            var converted = new BaseMilitaryInfo()
            {
                MilitaryKey = data.MilitaryKey,
                MilitaryName = data.MilitaryName,
                Health = data.Health,
                Attack = data.Attack,
                Defense = data.Defense,
                MoveSpeed = data.MoveSpeed,
                Range = data.Range,
                TrainableBuilding = data.TrainableBuilding
            };
            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseMilitaryInfos = converteds;
    }

    private static void ParseSymbolBg(ApiGetLowData fbData)
    {
        int count = fbData.DataSymbolBgLength;
        List<BaseDataSymbolBg> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataSymbolBg(i).Value;

            var converted = new BaseDataSymbolBg()
            {
                Key = data.Key,
                Sprite = data.Sprite,
                Random = data.Random
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseDataSymbolBgs = converteds;
    }

    private static void ParseSymbolLayout(ApiGetLowData fbData)
    {
        int count = fbData.DataSymbolLayoutLength;
        List<BaseDataSymbolLayout> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataSymbolLayout(i).Value;

            var converted = new BaseDataSymbolLayout()
            {
                Key = data.Key,
                Random = data.Random,
                IconA1 = data.IconA1,
                IconA2 = data.IconA2,
                IconA3 = data.IconA3,
                IconB = data.IconB
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseDataSymbolLayouts = converteds;
    }

    private static void ParseSymbolIconSub(ApiGetLowData fbData)
    {
        int count = fbData.DataSymbolIconSubLength;
        List<BaseDataSymbolIconSub> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataSymbolIconSub(i).Value;

            var converted = new BaseDataSymbolIconSub()
            {
                Key = data.Key,
                Sprite = data.Sprite,
                Random = data.Random
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseDataSymbolIconSubs = converteds;
    }

    private static void ParseSymbolIconMain(ApiGetLowData fbData)
    {
        int count = fbData.DataSymbolIconMainLength;
        List<BaseDataSymbolIconMain> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataSymbolIconMain(i).Value;

            var converted = new BaseDataSymbolIconMain()
            {
                Key = data.Key,
                Sprite = data.Sprite,
                Random = data.Random
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseDataSymbolIconMains = converteds;
    }

    private static void ParseSymbolColor(ApiGetLowData fbData)
    {
        int count = fbData.DataSymbolColorLength;
        List<BaseDataSymbolColor> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataSymbolColor(i).Value;

            var converted = new BaseDataSymbolColor()
            {
                Key = data.Key,
                Icon = data.Icon,
                IconDark = data.IconDark,
                Background = data.Background
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseDataSymbolColors = converteds;
    }

    private static void ParseSymbolDisplay(ApiGetLowData fbData)
    {
        int count = fbData.DataSymbolDisplayLength;
        List<BaseDataSymbolDisplay> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataSymbolDisplay(i).Value;

            var converted = new BaseDataSymbolDisplay()
            {
                Key = data.Key,
                FrameSprite = data.FrameSprite,
                ShowLandSymbol = data.ShowLandSymbol,
                ShowKingdomSymbol = data.ShowKingdomSymbol,
                TerritoryRepresent = data.TerritoryRepresent,
                DisplayType = data.DisplayType
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseDataSymbolDisplays = converteds;
    }

    private static void ParseBandDefaut(ApiGetLowData fbData)
    {
        int count = fbData.DataBandDefautLength;
        List<BaseBandDefaut> converteds = new();
        for (int i = 0; i < count; i++)
        {
            var data = fbData.DataBandDefaut(i).Value;

            var converted = new BaseBandDefaut()
            {
                BandKey = data.BandKey,
                MilitaryKey = data.MilitaryKey,
                Proficiency_1 = data.Proficiency1,
                Proficiency_2 = data.Proficiency2,
                Proficiency_3 = data.Proficiency3,
                Pawns = data.Pawns,
                MoraleCount = data.MoraleCount
            };

            converteds.Add(converted);
        }

        MasterDataStoreGlobal.Instance.BaseBandDefauts = converteds;
        MasterDataStoreGlobal.Instance.BaseBandDefautsDict = MasterDataStoreGlobal.ConvertToDict<int,BaseBandDefaut>( converteds , x => x.BandKey);
    }

    private void ParseItemMasterData(ApiGetLowData data)
    {
        _items.Init(data);
        _productFormulas.Init(data);
        _productionDesList.Init(data);
    }

    private void ParseItemGroupMasterData(ApiGetLowData data)
    {
        _itemGroups.Init(data);
    }

    private void ParseItemTypes(ApiGetLowData data)
    {
        _itemTypes.Init(data);
    }

    private void ParsePrefixMasterData(ApiGetLowData data)
    {
        _prefixes.Init(data);
    }

    private async void InitGame()
    {
        await UniTask.DelayFrame(4);
        MasterDataStore.Instance.SetPopulationAgeInfos();
        _ = GameManager.Instance.StartGame();
    }

}