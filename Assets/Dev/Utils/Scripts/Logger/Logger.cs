using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public static void Log(string message, Object context = null)
    {
#if INTERNAL_BUILD
        if (context == null)
        {
            Debug.Log(message);
            return;
        }
        Debug.Log(message, context);
#endif
    }

    public static void Log(string message, Color color, Object context = null)
    {
#if INTERNAL_BUILD
        string colorHex = ColorUtility.ToHtmlStringRGBA(color);
        if (context == null)
        {
            Debug.Log($"<color=#{colorHex}>{message}</color>");
            return;
        }
        Debug.Log($"<color=#{colorHex}>{message}</color>", context);
#endif
    }

    public static void LogWarning(string message, Object context = null)
    {
#if INTERNAL_BUILD
        if (context == null)
        {
            Debug.LogWarning(message);
            return;
        }
        Debug.LogWarning(message, context);
#endif
    }

    public static void LogError(string message, Object context = null)
    {
#if INTERNAL_BUILD
        if (context == null)
        {
            Debug.LogError(message);
            return;
        }
        Debug.LogError(message, context);
#endif
    }
}
