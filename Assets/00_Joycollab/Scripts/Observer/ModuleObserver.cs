/// <summary>
/// module 사용 변경 여부를 확인하기 위한 observer. Floating Menu 에서 주로 사용.
/// @author         : HJ Lee
/// @last update    : 2023. 08. 01
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 14) : 최초 생성.
///     v0.2 (2023. 06. 02) : module interface 관련 정리. 
///     v0.3 (2023. 08. 01) : 통합 게시판 추가. '미팅' 만 있는 메뉴와 '미팅+세미나' 가 있는 메뉴 분리.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2
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
        [SerializeField] private Button _btnMeetingAndSeminar;
        [SerializeField] private Button _btnCalendar;
        [SerializeField] private Button _btnKanban;
        [SerializeField] private Button _btnBuiltInBoard;


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
                _btnMeetingAndSeminar.gameObject.SetActive(m.meetingAndSeminar);
                _btnCalendar.gameObject.SetActive(m.calendar);
                _btnKanban.gameObject.SetActive(m.kanban);
                _btnBuiltInBoard.gameObject.SetActive(m.builtInBoard);
            }
        }

    #endregion  // ModuleObserver functions - implementations
    }
}