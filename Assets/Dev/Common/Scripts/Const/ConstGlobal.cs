public static class ConstGlobal
{
    public const int _TRUE = 1;
    public const int _FALSE = 0;

    public const int _UNKNOWN = -1;

    public const string _STR_TEMP = "class ConstGlobal";

    public const float _DRAG_THRESHOLD_CM = 0.5f;
    public const float _INCH_TO_CM = 2.54f;    
    public const int _V_SYNC_COUNT = 0;

    public const int _FRAME_RATE = 40;    //



    public const int _QUALITY_VERY_HIGH = 0;
    public const int _QUALITY_HIGH = 1;
    public const int _QUALITY_MEDIUM = 2;
    public const int _QUALITY_LOW = 3;
    public const int _QUALITY_VERY_LOW = 4;

    // Server Consttant
    public const int _USER_STATUS_RESUME = 0;     // 유저 상태 RESUME
    public const int _USER_STATUS_PAUSE = 1;      // 유저 상태 PAUSE
    public const int _USER_STATUS_EXIT = -1;      // 유저 상태 EXIT


    public const string _SCE_RESTART = "Restart";
    public const string _SCE_MAIN = "Main";


    
}
//PlayerPrefs
public static class ConstPlayerPrefs
{
    public const string _OPT_LANGUAGE = "OptLanguage";
    public const string _OPT_GAME_QUALITY = "OptGameQuality";
    public const string _OPT_BGM_VOLUME = "OptBgmVolume";
    public const string _OPT_EFFECT_VOLUME = "OptEffectBolume";
    public const string _OPT_BGM = "OptBgm";
    public const string _OPT_EFFECT = "OptEffect";
}

public enum E_GameTextDataType
{
    none,
    emBattle_rules,
    emBattle_subaction,
    emDefinition,
    emEmoticon,
    emGametip,
    emQuest,
    emTutorial,
    emUI,
    emNation,
    emPlayerSpell,
    emCommon,
}
public enum E_ServerType
{
    Local = 0,
    Remote = 1
}

public enum E_AccessType
{
    Dev = 0,
    Plan = 1
}
public enum E_MenuName
{
    None,
    TempMenu_1,
    TempMenu_2,
    TempMenu_3,
    TempMenu_4,
    MAX
};
public enum E_PopupName
{
    None,
    TempPopup_1,
    TempPopup_2,
    TempPopup_3,
    TempPopup_4,
    MAX
};
