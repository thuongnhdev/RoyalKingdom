using System.Collections;
using System.Collections.Generic;
using Fbs;
using UnityEngine;

public static class Fb 
{
   public static class PacketType
   {
      public const int HEART_BEAT = (int)Fbs.PacketType.HEART_BEAT;
      public const int COMMON_MSG = (int)Fbs.PacketType.COMMON_MSG;
   }

   public static class WorldMap
   {
      public const int PLAYER_JOIN_MAP = (int)Fbs.WorldMap.PLAYER_JOIN_MAP;
      public const int PLAYER_MOVE = (int)Fbs.WorldMap.PLAYER_MOVE;
      public const int PLAYER_LEAVE = (int)Fbs.WorldMap.PLAYER_LEAVE;
      public const int BATTLE_START = (int)Fbs.WorldMap.BATTLE_START;
      public const int BATTLE_END = (int)Fbs.WorldMap.BATTLE_END;
      public const int UPDATE_BATTLE = (int)Fbs.WorldMap.UPDATE_BATTLE;
   }

   public static class PacketCode
   {
      // COMMON Message (PacketType : COMMON_MSG)
      public const int CLIENT_PING = (int)Fbs.PacketCode.CLIENT_PING;
      public const int CLIENT_PONG = (int)Fbs.PacketCode.CLIENT_PONG;        
      public const int SERVER_PING = (int)Fbs.PacketCode.SERVER_PING;
      public const int SERVER_PONG = (int)Fbs.PacketCode.SERVER_PONG;
      
      
      public const int LOGIN = (int)Fbs.PacketCode.LOGIN;
   }
}
