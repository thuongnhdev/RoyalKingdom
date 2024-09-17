using CoreData.UniFlow.Commander;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreData.UniFlow.Common;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "VassarData", menuName = "VassarData", order = 1)]
[PreferBinarySerialization]
public class VassarData : ScriptableObject
{
    public int Key;
    public string FirstName;
    public string LastName;
    public int Birth;
    public Character.Gender Gender;
    public string Hometown;
    public int Grade;
    public int Nature;
    public string Religion;
    public int Leadership_Pot;
    public int Strength_Pot;
    public int Intelligence_Pot;
    public int Wisdom_Pot;
    public int Health_Pot;
    public int Dexterity_Pot;
    public int Charm_Pot;
    public int Eloquence_Pot;
    public int Elaborate_Pot;
    public int Analytical_Pot;
    public int Sociability_Pot;
    public int Patience_Pot;
    public int Communion_Pot;
    public int Belief_Pot;
    public int Karma_Pot;
    public int Lucky_Pot;
    public int Activity_Pot;
    public int Maritime_Pot;
    public int BaseStat;
    public int StatValue;
    public int BaseJobClass;
    public int JobClassLevel;
    public Sprite Avatar;
    public Character.CharacterList Type;

}
