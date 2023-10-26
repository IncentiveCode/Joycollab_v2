/// <summary>
/// World - Square 에서 사용할 Mirror Network Manager class
/// @author         : HJ Lee
/// @last update    : 2023. 03. 07 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
/// </summary>

using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class WorldNetworkManager : NetworkManager
    {
    #region override functions

        public override void Awake() 
        {
            base.Awake();
        }
        
        public override void OnStartServer() 
        {
            Debug.Log("[Server] server started.");
            base.OnStartServer();
        }

        public override void OnStopServer() 
        {
            Debug.Log("[Server] server stoped.");
            base.OnStopServer();
        }

        public override void OnClientConnect()
        {
            Debug.Log("> connect to server.");
            base.OnClientConnect();

            
        }

        public override void OnClientDisconnect() 
        {
            Debug.Log("> disconnect from server.");
            base.OnClientDisconnect();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn) 
        {
            Transform start = GetStartPosition();
            Debug.Log("[Server] OnServerAddPlayer() | transform : "+ start.position);

            GameObject player = (start == null) ? 
                Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) :
                Instantiate(playerPrefab, start); 

            player.transform.SetParent(null);
            NetworkServer.AddPlayerForConnection(conn, player);
            WorldAvatar script = player.GetComponent<WorldAvatar>();
            WorldAvatarInfo info = (WorldAvatarInfo) conn.authenticationData;
            script.UpdateAvatarInfo(info);
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn) 
        {
            if (conn.authenticationData != null) 
            {
                WorldAvatarList.avatarInfos.Remove((WorldAvatarInfo) conn.authenticationData);
            }

            WorldChatView.playerNames.Remove(conn);
            base.OnServerDisconnect(conn);
        }

    #endregion  // override functions 
    }
}