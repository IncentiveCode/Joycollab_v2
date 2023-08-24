/// <summary>
/// [PC Web]
/// 게스트가 특정 회사에 들어가거나 월드에 진입하는 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 23.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 23) : v1 에서 만들었던 GuestLogin 수정 후 적용.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2 
{
    public class Guest : FixedView
    {
        private const string TAG = "Guest";
        
        [Header("tag")]
        [TagSelector]
        [SerializeField] private string viewTag;

        [Header("module")]
        [SerializeField] private SignInModule _module;

        [Header("guest photo")]
        [SerializeField] private Button _btnPhoto;
        [SerializeField] private RawImage _imgUploadPhoto;
        [SerializeField] private Vector2 _v2PhotoSize;

        [Header("office logo")]
        [SerializeField] private RawImage _imgOfficeLogo;
        [SerializeField] private Vector2 _v2LogoSize;

        [Header("greetings")]
        [SerializeField] private Text _txtDesc;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputName;

        [Header("Button")]
        [SerializeField] private Button _btnEnter;
        [SerializeField] private Button _btnSignIn;
        [SerializeField] private Button _btnSignUp;

        [Header("default texture")]
        [SerializeField] private Texture2D _texDefaultUser;
        [SerializeField] private Texture2D _texDefaultLogo;

        // local variables
        private ImageUploader _handler;
        private RectTransform _rectLogo;
        private bool isWorld;
        private Locale currentLocale;


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


            // set 'image uploader'
            _handler = _btnPhoto.GetComponent<ImageUploader>();
            _handler.Init((int) _v2PhotoSize.x, (int) _v2PhotoSize.y);


            // set input field listener
            _inputName.onSubmit.AddListener((value) => {
                if (_inputName.isFocused) 
                {

                }
            });


            // set button listener
            _btnEnter.onClick.AddListener(() => {
                // TODO. Guest Process 추가.
                /**
                if (isWorld) 
                {
                    Debug.Log($"{TAG} | 광장으로 로그인.");
                } 
                else 
                {
                    Debug.Log($"{TAG} | 남의 사무실로 로그인.");
                }
                 */
            });
            _btnSignIn.onClick.AddListener(() => {
                if (ViewManager.singleton.GetStackCount() >= 1) 
                {
                    ViewManager.singleton.Pop();
                }
                else 
                {
                    if (isWorld) 
                    {
                        ViewManager.singleton.Push(S.WorldScene_SignIn);
                    }
                    else 
                    {
                #if UNITY_WEBGL && !UNITY_EDITOR
                        JsLib.Redirection(URL.INDEX);
                #else 
                        R.singleton.ClearParamValues();
                        ViewManager.singleton.PopAll();
                        ViewManager.singleton.Push(S.SignInScene_SignIn);
                #endif
                    }
                }
            });
            _btnSignUp.onClick.AddListener(() => {
                PopupBuilder.singleton.OpenConfirm(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "회원가입 안내", currentLocale),
                    () => ViewManager.singleton.Push(isWorld ? S.WorldScene_SignUp : S.SignInScene_SignUp)
                );
            });
        }

        public async override UniTaskVoid Show() 
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
            // set local variables
            currentLocale = LocalizationSettings.SelectedLocale;
            isWorld = viewTag.Equals(S.WorldScene_ViewTag);

            if (isWorld) 
            {

            }
            else 
            {

            }

            await UniTask.Yield();
            return 0;
        }

    #endregion  // event handling 
    }
}