using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserNumberMilitaryHelper : MonoBehaviour
{
    public static byte[] CreateFBAddNumberMilitaryRequestBody(AddNumberMilitaryRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestNumberMilitary.StartRequestNumberMilitary(builder);

        RequestNumberMilitary.AddMilitary(builder, body.Military);
        RequestNumberMilitary.AddArcher(builder, body.Archer);
        RequestNumberMilitary.AddCavalry(builder, body.Cavalry);
        RequestNumberMilitary.AddInfantry(builder, body.Infantry);
        RequestNumberMilitary.AddMilitary4(builder, body.Military4);
        RequestNumberMilitary.AddTotalCitizen(builder, body.TotalCitizen);

        var offset = RequestNumberMilitary.EndRequestNumberMilitary(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddNumberMilitaryRequestBody
{
    public int Military;
    public int TotalCitizen;
    public int Archer;
    public int Cavalry;
    public int Infantry;
    public int Military4;
}