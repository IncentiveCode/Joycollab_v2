/// <summary>
/// [PC Web - world]
/// 사용자 로그인 화면 for World
/// @author         : HJ Lee
/// @last update    : 2023. 08. 11.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 11) : v1 에서 만들었던 world login 수정 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class SignInW : FixedView
    {
        private const string TAG = "SignInW";

        [Header("Module")]
        [SerializeField] private SignInModule _module;
        
        [Header("InputField")]
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private TMP_InputField _inputPw;

        [Header("Toggle")] 
        [SerializeField] private Toggle _toggleRemember;
        [SerializeField] private Toggle _toggleGoToCenter;

        [Header("Button")]
        [SerializeField] private Button _btnGuest;
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Button _btnSignUp;
        [SerializeField] private Button _btnResetPw;

        // local variables
        private Locale currentLocale;


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


            // set input field listener
            _inputId.onSubmit.AddListener((value) => {
                if (_inputId.isFocused) _inputPw.Select();
            });
            _inputPw.onSubmit.AddListener((value) => {
                if (_inputPw.isFocused) Request().Forget();
            });


            // set button listener
            _btnGuest.onClick.AddListener(() => Debug.Log($"{TAG} | 손님용 입장 화면으로 이동."));
            _btnSignIn.onClick.AddListener(() => Request().Forget());
            _btnSignUp.onClick.AddListener(() => ViewManager.singleton.Push(S.WorldScene_Agreement));
            _btnResetPw.onClick.AddListener(() => Debug.Log($"{TAG} | 비밀번호 재설정 화면으로 이동."));
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

            _inputId.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            string toggleValue = JsLib.GetCookie(Key.TOGGLE_WORLD_ID_SAVED);
            bool isOn = toggleValue.Equals(S.TRUE);
            _toggleRemember.isOn = isOn;

            toggleValue = JsLib.GetCookie(Key.TOGGLE_GO_TO_CENTER);
            isOn = toggleValue.Equals(S.TRUE);
            _toggleGoToCenter.isOn = isOn;

            _inputId.text = isOn ? JsLib.GetCookie(Key.SAVED_WORLD_LOGIN_ID) : string.Empty;
            _inputPw.text = string.Empty;

            
            // set local variables
            currentLocale = LocalizationSettings.SelectedLocale;

            await UniTask.Yield();
            return 0;    
        }

    #endregion  // event handling


    #region login, world join

        private async UniTaskVoid Request() 
        {
            if (string.IsNullOrEmpty(_inputId.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이메일 없음", currentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPw.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 없음", currentLocale)
                );
                return;
            }

            string id = _inputId.text;
            string pw = _inputPw.text;

            PsResponse<ResToken> res = await _module.SignInAsync(id, pw);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "로그인 실패", currentLocale)
                );
                return;
            }

            R.singleton.ID = id;
            R.singleton.TokenInfo = res.data;

            JsLib.SetCookie(Key.TOGGLE_WORLD_ID_SAVED, _toggleRemember.isOn ? S.TRUE : S.FALSE);
            JsLib.SetCookie(Key.SAVED_WORLD_LOGIN_ID, _toggleRemember.isOn ? _inputId.text : string.Empty);
            JsLib.SetCookie(Key.TOGGLE_GO_TO_CENTER, _toggleGoToCenter.isOn ? S.TRUE : S.FALSE);
            JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
            JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

            PsResponse<ResWorkspaceInfo> res2 = await _module.WorldSignInAsync(); 
            if (! string.IsNullOrEmpty(res2.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res2.message);
                return;
            }


            WorldAvatarInfo info = new WorldAvatarInfo();
            info.seq = res2.data.seq;
            info.nickNm = res2.data.nickNm;
            info.photo = res2.data.photo;

            // TODO. set world avatar info
            // WorldAvatar.localPlayerInfo = info;
            // WorldChatView.localPlayerInfo = info;

            // TODO. Square 와 Map 설정 후 해제할 것.
            // if (_toggleGoToCenter.isOn)     SceneLoader.Load(eScenes.Square);
            // else                            SceneLoader.Load(eScenes.Map);
            if (_toggleGoToCenter.isOn)     Debug.Log($"{TAG} | Square scene 으로 이동.");
            else                            Debug.Log($"{TAG} | Map scene 으로 이동.");
        }

    #endregion  // login
    }
}