using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadComponentGetter : MonoBehaviour
{
    [field: SerializeField]
    public RoadObjectAppearance RoadAppearance {get; private set;}
}
