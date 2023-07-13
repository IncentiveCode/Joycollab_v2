/// <summary>
/// Module 테스트를 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 07. 13
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 14) : 최초 생성, observer pattern 을 이용한 Tester 구현.
///     v0.2 (2023. 07. 13) : font size change, scene load 등의 테스트 버튼 연결.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
{
    public class ModuleController : MonoBehaviour, IModuleController
    {
        private static List<IModuleObserver> listObservers = new List<IModuleObserver>();    
        private static FloatingMenu menuState = new FloatingMenu();
        private const string key = "__floating_menu_state";


        [Header("top menu")]
        [SerializeField] private Button _btnExit;
        [SerializeField] private Button _btnFontSmall;
        [SerializeField] private Button _btnFontNormal;
        [SerializeField] private Button _btnFontLarge;

        [Header("module list")]
        [SerializeField] private Toggle _toggleFileBox;
        [SerializeField] private Toggle _toggleTodo;
        [SerializeField] private Toggle _toggleContact;
        [SerializeField] private Toggle _toggleChat;
        [SerializeField] private Toggle _toggleCall;
        [SerializeField] private Toggle _toggleMeeting;
        [SerializeField] private Toggle _toggleSeminar;
        [SerializeField] private Toggle _toggleCalendar;
        [SerializeField] private Toggle _toggleKanban;
        [SerializeField] private Button _btnSave;


    #region Unity functions

        private void Awake()
        {
            _btnExit.onClick.AddListener(() => SystemManager.singleton.Exit());
            _btnFontSmall.onClick.AddListener(() => SystemManager.singleton.SetFontOpt(1));
            _btnFontNormal.onClick.AddListener(() => SystemManager.singleton.SetFontOpt(2));
            _btnFontLarge.onClick.AddListener(() => SystemManager.singleton.SetFontOpt(3));

            _btnSave.onClick.AddListener(() => {
                menuState.fileBox = _toggleFileBox.isOn;
                menuState.todo = _toggleTodo.isOn;
                menuState.contact = _toggleContact.isOn;
                menuState.chat = _toggleChat.isOn;
                menuState.call = _toggleCall.isOn;
                menuState.meeting = _toggleMeeting.isOn;
                menuState.seminar = _toggleSeminar.isOn;
                menuState.calendar = _toggleCalendar.isOn;
                menuState.kanban = _toggleKanban.isOn;

                PlayerPrefs.SetString(key, JsonUtility.ToJson(menuState));
                NotifyAll(menuState);
            }); 
        }

        private void OnEnable() 
        {
            string pref = PlayerPrefs.GetString(key, string.Empty);
            // Debug.Log(pref);

            if (! string.IsNullOrEmpty(pref)) 
            {
                menuState.UpdateState(JsonUtility.FromJson<FloatingMenu>(pref));
            }

            UpdateToggleState();
        }

    #endregion


    #region ModuleController functions - implementations

        private void UpdateToggleState() 
        {
            _toggleFileBox.isOn = menuState.fileBox;
            _toggleTodo.isOn = menuState.todo;
            _toggleContact.isOn = menuState.contact;
            _toggleChat.isOn = menuState.chat;
            _toggleCall.isOn = menuState.call;
            _toggleMeeting.isOn = menuState.meeting;
            _toggleSeminar.isOn = menuState.seminar;
            _toggleCalendar.isOn = menuState.calendar;
            _toggleKanban.isOn = menuState.kanban;
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