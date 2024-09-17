using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveWorldMapHelper : MonoBehaviour
{
    public static byte[] CreateLeaveWorldMapRequestBody(AddleaveWorldMapRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestRemoveWorldMap.StartRequestRemoveWorldMap(builder);

        RequestRemoveWorldMap.AddUid(builder, body.Uid);
        RequestRemoveWorldMap.AddMessagePackageCode(builder, Fb.WorldMap.PLAYER_LEAVE);

        var offset = RequestRemoveWorldMap.EndRequestRemoveWorldMap(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddleaveWorldMapRequestBody
{
    public long Uid;
}