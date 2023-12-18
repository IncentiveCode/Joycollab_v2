/// <summary>
/// Square 에서 사용할 instant message chatting view class
/// @author         : HJ Lee
/// @last update    : 2023. 12. 07 
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 09. 19) : history chat 을 legacy text 로 변경.
///     v0.3 (2023. 12. 07) : SSL 을 사용할 때는 chat view 가 사라지는 문제 수정 (중)
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace Joycollab.v2
{
    // public class WorldChatView : NetworkBehaviour
    public class WorldChatView : NetworkBehaviour
    {
        private const string TAG = "WorldChatView";

        [Header("Chat UI")]
        [SerializeField] private TMP_Text _txtChatHistory;
        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private TMP_InputField _inputMessage;
        [SerializeField] private Button _btnSend;

        // Player names
        internal static WorldAvatarInfo localPlayerInfo;
        internal static readonly Dictionary<NetworkConnectionToClient, string> playerNames = new Dictionary<NetworkConnectionToClient, string>();



    #region Unity functions

        private void Awake() 
        {
            Debug.Log($"{TAG} | Awake()");

            _inputMessage.onSubmit.AddListener((input) => {
                Send(input.Trim());
            });
            _inputMessage.onValueChanged.AddListener((input) => {
                _btnSend.interactable = !string.IsNullOrWhiteSpace(input);
            });
            _btnSend.onClick.AddListener(() => Send(_inputMessage.text.Trim()));
        }

        private void OnDisable() 
        {
            Debug.Log($"{TAG} | OnDisable()");
        }

    #endregion  // Unity functions


    #region override functions

        public override void OnStartServer() 
        {
            Debug.Log($"{TAG} | OnStartServer()");

            playerNames.Clear();
        } 

        public override void OnStartClient() 
        {
            Debug.Log($"{TAG} | OnStartClient()");

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

            yield return null;
            yield return null;

            _scrollbar.value = 0;
        }

        private void Send(string msg) 
        {
            if (! string.IsNullOrWhiteSpace(msg)) 
            {
                Debug.Log($"{TAG} | Send()");

                CmdSend(msg);
                _inputMessage.text = string.Empty;
                _inputMessage.ActivateInputField();
            }
        }

    #endregion


    #region Command functions

        [Command(requiresAuthority = false)]
        private void CmdSend(string message, NetworkConnectionToClient sender = null) 
        {
            Debug.Log($"{TAG} | CmdSend()");

            if (sender.identity.TryGetComponent(out WorldPlayer player)) 
            {
                if (!playerNames.ContainsKey(sender)) 
                {
                    playerNames.Add(sender, player.avatarName);
                }
                else
                {
                    if (! playerNames[sender].Equals(player.avatarName))
                        playerNames[sender] = player.avatarName;
                }

                if (!string.IsNullOrWhiteSpace(message)) 
                {
                    RpcReceive(playerNames[sender], message.Trim());
                    player.UpdateAvatarChat(message.Trim());
                }
            }
        }

    #endregion


    #region ClientRpc functions

        [ClientRpc]
        private void RpcReceive(string playerName, string message) 
        {
            Debug.Log($"{TAG} | RpcReceive(), palyer name : {playerName}, localPlayerInfo name : {localPlayerInfo.nickNm}");

            string msg = (playerName.Equals(localPlayerInfo.nickNm)) ? 
                $"<color=red>{playerName}</color> : {message}" :
                $"<color=blue>{playerName}</color> : {message}";

            AppendMessage(msg);
        }

    #endregion
    }
}