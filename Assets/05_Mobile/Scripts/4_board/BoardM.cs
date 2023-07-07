/// <summary>
/// [mobile]
/// 게시판 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 06
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 06) : 최초 생성.
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
    public class BoardM : FixedView
    {
        private const string TAG = "BoardM";
        
        [Header("module")]
        [SerializeField] private BoardModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("buttons")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnCreate;

        [Header("contents")]
        [SerializeField] private InfiniteScroll _scrollView;

        // local variables
        private int targetSpaceSeq;
        private RequestForBoard req;
        private bool firstRequest;


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
            viewID = ID.MobileScene_Board;


            // set infinite scrollview
            _scrollView.AddSelectCallback((data) => {
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
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnCreate.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateTodo));


            // init local variables
            targetSpaceSeq = 0;
            req = new RequestForBoard();
        }

        public async override UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();

            int temp = -1;
            int.TryParse(opt, out temp);

            if (targetSpaceSeq != temp)
            {
                Debug.Log($"{TAG} | Show(), target space seq : {targetSpaceSeq}, temp : {temp}");

                targetSpaceSeq = temp;
                firstRequest = true;
            }

            // 예외처리
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            if (targetSpaceSeq == -1)
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "잘못된 접근", currentLocale),
                    () => BackProcess()
                );
            }
            
            // 조회 권한 확인
            bool hasAuth = R.singleton.CheckHasAuth(targetSpaceSeq, S.AUTH_READ_BOARD);
            if (! hasAuth) 
            {
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "권한오류 (게시글 조회)", currentLocale),
                    () => BackProcess()
                );
            }

            // 쓰기 권한 확인
            hasAuth = R.singleton.CheckHasAuth(targetSpaceSeq, S.AUTH_CREATE_BOARD);
            _btnCreate.gameObject.SetActive(hasAuth);

            req.refresh = true;
            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(bool refresh) 
        {
            base.Show().Forget();
            // GetList(refresh).Forget();
            // await UniTask.Yield();

            req.refresh = refresh;
            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for list

        private async UniTaskVoid GetList(bool refresh=true) 
        {

            req.refresh = refresh;

            await UniTask.Yield();
            /**
            PsResponse<ResToDoList> res = await _module.GetList();
            if (string.IsNullOrEmpty(res.message)) 
            {
                if (refresh)
                {
                    _scrollView.Clear();
                    R.singleton.ClearToDoList();
                }

                ToDoData t;
                foreach (var item in res.data.content) 
                {
                    t = new ToDoData();
                    t.info = item;
                    t.loadMore = false;

                    // dataList.Add(t);
                    _scrollView.InsertData(t);
                    R.singleton.AddToDoInfo(item.seq, t);
                }

                if (res.data.hasNext) 
                {
                    t = new ToDoData();
                    t.loadMore = true;

                    // dataList.Add(t);
                    _scrollView.InsertData(t);
                    R.singleton.AddToDoInfo(-1, t);
                }
            }
            else 
            {
                // dataList.Clear();
                _scrollView.Clear();
                R.singleton.ClearToDoList();

                PopupBuilder.singleton.OpenAlert(res.message);
            }
             */
        }

    #endregion  // for list


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // get list
            if (firstRequest) 
            {
                req.workspaceSeq = R.singleton.workspaceSeq;
                req.spaceSeq = targetSpaceSeq;
                req.keyword = string.Empty;
                req.pageNo = 1;
                req.pageSize = 20;

                firstRequest = false;
                string res = await _module.GetList(req, _scrollView);
                if (! string.IsNullOrEmpty(res)) 
                {
                    PopupBuilder.singleton.OpenAlert(res, () => BackProcess());
                }
            }
            else 
            {
                Debug.Log($"{TAG} | update all data");
                _scrollView.UpdateAllData();
                await UniTask.Yield();
            }

            _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(_inputSearch.text));
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