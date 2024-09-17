using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserRemoveTaskDataHelper : MonoBehaviour
{
    public static byte[] CreateFBAddRemoveTaskDataRequestBody(AddRemoveTaskDataRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestRemoveTaskData.StartRequestRemoveTaskData(builder);

        RequestRemoveTaskData.AddBuildingPlayerId(builder, body.BuildingPlayerId);
        RequestRemoveTaskData.AddTaskId(builder, body.TaskId);

        var offset = RequestRemoveTaskData.EndRequestRemoveTaskData(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddRemoveTaskDataRequestBody
{
    public int BuildingPlayerId;
    public int TaskId;
}