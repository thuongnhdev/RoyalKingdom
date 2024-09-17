using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MilitaryData 
{
    public int Uid;
    public string Name;
    public int LandId;
    public int TownId;
    public int KingDomId;
    public MilitaryData(int uid,string name, int townId, int landId, int kingdomId)
    {
        this.Uid = uid;
        this.Name = name;
        this.TownId = townId;
        this.LandId = landId;
        this.KingDomId = kingdomId;
    }
}

[System.Serializable]
public class MilitaryObject
{
    public MilitaryItem MilitaryItem;
    public long Uid;
    public float PosX;
    public float PosY;
    public int CityId;
    public MilitaryObject(long uid, MilitaryItem playerMilitaryItem, float posX, float posY, int cityId)
    {
        this.Uid = uid;
        this.MilitaryItem = playerMilitaryItem;
        this.PosX = posX;
        this.PosY = posY;
        this.CityId = cityId;
    }
}

[System.Serializable]
public class PlayerActionMove
{
    public Transform MilitaryTransform;
    public Action OnComplete;
    public Vector3 Begin;
    public Vector3 End;
    public void SetData(PlayerActionMove data)
    {
        MilitaryTransform = data.MilitaryTransform;
        OnComplete = data.OnComplete;
        Begin = data.Begin;
        End = data.End;
    }
}

[System.Serializable]
public class MilitaryPrefab
{
    public long Uid;
    public GameObject ObjectMilitary;
    public MilitaryPrefab(long uid, GameObject ObjectMilitary)
    {
        this.Uid = uid;
        this.ObjectMilitary = ObjectMilitary;
    }
}

[System.Serializable]
public enum TypeBattle
{
    Victory = 0,
    Territory = 1
}