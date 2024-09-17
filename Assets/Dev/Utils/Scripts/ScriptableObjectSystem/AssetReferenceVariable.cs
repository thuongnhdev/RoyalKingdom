using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "newAssetReferenceVar", menuName = "ScriptableObjectSystem/AssetReference")]
public class AssetReferenceVariable : BaseScriptableObjectVariable<AssetReference>
{
    protected override bool IsSetNewValue(AssetReference value)
    {
        return value.RuntimeKey != _value.RuntimeKey;
    }
}
