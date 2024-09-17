using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseMilitaryFomation
{
	public int FormationKey;
	public string Memo;
	public void SetData(BaseMilitaryFomation data)
	{
		FormationKey = data.FormationKey;
		Memo = data.Memo;
	}
}
