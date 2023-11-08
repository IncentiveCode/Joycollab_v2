/// <summary>
/// Bookmark > member page Script
/// @author         : HJ Lee
/// @last update    : 2023. 11. 08
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 08) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class BookmarkMember : WindowView, iRepositoryObserver
    {
        private const string TAG = "BookmarkMember";

        [Header("contents")]
        [SerializeField] private InfiniteScroll _scrollView;


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


            // set infinite scroll view
            _scrollView.AddSelectCallback((data) => {
                Debug.Log($"{TAG} | 해당 사용자 상세 프로필 출력.");
                // Debug.Log($"{TAG} | 해당 사용자 상세 프로필 출력. member seq : {((WorldAvatarData)data).info.seq}, name : {((WorldAvatarData)data).info.nickNm}");
            });
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

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
                R.singleton.UnregisterObserver(this, eStorageKey.WindowRefresh);
            }
        }

    #endregion  // WindowPage functions


    #region Event handling

        private async UniTask<int> Refresh() 
        {
            await UniTask.Yield();
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