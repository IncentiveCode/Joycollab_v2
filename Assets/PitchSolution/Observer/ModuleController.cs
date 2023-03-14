using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PitchSolution
{
    public class ModuleController : MonoBehaviour, IModuleController
    {
        private static List<IModuleObserver> listObservers = new List<IModuleObserver>();    
        private static FloatingMenu menuState = new FloatingMenu();
        private const string key = "__floating_menu_state";


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