public static class ConstServer
{
    public const string REG_KIND_EDITOR = "unity";
    public const string REG_KIND_ANDROID = "android";
    public const string REG_KIND_IOS = "ios";
    public const string REG_KIND_GUEST = "guest";

    //Common
    public const string WEBSOCKET_RETURN = "FALSE";
    public const char PACKET_DELI_BASE = '#';
    public const char PACKET_DELI_DATA_IN = '^';
    public const char PACKET_DELI_DATA_OUT = ',';
       

    public static ConfigFile _config = null;
    public static string _targetConfigPath = "";
}
