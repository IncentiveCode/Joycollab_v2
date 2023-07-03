/// <summary>
/// [mobile]
/// 할 일 생성 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 30
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 30) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class CreateToDoM : FixedView
    {
        private const string TAG = "CreateToDoM";

        [Header("module")]
        [SerializeField] private ToDoModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputTitle;
        [SerializeField] private Button _btnClearTitle;
        [SerializeField] private TMP_InputField _inputDetail;

        [Header("dropdown")]
        [SerializeField] private TMP_Dropdown _dropdownShare;
        [SerializeField] private TMP_Dropdown _dropdownRemind;

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnSave;
        [SerializeField] private Button _btnStartDate;
        [SerializeField] private Button _btnDueDate;
        [SerializeField] private Button _btnStartTime;
        [SerializeField] private Button _btnDueTime;

        [Header("text")]
        [SerializeField] private TMP_Text _txtStartDate;
        [SerializeField] private TMP_Text _txtDueDate;
        [SerializeField] private TMP_Text _txtStartTime;
        [SerializeField] private TMP_Text _txtDueTime;

        // local variables
        private int seq;
        private ToDoData data;

        // temp
        private TMP_Text txtTarget;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        #if UNITY_ANDROID
        private void Update() 
        {
            // date picker 처리
            if (AndroidDateCallback.isDateUpdated && AndroidDateCallback.viewID == this.viewID) 
            {
                string result = AndroidDateCallback.SelectedDate.ToString("yyyy-MM-dd");
                if (txtTarget != null)
                {
                    txtTarget.text = result;
                }
                AndroidDateCallback.isDateUpdated = false;
            }

            // time picker 처리
            if (AndroidTimeCallback.isTimeUpdated && AndroidTimeCallback.viewID == this.viewID) 
            {
                if (txtTarget != null) 
                {
                    int hour = AndroidTimeCallback.SelectedHour;
                    int minute = AndroidTimeCallback.SelectedMinute;

                    if (minute > 30) hour ++;
                    if (hour >= 24) hour = 0;
                    minute = (minute <= 30) ? 30 : 0;

                    txtTarget.text = string.Format("{0:00}:{1:00}", hour, minute);

                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "할 일 시간 설정 안내", currentLocale);
                    PopupBuilder.singleton.OpenAlert(message);
                }
                AndroidTimeCallback.isTimeUpdated = false;
            }
        }
        #endif

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
            viewID = ID.MobileScene_CreateTodo;


            // set input field listener
            SetInputFieldListener(_inputTitle);
            _inputTitle.onValueChanged.AddListener((value) => {
                _btnClearTitle.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputTitle.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            SetInputFieldListener(_inputDetail);


            // set button listener
            _btnBack.onClick.AddListener(() => BackProcess());
            _btnSave.onClick.AddListener(() => SaveToDo().Forget());
            _btnStartDate.onClick.AddListener(() => {
                txtTarget = _txtStartDate;
                AndroidLib.singleton.ShowDatepicker(viewID);
            });
            _btnDueDate.onClick.AddListener(() => {
                txtTarget = _txtDueDate;
                AndroidLib.singleton.ShowDatepicker(viewID);
            });
            _btnStartTime.onClick.AddListener(() => {
                txtTarget = _txtStartTime;
                AndroidLib.singleton.ShowTimePicker(viewID);
            });
            _btnDueTime.onClick.AddListener(() => {
                txtTarget = _txtDueTime;
                AndroidLib.singleton.ShowTimePicker(viewID);
            });


            // init local variables
            seq = -1;
            data = null;
        }

        /// <summary>
        /// 새 할 일 등록.
        /// </summary>
        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            data = null;
            await Refresh();
            base.Appearing();
        }

        /// <summary>
        /// 기존 할 일 수정.
        /// </summary>
        public async override UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();

            int temp = -1;
            int.TryParse(opt, out temp);
            if (temp == -1)
            {
                // TODO. 예외처리
            }

            seq = temp;
            data = R.singleton.GetToDoInfo(seq);
            if (data == null)
            {
                // TODO. 예외처리
            }

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region to-do handling

        private async UniTaskVoid SaveToDo() 
        {
            ReqToDoInfo req = new ReqToDoInfo();
            req.content = _inputDetail.text;
            req.ed = _txtDueDate.text;
            req.repetition = -1;
            req.sd = _txtStartDate.text;
            req.shereType = _dropdownShare.value;
            req.st = _txtStartTime.text;
            req.et = _txtDueTime.text;
            req.title = _inputTitle.text;

            int remindOpt = -1;
            switch (_dropdownRemind.value)
            {
                case 0 :    remindOpt = -1;     break;
                case 1 :    remindOpt = 0;      break;
                case 2 :    remindOpt = 5;      break;
                case 3 :    remindOpt = 10;     break;
                case 4 :    remindOpt = 30;     break;
                case 5 :    remindOpt = 60;     break;
                case 6 :    remindOpt = 120;    break;
                default :   remindOpt = -1;     break;
            }

            PsResponse<string> res;
            if (this.seq == -1) 
                res = await _module.SaveToDo(JsonUtility.ToJson(req), _dropdownShare.value, remindOpt);
            else
                res = await _module.UpdateToDo(this.seq, JsonUtility.ToJson(req), _dropdownShare.value, remindOpt);

            if (string.IsNullOrEmpty(res.message)) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = (this.seq == -1) ? 
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "등록 완료", currentLocale) :
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "수정 완료", currentLocale);

                PopupBuilder.singleton.OpenAlert(message, () => {
                    if (this.seq == -1)
                    {
                        ViewManager.singleton.Pop(true);
                    }
                    else
                    {
                        data.info.content = req.content;
                        data.info.ed = req.ed;
                        data.info.sd = req.sd;
                        data.info.shereType = req.shereType;
                        data.info.st = req.st; 
                        data.info.et = req.et;
                        data.info.title = req.title;
                        R.singleton.AddToDoInfo(this.seq, data);

                        ViewManager.singleton.Pop(this.seq.ToString());
                    }
                });
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

    #endregion to-do handling


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // ready for new content
            if (data == null) 
            {
                // set input field value
                _inputTitle.text = _inputDetail.text = string.Empty;

                // calculate hour
                System.DateTime now = System.DateTime.Now;
                int tempMinute;
                tempMinute = (now.Minute <= 30) ? (30 - now.Minute) : (60 - now.Minute);
                now = now.AddMinutes(tempMinute);

                // set text value
                _txtStartDate.text = now.ToString("yyyy-MM-dd");
                _txtStartTime.text = now.ToString("HH:mm");

                now = now.AddMinutes(30);
                _txtDueDate.text = now.ToString("yyyy-MM-dd");
                _txtDueTime.text = now.ToString("HH:mm");

                // set dropdown value
                _dropdownShare.value = _dropdownRemind.value = 0;
            }
            // refresh for content detail
            else 
            {
                // set input field value
                _inputTitle.text = data.info.title;
                _inputDetail.text = data.info.content;

                // set text value
                _txtStartDate.text = data.info.sd;
                _txtStartTime.text = data.info.st;
                _txtDueDate.text = data.info.ed;
                _txtDueTime.text = data.info.et;

                // set dropdown value;
                _dropdownShare.value = data.info.shereType;

                if (string.IsNullOrEmpty(data.info.alarm)) 
                {
                    _dropdownRemind.value = 0;
                }
                else 
                {
                    string sdt = string.Format("{0} {1}", data.info.sd, data.info.st);
                    System.DateTime convertSdt = System.Convert.ToDateTime(sdt);
                    System.DateTime convertAlarm = System.Convert.ToDateTime(data.info.alarm);
                    System.TimeSpan diff = convertSdt - convertAlarm;

                    switch (diff.Minutes) 
                    {
                        case 0 :    _dropdownRemind.value = 1;      break;
                        case 5 :    _dropdownRemind.value = 2;      break;
                        case 10 :   _dropdownRemind.value = 3;      break;
                        case 30 :   _dropdownRemind.value = 4;      break;
                        case 60 :   _dropdownRemind.value = 5;      break;
                        case 120 :  _dropdownRemind.value = 6;      break;
                        default :   _dropdownRemind.value = 0;      break;
                    }
                }
            }

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
            bool isChanged = false;
            if (data == null) 
            {
                isChanged = ! string.IsNullOrEmpty(_inputTitle.text) || ! string.IsNullOrEmpty(_inputDetail.text);
            }
            else 
            {
                isChanged = 
                    (data.info.title != _inputTitle.text) ||
                    (data.info.sd != _txtStartDate.text) ||
                    (data.info.ed != _txtDueDate.text) ||
                    (data.info.st != _txtStartTime.text) ||
                    (data.info.et != _txtDueTime.text) ||
                    (data.info.shereType != _dropdownShare.value) ||
                    (data.info.content != _inputDetail.text);
            }

            if (isChanged) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "작성 취소 확인", currentLocale);
                PopupBuilder.singleton.OpenConfirm(message, () => {
                    if (data == null)   ViewManager.singleton.Pop();
                    else                ViewManager.singleton.Pop(this.seq.ToString());
                });
            }
            else 
            {
                if (data == null)   ViewManager.singleton.Pop();
                else                ViewManager.singleton.Pop(this.seq.ToString());
            }
        }

    #endregion  // event handling
    }
}