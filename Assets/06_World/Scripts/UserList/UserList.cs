/// <summary>
/// [world]
/// 사용자 리스트 Script
/// @author         : HJ Lee
/// @last update    : 2023. 11. 07
/// @version        : 0.3
/// @update
///     v0.1 (2023. 09. 18) : 최초 생성
///     v0.2 (2023. 10. 06) : world avatar data class 에 정보 담아서 리스트 출력
///     v0.3 (2023. 11. 07) : WindowViewData 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class UserList : WindowView, iRepositoryObserver
    {
        private const string TAG = "UserList";

        [Header("module")]
        [SerializeField] private ContactModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("button")]
        [SerializeField] private Button _btnClose;

        [Header("contents")]
        [SerializeField] private InfiniteScroll _scrollView;

        // local variables
        private bool firstRequest;


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
            viewID = ID.USER_LIST_W;
            viewData = new WindowViewData();
            viewDataKey = $"view_data_{viewID}";


            // set infinite scrollview
            _scrollView.AddSelectCallback((data) => {
                Debug.Log($"{TAG} | 해당 사용자의 정보 출력. user seq : {((WorldAvatarData)data).info.seq}, name : {((WorldAvatarData)data).info.nickNm}");
            });


            // set 'search' inputfield listener
            _inputSearch.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            _btnClear.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
            });
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));


            // set button listener
            _btnClose.onClick.AddListener(() => {
                base.SaveViewData(viewData);
                Hide();
            });
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            // load view data
            base.LoadViewData();

            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, eStorageKey.WindowRefresh);
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
                R.singleton.UnregisterObserver(this, eStorageKey.WindowRefresh);
            }
        }

    #endregion  // WindowView functions


    #region Event handling

        private async UniTask<int> Refresh() 
        {
            WorldAvatarData t;

            _scrollView.Clear();
            Debug.Log($"{TAG} | avatar list count : {WorldAvatarList.avatarInfos.Count}");
            foreach (var info in WorldAvatarList.avatarInfos) 
            {
                t = new WorldAvatarData(info);
                _scrollView.InsertData(t);
            }
            _scrollView.UpdateAllData();

            await UniTask.Yield(); 
            _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(_inputSearch.text));
            return 0;
        }

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == eStorageKey.WindowRefresh) 
            {
                // TODO. refresh event 를 어떻게 처리할지 연구.
                // Refresh().Forget();
                Debug.Log($"{TAG} | UpdateInfo() call.");
            }
        }

    #endregion  // Event handling
    }
}