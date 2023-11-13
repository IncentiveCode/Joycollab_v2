/// <summary>
/// [mobile]
/// 채널 목록 화면을 담당하는 클래스.
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
    public class ChannelM : FixedView
    {
        private const string TAG = "ChannelM";

        // [Header("module")]
        // [SerializeField] private ChannelModule _module;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("button")]
        [SerializeField] private Button _btnBack;

        [Header("content")]
        [SerializeField] private GameObject _goItem;
        [SerializeField] private GameObject _goSubItem;


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
            viewID = ID.MobileScene_Channel;


            // set infinite scrollView


            // set input field listener


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());


            // init local variables
        }

        public override async UniTaskVoid Show() 
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