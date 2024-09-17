using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConst 
{
    public const float DRAG_THRESHOLD_CM = 0.5f;
    public const float INCH_TO_CM = 2.54f;
    public static int FRAME_RATE = FRAME_RATE_MENU;
    public const int V_SYNC_COUNT = 0;

    public const int FRAME_RATE_MENU = 40;
    public const int FRAME_RATE_BATTLE = 40;


    public const int TRUE = 1;
    public const int FALSE = 0;    

    public const int DIR_LEFT = 0;
    public const int DIR_RIGHT = 1;

    public const int TUTORIAL_MAX_STEP = 0;

    public const long UID_UNKNOWN = -1;
    public const long UID_STATUSEFFECT = -99;
    public const int UNKNOWN = -1;

    public const int QUALITY_VERY_HIGH = 0;
    public const int QUALITY_HIGH = 1;
    public const int QUALITY_MEDIUM = 2;
    public const int QUALITY_LOW = 3;
    public const int QUALITY_VERY_LOW = 4;

    public const int BT_MODE_NORMAL = 0;
    public const int BT_MODE_REPLAY = 1;
    public const int BT_MODE_TUTORIAL = 2;
    public const int BT_MODE_TRAINING = 3;


    public const string TAG_SKILL_CAM = "SkillCam";
        

    public const string SCE_MAIN = "MainScene";
    public const string SCE_RESTART = "Restart";


    public enum TUTOR_TYPE : int
    {
        FOCUS = 0,
        POPUP,
        FOCUS_AND_POPUP,        
        INDUCE,                
        BLOCK_BLANK,
        FOCUS_AND_POPUP_EFFECT
    };
    public enum TUTOR_POP_TYPE : int
    {
        LEFT = 0,
        CENTER,
        RIGHT
    };
    public enum TUTOR_POP_ANCHOR : int
    {
        TOP = 0,
        MIDDLE,
        BOTTOM
    };
    public enum TUTOR_INDUCE_TYPE : int
    {
        UP = 0,
        DOWN
    };
    public enum TUTOR_FOCUS_TYPE : int
    {
        CIRCLE = 0,
        SQUARE
    };
    public enum TUTOR_NEXT_TYPE : int
    {
        NONE = 0,
        CONTINUE,
        AUTO
    };

    // Server Consttant
    public const int USER_STATUS_RESUME = 0;     // 유저 상태 RESUME
    public const int USER_STATUS_PAUSE = 1;      // 유저 상태 PAUSE
    public const int USER_STATUS_EXIT = -1;      // 유저 상태 EXIT

    //스파인 관련
    public const int SPINE_TYPE_BATTLE = 0;
    public const int SPINE_TYPE_INFO_UI = 1;
    public const int SPINE_TYPE_CARD = 2;
    public const int SPINE_TYPE_LOBBYCARD_BASE = 3;
    public const int SPINE_TYPE_LOBBYCARD_SELECT = 4;
    public enum SPINE_EXPLAIN
    {
        Top = 0,
        Bottom,
        Left,
        Right,
        Center,
        Max
    }
}
