using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;
using Google.FlatBuffers;
public class RequestChangeVassalTaskHelper : MonoBehaviour
{
    public static byte[] CreateFBChangeVassalTaskRequestBody(AddChangeVassalTaskRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestChangeVassalTask.StartRequestChangeVassalTask(builder);

        RequestChangeVassalTask.AddIdTask(builder, body.IdTask);
        RequestChangeVassalTask.AddWorkload(builder, body.Workload);
        RequestChangeVassalTask.AddIdVassalPlayer(builder, body.IdVassalPlayer);

        var offset = RequestChangeVassalTask.EndRequestChangeVassalTask(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddChangeVassalTaskRequestBody
{
    public int IdTask;
    public int Workload;
    public int IdVassalPlayer;
}