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
                // selectedData = (ToDoData) data;
                // int seq = selectedData.info.seq;
                // ViewManager.singleton.Push(S.MobileScene_ToDoDetail, seq.ToString());

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


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));
            _btnCreate.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_CreateTodo));


            // init local variables
            targetSpaceSeq = 0;
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
            }
            
            // TODO. 해당 게시판 쓰기 권한이 있는지 확인
            // _btnCreate.gameObject.SetActive(isMyInfo);

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
            // TODO. list 출력

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