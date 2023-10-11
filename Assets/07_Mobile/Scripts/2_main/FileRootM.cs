/// <summary>
/// [mobile]
/// 파일함 루트 화면을 담당하는 클래스.
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
    public class FileRootM : FixedView
    {
        private const string TAG = "FileRootM";

        [Header("button")]
        [SerializeField] private Button _btnCompany;
        [SerializeField] private Button _btnDept;
        [SerializeField] private Button _btnPrivate;
        [SerializeField] private Button _btnDrive;


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
            viewID = ID.MobileScene_FileRoot;

            // set button listener
            _btnCompany.onClick.AddListener(() => {
                bool auth = R.singleton.CheckHasAuth(0, S.AUTH_READ_FILE);
                Debug.Log($"{TAG} | Init(), 전사 파일함 권한 확인 : {auth}");
                if (auth)
                {
                    ViewManager.singleton.Push(S.MobileScene_FileBox, "0|/");
                }
                else 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenAlert(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "권한오류.파일 조회", currentLocale)
                    );
                }
            });
            _btnDept.onClick.AddListener(() => {
                string spaceName = R.singleton.mySpaceName;
                int spaceSeq = R.singleton.mySpaceSeq;
                bool auth = R.singleton.CheckHasAuth(spaceSeq, S.AUTH_READ_FILE);
                Debug.Log($"{TAG} | Init(), 부서 파일함 ({spaceName}) 권한 확인 : {auth}");
                if (auth)
                {
                    string temp = string.Format("{0}|/", spaceSeq);
                    ViewManager.singleton.Push(S.MobileScene_FileBox, temp);
                }
                else 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenAlert(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "권한오류.파일 조회", currentLocale)
                    );
                }
            });
            _btnPrivate.onClick.AddListener(() => {
                ViewManager.singleton.Push(S.MobileScene_FileBox, "-1|/");
            });
            _btnDrive.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "기능 준비 안내", currentLocale)
                ); 
            });
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