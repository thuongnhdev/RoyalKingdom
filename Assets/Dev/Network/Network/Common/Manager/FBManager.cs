using System.Collections;
using System.Collections.Generic;
using Fbs;
using Google.FlatBuffers;
using UnityEngine;

public class FBManager : MonoBehaviour
{
    [SerializeField]
    private UserProfile UserProfile;
    public void CreateFb_CommonMsg_Empty(ref FlatBufferBuilder fbb, int packetCode)
    {
        Offset<CommonMsg> Msg = CommonMsg.CreateCommonMsg(fbb, packetCode, 0, StatesGlobal.UID_PLAYER, fbb.CreateString(""), 0
        ,UserProfile.landId, UserProfile.kingdomId, UserProfile.CountMilitary, UserProfile.MilitaryType);
        fbb.Finish(Msg.Value);              
    }
}
