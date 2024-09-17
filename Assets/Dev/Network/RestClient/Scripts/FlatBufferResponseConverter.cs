using Google.FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FlatBufferResponseConverter<T> : IResponseRawDataConverter<T>
{
    public T Convert(byte[] data)
    {
        StringBuilder sb = new StringBuilder();

        Type type = typeof(T);
        string methodName = sb.Append("GetRootAs").Append(type.Name).ToString();

        MethodInfo method = type.GetMethod(methodName, new Type[] { typeof(ByteBuffer) });
        if (method == null)
        {
            return default;
        }

        if (data == null)
        {
            Debug.Log("No data for converting");
            return default;
        }
        
        ByteBuffer dataByteBuffer = new ByteBuffer(data);
        return (T)method.Invoke(null, new object[] { dataByteBuffer });
    }
}
