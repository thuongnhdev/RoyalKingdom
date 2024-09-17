using System.Threading.Tasks;
using BestHTTP.WebSocket;
using Fbs;
using Google.FlatBuffers;
using UnityEngine;
using CoreData.UniFlow.Common;
public class PacketManager : Singleton<PacketManager>
{
    //public FBManager FbManager;
    
    private WebSocket WebSocket_Lobby;
    private WebSocket WebSocket_WorldMap;
    private WebSocket WebSocket_War;
    
    private CommonMsg BaseCommonMsg;

    public delegate void ReceiveSocketMessageDel(int messageType, byte[] data);
    public static event ReceiveSocketMessageDel OnReceivedSocketMessage;

    [SerializeField]
    private UserProfile userProfile;
    [SerializeField]
    private TableWorldMapPlayer tableWorldMapPlayers;
    [SerializeField]
    private WorldMapTroopService troopService;
    [SerializeField]
    private LandAndKingdomService landAndKingdom;
    [SerializeField]
    private NetworkEnvironment _environment;
    
    public void ForceCloseSocket_Lobby()
    {
        if (WebSocket_Lobby != null)
        {
            WebSocket_Lobby.Close();
            WebSocket_Lobby = null;
        }
    }
    public void ForceCloseSocket_WorldMap()
    {
        if (WebSocket_WorldMap != null)
        {
            WebSocket_WorldMap.Close();
            WebSocket_WorldMap = null;
        }
    }

    public void ForceCloseSocket_War()
    {
        if (WebSocket_War != null)
        {
            WebSocket_War.Close();
            WebSocket_War = null;
        }
    }

    public /*async*/ void InitWebSocket_Lobby()
    {
        if (StatesGlobal.UID_PLAYER <= 0) return;

        //await Task.Delay(10000);
        Debug.Log("[EVENT]:InitWebSocket_Lobby [" + StatesGlobal.UID_PLAYER + "][" + _environment.GetSocketHostAndPort() + "]");
        // 전투서버 통신 서비스 초기화 
        WebSocket_Lobby = new WebSocket(new System.Uri(_environment.GetSocketHostAndPort()));
        WebSocket_Lobby.StartPingThread = false;

        WebSocket_Lobby.OnOpen += WebOpen_Lobby;
        WebSocket_Lobby.OnBinary += WebMessage_Lobby;
        //WebSocket_Lobby.OnMessage += WebMessage_Test;
        WebSocket_Lobby.OnClosed += WebClose_Lobby;
        WebSocket_Lobby.OnError += WebError_Lobby;

        WebSocket_Lobby.Open();
    }

    private void WebOpen_Lobby(WebSocket ws)
    {
        Debug.Log("[EVENT]:WebOpen_Lobby [" + ws.State + "][" + StatesGlobal.UID_PLAYER + "]");
        SendData_Lobby_Common(Fb.PacketCode.LOGIN, "", 0);
    }
    
    private void WebError_Lobby(WebSocket ws, System.Exception ex)
    {
        if (ex == null || WebSocket_Lobby == null)
            return;

        Debug.Log("[EVENT]:WebSocketError_Lobby [" + ex.Message + "]");
        Debug.Log(ws.State);

        // if (DontDestroyBase.Instance != null)
        // {
        //     DontDestroyBase.Instance.WebSocketErrorPopup(Fbs.CommonResult.SOCKET_ERROR.ToString(), true);
        // }
    }
    
    private void WebClose_Lobby(WebSocket ws, System.UInt16 code, string message)
    {
        if (WebSocket_Lobby == null)
            return;

        Debug.Log("[EVENT]:WebSocketClose_Lobby [" + WebSocket_Lobby.State + "][" + message + "]");
        Debug.Log(ws.State);

        // if (DontDestroyBase.Instance != null)
        // {
        //     DontDestroyBase.Instance.WebSocketErrorPopup(Fbs.CommonResult.SOCKET_ERROR.ToString(), true);
        // }
    }

