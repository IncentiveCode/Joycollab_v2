/// <summary>
/// 회원가입시 사용자 정보 업데이트 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 28.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 29) : v1 에서 만들었던 InfoView, WorldInfoView 수정 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class MemberInfo : FixedView
    {
        private const string TAG = "MemberInfo";
        private const string CALLBACK = "SearchAddressResult";

        [Header("Module")]
        [SerializeField] private SignInModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputId;
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private TMP_InputField _inputPhone;
        [SerializeField] private TMP_InputField _inputOffice;
        [SerializeField] private TMP_InputField _inputAddress1;
        [SerializeField] private TMP_InputField _inputAddress2;

        [Header("Button")]
        [SerializeField] private Button _btnPhoto;
        [SerializeField] private Button _btnSearchAddress;
        [SerializeField] private Button _btnNext;

        // local variables
        private ImageUploader uploader;
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
                    if (_inputName.isFocused) return;
                    else if (_inputPhone.isFocused) _inputName.Select();
                    else if (_inputOffice.isFocused) _inputPhone.Select();
                }
                else 
                {
                    if (_inputName.isFocused) _inputPhone.Select();
                    else if (_inputPhone.isFocused) _inputOffice.Select();
                    else if (_inputOffice.isFocused) return;
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


            // set 'image uploader'
            uploader = _btnPhoto.GetComponent<ImageUploader>();
            uploader.Init();


            // set inputfield listener
            // nothing...


            // set button listener
            _btnSearchAddress.onClick.AddListener(() => JsLib.SearchAddress(this.name, CALLBACK)); 
            _btnNext.onClick.AddListener(() => UpdateUserInfo().Forget());
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();

            _inputId.gameObject.SetActive(true);
            _inputName.gameObject.SetActive(true);
            _inputPhone.gameObject.SetActive(true);
            _inputOffice.gameObject.SetActive(true);
            _inputAddress1.gameObject.SetActive(true);
            _inputAddress2.gameObject.SetActive(true);

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            _inputId.gameObject.SetActive(false);
            _inputName.gameObject.SetActive(false);
            _inputPhone.gameObject.SetActive(false);
            _inputOffice.gameObject.SetActive(false);
            _inputAddress1.gameObject.SetActive(false);
            _inputAddress2.gameObject.SetActive(false);
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // set input field
            _inputId.text = R.singleton.ID;
            _inputName.text = _inputPhone.text = _inputOffice.text = _inputAddress1.text = _inputAddress2.text = string.Empty;

            // reset uploader
            uploader.Clear();

            await UniTask.Yield();
            return 0;
        }

        public void SearchAddressResult(string result) 
        {
            string[] arrResult = result.Split('|');
            string addr = (arrResult.Length >= 1) ? arrResult[0] : string.Empty;

            _inputAddress1.text = addr;
            _inputAddress2.text = string.Empty;
        }

    #endregion  // event handling


    #region process

        private async UniTaskVoid UpdateUserInfo() 
        {
            if (string.IsNullOrEmpty(_inputName.text)) 
            {
                PopupBuilder.singleton.OpenAlert( 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "이름 없음", R.singleton.CurrentLocale)
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

            if (string.IsNullOrEmpty(_inputOffice.text)) 
            {
                PopupBuilder.singleton.OpenAlert( 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "회사 이름 없음", R.singleton.CurrentLocale)
                );
                return;
            }

            await UniTask.Yield();
        }

    #endregion  // process
    }
}