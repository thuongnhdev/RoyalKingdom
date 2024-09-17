using Cysharp.Threading.Tasks;
using Fbs;
using Google.FlatBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ApiTest", menuName = "Uniflow/Tool/ApiTest")]
public class FlatBuffersApiTest : ScriptableObject
{
    public string host;
    public string url;
    public StringVariable token;
    public RequestType requestType;
    public List<Header> header;
    public List<BodyElement> body;
    public string bodyClassName;
    public string BodyClassQualifiedName => $"Fbs.{bodyClassName}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    public string responseClassName;
    public string ResponseClassQualifiedName => $"Fbs.{responseClassName}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
    [TextArea(1, 10)]
    public string result;

    public async UniTaskVoid SendRequestAndParseResult()
    {
        Type responseType = Type.GetType(ResponseClassQualifiedName);

        var response = await SendRequest();
        Debug.Log($"Response code [{response.responseCode}]");

        string readMethodName = $"GetRootAs{responseClassName}";
        MethodInfo readMethod = responseType.GetMethod(readMethodName, new Type[] { typeof(ByteBuffer) });

        ByteBuffer bb = new(response.rawdata);

        var data = readMethod.Invoke(null, new[] { bb });

        var props = data.GetType().GetProperties();
        StringBuilder sb = new();
        for (int i = 0; i < props.Length; i++)
        {
            var prop = props[i];
            sb.Append(prop.Name).Append(": ").Append(prop.GetValue(data));
            sb.Append("\n");
        }

        result = sb.ToString();
    }

    private async UniTask<WebRequestResponse> SendRequest()
    {
        var header = CreateHeader();

        if (requestType == RequestType.GET)
        {
            return await RequestSenderAsync.SendGetRequest(host + url, token.Value, header);
        }

        byte[] body = CreateBody();
        if (requestType == RequestType.PUT)
        {
            return await RequestSenderAsync.SendPutRequest(host + url, body, token.Value);
        }

        return await RequestSenderAsync.SendPostRequest(host + url, body, token.Value);
    }

    private Tuple<string, string>[] CreateHeader()
    {
        Tuple<string, string>[] result = new Tuple<string, string>[header.Count];
        for (int i = 0; i < header.Count; i++)
        {
            var hd = header[i];
            result[i] = new Tuple<string, string>(hd.key, hd.value);
        }

        return result;
    }

    private byte[] CreateBody()
    {
        FlatBufferBuilder builder = new(1024);
        List<string> methodsName = new();

        for (int i = 0; i < body.Count; i++)
        {
            var b = body[i];
            methodsName.Add("Add" + b.fieldName);
        }

        Type type = Type.GetType(BodyClassQualifiedName);

        var startMethod = type.GetMethod("Start" + bodyClassName);
        startMethod.Invoke(null, new[] { builder });
        for (int i = 0; i < methodsName.Count; i++)
        {
            var addMethod = type.GetMethod(methodsName[i]);
            object value = ParseValue(body[i]);

            addMethod.Invoke(null, new[] {builder, value });
        }
        var endMethod = type.GetMethod("End" + bodyClassName);
        var offset = endMethod.Invoke(null, new[] { builder });
        int offsetValue = (int)offset.GetType().GetField("Value").GetValue(offset);
        builder.Finish(offsetValue);

        return builder.SizedByteArray();
    }

    private object ParseValue(BodyElement bodyElement)
    {
        DataType type = bodyElement.type;
        if (type == DataType.Integer)
        {
            int.TryParse(bodyElement.value, out int intValue);
            return intValue;
        }

        if (type == DataType.Long)
        {
            long.TryParse(bodyElement.value, out long longValue);
            return longValue;
        }

        if (type == DataType.Float)
        {
            float.TryParse(bodyElement.value, out float floatValue);
            return floatValue;
        }

        if (type == DataType.Bool)
        {
            bool.TryParse(bodyElement.value, out bool boolValue);
            return boolValue;
        }

        return bodyElement.value;
    }

    [Serializable]
    public class Header
    {
        public string key;
        public string value;
    }
    [Serializable]
    public class BodyElement
    {
        public string fieldName;
        public DataType type;
        public string value;
    }
    public enum DataType
    {
        None = -1,
        Integer = 0,
        Float = 1,
        String = 2,
        Bool = 3,
        Long = 4

    }
}

[CustomEditor(typeof(FlatBuffersApiTest))]
public class FlatBufferApiTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Send!"))
        {
            var myTarget = (FlatBuffersApiTest)target;
            myTarget.SendRequestAndParseResult().Forget();
        }
    }
}
