/// <summary>
/// [world]
/// 환경설정 _ 개인설정 Script
/// @author         : HJ Lee
/// @last update    : 2023. 09. 15
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 15) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class IndividualSettings : WindowPage
    {
        private const string TAG = "IndividualSettings";
        private const string CALLBACK = "SearchAddressResult";

        // TODO. 모듈 추가 예정. 

        [Header("text")]
        [SerializeField] private Text _txtId;

        [Header("profile")]
        [SerializeField] private Button _btnProfile;
        [SerializeField] private RawImage _imgProfile;

        [Header("inputfield _ require")]
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private TMP_InputField _inputOffice;
        [SerializeField] private TMP_InputField _inputGrade;
        [SerializeField] private TMP_InputField _inputPhone;
        [SerializeField] private TMP_InputField _inputAddress1;
        [SerializeField] private TMP_InputField _inputAddress2;

        [Header("inputfield _ option")]
        [SerializeField] private TMP_InputField _inputBusinessNumber;
        [SerializeField] private TMP_InputField _inputCeo;
        [SerializeField] private TMP_InputField _inputSector;
        [SerializeField] private TMP_InputField _inputService;
        [SerializeField] private TMP_InputField _inputTel;
        [SerializeField] private TMP_InputField _inputHomepage;

        [Header("button")]
        [SerializeField] private Button _btnSearchAddress;

        // local variables
        private ImageUploader imageUploader;
        private ImageLoader imageLoader;
        private string id, tel;
        private float lat, lng;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

    #endregion  // Unity functions


    #region WindowPage functions

        protected override void Init() 
        {
            base.Init();


            // set 'image uploader'
            imageUploader = _btnProfile.GetComponent<ImageUploader>();
            imageUploader.Init();

            // set 'image loader'
            imageLoader = _imgProfile.GetComponent<ImageLoader>();


            // set button listener
            _btnSearchAddress.onClick.AddListener(() => JsLib.SearchAddress(this.name, CALLBACK));
        }

        public async override UniTaskVoid Show() 
        {
            Debug.Log($"{TAG} | Show() call.");
            base.Show().Forget();

            await Refresh();
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            SetInputFieldActive(false);
        }

    #endregion  // WindowPage functions


    #region Event handling

        private async UniTask<int> Refresh() 
        {
            SetInputFieldActive(true);

            // TODO. module 통신 이후 개인 정보 가지고 와서 출력.

            await UniTask.Yield();
            return 0;
        }

        private void SetInputFieldActive(bool isOn) 
        {
            _inputName.gameObject.SetActive(isOn);
            _inputOffice.gameObject.SetActive(isOn);
            _inputGrade.gameObject.SetActive(isOn);
            _inputPhone.gameObject.SetActive(isOn);
            _inputAddress1.gameObject.SetActive(isOn);
            _inputAddress2.gameObject.SetActive(isOn);
            _inputBusinessNumber.gameObject.SetActive(isOn);
            _inputCeo.gameObject.SetActive(isOn);
            _inputSector.gameObject.SetActive(isOn);
            _inputService.gameObject.SetActive(isOn);
            _inputTel.gameObject.SetActive(isOn);
            _inputHomepage.gameObject.SetActive(isOn);
        }

        public void SearchAddressResult(string result) 
        {
            string[] arrResult = result.Split('|');
            string addr = (arrResult.Length >= 1) ? arrResult[0] : string.Empty;

            _inputAddress1.text = addr;
            _inputAddress2.text = string.Empty;

            if (arrResult.Length >= 3) 
            {
                float temp = 0f;
                float.TryParse(arrResult[1], out temp);
                lat = temp;

                temp = 0f;
                float.TryParse(arrResult[2], out temp);
                lng = temp;
            }
            else 
            {
                lat = lng = 0f;
            }

            _inputAddress2.Select();
        }

    #endregion  // Event handling
    }
}