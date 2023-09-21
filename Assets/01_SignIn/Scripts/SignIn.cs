/// <summary>
/// 사용자 Sign In 화면
/// @author         : HJ Lee
/// @last update    : 2023. 09. 21.
/// @version        : 1.4
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
///     v1.0 (2023. 08. 04) : InputSubmitDetector -> TmpInputField 로 변경. 
///     v1.1 (2023. 08. 16) : tab key 처리 수정.
///     v1.2 (2023. 08. 23) : file name, class name 변경. (Login -> SignIn)
///     v1.3 (2023. 08. 28) : SignInW, v1 에서 만들었던 world login 과 통합.
///     v1.4 (2023. 09. 21) : world -> joycollab 으로 돌아가는 버튼과 테스트 버튼 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class SignIn : FixedView
    {
        private const string TAG = "SignIn";

        [Header("Module")]
        [SerializeField] private SignInModule _module;

        [Header("InputField")] 
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private TMP_InputField _inputPw;
        [SerializeField] private TMP_InputField _inputDomain;

        [Header("Toggle")]
        [SerializeField] private Toggle _toggleRemember;
        [SerializeField] private Toggle _toggleGoToCenter;

        [Header("Button")] 
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Button _btnSignUp;
        [SerializeField] private Button _btnGuest;
        [SerializeField] private Button _btnResetPw;
        [SerializeField] private Button _btnNext;
        [SerializeField] private Button _btnVersion;
        [SerializeField] private Button _btnSample;
        [SerializeField] private Button _btnWorld;
        [SerializeField] private Button _btnJoycollab;
        [SerializeField] private Button _btnTest;


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

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();


            // check scene opt
            if (! isOffice && ! isWorld && ! isMobile) 
            {
                Debug.Log($"{TAG} | view tag 설정이 잘못 되었습니다. office : {isOffice}, world : {isWorld}, mobile : {isMobile}");
                return;
            }


            // set inputfield listener
            _inputId.onSubmit.AddListener((value) => {
                if (_inputId.isFocused) _inputPw.Select();
            });
            _inputPw.onSubmit.AddListener((value) => {
                if (_inputPw.isFocused) SignInAsync().Forget();
            });
            if (isOffice)
            {
                _inputDomain.onSubmit.AddListener((value) => {
                    MoveToSubSignInAsync(value).Forget();
                });
            }


            // set button listener
            _btnSignIn.onClick.AddListener(() => SignInAsync().Forget());
            _btnSignUp.onClick.AddListener(() => {
                if (isOffice)
                {
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "회원가입 안내", R.singleton.CurrentLocale),
                        () => Debug.Log("TODO. 확인 누르면 약관 동의로 이동.")
                    );
                }
                else if (isWorld) 
                {
                    ViewManager.singleton.Push(S.WorldScene_Agreement);
                }
            });
            _btnResetPw.onClick.AddListener(() => ViewManager.singleton.Push(S.WorldScene_Reset));

            if (isOffice) 
            {
                _btnNext.onClick.AddListener(() => MoveToSubSignInAsync(_inputDomain.text).Forget());
                _btnVersion.onClick.AddListener(() => Debug.Log("TODO. ViewManager 생성 후, 패치 노트 화면으로 이동"));

                _btnSample.onClick.AddListener(() => {
                    SystemManager.singleton.SetFontOpt(1);
                    SceneLoader.Load(eScenes.Sample);
                });
                _btnSample.gameObject.SetActive(URL.DEV);

                _btnWorld.onClick.AddListener(() => {
                #if UNITY_WEBGL && !UNITY_EDITOR
                    JsLib.Redirection(URL.WORLD_INDEX);
                #else
                    SystemManager.singleton.SetFontOpt(1);
                    SceneLoader.Load(eScenes.World);
                #endif
                });
                _btnWorld.gameObject.SetActive(URL.DEV);
            }
            else if (isWorld) 
            {
                _btnGuest.onClick.AddListener(() => ViewManager.singleton.Push(S.WorldScene_Guest));
                _btnJoycollab.onClick.AddListener(() => {
                #if UNITY_WEBGL && !UNITY_EDITOR
                    JsLib.Redirection(URL.INDEX);
                #else
                    SystemManager.singleton.SetFontOpt(1);
                    SceneLoader.Load(eScenes.SignIn);
                #endif
                });
                if (_btnTest != null) 
                {
                    _btnTest.onClick.AddListener(() => {
                        _inputId.text = "hjlee@pitchsolution.co.kr";
                        _inputPw.text = "Qwer!234";
                        SignInAsync().Forget();
                    });
                }
            }
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();

            _inputId.gameObject.SetActive(true);
            _inputPw.gameObject.SetActive(true);
            if (isOffice) 
            {
                _inputDomain.gameObject.SetActive(true);
            }

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            _inputId.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
            if (isOffice)
            {
                _inputDomain.gameObject.SetActive(false);
            }
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            string isSaved = string.Empty;
            string savedId = string.Empty;

            if (isOffice) 
            {
                isSaved = JsLib.GetCookie(Key.TOGGLE_ID_SAVED);
                savedId = JsLib.GetCookie(Key.SAVED_SIGN_IN_ID);
            }
            else if (isWorld) 
            {
                _toggleGoToCenter.isOn = JsLib.GetCookie(Key.TOGGLE_GO_TO_CENTER).Equals(S.TRUE);
                isSaved = JsLib.GetCookie(Key.TOGGLE_WORLD_ID_SAVED);
                savedId = JsLib.GetCookie(Key.SAVED_WORLD_ID);
            }

            bool isOn = isSaved.Equals(S.TRUE);
            _toggleRemember.isOn = isOn;

            _inputId.text = isOn ? savedId : string.Empty;
            _inputPw.text = string.Empty;
            if (isOffice) 
            {
                _inputDomain.text = string.Empty;
            }

            await UniTask.Yield();
            return 0;    
        }

    #endregion  // event handling


    #region Sign-in

        private async UniTaskVoid SignInAsync() 
        {
            if (string.IsNullOrEmpty(_inputId.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이메일 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPw.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            string id = _inputId.text;
            string pw = _inputPw.text;

            PsResponse<ResToken> res = await _module.SignInAsync(id, pw);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그인 실패", R.singleton.CurrentLocale)
                );
                return;
            }

            // save info
            R.singleton.ID = id;
            R.singleton.TokenInfo = res.data;

            if (isOffice)
            {
                JsLib.SetCookie(Key.TOGGLE_ID_SAVED, _toggleRemember.isOn ? S.TRUE : S.FALSE);
                JsLib.SetCookie(Key.SAVED_SIGN_IN_ID, _toggleRemember.isOn ? _inputId.text : string.Empty);
                JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
                JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

                // TODO. Add space selecter
                Debug.Log($"{TAG} | office selector 이관 예정.");
            }
            else if (isWorld) 
            {
                JsLib.SetCookie(Key.TOGGLE_WORLD_ID_SAVED, _toggleRemember.isOn ? S.TRUE : S.FALSE);
                JsLib.SetCookie(Key.SAVED_WORLD_ID, _toggleRemember.isOn ? _inputId.text : string.Empty);
                JsLib.SetCookie(Key.TOGGLE_GO_TO_CENTER, _toggleGoToCenter.isOn ? S.TRUE : S.FALSE);
                JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
                JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

                WorldSignInAsync().Forget();
            }
        }

        private async UniTaskVoid WorldSignInAsync() 
        {
            PsResponse<WorkspaceInfo> res = await _module.WorldSignInAsync();
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            // workspace 정보 설정. 
            Debug.Log($"{TAG} | member seq : {res.data.seq}, workspace seq : {res.data.workspace.seq}");
            JsLib.SetCookie(Key.WORKSPACE_SEQ, res.data.workspace.seq.ToString());
            JsLib.SetCookie(Key.MEMBER_SEQ, res.data.seq.ToString());

            R.singleton.workspaceSeq = res.data.workspace.seq;
            R.singleton.memberSeq = res.data.seq;
            R.singleton.domainName = res.data.workspace.domain;
            R.singleton.uiType = res.data.uiType;

            // world avatar 정보 설정.
            WorldAvatarInfo info = new WorldAvatarInfo();
            info.seq = res.data.seq;
            info.nickNm = res.data.nickNm;
            info.photo = res.data.photo;
            WorldAvatar.localPlayerInfo = info;
            WorldChatView.localPlayerInfo = info;

            // 사용자 정보 로드 
            string resMyInfo = await _module.GetMyInfoAsync();
            if (! string.IsNullOrEmpty(resMyInfo)) 
            {
                PopupBuilder.singleton.OpenAlert(resMyInfo);
                return;
            }

            if (_toggleGoToCenter.isOn)
            {
                Debug.Log($"{TAG} | Square scene 으로 이동.");
                SceneLoader.Load(eScenes.Square);
            }
            else
            {
                Debug.Log($"{TAG} | Map scene 으로 이동.");
                SceneLoader.Load(eScenes.Map);
            }
        }

        private async UniTaskVoid MoveToSubSignInAsync(string domain)
        {
            string url = string.Format(URL.CHECK_DOMAIN, domain);
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "도메인 탐색 실패", R.singleton.CurrentLocale)
                );
                return;
            }

            R.singleton.AddParam(Key.WORKSPACE_SEQ, res.data.seq.ToString()); 
            R.singleton.AddParam(Key.WORKSPACE_NAME, res.data.nm);
            R.singleton.AddParam(Key.WORKSPACE_LOGO, res.data.logo);
            R.singleton.AddParam(Key.WORKSPACE_END_DATE, res.data.edtm);

        #if UNITY_WEBGL && !UNITY_EDITOR
            string redirectUrl = string.Format(URL.SUB_INDEX, domain);
            JsLib.Redirection(redirectUrl);
        #else
            JsLib.ClearTokenCookie();
            ViewManager.singleton.Push(S.SignInScene_SubSignIn);
        #endif
        }

    #endregion  // login
    }
}