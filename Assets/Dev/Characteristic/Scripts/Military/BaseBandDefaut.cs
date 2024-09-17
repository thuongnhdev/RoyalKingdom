using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseBandDefaut
{
    public int BandKey;
    public int MilitaryKey;
    public int Proficiency_1;
    public int Proficiency_2;
    public int Proficiency_3;
    public int Pawns;
    public int MoraleCount;
    public void SetData(BaseBandDefaut data)
    {
        BandKey = data.BandKey;
        MilitaryKey = data.MilitaryKey;
        Proficiency_1 = data.Proficiency_1;
        Proficiency_2 = data.Proficiency_2;
        Proficiency_3 = data.Proficiency_3;
        Pawns = data.Pawns;
        MoraleCount = data.MoraleCount;
    }
}
