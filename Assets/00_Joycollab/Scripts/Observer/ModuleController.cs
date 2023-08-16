/// <summary>
/// Module 테스트를 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 08. 16
/// @version        : 0.4
/// @update
///     v0.1 (2023. 03. 14) : 최초 생성, observer pattern 을 이용한 Tester 구현.
///     v0.2 (2023. 07. 13) : font size change, scene load 등의 테스트 버튼 연결.
///     v0.3 (2023. 08. 01) : 통합 게시판 추가. '미팅' 만 있는 메뉴와 '미팅+세미나' 가 있는 메뉴 분리.
///                           구독 플랜에 따라 모듈 설정되는 기능 추가.
///     v0.4 (2023. 08. 16) : 일본어 테스트 추가.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class ModuleController : MonoBehaviour, IModuleController
    {
        private const string TAG = "ModuleController";
        private const string key = "__floating_menu_state";

        private static List<IModuleObserver> listObservers = new List<IModuleObserver>();    
        private static FloatingMenu menuState = new FloatingMenu();


        [Header("top menu")]
        [SerializeField] private Button _btnExit;
        [SerializeField] private Button _btnFontSmall;
        [SerializeField] private Button _btnFontNormal;
        [SerializeField] private Button _btnFontLarge;
        [SerializeField] private Button _btnKorean;
        [SerializeField] private Button _btnEnglish;
        [SerializeField] private Button _btnJapanese;

        [Header("module list")]
        [SerializeField] private Toggle _toggleFileBox;
        [SerializeField] private Toggle _toggleTodo;
        [SerializeField] private Toggle _toggleContact;
        [SerializeField] private Toggle _toggleChat;
        [SerializeField] private Toggle _toggleCall;
        [SerializeField] private Toggle _toggleMeeting;
        [SerializeField] private Toggle _toggleMeetingAndSeminar;
        [SerializeField] private Toggle _toggleCalendar;
        [SerializeField] private Toggle _toggleKanban;
        [SerializeField] private Toggle _toggleBuiltInBoard;

        [Header("subscription list")]
        [SerializeField] private Toggle _toggleFree;
        [SerializeField] private Toggle _toggleBasic;
        [SerializeField] private Toggle _toggleStandard;
        [SerializeField] private Toggle _togglePremium;
        [SerializeField] private Toggle _toggleTrial;
        [SerializeField] private Toggle _toggleCustom;

        [SerializeField] private Button _btnSave;

    #region Unity functions

        private void Awake()
        {
            // set 'top menu' listener
            _btnExit.onClick.AddListener(() => SystemManager.singleton.Exit());
            _btnFontSmall.onClick.AddListener(() => SystemManager.singleton.SetFontOpt(1));
            _btnFontNormal.onClick.AddListener(() => SystemManager.singleton.SetFontOpt(2));
            _btnFontLarge.onClick.AddListener(() => SystemManager.singleton.SetFontOpt(3));
            _btnKorean.onClick.AddListener(() => R.singleton.ChangeLocale(ID.LANGUAGE_KOREAN));
            _btnEnglish.onClick.AddListener(() => R.singleton.ChangeLocale(ID.LANGUAGE_ENGLISH));
            _btnJapanese.onClick.AddListener(() => R.singleton.ChangeLocale(ID.LANGUAGE_JAPANESE));


            // set 'plan toggle' listener
            _toggleFree.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _toggleFileBox.isOn = _toggleTodo.isOn = _toggleContact.isOn = _toggleCall.isOn = _toggleChat.isOn = true;
                    _toggleMeeting.isOn = _toggleMeetingAndSeminar.isOn = _toggleCalendar.isOn = _toggleKanban.isOn = _toggleBuiltInBoard.isOn = false;
                }
            });
            _toggleBasic.onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    _toggleFileBox.isOn = _toggleTodo.isOn = _toggleContact.isOn = _toggleCall.isOn = _toggleChat.isOn = true;
					_toggleMeeting.isOn = true;
					_toggleMeetingAndSeminar.isOn = _toggleCalendar.isOn = _toggleBuiltInBoard.isOn = false;
                    _toggleKanban.isOn = false;
                }
            });
            _toggleStandard.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _toggleFileBox.isOn = _toggleTodo.isOn = _toggleContact.isOn = _toggleCall.isOn = _toggleChat.isOn = true;
					_toggleMeeting.isOn = false;
                    _toggleMeetingAndSeminar.isOn = _toggleCalendar.isOn = _toggleBuiltInBoard.isOn = true;
					_toggleKanban.isOn = false;
                }
            });
            _togglePremium.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _toggleFileBox.isOn = _toggleTodo.isOn = _toggleContact.isOn = _toggleCall.isOn = _toggleChat.isOn = true;
					_toggleMeeting.isOn = false;
                    _toggleMeetingAndSeminar.isOn = _toggleCalendar.isOn = _toggleBuiltInBoard.isOn = true;
					_toggleKanban.isOn = true;
                }
            });
            _toggleTrial.onValueChanged.AddListener((isOn) => {
                if (isOn) 
                {
                    _toggleFileBox.isOn = _toggleTodo.isOn = _toggleContact.isOn = _toggleCall.isOn = _toggleChat.isOn = true;
					_toggleMeeting.isOn = false;
                    _toggleMeetingAndSeminar.isOn = _toggleCalendar.isOn = _toggleBuiltInBoard.isOn = true;
					_toggleKanban.isOn = true;
                }
            });


            // set 'module list' listener
            _btnSave.onClick.AddListener(() => {
                // plan set
                if (_toggleFree.isOn)           menuState.planType = ePlanType.Free; 
                else if (_toggleBasic.isOn)     menuState.planType = ePlanType.Basic; 
                else if (_toggleStandard.isOn)  menuState.planType = ePlanType.Standard; 
                else if (_togglePremium.isOn)   menuState.planType = ePlanType.Premium; 
                else if (_toggleTrial.isOn)     menuState.planType = ePlanType.Trial; 
                else if (_toggleCustom.isOn)    menuState.planType = ePlanType.Custom; 

                // feature set
                menuState.fileBox = _toggleFileBox.isOn;
                menuState.todo = _toggleTodo.isOn;
                menuState.contact = _toggleContact.isOn;
                menuState.chat = _toggleChat.isOn;
                menuState.call = _toggleCall.isOn;
                menuState.meeting = _toggleMeeting.isOn;
                menuState.meetingAndSeminar = _toggleMeetingAndSeminar.isOn;
                menuState.calendar = _toggleCalendar.isOn;
                menuState.kanban = _toggleKanban.isOn;
                menuState.builtInBoard = _toggleBuiltInBoard.isOn;

                // and notify
                string pref = JsonUtility.ToJson(menuState);
                // Debug.Log($"{TAG} | save pref : {pref}");

                PlayerPrefs.SetString(key, pref);
                NotifyAll(menuState);
            }); 
        }

        private void OnEnable() 
        {
            string pref = PlayerPrefs.GetString(key, string.Empty);
            // Debug.Log($"{TAG} | load pref : {pref}");

            FloatingMenu loadMenu = null;
            if (string.IsNullOrEmpty(pref)) 
                loadMenu = new FloatingMenu();
            else 
                loadMenu = JsonUtility.FromJson<FloatingMenu>(pref);

            menuState.UpdateState(loadMenu);
            UpdateToggleState();
        }

    #endregion


    #region ModuleController functions - implementations

        private void UpdateToggleState() 
        {
            // Debug.Log($"{TAG} | current plan : {menuState.planType}");
            switch (menuState.planType) 
            {
                case ePlanType.Free :
                    _toggleFree.isOn = true;
					break;

				case ePlanType.Basic :
                    _toggleBasic.isOn = true;
					break;

				case ePlanType.Standard :
                    _toggleStandard.isOn = true;
					break;

				case ePlanType.Premium :
                    _togglePremium.isOn = true;
                    break;

				case ePlanType.Trial :
                    _toggleTrial.isOn = true;
					break;

				case ePlanType.Custom :
                    _toggleCustom.isOn = true;

                    _toggleFileBox.isOn = menuState.fileBox;
                    _toggleTodo.isOn = menuState.todo;
                    _toggleContact.isOn = menuState.contact;
                    _toggleChat.isOn = menuState.chat;
                    _toggleCall.isOn = menuState.call;
                    _toggleMeeting.isOn = menuState.meeting;
                    _toggleMeetingAndSeminar.isOn = menuState.meetingAndSeminar;
                    _toggleCalendar.isOn = menuState.calendar;
                    _toggleKanban.isOn = menuState.kanban;
                    _toggleBuiltInBoard.isOn = menuState.builtInBoard;
					break;
            }
        }

        public void RegisterObserver(IModuleObserver observer) 
        {
            if (listObservers.Contains(observer)) return;

            listObservers.Add(observer);
            observer.UpdateModuleList(menuState);
        }

        public void UnregisterObserver(IModuleObserver observer) 
        {
            if (listObservers.Contains(observer)) 
            {
                listObservers.Remove(observer);
            }
        }

        public void Notify<T>(IModuleObserver observer, T menu) 
        {
            if (listObservers.Contains(observer)) 
            {
                observer.UpdateModuleList(menu);
            }
        }

        public void NotifyAll<T>(T menu) 
        {
            foreach (IModuleObserver observer in listObservers) 
            {
                observer.UpdateModuleList(menu);
            }
        }

    #endregion  // ModuleController functions - implementations
    }
}