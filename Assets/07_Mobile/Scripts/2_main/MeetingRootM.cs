/// <summary>
/// [mobile]
/// 회의실 루트 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 19
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 19) : 최초 생성.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class MeetingRootM : FixedView
    {
        [Header("button")]
        [SerializeField] private Button _btnCreateMeeting;
        [SerializeField] private Button _btnMeetingList;
        [SerializeField] private Button _btnMeetingHistory;
        [SerializeField] private Button _btnCreateSeminar;
        [SerializeField] private Button _btnSeminarList;


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
            viewID = ID.MobileScene_MeetingRoot;

            // set button listener
            _btnCreateMeeting.onClick.AddListener(() => {
                // TODO. 미팅 생성 권한 체크.
                ViewManager.singleton.Push(S.MobileScene_MeetingCreate);
            });
            _btnMeetingList.onClick.AddListener(() => {
                // TODO. 미팅 정보 확인 권한 체크.
                ViewManager.singleton.Push(S.MobileScene_Meeting);
            });
            _btnMeetingHistory.onClick.AddListener(() => {
                // TODO. 미팅 정보 확인 권한 체크.
                ViewManager.singleton.Push(S.MobileScene_MeetingHistory);
            });
            _btnCreateSeminar.onClick.AddListener(() => {
                // TODO. 세미나 생성 권한 체크.
                ViewManager.singleton.Push(S.MobileScene_SeminarCreate);
            });
            _btnSeminarList.onClick.AddListener(() => {
                // TODO. 세미나 정보 확인 권한 체크.
                ViewManager.singleton.Push(S.MobileScene_Seminar);
            });
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling 

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(true);

            await UniTask.Yield();
            return 0;
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            if (PopupBuilder.singleton.GetPopupCount() > 0)
            {
                PopupBuilder.singleton.RequestClear();
            }
            else 
            {
                BackProcess();
            }
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