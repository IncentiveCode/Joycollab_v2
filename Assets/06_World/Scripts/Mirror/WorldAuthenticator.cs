/// <summary>
/// [Mirror] WorldScene - SquareScene 에서 사용할 사용자 인증 툴
/// @author         : HJ Lee
/// @last update    : 2023. 12. 05 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 12. 05) : SSL 을 사용할 때는 chat view 가 사라지는 문제 수정 (중)
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class WorldAuthenticator : NetworkAuthenticator
    {
        private const string TAG = "WorldAuthenticator";

        readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>(); 
        internal static readonly HashSet<string> playerNames = new HashSet<string>();

        [Header("client user name")]
        public string playerName;


	#region Messages 

		public struct AuthRequestMessage : NetworkMessage 
		{
			public string authUserName;
		}

		public struct AuthResponseMessage : NetworkMessage 
		{
			public byte code;
			public string message;
		}

	#endregion  // Messages


    #region override functions - for server

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void ResetStatics() 
        {
            playerNames.Clear();
        }

        public override void OnStartServer() 
        {
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        public override void OnStopServer() 
        {
            NetworkServer.UnregisterHandler<AuthRequestMessage>();
        }

        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            // do nothing...wait for AuthRequestMessage from client
            Debug.Log($"{TAG} | OnServerAuthenticate()");
        }

		private void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage message) 
        { 
            Debug.Log($"Authentication Request: {message.authUserName}");

            if (connectionsPendingDisconnect.Contains(conn)) return;

            if (! playerNames.Contains(message.authUserName)) 
            {
                playerNames.Add(message.authUserName);
                conn.authenticationData = message.authUserName;

                AuthResponseMessage res = new AuthResponseMessage
                {
                    code = 100, 
                    message = "Success"
                };
                conn.Send(res);
                ServerAccept(conn);
            }
            else 
            {
                connectionsPendingDisconnect.Add(conn);

                AuthResponseMessage res = new AuthResponseMessage
                {
                    code = 200, 
                    message = "User already in use...try again"
                };
                conn.Send(res);
                conn.isAuthenticated = false;

                StartCoroutine(DelayedDisconnect(conn, 1f));
            }
        }	

		private IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float delay) 
        {
            yield return new WaitForSeconds(delay);
            ServerReject(conn);

            yield return null;
            connectionsPendingDisconnect.Remove(conn);
        }

    #endregion  // override functions - for server


	#region override functions - for client

		public override void OnStartClient() 
        {
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);

            SetPlayerName(R.singleton.myName);
        }

        public override void OnStopClient() 
        {
            NetworkClient.UnregisterHandler<AuthResponseMessage>();
        }

        public override void OnClientAuthenticate()
        {
            NetworkClient.Send(new AuthRequestMessage { authUserName = playerName });
        }

        private void OnAuthResponseMessage(AuthResponseMessage message) 
        {
            if (message.code == 100) 
            {
                Debug.Log($"Authentication Response: {message.message}");
                ClientAccept();
            }
            else 
            {
                Debug.LogError($"Authentication Response: {message.message}");
                NetworkManager.singleton.StopHost();
            }
        }

	#endregion	// override functions - for client
        

	#region other functions

        public void SetPlayerName(string userName) 
        {
            playerName = userName;
        }

	#endregion	// other functions
    }
}