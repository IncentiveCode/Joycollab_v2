/// <summary>
/// [mobile] 
/// BOTTOM Navigation Bar
/// @author         : HJ Lee
/// @last update    : 2023. 03. 22
/// @version        : 0.3
/// @update         :
///     v0.1 : 최초 생성.
///     v0.2 (2022. 06. 08) : toggle 에 속한 이미지 중 background 이미지는 on 상태일 때 Active false 처리. 
///     v0.3 (2023. 03. 22) : FixedView 실험, UI 최적화 (TMP 제거)
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class NaviBottomM : FixedView
    {
        private const string TAG = "NaviBottomM";

        [Header("Toggles")] 
        [SerializeField] private Toggle _toggleOffice;
        [SerializeField] private Toggle _toggleMySeat;
        [SerializeField] private Toggle _toggleFileBox;
        [SerializeField] private Toggle _toggleMeeting;
        [SerializeField] private Toggle _toggleChat;


    #region Unity functions
        private void Awake() 
        {
            Init();
            Reset();
        }
    #endregion  // Unity functions


    #region FixedView functions
        protected override void Init() 
        {
            base.Init();


            // set toggle listener
            _toggleOffice.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (MobileManager.singleton.GetTopViewName().Equals(S.MobileScene_Office)) return;

                    MobileManager.singleton.PopAll();
                    MobileManager.singleton.Push(S.MobileScene_Office);
                }
            });

            _toggleMySeat.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (MobileManager.singleton.GetTopViewName().Equals(S.MobileScene_MySeat)) return;

                    MobileManager.singleton.PopAll();
                    MobileManager.singleton.Push(S.MobileScene_MySeat);
                }
            });
            
            _toggleFileBox.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (MobileManager.singleton.GetTopViewName().Equals(S.MobileScene_FileBox)) return;

                    MobileManager.singleton.PopAll();
                    MobileManager.singleton.Push(S.MobileScene_FileBox);
                }
            });

            _toggleMeeting.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (MobileManager.singleton.GetTopViewName().Equals(S.MobileScene_Meeting)) return;

                    MobileManager.singleton.PopAll();
                    MobileManager.singleton.Push(S.MobileScene_Meeting);
                }
            });

            _toggleChat.onValueChanged.AddListener((on) => {
                if (on) 
                {
                    if (MobileManager.singleton.GetTopViewName().Equals(S.MobileScene_Chat)) return;

                    MobileManager.singleton.PopAll();
                    MobileManager.singleton.Push(S.MobileScene_Chat);
                }
            });
        }
    #endregion  // FixedView functions


    #region Other functions
        public void ShowNavigation(bool on) 
        {
            canvasGroup.alpha = on ? 1 : 0;
            canvasGroup.interactable = on ? true : false;
            canvas.enabled = on ? true : false;
        }

        public void StartOnMySeat(bool on) 
        {
            if (on) 
            {
                if (_toggleMySeat.isOn) 
                    MobileManager.singleton.Push(S.MobileScene_MySeat);
                else 
                    _toggleMySeat.isOn = true;
            }
            else 
            {
                if (_toggleOffice.isOn) 
                    MobileManager.singleton.Push(S.MobileScene_Office);
                else
                    _toggleOffice.isOn = true;
            }
        }
    #endregion  // Other functions
    }
}