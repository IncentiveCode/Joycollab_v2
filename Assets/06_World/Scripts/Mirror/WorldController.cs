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
        /// 특정 모임방에 들어가 있는 사용자 리스트
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
                }

                HashSet<NetworkConnectionToClient> connections;
                if (connectionsInRoom.TryGetValue(avatarInfo.roomId, out connections)) 
                {
                    WorldAvatarInfo[] infos = connections.Select(playerConn => avatarInfos[playerConn]).ToArray();

                    foreach (var playerConn in connectionsInRoom[avatarInfo.roomId]) 
                    {
                        if (playerConn != conn) 
                            playerConn.Send(new ClientMessage { clientOperation = ClientOperation.UpdateRoom, avatarInfos = infos });
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

            // SendRoomList();
            // SendUserList();

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
            // avatarInfos.Add(NetworkClient.connection, new 
        }

    #endregion  // Server & Client callbacks


    #region Server message handler

        private void OnServerMessage(NetworkConnectionToClient conn, ServerMessage msg) 
        {

        }

    #endregion  // Server message handler
    }
}