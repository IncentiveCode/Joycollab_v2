/// <summary>
/// [mobile]
/// 로그인 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 12
/// @version        : 0.7
/// @update
///     v0.1 : 최초 생성.
///     v0.2 : 새로운 디자인 적용.
///     v0.3 (2022. 05. 25) : 디자인 수정안 적용. input field clear button 추가.
///     v0.4 (2023. 03. 20) : FixedView 실험, UniTask 적용, UI canvas 최적화.
///     v0.5 (2023. 03. 30) : Slide Popup 적용.
///     v0.6 (2023. 03. 31) : Popup Builder 적용.
///     v0.7 (2023. 06. 12) : Legacy InputField 를 TMP_InputField 로 변경, softkeyboard 출력상태에서 back button 입력시 내용 사라지는 오류 수정.
/// </summary>

using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class LoginM : FixedView
    {
        [Header("Module")]
        [SerializeField] private LoginModule loginModule;

        [Header("Input e-mail")]
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private Button _btnClearId;

        [Header("Input password")]
        [SerializeField] private TMP_InputField _inputPw;
        [SerializeField] private Button _btnClearPw;

        [Header("buttons")]
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Button _btnSignUp;
        [SerializeField] private Button _btnResetPw;
        [SerializeField] private Button _btnVersion;
        [SerializeField] private Button _btnTest;
        [SerializeField] private Button _btnProgressTest;

        // local variables
        private List<ResWorkspaceInfo> workspaces;
        private StringBuilder stringBuilder;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        #if UNITY_ANDROID
        private void Update() 
        {
            if (AndroidSelectCallback.ViewID == viewID && AndroidSelectCallback.isUpdated) 
            {
                if (visibleState != eVisibleState.Appeared) return;

                int index = AndroidSelectCallback.SelectedIndex;
                R.singleton.domainName = workspaces[index].workspace.domain;
                R.singleton.workspaceSeq = workspaces[index].workspace.seq;
                R.singleton.memberSeq = workspaces[index].seq;
                R.singleton.uiType = workspaces[index].uiType;

                ViewManager.singleton.Push(S.MobileScene_LoadInfo);
                ViewManager.singleton.PopAll();

                AndroidSelectCallback.isUpdated = false;
            }
        }
        #endif

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
            viewID = ID.MobileScene_Login;

            // set 'e-mail' inputfield listener
            SetInputFieldListener(_inputId);
            _inputId.onValueChanged.AddListener((value) => {
                _btnClearId.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputId.onSubmit.AddListener((value) => _inputPw.Select());
            _btnClearId.onClick.AddListener(() => {
                _inputId.text = string.Empty;
                _inputId.Select();
            });


            // set 'password' inputfield listener
            SetInputFieldListener(_inputPw);
            _inputPw.onValueChanged.AddListener((value) => {
                _btnClearPw.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _btnClearPw.onClick.AddListener(() => {
                _inputPw.text = string.Empty;
                _inputPw.Select();
            });


            // set button listener
            _btnSignIn.onClick.AddListener(() => Request().Forget());
            _btnSignUp.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "회원가입 안내", currentLocale),
                    () => Debug.Log("준비 중 입니다.")
                );
            });
            _btnResetPw.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Reset));
            _btnVersion.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_PatchNote));

            _btnTest.onClick.AddListener(() => {
                _inputId.text = "hjlee@pitchsolution.co.kr";
                _inputPw.text = "123123123";
                Request().Forget();
            });
            _btnTest.gameObject.SetActive(URL.DEV);

            _btnProgressTest.onClick.AddListener(() => {
                ProgressBuilder.singleton.OpenProgress(2f);
            });
            _btnProgressTest.gameObject.SetActive(URL.DEV);

            // set local variables
            workspaces = new List<ResWorkspaceInfo>();
            workspaces.Clear();

            stringBuilder = new StringBuilder();
            stringBuilder.Clear();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing();
        }

    #endregion  // FixedView functions


    #region Login functions

        private async UniTaskVoid Request() 
        {
            if (string.IsNullOrEmpty(_inputId.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이메일 없음", currentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPw.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 없음", currentLocale)
                );
                return;
            }

            string id = _inputId.text;
            string pw = _inputPw.text;

            PsResponse<ResToken> res = await loginModule.LoginAsync(id, pw);
            if (string.IsNullOrEmpty(res.message)) 
            {
                _inputId.interactable = _inputPw.interactable = false;

                R.singleton.ID = id;
                R.singleton.TokenInfo = res.data;

                GetWorkspaceList().Forget();
            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그인 실패", currentLocale)
                );
            }
        }

    #endregion  // Login functions


    #region workspace list 

        private async UniTaskVoid GetWorkspaceList() 
        {
            string token = R.singleton.token;
            string url = URL.WORKSPACE_LIST;

            PsResponse<ResWorkspaceList> res = await NetworkTask.RequestAsync<ResWorkspaceList>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message)) 
            {
                workspaces.Clear();
                stringBuilder.Clear();

                // ready
                for (int i = 0; i < res.data.list.Count; i++) 
                {
                    if (res.data.list[i].workspace.workspaceType.Equals(S.WORLD)) continue;
                    
                    if (res.data.list[i].useYn.Equals("Y")) 
                    {
                        workspaces.Add(res.data.list[i]);

                        if (stringBuilder.Length != 0) stringBuilder.Append(",");
                        stringBuilder.Append(res.data.list[i].workspace.nm);
                    }
                }

                // display
                int cnt = workspaces.Count;
                if (cnt == 0) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenAlert(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "모바일 스페이스 생성 안내", currentLocale),
                        () => {
                            R.singleton.Clear();
                            ViewManager.singleton.Push(S.MobileScene_Login);
                        }
                    );
                }
                else if (cnt == 1) 
                {
                    R.singleton.domainName = workspaces[0].workspace.domain;
                    R.singleton.workspaceSeq = workspaces[0].workspace.seq;
                    R.singleton.memberSeq = workspaces[0].seq;
                    R.singleton.uiType = workspaces[0].uiType;

                    ViewManager.singleton.Push(S.MobileScene_LoadInfo);
                }
                else 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string title = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "스페이스 선택", currentLocale);
					string temp = stringBuilder.ToString();
					string[] options = temp.Split(',');
                    PopupBuilder.singleton.OpenSlide(title, options, new string[] { }, viewID, false);
                }
            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "정보 갱신 실패", currentLocale)
                );
            }
        }

    #endregion  // workspace list


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            // MobileManager.singleton.ShowNavigation(false);

            // refresh
            _inputPw.text = string.Empty;
            _inputPw.interactable = true;
            _btnClearPw.gameObject.SetActive(false);

            string id = JsLib.GetCookie(Key.SAVED_LOGIN_ID);
            _inputId.text = id;
            _inputId.interactable = true;
            _btnClearId.gameObject.SetActive(! string.IsNullOrEmpty(id));

            // MobileManager.singleton.StopLoading();

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
            else 
            {
                BackProcess();
            }
        }

        private void BackProcess() 
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            PopupBuilder.singleton.OpenConfirm(
                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "앱 종료 확인", currentLocale),
                () => Application.Quit()
            );
        }

    #endregion  // event handling
    }
}