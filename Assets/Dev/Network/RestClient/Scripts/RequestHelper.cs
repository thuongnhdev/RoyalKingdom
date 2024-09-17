using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RequestHelper
{
    public static UnityWebRequest GetUnityWebRequest(string url, RequestType requestType, byte[] data, string token, params Tuple<string, string>[] customHeader)
    {
        if (requestType == RequestType.GET)
        {
            return CreateGetRequest(url, token, customHeader);
        }

        return CreatePostOrPutRequest(url, requestType, data, token, customHeader);
    }

    private static UnityWebRequest CreateGetRequest(string url, string token, params Tuple<string, string>[] customHeader)
    {
        var request = UnityWebRequest.Get(url);

        if (token != null)
        {
            StringBuilder tokenSb = new StringBuilder();
            tokenSb.Append(token);
            request.SetRequestHeader("Authorization", tokenSb.ToString());
        }

        if (customHeader.Length != 0)
        {
            for (int i = 0; i < customHeader.Length - 1; i++)
            {
                var headerField = customHeader[i];
                request.SetRequestHeader(headerField.Item1, headerField.Item2);
            }
        }

        return request;
    }

    private static UnityWebRequest CreatePostOrPutRequest(string url, RequestType requestType, byte[] requestBody, string token, params Tuple<string, string>[] customHeader)
    {
        UnityWebRequest request = new UnityWebRequest
        {
            url = url,
            method = requestType.ToString()
        };

        request.downloadHandler = new DownloadHandlerBuffer();

        if (requestBody != null)
        {
            request.uploadHandler = new UploadHandlerRaw(requestBody);
        }

        if (token != null)
        {
            request.SetRequestHeader("Authorization", token);
        }

        if (customHeader.Length != 0)
        {
            for (int i = 0; i < customHeader.Length; i++)
            {
                var headerField = customHeader[i];
                request.SetRequestHeader(headerField.Item1, headerField.Item2);
            }
        }

        string contentType = request.GetRequestHeader("Content-Type");
        if (string.IsNullOrEmpty(contentType))
        {
            request.SetRequestHeader("Content-Type", "application/json");
        }
        return request;
    }
}
