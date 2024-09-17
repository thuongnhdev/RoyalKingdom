using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinWorldMapHelper 
{
    public static byte[] CreateJoinWorldMapHelperRequestBody()
    {
        FlatBufferBuilder builder = new(1024);
        RequestJoinWorldMap.StartRequestJoinWorldMap(builder);
        RequestJoinWorldMap.AddUid(builder, StatesGlobal.UID_PLAYER);
        RequestJoinWorldMap.AddMessagePackageCode(builder, Fb.WorldMap.PLAYER_JOIN_MAP);
        var offset = RequestJoinWorldMap.EndRequestJoinWorldMap(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
public class AddJoinWorldMapHelperRequestBody
{
    public long Uid;
}