    private void WebMessage_Test(WebSocket ws, string Message)
    {
        Debug.Log("Message --- "+Message);
    }
    private void WebMessage_Lobby(WebSocket ws, byte[] bytes)
    {
        
        Debug.Log("MESSAGE LOBBY RECEIVE SERVER ---- "+bytes[0]);
        Packet<Object> pkt = new Packet<Object>(bytes[0], getBody(bytes));
        ByteBuffer bb = new ByteBuffer(pkt.getBody());
        int type = pkt.getType();
        if (type != Fb.WorldMap.BATTLE_END)
            OnReceivedSocketMessage?.Invoke(type, pkt.getBody());
        switch (type)
        {   
            case Fb.WorldMap.UPDATE_BATTLE:
                UpdateBattle dataUpdateBattle = UpdateBattle.GetRootAsUpdateBattle(bb);
                MilitaryManager.Instance.UpdateBattle(dataUpdateBattle);
                break;
            case Fb.WorldMap.BATTLE_END:
                EndBattel dataEndBattle = EndBattel.GetRootAsEndBattel(bb);
                EndBattleModel dataEnd = new EndBattleModel();
                dataEnd.IdLandUserLose = dataEndBattle.IdLandUserLose;
                dataEnd.IdLandUserWin = dataEndBattle.IdLandUserWin;
                dataEnd.UidPlayerLose = dataEndBattle.UidPlayerLose;
                dataEnd.UidPlayerWin = dataEndBattle.UidPlayerWin;
                MilitaryManager.Instance.OnStopCourotineEndBattle();
                MilitaryManager.Instance.EndBattle(dataEnd);
                break;
            case Fb.WorldMap.BATTLE_START:
                {
                    StartBattleWar respone =
                        StartBattleWar.GetRootAsStartBattleWar(bb);

                    WorldMapTroop current;
                    var attacker = respone.TroopPlayerAttack.Value;
                    var victim = respone.TroopPlayerBeAttack.Value;
                    var land = respone.LandBeAttack.Value;

                    current = troopService.Get(attacker.IdTroop);
                    if (current != null) {
                        current.Value_Archer = attacker.ValueArcher;
                        current.Value_Cavalry = attacker.ValueCavalry;
                        current.Value_Infantry = attacker.ValueInfantry;
                        current.AttackTroop = victim.IdTroop;
                        current.AttackLand = land.IdLand;
                        troopService.Save(current);
                    }

                    if (victim.IdTroop > 0)
                    {
                        current = troopService.Get(victim.IdTroop);
                        if (current != null)
                        {
                            current.Value_Archer = victim.ValueArcher;
                            current.Value_Cavalry = victim.ValueCavalry;
                            current.Value_Infantry = victim.ValueInfantry;
                            current.AttackTroop = attacker.IdTroop;
                            current.AttackLand = 0;
                            troopService.Save(current);
                        }

                    }


                    if (land.IdLand > 0) {
                        landAndKingdom.UpdateDynamicLand(land.IdLand, land.Hp, land.IdLandParent);
                    }


                    //troopService.SyncAttackLandData(respone.LandBeAttack.Value);

                    //int lengh = dataRequestStartBattleWar.PlayerStartBattleLength;
                    //long[] players = new long[lengh];
                    /*
                    for (int i = 0; i < lengh; i++)
                    {
                        var player = dataRequestStartBattleWar.PlayerStartBattle(i);
                        players[i] = player.Value.Uid;
                        Debug.Log("PLAYER IN WAR --- "+players[i]);
                    }
                    //MilitaryManager.Instance.StartBattle(players);
                    */
                }
                break;
            case Fb.WorldMap.PLAYER_LEAVE:
                RemovePlayerWorldMap dataRemoveUpdatePlayerInWorldMap = RemovePlayerWorldMap.GetRootAsRemovePlayerWorldMap(bb);
                MilitaryManager.Instance.PlayerLeaveWorldMap(dataRemoveUpdatePlayerInWorldMap.PlayerWorldMap.Value.Uid);
                break;
            case Fb.WorldMap.PLAYER_JOIN_MAP: // Player joinWorldMap
                {
                    playerJoinMap dataPlayerJoinMap = playerJoinMap.GetRootAsplayerJoinMap(bb);
                    long uidPlayerJoin = dataPlayerJoinMap.Uid;
                    string playerName = dataPlayerJoinMap.PlayerName;
                    int possition = dataPlayerJoinMap.Possition;
                    int idLand = dataPlayerJoinMap.IdLand;
                    int status = dataPlayerJoinMap.Status;
                    int militaryCount = dataPlayerJoinMap.MilitaryCount;
                    int typeMilitary = dataPlayerJoinMap.TypeMilitary;

                    WorldMapPlayer player = new WorldMapPlayer()
                    {
                        Uid = dataPlayerJoinMap.Uid,
                        IdLand = dataPlayerJoinMap.IdLand,
                        PlayerName = dataPlayerJoinMap.PlayerName,
                        Position = dataPlayerJoinMap.Possition,
                        Status = dataPlayerJoinMap.Status,
                    };

                    long[] troops = new long[dataPlayerJoinMap.TroopsInfoLength];
                    for (int i = 0; i < troops.Length; i++)
                    {
                        var troopData = dataPlayerJoinMap.TroopsInfo(i).Value;
                        WorldMapTroop troop = new WorldMapTroop()
                        {
                            IdTroop = troopData.IdTroop,
                            Vassal_1 = troopData.Vassal1,
                            Vassal_2 = troopData.Vassal2,
                            Vassal_3 = troopData.Vassal3,

                            Buff_1 = troopData.Buff1,
                            Buff_2 = troopData.Buff2,
                            Buff_3 = troopData.Buff3,
                            Buff_4 = troopData.Buff4,

                            Attack_1 = troopData.Attack1,
                            Attack_2 = troopData.Attack2,
                            Attack_3 = troopData.Attack3,
                            Attack_4 = troopData.Attack4,
                            Attack_5 = troopData.Attack5,

                            Value_Archer = troopData.ValueArcher,
                            Value_Cavalry = troopData.ValueCavalry,
                            Value_Infantry = troopData.ValueInfantry,

                            Pawns = troopData.Pawn,
                            Status = troopData.Status,
                            IdType = troopData.IdType,

                            idPlayer = player.Uid,
                            Position = troopData.Possition,
                            TargetPosition = troopData.TargetCity,
                            StartTime = troopData.TimeStartMove
                        };
                        troops[i] = troop.IdTroop;
                        troopService.Save(troop);
                    }
                    player.Troops = troops;
                    tableWorldMapPlayers.Set(player);

                    /*
                    for (int i = 0; i < dataPlayerJoinMap.MilitaryLength; i++)
                    {
                        //int typeMilitary = dataPlayerJoinMap.Military(i).Value.TypeMilitary;
                        int Count = dataPlayerJoinMap.Military(i).Value.Count;
                    }
                    Debug.Log("PLAYER JOIN MAPPP ---- " + uidPlayerJoin + " playerName " + playerName + " possition " + possition + " militaryCount " + militaryCount);
                    if (militaryCount > 0)
                    {

                    }
                    */
                    //MilitaryManager.Instance.PlayerJoinMap(dataPlayerJoinMap);
                    break;
                }
            case Fb.WorldMap.PLAYER_MOVE: // Player move in worldmap
                playerMove dataPlayerMove = playerMove.GetRootAsplayerMove(bb);
                long uidPlayerMove = dataPlayerMove.Uid;
                int count = dataPlayerMove.IdTroopsLength;
                int target = dataPlayerMove.IdCityTarget;
                for (int i = 0; i < count; i++)
                {
                    int id = dataPlayerMove.IdTroops(i);
                    dataPlayerMove.TimeStart(i);
                    var troop = troopService.Get(id);
                    troop.TargetPosition = target;
                    troop.StartTime = dataPlayerMove.TimeStart(i);
                    troopService.Save(troop);
                }



                //int[] CityCross = new int[dataPlayerMove.CityCrossLength];
                //for (int i = 0; i < CityCross.Length; i++)
                //{
                //    CityCross[i] = dataPlayerMove.CityCross(i);
                //}

                //MilitaryManager.Instance.MoveMilitaryAllUser(uidPlayerMove, CityCross);
                Debug.Log("PLAYER MOVE MAPPP ---- "+uidPlayerMove);
                break;
            case Fb.PacketType.HEART_BEAT:
                Debug.Log("[RESPONSE_BASE]::WebMessage_Lobby ::HEART_BEAT");
                break;
            case Fb.PacketType.COMMON_MSG:
                BaseCommonMsg = CommonMsg.GetRootAsCommonMsg(bb);
                Debug.Log("Base Common msg ----- "+BaseCommonMsg.Uid);
                break;
        }
    }
    
