/// <summary>
/// [mobile]
/// My Page 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 16
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 16) : 최초 생성.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class MyPageM : FixedView
    {
        [Header("text")]
        [SerializeField] private TMP_Text _txtMemberType;
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtDesc;

        [Header("button")]
        [SerializeField] private Button _btnBack; 
        [SerializeField] private Button _btnMyInfo;
        [SerializeField] private Button _btnAttendance;
        [SerializeField] private Button _btnMyWorkspace;
        [SerializeField] private Button _btnSettings;
        [SerializeField] private Button _btnSendFeedback;
        [SerializeField] private Button _btnSystemNotice;
        [SerializeField] private Button _btnTutorial;
        [SerializeField] private Button _btnLogout;

        // local variables
        private WebviewController ctrl;

    
    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        private void OnDestroy() 
        {
            if (MobileEvents.singleton != null) 
            {
                MobileEvents.singleton.OnBackButtonProcess -= BackButtonProcess;
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_MyPage;


            // set button listener
            _btnBack.onClick.AddListener(() => BackProcess());
            _btnMyInfo.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_MyInfo));
            _btnAttendance.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Attendance));
            _btnMyWorkspace.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_MyWorkspace));
            _btnSettings.onClick.AddListener(() => {
                // ViewManager.singleton.Push(S.MobileScene_MyWorkspace);
                PopupBuilder.singleton.OpenAlert("정리 중 입니다.");
            });
            _btnSendFeedback.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Feedback));
            _btnSystemNotice.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_SystemNotice));
            _btnTutorial.onClick.AddListener(() => {
                string url = string.Format(URL.TUTORIAL_PATH, R.singleton.isKorean ? string.Empty : "_EN");
        #if UNITY_ANDROID || UNITY_IOS 
                ctrl = WebviewBuilder.singleton.Build();
                ctrl.ShowMobileWebview(url);
        #else 
                ctrl = null;
                Application.OpenURL(url);
        #endif
            });
            _btnLogout.onClick.AddListener(() => Logout());
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region private functions

        private void Logout() 
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            PopupBuilder.singleton.OpenConfirm(
                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그아웃 질문", currentLocale),
                () => LogoutProcess()
            );
        }

        private void LogoutProcess() 
        {
            R.singleton.Clear();

            // TODO. 기타 작업 추가 (FCM logout, XMPP logout, PingManager stop, and etc)

            ViewManager.singleton.Push(S.MobileScene_Login);
        }

    #endregion


    #region event handling 

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // refresh
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            switch (R.singleton.myMemberType) 
            {
                case S.MANAGER :
                    _txtMemberType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "멤버타입 매니저", currentLocale);
                    break;

                case S.ADMIN :
                    _txtMemberType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "멤버타입 관리자", currentLocale);
                    break;

                case S.OWNER :
                    _txtMemberType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "멤버타입 소유자", currentLocale);
                    break;

                case S.COLLABORATOR :
                    _txtMemberType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "멤버타입 협업자", currentLocale);
                    break;

                case S.GUEST :
                    _txtMemberType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "멤버타입 게스트", currentLocale);
                    break;

                case S.USER :
                default :
                    _txtMemberType.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "멤버타입 일반사용자", currentLocale);
                    break;
            }

            _txtName.text = R.singleton.myName;
            _txtDesc.text = string.Format("{0} / {1}", R.singleton.mySpaceName, R.singleton.myGrade);

            await UniTask.Yield();
            return 0;
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            if (PopupBuilder.singleton.GetPopupCount() > 0)
            {
                PopupBuilder.singleton.RequestClear();
            }
            else if (WebviewBuilder.singleton.Active()) 
            {
                ctrl.GoBack();
            }
            else 
            {
                BackProcess();
            }
        }

        private void BackProcess() 
        {
            ViewManager.singleton.Pop();
        }

    #endregion  // event handling 
    }
}