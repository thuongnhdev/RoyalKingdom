using CoreData.UniFlow.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WorkerData", menuName = "WorkerData", order = 1)]
[PreferBinarySerialization]
public class WorkerData : ScriptableObject
{
    public int ID;
    public string Name;
    public string Age;
    public Character.Gender Gender;
    public float Height;
    public float Weight;
    public string PlaceOfBirth;
    public int UserID;
    public string Title;
    public string Magistrate;
    public int IDOfSubordinate;
    public int IdJob;
    public Character.AbilityStats AbilityStats;
    public Character.HealthStatus HealthStatus;
    public Character.Mood Mood;
    public int Loyalty;
    public Character.CharacterList Type;


}