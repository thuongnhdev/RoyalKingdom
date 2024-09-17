using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreData.UniFlow.Common
{
    [System.Serializable]
    public class BandItemData
    {
        public int Branch;
        public int Personnel;
        public int Proficiency;

        public BandItemData(BandItemData data)
        {
            Branch = data.Branch;
            Personnel = data.Personnel;
            Proficiency = data.Proficiency;
        }
    }

    [System.Serializable]
    public class WareHouseItemData
    {
        public int Branch;
        public int Personnel;
        public int Proficiency;

        public WareHouseItemData(WareHouseItemData data)
        {
            Branch = data.Branch;
            Personnel = data.Personnel;
            Proficiency = data.Proficiency;
        }
    }
}
