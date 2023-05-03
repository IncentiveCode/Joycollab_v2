/// <summary>
/// [PC Web]
/// 사용자 Login 화면
/// @author         : HJ Lee
/// @last update    : 2023. 04. 22
/// @version        : 0.8
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
        // [SerializeField] private Button _btnWorld;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
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
                if (_inputPw.isFocused) CheckParams();
            });
            _inputDomain.onSubmit.AddListener((value) => {
                MoveToSubLoginAsync(value).Forget();
            });


            // set button listener
            _btnSignIn.onClick.AddListener(() => CheckParams());
            _btnResetPw.onClick.AddListener(() => Debug.Log("TODO. LoginManager 생성 후, Reset 화면으로 이동"));
            _btnSignUp.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "text.회원가입 안내", currentLocale),
                    () => Debug.Log("TODO. LoginManager 생성 후, 확인 누르면 약관 동의로 이동.")
                );
            });

            _btnNext.onClick.AddListener(() => MoveToSubLoginAsync(_inputDomain.text).Forget());
            _btnVersion.onClick.AddListener(() => Debug.Log("TODO. LoginManager 생성 후, 패치 노트 화면으로 이동"));

            _btnSample.onClick.AddListener(() => {
                SceneLoader.Load(eScenes.Sample);
            });
            _btnSample.gameObject.SetActive(URL.DEV);

            /**
            _btnWorld.onClick.AddListener(() => {
        #if UNITY_WEBGL && !UNITY_EDITOR
                JsLib.Redirection(URL.WORLD_INDEX);
        #else
                Debug.Log("TODO. LoginManager 생성 후, World Login 화면으로 이동.");
        #endif
            });
            _btnWorld.gameObject.SetActive(URL.DEV);
             */
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing().Forget();
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

        private void CheckParams() 
        {
            if (string.IsNullOrEmpty(_inputId.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "alert.이메일 없음", currentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPw.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "alert.비밀번호 없음", currentLocale)
                );
                return;
            }

            LoginAsync(_inputId.text, _inputPw.text).Forget();
        }

        private async UniTaskVoid LoginAsync(string id, string pw) 
        {
            WWWForm form = new WWWForm();
            form.AddField(NetworkTask.GRANT_TYPE, NetworkTask.GRANT_TYPE_PW);
            form.AddField(NetworkTask.PASSWORD, pw);
            form.AddField(NetworkTask.REFRESH_TOKEN, string.Empty);
            form.AddField(NetworkTask.SCOPE, NetworkTask.SCOPE_ADM);
            form.AddField(NetworkTask.USERNAME, id);

            PsResponse<ResToken> res = await NetworkTask.PostAsync<ResToken>(URL.REQUEST_TOKEN, form, string.Empty, NetworkTask.BASIC_TOKEN);
            if (string.IsNullOrEmpty(res.message)) 
            {
                JsLib.SetCookie(Key.TOGGLE_ID_SAVED, _toggleRemember.isOn ? S.TRUE : S.FALSE);
                JsLib.SetCookie(Key.SAVED_LOGIN_ID, _toggleRemember.isOn ? _inputId.text : string.Empty);
                JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
                JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

                R.singleton.ID = id;
                R.singleton.TokenInfo = res.data;

                // TODO. LoginManager 생성 후, SpaceSelector 추가
                // LoginViewManager.Instance.ShowSpaceSelector();
            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "alert.로그인 실패", currentLocale)
                );
            }
        }

        private async UniTaskVoid MoveToSubLoginAsync(string domain)
        {
            string url = string.Format(URL.CHECK_DOMAIN, domain);
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET);
            if (string.IsNullOrEmpty(res.message)) 
            {

            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "alert.도메인 탐색 실패", currentLocale)
                );
            }
        }

    #endregion  // login
    }
}