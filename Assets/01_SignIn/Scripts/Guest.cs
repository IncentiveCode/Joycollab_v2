/// <summary>
/// 게스트가 특정 회사에 들어가거나 월드에 진입하는 화면
/// @author         : HJ Lee
/// @last update    : 2023. 11. 20.
/// @version        : 0.3
/// @update
///     v0.1 (2023. 08. 23) : v1 에서 만들었던 GuestLogin 수정 후 적용.
///     v0.2 (2023. 11. 06) : world seqeuence 를 확인 후, world 진입할 수 있도록 연결.
///     v0.3 (2023. 11. 20) : 로그인/회원가입 버튼 주석처리, 뒤로가기 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2 
{
    public class Guest : FixedView
    {
        private const string TAG = "Guest";
        
        [Header("module")]
        [SerializeField] private SignInModule _module;

        [Header("guest photo, office logo")]
        [SerializeField] private Button _btnPhoto;
        [SerializeField] private RawImage _imgOfficeLogo;

        [Header("greetings")]
        [SerializeField] private Text _txtDesc;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputName;

        [Header("Button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnEnter;
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Button _btnSignUp;

        // local variables
        private ImageUploader uploader;
        private ImageLoader imageLoader;


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


            // check scene opt
            if (! isOffice && ! isWorld && ! isMobile) 
            {
                Debug.Log($"{TAG} | view tag 설정이 잘못 되었습니다. office : {isOffice}, world : {isWorld}, mobile : {isMobile}");
                return;
            }


            // set 'image uploader' & 'image loader'
            uploader = _btnPhoto.GetComponent<ImageUploader>();
            uploader.Init();

            if (isOffice)
            {
                imageLoader = _imgOfficeLogo.GetComponent<ImageLoader>();
            }


            // set input field listener
            _inputName.onSubmit.AddListener((value) => {
                if (_inputName.isFocused) GuestSignInAsync().Forget();
            });


            // set button listener
            if (_btnBack != null) 
            {
                _btnBack.onClick.AddListener(() => {
                    ViewManager.singleton.PopAll();
                    ViewManager.singleton.Push(
                        isWorld ? S.WorldScene_SignIn : S.SignInScene_SignIn
                    );
                });
            }
            _btnEnter.onClick.AddListener(() => {
                if (isOffice) 
                {
                    Debug.Log($"{TAG} | 남의 사무실로 로그인.");
                }
                else if (isWorld) 
                {
                    Debug.Log($"{TAG} | 광장으로 로그인.");
                    GuestSignInAsync().Forget();
                }
            });
            if (_btnSignIn != null) 
            {
                _btnSignIn.onClick.AddListener(() => {
                    if (isOffice) 
                    {
                #if UNITY_WEBGL && !UNITY_EDITOR
                        JsLib.Redirection(URL.INDEX);
                #else 
                        R.singleton.ClearParamValues();
                        ViewManager.singleton.PopAll();
                        ViewManager.singleton.Push(S.SignInScene_SignIn);
                #endif
                    }
                    else if (isWorld) 
                    {
                        ViewManager.singleton.Push(S.WorldScene_SignIn);
                    }
                });
            }
            if (_btnSignUp != null) 
            {
                _btnSignUp.onClick.AddListener(() => {
                    if (isOffice) 
                    {
                        PopupBuilder.singleton.OpenConfirm(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "회원가입 안내", R.singleton.CurrentLocale),
                            () => ViewManager.singleton.Push(isWorld ? S.WorldScene_SignUp : S.SignInScene_SignUp)
                        );
                    }
                    else if (isWorld)
                    {
                        ViewManager.singleton.Push(S.WorldScene_Agreement);
                    }
                });
            }
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();

            _inputName.gameObject.SetActive(true);

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            _inputName.gameObject.SetActive(false);
        }

    #endregion  // FixedView functions


    #region event handling 

        private async UniTask<int> Refresh() 
        {
            // set info
            if (isOffice) 
            {
                string t = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "환영인사 게스트", R.singleton.CurrentLocale);
                _txtDesc.text = string.Format(t, R.singleton.GetParam(Key.WORKSPACE_NAME));

                string logoPath = R.singleton.GetParam(Key.WORKSPACE_LOGO);
                string url = $"{URL.SERVER_PATH}{logoPath}";
                imageLoader.LoadImage(url).Forget();
            }
            else if (isWorld)
            {
                _txtDesc.text = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "환영인사 게스트 w", R.singleton.CurrentLocale);
            }

            // set inputfield
            string userName = R.singleton.GetParam(Key.USER_NAME);
            _inputName.text = userName;
            _inputName.interactable = string.IsNullOrEmpty(userName); 

            // reset uploader
            uploader.Clear(); 

            await UniTask.Yield();
            return 0;
        }

    #endregion  // event handling 


    #region guest sign-in

        private async UniTaskVoid GuestSignInAsync() 
        {
            if (string.IsNullOrEmpty(_inputName.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이름 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            // get 'workspace seq'
            int workspaceSeq = 0;
            if (isOffice) 
            {
                int.TryParse(R.singleton.GetParam(Key.WORKSPACE_SEQ), out workspaceSeq);
            }
            else if (isWorld) 
            {
                workspaceSeq = await _module.GetWorldSequenceAsync();
            }

            if (workspaceSeq <= 0) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "잘못된 접근", R.singleton.CurrentLocale)
                );
                return;
            }

            // get 'guest id, pw'
            PsResponse<ResGuest> resGuest = await _module.GuestSignInAsync(workspaceSeq, _inputName.text, uploader.imageInfo);
            if (! string.IsNullOrEmpty(resGuest.message)) 
            {
                PopupBuilder.singleton.OpenAlert(resGuest.message);
                return;
            }

            // get 'token info'
            PsResponse<ResToken> resToken = await _module.SignInAsync(resGuest.data.id, resGuest.data.pw);
            if (! string.IsNullOrEmpty(resToken.message)) 
            {
                PopupBuilder.singleton.OpenAlert(resToken.message);
                return;
            }

            // save info
            R.singleton.TokenInfo = resToken.data;
            R.singleton.workspaceSeq = workspaceSeq;
            R.singleton.memberSeq = resGuest.data.seq;

            JsLib.SetCookie(Key.GUEST_ID, resGuest.data.id);
            JsLib.SetCookie(Key.GUEST_PASSWORD, resGuest.data.pw);
            JsLib.SetCookie(Key.MEMBER_SEQ, resGuest.data.seq.ToString());
            JsLib.SetCookie(Key.TOKEN_TYPE, resToken.data.token_type);
            JsLib.SetCookie(Key.ACCESS_TOKEN, resToken.data.access_token);
            JsLib.SetCookie(Key.WORKSPACE_SEQ, workspaceSeq.ToString());

            // 후속조치
            if (isOffice) 
            {
                // send alarm and enter workspace
                string email = R.singleton.GetParam(Key.EMAIL); 
                string url = string.Format(URL.SEND_GUEST_ALARM, resGuest.data.seq, email);
                PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PATCH, string.Empty, R.singleton.token);
                if (! string.IsNullOrEmpty(res.message)) 
                {
                    PopupBuilder.singleton.OpenAlert(res.message);
                    return;
                }

                SceneLoader.Load(eScenes.GraphicUI);
            }
            else if (isWorld) 
            {
                // world avata 정보 설정
                WorldAvatarInfo info = new WorldAvatarInfo(
                    resGuest.data.seq,
                    _inputName.text,
                    uploader.imageInfo,
                    S.GUEST,
                    S.ONLINE
                );
                // WorldAvatar.localPlayerInfo = info;
                // WorldChatView.localPlayerInfo = info;  
                WorldPlayer.localPlayerInfo = info;

                // 사용자 정보 로드
                string resMyInfo = await _module.GetMyInfoAsync();
                if (! string.IsNullOrEmpty(resMyInfo)) 
                {
                    PopupBuilder.singleton.OpenAlert(resMyInfo);                   
                    return;
                }

                // xmpp login
            #if UNITY_WEBGL && !UNITY_EDITOR
                SystemManager.singleton.XMPP.XmppLoginForWebGL(R.singleton.memberSeq, R.singleton.myXmppPw);
            #else
                SystemManager.singleton.XMPP.XmppLogin(R.singleton.myXmppId, R.singleton.myXmppPw);
            #endif

                // system manager init 
                int initRes = await SystemManager.singleton.Init();
                if (initRes != 0)
                {
                    Debug.Log($"{TAG} | system manager init() 중에 오류 발생.");
                    return;
                }

                SceneLoader.Load(eScenes.Map);
            }
        }

    #endregion  // guest sign-in
    }
}