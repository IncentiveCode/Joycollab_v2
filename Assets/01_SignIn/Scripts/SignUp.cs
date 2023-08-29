/// <summary>
// 사용자 가입 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 28 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 08. 16) : v1 에서 사용하던 Join 수정, 적용.
///     v0.2 (2023. 08. 28) : class name 변경. Localization 사용 방식 변경. 회원 가입 이후의 조치 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2 
{
    public class SignUp : FixedView
    {
        private const string TAG = "SignUp";

        [Header("Module")]
        [SerializeField] private SignInModule _module;

        [Header("InputField")] 
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private TMP_InputField _inputPw;
        [SerializeField] private TMP_InputField _inputPwConfirm;

        [Header("Button")] 
        [SerializeField] private Button _btnCheck;
        [SerializeField] private Button _btnNext;
        [SerializeField] private Button _btnBack;


        // local variables
        private bool isInvite;
        private bool isCheckDone;


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
                    else if (_inputPw.isFocused)        _inputId.Select();
                    else if (_inputPwConfirm.isFocused) _inputPw.Select();
                }
                else 
                {
                    if (_inputId.isFocused)             _inputPw.Select();
                    else if (_inputPw.isFocused)        _inputPwConfirm.Select();
                    else if (_inputPwConfirm.isFocused) return;
                }
            }
        }
        #endif

    #endregion  // unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();


            // set input field listener
            _inputId.onValueChanged.AddListener((value) => {
                if (isCheckDone) 
                {
                    isCheckDone = false;

                    _btnCheck.interactable = true;
                    _btnNext.interactable = false;
                }
            });
            _inputId.onSubmit.AddListener((value) => {
                if (_inputId.isFocused) CheckAsync().Forget();
            });
            _inputPw.onSubmit.AddListener((value) => {
                if (_inputPw.isFocused) _inputPwConfirm.Select();
            });
            _inputPwConfirm.onSubmit.AddListener((value) => {
                if (_inputPwConfirm.isFocused) JoinAsync().Forget();
            });


            // set button listener
            _btnCheck.onClick.AddListener(() => CheckAsync().Forget());
            _btnNext.onClick.AddListener(() => JoinAsync().Forget());
            _btnBack.onClick.AddListener(() => {
                ViewManager.singleton.PopAll();
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_SignIn : S.SignInScene_SignIn
                );
            });
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();

            _inputId.gameObject.SetActive(true);
            _inputPw.gameObject.SetActive(true);
            _inputPwConfirm.gameObject.SetActive(true);

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            _inputId.gameObject.SetActive(false);
            _inputPw.gameObject.SetActive(false);
            _inputPwConfirm.gameObject.SetActive(false);
        }

    #endregion  // FixedView functions


    #region event hnadling 

        private async UniTask<int> Refresh()
        {
            if (isWorld) 
            {
                _btnBack.gameObject.SetActive(true);

                _inputId.text = _inputPw.text = _inputPwConfirm.text = string.Empty;
                _inputId.interactable = true;
                _btnCheck.interactable = true;
                _btnNext.interactable = false;
            }
            else
            {
                isInvite = R.singleton.GetParam(Key.INVITED).Equals(S.TRUE);
                _btnBack.gameObject.SetActive(! isInvite);

                _inputId.text = isInvite ? R.singleton.GetParam(Key.EMAIL) : string.Empty;
                _inputId.interactable = !isInvite;
                _btnCheck.interactable = !isInvite;
                _btnNext.interactable = isInvite;
            }    


            // set local variables
            isCheckDone = false;

            await UniTask.Yield();
            return 0;    
        }
    
    #endregion  // event hnadling 


    #region join

        private bool CheckPassword()
        {
            return _inputPw.text.Equals(_inputPwConfirm.text);
        }

        private async UniTaskVoid CheckAsync() 
        {
            string id = _inputId.text;
            if (id.Length == 0) return;

            string url = string.Format(URL.CHECK_ID, id);
            PsResponse<ResCheckId> res = await NetworkTask.RequestAsync<ResCheckId>(url, eMethodType.GET);

            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            switch (res.code) 
            {
                case NetworkTask.HTTP_STATUS_CODE_OK :
                    if (res.data.useYn.Equals("Y")) 
                    {
                        PopupBuilder.singleton.OpenAlert(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "사용 중인 계정", R.singleton.CurrentLocale)
                        );
                    }
                    else 
                    {
                        PopupBuilder.singleton.OpenConfirm(
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "탈퇴회원 안내", R.singleton.CurrentLocale),
                            LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "탈퇴회원 설명", R.singleton.CurrentLocale),
                            () => {
                                ViewManager.singleton.Push(
                                    isWorld ? S.WorldScene_Restore : S.SignInScene_Restore,
                                    id
                                );
                            },
                            () => {
                                PopupBuilder.singleton.OpenAlert(
                                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "다른 계정 사용 안내", R.singleton.CurrentLocale)
                                );
                            }
                        );
                    }
                    break;

                case NetworkTask.HTTP_STATUS_CODE_NO_CONTENT :
                    PopupBuilder.singleton.OpenAlert(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "사용 가능 계정", R.singleton.CurrentLocale)
                    );

                    _btnCheck.interactable = false;
                    _btnNext.interactable = true;
                    isCheckDone = true;

                    break;

                default :
                    Debug.Log($"{TAG} | CheckAsync(), 알 수 없는 상태 코드 : {res.code}");
                    break;
            }
        }

        private async UniTaskVoid JoinAsync() 
        {
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

            if (! CheckPassword()) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "비밀번호 불일치", R.singleton.CurrentLocale)
                );
                return;
            }

            string id = _inputId.text;
            string pw = _inputPw.text;
            PsResponse<string> res = await _module.SignUpAsync(id, pw);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            // TODO. 후속 조치
        }

    #endregion  // join 
    }
}