/// <summary>
/// [mobile]
/// FixedView Template Script
/// @author         : HJ Lee
/// @last update    : 2023. 05. 30
/// @version        : 0.1
/// @update
///     v0.1 (2022. 05. 30) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class TemplateM : FixedView
    {
        private const string TAG = "TemplateM";

        [SerializeField] private bool _showTopNavigation;
        [SerializeField] private bool _showBottomNavigation;
        [SerializeField] private Button _btnBack;
        [SerializeField] private int id;


    #region Unity functions

        private void Awake()
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        private void OnDestroy() 
        {
            if (MobileEvents.singleton != null) 
            {
                MobileEvents.singleton.OnBackButtonProcess -= BackButtonProcess;
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init()
        {
            base.Init();
            viewID = id;

            if (_btnBack != null) 
            {
                _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            }
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            if (_showTopNavigation && _showBottomNavigation)
                ViewManager.singleton.ShowNavigation(true);
            else if (! _showTopNavigation && _showBottomNavigation)
                ViewManager.singleton.ShowBottomNavigation();
            else
                ViewManager.singleton.ShowNavigation(false);

            await UniTask.Yield();
            return 0;
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return;
            if (visibleState != eVisibleState.Appeared) return;

            BackProcess();
        }

        private void BackProcess() 
        {
            ViewManager.singleton.Pop();
        }

    #endregion  // event handling
    }
}