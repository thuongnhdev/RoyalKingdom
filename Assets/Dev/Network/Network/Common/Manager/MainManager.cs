using System.Collections;
using System.Collections.Generic;
using Fbs;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {        
        PacketManager.Instance.InitWebSocket_Lobby();
    }
    
    
    void OnComplete_Login_API(Fbs.ApiLoginResult result)
    {
        StatesGlobal.UID_PLAYER = result.Uid;

        StatesTown.LoadServerUserInfo(result);
        StatesTown.LoadPopulationInfo(result);
    }
    

}
