using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ApiResultCode : int
{
    NO_DB_DATA = -9005,     
    WEBAPI_ERROR = -9004,   
    PACKET_ERROR = -9003,   
    TIMEOUT_ERROR = -9002,  
    SOCKET_ERROR = -9001,   
    DB_ERROR = -9000,       
    FAIL_DISCOMP = -4000,   
    LACK_TICKET = -3105,    
    NO_REWARD = -3104,      
    MAX_TIER = -3103,       
    MAX_LEVEL = -3102,      
    LACK_ITEMS = -3101,     
    LACK_GOODS = -3100,     
    TOP_LEVELUP = -2200,    
    PARAM_ERROR = -2100,    
    COUNT_NICK = -1004,     
    SPACE_NICK = -1003,     
    EMPTY_NICK = -1002,     
    BAD_NICK = -1001,       
    DUP_NICK = -1000,       
    FAIL = -1,              
    NONE = 0,               
    SUCCESS,                
    SUCCESS_ENCHANT,        
    FAIL_ENCHANT,           
    BIG_FAIL_ENCHANT,       
    FAIL_MAX_ENCHANT,       
}
