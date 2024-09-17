using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseResponeJoinWorldMap
{
	public int ApiResult;
	public List<PlayerInWorldMap> PlayerWorldMap;
	public void SetData(BaseResponeJoinWorldMap data)
	{
		ApiResult = data.ApiResult;
		PlayerWorldMap = data.PlayerWorldMap;
	}
}

[System.Serializable]
public class PlayerInWorldMap
{
	public long Uid;
	public int IdLand;
	public string PlayerName;
	public int Possition;
	public int Status;
	public int MilitaryCount;
	public int TypeMilitary;
	public List<MilitaryPlayerInWorldMap> Military;
	public void SetData(PlayerInWorldMap data)
	{
		Uid = data.Uid;
		IdLand = data.IdLand;
		PlayerName = data.PlayerName;
		Possition = data.Possition;
		Status = data.Status;
		Military = data.Military;
		MilitaryCount = data.MilitaryCount;
		TypeMilitary = data.TypeMilitary;
	}
}

[System.Serializable]
public class MilitaryPlayerInWorldMap
{
	public int TypeMilitary;
	public int Count;
	public void SetData(MilitaryPlayerInWorldMap data)
	{
		TypeMilitary = data.TypeMilitary;
		Count = data.Count;
	}
}