using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectListVariable", menuName = "ScriptableObjectSystem/GameObjectListVariable")]
public class GameObjectListVariable : BaseListVariable<GameObject>
{
    protected override bool Compare(GameObject item1, GameObject item2)
    {
        return item1.GetInstanceID() == item2.GetInstanceID();
    }
}
