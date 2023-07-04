/// <summary>
/// [mobile]
/// OKR 리스트 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 03
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 03) : 최초 생성.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
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

        // local variables
        private int filterOpt;
        private int viewOpt;
        private DateTime selectDate, startDate, endDate;

        private OkrData selectedData;
        private int targetMemberSeq;
        private bool isMyInfo;
        private ReqOkrList req;
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
                GetList(req, true).Forget();

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
                selectedData = (OkrData) data;
                int seq = selectedData.info.seq;
                ViewManager.singleton.Push(S.MobileScene_OkrDetail, seq.ToString());
            });


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
            if (_dropdownFilter != null) 
            {
                _dropdownFilter.onValueChanged.AddListener((value) => {
                    Debug.Log($"{TAG} | filter changed. selected value : {value}"); 
                    filterOpt = value;
                    req.filterOpt = value;
                    GetList(req, true).Forget();
                });
            }
            _toggleDaily.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 0;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    GetList(req, true).Forget();
                }
            });
            _toggleWeekly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 1;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    GetList(req, true).Forget();
                }
            });
            _toggleMonthly.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    viewOpt = 2;
                    DisplayDate();

                    req.viewOpt = viewOpt;
                    GetList(req, true).Forget();
                }
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));
            _btnDate.onClick.AddListener(() => {
                txtTarget = _txtDate;
                AndroidLib.singleton.ShowDatepicker(viewID);
            });
            _btnPrev.onClick.AddListener(() => ChangeDate(true));
            _btnNext.onClick.AddListener(() => ChangeDate(false));
            _btnCreate.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateTodo));


            // init local variables
            selectDate = startDate = endDate = DateTime.Now;
            targetMemberSeq = 0;
            isMyInfo = false;
            req = new ReqOkrList();
            firstRequest = true;
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();

            int temp = -1;
            int.TryParse(opt, out temp);

            if (targetMemberSeq != temp)
            {
                Debug.Log($"{TAG} | Show(), targetMemberSeq : {targetMemberSeq}, temp : {temp}");
                targetMemberSeq = temp;
                selectDate = startDate = endDate = DateTime.Now;
                viewOpt = 0;
                firstRequest = true;
            }
            _btnCreate.gameObject.SetActive(true);

            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(bool refresh) 
        {
            base.Show().Forget();
            GetList(req, refresh).Forget();
            await UniTask.Yield();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for list

        private async UniTaskVoid GetList(ReqOkrList req, bool refresh=true) 
        {
            string url = req.url;
            PsResponse<ResOkrList> res = await _module.GetList(url);
            if (string.IsNullOrEmpty(res.message)) 
            {
                if (refresh)
                {
                    _scrollView.Clear();
                    R.singleton.ClearOkrList();
                }

                OkrData t;
                foreach (var item in res.data.content) 
                {
                    t = new OkrData();
                    t.info = item;
                    t.loadMore = false;

                    _scrollView.InsertData(t);
                    R.singleton.AddOkrInfo(item.seq, t);
                }

                if (res.data.hasNext) 
                {
                    t = new OkrData();
                    t.loadMore = true;

                    _scrollView.InsertData(t);
                    R.singleton.AddOkrInfo(-1, t);
                }
            }
            else 
            {
                _scrollView.Clear();
                R.singleton.ClearOkrList();

                PopupBuilder.singleton.OpenAlert(res.message);
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
            GetList(req, true).Forget();
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
                req.startDate = selectDate.ToString("yyyy-MM-dd");
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
                    GetList(req, true).Forget();

                firstRequest = false;
            }
            else
            {
                _scrollView.UpdateAllData();
                _scrollView.MoveTo(selectedData, (InfiniteScroll.MoveToType) 0);
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