using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserVassalChoiceHelper : MonoBehaviour
{
    public static byte[] CreateFBAddVassalChoiceRequestBody(AddVassalChoiceRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestVassalChoice.StartRequestVassalChoice(builder);

        RequestVassalChoice.AddBuildingPlayerId(builder, body.BuildingPlayerId);
        RequestVassalChoice.AddIdVassal(builder, body.IdVassal);
        RequestVassalChoice.AddIsChoose(builder,body.IsChoose);
        RequestVassalChoice.AddIsRemove(builder,body.IsRemove);
        RequestVassalChoice.AddIdTask(builder,body.IdTask);

        var offset = RequestVassalChoice.EndRequestVassalChoice(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
public class AddVassalChoiceRequestBody
{
    public int BuildingPlayerId;
    public int IdVassal;
    public int IsChoose;
    public int IsRemove;
    public int IdTask;
}