     public void SendData_Lobby_Common(int packet_type, string content, Fbs.CommonResult resultCode) // send data to server by websocket
    {
        if (packet_type != Fb.PacketCode.CLIENT_PONG)
        {
            Debug.Log("[REQUEST]::SendData_Control_Common::[" + LogString.GetFb_PacketType(packet_type) + "][" + StatesGlobal.UID_PLAYER + "][" + content + "][" + resultCode + "]");
        }

        FlatBufferBuilder fbb = new FlatBufferBuilder(1);
        switch (packet_type)
        {
            case Fb.PacketCode.LOGIN:
                CreateFb_CommonMsg_Empty(ref fbb, packet_type);
                ApiLoginResult result = ApiLoginResult.GetRootAsApiLoginResult(fbb.DataBuffer);
                SendMessage(fbb, Fb.PacketType.COMMON_MSG);
                break;

        }
    }
    void CreateFb_CommonMsg_Empty(ref FlatBufferBuilder fbb, int packetCode)
    {
        Offset<CommonMsg> Msg = CommonMsg.CreateCommonMsg(fbb, packetCode, 0, StatesGlobal.UID_PLAYER, fbb.CreateString(""), 0
        ,userProfile.landId, userProfile.kingdomId, userProfile.CountMilitary, userProfile.MilitaryType);
        fbb.Finish(Msg.Value);              
    }

