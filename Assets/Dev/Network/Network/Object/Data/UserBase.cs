using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBase
{ 
    public long Uid;
    public int Citizen;
    public int Rice;
    public int Wheat;
    public int Iron;
    public int PineWood;
    public int Glass;
    public int Stone;
    public int Granite;
    public int Gold;
   

    public UserBase(Fbs.ApiLoginResult userInfo)
    {
        Uid = userInfo.Uid;
    }
    
}
