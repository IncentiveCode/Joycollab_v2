/// <summary>
/// World 에서 사용할 네트워크 메시지 모음.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 22 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 22) : Mirror.Examples.MultipleMatch 참고해서 최초 생성
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/**
    playerMatches 		=> avatarMatches
    openMatches 		=> roomInfos
    matchConnections 	=> connectionsInRoom
    playerInfos 		=> avatarInfos
    waitingConnections 	=> connectionsInCenter
    waitingInfos 		=> avatarsInCenter
 */

namespace Joycollab.v2
{
    public class WorldController : MonoBehaviour
    {
        private const string Tag = "WorldController";
        
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

        /// <summary>
        /// 특정 모임방에 들어가 있는 사용자 리스트
        /// </summary>
        internal static readonly Dictionary<int, WorldAvatarInfo> avatarsInRoom = new Dictionary<int, WorldAvatarInfo>();

        /// <summary>
        /// GUID of a match the local player has created
        /// </summary>
        internal Guid selectRoomId = Guid.Empty;
        
        // local variables
        internal static WorldAvatarInfo localPlayerInfo;


        [Header("GUI, World, Room")]
        [SerializeField] private GameObject goCenter;
        [SerializeField] private GameObject goRoom;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ResetStatics()
        {
            avatarMatches.Clear();
            roomInfos.Clear();
            connectionsInRoom.Clear();
            avatarInfos.Clear();
            connectionsInCenter.Clear();
            avatarsInCenter.Clear();
            avatarsInRoom.Clear();
        }
 

    #region UI functions

        // Called from several places to ensure a clean reset
        //  - MatchNetworkManager.Awake
        //  - OnStartServer
        //  - OnStartClient
        //  - OnClientDisconnect
        //  - ResetCanvas
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

            goCenter.SetActive(true);
            goRoom.SetActive(false);
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
                Debug.Log($"{Tag} | RequestJoinRoom(), select room id == empty. return.");
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
                Debug.Log($"{Tag} | RequestLeaveRoom(), select room id == empty. return.");
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

        [ServerCallback]
        internal void OnStartServer() 
        {
            InitializeData();
            NetworkServer.RegisterHandler<ServerMessage>(OnServerMessage); 
        }

        [ServerCallback]
        internal void OnServerReady(NetworkConnectionToClient conn) 
        {
            connectionsInCenter.Add(conn);

            avatarsInCenter.Add(localPlayerInfo.seq, localPlayerInfo);

            SendRoomList();
            SendUserList();
        }

        [ServerCallback]
        internal IEnumerator OnServerDisconnect(NetworkConnectionToClient conn) 
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
            if (avatarInfo.roomId != Guid.Empty) 
            {
                ClasInfo clasInfo;
                if (roomInfos.TryGetValue(avatarInfo.roomId, out clasInfo))
                {
                    // ...
                    Debug.Log($"{Tag} | OnServerDisconnect(), 원래는 여기에서 player 수를 줄이고 match 목록을 정리함.");
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

            SendRoomList();
            SendUserList();

            yield return null;
        }

        [ServerCallback]
        internal void OnStopServer() 
        {
            ResetSpace();
        }

        [ServerCallback]
        internal void OnClientConnect() 
        {
            Debug.Log($"CanvasController | OnClientConnect(), seq : {localPlayerInfo.seq}, name : {localPlayerInfo.nickNm}");
        }

        [ClientCallback]
        internal void OnStartClient() 
        {
            InitializeData();

            NetworkClient.RegisterHandler<ClientMessage>(OnClientMessage);
        }

        [ClientCallback]
        internal void OnClientDisconnect() 
        {
            InitializeData();
        }

        [ClientCallback]
        internal void OnStopClient() 
        {
            ResetSpace();
        }

    #endregion  // Server & Client callbacks


    #region Server message handler

        private void OnServerMessage(NetworkConnectionToClient conn, ServerMessage msg) 
        {
            switch (msg.serverOperation) 
            {
                case ServerOperation.None :
                    Debug.LogWarning($"{Tag} | OnServerMessage(), Missing ServerMatchOperation");
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
            Debug.Log($"{Tag} | Missing ServerMatchOperation");
            connectionsInRoom.Add(id, new HashSet<NetworkConnectionToClient>());

            // TODO. 이 부분은 모임방 생성 UI 에서 추가할 예정.
            // roomInfos.Add(id, new ClasInfo());

            conn.Send(new ClientMessage { clientOperation = ClientOperation.RoomCreated, roomId = id, 
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
                Debug.Log($"{Tag} | OnServerJoinRoom(), room id not exist. return.");
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
                Debug.Log($"{Tag} | OnServerLeaveRoom(), selected room id not exist. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.LeaveRoom, roomId = selectRoomId });
        }

        [ServerCallback]
        private void OnServerDeleteRoom(NetworkConnectionToClient conn, Guid id) 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{Tag} | OnServerDeleteRoom(), selected room id not exist. return.");
                return;
            }

            NetworkClient.Send(new ServerMessage { serverOperation = ServerOperation.DeleteRoom, roomId = selectRoomId });
        }

        [ServerCallback]
        private void OnServerRemoveFromRoom(NetworkConnectionToClient conn, Guid id) 
        {
            if (selectRoomId == Guid.Empty)
            {
                Debug.Log($"{Tag} | OnServerRemoveFromRoom(), selected room id not exist. return.");
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
            switch (msg.clientOperation)
            {
                case ClientOperation.None :
                    Debug.LogWarning($"{Tag} | OnClientMessage(), Missing ServerMatchOperation");
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
                    break;
            }
        }

    #endregion  // Client message handler
    }
}