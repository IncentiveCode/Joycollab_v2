/// <summary>
/// [PC Web]
/// 사용자 Login 화면
/// @author         : HJ Lee
/// @last update    : 2023. 05. 10
/// @version        : 0.9
/// @update
///     v0.1 : UI Canvas 최적화 (static canvas, active canvas 분리)
///     v0.2 : Tab key 로 input field 이동할 수 있게 수정.
///     v0.3 (2023. 02. 13) : Unitask 적용.
///     v0.4 (2023. 02. 15) : world 연결 버튼 추가. (개발 서버에만 적용).
///     v0.5 (2023. 03. 06) : 수정한 jslib 적용
///     v0.6 (2023. 04. 04) : Joycollab.v2 package 적용. R class 실험. 
///                 Strings class 에 몰아넣은 문자열들도 필요한 부분에서 사용할 수 있도록 분리. (S, Key, NetworkTask class, and etc) 
///     v0.7 (2023. 04. 14) : Popup Builder 적용
///     v0.8 (2023. 04. 22) : FixedView 적용
///     v0.9 (2023. 05. 10) : LoginModule 분리 및 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class Login : FixedView
    {
        [Header("Module")]
        [SerializeField] private LoginModule loginModule;

        [Header("InputField")] 
        [SerializeField] private InputSubmitDetector _inputId;
        [SerializeField] private InputSubmitDetector _inputPw;
        [SerializeField] private InputSubmitDetector _inputDomain;

        [Header("Button, Toggle")] 
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Toggle _toggleRemember;
        [SerializeField] private Button _btnResetPw;
        [SerializeField] private Button _btnSignUp;
        [SerializeField] private Button _btnNext;
        [SerializeField] private Button _btnVersion;
        [SerializeField] private Button _btnSample;
        [SerializeField] private Button _btnWorld;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

        private void Update() 
        {
            // ctrl-c + ctrl-v
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
            {
                if (Input.GetKeyDown(KeyCode.V)) 
                {
                    if (_inputId.isFocused) _inputId.text = ClipBoard.singleton.contents;
                    if (_inputPw.isFocused) _inputPw.text = ClipBoard.singleton.contents;
                    if (_inputDomain.isFocused) _inputDomain.text = ClipBoard.singleton.contents;
                }
            }

            // tab key process
            if (Input.GetKeyDown(KeyCode.Tab)) 
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
                {
                    if (_inputId.isFocused) return;
                    else if (_inputPw.isFocused) _inputId.Select();
                }
                else 
                {
                    if (_inputId.isFocused) _inputPw.Select();
                    else if (_inputPw.isFocused) return;
                }
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();


            // set inputfield listener
            _inputId.onSubmit.AddListener((value) => {
                if (_inputId.isFocused) _inputPw.Select();
            });
            _inputPw.onSubmit.AddListener((value) => {
                if (_inputPw.isFocused) Request().Forget();
            });
            _inputDomain.onSubmit.AddListener((value) => {
                MoveToSubLoginAsync(value).Forget();
            });


            // set button listener
            _btnSignIn.onClick.AddListener(() => Request().Forget());
            _btnResetPw.onClick.AddListener(() => Debug.Log("TODO. LoginManager 생성 후, Reset 화면으로 이동"));
            _btnSignUp.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "text.회원가입 안내", currentLocale),
                    () => Debug.Log("TODO. LoginManager 생성 후, 확인 누르면 약관 동의로 이동.")
                );
            });

            _btnNext.onClick.AddListener(() => MoveToSubLoginAsync(_inputDomain.text).Forget());
            _btnVersion.onClick.AddListener(() => Debug.Log("TODO. ViewManager 생성 후, 패치 노트 화면으로 이동"));

            _btnSample.onClick.AddListener(() => {
                SceneLoader.Load(eScenes.Sample);
            });
            _btnSample.gameObject.SetActive(URL.DEV);

            _btnWorld.onClick.AddListener(() => {
        #if UNITY_WEBGL && !UNITY_EDITOR
                JsLib.Redirection(URL.WORLD_INDEX);
        #else
                Debug.Log("TODO. ViewManager 생성 후, World Login 화면으로 이동.");
        #endif
            });
            _btnWorld.gameObject.SetActive(URL.DEV);
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            string toggleValue = JsLib.GetCookie(Key.TOGGLE_ID_SAVED);
            bool isOn = toggleValue.Equals(S.TRUE);
            _toggleRemember.isOn = isOn;

            _inputId.text = isOn ? JsLib.GetCookie(Key.SAVED_LOGIN_ID) : string.Empty;
            _inputPw.text = string.Empty;
            _inputDomain.text = string.Empty;

            await UniTask.Yield();
            return 0;    
        }

    #endregion  // event handling


    #region login

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
                JsLib.SetCookie(Key.TOGGLE_ID_SAVED, _toggleRemember.isOn ? S.TRUE : S.FALSE);
                JsLib.SetCookie(Key.SAVED_LOGIN_ID, _toggleRemember.isOn ? _inputId.text : string.Empty);
                JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
                JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

                R.singleton.ID = id;
                R.singleton.TokenInfo = res.data;

                // TODO. LoginManager 생성 후, SpaceSelector 추가
            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그인 실패", currentLocale)
                );
            }
        }

        private async UniTaskVoid MoveToSubLoginAsync(string domain)
        {
            string url = string.Format(URL.CHECK_DOMAIN, domain);
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET);
            if (string.IsNullOrEmpty(res.message)) 
            {
                R.singleton.AddParam(Key.WORKSPACE_SEQ, res.data.seq.ToString()); 
                R.singleton.AddParam(Key.WORKSPACE_NAME, res.data.nm);
                R.singleton.AddParam(Key.WORKSPACE_LOGO, res.data.logo);
                R.singleton.AddParam(Key.WORKSPACE_END_DATE, res.data.edtm);

        #if UNITY_WEBGL && !UNITY_EDITOR
                string url = string.Format(URL.SUB_INDEX, domain);
                JsLib.Redirection(url);
        #else
                ViewManager.singleton.Push(S.LoginScene_SubLogin);
        #endif
            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "도메인 탐색 실패", currentLocale)
                );
            }
        }

    #endregion  // login
    }
}