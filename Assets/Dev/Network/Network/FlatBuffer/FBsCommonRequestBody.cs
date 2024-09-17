using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBsCommonRequestBody : MonoBehaviour
{
    public static byte[] Create1IntRequestBody(int input)
    {
        FlatBufferBuilder builder = new(1);
        builder.StartTable(1);
        builder.AddInt(input);
        int rootTable = builder.EndTable();
        builder.Finish(rootTable);

        return builder.SizedByteArray();
    }
}
