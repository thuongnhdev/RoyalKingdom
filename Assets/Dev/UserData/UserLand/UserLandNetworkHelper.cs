using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLandNetworkHelper
{
    public static byte[] CreateFbChooseLandRequestBody(int landId)
    {
        FlatBufferBuilder builder = new(1);
        RequestChooseLand.StartRequestChooseLand(builder);

        RequestChooseLand.AddIdLand(builder, landId);

        var offset = RequestChooseLand.EndRequestChooseLand(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFbGetLandRequestBody(int landId)
    {
        FlatBufferBuilder builder = new(1);
        RequestInfoLand.StartRequestInfoLand(builder);
        RequestInfoLand.AddIdLandRequest(builder, landId);
        var offset = RequestInfoLand.EndRequestInfoLand(builder);

        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFbGetKingdomRequestBody(int kingdomId)
    {
        FlatBufferBuilder builder = new(1);

        RequestInfoKingdom.StartRequestInfoKingdom(builder);
        RequestInfoKingdom.AddIdKingdomRequest(builder, kingdomId);
        var offset = RequestInfoKingdom.EndRequestInfoKingdom(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
