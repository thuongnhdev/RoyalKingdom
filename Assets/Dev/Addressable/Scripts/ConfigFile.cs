using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class TargetConfigPath
{
    public string AOS_DEV;
    public string AOS_Service;
    public string iOS_DEV;
    public string iOS_Service;
}
[System.Serializable]
public class ConfigFile 
{
    public int c_State;
    
    public string c_Msg;
    public string c_UpdateUrl;
    public int s_State;
    public string s_Msg;
    public string api_Ip;
    public string Lobby_Ip;
    public string Game_Ip;
    public string addressablePath;
    public string version;
    
    public List<string> Users;
    public List<string> errorMsg;
    
}
