using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserPopulationChangeDetailHelper : MonoBehaviour
{
    public static byte[] CreateFBAddPopulationChangeDeatailRequestBody()
    {
        FlatBufferBuilder builder = new(1024);
        RequestPopulationChangeDetail.StartRequestPopulationChangeDetail(builder);

        var offset = RequestPopulationChangeDetail.EndRequestPopulationChangeDetail(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

