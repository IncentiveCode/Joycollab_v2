using UnityEngine;
using UnityEngine.UI;

namespace PitchSolution
{
    public class ModuleObserver : MonoBehaviour, IModuleObserver
    {
        [Header("Module Controller")]
        [SerializeField] private ModuleController _moduleController;

        [Header("Floating Menu")]
        [SerializeField] private Button _btnFileBox;
        [SerializeField] private Button _btnTodo;
        [SerializeField] private Button _btnContact;
        [SerializeField] private Button _btnChat;
        [SerializeField] private Button _btnCall;
        [SerializeField] private Button _btnMeeting;
        [SerializeField] private Button _btnSeminar;
        [SerializeField] private Button _btnCalendar;
        [SerializeField] private Button _btnKanban;


    #region Unity functions
        private void Start() 
        {
            if (_moduleController != null) 
            {
                _moduleController.RegisterObserver(this);
            }
        }

        private void OnDestroy() 
        {
            if (_moduleController != null) 
            {
                _moduleController.UnregisterObserver(this);
            }
        }
    #endregion  // Unity functions


    #region ModuleObserver functions - implementations
        public void UpdateModuleList<T>(T menu) 
        {
            if (menu.GetType() == typeof(FloatingMenu)) 
            {
                FloatingMenu m = menu as FloatingMenu;  
                _btnFileBox.gameObject.SetActive(m.fileBox);
                _btnTodo.gameObject.SetActive(m.todo);
                _btnContact.gameObject.SetActive(m.contact);
                _btnChat.gameObject.SetActive(m.chat);
                _btnCall.gameObject.SetActive(m.call);
                _btnMeeting.gameObject.SetActive(m.meeting);
                _btnSeminar.gameObject.SetActive(m.seminar);
                _btnCalendar.gameObject.SetActive(m.calendar);
                _btnKanban.gameObject.SetActive(m.kanban);
            }
        }
    #endregion  // ModuleObserver functions - implementations
    }
}