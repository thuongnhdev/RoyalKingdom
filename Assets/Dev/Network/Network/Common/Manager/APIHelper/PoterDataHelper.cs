using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using CoreData.UniFlow.Common;
using UnityEngine;
using Fbs;
using UnityEngine.UIElements;
using TaskData = CoreData.UniFlow.Common.TaskData;

public class PoterDataHelper : MonoBehaviour
{
    public static byte[] CreateFbpoterDataRequestBody(PoterDataRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        var commonResource = CreateFbCommonResource(builder, body.CommonResource);
        var poterPosition = CreateFbPoterPosition(builder, body.PosterPositions);
        var ApiRequest = RequestPoterData.CreateRequestPoterData(builder,
            body.TaskId,
            body.BuildingPlayerId,
            body.Priority,
            body.IdJob,
            builder.CreateVectorOfTables(commonResource),
            builder.CreateVectorOfTables(poterPosition)
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

    private static Offset<Fbs.PosterPosition>[] CreateFbPoterPosition(FlatBufferBuilder builder, List<PosterPosition> data)
    {
        var itemQueue = new Offset<Fbs.PosterPosition>[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            var typeHouseMove = CreateFbTypeHouseMove(builder, data[i].TypeHouseMove);
            Offset<Fbs.PosterPosition> offset = Fbs.PosterPosition.CreatePosterPosition(builder,
                data[i].Index,
                builder.CreateString(data[i].Position),
                data[i].TimeMove,
                data[i].TimeAnimation,
                typeHouseMove
                );
            itemQueue[i] = offset;
        }
        return itemQueue;

    }

    private static Offset<Fbs.TypeHouseMove> CreateFbTypeHouseMove(FlatBufferBuilder builder, TypeHouseMove data)
    {
        var itemQueue = new Offset<Fbs.TypeHouseMove>();
        Offset<Fbs.TypeHouseMove> offset = Fbs.TypeHouseMove.CreateTypeHouseMove(builder,
            data.WareHouse,
            data.StoreHouse,
            data.BuildingHouse,
            data.BackHouse
        );
        itemQueue = offset;
        return itemQueue;

    }
}

public class PoterDataRequestBody
{
    public int TaskId;
    public int BuildingPlayerId;
    public int Priority;
    public int IdJob;
    public List<ResourcePoco> CommonResource;
    public List<PosterPosition> PosterPositions;
}

public class PosterPosition
{
    public int Index;
    public string Position;
    public float TimeMove;
    public float TimeAnimation;
    public TypeHouseMove TypeHouseMove;
}

public class TypeHouseMove
{
    public int WareHouse;
    public int StoreHouse;
    public int BuildingHouse;
    public int BackHouse;
}
