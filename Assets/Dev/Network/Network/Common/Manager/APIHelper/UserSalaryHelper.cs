using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class UserSalaryHelper : MonoBehaviour
{
    public static byte[] CreateFBAddSalaryRequestBody(AddSalaryRequestBody body)
    {
        FlatBufferBuilder builder = new(1024);
        RequestSalary.StartRequestSalary(builder);

        RequestSalary.AddGold(builder, body.Gold);
        RequestSalary.AddFood(builder, body.Food);
        RequestSalary.AddFoodSalaryInMonth(builder, body.FoodSalaryInMonth);
        RequestSalary.AddGoldSalaryInMonth(builder, body.GoldSalaryInMonth);
        RequestSalary.AddPerworkLoad(builder, body.PerworkLoad);
        RequestSalary.AddMood(builder, body.Mood);

        var offset = RequestSalary.EndRequestSalary(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}

public class AddSalaryRequestBody
{
    public float Gold;
    public float Food;
    public float GoldSalaryInMonth;
    public float FoodSalaryInMonth;
    public float PerworkLoad;
    public int Mood;
}