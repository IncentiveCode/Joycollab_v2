/// <summary>
/// 알림 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 11. 08.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 08) : 최초 생성, v1 & mobile 의 내용 수정 & 기능 확장 후 적용.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class AlarmList : WindowView, iRepositoryObserver
    {
        private const string TAG = "AlarmList";

        [Header("module")] 
        [SerializeField] private AlarmModule _module;

        [Header("head")]
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtAlarmCount;
        [SerializeField] private Button _btnTruncate;
        [SerializeField] private Button _btnClose;

        [Header("body")]
        [SerializeField] private InfiniteScroll _alarmList;

        // local variables
        private int alarmCount;


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
            viewID = ID.ALARM_W;
            viewData = new WindowViewData();
            viewDataKey = $"view_data_{viewID}";


            // set infinite scrollview
            _alarmList.AddSelectCallback(async (data) => {
                AlarmData t = (AlarmData)data;
                int seq = t.info.seq;
                string id = t.info.tp.id;

                // 읽음 처리.
                string res = await OnRead(seq);
                Debug.Log($"{TAG} | OnRead({seq}) result : {res}");

                // 상세 화면으로 이동 또는 호출
                int targetSeq = 0;
                if (id.Equals(S.ALARM_TO_DO))
                    int.TryParse(t.info.content, out targetSeq);
                else
                    int.TryParse(t.info.contentJson, out targetSeq);
                OnDetail(id, targetSeq);                
            });


            // set button listener
            _btnTruncate.onClick.AddListener(async () => {
                string res = await _module.TruncateAlarm();
                if (! string.IsNullOrEmpty(res)) 
                {
                    PopupBuilder.singleton.OpenAlert(res);
                    return;
                }

                _alarmList.Clear();
            });
            _btnClose.onClick.AddListener(() => {
                base.SaveViewData(viewData);
                Hide();
            });


            // set local variables
            alarmCount = -1;
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            // load view data
            base.LoadViewData();

            if (R.singleton != null) 
            {
                R.singleton.RegisterObserver(this, eStorageKey.Alarm);
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
                R.singleton.UnregisterObserver(this, eStorageKey.Alarm);
                R.singleton.UnregisterObserver(this, eStorageKey.WindowRefresh);
            }
        }

    #endregion  // WindowView functions
        

    #region Event handling 

        private async UniTask<int> Refresh() 
        {
            string res = await _module.GetAlarmList(_alarmList);
            if (! string.IsNullOrEmpty(res)) 
            {
                PopupBuilder.singleton.OpenAlert(res, () => Hide());
                return -1;
            }

            _btnTruncate.gameObject.SetActive(_alarmList.GetItemCount() != 0);
            return 0;
        }

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == eStorageKey.Alarm) 
            {
                Debug.Log($"{TAG} | UpdateInfo(Alarm) call.");
                if (alarmCount != R.singleton.UnreadAlarmCount)
                {
                    alarmCount = R.singleton.UnreadAlarmCount;
                    _txtAlarmCount.text = alarmCount > 99 ? "99+" : $"{alarmCount}";
                }
            }
            else if (key == eStorageKey.WindowRefresh) 
            {
                Debug.Log($"{TAG} | UpdateInfo(WindowRefresh) call.");
                Refresh().Forget();
            }
        }

        private async UniTask<string> OnRead(int seq) 
        {
            Debug.Log($"{TAG} | 해당 알림을 읽음으로 처리합니다. alarm seq : {seq}");

            int idx = R.singleton.GetIndex(seq);
            AlarmData t = (AlarmData)_alarmList.GetData(idx);

            if (! t.info.read) 
            {
                string res = await _module.ReadAlarm(seq);
                if (! string.IsNullOrEmpty(res))
                {
                    PopupBuilder.singleton.OpenAlert(res);
                    return res;
                }

                R.singleton.UnreadAlarmCount --;
                string form = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "미확인 알림", R.singleton.CurrentLocale);

                alarmCount = R.singleton.UnreadAlarmCount;
                _txtAlarmCount.text = alarmCount > 99 ? "99+" : $"{alarmCount}";

                t.info.read = true; 
                _alarmList.UpdateData(t);
            }

            return string.Empty;
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
                    if (isOffice)       Debug.Log($"{TAG} | office meeting detail 준비 중.");
                    else if (isMobile)  ViewManager.singleton.Push(S.MobileScene_MeetingDetail, targetSeq); 
                    else if (isWorld)   Debug.Log($"{TAG} | world meeting detail 준비 중.");
                    break;

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :
                    if (isOffice)       Debug.Log($"{TAG} | office seminar detail 준비 중.");
                    else if (isMobile)  ViewManager.singleton.Push(S.MobileScene_SeminarDetail, targetSeq);
                    else if (isWorld)   Debug.Log($"{TAG} | world seminar detail 준비 중.");
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

    #endregion  // Event handling 
    }
}