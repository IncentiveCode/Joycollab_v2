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

        [Header("module")]
        [SerializeField] private SettingModule _module;

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

        private ReqMemberInfo memberInfo;
        private ReqMemberCompanyInfo companyInfo;
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
            PsResponse<ResMemberInfo> res = await _module.GetMyInfo();
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return -1;
            }

            R.singleton.MemberInfo = res.data;
            memberInfo = new ReqMemberInfo(res.data);
            companyInfo = new ReqMemberCompanyInfo(res.data);
            
            imageUploader.Init();
            string photoPath = $"{URL.SERVER_PATH}{memberInfo.photo}";
            imageLoader.LoadProfile(photoPath, R.singleton.memberSeq).Forget();

            SetInputFieldActive(true);
            _txtId.text = R.singleton.myId; 
            _inputName.text = memberInfo.nickNm;; 
            _inputOffice.text = companyInfo.compName;
            _inputGrade.text = memberInfo.jobGrade;
            _inputPhone.text = memberInfo.tel; 
            _inputAddress1.text = memberInfo.addr;
            _inputAddress2.text = memberInfo.addrDtl;

            _inputBusinessNumber.text = companyInfo.businessNum; 
            _inputCeo.text = companyInfo.ceoNm; 
            _inputSector.text = companyInfo.business; 
            _inputService.text = companyInfo.mainBusiness; 
            _inputTel.text = companyInfo.tel; 
            _inputHomepage.text = companyInfo.homepage;

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
                memberInfo.photo = imageUploader.imageInfo;

            memberInfo.nickNm = _inputName.text;
            memberInfo.jobGrade = _inputGrade.text;
            memberInfo.tel = RegExp.ReplaceOnlyNumber(_inputPhone.text);
            memberInfo.addr = _inputAddress1.text;
            memberInfo.addrDtl = _inputAddress2.text;
            memberInfo.lat = lat;
            memberInfo.lng = lng;

            companyInfo.compName = _inputOffice.text;
            companyInfo.businessNum = _inputBusinessNumber.text;
            companyInfo.ceoNm = _inputCeo.text;
            companyInfo.business = _inputSector.text;
            companyInfo.mainBusiness = _inputService.text;
            companyInfo.tel = RegExp.ReplaceOnlyNumber(_inputTel.text);
            companyInfo.homepage = _inputHomepage.text;

            string memberInfoBody = JsonUtility.ToJson(memberInfo);
            string companyInfoBody = JsonUtility.ToJson(companyInfo);

            var (memberRes, companyRes) = await UniTask.WhenAll(
                _module.UpdateMyInfo(memberInfoBody),
                _module.UpdateMyCompanyInfo(companyInfoBody)
            );

            if (! string.IsNullOrEmpty(memberRes))
                return memberRes;
            
            if (! string.IsNullOrEmpty(companyRes))
                return companyRes;

            return string.Empty;
        }

    #endregion  // Event handling
    }
}