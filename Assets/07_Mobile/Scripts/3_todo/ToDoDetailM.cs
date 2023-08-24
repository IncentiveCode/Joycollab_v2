/// <summary>
/// [mobile]
/// To-Do 상세 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 25
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 29) : 최초 생성.
///     v0.2 (2023. 07. 25) : Remind option 출력 추가. Show() 매개변수 변경  [string 형 opt -> int 형 seq], period 에 시간값도 출력.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class ToDoDetailM : FixedView
    {
        private const string TAG = "ToDoDetailM";
        
        [Header("module")]
        [SerializeField] private ToDoModule _module;

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnEdit;
        [SerializeField] private Button _btnDelete;

        [Header("check")]
        [SerializeField] private Button _btnDone;
        [SerializeField] private Image _imgCheck;

        [Header("text")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtCreator;
        [SerializeField] private TMP_Text _txtCreateDate;
        [SerializeField] private TMP_Text _txtPeriod;
        [SerializeField] private TMP_Text _txtDoneDate;
        [SerializeField] private TMP_Text _txtShareOpt;
        [SerializeField] private TMP_Text _txtRemindOpt;
        [SerializeField] private TMP_Text _txtDetail;

        // local variables
        private ToDoData data;
        private int seq;
        private bool isDone;


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
            viewID = ID.MobileScene_ToDoDetail;


            // set 'button' listener
            _btnBack.onClick.AddListener(() => BackProcess());
            _btnEdit.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateTodo, seq));
            _btnDelete.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "삭제 안내", currentLocale);

                PopupBuilder.singleton.OpenConfirm(message, () => DeleteInfo().Forget());
            });
            _btnDone.onClick.AddListener(async () => {
                PsResponse<string> res = await _module.CheckItem(this.seq);
                if (string.IsNullOrEmpty(res.message)) 
                {
                    isDone = !isDone;
                    DoneProcess(isDone);
                }
                else 
                {
                    PopupBuilder.singleton.OpenAlert(res.message);
                }
            });


            // init local variables
            data = null;
            seq = 0;
            isDone = false;
        }

        public async override UniTaskVoid Show(int seq) 
        {
            base.Show().Forget();

            // TODO. 상세 조회 API 추가 예정.
            data = Tmp.singleton.GetToDoInfo(seq);
            if (data == null) data = Tmp.singleton.GetSearchToDo(seq);
            if (data == null) 
            {
                PopupBuilder.singleton.OpenAlert("오류가 발생했습니다.", () => BackProcess());
            }

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // display info 
            this.seq = data.info.seq;
            this.isDone = data.info.completeYn.Equals("Y");

            _txtTitle.text = data.info.title;
            _txtCreator.text = string.Format("{0} ({1})", data.info.createMember.nickNm, data.info.space.nm);
            _txtCreateDate.text = data.info.createdDate;
            _txtPeriod.text = string.Format("{0} {1} - {2} {3}", data.info.sd, data.info.st, data.info.ed, data.info.et);
            _txtDoneDate.text = data.info.completeTime;
            _txtDetail.text = data.info.content;

            Locale currentLocale = LocalizationSettings.SelectedLocale;

            // share option
            switch (data.info.shereType) 
            {
                case S.SHARE_DEPARTMENT :
                    _txtShareOpt.text = data.info.space.nm;
                    break;

                case S.SHARE_COMPANY :
                    _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "전사", currentLocale);
                    break;

                case S.SHARE_NONE :
                default :
                    _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "없음", currentLocale);
                    break;
            }

            // remind option
            if (string.IsNullOrEmpty(data.info.alarm))
            {
                _txtRemindOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "미리 알림 없음", currentLocale);
            }
            else 
            {
                string sdt = string.Format("{0} {1}", data.info.sd, data.info.st);
                System.DateTime convertSdt = System.Convert.ToDateTime(sdt);
                System.DateTime convertAlarm = System.Convert.ToDateTime(data.info.alarm);
                System.TimeSpan diff = convertSdt - convertAlarm;

                string key = string.Format("미리 알림 {0}분 전", diff.Minutes);
                _txtRemindOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", key, currentLocale);
            }

            // 내 정보인 경우에만 버튼 출력
            bool isMyInfo = (R.singleton.memberSeq == data.info.createMember.seq);
            _btnDone.interactable = isMyInfo;
            _btnEdit.gameObject.SetActive(isMyInfo);
            _btnDelete.gameObject.SetActive(isMyInfo);

            // 완료 마크 처리
            Color tempColor = _imgCheck.color;
            tempColor.a = isMyInfo ? 1f : 0.5f;
            _imgCheck.color = tempColor;
            
            // 완료 처리
            DoneProcess(isDone);

            await UniTask.Yield();
            return 0;
        }

        private void DoneProcess(bool done) 
        {
            _imgCheck.gameObject.SetActive(done);
            _txtDoneDate.gameObject.SetActive(done);
            _txtTitle.fontStyle = done ? FontStyles.Strikethrough : FontStyles.Normal;
            _txtPeriod.fontStyle = done ? FontStyles.Strikethrough : FontStyles.Normal;

            data.info.completeYn = done ? "Y" : "N";
            data.info.completeTime = done ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            _txtDoneDate.text = data.info.completeTime;
            
            Tmp.singleton.AddToDoInfo(this.seq, data);
        }

        private async UniTaskVoid DeleteInfo()
        {
            PsResponse<string> res = await _module.DeleteToDo(this.seq);
            if (string.IsNullOrEmpty(res.message)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "삭제 완료", currentLocale);
                PopupBuilder.singleton.OpenAlert(message, () => {
                    ViewManager.singleton.Pop(true);
                });
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
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