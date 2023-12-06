/// <summary>
/// [world]
/// Floating bar 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 11. 15 
/// @version        : 0.5
/// @update
///     v0.1 (2023. 09. 18) : v1 에서 사용하던 항목 수정 후 적용. (진행 중)
///     v0.2 (2023. 10. 30) : Expandable 적용. meeting, seminar 버튼 추가.
///     v0.3 (2023. 10. 31) : 모임방 버튼 추가.
///     v0.4 (2023. 11. 06) : guest 를 위한 Floating bar 구성 추가.
///     v0.5 (2203. 11. 13) : Map scene 에서 사용하기 위한 Floating bar 구성 추가.
///     v0.6 (2023. 11. 15) : last sibling 기능 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class FloatingBarW : MonoBehaviour, iRepositoryObserver
    {
        private const string TAG = "FloatingBarW"; 

        [Header("panel")]
        [SerializeField] private RectTransform _rect;
        
        [Header("profile")]
        [SerializeField] private RawImage _imgProfile;
        private ImageLoader loader;

        [Header("alarm count")]
        [SerializeField] private Image _imgAlarmPanel;
        [SerializeField] private Text _txtAlarmCount;

        [Header("chat")]
        [SerializeField] private Image _imgChatPanel;
        [SerializeField] private Text _txtChatCount;

        [Header("user list")]
        [SerializeField] private Image _imgUserPanel;
        [SerializeField] private Text _txtUserCount;

        [Header("button")]
        [SerializeField] private Button _btnProfile;
        [SerializeField] private Button _btnMicControl;
        [SerializeField] private Button _btnAlarm;
        [SerializeField] private Button _btnBookmark;
        [SerializeField] private Button _btnChat;
        [SerializeField] private Button _btnMeeting;
        [SerializeField] private Button _btnSeminar;
        [SerializeField] private Button _btnGathering;
        [SerializeField] private Button _btnUserList;
        [SerializeField] private Button _btnSettings;
        [SerializeField] private Button _btnSignOut;

        [Header("Expandable")]
        [SerializeField] private Expandable _expandable;
        [SerializeField] private Button _btnExpand;
        [SerializeField] private Button _btnClose;

        // local variables
        private string myPhoto;
        private int alarmCount, chatCount, userCount;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnProfile.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_MyProfile);
            });
            _btnMicControl.onClick.AddListener(() => {
                OnPointerDown();
                Debug.Log($"{TAG} | mic option change.");
            });
            _btnAlarm.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_AlarmList);
            });
            _btnBookmark.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_Bookmark);
            });
            _btnChat.onClick.AddListener(() => {
                OnPointerDown();
                string url = string.Format(URL.CHAT_LINK, R.singleton.memberSeq, R.singleton.Region);
                JsLib.OpenChat(url);
            });
            _btnMeeting.onClick.AddListener(() => {
                OnPointerDown();
                Debug.Log($"{TAG} | meeting panel open.");
            });
            _btnSeminar.onClick.AddListener(() => {
                OnPointerDown();
                Debug.Log($"{TAG} | seminar panel open.");
            });
            _btnGathering.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_RoomList);
            });
            _btnUserList.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_UserList);
            });
            _btnSettings.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_Settings);
            });
            _btnSignOut.onClick.AddListener(() => {
                OnPointerDown();
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그아웃 확인", R.singleton.CurrentLocale),
                    () => SystemManager.singleton.SignOut().Forget()
                );
            });
            _btnExpand.onClick.AddListener(() => {
                OnPointerDown();
                _expandable.RequestExpand();
            });
            _btnClose.onClick.AddListener(() => {
                OnPointerDown();
                _expandable.RequestClose();
            });


            // set local variables
            loader = _imgProfile.GetComponent<ImageLoader>();
            myPhoto = string.Empty;
            alarmCount = chatCount = userCount = -1;


            // register event 
            if (R.singleton != null)
            {
                R.singleton.RegisterObserver(this, eStorageKey.MemberInfo);
                R.singleton.RegisterObserver(this, eStorageKey.Alarm);
                R.singleton.RegisterObserver(this, eStorageKey.Chat);
                R.singleton.RegisterObserver(this, eStorageKey.WindowRefresh);
                R.singleton.RegisterObserver(this, eStorageKey.UserCount);
            }
        }
        
        private void OnEnable() 
        {
            if (R.singleton != null) 
            {
                R.singleton.RequestInfo(this, eStorageKey.MemberInfo);
                R.singleton.RequestInfo(this, eStorageKey.Alarm);
                R.singleton.RequestInfo(this, eStorageKey.Chat);
                R.singleton.RequestInfo(this, eStorageKey.WindowRefresh);
                // R.singleton.RequestInfo(this, eStorageKey.UserCount);
            }
        } 

        private void OnDestroy() 
        {
            myPhoto = string.Empty;
            alarmCount = chatCount = userCount = -1;

            // unregister event
            if (R.singleton != null)
            {
                R.singleton.UnregisterObserver(this, eStorageKey.MemberInfo);
                R.singleton.UnregisterObserver(this, eStorageKey.Alarm);
                R.singleton.UnregisterObserver(this, eStorageKey.Chat);
                R.singleton.UnregisterObserver(this, eStorageKey.WindowRefresh);
                R.singleton.UnregisterObserver(this, eStorageKey.UserCount);
            }
        }

    #endregion  // Unity functions


    #region Event handling

        public void UpdateInfo(eStorageKey key) 
        {
            Debug.Log($"{TAG} | UpdateInfo() call. key : {key}");
            switch (key) 
            {
                case eStorageKey.MemberInfo :
                    Debug.Log($"{TAG} | UpdateInfo (UserPhoto) - photo : {myPhoto}, photo in R : {R.singleton.myPhoto}");
                    if (!myPhoto.Equals(R.singleton.myPhoto)) 
                    {
                        myPhoto = R.singleton.myPhoto;

                        string url = $"{URL.SERVER_PATH}{myPhoto}";
                        Debug.Log($"{TAG} | photo url : {url}");
                        int seq = R.singleton.memberSeq;
                        loader.LoadProfile(url, seq).Forget();
                    }     
                    break;

                case eStorageKey.Alarm :
                    if (alarmCount != R.singleton.UnreadAlarmCount) 
                    {
                        alarmCount = R.singleton.UnreadAlarmCount;
                        _txtAlarmCount.text = alarmCount > 99 ? "99+" : $"{alarmCount}";
                        _imgAlarmPanel.gameObject.SetActive(alarmCount != 0);
                    }
                    break;

                case eStorageKey.Chat :
                    if (chatCount != R.singleton.UnreadChatCount) 
                    {
                        chatCount = R.singleton.UnreadChatCount;
                        _txtChatCount.text = chatCount > 99 ? "99+" : $"{chatCount}";
                        _imgChatPanel.gameObject.SetActive(chatCount != 0);
                    }
                    break; 

                case eStorageKey.WindowRefresh :
                    RefreshPanel();
                    break;

                case eStorageKey.UserCount :
                    if (SceneLoader.isSquare())
                    {
                        if (userCount != R.singleton.CurrentUserCount) 
                        {
                            _txtUserCount.text = userCount > 99 ? "99+" : $"{userCount}";
                            _imgUserPanel.gameObject.SetActive(userCount > 0);
                        }
                    }
                    else if (SceneLoader.isRoom()) 
                    {
                        // TODO. 모임방에서 인원수 표시 수정할 것.
                    }
                    break;

                default :
                    Debug.Log($"{TAG} | 여기에서 사용하지 않는 항목. key : {key}");
                    break;
            }
        }

        private void RefreshPanel() 
        {
            // member, guest check
            bool isGuest = R.singleton.myMemberType.Equals(S.GUEST);

            // map, square, room scene check
            bool isMap = SceneLoader.isMap();
            bool isSquare = SceneLoader.isSquare();
            bool isRoom = SceneLoader.isRoom();
            Debug.Log($"{TAG} | isMap ? {isMap}, isSquare ? {isSquare}, isRoom ? {isRoom}");

            // common setting
            _btnProfile.interactable = !isGuest;
            if (isGuest) 
            {
                _btnMicControl.gameObject.SetActive(true);
                _btnUserList.gameObject.SetActive(true);
                _btnAlarm.gameObject.SetActive(false);
                _btnBookmark.gameObject.SetActive(false);
                _btnChat.gameObject.SetActive(false);
                _btnMeeting.gameObject.SetActive(false);
                _btnSeminar.gameObject.SetActive(false);
                _btnGathering.gameObject.SetActive(false);
                _btnSettings.gameObject.SetActive(false);
                _btnSignOut.gameObject.SetActive(true);
                _btnExpand.gameObject.SetActive(false);
                _btnClose.gameObject.SetActive(false);
            }
            else
            {
                _btnAlarm.gameObject.SetActive(true);
                _btnChat.gameObject.SetActive(true);

                if (isMap) 
                {
                    _expandable.RequestCloseForMap();
                    _btnSignOut.gameObject.SetActive(true);
                }
                else if (isSquare) 
                {
                    _expandable.RequestClose();
                    _btnSignOut.gameObject.SetActive(false);
                }
            }
        }

        public void OnPointerDown() 
        {
            if (_rect == null) return;

            _rect.SetAsLastSibling();
        }

    #endregion  // Event handling
    }
}