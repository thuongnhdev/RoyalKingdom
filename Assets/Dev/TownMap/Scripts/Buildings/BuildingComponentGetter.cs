using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentGetter : MonoBehaviour
{
    [field: SerializeField]
    public BuildingOperation Operation {get; private set;}
    [field: SerializeField]
    public BuildingProduction Production { get; private set; }
    [field: SerializeField]
    public TilingTransform TilingTransform { get; private set; }
    [field: SerializeField]
    public BuildingPolisher Polisher { get; private set; }
    [field: SerializeField]
    public Transform BuildingModelHolder { get; private set; }
    [field: SerializeField]
    public BuildingObjectUI BuildingObjectUI { get; private set; }
    [field: SerializeField]
    public GameObject BuildingCommand1Button { get; private set; }
    [field:SerializeField]
    public GameObject BuildingCommand2Button { get; private set; }
}