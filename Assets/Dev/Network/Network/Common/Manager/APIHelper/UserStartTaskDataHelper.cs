using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using CoreData.UniFlow.Common;
using UnityEngine;
using Fbs;
using UnityEngine.UIElements;

public class UserStartTaskDataHelper : MonoBehaviour
{
    public static byte[] CreateFBAddStartTaskDataRequestBody(AddNextTaskDataRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        var commonResource = CreateFbCommonResource(builder, body.CommonResource);
        var workerPosition = CreateFbWorkerPosition(builder, body.WorkerPossition);
        var vassalPosition = CreateFbVassalPosition(builder, body.VassalPossition);
        var ApiRequest = RequestStartTaskData.CreateRequestStartTaskData(builder,
            body.IdTaskData,
            body.BuildingObjectId,
            body.Worker,
            body.Model,
            body.Size,
            builder.CreateString(body.Name),
            body.Priority,
            body.Timetask,
            builder.CreateString(body.Position),
            body.TimeBegin,
            body.IdJob,
            builder.CreateString(body.RotationDoor),
            body.StatusFillResource,
            body.Workload,
            body.IdVassal,
            body.BuildingId,
            body.SpeedRate,
            body.ProductId,
            RequestAddTaskData.CreateCommonResourceVector(builder, commonResource),
            RequestAddTaskData.CreateWorkerPossitionVector(builder, workerPosition),
            RequestAddTaskData.CreateVassalPossitionVector(builder, vassalPosition)
            );
        
        builder.Finish(ApiRequest.Value);

        return builder.SizedByteArray();
    }

    private static Offset<CurrentItemMaterial>[] CreateFbCommonResource(FlatBufferBuilder builder, List<ResourcePoco> data)
    {
        var itemQueue = new Offset<CurrentItemMaterial>[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            Offset<CurrentItemMaterial> offset = CurrentItemMaterial.CreateCurrentItemMaterial(builder,
                        data[i].itemId,
                        data[i].itemCount);
            itemQueue[i] = offset;


        }
        return itemQueue;

    }

    private static Offset<WorkerPosition>[] CreateFbWorkerPosition(FlatBufferBuilder builder, List<PositionWorkerBuilding> data)
    {
        var itemQueue = new Offset<WorkerPosition>[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            var pos = CreateFbWorkerVector(builder, data[i].Position);
            Offset<WorkerPosition> offset = WorkerPosition.CreateWorkerPosition(builder,
                        data[i].Index,
                        pos,
                        data[i].IsEmpty,
                        data[i].RotationY);
            itemQueue[i] = offset;


        }
        return itemQueue;
    }


    private static Offset<WorkerPosition>[] CreateFbVassalPosition(FlatBufferBuilder builder, List<PositionWorkerBuilding> data)
    {
        var itemQueue = new Offset<WorkerPosition>[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            var pos = CreateFbWorkerVector(builder, data[i].Position);
            Offset<WorkerPosition> offset = WorkerPosition.CreateWorkerPosition(builder,
                        data[i].Index,
                        pos,
                        data[i].IsEmpty,
                        data[i].RotationY);
            itemQueue[i] = offset;


        }
        return itemQueue;
    }

    private static Offset<Fbs.Vector3> CreateFbWorkerVector(FlatBufferBuilder builder, UnityEngine.Vector3 position)
    {
        Offset<Fbs.Vector3> offset = Fbs.Vector3.CreateVector3(builder,
                         position.x,
                         position.y,
                         position.z);
        return offset;
    }
}

public class AddNextTaskDataRequestBody
{
    public int IdTaskData;
    public int BuildingObjectId;
    public int Worker;
    public int Model;
    public float Size;
    public string Name;
    public int Priority;
    public float Timetask;
    public string Position;
    public long TimeBegin;
    public int IdJob;
    public string RotationDoor;
    public int StatusFillResource;
    public float Workload;
    public int IdVassal;
    public int BuildingId;
    public float SpeedRate;
    public int ProductId;
    public List<ResourcePoco> CommonResource;
    public List<PositionWorkerBuilding> WorkerPossition;
    public List<PositionWorkerBuilding> VassalPossition;
}