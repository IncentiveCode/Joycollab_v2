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
                    txtTarget.text = string.Format("{0:00}:{1:00}", 
                        AndroidTimeCallback.SelectedHour,
                        AndroidTimeCallback.SelectedMinute
                    );
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


            // set infinite scrollView


            // set input field listener
            SetInputFieldListener(_inputTitle);
            _inputTitle.onValueChanged.AddListener((value) => {
                _btnClearTitle.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputTitle.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            SetInputFieldListener(_inputDetail);


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnSave.onClick.AddListener(() => PopupBuilder.singleton.OpenAlert("준비 중 입니다."));
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
            ViewManager.singleton.ShowNavigation(false);

            // refresh content

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