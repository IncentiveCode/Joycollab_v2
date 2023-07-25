/// <summary>
/// [mobile]
/// To-Do 리스트 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 25
/// @version        : 0.5
/// @update
///     v0.1 (2023. 06. 13) : 최초 생성.
///     v0.2 (2023. 06. 29) : ToDoM 과 ToDoShareM 으로 분리. 분리하면서 filter 는 삭제.
///     v0.3 (2023. 06. 30) : date picker 처리 수정. share option 추가.
///     v0.4 (2023. 07. 21) : 검색결과 창 추가.
///     v0.5 (2023. 07. 25) : 검색 기능 추가.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class ToDoM : FixedView
    {
        private const string TAG = "ToDoM";
        
        [Header("module")]
        [SerializeField] private ToDoModule _module;

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
        [SerializeField] private bool _isShare;
        [SerializeField] private InfiniteScroll _scrollView;

        [Header("search result")]
        [SerializeField] private InfiniteScroll _searchView;
        [SerializeField] private Button _btnCloseSearch;
        [SerializeField] private GameObject _goSearchGuide;

        // local variables
        private int filterOpt;
        private int viewOpt;
        private DateTime selectDate, startDate, endDate;

        private int targetMemberSeq;
        private bool isMyInfo;
        private ReqToDoList req;
        private ReqToDoList reqSearch;
        private bool firstRequest;

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
            viewID = _isShare ? ID.MobileScene_ShareToDo : ID.MobileScene_ToDo;


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
                reqSearch = req;
                reqSearch.pageNo = 1;
                reqSearch.keyword = value;
                GetSearch(true).Forget();
            });
            _btnClear.onClick.AddListener(() => {
                _searchView.gameObject.SetActive(false);
                Tmp.singleton.ClearToDoSearchList();

                _inputSearch.text = string.Empty;
                _inputSearch.Select();
            });
            _btnSearch.onClick.AddListener(() => {
                reqSearch = req;
                reqSearch.pageNo = 1;
                reqSearch.keyword = _inputSearch.text;
                GetSearch(true).Forget();
            });


            // set 'filter, toggle' listener
            if (_dropdownFilter != null) 
            {
                _dropdownFilter.onValueChanged.AddListener((value) => {
                    Debug.Log($"{TAG} | filter changed. selected value : {value}"); 
                    filterOpt = value;
                    req.filterOpt = value;
                    GetList(true).Forget();
                });
            }
            _toggleDaily.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 0;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    GetList(true).Forget();
                }
            });
            _toggleWeekly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 1;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    GetList(true).Forget();
                }
            });
            _toggleMonthly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 2;
                    DisplayDate();

                    req.viewOpt = viewOpt;
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
            _btnCreate.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateTodo));
            _btnCloseSearch.onClick.AddListener(() => _searchView.gameObject.SetActive(false));


            // init local variables
            selectDate = startDate = endDate = DateTime.Now;
            targetMemberSeq = 0;
            isMyInfo = false;
            req = new ReqToDoList();
            reqSearch = new ReqToDoList();
            firstRequest = true;
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(int seq) 
        {
            base.Show().Forget();

            if (targetMemberSeq != seq)
            {
                Debug.Log($"{TAG} | Show(), targetMemberSeq : {targetMemberSeq}, temp : {seq}");
                targetMemberSeq = seq;
                selectDate = startDate = endDate = DateTime.Now;
                viewOpt = 0;
                firstRequest = true;
            }
            isMyInfo = (R.singleton.memberSeq == targetMemberSeq);
            _btnCreate.gameObject.SetActive(isMyInfo);

            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(bool refresh) 
        {
            base.Show().Forget();
            GetList(refresh).Forget();
            await UniTask.Yield();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for list

        private async UniTaskVoid GetList(bool refresh=true) 
        {
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

            req.startDate = selectDate.ToString("yyyy-MM-dd");
            GetList(true).Forget();
        }

    #endregion  // for date


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // date
            DisplayDate();

            // get list
            if (firstRequest)
            {
                req.share = _isShare;
                req.startDate = selectDate.ToString("yyyy-MM-dd");
                req.targetMemberSeq = targetMemberSeq;
                req.viewOpt = viewOpt;
                req.filterOpt = filterOpt;
                req.keyword = string.Empty;
                req.pageNo = 1; 
                req.pageSize = 20;
                req.sortDescending = true;
                req.sortProperty = "sd";

                // 기본값은 daily 로 출력.
                if (! _toggleDaily.isOn)
                    _toggleDaily.isOn = true;
                else 
                    GetList(true).Forget();

                _searchView.gameObject.SetActive(false);
                Tmp.singleton.ClearToDoSearchList();

                firstRequest = false;
            }
            else
            {
                _scrollView.UpdateAllData();

                if (_searchView.gameObject.activeSelf) 
                    _searchView.UpdateAllData();
                else
                    Tmp.singleton.ClearToDoSearchList();
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