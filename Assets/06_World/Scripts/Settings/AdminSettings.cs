/// <summary>
/// [world]
/// 환경설정 _ 관리자 설정 Script
/// @author         : HJ Lee
/// @last update    : 2024. 01. 02
/// @version        : 0.1
/// @update
///     v0.1 (2024. 01. 02) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class AdminSettings : WindowPage
    {
        private const string TAG = "AdminSettings";

        [Header("module")]
        [SerializeField] private SettingModule _module;

        [Header("전광판 - 좌1")]
        [SerializeField] private TMP_InputField _inputDisplayScreenLeft1;
        [SerializeField] private Button _btnDeleteDisplayScreenLeft1;
        [SerializeField] private Button _btnUploadDisplayScreenLeft1;
        [SerializeField] private RawImage _imgDisplayLeft1;
        [SerializeField] private TMP_InputField _inputDisplayLinkLeft1;
        private ImageUploader uploaderDisplayLeft1;
        private ImageLoader loaderDisplayLeft1;

        [Header("전광판 - 좌2")]
        [SerializeField] private TMP_InputField _inputDisplayScreenLeft2;
        [SerializeField] private Button _btnDeleteDisplayScreenLeft2;
        [SerializeField] private Button _btnUploadDisplayScreenLeft2;
        [SerializeField] private RawImage _imgDisplayLeft2;
        [SerializeField] private TMP_InputField _inputDisplayLinkLeft2;
        private ImageUploader uploaderDisplayLeft2;
        private ImageLoader loaderDisplayLeft2;

        [Header("전광판 - 중앙")]
        [SerializeField] private TMP_InputField _inputDisplayScreenCenter;
        [SerializeField] private Button _btnDeleteDisplayScreenCenter;
        [SerializeField] private Button _btnUploadDisplayScreenCenter;
        [SerializeField] private RawImage _imgDisplayCenter;
        [SerializeField] private TMP_InputField _inputDisplayLinkCenter;
        private ImageUploader uploaderDisplayCenter;
        private ImageLoader loaderDisplayCenter;

        [Header("전광판 - 우2")]
        [SerializeField] private TMP_InputField _inputDisplayScreenRight2;
        [SerializeField] private Button _btnDeleteDisplayScreenRight2;
        [SerializeField] private Button _btnUploadDisplayScreenRight2;
        [SerializeField] private RawImage _imgDisplayRight2;
        [SerializeField] private TMP_InputField _inputDisplayLinkRight2;
        private ImageUploader uploaderDisplayRight2;
        private ImageLoader loaderDisplayRight2;

        [Header("전광판 - 우1")]
        [SerializeField] private TMP_InputField _inputDisplayScreenRight1;
        [SerializeField] private Button _btnDeleteDisplayScreenRight1;
        [SerializeField] private Button _btnUploadDisplayScreenRight1;
        [SerializeField] private RawImage _imgDisplayRight1;
        [SerializeField] private TMP_InputField _inputDisplayLinkRight1;
        private ImageUploader uploaderDisplayRight1;
        private ImageLoader loaderDisplayRight1;

        [Header("홍보관 - 모니터 좌")]
        [SerializeField] private TMP_InputField _inputMonitorScreenLeft;
        [SerializeField] private Button _btnDeleteMonitorScreenLeft;
        [SerializeField] private Button _btnUploadMonitorScreenLeft;
        [SerializeField] private RawImage _imgMonitorScreenLeft;
        [SerializeField] private TMP_InputField _inputMonitorVideoLeft;
        private ImageUploader uploaderMonitorScreenLeft;
        private ImageLoader loaderMonitorScreenLeft;

        [Header("홍보관 - 모니터 우")]
        [SerializeField] private TMP_InputField _inputMonitorScreenRight;
        [SerializeField] private Button _btnDeleteMonitorScreenRight;
        [SerializeField] private Button _btnUploadMonitorScreenRight;
        [SerializeField] private RawImage _imgMonitorScreenRight;
        [SerializeField] private TMP_InputField _inputMonitorVideoRight;
        private ImageUploader uploaderMonitorScreenRight;
        private ImageLoader loaderMonitorScreenRight;

        [Header("홍보관 - Social media")]
        [SerializeField] private TMP_InputField _inputYouTube;
        [SerializeField] private TMP_InputField _inputInstagram;
        [SerializeField] private TMP_InputField _inputBlog;
        [SerializeField] private TMP_InputField _inputHomepage;

        [Header("마스코트")]
        [SerializeField] private Toggle _toggleMascotUsage;
        [SerializeField] private TMP_InputField _inputMascotGreeting;

        // local variables
        private WorldOption worldOpt;


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


            // 1층 - 전광판 좌1 관련 event listener
            _btnDeleteDisplayScreenLeft1.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 좌1 - 이미지 제거.");
            });
            _btnUploadDisplayScreenLeft1.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 좌1 - 이미지 업로드.");
            });
            uploaderDisplayLeft1 = _btnUploadDisplayScreenLeft1.GetComponent<ImageUploader>();
            uploaderDisplayLeft1.Init();
            loaderDisplayLeft1 = _imgDisplayLeft1.GetComponent<ImageLoader>();


            // 1층 - 전광판 좌2 관련 event listener
            _btnDeleteDisplayScreenLeft2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 좌2 - 이미지 제거.");
            });
            _btnUploadDisplayScreenLeft2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 좌2 - 이미지 업로드.");
            });
            uploaderDisplayLeft2 = _btnUploadDisplayScreenLeft2.GetComponent<ImageUploader>();
            uploaderDisplayLeft2.Init();
            loaderDisplayLeft2 = _imgDisplayLeft2.GetComponent<ImageLoader>();

            // 1층 - 전광판 중앙 관련 event listener
            _btnDeleteDisplayScreenCenter.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 중앙 - 이미지 제거.");
            });
            _btnUploadDisplayScreenCenter.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 중앙 - 이미지 업로드.");
            });
            uploaderDisplayCenter = _btnUploadDisplayScreenCenter.GetComponent<ImageUploader>();
            uploaderDisplayCenter.Init();
            loaderDisplayCenter = _imgDisplayCenter.GetComponent<ImageLoader>();

            // 1층 - 전광판 우2 관련 event listener
            _btnDeleteDisplayScreenRight2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우2 - 이미지 제거.");
            });
            _btnUploadDisplayScreenRight2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우2 - 이미지 업로드.");
            });
            uploaderDisplayRight2 = _btnUploadDisplayScreenRight2.GetComponent<ImageUploader>();
            uploaderDisplayRight2.Init();
            loaderDisplayRight2 = _imgDisplayRight2.GetComponent<ImageLoader>();

            // 1층 - 전광판 우1 관련 event listener
            _btnDeleteDisplayScreenRight1.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우1 - 이미지 제거.");
            });
            _btnUploadDisplayScreenRight1.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우1 - 이미지 업로드.");
            });
            uploaderDisplayRight1 = _btnUploadDisplayScreenRight1.GetComponent<ImageUploader>();
            uploaderDisplayRight1.Init();
            loaderDisplayRight1 = _imgDisplayRight1.GetComponent<ImageLoader>();

            // 홍보관 - 모니터 좌 관련 event listener
            _btnDeleteMonitorScreenLeft.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 좌 - 이미지 제거.");
            });
            _btnUploadMonitorScreenLeft.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 좌 - 이미지 업로드.");
            });
            uploaderMonitorScreenLeft = _btnUploadMonitorScreenLeft.GetComponent<ImageUploader>();
            uploaderMonitorScreenLeft.Init();
            loaderMonitorScreenLeft = _imgMonitorScreenLeft.GetComponent<ImageLoader>();

            // 홍보관 - 모니터 우 관련 event listener
            _btnDeleteMonitorScreenRight.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 우 - 이미지 제거.");
            });
            _btnUploadMonitorScreenRight.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 우 - 이미지 업로드.");
            });
            uploaderMonitorScreenRight = _btnUploadMonitorScreenRight.GetComponent<ImageUploader>();
            uploaderMonitorScreenRight.Init();
            loaderMonitorScreenRight = _imgMonitorScreenRight.GetComponent<ImageLoader>();

            // 마스코트
            _toggleMascotUsage.onValueChanged.AddListener((isOn) => {
                _inputMascotGreeting.interactable = isOn; 
                Debug.Log($"{TAG} | 마스코트 사용 여부 : {isOn}");
            });
        } 

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();

            SetInputFieldActive(true);
            await UniTask.Yield();

            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            SetInputFieldActive(false);
        }

    #endregion  // WindowPage functions


    #region Event handling

        public async UniTask<int> Refresh() 
        {
            PsResponse<WorldOption> res = await _module.GetWorldInfo();
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return -3;
            }

            R.singleton.WorldOpt = res.data;
            worldOpt = new WorldOption(res.data);

            // input field setting
            _inputDisplayScreenLeft1.text = res.data.billboardL1;
            _inputDisplayLinkLeft1.text = res.data.billboardL1Url;
            _inputDisplayScreenLeft2.text = res.data.billboardL2;
            _inputDisplayLinkLeft2.text = res.data.billboardL2Url;
            _inputDisplayScreenCenter.text = res.data.billboard;
            _inputDisplayLinkCenter.text = res.data.billboardUrl;
            _inputDisplayScreenRight2.text = res.data.billboardR2;
            _inputDisplayLinkRight2.text = res.data.billboardR2Url;
            _inputDisplayScreenRight1.text = res.data.billboardR1;
            _inputDisplayLinkRight1.text = res.data.billboardR1Url;
            _inputMonitorScreenLeft.text = res.data.monitL;
            _inputMonitorVideoLeft.text = res.data.monitLUrl;
            _inputMonitorScreenRight.text = res.data.monitR;
            _inputMonitorVideoRight.text = res.data.monitRUrl;
            _inputYouTube.text = res.data.youtubeUrl;
            _inputInstagram.text = res.data.instagramUrl;
            _inputBlog.text = res.data.blogUrl;
            _inputHomepage.text = res.data.homepUrl;
            _inputMascotGreeting.text = res.data.seminarMascot;
            SetInputFieldActive(true);

            // image loader, uploader refresh 
            string imagePath = string.Empty;
            if (! string.IsNullOrEmpty(res.data.billboardL1))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.billboardL1}";
                loaderDisplayLeft1.LoadImage(imagePath).Forget();
            }
            uploaderDisplayLeft1.Init();

            if (! string.IsNullOrEmpty(res.data.billboardL2))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.billboardL2}";
                loaderDisplayLeft2.LoadImage(imagePath).Forget();
            }
            uploaderDisplayLeft2.Init();

            if (! string.IsNullOrEmpty(res.data.billboard))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.billboard}";
                loaderDisplayCenter.LoadImage(imagePath).Forget();
            }
            uploaderDisplayCenter.Init();

            if (! string.IsNullOrEmpty(res.data.billboardR2))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.billboardR2}";
                loaderDisplayRight2.LoadImage(imagePath).Forget();
            }
            uploaderDisplayRight2.Init();

            if (! string.IsNullOrEmpty(res.data.billboardR1))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.billboardR1}";
                loaderDisplayRight1.LoadImage(imagePath).Forget();
            }
            uploaderDisplayRight1.Init();

            if (! string.IsNullOrEmpty(res.data.monitL))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.monitL}";
                loaderMonitorScreenLeft.LoadImage(imagePath).Forget();
            }
            uploaderMonitorScreenLeft.Init();

            if (! string.IsNullOrEmpty(res.data.monitR))
            {
                imagePath = $"{URL.SERVER_PATH}{res.data.monitR}";
                loaderMonitorScreenRight.LoadImage(imagePath).Forget();
            }
            uploaderMonitorScreenRight.Init();

            return 0;
        }

        private void SetInputFieldActive(bool isOn) 
        {
            _inputDisplayScreenLeft1.gameObject.SetActive(isOn);
            _inputDisplayLinkLeft1.gameObject.SetActive(isOn);
            _inputDisplayScreenLeft2.gameObject.SetActive(isOn);
            _inputDisplayLinkLeft2.gameObject.SetActive(isOn);
            _inputDisplayScreenCenter.gameObject.SetActive(isOn);
            _inputDisplayLinkCenter.gameObject.SetActive(isOn);
            _inputDisplayScreenRight2.gameObject.SetActive(isOn);
            _inputDisplayLinkRight2.gameObject.SetActive(isOn);
            _inputDisplayScreenRight1.gameObject.SetActive(isOn);
            _inputDisplayLinkRight1.gameObject.SetActive(isOn);
            _inputMonitorScreenLeft.gameObject.SetActive(isOn);
            _inputMonitorVideoLeft.gameObject.SetActive(isOn);
            _inputMonitorScreenRight.gameObject.SetActive(isOn);
            _inputMonitorVideoRight.gameObject.SetActive(isOn);
            _inputYouTube.gameObject.SetActive(isOn);
            _inputInstagram.gameObject.SetActive(isOn);
            _inputBlog.gameObject.SetActive(isOn);
            _inputHomepage.gameObject.SetActive(isOn);
            _inputMascotGreeting.gameObject.SetActive(isOn);
        }

        public async UniTask<string> UpdateCenterInfo() 
        {
            // image uploader check
            if (! string.IsNullOrEmpty(uploaderDisplayLeft1.imageInfo))
            {
                Debug.Log($"{TAG} | UpdateCenterInfo(), image info : {uploaderDisplayLeft1.imageInfo}");
                worldOpt.billboardL1 = uploaderDisplayLeft1.imageInfo;
            }

            if (! string.IsNullOrEmpty(uploaderDisplayLeft2.imageInfo))
                worldOpt.billboardL2 = uploaderDisplayLeft2.imageInfo;

            if (! string.IsNullOrEmpty(uploaderDisplayCenter.imageInfo))
                worldOpt.billboard = uploaderDisplayCenter.imageInfo;

            if (! string.IsNullOrEmpty(uploaderDisplayRight2.imageInfo))
                worldOpt.billboardR2 = uploaderDisplayRight2.imageInfo;

            if (! string.IsNullOrEmpty(uploaderDisplayRight1.imageInfo))
                worldOpt.billboardR1 = uploaderDisplayRight1.imageInfo;

            if (! string.IsNullOrEmpty(uploaderMonitorScreenLeft.imageInfo))
                worldOpt.monitL = uploaderMonitorScreenLeft.imageInfo;

            if (! string.IsNullOrEmpty(uploaderMonitorScreenRight.imageInfo))
                worldOpt.monitR = uploaderMonitorScreenRight.imageInfo;

            // 전광판
            worldOpt.billboardL1Url = _inputDisplayLinkLeft1.text;
            worldOpt.billboardL2Url = _inputDisplayLinkLeft2.text;
            worldOpt.billboardUrl = _inputDisplayLinkCenter.text;
            worldOpt.billboardR2Url = _inputDisplayLinkRight2.text;
            worldOpt.billboardR1Url = _inputDisplayLinkRight1.text;

            // 모니터
            worldOpt.monitLUrl = _inputMonitorVideoLeft.text;
            worldOpt.monitRUrl = _inputMonitorVideoRight.text;

            // SNS
            worldOpt.youtubeUrl = _inputYouTube.text;
            worldOpt.instagramUrl = _inputInstagram.text;
            worldOpt.blogUrl = _inputBlog.text;
            worldOpt.homepUrl = _inputHomepage.text;

            // 마스코트
            worldOpt.seminarMascotIsUse = _toggleMascotUsage.isOn;
            worldOpt.seminarMascot = _inputMascotGreeting.text;

            // api call
            string body = JsonUtility.ToJson(worldOpt);
            var res = await _module.UpdateCenterOptions(body);
            return res;
        }

    #endregion  // Event handling
    }
}