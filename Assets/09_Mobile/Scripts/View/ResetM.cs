/// <summary>
/// [mobile]
/// 비밀번호 재설정을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 20
/// @version        : 0.4
/// @update
///     v0.1 : 최초 생성
///     v0.2 : 새로운 디자인 적용.
///     v0.3 (2022. 05. 25) : 디자인 수정안 적용. input field clear button 추가.
///     v0.4 (2023. 03. 20) : FixedView 실험, UniTask 적용, UI Canvas 최적화.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class ResetM : FixedView
    {
        [Header("Before auth")]
        [SerializeField] private GameObject _goBeforeAuth;
        [SerializeField] private InputField _inputId;
        [SerializeField] private Button _btnClearId;
        [SerializeField] private InputField _inputPhone0;
        [SerializeField] private InputField _inputPhone1;
        [SerializeField] private InputField _inputPhone2;
        [SerializeField] private Button _btnFind;
        [SerializeField] private Button _btnBack;

        [Header("After auth")]
        [SerializeField] private GameObject _goAfterAuth;
        [SerializeField] private InputField _inputCode;
        [SerializeField] private Button _btnClearCode;
        [SerializeField] private InputField _inputPw;
        [SerializeField] private Button _btnClearPw;
        [SerializeField] private InputField _inputPwConfirm;
        [SerializeField] private Button _btnClearPwConfirm;
        [SerializeField] private Button _btnReset;
        [SerializeField] private Button _btnCancel;

        // local variables
        private bool oneTimeCheck;
        private bool isAuth;


    #region Unity functions
        private void Awake()
        {
            Init();
            Reset();
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

            // 0. before auth view
            _inputId.onValueChanged.AddListener((value) => {
                _btnClearId.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputId.onEndEdit.AddListener((value) => _inputPhone0.Select());
            _btnClearId.onClick.AddListener(() => _inputId.text = string.Empty);

            _inputPhone0.onEndEdit.AddListener((value) => _inputPhone1.Select());
            _inputPhone1.onEndEdit.AddListener((value) => _inputPhone2.Select());

            _btnFind.onClick.AddListener(() => CheckValidation());
            _btnBack.onClick.AddListener(() => BackProcess());


            // 1. after auth view
            _inputCode.onValueChanged.AddListener((value) => {
                _btnClearCode.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputCode.onEndEdit.AddListener((value) => _inputPw.Select());
            _btnClearCode.onClick.AddListener(() => _inputCode.text = string.Empty);

            _inputPw.onValueChanged.AddListener((value) => {
                _btnClearPw.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputPw.onEndEdit.AddListener((value) => _inputPwConfirm.Select());
            _btnClearPw.onClick.AddListener(() => _inputPw.text = string.Empty);

            _inputPwConfirm.onValueChanged.AddListener((value) => {
                _btnClearPwConfirm.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _btnClearPwConfirm.onClick.AddListener(() => _inputPwConfirm.text = string.Empty);

            _btnReset.onClick.AddListener(() => ResetPassword());
            _btnCancel.onClick.AddListener(() => BackProcess());


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


    #region Before auth functions
        private void CheckValidation() 
        {
            if (string.IsNullOrEmpty(_inputId.text))
            {
                Debug.Log("이메일 없음.");
                return;
            }

            if (string.IsNullOrEmpty(_inputPhone0.text))
            {
                Debug.Log("전화번호 0 없음.");
                return;
            }

            if (string.IsNullOrEmpty(_inputPhone1.text))
            {
                Debug.Log("전화번호 1 없음.");
                return;
            }

            if (string.IsNullOrEmpty(_inputPhone2.text))
            {
                Debug.Log("전화번호 2 없음.");
                return;
            }

            string id = _inputId.text;
            string phone = $"{_inputPhone0.text}{_inputPhone1.text}{_inputPhone2.text}";
            RequestProcess(id, phone).Forget();
        }

        private async UniTaskVoid RequestProcess(string id, string phone) 
        {
            bool isAvailable = await CheckIdAsync(id);
            if (! isAvailable) return;

            string message = await RequestCodeAsync(id, phone);
            if (string.IsNullOrEmpty(message)) 
            {
                Debug.Log("인증코드 요청 성공.");
                isAuth = true;

                _inputCode.text = _inputPw.text = _inputPwConfirm.text = string.Empty;
                _goAfterAuth.SetActive(true); 
                _goBeforeAuth.SetActive(false);
            }
            else 
            {
                Debug.Log("인증코드 요청 실패.");
                isAuth = false;
            }
        }

        // step 1. 사용자 정보 확인
        private async UniTask<bool> CheckIdAsync(string id) 
        {
            bool isAvailable = false;

            string url = string.Format(URL.CHECK_ID, id);            
            PsResponse<ResCheckId> res = await NetworkTask.RequestAsync<ResCheckId>(url, MethodType.GET);
            if (! string.IsNullOrEmpty(res.message))
            {
                Debug.Log(res.message);
                return false;
            }

            switch (res.code) 
            {
                case NetworkTask.HTTP_STATUS_CODE_OK :
                    isAvailable = true;
                    break;

                case NetworkTask.HTTP_STATUS_CODE_NO_CONTENT :
                    Debug.Log("존재하지 않는 사용자.");
                    break;

                default :
                    Debug.Log("왜 여기서 이 코드가...? :"+ res.code);
                    break;
            }

            return isAvailable;
        }

        // step 2. id, phone 정보로 인증 코드 요청
        private async UniTask<string> RequestCodeAsync(string id, string phone) 
        {
            string url = string.Format(URL.REQUEST_RESET, id, phone);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, MethodType.GET);
            return res.message;
        }
    #endregion  // Before auth functions


    #region After auth functions
        private void ResetPassword() 
        {
            Debug.Log("reset password...");
        }
    #endregion  // After auth functions


    #region event handling functions
        private async UniTask<int> Refresh() 
        {
            oneTimeCheck = true;
            isAuth = false;

            _goBeforeAuth.SetActive(true);
            _inputId.text = _inputPhone0.text = _inputPhone1.text = _inputPhone2.text = string.Empty;
            _btnClearId.gameObject.SetActive(false);

            _goAfterAuth.SetActive(false);
            _inputCode.text = _inputPw.text = _inputPwConfirm.text = string.Empty;
            _btnClearCode.gameObject.SetActive(false);
            _btnClearPw.gameObject.SetActive(false);
            _btnClearPwConfirm.gameObject.SetActive(false);

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
            if (isAuth) 
            {
                // 확인 후 이동.
            }
            else 
            {
                MobileManager.singleton.Pop();
            }
        }
    #endregion  // event handling functions
    }
}