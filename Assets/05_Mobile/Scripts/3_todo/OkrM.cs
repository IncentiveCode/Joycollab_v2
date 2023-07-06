/// <summary>
/// [mobile]
/// OKR 리스트 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 05
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 03) : 최초 생성.
///     v0.2 (2023. 07. 05) : OKR 출력, 상세 화면 이동, 생성 화면 이동 등의 기능 추가
/// </summary>

using System;
using System.Collections.Generic;
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
        private int filterOpt, viewOpt;
        private bool shareOpt;
        private DateTime selectDate, startDate, endDate;

        private ReqOkrList req;
        private bool firstRequest;
        private List<int> listKeyResult;

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
                // selectedData = (OkrData) data;
                // int seq = selectedData.info.seq;
                // ViewManager.singleton.Push(S.MobileScene_OkrDetail, seq.ToString());

                Debug.Log($"{TAG} | Load More");
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

                    shareOpt = (value != 3);
                    filterOpt = (value == 3) ? -1 : value;

                    req.share = shareOpt;
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
                AndroidLib.singleton.ShowDatepicker(viewID, selectDate.ToString("yyyy-MM-dd"));
            });
            _btnPrev.onClick.AddListener(() => ChangeDate(true));
            _btnNext.onClick.AddListener(() => ChangeDate(false));
            _btnCreate.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateOkr));


            // init local variables
            selectDate = startDate = endDate = DateTime.Now;
            req = new ReqOkrList();
            firstRequest = true;

            listKeyResult = new List<int>();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            selectDate = startDate = endDate = DateTime.Now;
            filterOpt = viewOpt = 0;
            shareOpt = true;
            firstRequest = true;

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

                    listKeyResult.Clear();
                }

                int index = 0;
                OkrData t;

                // 공유 OKR
                if (req.share)
                {
                    // key result 정리
                    foreach (var item in res.data.content)
                    {
                        if (! string.IsNullOrEmpty(item.topOkr.title))
                            listKeyResult.Add(item.seq);
                    }

                    // objective 출력 
                    foreach (var item in res.data.content) 
                    {
                        if (! string.IsNullOrEmpty(item.topOkr.title)) continue;

                        t = new OkrData(item);
                        // Debug.Log($"{TAG} | share objective, title : {item.title}, share type : {t.shareType}");
                        R.singleton.AddOkrInfo(item.seq, t);
                        _scrollView.InsertData(t);

                        // key result 출력
                        foreach (var subItem in res.data.content) 
                        {
                            if (item.seq == subItem.topOkr.seq && listKeyResult.Contains(subItem.seq))
                            {
                                t = new OkrData(subItem, true);
                                // Debug.Log($"{TAG} | share key result, title : {subItem.title}, share type : {t.shareType}");
                                R.singleton.AddOkrInfo(subItem.seq, t);
                                _scrollView.InsertData(t);

                                listKeyResult.Remove(subItem.seq);
                            } 
                        }                        
                    }
                }
                // 개인 OKR
                else 
                {
                    // key result 정리
                    foreach (var item in res.data.content) 
                    {
                        if (item.shereType == 0) 
                            listKeyResult.Add(item.seq);
                    }

                    // objective 출력
                    foreach (var item in res.data.content) 
                    {
                        if (item.shereType == 0) continue;

                        t = new OkrData(item);
                        Debug.Log($"{TAG} | personal objective, title : {item.title}, share type : {t.shareType}");
                        R.singleton.AddOkrInfo(item.seq, t);
                        _scrollView.InsertData(t);

                        // key result 출력
                        foreach (var subItem in item.subOkr) 
                        {
                            t = new OkrData(subItem, item.shereType, item.title);
                            Debug.Log($"{TAG} | personal key result, title : {subItem.title}, share type : {t.shareType}");
                            index = R.singleton.AddOkrInfo(subItem.seq, t);
                            _scrollView.InsertData(t);

                            listKeyResult.Remove(subItem.seq);
                        }
                    }
                }

                // 남은 key result 출력
                if (listKeyResult.Count > 0) 
                {
                    foreach (var subItem in res.data.content) 
                    {
                        if (listKeyResult.Contains(subItem.seq))
                        {
                            t = new OkrData(subItem, true);
                            Debug.Log($"{TAG} | other key result, title : {subItem.title}, share type : {t.shareType}");
                            R.singleton.AddOkrInfo(subItem.seq, t);
                            _scrollView.InsertData(t);

                            listKeyResult.Remove(subItem.seq);
                        } 
                    }
                }

                if (res.data.hasNext) 
                {
                    t = new OkrData();
                    R.singleton.AddOkrInfo(-1, t);
                    _scrollView.InsertData(t);
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
                req.share = true;
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
                Debug.Log($"{TAG} | update all data");
                _scrollView.UpdateAllData();
                // _scrollView.MoveTo(selectedData, (InfiniteScroll.MoveToType) 0);
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