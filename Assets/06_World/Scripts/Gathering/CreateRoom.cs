/// <summary>
/// [world]
/// 모임방 생성 class
/// @author         : HJ Lee
/// @last update    : 2023. 12. 22
/// @version        : 0.3
/// @update
///     v0.1 (2023. 09. 22) : 최초 생성
///     v0.2 (2023. 12. 18) : 생성 api 수정, view data 추가.
///     v0.3 (2023. 12. 22) : 카테고리, 테마 정보 누락되는 부분 수정.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
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
        private List<TpsInfo> listCategoryInfo;

        [Header("theme option")]
        // [SerializeField] private List<Toggle> _listThemeOption;
        [SerializeField] private Toggle _toggleCozy;
        [SerializeField] private Toggle _toggleLife;
        [SerializeField] private Toggle _toggleBnB;
        [SerializeField] private Toggle _toggleDebate;
        [SerializeField] private Toggle _toggleSupport;
        private List<TpsInfo> listThemeInfo;
        private int themeNo;

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
        [SerializeField] private List<Sprite> _listDefaultThumbnails;

        
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
            viewData = new WindowViewData();
            viewDataKey = $"view_data_{viewID}_{R.singleton.memberSeq}";


            // set button listener
            _btnClose.onClick.AddListener(() => Hide());
            _btnThumbnail.onClick.AddListener(() => {
                lastThumbnail = _inputThumbnail.text;
                _goThumbnailPopup.SetActive(true);
            });
            _btnCreate.onClick.AddListener(() => {
                CreateAsync().Forget();
            });
            _btnCancel.onClick.AddListener(() => {
                base.SaveViewData(viewData); 
                Hide();
            });


            // set toggle listener
            _toggleCozy.onValueChanged.AddListener((isOn) => {
                if (isOn) themeNo = 0;
            });
            _toggleLife.onValueChanged.AddListener((isOn) => {
                if (isOn) themeNo = 1;
            });
            _toggleBnB.onValueChanged.AddListener((isOn) => {
                if (isOn) themeNo = 2;
            });
            _toggleDebate.onValueChanged.AddListener((isOn) => {
                if (isOn) themeNo = 3;
            });
            _toggleSupport.onValueChanged.AddListener((isOn) => {
                if (isOn) themeNo = 4;
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

            listCategoryInfo = new List<TpsInfo>();
            listThemeInfo = new List<TpsInfo>();
            themeNo = 0;
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

            ClearView();
        }

    #endregion  // WindowView functions


    #region Event Handling

        private async UniTask<int> Refresh() 
        {
            ClearView();

            var (categoryRes, themesRes) = await UniTask.WhenAll(
                _module.GetCategories(),
                _module.GetRoomThemes()
            );

            if (! string.IsNullOrEmpty(categoryRes.message)) 
            {
                PopupBuilder.singleton.OpenAlert(categoryRes.message);
                return -1;
            }
            SetCategory(categoryRes.data);

            if (! string.IsNullOrEmpty(themesRes.message))
            {
                PopupBuilder.singleton.OpenAlert(themesRes.message);
                return -2;
            }
            SetThemes(themesRes.data);

            return 0;
        }

        private void ClearView() 
        {
            // clear inputfield
            _inputTitle.text = _inputDetail.text = _inputThumbnail.text = string.Empty;
            
            // toggle setting
            // if (_listThemeOption.Count > 0)     _listThemeOption[0].isOn = true;
            _toggleCozy.isOn = true;
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
                listCategoryInfo.Clear();
                foreach (var item in data.list) 
                {
                    _dropdownCategory.options.Add(new Dropdown.OptionData() { text = item.id });
                    listCategoryInfo.Add(item);
                }
            }
            else 
            {
                _dropdownCategory.options.Add(new Dropdown.OptionData() { text = "카테고리 없음" });
            }

            _dropdownCategory.value = 0;
            _dropdownCategory.RefreshShownValue();
        }

        private void SetThemes(TpsList data) 
        {
            if (data.list.Count > 0)
            {
                listThemeInfo.Clear();
                foreach (var item in data.list) 
                {
                    listThemeInfo.Add(item);
                }
            }
            else 
            {
                Debug.Log($"{TAG} | SetThemes(), theme 정보가 비어있음.");
            }
        }

        private async UniTaskVoid CreateAsync() 
        {
            if (string.IsNullOrEmpty(_inputTitle.text)) 
            {
                Debug.Log($"{TAG} | 모임방 이름을 입력하세요.");
                return;
            }

            RequestClas req = new RequestClas();
            req.nm = _inputTitle.text;
            req.logo = string.Empty;
            req.clas.category = new Cd(listCategoryInfo[_dropdownCategory.value].cd);
            req.clas.themes = new Cd(listThemeInfo[themeNo].cd);
            req.clas.bigo = _inputDetail.text;
            string openType = string.Empty;
            for (int i = 0; i < _listPublicOption.Count; i++) 
            {
                if (_listPublicOption[i].isOn) 
                {
                    openType = Util.EnumToString<eRoomOpenType>((eRoomOpenType)i);
                    break;
                }
            }
            req.clas.openType = openType;
            string body = JsonUtility.ToJson(req);

            PsResponse<string> res = await _module.CreateRoom(body);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            PopupBuilder.singleton.OpenAlert(
                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "모임방 생성 안내", R.singleton.CurrentLocale),
                () => { 
                    R.singleton.RequestNotify(eStorageKey.RoomList);
                    Hide();
                }
            );
        }

    #endregion  // Event Handling
    }
}