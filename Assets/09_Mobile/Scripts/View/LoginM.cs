/// <summary>
/// [mobile]
/// 로그인 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 20
/// @version        : 0.4
/// @update
///     v0.1 : 최초 생성.
///     v0.2 : 새로운 디자인 적용.
///     v0.3 (2022. 05. 25) : 디자인 수정안 적용. input field clear button 추가.
///     v0.4 (2023. 03. 20) : FixedView 실험, UniTask 적용, UI canvas 최적화.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class LoginM : FixedView
    {
        [Header("Input e-mail")]
        [SerializeField] private MobileSubmitDetector _inputId;
        [SerializeField] private Button _btnClearId;

        [Header("Input password")]
        [SerializeField] private MobileSubmitDetector _inputPw;
        [SerializeField] private Button _btnClearPw;

        [Header("buttons")]
        [SerializeField] private Button _btnLogin;
        [SerializeField] private Button _btnJoin;
        [SerializeField] private Button _btnResetPw;
        [SerializeField] private Button _btnVersion;

        // local variable
        private bool oneTimeCheck;


    #region Unity functions
        private void Awake() 
        {
            Init();
            Reset();
        }

        private void Update() 
        {

        }

        private void OnDestroy() 
        {
            if (MobileEvents.Instance != null) 
            {
                MobileEvents.Instance.OnBackButtonProcess -= BackButtonProcess;
            }
        }
    #endregion  // Unity functions


    #region FixedView functions
        protected override void Init() 
        {
            base.Init();

            // set 'e-mail' inputfield listener
            _inputId.onValueChanged.AddListener((value) => {
                _btnClearId.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputId.onKeyboardDone.AddListener(() => {
                Debug.Log("hello 1");
                _inputPw.Select();
            });
            _btnClearId.onClick.AddListener(() => {
                _inputId.text = string.Empty;
                _inputId.Select();
            });


            // set 'password' inputfield listener
            _inputPw.onValueChanged.AddListener((value) => {
                _btnClearPw.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputPw.onKeyboardDone.AddListener(() => {
                Debug.Log("hello 2?");
            });
            _btnClearPw.onClick.AddListener(() => {
                _inputPw.text = string.Empty;
                _inputPw.Select();
            });


            // set button listener
            _btnLogin.onClick.AddListener(() => Login());
            _btnJoin.onClick.AddListener(() => {
                Application.OpenURL(URL.PATH);
            });
            _btnResetPw.onClick.AddListener(() => MobileManager.singleton.Push(S.MobileScene_Reset));
            _btnVersion.onClick.AddListener(() => MobileManager.singleton.Push(S.MobileScene_PatchNote));


            // add event handling
            MobileEvents.Instance.OnBackButtonProcess += BackButtonProcess;
        }

        protected override void Reset() 
        {
            base.Reset();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            int d = await Refresh();

            base.Appearing().Forget();
        }
    #endregion  // FixedView functions


    #region Login functions
        private void Login() 
        {
            if (string.IsNullOrEmpty(_inputId.text)) 
            {
                Debug.Log("이메일 없음.");
                return;
            }

            if (string.IsNullOrEmpty(_inputPw.text)) 
            {
                Debug.Log("패스워드 없음.");
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
            form.AddField(NetworkTask.SCOPE, NetworkTask.SCOPE_APP);
            form.AddField(NetworkTask.USERNAME, id);

            PsResponse<ResToken> res = await NetworkTask.PostAsync<ResToken>(URL.REQUEST_TOKEN, form, string.Empty, NetworkTask.MOBILE_BASIC_TOKEN);
            if (string.IsNullOrEmpty(res.message)) 
            {
                Debug.Log($"로그인 성공. token : {res.data.token_type} {res.data.access_token}");
            }
            else 
            {
                Debug.Log("로그인 실패");
            }
        }
    #endregion  // Login functions


    #region event handling
        private async UniTask<int> Refresh() 
        {
            _inputPw.text = string.Empty;
            _inputPw.interactable = true;
            _btnClearPw.gameObject.SetActive(false);

            string id = JsLib.GetCookie(Key.SAVED_LOGIN_ID);
            _inputId.text = id;
            _inputId.interactable = true;
            _btnClearId.gameObject.SetActive(! string.IsNullOrEmpty(id));

            // MobileManager.singleton.StopLoading();
            oneTimeCheck = true;

            await UniTask.Yield();
            return 0;
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            BackProcess();
        }

        private void BackProcess() 
        {
            Debug.Log("종료 여부 확인");
        }
    #endregion  // event handling
    }
}