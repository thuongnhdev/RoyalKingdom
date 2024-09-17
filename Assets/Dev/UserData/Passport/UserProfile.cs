using Fbs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserProfile", menuName = "Uniflow/User/UserProfile")]
public class UserProfile : ScriptableObject
{
    public long id;
    public string userName;
    public int landId;
    public int ownedLandId;
    public int kingdomId;
    public int ownedKingdomId;
    public int kingdomAuthority;
    public int CountMilitary;
    public int MilitaryType;
    public LandAuthority landAuthority;

    public void Init(ApiLoginResult data)
    {
        id = data.Uid;
        landId = data.IdLand;
        kingdomId = data.IdKingdom;
        CountMilitary = data.MilitaryCout;
        MilitaryType = data.TypeMilitary;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
}