    public void InitWebSocket_WorldMap()
    {
        
    }

    public void InitWebSocket_War()
    {
        
    }
    
    private void SendMessage(FlatBufferBuilder fbb, int type, bool isLobby)
    {
        byte[] header = new byte[1] { (byte)type };
        byte[] body = fbb.SizedByteArray();
        byte[] bytes = new byte[header.Length + body.Length];
        header.CopyTo(bytes, 0);                // 헤더 카피
        body.CopyTo(bytes, header.Length);      // 바디 카피

        try
        {
            if (isLobby)
            {
                SendWebSocketMessage_Lobby(bytes);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("<color=red>Socket send or receive error !</color> : " + e.ToString());
        }
    }
    
    private void SendMessage(FlatBufferBuilder fbb, int type)
    {
        byte[] header = new byte[1] { (byte)type };
        byte[] body = fbb.SizedByteArray();
        byte[] bytes = new byte[header.Length + body.Length];
        header.CopyTo(bytes, 0);                // 헤더 카피
        body.CopyTo(bytes, header.Length);      // 바디 카피

        try
        {
            switch (type)
            {
                case Fb.PacketType.COMMON_MSG:
                    SendWebSocketMessage_Lobby(bytes);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Socket send or receive error ! : " + e.ToString());
        }
    }
    
    public void SendWebSocketMessage_Lobby(byte[] packet)
    {
        if (WebSocket_Lobby != null)
        {
            if (WebSocket_Lobby.State == WebSocketStates.Open)
            {
                WebSocket_Lobby.Send(packet);
            }
            else
            {
                Debug.LogWarning("Websocket_Lobby State : " + WebSocket_Lobby.State);
            }
        }
        else
        {
            Debug.LogWarning("Websocket_Lobby NULL!!!");
        }
    }
    
    private byte[] getBody(byte[] origin)
    {
        byte[] body = new byte[origin.Length - 1];
        System.Array.Copy(origin, 1, body, 0, body.Length);
        return body;
    }
    
    
}
