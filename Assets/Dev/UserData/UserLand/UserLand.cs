using UnityEngine;

[CreateAssetMenu(fileName = "UserLand", menuName = "Uniflow/User/Land")]
public class UserLand : ScriptableObject
{
    public int landId;
    public int kingdomId;
    public long population;
    public float taxRate;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
