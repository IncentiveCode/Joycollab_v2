/// <summary>
/// 즐겨찾기 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 11. 08.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 08) : 최초 생성, v1 & mobile 의 내용 수정 & 기능 확장 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class BookmarkList : WindowView, iRepositoryObserver
    {
        private const string TAG = "BookmarkList";

        // [Header("module")]

        [Header("toggle")]
        [SerializeField] private Toggle _toggleMember;
        [SerializeField] private Toggle _toggleSeminar;
        [SerializeField] private Toggle _toggleBoard;

        [Header("button")]
        [SerializeField] private Button _btnClose;

        [Header("contents")]
        [SerializeField] private InfiniteScroll _scrollView;


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
            viewID = ID.BOOKMARK_W;
            viewData = new WindowViewData();
            viewDataKey = $"view_data_{viewID}";


            // set button listener
            _btnClose.onClick.AddListener(() => {
                base.SaveViewData(viewData);
                Hide();
            });


            // set toggle listener
            _toggleMember.onValueChanged.AddListener((isOn) => {
                if (!isOn) return;
                Debug.Log($"{TAG} | open member list.");
            });
            _toggleSeminar.onValueChanged.AddListener((isOn) => {
                if (!isOn) return;
                Debug.Log($"{TAG} | open seminar list.");
            });
            _toggleBoard.onValueChanged.AddListener((isOn) => {
                if (!isOn) return;
                Debug.Log($"{TAG} | open board list.");
            });
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();

            // load view data
            base.LoadViewData();

            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, eStorageKey.WindowRefresh);
            }

            await Refresh();
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, eStorageKey.WindowRefresh);
            }
        }

    #endregion  // WindowView functions


    #region Event handling

        private async UniTask<int> Refresh() 
        {
            await UniTask.Yield();
            return 0;
        }

        public void UpdateInfo(eStorageKey key) 
        {
            Debug.Log($"{TAG} | UpdateInfo() call.");
        }

    #endregion  // Event handling
    }
}