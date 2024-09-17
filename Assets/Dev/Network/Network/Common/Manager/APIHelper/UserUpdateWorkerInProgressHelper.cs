using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserUpdateWorkerInProgressHelper : MonoBehaviour
{
    public static byte[] CreateFBAddUpdateWorkerInProgressRequestBody(AddUpdateWorkerInProgressRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestUpdateWorkerInProgress.StartRequestUpdateWorkerInProgress(builder);

        RequestUpdateWorkerInProgress.AddPercent(builder, body.Percent);
        RequestUpdateWorkerInProgress.AddPrioriry(builder, body.Priority);

        var offset = RequestUpdateWorkerInProgress.EndRequestUpdateWorkerInProgress(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddUpdateWorkerInProgressRequestBody
{
    public int Priority;
    public float Percent;
}