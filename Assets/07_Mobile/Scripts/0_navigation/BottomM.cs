/// <summary>
/// [mobile] 
/// BOTTOM Navigation Bar
/// @author         : HJ Lee
/// @last update    : 2023. 07. 19
/// @version        : 0.5
/// @update         :
///     v0.1 : 최초 생성.
///     v0.2 (2022. 06. 08) : toggle 에 속한 이미지 중 background 이미지는 on 상태일 때 Active false 처리. 
///     v0.3 (2023. 03. 22) : FixedView 실험, UI 최적화 (TMP 제거)
///     v0.4 (2023. 06. 16) : meeting_m -> meeting root_m 로 변경.
///     v0.5 (2023. 07. 19) : Repository observer 추가. 읽지 않은 메시지 카운트 출력용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class BottomM : FixedView, iRepositoryObserver
    {
        private const string TAG = "BottomM";

        [Header("Toggles")] 
        [SerializeField] private Toggle _toggleOffice;
        [SerializeField] private Toggle _toggleMySeat;
        [SerializeField] private Toggle _toggleFileBox;
        [SerializeField] private Toggle _toggleMeeting;
        [SerializeField] private Toggle _toggleChat;

        [Header("unread chat count")]
        [SerializeField] private Image _imgChatOn;
        [SerializeField] private TMP_Text _txtChatCount;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            R.singleton.RegisterObserver(this, eChatKey);
        }

        private void OnDestory() 
        {
            if (R.singleton != null) 
            {
                R.singleton.UnregisterObserver(this, eChatKey);
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_NaviBottom;

            // set toggle listener
            _toggleOffice.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (ViewManager.singleton.GetTopViewName().Equals(S.MobileScene_Office)) return;

                    ViewManager.singleton.PopAll();
                    ViewManager.singleton.Push(S.MobileScene_Office);
                }
            });

            _toggleMySeat.onValueChanged.AddListener((on) => {
                Debug.Log($"{TAG} | top view : {ViewManager.singleton.GetTopViewName()}"); 
                if (on) 
                {
                    if (ViewManager.singleton.GetTopViewName().Equals(S.MobileScene_MySeat)) return;

                    ViewManager.singleton.PopAll();
                    ViewManager.singleton.Push(S.MobileScene_MySeat);
                }
            });
            
            _toggleFileBox.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (ViewManager.singleton.GetTopViewName().Equals(S.MobileScene_FileRoot)) return;

                    ViewManager.singleton.PopAll();
                    ViewManager.singleton.Push(S.MobileScene_FileRoot);
                }
            });

            _toggleMeeting.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (ViewManager.singleton.GetTopViewName().Equals(S.MobileScene_MeetingRoot)) return;

                    ViewManager.singleton.PopAll();
                    ViewManager.singleton.Push(S.MobileScene_MeetingRoot);
                }
            });

            _toggleChat.onValueChanged.AddListener((on) => {
                Debug.Log($"{TAG} | top view : {ViewManager.singleton.GetTopViewName()}"); 
                if (on) 
                {
                    if (ViewManager.singleton.GetTopViewName().Equals(S.MobileScene_Chat)) return;

                    ViewManager.singleton.PopAll();
                    ViewManager.singleton.Push(S.MobileScene_Chat);
                }
            });

            // set event variables
            eChatKey = eStorageKey.Chat;
            unReadCount = -1;
        }

    #endregion  // FixedView functions


    #region Event Listener

        private eStorageKey eChatKey;
        private int unReadCount;

        public void UpdateInfo(eStorageKey key) 
        {
            Debug.Log($"{TAG} | key : {key}");
            if (key == eChatKey) 
            {
                if (unReadCount != R.singleton.UnreadChatCount) 
                {
                    unReadCount = R.singleton.UnreadChatCount;
                    _txtChatCount.text = unReadCount > 99 ? "99+" : $"{unReadCount}";
                    _imgChatOn.gameObject.SetActive(unReadCount != 0);
                }
            }
        }

    #endregion  // Event Listener


    #region Other functions

        public void ShowNavigation(bool on) 
        {
            canvasGroup.alpha = on ? 1 : 0;
            canvasGroup.interactable = on ? true : false;
            canvasGroup.blocksRaycasts = on ? true : false;
        }

        public void StartOnMySeat(bool on) 
        {
            if (on) 
            {
                if (_toggleMySeat.isOn) 
                    ViewManager.singleton.Push(S.MobileScene_MySeat);
                else 
                    _toggleMySeat.isOn = true;
            }
            else 
            {
                if (_toggleOffice.isOn) 
                    ViewManager.singleton.Push(S.MobileScene_Office);
                else
                    _toggleOffice.isOn = true;
            }
        }

    #endregion  // Other functions
    }
}