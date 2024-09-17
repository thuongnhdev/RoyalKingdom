using UnityEngine;

public class JsonResponseConverter<T> : IResponseRawDataConverter<T>
{
    public T Convert(byte[] rawData)
    {
        string jsonString = System.Text.Encoding.UTF8.GetString(rawData);
        return JsonUtility.FromJson<T>(jsonString);
    }
}
