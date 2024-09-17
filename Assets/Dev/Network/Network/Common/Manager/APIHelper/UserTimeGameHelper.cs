using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fbs;


public class UserTimeGameHelper : MonoBehaviour
{
    public static byte[] CreateFBTimeGameRequestBody()
    {
        FlatBufferBuilder builder = new(1024);
        RequestTimeGame.StartRequestTimeGame(builder);

        var offset = RequestTimeGame.EndRequestTimeGame(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
}
