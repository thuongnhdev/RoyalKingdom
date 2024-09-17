using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserLogChangeWorkerHelper : MonoBehaviour
{
    public static byte[] CreateFBAddLogWorkerRequestBody()
    {
        FlatBufferBuilder builder = new(1024);
        RequestLogChangeWorker.StartRequestLogChangeWorker(builder);

        var offset = RequestLogChangeWorker.EndRequestLogChangeWorker(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
