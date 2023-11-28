/// <summary>
/// World - Square 에서 사용할 Mirror Network Manager class
/// @author         : HJ Lee
/// @last update    : 2023. 11. 28
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 11. 21) : Mirror.Examples.MultipleMatch 참고해서 수정. -> WorldController 로 분리.
///     v0.3 (2023. 11. 28) : WorldController 내용 수정해서 다시 적용.
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Joycollab.v2
{
    public class WorldNetworkManager : NetworkManager
    {
        private const string TAG = "WorldNetworkManager";
        public static new WorldNetworkManager singleton { get; private set; }


    #region avatar info, room info

    /**
        playerMatches 		=> avatarMatches
        openMatches 		=> roomInfos
        matchConnections 	=> connectionsInRoom
        playerInfos 		=> avatarInfos
        waitingConnections 	=> connectionsInCenter
        waitingInfos 		=> avatarsInCenter
    */

        /// <summary>
        /// 이 항목은 확인이 필요함. (for Cross-reference) 
        /// </summary>
        internal static readonly Dictionary<NetworkConnectionToClient, Guid> avatarMatches = new Dictionary<NetworkConnectionToClient, Guid>();

        /// <summary>
        /// 모임방 리스트
        /// </summary>
        internal static readonly Dictionary<Guid, ClasInfo> roomInfos = new Dictionary<Guid, ClasInfo>();

        /// <summary>
        /// 특정 모임방에 들어가 있는 Connection 리스트
        /// </summary>
        internal static readonly Dictionary<Guid, HashSet<NetworkConnectionToClient>> connectionsInRoom = new Dictionary<Guid, HashSet<NetworkConnectionToClient>>();

        /// <summary>
        /// Network Connection 되어 있는 World Avatar Info 
        /// </summary>
        internal static readonly Dictionary<NetworkConnection, WorldAvatarInfo> avatarInfos = new Dictionary<NetworkConnection, WorldAvatarInfo>();

        /// <summary>
        /// Center 에 있는 Network Connection 리스트
        /// </summary>
        internal static readonly List<NetworkConnectionToClient> connectionsInCenter = new List<NetworkConnectionToClient>();

        /// <summary>
        /// 커뮤니티 센터에 있는 사용자 리스트
        /// </summary>
        internal static readonly Dictionary<int, WorldAvatarInfo> avatarsInCenter = new Dictionary<int, WorldAvatarInfo>(); 
        public int avatarCountInCenter = avatarsInCenter.Count;
        public WorldAvatarInfo[] GetAvatarListInCneter() 
        {
            var infos = from pair in avatarsInCenter
                orderby pair.Value.nickNm ascending
                select pair; 

            var list = new List<WorldAvatarInfo>();   
            list.Clear();
            foreach (var pair in infos) 
            {
                list.Add(pair.Value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 특정 모임방에 들어가 있는 사용자 리스트
        /// </summary>
        internal static readonly Dictionary<int, WorldAvatarInfo> avatarsInRoom = new Dictionary<int, WorldAvatarInfo>();
        public int avatarCountInRoom = avatarsInRoom.Count;
        public WorldAvatarInfo[] GetAvatarListInRoom() 
        {
            var infos = from pair in avatarsInRoom
                orderby pair.Value.nickNm ascending
                select pair; 

            var list = new List<WorldAvatarInfo>();   
            list.Clear();
            foreach (var pair in infos) 
            {
                list.Add(pair.Value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// GUID of a match the local player has created
        /// </summary>
        internal Guid selectRoomId = Guid.Empty;

    #endregion  // avatar info, room info


    #region override functions

        public override void Awake() 
        {
            base.Awake();
            singleton = this;

            avatarMatches.Clear();
            roomInfos.Clear();
            connectionsInRoom.Clear();
            avatarInfos.Clear();
            connectionsInCenter.Clear();
            avatarsInCenter.Clear();
            avatarsInRoom.Clear();
        }

    #endregion  // override functions


    #region Server callbacks

        // called on the server when a client is ready.
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            Debug.Log($"{TAG} | server ready.");
            base.OnServerReady(conn);

            DoServerReady(conn);
        }

        // called on the server when a client disconnects.
        public override void OnServerDisconnect(NetworkConnectionToClient conn) 
        {
            if (conn.authenticationData != null) 
            {
                Debug.Log($"{TAG} | OnServerDisconnect(), 정보 삭제.");
            }

            WorldChatView.playerNames.Remove(conn);
            StartCoroutine(DoServerDisconnect(conn));
        }

        private IEnumerator DoServerDisconnect(NetworkConnectionToClient conn) 
        {
            yield return ServerDisconnectProcess(conn);
            base.OnServerDisconnect(conn);
        }

    #endregion  // Server callbacks


    #region Client callbacks

        public override void OnClientConnect()
        {
            Debug.Log($"{TAG} | connect to server.");
            base.OnClientConnect();

            DoClientConnect();
        }

        public override void OnClientDisconnect() 
        {
            Debug.Log($"{TAG} | disconnect from server.");

            DoClientDisconnect();
            base.OnClientDisconnect();
        }

    #endregion  // Client callbacks
    

    #region start & stop callbacks

        public override void OnStartServer() 
        {
            Debug.Log($"{TAG} | server started.");
            base.OnStartServer();

            DoStartServer();
        }

        public override void OnStopServer() 
        {
            Debug.Log($"{TAG} | server stoped.");
            base.OnStopServer();

            DoStopServer();
        }

        public override void OnStartClient()
        {
            Debug.Log($"{TAG} | client started.");

            DoStartClient();
        }

        public override void OnStopClient() 
        {
            Debug.Log($"{TAG} | client stoped.");

            DoClientDisconnect();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn) 
        {
            Debug.Log($"{TAG} | OnServerAddPlayer() | conn : {conn}");

            Transform start = GetStartPosition();
            Debug.Log($"{TAG} | OnServerAddPlayer() | transform : {start.position}");

            GameObject player = (start == null) ? 
                Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) :
                Instantiate(playerPrefab, start); 

            player.transform.SetParent(null);
            NetworkServer.AddPlayerForConnection(conn, player);
            WorldAvatar script = player.GetComponent<WorldAvatar>();
            WorldAvatarInfo info = (WorldAvatarInfo) conn.authenticationData;
            script.UpdateAvatarInfo(info);

            avatarInfos.Add(conn, info);
            avatarsInCenter.Add(info.seq, info);
            SendRoomList();
            SendUserList();
        }

    #endregion  // start & stop callbacks


    #region UI functions

        // Called from several places to ensure a clean reset
        //  - WorldNetworkManager.Awake
        //  - OnStartServer
        //  - OnStartClient
        //  - OnClientDisconnect
        //  - ResetSpace
        internal void InitializeData() 
        {
            avatarMatches.Clear();
            roomInfos.Clear();
            connectionsInRoom.Clear();
            connectionsInCenter.Clear();

            selectRoomId = Guid.Empty;
        }

        void ResetSpace() 
        {
            InitializeData();

            // goCenter.SetActive(true);
            // goRoom.SetActive(false);
        }

    #endregion  // UI functions


    #region Request functions

        /// <summary>
        /// 새 모임방 생성
        /// </summary>
        [ClientCallback]
        public void RequestCreateRoom(int seq) 
        {
            string id = $"room_{seq}";
            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.CreateRoom, roomId = new Guid(id) });
        }

        /// <summary>
        /// 기존 모임방에 가입
        /// </summary>
        [ClientCallback]
        public void RequestJoinRoom() 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{TAG} | RequestJoinRoom(), select room id == empty. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.JoinRoom, roomId = selectRoomId });
        }

        /// <summary>
        /// 기존 모임방에서 나옴 (탈퇴 아님)
        /// </summary>
        [ClientCallback]
        public void RequestLeaveRoom() 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{TAG} | RequestLeaveRoom(), select room id == empty. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.LeaveRoom, roomId = selectRoomId });
        }

        /// <summary>
        /// 기존 모임방에서 탈퇴
        /// </summary>
        [ClientCallback]
        public void RequestDeleteRoom(Guid id) 
        {
            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.DeleteRoom, roomId = id });
        }

    #endregion  // Request functions


    #region Server & Client callbacks

        private void DoStartServer() 
        {
            InitializeData();
            NetworkServer.RegisterHandler<ServerMessage>(OnServerMessage); 
        }

        private void DoServerReady(NetworkConnectionToClient conn) 
        {
            connectionsInCenter.Add(conn);

            /**
            if (! avatarsInCenter.ContainsKey(WorldAvatar.localPlayerInfo.seq))
            {
                avatarsInCenter.Add(WorldAvatar.localPlayerInfo.seq, WorldAvatar.localPlayerInfo);
            }

            SendRoomList();
            SendUserList();
             */
        }

        private IEnumerator ServerDisconnectProcess(NetworkConnectionToClient conn) 
        {
            Guid roomId;
            if (avatarMatches.TryGetValue(conn, out roomId)) 
            {
                avatarMatches.Remove(conn);
                // roomInfos.Remove(roomId);

                foreach (var playerConn in connectionsInRoom[roomId]) 
                {
                    playerConn.Send(new ClientMessage { clientOperation = ClientOperation.RoomDeparted });
                }
            }

            foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> kvp in connectionsInRoom)
                kvp.Value.Remove(conn);

            WorldAvatarInfo avatarInfo = avatarInfos[conn];
            if (avatarInfo != null)
            {
                if (avatarInfo.roomId != Guid.Empty) 
                {
                    ClasInfo clasInfo;
                    if (roomInfos.TryGetValue(avatarInfo.roomId, out clasInfo))
                    {
                        // ...
                        Debug.Log($"{TAG} | OnServerDisconnect(), 원래는 여기에서 player 수를 줄이고 match 목록을 정리함.");
                    }

                    HashSet<NetworkConnectionToClient> connections;
                    if (connectionsInRoom.TryGetValue(avatarInfo.roomId, out connections)) 
                    {
                        WorldAvatarInfo[] infos = connections.Select(playerConn => avatarInfos[playerConn]).ToArray();

                        foreach (var playerConn in connectionsInRoom[avatarInfo.roomId]) 
                        {
                            if (playerConn != conn) 
                                playerConn.Send(new ClientMessage { clientOperation = ClientOperation.UpdateRoom, avatarsInRoom = infos });
                        }

                        foreach (var avatar in infos) 
                        {
                            if (avatarsInCenter.ContainsKey(avatar.seq))
                                avatarsInCenter.Remove(avatar.seq);
                        }
                    }
                }
                else 
                {
                    if (avatarsInCenter.ContainsKey(avatarInfo.seq)) 
                        avatarsInCenter.Remove(avatarInfo.seq);
                }
            }

            SendRoomList();
            SendUserList();

            yield return null;
        }

        private void DoStopServer() 
        {
            ResetSpace();
        }

        private void DoClientConnect() 
        {
            Debug.Log($"{TAG} | DoClientConnect(), seq : {WorldAvatar.localPlayerInfo.seq}, name : {WorldAvatar.localPlayerInfo.nickNm}");
        }

        private void DoStartClient() 
        {
            InitializeData();

            NetworkClient.RegisterHandler<ClientMessage>(OnClientMessage);
        }

        private void DoClientDisconnect() 
        {
            InitializeData();
        }

        private void DoStopClient() 
        {
            ResetSpace();
        }

    #endregion  // Server & Client callbacks


    #region Server message handler

        private void OnServerMessage(NetworkConnectionToClient conn, ServerMessage msg) 
        {
            Debug.Log($"{TAG} | OnServerMessage(), msg : {msg.serverOperation}");
            switch (msg.serverOperation) 
            {
                case ServerOperation.None :
                    Debug.LogWarning($"{TAG} | OnServerMessage(), Missing ServerMatchOperation");
                    break;

                case ServerOperation.CreateRoom :
                    OnServerCreateRoom(conn, msg.roomId);
                    break;

                case ServerOperation.JoinRoom :
                    OnServerJoinRoom(conn, msg.roomId);
                    break;

                case ServerOperation.LeaveRoom :
                    OnServerLeaveRoom(conn, msg.roomId);
                    break;

                case ServerOperation.DeleteRoom :
                    OnServerDeleteRoom(conn, msg.roomId);
                    break;

                case ServerOperation.RemoveFromRoom :
                    OnServerRemoveFromRoom(conn, msg.roomId);
                    break;
            }
        }

        [ServerCallback]
        private void OnServerCreateRoom(NetworkConnectionToClient conn, Guid id) 
        {
            Debug.Log($"{TAG} | Missing ServerMatchOperation");
            connectionsInRoom.Add(id, new HashSet<NetworkConnectionToClient>());

            // TODO. 이 부분은 모임방 생성 UI 에서 추가할 예정.
            // roomInfos.Add(id, new ClasInfo());

            conn.Send(new ClientMessage { clientOperation = ClientOperation.RoomCreated, 
                roomId = id, 
                roomList = roomInfos.Values.ToArray(), 
                avatarsInRoom = new List<WorldAvatarInfo>().ToArray(),
                avatarsInCenter = avatarsInCenter.Values.ToArray(),
            });
            SendRoomList();
        }

        [ServerCallback]
        private void OnServerJoinRoom(NetworkConnectionToClient conn, Guid id) 
        {
            if (! connectionsInRoom.ContainsKey(id) || ! roomInfos.ContainsKey(id)) 
            {
                Debug.Log($"{TAG} | OnServerJoinRoom(), room id not exist. return.");
                return;
            }

            WorldAvatarInfo info = avatarInfos[conn];
            info.roomId = id;
            avatarInfos[conn] = info;

            WorldAvatarInfo[] infos = connectionsInRoom[id].Select(playerConn => avatarInfos[playerConn]).ToArray();
            SendRoomList();
            SendUserList();
        }

        [ServerCallback]
        private void OnServerLeaveRoom(NetworkConnectionToClient conn, Guid id) 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{TAG} | OnServerLeaveRoom(), selected room id not exist. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.LeaveRoom, roomId = selectRoomId });
        }

        [ServerCallback]
        private void OnServerDeleteRoom(NetworkConnectionToClient conn, Guid id) 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{TAG} | OnServerDeleteRoom(), selected room id not exist. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.DeleteRoom, roomId = selectRoomId });
        }

        [ServerCallback]
        private void OnServerRemoveFromRoom(NetworkConnectionToClient conn, Guid id) 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{TAG} | OnServerRemoveFromRoom(), selected room id not exist. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.RemoveFromRoom, roomId = selectRoomId });
        }

        /// <summary>
        /// 모임방 목록 업데이트 신호 전달
        /// </summary>
        [ServerCallback]
        internal void SendRoomList(NetworkConnectionToClient conn = null) 
        {
            if (conn != null) 
            {
                conn.Send(new ClientMessage { clientOperation = ClientOperation.RoomList, roomList = roomInfos.Values.ToArray() });
            }
            else 
            {
                foreach (var avatar in connectionsInCenter) 
                {
                    avatar.Send(new ClientMessage { clientOperation = ClientOperation.RoomList, roomList = roomInfos.Values.ToArray() });
                }
            }
        }    


        /// <summary>
        /// 광장 내부 사용자 목록 업데이트 신호 전달
        /// </summary>
        [ServerCallback]
        internal void SendUserList(NetworkConnectionToClient conn = null)
        {
            if (conn != null)
            {
                conn.Send(new ClientMessage { clientOperation = ClientOperation.UserList, avatarsInCenter = avatarsInCenter.Values.ToArray() });
            }
            else
            {
                foreach (var avatar in connectionsInCenter)
                {
                    avatar.Send(new ClientMessage { clientOperation = ClientOperation.UserList, avatarsInCenter = avatarsInCenter.Values.ToArray() });
                }
            }
        }

    #endregion  // Server message handler


    #region Client message handler

        private void OnClientMessage(ClientMessage msg) 
        {
            Debug.Log($"{TAG} | OnClientMessage(), msg : {msg.clientOperation}");
            switch (msg.clientOperation)
            {
                case ClientOperation.None :
                    Debug.LogWarning($"{TAG} | OnClientMessage(), Missing ServerMatchOperation");
                    break;

                case ClientOperation.RoomList :
                    roomInfos.Clear();
                    foreach (var roomInfo in msg.roomList) 
                        roomInfos.Add(roomInfo.roomId, roomInfo);
                    break;

                case ClientOperation.RoomCreated :
                    SendRoomList();
                    SendUserList(); 
                    break;

                case ClientOperation.RoomJoined :
                    SendRoomList();
                    break;

                case ClientOperation.RoomDeparted :
                    selectRoomId = Guid.Empty;
                    SendRoomList();
                    SendUserList();
                    break;

                case ClientOperation.UpdateRoom :
                    avatarsInRoom.Clear();
                    foreach (var avatarInfo in msg.avatarsInRoom) 
                        avatarsInRoom.Add(avatarInfo.seq, avatarInfo);
                    break;

                case ClientOperation.UserList :
                    avatarsInCenter.Clear();
                    foreach (var avatarInfo in msg.avatarsInCenter)
                        avatarsInCenter.Add(avatarInfo.seq, avatarInfo);

                    /**
                    foreach (var avatarInfo in msg.avatarsInRoom) 
                        avatarsInRoom.Add(avatarInfo.seq, avatarInfo);
                     */

                    R.singleton.CurrentUserCount = msg.avatarsInCenter.Length;
                    break;
            }
        }

    #endregion  // Client message handler
    }
}