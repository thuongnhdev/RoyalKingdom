using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    public class DataManager : MonoSingleton<DataManager>
    {
        public void OnInitDataMaster()
        {
            MasterDataStore.Instance.SetPopulationAgeInfos();
            GameData.Instance.LoadGameData(GameData.LoadType.LOCAL);
            MasterDataStore.Instance.Init();
            MasterDataStoreGlobal.Instance.InitTroopData();
        }
    }
}
