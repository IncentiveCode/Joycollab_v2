/// <summary>
/// 내 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 11. 15.
/// @version        : 0.2
/// @update
///     v0.1 (2023. 11. 02) : MemberProfile 에서 edit 가능한 파트를 분리. 
///     v0.2 (2023. 11. 15) : TAG 와 hiddenTel 기능 추가.
/// </summary>

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class MyProfile : WindowView
    {
        private const string TAG = "MyProfile";
        private const string CALLBACK = "SearchAddressResult";

        [Header("module")] 
        [SerializeField] private SettingModule _settingModule;

        [Header("profile")]
        [SerializeField] private Button _btnProfile;
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private Dropdown _dropdownState;
        [SerializeField] private Image _imgState;
        [SerializeField] private LocalizeStringEvent _txtState;
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtInfo;

        [Header("button")]
        [SerializeField] private Button _btnSave;
        [SerializeField] private Button _btnClose;

        [Header("input")]
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private TMP_InputField _inputOffice;
        [SerializeField] private TMP_InputField _inputGrade;
        [SerializeField] private TMP_InputField _inputPhone;
        [SerializeField] private Toggle _toggleHiddenTel;
        [SerializeField] private TMP_InputField _inputTag;
        [SerializeField] private Button _btnSearchAddress;
        [SerializeField] private TMP_InputField _inputAddress1;
        [SerializeField] private TMP_InputField _inputAddress2;

        // tools
        private ImageUploader uploader;
        private ImageLoader loader;
        private StringBuilder builder;

        // local variables
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


    #region WindowView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MY_PROFILE_W;
            viewData = new WindowViewData();
            viewDataKey = $"view_data_{viewID}";


            // set button listener
            _btnSave.onClick.AddListener(async () => {
                base.SaveViewData(viewData);

                canvasGroup.interactable = false;
                Debug.Log($"{TAG} | 저장 시작.");

                string res = await UpdateMyInfo();
                if (! string.IsNullOrEmpty(res))
                {
                    PopupBuilder.singleton.OpenAlert(res);
                    return;
                }

                if (!string.IsNullOrEmpty(uploader.ImageUrl))
                {
                    Debug.Log($"{TAG} | 변경된 사진 정보가 있다면 R 에 저장. url : {uploader.ImageUrl}");
                    // R.singleton.AddPhoto(uploader.ImageUrl, (Texture2D)_imgProfile.texture); 
                    R.singleton.AddPhoto(R.singleton.memberSeq, (Texture2D)_imgProfile.texture); 
                }

                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "환경설정.변경 완료 안내", R.singleton.CurrentLocale)
                );
                Show().Forget();

                Debug.Log($"{TAG} | 저장 종료.");
                canvasGroup.interactable = true;
            });
            _btnClose.onClick.AddListener(() => {
                base.SaveViewData(viewData);

                Debug.Log($"{TAG} | 수정 중지. confirm 창 출력 후, 동의하면 창 닫기.");
                Hide();
            });
            _btnSearchAddress.onClick.AddListener(() => JsLib.SearchAddress(this.name, CALLBACK));


            // init local variables
            uploader = _btnProfile.GetComponent<ImageUploader>();
            uploader.Init();
            loader = _imgProfile.GetComponent<ImageLoader>();
            builder = new StringBuilder();
            builder.Clear();
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();

            // load view data
            base.LoadViewData();

            await Refresh();
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            SetInputFieldActive(false);
        }

    #endregion  // WindowView functions


    #region Event handling

        private async UniTask<int> Refresh() 
        {
            PsResponse<ResMemberInfo> myInfo = await _settingModule.GetMyInfo();
            if (! string.IsNullOrEmpty(myInfo.message)) 
            {
                Debug.Log($"{TAG} | 내 정보 출력 오류 : {myInfo.message}");
                PopupBuilder.singleton.OpenAlert(myInfo.message);
                return -1;
            }

            DisplayInfo(myInfo.data);
            SetInputFieldActive(true);
            
            return 0;
        }

        private void SetInputFieldActive(bool isOn) 
        {
            _inputName.gameObject.SetActive(isOn);
            _inputOffice.gameObject.SetActive(isOn);
            _inputGrade.gameObject.SetActive(isOn);
            _inputPhone.gameObject.SetActive(isOn);
            _inputTag.gameObject.SetActive(isOn);
            _inputAddress1.gameObject.SetActive(isOn);
            _inputAddress2.gameObject.SetActive(isOn);
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

        private void DisplayInfo(ResMemberInfo info) 
        {
            R.singleton.MemberInfo = info;
            Debug.Log($"{TAG} | tag : {info.tag}");
            memberInfo = new ReqMemberInfo(info);
            companyInfo = new ReqMemberCompanyInfo(info);

            // profile image 
            string photoPath = $"{URL.SERVER_PATH}{info.photo}";
            loader.LoadProfile(photoPath, info.seq).Forget();

            // text
            _txtName.text = info.nickNm;
            builder.Clear();
            builder.Append($"{info.compName} / {info.jobGrade} \n");
            builder.Append($"{info.user.id} \n");
            builder.Append(info.user.tel);
            _txtInfo.text = builder.ToString();

            // state
            _imgState.sprite = SystemManager.singleton.GetStateIcon(info.status.id);
            _txtState.StringReference.SetReference("Word", $"상태.{info.status.id}");

            // input field
            _inputName.text = memberInfo.nickNm;
            _inputOffice.text = companyInfo.compName;
            _inputGrade.text = memberInfo.jobGrade;
            _inputPhone.text = memberInfo.tel;
            _toggleHiddenTel.isOn = memberInfo.hiddenTel;
            _inputTag.text = memberInfo.tag;
            _inputAddress1.text = memberInfo.addr;
            _inputAddress2.text = memberInfo.addrDtl;
        }

        public async UniTask<string> UpdateMyInfo() 
        {
            if (! string.IsNullOrEmpty(uploader.imageInfo)) 
                memberInfo.photo = uploader.imageInfo;

            memberInfo.nickNm = _inputName.text;
            memberInfo.jobGrade = _inputGrade.text;
            memberInfo.tel = RegExp.ReplaceOnlyNumber(_inputPhone.text);
            memberInfo.hiddenTel = _toggleHiddenTel.isOn;
            memberInfo.tag = _inputTag.text;
            memberInfo.addr = _inputAddress1.text;
            memberInfo.addrDtl = _inputAddress2.text;
            memberInfo.lat = lat;
            memberInfo.lng = lng;

            companyInfo.compName = _inputOffice.text;

            string memberInfoBody = JsonUtility.ToJson(memberInfo);
            string companyInfoBody = JsonUtility.ToJson(companyInfo);

            var (memberRes, companyRes) = await UniTask.WhenAll(
                _settingModule.UpdateMyInfo(memberInfoBody),
                _settingModule.UpdateMyCompanyInfo(companyInfoBody)
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