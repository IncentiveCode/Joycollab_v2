/// <summary>
/// [mobile]
/// 알람 목록 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 21
/// @version        : 0.3
/// @update
///     v0.1 (2023. 06. 14) : 최초 생성
///     v0.2 (2023. 06. 28) : alarm api 모듈 연결
///     v0.3 (2023. 07. 21) : iRepositoryObserver 추가.
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
    public class AlarmM : FixedView, iRepositoryObserver
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

            R.singleton.RegisterObserver(this, eAlarmKey);

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        #if UNITY_ANDROID
        private void Update() 
        {
            if (AndroidSelectCallback.ViewID == viewID && AndroidSelectCallback.isUpdated) 
            {
                if (visibleState != eVisibleState.Appeared) return;
                if (AndroidSelectCallback.extraData.Count < 3)
                {
                    Debug.Log($"{TAG} | android select callback 의 extra data 가 부족하게 넘어왔음");
                    AndroidSelectCallback.isUpdated = false;
                    return;
                }

                int alarmSeq = 0;
                int targetSeq = 0;
                int.TryParse(AndroidSelectCallback.extraData[0], out alarmSeq);
                int.TryParse(AndroidSelectCallback.extraData[2], out targetSeq);

                int index = AndroidSelectCallback.SelectedIndex;
                switch (index) 
                {
                    case 0 :    // 알림 상세 확인.
                        OnDetail(AndroidSelectCallback.extraData[1], alarmSeq);
                        break;

                    case 1 :    // 읽음으로 처리.
                        OnRead(alarmSeq).Forget();
                        break;

                    case 2 :    // 알림 삭제.
                        OnDelete(alarmSeq).Forget();
                        break;
                }

                AndroidSelectCallback.isUpdated = false;
            }
        }
        #endif

        private void OnDestroy() 
        {
            if (R.singleton != null) 
            {
                R.singleton.UnregisterObserver(this, eAlarmKey);
            }

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
                string id = t.info.tp.id;

                // 읽음 처리.
                string res = await OnRead(seq);
                Debug.Log($"{TAG} | OnRead({seq}) result : {res}");

                // 상세 화면으로 이동.
                int targetSeq = 0;
                if (id.Equals(S.ALARM_TO_DO))
                    int.TryParse(t.info.content, out targetSeq);
                else
                    int.TryParse(t.info.contentJson, out targetSeq);
                OnDetail(id, targetSeq);
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnTruncate.onClick.AddListener(() => {
                if (R.singleton.AlarmCount == 0) return;

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string text = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "모든 알림 삭제 확인", currentLocale);
                PopupBuilder.singleton.OpenConfirm(text, async () => {
                    string res = await _module.TruncateAlarm();
                    if (string.IsNullOrEmpty(res)) 
                    {
                        GetList().Forget();
                    }
                    else 
                    {
                        PopupBuilder.singleton.OpenAlert(res);
                    }
                });
            });


            eAlarmKey = eStorageKey.Alarm;
            myAlarmCount = -1;
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
            string res = await _module.GetAlarmList(_scrollView);
            if (string.IsNullOrEmpty(res)) 
            {
                int cnt = _scrollView.GetDataCount();
                _txtGuide.gameObject.SetActive(cnt == 0);
                _btnTruncate.gameObject.SetActive(cnt > 0);

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string form = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "미확인 알림", currentLocale);
                string text = string.Format(form, R.singleton.UnreadAlarmCount);
                _txtCount.text = text;
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res);
            }
        }

        private void OnDetail(string id, int targetSeq) 
        {
            Debug.Log($"{TAG} | 알림 상세를 출력합니다. id : {id}, target seq : {targetSeq}");

            // action 정리
            switch (id) 
            {
				case S.ALARM_RESERVE_MEETING :
				case S.ALARM_UPDATE_MEETING :
				case S.ALARM_DELETE_MEETING :
				case S.ALARM_INVITE_MEETING :
				case S.ALARM_INVITE_MEETING_CANCEL :
				case S.ALARM_START_MEETING :
				case S.ALARM_DONE_MEETING :
                    ViewManager.singleton.Push(S.MobileScene_MeetingDetail, targetSeq); 
                    break;

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :
                    ViewManager.singleton.Push(S.MobileScene_SeminarDetail, targetSeq);
                    break;

                case S.ALARM_TO_DO :
                    // TODO. API 연결 후 아래 주석 다시 해제할 것.
                    // ViewManager.singleton.Push(S.MobileScene_ToDoDetail, targetSeq);
                    PopupBuilder.singleton.OpenAlert("개별 TO-DO 조회 API 준비 중.");
                    break;

				case S.ALARM_VOICE_CALL :
				case S.ALARM_REJECT_CALL :

				case S.ALARM_UPDATE_MEMBER :
				case S.ALARM_UPDATE_SPACE :
				case S.ALARM_UPDATE_SEAT :

				case S.ALARM_TASK : 
				default :
                    Debug.Log($"{TAG} | 해당 항목들은 상세 화면이 존재하지 않습니다. 따로 보여줄 수 없습니다.");
                    break;
            }
        }

        private async UniTask<string> OnRead(int seq) 
        {
            Debug.Log($"{TAG} | 해당 알림을 읽음으로 처리합니다. alarm seq : {seq}");

            int idx = R.singleton.GetIndex(seq);
            AlarmData t = (AlarmData)_scrollView.GetData(idx);

            if (! t.info.read) 
            {
                string res = await _module.ReadAlarm(seq);
                if (string.IsNullOrEmpty(res)) 
                {
                    R.singleton.UnreadAlarmCount --;
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string form = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "미확인 알림", currentLocale);
                    string text = string.Format(form, R.singleton.UnreadAlarmCount);
                    _txtCount.text = text;

                    t.info.read = true; 
                    _scrollView.UpdateData(t);
                }
                else
                {
                    PopupBuilder.singleton.OpenAlert(res);
                    return res;
                }
            }

            return string.Empty;
        }

        private async UniTaskVoid OnDelete(int seq) 
        {
            Debug.Log($"{TAG} | 해당 알림을 삭제합니다. alarm seq : {seq}");

            string res = await _module.DeleteAlarm(seq);
            if (string.IsNullOrEmpty(res)) 
                GetList().Forget();
            else 
                PopupBuilder.singleton.OpenAlert(res);
        }

    #endregion  // for list


    #region Event Listener

        private eStorageKey eAlarmKey;
        private int myAlarmCount;

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == eAlarmKey) 
            {
                if (myAlarmCount != R.singleton.UnreadAlarmCount)
                {
                    myAlarmCount = R.singleton.UnreadAlarmCount;
                    GetList().Forget();
                }
            }
        }

    #endregion  // Event Listener


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