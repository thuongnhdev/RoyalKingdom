using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseMilitaryProficiency
{
	public int ProficiencyKey;
	public int ProficiencyMAXExp;
	public void SetData(BaseMilitaryProficiency data)
	{
		ProficiencyKey = data.ProficiencyKey;
		ProficiencyMAXExp = data.ProficiencyMAXExp;
	}
}
