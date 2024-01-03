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

        [Header("전광판 - 좌2")]
        [SerializeField] private TMP_InputField _inputDisplayScreenLeft2;
        [SerializeField] private Button _btnDeleteDisplayScreenLeft2;
        [SerializeField] private Button _btnUploadDisplayScreenLeft2;
        [SerializeField] private RawImage _imgDisplayLeft2;
        [SerializeField] private TMP_InputField _inputDisplayLinkLeft2;
        private ImageUploader uploaderDisplayLeft2;

        [Header("전광판 - 중앙")]
        [SerializeField] private TMP_InputField _inputDisplayScreenCenter;
        [SerializeField] private Button _btnDeleteDisplayScreenCenter;
        [SerializeField] private Button _btnUploadDisplayScreenCenter;
        [SerializeField] private RawImage _imgDisplayCenter;
        [SerializeField] private TMP_InputField _inputDisplayLinkCenter;
        private ImageUploader uploaderDisplayCenter;

        [Header("전광판 - 우2")]
        [SerializeField] private TMP_InputField _inputDisplayScreenRight2;
        [SerializeField] private Button _btnDeleteDisplayScreenRight2;
        [SerializeField] private Button _btnUploadDisplayScreenRight2;
        [SerializeField] private RawImage _imgDisplayRight2;
        [SerializeField] private TMP_InputField _inputDisplayLinkRight2;
        private ImageUploader uploaderDisplayRight2;

        [Header("전광판 - 우1")]
        [SerializeField] private TMP_InputField _inputDisplayScreenRight1;
        [SerializeField] private Button _btnDeleteDisplayScreenRight1;
        [SerializeField] private Button _btnUploadDisplayScreenRight1;
        [SerializeField] private RawImage _imgDisplayRight1;
        [SerializeField] private TMP_InputField _inputDisplayLinkRight1;
        private ImageUploader uploaderDisplayRight1;

        [Header("홍보관 - 모니터 좌")]
        [SerializeField] private TMP_InputField _inputMonitorScreenLeft;
        [SerializeField] private Button _btnDeleteMonitorScreenLeft;
        [SerializeField] private Button _btnUploadMonitorScreenLeft;
        [SerializeField] private RawImage _imgMonitorScreenLeft;
        [SerializeField] private TMP_InputField _inputMonitorVideoLeft;
        private ImageUploader uploaderMonitorScreenLeft;

        [Header("홍보관 - 모니터 우")]
        [SerializeField] private TMP_InputField _inputMonitorScreenRight;
        [SerializeField] private Button _btnDeleteMonitorScreenRight;
        [SerializeField] private Button _btnUploadMonitorScreenRight;
        [SerializeField] private RawImage _imgMonitorScreenRight;
        [SerializeField] private TMP_InputField _inputMonitorVideoRight;
        private ImageUploader uploaderMonitorScreenRight;

        [Header("홍보관 - Social media")]
        [SerializeField] private TMP_InputField _inputYouTube;
        [SerializeField] private TMP_InputField _inputInstagram;
        [SerializeField] private TMP_InputField _inputBlog;
        [SerializeField] private TMP_InputField _inputHomepage;

        [Header("마스코트")]
        [SerializeField] private Toggle _toggleMascotUsage;
        [SerializeField] private TMP_InputField _inputMascotGreeting;

        // local variables


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


            // 1층 - 전광판 좌2 관련 event listener
            _btnDeleteDisplayScreenLeft2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 좌2 - 이미지 제거.");
            });
            _btnUploadDisplayScreenLeft2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 좌2 - 이미지 업로드.");
            });
            uploaderDisplayLeft2 = _btnUploadDisplayScreenLeft2.GetComponent<ImageUploader>();
            uploaderDisplayLeft2.Init();

            // 1층 - 전광판 중앙 관련 event listener
            _btnDeleteDisplayScreenCenter.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 중앙 - 이미지 제거.");
            });
            _btnUploadDisplayScreenCenter.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 중앙 - 이미지 업로드.");
            });
            uploaderDisplayCenter = _btnUploadDisplayScreenCenter.GetComponent<ImageUploader>();
            uploaderDisplayCenter.Init();

            // 1층 - 전광판 우2 관련 event listener
            _btnDeleteDisplayScreenRight2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우2 - 이미지 제거.");
            });
            _btnUploadDisplayScreenRight2.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우2 - 이미지 업로드.");
            });
            uploaderDisplayRight2 = _btnUploadDisplayScreenRight2.GetComponent<ImageUploader>();
            uploaderDisplayRight2.Init();

            // 1층 - 전광판 우1 관련 event listener
            _btnDeleteDisplayScreenRight1.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우1 - 이미지 제거.");
            });
            _btnUploadDisplayScreenRight1.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 전광판 - 우1 - 이미지 업로드.");
            });
            uploaderDisplayRight1 = _btnUploadDisplayScreenRight1.GetComponent<ImageUploader>();
            uploaderDisplayRight1.Init();

            // 홍보관 - 모니터 좌 관련 event listener
            _btnDeleteMonitorScreenLeft.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 좌 - 이미지 제거.");
            });
            _btnUploadMonitorScreenLeft.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 좌 - 이미지 업로드.");
            });
            uploaderMonitorScreenLeft = _btnUploadMonitorScreenLeft.GetComponent<ImageUploader>();
            uploaderMonitorScreenLeft.Init();

            // 홍보관 - 모니터 우 관련 event listener
            _btnDeleteMonitorScreenRight.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 우 - 이미지 제거.");
            });
            _btnUploadMonitorScreenRight.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 홍보관 - 모니터 우 - 이미지 업로드.");
            });
            uploaderMonitorScreenRight = _btnUploadMonitorScreenRight.GetComponent<ImageUploader>();
            uploaderMonitorScreenRight.Init();

            // 마스코트
            _toggleMascotUsage.onValueChanged.AddListener((isOn) => {
                _inputMascotGreeting.interactable = isOn; 
                Debug.Log($"{TAG} | 마스코트 사용 여부 : {isOn}");
            });
        } 

        public override async UniTaskVoid Show() 
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
            PsResponse<ResWorldOpt> res = await _module.GetWorldInfo();
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return -1;
            }
        }

    #endregion  // Event handling
    }
}