using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseMilitaryInfo
{
    public int MilitaryKey;
    public string MilitaryName;
    public int Health;
    public int Attack;
    public int Defense;
    public int MoveSpeed;
    public int Range;
    public int TrainableBuilding;
	public void SetData(BaseMilitaryInfo data)
	{
        MilitaryKey = data.MilitaryKey;
        MilitaryName = data.MilitaryName;
        Health = data.Health;
        Attack = data.Attack;
        Defense = data.Defense;
        MoveSpeed = data.MoveSpeed;
        Range = data.Range;
        TrainableBuilding = data.TrainableBuilding;
    }
}
