using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestStartBattleWarHelper : MonoBehaviour
{
    public static byte[] CreateStartBattleRequestBody(AddStartBattleRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestStartBattleWar.StartRequestStartBattleWar(builder);

        RequestStartBattleWar.AddUid(builder, body.Uid);
        RequestStartBattleWar.AddMessagePacketCode(builder, Fb.WorldMap.BATTLE_START);

        var offset = RequestStartBattleWar.EndRequestStartBattleWar(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddStartBattleRequestBody
{
    public long Uid;
    public int TypeBattle;
}