using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataVersionChecker : MonoBehaviour
{
    [Header("Events out")]
    [SerializeField]
    private GameEvent _onNeedUpdateMasterData;
    [SerializeField]
    private GameEvent _onUseCachedMasterData;


    public void RequestCheckMasterDataVersion()
    {
        // TODO Server logic here

        // No new version
        _onUseCachedMasterData.Raise();

        // Need new version
        _onNeedUpdateMasterData.Raise();
    }
}
