using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Common.Static
{
    public static class StatesGlobal
    {
        public static string TODO_JIN;

        public static bool DEBUG_MODE = false;

        public static bool IS_TITLE_START = false;


        public static int OPT_LANGUAGE = ConstLanguage._KO;  //로컬



        public static bool DEBUG_MASTER = false;
        public static bool DEBUG_NO_NETWORK = false;

        public static bool DEBUG_IGNORE_LOADING = false;
        public static bool DEBUG_IGNORE_TUTORIAL = false;

        //public static bool DEBUG_CAM_DIRECTOR_OFF = false;

        public static bool DEBUG_LOG_ACT_ORDER = true;
        public static bool DEBUG_SHOW_STATUS_HideUI = false;

        public static SERVER_TYPE TYPE_SERVER = SERVER_TYPE.REMOTE;
        public static ACCESS_TYPE TYPE_ACCESS = ACCESS_TYPE.DEV;

        public static bool LIVE_MODE = false;
        public static bool QA_MODE = false;

        public static bool IS_LOCAL_SERVER = false;

        public static bool IS_INIT_LOGIN = false;

        public enum SERVER_TYPE
        {
            LOCAL = 0,
            REMOTE = 1
        }

        public enum ACCESS_TYPE
        {
            DEV = 0,
            PLAN = 1
        }

        public enum STATUS_BATTLE
        {
            EMPTY = 0,
            BATTLE = 1
        }


        public static string SESSION_FOR_LOG = "";

        public static string DUID = "";
        public static string LOGIN_ID;

        public static long UID_PLAYER = GlobalConst.UID_UNKNOWN;

        public static string NICK_NAME_PLAYER = "";
        public static int USER_REP_ICON = 0;
        public static int USER_LV;
        public static int ARENA_TIER;
        public static string USER_GUILD_NAME = "";

        public static long UID_WATCH;

        private static int ChapStageIdx = GlobalConst.UNKNOWN;
        private static int ChapterIdx = GlobalConst.UNKNOWN;
        private static int StageIdx = GlobalConst.UNKNOWN;


        public static bool CONTINUE_BATTLE = false;

        // public static int OPT_LANGUAGE = LanguageConst.KO;  //로컬
        public static int OPT_GAME_QUALITY = GlobalConst.QUALITY_HIGH;          //로컬
        public static bool OPT_BGM = true;                  //로컬
        public static bool OPT_EFFECT = true;               //로컬    
        public static bool OPT_EFFECT_UI = true;               //로컬    
        public static bool OPT_EFFECT_BATTLE = true;               //로컬    
        public static bool OPT_VOICE = true;               //로컬    

        public static float OPT_BGM_VOLUME = 1f;                  //로컬
        public static float OPT_EFFECT_VOLUME = 1f;               //로컬      
        public static float OPT_VOICE_VOLUME = 1f;               //로컬      

        public static bool NEXT_CHAPTER = false;

        public static long ROOT_MINING_UID = -1;
        public static long ROOT_MINING_INDEX = -1;
        public static long ROOT_MINING_UIDX = -1;



        //챕터 변경 여부를 반환(챕터가 변경되면 true)
        // public static bool SetChapStageIdx(int iChapterStageIdx)
        // {
        //     Debug.Log("SetChapStageIdx : " + iChapterStageIdx);
        //     if (iChapterStageIdx > 0)
        //     {
        //         int beforeChapter = ChapterIdx;
        //
        //         ChapStageIdx = iChapterStageIdx;
        //         ChapterIdx = GetChapterIdx(iChapterStageIdx);
        //         StageIdx = GetStageIdx(iChapterStageIdx);
        //         return (iChapterStageIdx > ClientDataTable.Instance.GetClientData<Client_DataStageInfo>().param[0].Idx && beforeChapter != ChapterIdx);
        //     }
        //     else
        //     {
        //         Debug.LogError("ChapterStageIdx = " + iChapterStageIdx);
        //         return false;
        //     }
        // }

        public static int GetChapStageIdx()
        {
            return ChapStageIdx;
        }

        public static int GetChapterIdx()
        {
            return ChapterIdx;
        }

        public static int GetStageIdx()
        {
            return StageIdx;
        }

        public static int GetChapStageIdx(int iChapter, int iStage)
        {
            return 100000 + iChapter * 100 + iStage;
        }

        public static int GetChapterIdx(int iChapterStageIdx)
        {
            return iChapterStageIdx % 100000 / 100;
        }

        public static int GetStageIdx(int iChapterStageIdx)
        {
            return iChapterStageIdx % 100;
        }
    }
}