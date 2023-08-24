/// <summary>
/// [mobile]
/// OKR 리스트 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.3
/// @update
///     v0.1 (2023. 07. 03) : 최초 생성.
///     v0.2 (2023. 07. 05) : OKR 출력, 상세 화면 이동, 생성 화면 이동 등의 기능 추가
///     v0.3 (2023. 07. 26) : 검색 결과 창 추가, 검색 기능 추가.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class OkrM : FixedView
    {
        private const string TAG = "OkrM";
        
        [Header("module")]
        [SerializeField] private OkrModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("Dropdown, toggle")]
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

        [Header("contents")]
        [SerializeField] private InfiniteScroll _scrollView;

        [Header("search result")]
        [SerializeField] private InfiniteScroll _searchView;
        [SerializeField] private Button _btnCloseSearch;
        [SerializeField] private GameObject _goSearchGuide;


        // local variables
        private int filterOpt, viewOpt;
        private bool shareOpt;
        private DateTime selectDate, startDate, endDate;

        private ReqOkrList req;
        private ReqOkrList reqSearch;
        private bool firstRequest;
        private Locale currentLocale;

        // for date picker, time picker
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

                // 후속 조치
                selectDate = AndroidDateCallback.SelectedDate;
                DisplayDate();

                req.startDate = result;
                reqSearch.startDate = result;
                GetList(true).Forget();

                AndroidDateCallback.isDateUpdated = false;
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
            viewID = ID.MobileScene_Okr;


            // set infinite scrollview
            _scrollView.AddSelectCallback((data) => {
                _scrollView.RemoveData(data);
                req.pageNo ++;
                GetList(false).Forget();
            });

            _searchView.AddSelectCallback((data) => {
                _searchView.RemoveData(data);
                reqSearch.pageNo ++;
                GetSearch(false).Forget();
            });


            // set 'search' inputfiled listener
            SetInputFieldListener(_inputSearch);
            _inputSearch.onValueChanged.AddListener((value) => {
                _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputSearch.onSubmit.AddListener((value) => {
                if (string.IsNullOrEmpty(value)) 
                {
                    PopupBuilder.singleton.OpenAlert(LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "검색어 없음", currentLocale));
                    return;
                }

                reqSearch.keyword = value;
                GetSearch(true).Forget();
            });
            _btnClear.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
                _inputSearch.Select();
            });
            _btnSearch.onClick.AddListener(() => {
                if (string.IsNullOrEmpty(_inputSearch.text)) 
                {
                    PopupBuilder.singleton.OpenAlert(LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "검색어 없음", currentLocale));
                    return;
                }

                reqSearch.keyword = _inputSearch.text;
                GetSearch(true).Forget();
            });


            // set 'filter, toggle' listener
            if (_dropdownFilter != null) 
            {
                _dropdownFilter.onValueChanged.AddListener((value) => {
                    shareOpt = (value != 3);
                    filterOpt = (value == 3) ? -1 : value;

                    req.share = shareOpt;
                    req.filterOpt = value;
                    reqSearch.share = shareOpt;
                    reqSearch.filterOpt = value;
                    GetList(true).Forget();
                });
            }
            _toggleDaily.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 0;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    reqSearch.viewOpt = viewOpt;
                    GetList(true).Forget();
                }
            });
            _toggleWeekly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 1;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    reqSearch.viewOpt = viewOpt;
                    GetList(true).Forget();
                }
            });
            _toggleMonthly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 2;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    reqSearch.viewOpt = viewOpt;
                    GetList(true).Forget();
                }
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnDate.onClick.AddListener(() => {
                txtTarget = _txtDate;
                AndroidLib.singleton.ShowDatepicker(viewID, selectDate.ToString("yyyy-MM-dd"));
            });
            _btnPrev.onClick.AddListener(() => ChangeDate(true));
            _btnNext.onClick.AddListener(() => ChangeDate(false));
            _btnCreate.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateOkr));
            _btnCloseSearch.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
                _searchView.gameObject.SetActive(false);
            });


            // init local variables
            selectDate = startDate = endDate = DateTime.Now;
            filterOpt = viewOpt = 0;
            shareOpt = true;
            firstRequest = true;

            req = new ReqOkrList();
            reqSearch = new ReqOkrList();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            Debug.Log($"{TAG} | Show()");
            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(bool refresh) 
        {
            base.Show().Forget();
            Debug.Log($"{TAG} | Show({refresh})");
            if (refresh) GetList().Forget();
            await UniTask.Yield();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for list

        private async UniTaskVoid GetList(bool refresh=true) 
        {
            if (refresh) req.pageNo = 1;

            string res = await _module.Get(_scrollView, req, refresh);
            if (string.IsNullOrEmpty(res)) 
            {
                // nothing...
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res);
            }
        }

        private async UniTaskVoid GetSearch(bool refresh=true) 
        {
            if (refresh) reqSearch.pageNo = 1;

            string res = await _module.Search(_searchView, reqSearch, refresh);
            if (string.IsNullOrEmpty(res)) 
            {
                _searchView.gameObject.SetActive(true);
                _goSearchGuide.SetActive(_searchView.GetDataCount() == 0);
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res);
            }
        }

    #endregion  // for list


    #region for date

        private void DisplayDate() 
        {
            string tempDate = string.Empty;

            switch (viewOpt) 
            {
                case S.TYPE_DAILY:
                    tempDate = selectDate.ToString("yyyy-MM-dd");
                    break;

                case S.TYPE_WEEKLY:
                    int day = (int)selectDate.DayOfWeek;
                    startDate = selectDate.AddDays(-day);
                    endDate = startDate.AddDays(6);
                    tempDate = string.Format("{0} - {1}",
                        startDate.ToString("yyyy-MM-dd"),
                        endDate.ToString("yyyy-MM-dd")
                    );
                    break;

                case S.TYPE_MONTHLY:
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
                case S.TYPE_DAILY:
                    selectDate = selectDate.AddDays(prev ? -1 : 1);
                    startDate = startDate.AddDays(prev ? -1 : 1);
                    endDate = endDate.AddDays(prev ? -1 : 1);
                    break;

                case S.TYPE_WEEKLY:
                    selectDate = selectDate.AddDays(prev ? -7 : 7);
                    startDate = startDate.AddDays(prev ? -7 : 7);
                    endDate = endDate.AddDays(prev ? -7 : 7);
                    break;

                case S.TYPE_MONTHLY:
                    selectDate = selectDate.AddMonths(prev ? -1 : 1);
                    startDate = startDate.AddMonths(prev ? -1 : 1);
                    endDate = endDate.AddMonths(prev ? -1 : 1);
                    break;
            }

            DisplayDate();

            string date = selectDate.ToString("yyyy-MM-dd");
            req.startDate = date;
            reqSearch.startDate = date;
            GetList(true).Forget();
        }

    #endregion  // for date


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // language
            currentLocale = LocalizationSettings.SelectedLocale;

            // date
            DisplayDate();

            // get list
            if (firstRequest)
            {
                req.share = true;
                req.startDate = selectDate.ToString("yyyy-MM-dd");
                req.viewOpt = viewOpt;
                req.filterOpt = filterOpt;
                req.keyword = string.Empty;
                req.pageNo = 1; 
                req.pageSize = 20;
                req.sortDescending = true;
                req.sortProperty = "sd";

                reqSearch.share = true;
                reqSearch.startDate = selectDate.ToString("yyyy-MM-dd");
                reqSearch.viewOpt = viewOpt;
                reqSearch.filterOpt = filterOpt;
                reqSearch.keyword = string.Empty;
                reqSearch.pageNo = 1; 
                reqSearch.pageSize = 20;
                reqSearch.sortDescending = true;
                reqSearch.sortProperty = "sd";

                // 기본값은 daily 로 출력.
                if (! _toggleDaily.isOn)
                    _toggleDaily.isOn = true;
                else 
                    GetList(true).Forget();

                _searchView.gameObject.SetActive(false);
                Tmp.singleton.ClearOkrSearchList();

                firstRequest = false;
            }
            else
            {
                _scrollView.UpdateAllData();

                if (_searchView.gameObject.activeSelf)
                    _searchView.UpdateAllData();
                else
                    Tmp.singleton.ClearOkrSearchList();
            }

            _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(_inputSearch.text));

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