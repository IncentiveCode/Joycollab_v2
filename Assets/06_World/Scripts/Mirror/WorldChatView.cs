/// <summary>
/// Square 에서 사용할 instant message chatting view class
/// @author         : HJ Lee
/// @last update    : 2024. 01. 04 
/// @version        : 0.4
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성, mirror-test 에서 작업한 항목 migration.
///     v0.2 (2023. 09. 19) : history chat 을 legacy text 로 변경.
///     v0.3 (2023. 12. 07) : SSL 을 사용할 때는 chat view 가 사라지는 문제 수정 (중) -> (실패)
///     v0.4 (2024. 01. 04) : 기존 내용 중 Command 와 ClientRpc 항목을 WorldPlayer 로 이전.
/// </summary>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace Joycollab.v2
{
    public class WorldChatView : MonoBehaviour
    {
        private const string TAG = "WorldChatView";
        public static WorldChatView singleton { get; private set; }

        [Header("Chat UI")]
        [SerializeField] private TMP_Text _txtChatHistory;
        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private TMP_InputField _inputMessage;
        [SerializeField] private Button _btnSend;


    #region Unity functions

        private void Awake() 
        {
            Debug.Log($"{TAG} | Awake()");
            singleton = this;

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


    #region chat view functions

        public void Clear() 
        {
            _txtChatHistory.text = string.Empty;
            _inputMessage.text = string.Empty;
        }

        public void AppendMessage(string msg) 
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

                WorldPlayer.localPlayer.CmdSend(msg);
                _inputMessage.text = string.Empty;
                _inputMessage.ActivateInputField();
            }
        }

    #endregion  // chat view functions
    }
}