/// <summary>
/// [mobile]
/// 패치 노트 화면을 제어하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 0.4
/// @update
///     v0.1 : 최초 생성, 새로운 디자인 적용.
///     v0.2 : 디자인 수정안 적용.
///     v0.3 (2022. 12. 30) : UI Canvas 최적화 (static canvas, active canvas 분리)
///     v0.4 (2023. 03. 21) : FixedView 실험, UI Canvas 최적화. 
/// </summary>
 
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class PatchNoteM : FixedView
    {
        [SerializeField] private Button _btnBack;
        [SerializeField] private Scrollbar _scrollbar;


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
            viewID = ID.MobileScene_PatchNote;

            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing().Forget();
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            await UniTask.Yield();

            _scrollbar.value = 1;

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