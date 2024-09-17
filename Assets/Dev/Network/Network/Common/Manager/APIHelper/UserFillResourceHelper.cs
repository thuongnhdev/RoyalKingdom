using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFillResourceHelper 
{
    public static byte[] CreateFBAddFillResourceRequestBody(AddFillResourceRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestUpdateMaterial.StartRequestUpdateMaterial(builder);

        RequestUpdateMaterial.AddBuildingPlayerId(builder, body.BuildingPlayerId);
        RequestUpdateMaterial.AddIdItem(builder, body.IdItem);
        RequestUpdateMaterial.AddItemCount(builder, body.ItemCount);

        var offset = RequestUpdateMaterial.EndRequestUpdateMaterial(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
public class AddFillResourceRequestBody
{
    public int BuildingPlayerId;
    public int IdItem;
    public int ItemCount;
}