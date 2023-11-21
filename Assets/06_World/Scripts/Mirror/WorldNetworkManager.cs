/// <summary>
/// World - Square 에서 사용할 Mirror Network Manager class
/// @author         : HJ Lee
/// @last update    : 2023. 11. 21
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 11. 21) : Mirror.Examples.MultipleMatch 참고해서 수정.
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class WorldNetworkManager : NetworkManager
    {
        private const string TAG = "WorldNetworkManager";

        /// <summary>
        /// 모임방 내부 사용자 (Cross-reference of client)
        /// </summary>
        internal static readonly Dictionary<NetworkConnectionToClient, Guid> playerInRoom = new Dictionary<NetworkConnectionToClient, Guid>();

        /// <summary>
        /// 현재 모임방 리스트 (game 을 시작하지는 않을 예정)
        /// </summary>
        internal static readonly Dictionary<Guid, ClasInfo> openRooms = new Dictionary<Guid, ClasInfo>();


    #region override functions

        public override void Awake() 
        {
            base.Awake();
        }
        
        public override void OnStartServer() 
        {
            Debug.Log($"{TAG} | server started.");
            base.OnStartServer();
        }

        public override void OnStopServer() 
        {
            Debug.Log($"{TAG} | server stoped.");
            base.OnStopServer();
        }

        public override void OnClientConnect()
        {
            Debug.Log($"{TAG} | connect to server.");
            base.OnClientConnect();
        }

        public override void OnClientDisconnect() 
        {
            Debug.Log($"{TAG} | disconnect from server.");
            base.OnClientDisconnect();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn) 
        {
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

            Debug.Log($"{TAG} | OnServerAddPlayer(), WorldAvatarList 에 정보 추가.");
            WorldAvatarList.avatarInfos.Add(info);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn) 
        {
            if (conn.authenticationData != null) 
            {
                Debug.Log($"{TAG} | OnServerDisconnect(), WorldAvatarList 에서 정보 삭제.");
                WorldAvatarList.avatarInfos.Remove((WorldAvatarInfo) conn.authenticationData);
            }

            WorldChatView.playerNames.Remove(conn);
            base.OnServerDisconnect(conn);
        }

    #endregion  // override functions 
    }
}