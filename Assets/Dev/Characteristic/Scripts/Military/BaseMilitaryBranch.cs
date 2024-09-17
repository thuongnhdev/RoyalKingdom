using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseMilitaryBranch
{
    public int BranchKey;
    public string BranchName;
    public int MilitaryKey;
    public int AvaliableCuture;
    public int AvailableReligion;
    public int Branch_ItemKey;
    public int Branch_ItemValue;
    public int SupplyConsumption;
    public int StatusFactor;
    public int ItemHoldCountL;
    public string Req_Content_1;
    public int Req_Key_1;
    public string Req_Condition_1;
    public int Req_Value_1;
    public string Req_Content_2;
    public int Req_Key_2;
    public string Req_Condition_2;
    public int Req_Value_2;
	public void SetData(BaseMilitaryBranch data)
	{
        BranchKey = data.BranchKey;
        BranchName = data.BranchName;
        MilitaryKey = data.MilitaryKey;
        AvaliableCuture = data.AvaliableCuture;
        AvailableReligion = data.AvailableReligion;
        Branch_ItemKey = data.Branch_ItemKey;
        Branch_ItemValue = data.Branch_ItemValue;
        SupplyConsumption = data.SupplyConsumption;
        StatusFactor = data.StatusFactor;
        ItemHoldCountL = data.ItemHoldCountL;
        Req_Content_1 = data.Req_Content_1;
        Req_Key_1 = data.Req_Key_1;
        Req_Condition_1 = data.Req_Condition_1;
        Req_Value_1 = data.Req_Value_1;
        Req_Content_2 = data.Req_Content_2;
        Req_Key_2 = data.Req_Key_2;
        Req_Condition_2 = data.Req_Condition_2;
        Req_Value_2 = data.Req_Value_2;
    }
}
