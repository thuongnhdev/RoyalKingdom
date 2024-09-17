using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogString 
{
       public static string GetFb_PacketType(int type)
    {
        string retLog = "Unknown";
        switch (type)
        {
            case Fb.PacketCode.LOGIN:
                retLog = "LOGIN";
                break;
        }
        retLog += "[" + type + "]";
        return retLog;
    }
}
