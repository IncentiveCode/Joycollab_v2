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
using UnityEngine.Localization.Settings;
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

        private ResMemberInfo currentInfo;
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
            currentInfo = JsonUtility.FromJson<ResMemberInfo>(R.singleton.myInfoSerialize);

            imageUploader.Init();

            string url = $"{URL.SERVER_PATH}{currentInfo.photo}";
            imageLoader.LoadProfile(url, R.singleton.memberSeq).Forget();

            SetInputFieldActive(true);
            _txtId.text = currentInfo.user.id;
            _inputName.text = currentInfo.nickNm;; 
            _inputOffice.text = currentInfo.compName;
            _inputGrade.text = currentInfo.jobGrade;
            _inputPhone.text = currentInfo.user.tel; 
            _inputAddress1.text = currentInfo.addr;
            _inputAddress2.text = currentInfo.addrDtl;

            _inputBusinessNumber.text = currentInfo.businessNum; 
            _inputCeo.text = currentInfo.ceoNm; 
            _inputSector.text = currentInfo.business; 
            _inputService.text = currentInfo.mainBusiness; 
            _inputTel.text = currentInfo.tel; 
            _inputHomepage.text = currentInfo.homepage;

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

        public async UniTask<string> UpdateMyInfo() 
        {
            if (! string.IsNullOrEmpty(imageUploader.imageInfo))
                currentInfo.photo = imageUploader.imageInfo;

            currentInfo.nickNm = _inputName.text;
            currentInfo.compName = _inputOffice.text;
            currentInfo.jobGrade = _inputGrade.text;
            currentInfo.user.tel = RegExp.ReplaceOnlyNumber(_inputPhone.text);
            currentInfo.addr = _inputAddress1.text;
            currentInfo.addrDtl = _inputAddress2.text;

            currentInfo.businessNum = _inputBusinessNumber.text;
            currentInfo.ceoNm = _inputCeo.text;
            currentInfo.business = _inputSector.text;
            currentInfo.mainBusiness = _inputService.text;
            currentInfo.tel = RegExp.ReplaceOnlyNumber(_inputTel.text);
            currentInfo.homepage = _inputHomepage.text;

            string url = string.Format(URL.MEMBER_INFO, R.singleton.memberSeq);
            string body = JsonUtility.ToJson(currentInfo);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, R.singleton.token);

            return res.message;
        }

    #endregion  // Event handling
    }
}