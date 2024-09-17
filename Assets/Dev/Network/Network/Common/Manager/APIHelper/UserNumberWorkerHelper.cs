using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserNumberWorkerHelper : MonoBehaviour
{
    public static byte[] CreateFBAddNumberWorkerRequestBody(AddNumberWorkerRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestNumberWorker.StartRequestNumberWorker(builder);

        RequestNumberWorker.AddWorker(builder, body.Worker);
        RequestNumberWorker.AddTotalCitizen(builder, body.TotalCitizen);
        RequestNumberWorker.AddPriority1(builder,body.Priority1);
        RequestNumberWorker.AddPriority2(builder, body.Priority2);
        RequestNumberWorker.AddPrioriry3(builder, body.Priority3);
        RequestNumberWorker.AddPriority4(builder, body.Priority4);

        var offset = RequestNumberWorker.EndRequestNumberWorker(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddNumberWorkerRequestBody
{
    public int Worker;
    public int TotalCitizen;
    public float Priority1;
    public float Priority2;
    public float Priority3;
    public float Priority4;
}