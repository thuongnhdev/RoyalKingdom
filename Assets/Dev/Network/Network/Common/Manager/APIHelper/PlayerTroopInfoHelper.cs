using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;

public class PlayerTroopInfoHelper : MonoBehaviour
{
    public static byte[] CreateFBPlayerTroopInfoRequestBody(List<PlayerTroopInfoBody> playerTroopInfoBodys)
    {
        
        FlatBufferBuilder builder = new FlatBufferBuilder(1024);
        //RequestUpdateTroops.StartRequestUpdateTroops(builder);
        var troopQueue = CreateFbTroopQueue(builder, playerTroopInfoBodys);
        Debug.Log("REQUEST DATA UPDATE TROOPS ===== "+troopQueue);
        var DataRequestUpdateTroops = RequestUpdateTroops.CreateRequestUpdateTroops(
                builder,
                RequestUpdateTroops.CreateDataTroopsInfoVector(builder, troopQueue)
            );
        //RequestUpdateTroops.StartRequestUpdateTroops(builder);
        //var queuedTroopOffsetVector = RequestUpdateTroops.CreateDataTroopsInfoVector(builder, troopQueue);
        //RequestUpdateTroops.AddDataTroopsInfo(builder, queuedTroopOffsetVector);

        //var offset = RequestUpdateTroops.EndRequestUpdateTroops(builder);
        if (DataRequestUpdateTroops.Value > 0)
        {
            builder.Finish(DataRequestUpdateTroops.Value);
        }
      
        
        return builder.SizedByteArray();
    }

    private static Offset<RequestPlayerTroopInfo>[] CreateFbTroopQueue(FlatBufferBuilder builder, List<PlayerTroopInfoBody> playerTroopInfoBodys)
    {
        var itemQueue = new Offset<RequestPlayerTroopInfo>[playerTroopInfoBodys.Count];
        for (int i = 0; i < playerTroopInfoBodys.Count; i++)
        {
            itemQueue[i] = RequestPlayerTroopInfo.CreateRequestPlayerTroopInfo(
                builder,
                playerTroopInfoBodys[i].IdTroop,
                playerTroopInfoBodys[i].Vassal_1,
                    playerTroopInfoBodys[i].Vassal_2,
                    playerTroopInfoBodys[i].Vassal_3,
                    playerTroopInfoBodys[i].Attack_1,
                    playerTroopInfoBodys[i].Attack_2,
                    playerTroopInfoBodys[i].Attack_3,
                    playerTroopInfoBodys[i].Attack_4,
                    playerTroopInfoBodys[i].Attack_5,
                    playerTroopInfoBodys[i].Pawns,
                    playerTroopInfoBodys[i].Value_Infantry,
                    playerTroopInfoBodys[i].Value_Archer,
                    playerTroopInfoBodys[i].Value_Cavalry,
                    playerTroopInfoBodys[i].Status,
                    playerTroopInfoBodys[i].Buff_1,
                    playerTroopInfoBodys[i].Buff_2,
                    playerTroopInfoBodys[i].Buff_3,
                      playerTroopInfoBodys[i].Buff_4,
                      playerTroopInfoBodys[i].idType
            );
        }

        return itemQueue;
    }

}


public class PlayerTroopInfoBody
{
    public int IdTroop;
    public int Vassal_1;
    public int Vassal_2;
    public int Vassal_3;
    public int Attack_1;
    public int Attack_2;
    public int Attack_3;
    public int Attack_4;
    public int Attack_5;
    public int Pawns;
    public int Value_Infantry; // bộ binh
    public int Value_Archer;   // cung thủ
    public int Value_Cavalry; // kỵ binh
    public int Status;
    public int Buff_1;
    public int Buff_2;
    public int Buff_3;
    public int Buff_4;
    public int idType;
}