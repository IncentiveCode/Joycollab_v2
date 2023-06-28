/// <summary>
/// [mobile]
/// 연락처 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 27
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 27) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class ContactM : FixedView
    {
        private const string TAG = "ContactM";

        [Header("module")]
        [SerializeField] private ContactModule _module; 

        [Header("search field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("search field")]
        [SerializeField] private Button _btnBack;

        [Header("contents")]
        [SerializeField] private InfiniteScroll _scrollView;

        // local variables
        private List<ContactData> dataList;
        private List<ContactData> filterList;
        

    #region Unity function

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

    #endregion  // Unity function


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_Contact;


            // set infinite scrollView
            _scrollView.AddSelectCallback((data) => {
                int seq = ((ContactData) data).info.seq;
                ViewManager.singleton.Overlay(S.MobileScene_MemberDetail, seq.ToString());
            });


            // set 'search' inputfield listener
            SetInputFieldListener(_inputSearch);
            _inputSearch.onValueChanged.AddListener((value) => {
                _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputSearch.onSubmit.AddListener((value) => Search(value));
            _btnClear.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
                Search(string.Empty);
            });
            _btnSearch.onClick.AddListener(() => Search(_inputSearch.text));


            // set 'button' listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());


            // init local variables
            dataList = new List<ContactData>();
            filterList = new List<ContactData>();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for list

        private async UniTaskVoid GetList() 
        {
            PsResponse<WorkspaceMemberList> res = await _module.GetMemberList();

            _scrollView.Clear();
            dataList.Clear();

            if (string.IsNullOrEmpty(res.message)) 
            {
                ContactData t;
                foreach (var item in res.data.list) 
                {
                    t = new ContactData(item);

                    dataList.Add(t);
                    _scrollView.InsertData(t);
                }
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

        private void Search(string value) 
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                Filter(string.Empty);
            else
                Filter(value);
        }

        private void Filter(string keyword) 
        {
            string key = keyword.ToLower();
            filterList.Clear();

            if (string.IsNullOrEmpty(key)) 
            {
                _scrollView.Clear();
                foreach (var item in dataList) _scrollView.InsertData(item);
            }
            else 
            {
                foreach (var item in dataList) 
                {
                    if (item.info.nickNm.ToLower().Contains(key)) 
                    {
                        filterList.Add(item);
                    }
                }

                _scrollView.Clear();
                foreach (var item in filterList) _scrollView.InsertData(item);
            }
        }

    #endregion  // for list


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // get list
            GetList().Forget();
            await UniTask.Yield();

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