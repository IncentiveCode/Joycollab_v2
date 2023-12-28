/// <summary>
/// [world - Map]
/// Floating bar 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 12. 21 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 12. 14) : FloatingBarW class 에서 분리 (지금은 FloatingBarInSquare)
///     v0.2 (2023. 12. 21) : user list 는 map 에서 볼 수 없도록 조치, repository event 도 수신하지 않게끔 수정.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class FloatingBarInMap : MonoBehaviour, iRepositoryObserver
    {
        private const string TAG = "FloatingBarInMap"; 

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

        [Header("button")]
        [SerializeField] private Button _btnProfile;
        [SerializeField] private Button _btnAlarm;
        [SerializeField] private Button _btnChat;
        [SerializeField] private Button _btnSettings;
        [SerializeField] private Button _btnSignOut;

        // local variables
        private string myPhoto;
        private int alarmCount, chatCount;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnProfile.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_MyProfile);
            });
            _btnAlarm.onClick.AddListener(() => {
                OnPointerDown();
                WindowManager.singleton.Push(S.WorldScene_AlarmList);
            });
            _btnChat.onClick.AddListener(() => {
                OnPointerDown();
                string url = string.Format(URL.CHAT_LINK, R.singleton.memberSeq, R.singleton.Region);
                JsLib.OpenChat(url);
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


            // set local variables
            loader = _imgProfile.GetComponent<ImageLoader>();
            myPhoto = string.Empty;
            alarmCount = chatCount = -1;


            // register event 
            if (R.singleton != null)
            {
                R.singleton.RegisterObserver(this, eStorageKey.MemberInfo);
                R.singleton.RegisterObserver(this, eStorageKey.Alarm);
                R.singleton.RegisterObserver(this, eStorageKey.Chat);
                R.singleton.RegisterObserver(this, eStorageKey.WindowRefresh);
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
            }
        } 

        private void OnDestroy() 
        {
            myPhoto = string.Empty;
            alarmCount = chatCount = -1;

            // unregister event
            if (R.singleton != null)
            {
                R.singleton.UnregisterObserver(this, eStorageKey.MemberInfo);
                R.singleton.UnregisterObserver(this, eStorageKey.Alarm);
                R.singleton.UnregisterObserver(this, eStorageKey.Chat);
                R.singleton.UnregisterObserver(this, eStorageKey.WindowRefresh);
            }
        }

    #endregion  // Unity functions


    #region Event handling

        public void UpdateInfo(eStorageKey key) 
        {
            // Debug.Log($"{TAG} | UpdateInfo() call. key : {key}");
            switch (key) 
            {
                case eStorageKey.MemberInfo :
                    // Debug.Log($"{TAG} | UpdateInfo (UserPhoto) - photo : {myPhoto}, photo in R : {R.singleton.myPhoto}");
                    if (!myPhoto.Equals(R.singleton.myPhoto)) 
                    {
                        myPhoto = R.singleton.myPhoto;

                        string url = $"{URL.SERVER_PATH}{myPhoto}";
                        // Debug.Log($"{TAG} | photo url : {url}");
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

                default :
                    Debug.Log($"{TAG} | 여기에서 사용하지 않는 항목. key : {key}");
                    break;
            }
        }

        private void RefreshPanel() 
        {
            // member, guest check
            bool isGuest = R.singleton.myMemberType.Equals(S.GUEST);

            // common setting
            _btnProfile.interactable = !isGuest;
            _btnAlarm.gameObject.SetActive(! isGuest);
            _btnChat.gameObject.SetActive(! isGuest);
            _btnSettings.gameObject.SetActive(! isGuest);
            _btnSignOut.gameObject.SetActive(true);
        }

        public void OnPointerDown() 
        {
            if (_rect == null) return;

            _rect.SetAsLastSibling();
        }

    #endregion  // Event handling
    }
}