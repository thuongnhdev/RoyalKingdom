using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Fbs;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace CoreData.UniFlow.Common
{
    public class VariableManager : MonoSingleton<VariableManager>
    {
        [SerializeField]
        private ResourceFinder _resourceFinder;

        [SerializeField]
        private ResourceSpawner _resourceSpawner;

        [SerializeField]
        private UserItemStorage _userItemStorage;

        [SerializeField]
        private UserBuildingList _userBuildingList;

        [SerializeField]
        private ProductFormulaList _productFormulaList;

        [SerializeField]
        private BuildingObjectFinder _buildingFinder;

        [SerializeField]
        private TownBaseBuildingSOList _buildingListSO;

        [SerializeField]
        private BuildingUpgradeInfoList _buildingUpgradeInfos;

        [SerializeField]
        private UserBuildingProductionList _userBuildingProductionList;


        // ScriptableObject 
        public ResourceFinder ResourceFinder => _resourceFinder;
        public ResourceSpawner ResourceSpawner => _resourceSpawner;
        public UserItemStorage UserItemStorage => _userItemStorage;
        public UserBuildingList UserBuildingList => _userBuildingList;
        public ProductFormulaList ProductFormulaList => _productFormulaList;
        public BuildingObjectFinder BuildingObjectFinder => _buildingFinder;
        public TownBaseBuildingSOList TownBaseBuildingSOList => _buildingListSO;
        public BuildingUpgradeInfoList BuildingUpgradeInfoList => _buildingUpgradeInfos;
        public UserBuildingProductionList UserBuildingProductionList => _userBuildingProductionList;
    }
}
