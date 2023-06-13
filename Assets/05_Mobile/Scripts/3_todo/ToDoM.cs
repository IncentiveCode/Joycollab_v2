/// <summary>
/// [mobile]
/// To-Do 리스트 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 13
/// @version        : 0.1
/// @update
///     v0.1 : 최초 생성.
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class ToDoM : FixedView
    {
        private const string TAG = "ToDoM";

        // filter Type
        public const int typeAll = 0;
        public const int typeTeam = 1;
        public const int typeOffice = 2;

        // View Type
        public const int typeDaily = 0;
        public const int typeWeekly = 1;
        public const int typeMonthly = 2;


        [Header("module")]    
        [SerializeField] private ToDoModule _module;

        [Header("contents")]
        [SerializeField] private Transform _transformList;
        [SerializeField] private GameObject _goItem;
        [SerializeField] private GameObject _goLoadMore;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("dropdown, toggle")]
        [SerializeReference] private TMP_Dropdown _dropdownFilter;
        [SerializeField] private Toggle _toggleDaily;
        [SerializeField] private Toggle _toggleWeekly;
        [SerializeField] private Toggle _toggleMonthly;

        [Header("buttons for date")]
        [SerializeField] private Button _btnDate;
        [SerializeField] private TMP_Text _txtDate;
        [SerializeField] private Button _btnPrev;
        [SerializeField] private Button _btnNext;

        [Header("buttons")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnCreate;

        // local variables
        private int filterOpt;
        private int viewOpt;
        private DateTime selectDate, startDate, endDate;
        

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
            viewID = ID.MobileScene_ToDo;


            // set 'search' inputfiled listener
            SetInputFieldListener(_inputSearch);
            _inputSearch.onValueChanged.AddListener((value) => {
                _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputSearch.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            _btnClear.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
                _inputSearch.Select();
            });


            // set 'filter, toggle' listener
            _dropdownFilter.onValueChanged.AddListener((value) => {
                Debug.Log($"{TAG} | filter changed. selected value : {value}"); 
                filterOpt = value;
            });
            _toggleDaily.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 0;
                    DisplayDate();
                }
            });
            _toggleWeekly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 1;
                    DisplayDate();
                }
            });
            _toggleMonthly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 2;
                    DisplayDate();
                }
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));
            _btnDate.onClick.AddListener(() => PickDate());
            _btnPrev.onClick.AddListener(() => ChangeDate(true));
            _btnNext.onClick.AddListener(() => ChangeDate(false));
            _btnCreate.onClick.AddListener(() => { });


            // init local variables
            selectDate = startDate = endDate = DateTime.Now;
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for date

        private void PickDate() 
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string title = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "날짜 선택", currentLocale);
            AndroidLib.singleton.ShowDatepicker(_txtDate, title);
        }

        private void DisplayDate() 
        {
            string tempDate = string.Empty;

            switch (viewOpt) 
            {
                case typeDaily:
                    tempDate = selectDate.ToString("yyyy-MM-dd");
                    break;

                case typeWeekly:
                    int day = (int)selectDate.DayOfWeek;
                    startDate = selectDate.AddDays(-day);
                    endDate = startDate.AddDays(6);
                    tempDate = string.Format("{0} - {1}",
                        startDate.ToString("yyyy-MM-dd"),
                        endDate.ToString("yyyy-MM-dd")
                    );
                    break;

                case typeMonthly:
                    tempDate = selectDate.ToString("yyyy-MM");
                    break;
            }

            _txtDate.text = tempDate;
        }

        private void ChangeDate(bool prev) 
        {
            string tempDate = string.Empty;

            switch (viewOpt) 
            {
                case typeDaily:
                    selectDate = selectDate.AddDays(prev ? -1 : 1);
                    startDate = startDate.AddDays(prev ? -1 : 1);
                    endDate = endDate.AddDays(prev ? -1 : 1);
                    break;

                case typeWeekly:
                    selectDate = selectDate.AddDays(prev ? -7 : 7);
                    startDate = startDate.AddDays(prev ? -7 : 7);
                    endDate = endDate.AddDays(prev ? -7 : 7);
                    break;

                case typeMonthly:
                    selectDate = selectDate.AddMonths(prev ? -1 : 1);
                    startDate = startDate.AddMonths(prev ? -1 : 1);
                    endDate = endDate.AddMonths(prev ? -1 : 1);
                    break;
            }

            DisplayDate();
        }

    #endregion  // for date


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // date
            DisplayDate();

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