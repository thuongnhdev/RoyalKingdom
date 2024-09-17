using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserStartBuildingCacheHelper : MonoBehaviour
{
    public static byte[] CreateFBAddVassalChoiceRequestBody(AddStartBuildingCacheRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestFillResource.StartRequestFillResource(builder);

        RequestFillResource.AddBuildingPlayerId(builder, body.BuildingPlayerId);

        var offset = RequestFillResource.EndRequestFillResource(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddStartBuildingCacheRequestBody
{
    public int BuildingPlayerId;
}