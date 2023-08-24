/// <summary>
/// [PC Web]
/// 특정 회사 사용자 Login 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 23
/// @version        : 0.9
/// @update
///     v0.1 : UI Canvas 최적화 (static canvas, active canvas 분리)
///     v0.2 (2023. 02. 02) : Unitask 적용.
///     v0.3 (2023. 03. 06) : 수정한 jslib 적용, 외부에서 받아온 Texture2D 제거 스크립트(OnHide()) 추가
///     v0.4 (2023. 04. 03) : Joycollab.v2 package 적용. R class 실험.
///     v0.5 (2023. 04. 14) : Popup Builder 적용
///                 Strings class 에 몰아넣은 문자열들도 필요한 부분에서 사용할 수 있도록 분리. (S, Key, NetworkTask class, and etc) 
///     v0.6 (2023. 05. 10) : FixedView 적용
///     v0.7 (2023. 08. 09) : InputSubmitDetector -> TmpInputField 로 변경. 
///     v0.8 (2023. 08. 16) : tab key 처리 수정.
///     v0.9 (2023. 08. 23) : file name, class name 변경. (SubLogin -> SubSignIn)
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class SubSignIn : FixedView
    {
        private const string TAG = "SubSignIn";


        [Header("Module")]
        [SerializeField] private SignInModule _module;

        [Header("RawImage controller")]
        [SerializeField] private RawImage _imgOfficeLogo;

        [Header("Text")]
        [SerializeField] private Text _txtGreetings;

        [Header("InputField")] 
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private TMP_InputField _inputPw;

        [Header("Button, Toggle")] 
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Toggle _toggleRemember;
        [SerializeField] private Button _btnResetPw;
        [SerializeField] private Button _btnSignUp;
        [SerializeField] private Button _btnGuest;
        [SerializeField] private Button _btnVersion;

        // local vairables
        private ImageLoader imageLoader;

        private bool isInvite;
        private bool isExpire;

        
    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

        #if UNITY_EDITOR
        private void Update() 
        {
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
        #endif

        private void OnDestroy() 
        {
            _imgOfficeLogo.texture = null;
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


            // set button listener
            _btnBack.onClick.AddListener(() => {
        #if UNITY_WEBGL && !UNITY_EDITOR
                JsLib.Redirection(URL.INDEX);
        #else
                R.singleton.ClearParamValues();
                ViewManager.singleton.PopAll();
                ViewManager.singleton.Push(S.SignInScene_SignIn);
        #endif
            });
            _btnSignIn.onClick.AddListener(() => Request().Forget());
            _btnResetPw.onClick.AddListener(() => Debug.Log("TODO. LoginManager 생성 후, Reset 화면으로 이동"));
            _btnSignUp.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "회원가입 안내", currentLocale),
                    () => Debug.Log("TODO. LoginManager 생성 후, 확인 누르면 약관 동의로 이동.")
                );
            });
            _btnGuest.onClick.AddListener(() => {
                if (isExpire) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenAlert(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "만료된 스페이스 안내", currentLocale)
                    );
                }
                else 
                {
                    ViewManager.singleton.Push(S.SignInScene_Guest);
                }
            });
            _btnVersion.onClick.AddListener(() => ViewManager.singleton.Push(S.SignInScene_PatchNote));


            // set local variables
            // rectLogo = _imgOfficeLogo.GetComponent<RectTransform>();   
            imageLoader = _imgOfficeLogo.GetComponent<ImageLoader>();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();

            _inputId.gameObject.SetActive(true);
            _inputPw.gameObject.SetActive(true);

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            _imgOfficeLogo.texture = null;

            _inputId.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            PsResponse<ResCheckToken> res = await _module.CheckTokenAsync();
            if (res == null) 
            {
                OpenView();
                return -1;
            }

            if (! string.IsNullOrEmpty(res.message)) 
            {
                Debug.LogError($"{TAG} | Refresh(), Token check failure - {res.message}");

                JsLib.SetCookie(Key.TOKEN_TYPE, string.Empty);
                JsLib.SetCookie(Key.ACCESS_TOKEN, string.Empty);

                OpenView();
                return -2;
            }
            
            if (string.IsNullOrEmpty(res.message)) 
            {
                Debug.Log($"{TAG} | Refresh(), Token check success.");

                bool isGuest = res.data.account_role.Equals(S.GUEST);
                bool isActive = res.data.active;
                string workspaceSeq = JsLib.GetCookie(Key.WORKSPACE_SEQ);
                string paramWorkspaceSeq = R.singleton.GetParam(Key.WORKSPACE_SEQ);

                // - case 'Guest'
                // if (isGuest && isActive && workspaceSeq.Equals(paramWorkspaceSeq)) 
                if (isGuest) 
                {
                    if (workspaceSeq.Equals(paramWorkspaceSeq)) 
                    {
                        _btnGuest.onClick.Invoke();
                        return 1;
                    }
                }

                // - case 'Member'
                else
                {
                    EnterWorkspace(res.extra,
                        string.IsNullOrEmpty(paramWorkspaceSeq) ? workspaceSeq : paramWorkspaceSeq
                    ).Forget();
                    return 1;
                }
            }

            OpenView();
            return 0;
        }

        private void PostCheckToken() 
        {
            // get office logo
            string logoPath = R.singleton.GetParam(Key.WORKSPACE_LOGO);
            string url = URL.SERVER_PATH + logoPath;
            imageLoader.LoadOfficeLogo(url).Forget();

            // set info
            string officeName = R.singleton.GetParam(Key.WORKSPACE_NAME);
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string greetings = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "환영인사2", currentLocale);
            _txtGreetings.text = string.Format(greetings, officeName);

            string toggleValue = JsLib.GetCookie(Key.TOGGLE_SUB_ID_SAVED);
            bool isOn = toggleValue.Equals(S.TRUE);
            _toggleRemember.isOn = isOn;

            _inputId.text = isOn ? JsLib.GetCookie(Key.SAVED_SUB_LOGIN_ID) : string.Empty;
            _inputPw.text = string.Empty;


            // check expire
            string edtm = R.singleton.GetParam(Key.WORKSPACE_END_DATE);
            System.DateTime dEdtm = System.Convert.ToDateTime(edtm);
            System.TimeSpan diff = dEdtm - System.DateTime.Now;
            isExpire = diff.TotalSeconds <= 0;


            // check invite
            isInvite = R.singleton.GetParam(Key.INVITED).Equals(S.TRUE);
            string email = R.singleton.GetParam(Key.EMAIL);
            if (! string.IsNullOrEmpty(email)) _inputId.text = email;
        }

        private void OpenView() 
        {
            R.singleton.Clear();
            Tmp.singleton.Clear();
            PostCheckToken();
        }

        private async UniTaskVoid EnterWorkspace(string token, string workspaceSeq) 
        {
            await UniTask.Yield();
            return;
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

            PsResponse<ResToken> res = await _module.LoginAsync(id, pw);
            if (string.IsNullOrEmpty(res.message)) 
            {
                JsLib.SetCookie(Key.TOGGLE_ID_SAVED, _toggleRemember.isOn ? S.TRUE : S.FALSE);
                JsLib.SetCookie(Key.SAVED_LOGIN_ID, _toggleRemember.isOn ? _inputId.text : string.Empty);
                JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
                JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

                R.singleton.ID = id;
                R.singleton.TokenInfo = res.data;

                string token = $"{res.data.token_type} {res.data.access_token}";
                string seq = R.singleton.GetParam(Key.WORKSPACE_SEQ);
                EnterWorkspace(token, seq).Forget();
            }
            else 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그인 실패", currentLocale)
                );
            }
        }

    #endregion  // login
    }
}