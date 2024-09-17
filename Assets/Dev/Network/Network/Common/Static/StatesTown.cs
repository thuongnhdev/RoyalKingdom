using System.Collections;
using System.Collections.Generic;
using _0.PersonalWork.Harry.Scripts.Common.Static;
using Fbs;
using UnityEngine;

public static class StatesTown 
{
    public static UserBase userBase;
    public static PopulationBase populationBase;

    public static Dictionary<int, PlayerBase> VASSAL_LIST = new Dictionary<int, PlayerBase>();
    public static Dictionary<int, BuildingBase> BUILDING_LIST = new Dictionary<int, BuildingBase>();




    public static void LoadServerUserInfo(Fbs.ApiLoginResult userInfo)
    {
        userBase = new UserBase(userInfo);
    }

    public static void LoadVassalInfo(Fbs.LoginVassalInfo vassalInfo)
    {
        VASSAL_LIST.Add(vassalInfo.IdVassalPlayer, new PlayerBase(vassalInfo));
    }
    public static void LoadBuildingInfo(Fbs.LoginBuildingInfo buildingInfo)
    {
        BUILDING_LIST.Add(buildingInfo.BuildingPlayerId, new BuildingBase(buildingInfo));
    }

    public static void LoadPopulationInfo(Fbs.ApiLoginResult result)
    {
        populationBase = new PopulationBase(result);
    }
    
    
    

}
