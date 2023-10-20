/// <summary>
/// Square 에서 사용할 instant message chatting view class
/// @author         : HJ Lee
/// @last update    : 2023. 09. 19 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 09. 19) : history chat 을 legacy text 로 변경.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace Joycollab.v2
{
    public class WorldChatView : NetworkBehaviour
    {
        public static WorldChatView Instance;
        public bool OnChat { get; private set; }

        [Header("Chat UI")]
        [SerializeField] private Text _txtChatHistory;
        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private TMP_InputField _inputMessage;
        [SerializeField] private Button _btnSend;

        // Player names
        internal static WorldAvatarInfo localPlayerInfo;
        internal static readonly Dictionary<NetworkConnectionToClient, string> playerNames = new Dictionary<NetworkConnectionToClient, string>();

        // local variables
        private WaitForSeconds delay;
        private NetworkManager networkManager;


    #region Unity functions

        private void Awake() 
        {
            Instance = this;
            delay = new WaitForSeconds(1f);
            networkManager = WorldNetworkManager.singleton;

            _inputMessage.onSubmit.AddListener((input) => {
                Send(input.Trim());
            });
            _inputMessage.onValueChanged.AddListener((input) => {
                _btnSend.interactable = !string.IsNullOrWhiteSpace(input);
            });
            _inputMessage.onSelect.AddListener((input) => OnChat = true);
            _inputMessage.onDeselect.AddListener((input) => OnChat = false);
            _btnSend.onClick.AddListener(() => Send(_inputMessage.text.Trim()));
        }

        /**
        private void OnDestroy() 
        {
            // close host
            if (NetworkServer.active && NetworkClient.isConnected) 
            {
                networkManager.StopClient();
                networkManager.StopHost();
            }
            // close client
            if (NetworkClient.isConnected) 
            {
                networkManager.StopClient();
            }
        }
         */

    #endregion  // Unity functions


    #region override functions

        public override void OnStartServer() 
        {
            _txtChatHistory.text = string.Empty;
            playerNames.Clear();
        } 

        public override void OnStartClient() 
        {
            _txtChatHistory.text = string.Empty;
            _inputMessage.text = string.Empty;
        }

    #endregion


    #region Private functions

        private void AppendMessage(string msg) 
        {
            StartCoroutine(AppendAndScroll(msg));
        }

        private IEnumerator AppendAndScroll(string msg) 
        {
            _txtChatHistory.text += msg + "\n";

            yield return delay;

            _scrollbar.value = 0;
        }

        private void Send(string msg) 
        {
            if (! string.IsNullOrWhiteSpace(msg)) 
            {
                CommandSend(msg);
                _inputMessage.text = string.Empty;
                _inputMessage.ActivateInputField();
            }
        }

    #endregion


    #region Command functions

        [Command(requiresAuthority = false)]
        private void CommandSend(string message, NetworkConnectionToClient sender = null) 
        {
            if (! sender.identity.TryGetComponent(out WorldAvatar avatar)) 
            {
                Debug.Log("WorldAvatar component 가 없음.");
                return;
            }

            /**
            if (!playerNames.ContainsKey(sender)) 
                playerNames.Add(sender, sender.identity.GetComponent<WorldAvatar>().avatarName);

            if (!string.IsNullOrWhiteSpace(message)) 
            {
                RpcReceive(playerNames[sender], message.Trim());
                sender.identity.GetComponent<WorldAvatar>().UpdateAvatarChat(message.Trim());
            }
             */

            if (!playerNames.ContainsKey(sender)) 
                playerNames.Add(sender, avatar.avatarName);

            if (!string.IsNullOrWhiteSpace(message)) 
            {
                RpcReceive(playerNames[sender], message.Trim());
                avatar.UpdateAvatarChat(message.Trim());
            }
        }

    #endregion


    #region ClientRpc functions

        [ClientRpc]
        private void RpcReceive(string playerName, string message) 
        {
            string msg = (playerName.Equals(localPlayerInfo.nickNm)) ? 
                $"<color=red>{playerName}</color> : {message}" :
                $"<color=blue>{playerName}</color> : {message}";

            AppendMessage(msg);
        }

    #endregion
    }
}