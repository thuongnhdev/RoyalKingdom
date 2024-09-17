using CoreData.UniFlow.Commander;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreData.UniFlow.Common;

[CreateAssetMenu(fileName = "BaseStructure", menuName = "BaseStructure", order = 1)]
[PreferBinarySerialization]
public class BaseStructure : ScriptableObject
{
    public int Age;
    public float BaseDeathProb;
    public float FertilityProb;
    public float POPULATION_MALE_BIRTH_RATE;
    public float POPULATION_MAX_DEATH_PROB;
    public float POPULATION_PRODUCTION_AGE_MIN;
    public float POPULATION_PRODUCTION_AGE_MAX;
    public float POPULATION_PRODUCTION_RATE;
    public float POPULATION_SOLDIER_AGE_MIN;
    public float POPULATION_SOLDIER_AGE_MAX;
    public float POPULATION_SOLDIER_RATE;
}
