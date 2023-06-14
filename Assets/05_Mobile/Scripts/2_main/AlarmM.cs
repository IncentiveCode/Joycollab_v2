/// <summary>
/// [mobile]
/// Alarm list
/// @author         : HJ Lee
/// @last update    : 2023. 06. 14
/// @version        : 0.1
/// @update
///     v0.1 (2022. 06. 14) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class AlarmM : FixedView
    {
        private const string TAG = "AlarmM";

        [Header("buttons")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnTruncate;
        [SerializeField] private Button _btnTest;

        [Header("contents")]
        [SerializeField] private TMP_Text _txtGuide;
        [SerializeField] private TMP_Text _txtCount;
        [SerializeField] private InfiniteScroll _scrollView;

        // local variables
        private List<AlarmData> dataList;
        private int seq;


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
            viewID = ID.MobileScene_Alarm;


            // set infinite scrollview
            _scrollView.AddSelectCallback((data) => {
                Debug.Log($"{TAG} | alarm select, title : {((AlarmData)data).info.title}");
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnTruncate.onClick.AddListener(() => {
                _scrollView.Clear();     
                dataList.Clear();

                _txtGuide.gameObject.SetActive(true);
            });
            _btnTest.onClick.AddListener(() => {
                AlarmData data = new AlarmData();
                dataList.Add(data);

                _scrollView.InsertData(data);
                seq++;

                _txtGuide.gameObject.SetActive(false);
            });


            // init local variables
            dataList = new List<AlarmData>();
            seq = 0;
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