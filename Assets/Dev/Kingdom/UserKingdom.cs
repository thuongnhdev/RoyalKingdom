using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserKingdom", menuName = "Uniflow/User/UserKingdom")]
public class UserKingdom : ScriptableObject
{
    public int id;
    public int level;
    public long population;
    public float taxRate;
}
