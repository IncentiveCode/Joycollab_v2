/// <summary>
/// 사용자 계정 복구 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 29.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 29) : v1 에서 만들었던 UserRestoreView 수정 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2 
{
    public class Restore : FixedView
    {
        private const string TAG = "Restore";

        [Header("Module")]
        [SerializeField] private SignInModule _module;

        [Header("before auth")] 
        [SerializeField] private GameObject _goBefore;
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private TMP_InputField _inputPhone;
        [SerializeField] private Button _btnNext;
        [SerializeField] private Button _btnCancel;

        [Header("after auth")] 
        [SerializeField] private GameObject _goAfter;
        [SerializeField] private TMP_InputField _inputCode;
        [SerializeField] private TMP_InputField _inputPw;
        [SerializeField] private TMP_InputField _inputPwConfirm;
        [SerializeField] private Button _btnRestore;
        [SerializeField] private Button _btnCancelForce;

        // private variables
        private string id, tel;


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
                    else if (_inputPhone.isFocused) _inputId.Select();

                    if (_inputCode.isFocused) return;
                    else if (_inputPw.isFocused) _inputCode.Select();
                    else if (_inputPwConfirm.isFocused) _inputPw.Select();
                }
                else 
                {
                    if (_inputId.isFocused) _inputPhone.Select();
                    else if (_inputPhone.isFocused) return;

                    if (_inputCode.isFocused) _inputPw.Select();
                    else if (_inputPw.isFocused) _inputPwConfirm.Select();
                    else if (_inputPwConfirm.isFocused) return;
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
            // nothing...


            // set button listener
            _btnNext.onClick.AddListener(() => RequestCodeAsync().Forget());
            _btnCancel.onClick.AddListener(() => BackProcess());
            _btnRestore.onClick.AddListener(() => RequestRestoreAsync().Forget());
            _btnCancelForce.onClick.AddListener(() => {
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "계정 복구 취소 확인", R.singleton.CurrentLocale),
                    () => BackProcess()
                );
            });
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh(string.Empty);

            _inputId.gameObject.SetActive(true);
            _inputPhone.gameObject.SetActive(true);
            _inputCode.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
            _inputPwConfirm.gameObject.SetActive(false);

            base.Appearing();
        }

        public async override UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();
            await Refresh(opt);

            _inputId.gameObject.SetActive(true);
            _inputPhone.gameObject.SetActive(true);
            _inputCode.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
            _inputPwConfirm.gameObject.SetActive(false);

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            _inputId.gameObject.SetActive(false);
            _inputPhone.gameObject.SetActive(false);
            _inputCode.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
            _inputPwConfirm.gameObject.SetActive(false);
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh(string opt) 
        {
            // set local variables
            id = opt;
            tel = string.Empty;

            _goBefore.SetActive(true);
            _inputId.text = opt;
            _inputId.interactable = string.IsNullOrEmpty(opt);
            _inputPhone.text = string.Empty;
            _inputPhone.interactable = true;

            _goAfter.SetActive(false);
            _inputCode.text = string.Empty;
            _inputCode.interactable = false;
            _inputPw.text = string.Empty;
            _inputPw.interactable = false;
            _inputPwConfirm.text = string.Empty;
            _inputPwConfirm.interactable = false;

            await UniTask.Yield();
            return 0;
        }

        private void BackProcess() 
        {
            ViewManager.singleton.PopAll();

            if (isOffice) ViewManager.singleton.Push(S.SignInScene_SignIn);
            else if (isWorld) ViewManager.singleton.Push(S.WorldScene_SignIn);
            else if (isMobile) ViewManager.singleton.Push(S.MobileScene_Login);
        }

    #endregion  // event handling


    #region process

        private async UniTaskVoid RequestCodeAsync() 
        {
            if (string.IsNullOrEmpty(_inputId.text)) 
            {
                PopupBuilder.singleton.OpenAlert( 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이메일 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPhone.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "휴대폰 번호 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            tel = string.Empty;
            if (! RegExp.MatchPhoneNumber(_inputPhone.text, out tel))
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "휴대폰 번호 정규식검사 실패", R.singleton.CurrentLocale)
                );
                return;
            }

            PsResponse<string> res = await _module.RequestCodeAsync(_inputId.text, tel);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            PopupBuilder.singleton.OpenAlert(
                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "계정 복구 인증번호 전송 안내", R.singleton.CurrentLocale)
            );

            if (string.IsNullOrEmpty(id))
                id = _inputId.text;

            // page change
            _inputId.interactable = false;
            _inputId.gameObject.SetActive(false);
            _inputPhone.interactable = false;
            _inputPhone.gameObject.SetActive(false);
            _goBefore.SetActive(false);

            _inputCode.gameObject.SetActive(true);
            _inputCode.interactable = true;
            _inputPw.gameObject.SetActive(true);
            _inputPw.interactable = true;
            _inputPwConfirm.gameObject.SetActive(true);
            _inputPwConfirm.interactable = true;
            _goAfter.SetActive(true);
        }

        private async UniTaskVoid RequestRestoreAsync() 
        {
            if (string.IsNullOrEmpty(_inputCode.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "인증번호 없음", R.singleton.CurrentLocale)
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

            if (! RegExp.MatchPasswordRule(_inputPw.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 안내", R.singleton.CurrentLocale)
                );
                return;
            }

            if (string.IsNullOrEmpty(_inputPwConfirm.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호확인 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            if (! _inputPw.text.Equals(_inputPwConfirm.text)) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 불일치", R.singleton.CurrentLocale)
                );
                return;
            }

            PsResponse<string> res = await _module.RequestRestoreAsync(_inputCode.text, id, _inputPw.text, tel);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            PopupBuilder.singleton.OpenAlert(
                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "계정 복구 완료 안내", R.singleton.CurrentLocale),
                () => BackProcess()
            );
        }

    #endregion  // process
    }
}