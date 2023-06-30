/// <summary>
/// [mobile]
/// 알람 목록 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 28
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 14) : 최초 생성
///     v0.2 (2023. 06. 28) : alarm api 모듈 연결
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class AlarmM : FixedView
    {
        private const string TAG = "AlarmM";

        [Header("module")]
        [SerializeField] private AlarmModule _module; 

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnTruncate;

        [Header("content")]
        [SerializeField] private TMP_Text _txtGuide;
        [SerializeField] private TMP_Text _txtCount;
        [SerializeField] private InfiniteScroll _scrollView;


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
            viewID = ID.MobileScene_Alarm;


            // set infinite scrollview
            _scrollView.AddSelectCallback(async (data) => {
                AlarmData t = (AlarmData)data;
                int seq = t.info.seq;
                string res = await _module.ReadAlarm(seq);
                if (string.IsNullOrEmpty(res)) 
                {
                    R.singleton.UnreadAlarmCount --;
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string form = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "미확인 알림", currentLocale);
                    string text = string.Format(form, R.singleton.UnreadAlarmCount);
                    _txtCount.text = text;

                    t.info.read = true; 
                    _scrollView.UpdateData(data);
                }
                else
                {
                    PopupBuilder.singleton.OpenAlert(res);
                }
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnTruncate.onClick.AddListener(() => {
                if (R.singleton.AlarmCount == 0) return;

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string text = LocalizationSettings.StringDatabase.GetLocalizedString("Alerts", "모든 알림 삭제 질문", currentLocale);
                PopupBuilder.singleton.OpenConfirm(text, async () => {
                    string res = await _module.TruncateAlarm();
                    if (string.IsNullOrEmpty(res)) 
                    {
                        _scrollView.Clear();     
                        R.singleton.ClearAlarmInfo();

                        _txtGuide.gameObject.SetActive(true);
                    }
                    else 
                    {
                        PopupBuilder.singleton.OpenAlert(res);
                    }
                });
            });
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for list

        private async UniTaskVoid GetList() 
        {
            PsResponse<ResAlarmList> res = await _module.GetAlarmList();

            _scrollView.Clear();
            R.singleton.ClearAlarmInfo();

            int unreadCnt = 0;
            if (string.IsNullOrEmpty(res.message)) 
            {
                AlarmData t;
                foreach (var item in res.data.list) 
                {
                    t = new AlarmData(item);
                    _scrollView.InsertData(t);

                    R.singleton.AddAlarmInfo(item);
                    if (! item.read) unreadCnt ++;
                }

                R.singleton.UnreadAlarmCount = unreadCnt;
                _txtGuide.gameObject.SetActive(res.data.list.Count == 0);
                _btnTruncate.gameObject.SetActive(res.data.list.Count > 0);

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string form = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "미확인 알림", currentLocale);
                string text = string.Format(form, unreadCnt);
                _txtCount.text = text;
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

    #endregion  // for list


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // get list
            GetList().Forget();            
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
            ViewManager.singleton.Pop();
        }

    #endregion  // event handling
    }
}