/// <summary>
/// [mobile]
/// 내 자리 화면을 제어하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 05. 25
/// @version        : 0.5
/// @update         
///     v0.1 : 최초 생성. swipe ui 사용.
///     v0.2 : 새로운 기획 & 디자인 적용. swipe ui 삭제.
///     v0.3 (2022. 05. 26) : 디자인 수정안 적용, 이벤트 핸들러 추가.
///     v0.4 (2023. 03. 23) : 디자인 최적화. FixedView 실험. 스크립트 최적화, To-Do 합병 & calendar 삭제.
///     v0.5 (2023. 05. 25) : unity localization 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class MySeatM : FixedView
    {
        private const string TAG = "MySeatM";

        [Header("office info")]
        [SerializeField] private Text _txtOfficeName;

        [Header("menu")]
        [SerializeField] private Button _btnTodo;
        [SerializeField] private Button _btnBoard;
        [SerializeField] private Button _btnBookmark;
        [SerializeField] private Button _btnContact;


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

    #endregion


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_MySeat;

            // set button listener
            _btnTodo.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_ToDo));
            _btnBoard.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Board));
            _btnBookmark.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Bookmark));
            _btnContact.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_Contact));
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
            // navigation control
            ViewManager.singleton.ShowNavigation(true);

            _txtOfficeName.text = JsLib.GetCookie(Key.WORKSPACE_NAME);

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
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            PopupBuilder.singleton.OpenConfirm(
                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "앱 종료 확인", currentLocale),
                () => Application.Quit()
            );
        }
    #endregion  // event handling
    }
}