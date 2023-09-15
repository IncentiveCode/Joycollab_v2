/// <summary>
/// [world]
/// 모임방 리스트 Script
/// @author         : HJ Lee
/// @last update    : 2023. 09. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 14) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class RoomList : WindowView, iRepositoryObserver
    {
        private const string TAG = "RoomList";
        
        [Header("module")]
        [SerializeField] private GatheringModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        // TODO. 추후 카테고리 관련 항목이 들어갈 수 있음. 지금은 고정값으로 처리.
        // [Header("category")]

        [Header("buttons")]
        [SerializeField] private Button _btnCreate;
        [SerializeField] private Button _btnClose;

        [Header("contents")]
        [SerializeField] private TMP_Text _txtCurrentCategory;
        [SerializeField] private Button _btnSortByRoomName;
        [SerializeField] private Button _btnSortByPublicOpt;
        [SerializeField] private Button _btnSortByOwnerName;
        [SerializeField] private InfiniteScroll _scrollView;

        // local variables
        private RequestForClas req;
        private bool firstRequest;
        private eStorageKey keyRefresh;
        
    
    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            keyRefresh = eStorageKey.WindowRefresh;
        }

    #endregion  // Unity functions


    #region WindowView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.ROOM_LIST_W;


            // set infinite scrollview
            _scrollView.AddSelectCallback((data) => {
                Debug.Log($"{TAG} | Load More");
            });


            // set 'search' inputfield listener
            _inputSearch.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            _btnClear.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
            });
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));


            // set button listener
            _btnCreate.onClick.AddListener(() => Debug.Log($"{TAG} | create window open."));
            _btnClose.onClick.AddListener(() => Hide());


            // set local variables
            req = new RequestForClas();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, keyRefresh);
            }

            firstRequest = true;

            await Refresh();
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            if (R.singleton != null) 
            {
                R.singleton.UnregisterObserver(this, keyRefresh);
            }
        }

    #endregion  // WindowView functions


    #region Event handling

        private async UniTask<int> Refresh() 
        {
            if (firstRequest) 
            {
                req.keyword = string.Empty;
                req.pageNo = 1;
                req.pageSize = 20;
                req.refresh = true;
                firstRequest = false;

                string res = await _module.GetRoomList(req, _scrollView);
                if (! string.IsNullOrEmpty(res)) 
                {
                    PopupBuilder.singleton.OpenAlert(res, () => Hide());
                }
                Debug.Log($"{TAG} | scroll view item count : {_scrollView.GetItemCount()}");
            }
            else 
            {
                Debug.Log($"{TAG} | update all data");
                _scrollView.UpdateAllData();
                await UniTask.Yield();
            }

            _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(_inputSearch.text));
            return 0;
        }

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == keyRefresh)
            {
                // TODO. refresh event 를 어떻게 처리할지 연구.
                // Refresh().Forget();
                Debug.Log($"{TAG} | UpdateInfo() call.");
            }
        }

    #endregion  // Event handling
    }
}