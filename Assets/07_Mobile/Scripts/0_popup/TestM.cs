/// <summary>
/// [mobile]
/// 블로그 작업을 위해 추가한 화면.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 30
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 30) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class TestM : FixedView
    {
        private const string TAG = "TestM";

        [Header("InputField Basic")]
        [SerializeField] private TMP_InputField _inputBasic;
        [SerializeField] private Button _btnClearBasic;

        [Header("InputField addon")]
        [SerializeField] private TMP_InputField _inputAddon;
        [SerializeField] private Button _btnClearAddon;

        [Header("button")]
        [SerializeField] private Button _btnBack;
        

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
            viewID = ID.MobileScene_Test;


            // set 'InputField basic' 
            _inputBasic.onValueChanged.AddListener((value) => {
                _btnClearBasic.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _btnClearBasic.onClick.AddListener(() => {
                _inputBasic.text = string.Empty;
                _inputBasic.Select();
            });

            
            // set 'InputField add listener' 
            SetInputFieldListener(_inputAddon);
            _inputAddon.onValueChanged.AddListener((value) => {
                _btnClearAddon.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _btnClearAddon.onClick.AddListener(() => {
                _inputAddon.text = string.Empty;
                _inputAddon.Select();
            });


            // set 'button' listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
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

            _btnClearBasic.gameObject.SetActive(! string.IsNullOrEmpty(_inputBasic.text));
            _btnClearAddon.gameObject.SetActive(! string.IsNullOrEmpty(_inputAddon.text));

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