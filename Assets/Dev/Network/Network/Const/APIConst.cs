using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIConst 
{
    public enum eType
    {
        None,
        BattleEnd
    }
    public enum TYPE
    {
        NONE = -1,
        LOGIN = 0,      //로그인, 동기
        UPDATE_HERO_DECK_INFO,  //영웅덱정보 설정, 동기
        SET_ACTIVE_BATTLE_DECK, //전투덱정보 설정, 비동기
        GET_FRIENDS,        // 친구 목록 가져오기, 동기
        GET_TEAM_DECK_INFO, //팀덱정보가져오기, 사용 안함
        GET_DECK_INFO,      //엑티브 덱 정보가져오기, 동기
        UPDATE_NICKNAME,    //유저닉네임변경, 동기
        GET_TUTORIAL_DECKS, //튜토리얼덱정보가져오기, 동기
        SET_TUTORIAL_DECK,  //튜토리얼 덱 설정, 동기
        GET_TUTORIAL_INDEX_INFO,    //튜토리얼 히스토리 인덱스 정보 가져오기, 사용안함
        SET_TUTORIAL_INDEX, //튜토리얼 히스토리 인덱스 설정하기, ??? 동기/비동기
        GET_HERO_INFO,      //유저영웅정보가져오기(현재 튜토리얼 이후 사용), 동기
        UPDATE_GOODS,       //재화 및 영웅샤드 추가하기, 디버그, 동기
        GET_PLAYER_INFO,    //로그인 정보와 동일한 플레이어 정보, 동기
        LEVELUP_HERO,       //영웅 레벨 업그레이드 하기, 동기
        BUY_CARD_PACK_1,    //카드팩 구매하기 (1개), 동기
        BUY_CARD_PACK_10,   //카드팩 구매하기 (10개), 동기
        GET_GOODS_INFO,      //재화 정보 가져오기, 비동기
        GET_OPEN_BATTLE_CONTENT,   //유저 레벨업시 오픈되는 컨텐츠 정보, 동기
        GET_MATCH_PVE_INFO,
        GET_QUEST_MANAGE_ALL,
        GET_GAME_CLIENT_DATA,
        GET_BATTLELOG_INFO,
        GET_MAIL_LIST,
        GET_DAILYQUEST,
        GET_SEASONQUEST,
        GET_CHAMPIONREWARD,
        GET_SPECIALQUEST,
        GET_ACHIMENTQUEST,
        REWARD_ACHIMENTQUEST,
        REWARD_DailyQUEST,
        REWARD_SEASONQUEST,
        REWARD_CHAMPIONPASS,
        REWARD_SPEICALQUEST,
        GET_QUESTALL,
        GET_SEASONINFO,
        GET_QuestStatusAll,
        GET_MAIL_REWARD,
        BATTLE_FINISH,

        SOCKET_MATCHING = 1000
    }
    
    public static TYPE[] TypeBase = new TYPE[] {  };
    public static TYPE[] TypeAsync = new TYPE[] { TYPE.SET_ACTIVE_BATTLE_DECK, TYPE.SET_TUTORIAL_INDEX, TYPE.GET_GOODS_INFO, TYPE.GET_OPEN_BATTLE_CONTENT, TYPE.GET_MATCH_PVE_INFO};   
}
