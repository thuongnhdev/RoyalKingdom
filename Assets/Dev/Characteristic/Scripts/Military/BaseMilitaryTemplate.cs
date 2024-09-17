using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseMilitaryTemplate
{
	public int idMilitaryTemplaye;
	public string NameMilitary;
	public void SetData(BaseMilitaryTemplate data)
	{
		idMilitaryTemplaye = data.idMilitaryTemplaye;
		NameMilitary = data.NameMilitary;
	}
}
