/// <summary>
/// [world]
/// 모임방 생성 class
/// @author         : HJ Lee
/// @last update    : 2023. 09. 22
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 22) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class CreateRoom : WindowView
    {
        private const string TAG = "CreateRoom";
        
        [Header("module")]
        [SerializeField] private GatheringModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputTitle;
        [SerializeField] private TMP_InputField _inputDetail;
        [SerializeField] private TMP_InputField _inputThumbnail;

        [Header("dropdown")]
        [SerializeField] private Dropdown _dropdownCategory;

        [Header("theme option")]
        [SerializeField] private List<Toggle> _listThemeOption;
        /**
        [SerializeField] private Toggle _toggleCozy;
        [SerializeField] private Toggle _toggleLife;
        [SerializeField] private Toggle _toggleBnB;
        [SerializeField] private Toggle _toggleDebate;
        [SerializeField] private Toggle _toggleKstyle;
         */

        [Header("public option")]
        [SerializeField] private List<Toggle> _listPublicOption;
        /**
        [SerializeField] private Toggle _toggleOpen;
        [SerializeField] private Toggle _togglePublic;
        [SerializeField] private Toggle _togglePrivate;
         */

        [Header("Button")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnThumbnail;
        [SerializeField] private Button _btnCreate;
        [SerializeField] private Button _btnCancel;

        [Header("thumbnail popup")]
        [SerializeField] private GameObject _goThumbnailPopup;
        [SerializeField] private RawImage _imgThumbnail;
        [SerializeField] private Button _btnThumbnailUpload;
        [SerializeField] private Button _btnPopupConfirm;
        [SerializeField] private Button _btnPopupCancel;

        [Header("thumbnail option")]
        [SerializeField] private List<Toggle> _listThumbnailOption;
        /**
        [SerializeField] private Toggle _toggleThumbnail_0;
        [SerializeField] private Toggle _toggleThumbnail_1;
        [SerializeField] private Toggle _toggleThumbnail_2;
        [SerializeField] private Toggle _toggleThumbnail_3;
        [SerializeField] private Toggle _toggleThumbnail_4;
         */
        
        // local variables
        private ImageUploader uploader;
        private string lastThumbnail;


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
            viewID = ID.ROOM_CREATE_W;


            // set button listener
            _btnClose.onClick.AddListener(() => Hide());
            _btnThumbnail.onClick.AddListener(() => {
                lastThumbnail = _inputThumbnail.text;

                _goThumbnailPopup.SetActive(true);
            });
            _btnCreate.onClick.AddListener(() => {
                Debug.Log($"{TAG} | Create calll.");
                CreateAsync().Forget();
            });
            _btnCancel.onClick.AddListener(() => {
                Debug.Log($"{TAG} | Cancel calll.");
                Hide();
            });

            
            // set popup button listener
            _btnPopupConfirm.onClick.AddListener(() => {
                Debug.Log($"{TAG} | popup confirm call."); 

                _goThumbnailPopup.SetActive(false);
            });
            _btnPopupCancel.onClick.AddListener(() => {
                Debug.Log($"{TAG} | popup cancel call."); 

                _inputThumbnail.text = lastThumbnail;
                _goThumbnailPopup.SetActive(false);
            });


            // set local variables
            uploader = _btnThumbnailUpload.GetComponent<ImageUploader>();
            uploader.Init();
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

            ClearView();
        }

    #endregion  // WindowView functions


    #region Event Handling

        private async UniTask<int> Refresh() 
        {
            ClearView();

            PsResponse<TpsList> res = await _module.GetCategories();
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return -1;
            }
            SetCategory(res.data);

            return 0;
        }

        private void ClearView() 
        {
            // clear inputfield
            _inputTitle.text = _inputDetail.text = _inputThumbnail.text = string.Empty;
            
            // toggle setting
            if (_listThemeOption.Count > 0)     _listThemeOption[0].isOn = true;
            if (_listPublicOption.Count > 0)    _listPublicOption[0].isOn = true;
            if (_listThumbnailOption.Count > 0) _listThumbnailOption[0].isOn = true;

            // thumbnail popup close
            lastThumbnail = string.Empty;
            _goThumbnailPopup.SetActive(false);
        }

        private void SetCategory(TpsList data) 
        {
            _dropdownCategory.options.Clear();

            if (data.list.Count > 0)
            {
                foreach (var item in data.list) 
                {
                    _dropdownCategory.options.Add(new Dropdown.OptionData() { text = item.id });
                }
            }
            else 
            {
                _dropdownCategory.options.Add(new Dropdown.OptionData() { text = "카테고리 없음" });
            }

            _dropdownCategory.value = 0;
            _dropdownCategory.RefreshShownValue();
        }

        private async UniTaskVoid CreateAsync() 
        {
            await UniTask.Yield();
            return;
        }

    #endregion  // Event Handling
    }
}