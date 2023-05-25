/// <summary>
/// [mobile]
/// 비밀번호 재설정을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 31
/// @version        : 0.5
/// @update
///     v0.1 : 최초 생성
///     v0.2 : 새로운 디자인 적용.
///     v0.3 (2022. 05. 25) : 디자인 수정안 적용. input field clear button 추가.
///     v0.4 (2023. 03. 20) : FixedView 실험, UniTask 적용, UI Canvas 최적화.
///     v0.5 (2023. 03. 31) : PopupBuilder 적용, InputSubmitDetector 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class ResetM : FixedView
    {
        [Header("view - Before auth ")]
        [SerializeField] private GameObject _goBeforeAuth;

        [Header(" - input e-mail")]
        [SerializeField] private InputSubmitDetector _inputId;
        [SerializeField] private Button _btnClearId;

        [Header(" - input phone")]
        [SerializeField] private InputSubmitDetector _inputPhone;
        [SerializeField] private Button _btnClearPhone;

        [Header(" - buttons")]
        [SerializeField] private Button _btnFind;
        [SerializeField] private Button _btnBack;

        [Header("view - After auth")]
        [SerializeField] private GameObject _goAfterAuth;

        [Header(" - input code")]
        [SerializeField] private InputSubmitDetector _inputCode;
        [SerializeField] private Button _btnClearCode;

        [Header(" - input new password")]
        [SerializeField] private InputSubmitDetector _inputPw;
        [SerializeField] private Button _btnClearPw;

        [Header(" - input password confirm")]
        [SerializeField] private InputSubmitDetector _inputPwConfirm;
        [SerializeField] private Button _btnClearPwConfirm;

        [Header(" - buttons")]
        [SerializeField] private Button _btnReset;
        [SerializeField] private Button _btnCancel;

        // local variables
        private bool isAuth;


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
            viewID = ID.MobileScene_Reset;


            // 0. before auth view
            _inputId.onValueChanged.AddListener((value) => {
                _btnClearId.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputId.onKeyboardDone.AddListener(() => _inputPhone.Select());
            _btnClearId.onClick.AddListener(() => _inputId.text = string.Empty);

            _inputPhone.onValueChanged.AddListener((value) => {
                _btnClearPhone.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
			_btnClearPhone.onClick.AddListener(() => _inputPhone.text = string.Empty);

            _btnFind.onClick.AddListener(() => CheckValidation());
            _btnBack.onClick.AddListener(() => BackProcess());


            // 1. after auth view
            _inputCode.onValueChanged.AddListener((value) => {
                _btnClearCode.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputCode.onKeyboardDone.AddListener(() => _inputPw.Select());
            _btnClearCode.onClick.AddListener(() => _inputCode.text = string.Empty);

            _inputPw.onValueChanged.AddListener((value) => {
                _btnClearPw.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputPw.onKeyboardDone.AddListener(() => _inputPwConfirm.Select());
            _btnClearPw.onClick.AddListener(() => _inputPw.text = string.Empty);

            _inputPwConfirm.onValueChanged.AddListener((value) => {
                _btnClearPwConfirm.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _btnClearPwConfirm.onClick.AddListener(() => _inputPwConfirm.text = string.Empty);

            _btnReset.onClick.AddListener(() => ResetPassword());
            _btnCancel.onClick.AddListener(() => BackProcess());
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh(); 

            base.Appearing();
        }

    #endregion  // FixedView functions


    #region Before auth functions

        private void CheckValidation() 
        {
            if (string.IsNullOrEmpty(_inputId.text))
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이메일 없음", currentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPhone.text))
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "휴대폰 번호 없음", currentLocale)
                );
                return;
            }

            string onlyNumber = string.Empty;
            if (! RegExp.MatchPhoneNumber(_inputPhone.text, out onlyNumber))
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "휴대폰 번호 정규식검사 실패", currentLocale)
                );
                return;
            }

            string id = _inputId.text;
            string phone = _inputPhone.text;
            RequestProcess(id, phone).Forget();
        }

        private async UniTaskVoid RequestProcess(string id, string phone) 
        {
            bool isAvailable = await CheckIdAsync(id);
            if (! isAvailable) return;

            string message = await RequestCodeAsync(id, phone);
            if (string.IsNullOrEmpty(message)) 
            {
                isAuth = true;

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "비밀번호 변경 안내", currentLocale)
                );

                _inputCode.text = _inputPw.text = _inputPwConfirm.text = string.Empty;
                _goAfterAuth.SetActive(true); 
                _goBeforeAuth.SetActive(false);
            }
            else 
            {
                isAuth = false;

                PopupBuilder.singleton.OpenAlert(message);
            }
        }

        // step 1. 사용자 정보 확인
        private async UniTask<bool> CheckIdAsync(string id) 
        {
            bool isAvailable = false;
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            
            string url = string.Format(URL.CHECK_ID, id);            
            PsResponse<ResCheckId> res = await NetworkTask.RequestAsync<ResCheckId>(url, eMethodType.GET);
            if (! string.IsNullOrEmpty(res.message))
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return false;
            }

            switch (res.code) 
            {
                case NetworkTask.HTTP_STATUS_CODE_OK :
                    isAvailable = res.data.useYn.Equals("Y");
                    if (res.data.useYn.Equals("N")) 
                    {
						PopupBuilder.singleton.OpenAlert(
							LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "사용 정지 계정", currentLocale)
						);
                    }
                    break;

                case NetworkTask.HTTP_STATUS_CODE_NO_CONTENT :
					PopupBuilder.singleton.OpenAlert(
						LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "존재하지 않는 계정", currentLocale)
					);
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
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.GET);
            return res.message;
        }

    #endregion  // Before auth functions


    #region After auth functions

        private bool CheckPassword() 
        {
            return _inputPw.text.Equals(_inputPwConfirm.text);
        }

        private void ResetPassword() 
        {
            if (string.IsNullOrEmpty(_inputCode.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "인증번호 없음", currentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPw.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "새 비밀번호 없음", currentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPwConfirm.text)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "새 비밀번호확인 없음", currentLocale)
                );
                return;
            }

            if (! CheckPassword()) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 불일치", currentLocale)
                );
                return;
            }

            ResetProcess().Forget();
        }

        private async UniTaskVoid ResetProcess() 
        {
            string id = _inputId.text;
            string phone = _inputPhone.text;
            string code = _inputCode.text;
            string pw = _inputPw.text;

            string url = string.Format(URL.RESET_PW, code, id, pw, phone);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PATCH);

            if (string.IsNullOrEmpty(res.message)) 
            {
                isAuth = false;

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "비밀번호 변경 완료", currentLocale),
                    () => ViewManager.singleton.Pop()
                );
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

    #endregion  // After auth functions


    #region event handling functions
        private async UniTask<int> Refresh() 
        {
            isAuth = false;

            _inputId.text = _inputPhone.text = string.Empty;
            _btnClearId.gameObject.SetActive(false);
            _btnClearPhone.gameObject.SetActive(false);
            _goBeforeAuth.SetActive(true);

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
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 재설정 취소 확인", currentLocale),
                    () => ViewManager.singleton.Pop()
                );
            }
            else 
            {
                ViewManager.singleton.Pop();
            }
        }
    #endregion  // event handling functions
    }
}