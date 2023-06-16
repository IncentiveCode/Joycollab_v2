/// <summary>
/// 자주 사용하는 string 값 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 04. 07
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성. Scene 이름, LoginView, MobileView 이름 정리
///     v0.2 (2023. 04. 07) : Language, region 관련 상수 정리 
/// </summary>

namespace Joycollab.v2
{
    public class S
    {
    #region Scene name

        public const string SceneName_Login = "LoginScene";
        public const string SceneName_Graphic = "MainScene";
        public const string SceneName_Text = "TextUIScene";
        public const string SceneName_Arrange = "ArrangeSpaceScene";
        public const string SceneName_Management = "ManagementScene";
        public const string SceneName_World = "WorldScene";
        public const string SceneName_Square = "SquareScene";
        public const string SceneName_Mobile = "MobileScene";
        public const string SceneName_Loading = "LoadingScene";
        public const string SceneName_Test = "TestScene";

    #endregion  // Scene name


    #region Canvas name

        public const string Canvas_Popup = "Popup Canvas";
        public const string Canvas_Popup_M = "SafeArea _ popup";
        public const string Canvas_Progress_M = "SafeArea _ progress";

    #endregion  // Canvas name


    #region LoginScene FixedView Name 

        // workspace login -> 추후 변경 예정
        public const string LoginScene_Login = "login";
        public const string LoginScene_SubLogin = "sub login";
        public const string LoginScene_GuestLogin = "guest login";
        public const string LoginScene_PatchNote = "patch note";
        public const string LoginScene_Reset = "reset";
        public const string LoginScene_Restore = "UIView_Restore"; // "Restore";
        public const string LoginScene_Agreement = "UIView_Agreement"; // "Agreement";
        public const string LoginScene_Terms = "UIView_Terms"; // "Terms";
        public const string LoginScene_Greetings = "UIView_Greetings"; // "Greetings";
        public const string LoginScene_Join = "UIView_Join"; // "Join";
        public const string LoginScene_Info = "UIView_Info"; // "Info";
        public const string LoginScene_JoinDone = "UIView_JoinDone"; // "JoinDone";
        public const string LoginScene_CreateOffice = "UIView_CreateOffice"; // "CreateOffice";

        // world login view
        public const string LoginScene_World_Login = "WorldView_Login"; // "Login_w";
        public const string LoginScene_World_SubLogin = "SubLogin_w";
        public const string LoginScene_World_GuestLogin = "WorldView_GuestLogin"; // "GuestLogin_w";
        public const string LoginScene_World_Reset = "WorldView_Reset"; // "Reset_w";
        public const string LoginScene_World_Restore = "WorldView_Restore"; // "Restore_w";
        public const string LoginScene_World_Agreement = "WorldView_Agreement"; // "Agreement_w";
        public const string LoginScene_World_Terms = "WorldView_Terms"; // "Terms_w";
        public const string LoginScene_World_Join = "WorldView_Join"; // "Join_w";
        public const string LoginScene_World_Info = "WorldView_Info"; // "Info_w";

    #endregion  // LoginScene FixedView Name 


    #region MobileScene FixedView Name 


        // ---------- ---------- ----------
        // 01. login

        public const string MobileScene_Login = "login_m";
        public const string MobileScene_Reset = "reset_m";
        public const string MobileScene_PatchNote = "patch note_m";
        public const string MobileScene_LoadInfo = "load info_m";


        // ---------- ---------- ----------
        // 02. main (top, bottom navigation menu & main menu) 

        public const string MobileScene_MyPage = "my page_m";
        public const string MobileScene_Alarm = "alarm_m";
        public const string MobileScene_Channel = "channel_m";

        public const string MobileScene_Office = "office_m";
        public const string MobileScene_MySeat = "my seat_m";
        public const string MobileScene_FileRoot = "file root_m";
        public const string MobileScene_MeetingRoot = "meeting root_m";
        public const string MobileScene_Chat = "chat_m";


        // ---------- ---------- ----------
        // 03. To-Do 

        public const string MobileScene_ToDo = "todo_m";
        public const string MobileScene_ToDoDetail = "todo detail_m";
        public const string MobileScene_CreateTodo = "create todo_m";
        public const string MobileScene_Okr = "okr_m";
        public const string MobileScene_OkrDetail = "okr detail_m";
        public const string MobileScene_CreateOkr = "create okr_m";


        // ---------- ---------- ----------
        // 04. Board

        public const string MobileScene_Board = "board_m";
        public const string MobileScene_PostDetail = "post detail_m";
        public const string MobileScene_PostWrite = "post write_m";


        // ---------- ---------- ----------
        // 05. Notice

        public const string MobileScene_Notice = "notice_m";
        public const string MobileScene_NoticeWrite = "notice write_m";
        public const string MobileScene_NoticeDetail = "notice detail_m";
        public const string MobileScene_SystemNotice = "system notice_m";
        public const string MobileScene_SystemNoticeDetail = "system notice detail_m";


        // ---------- ---------- ----------
        // 06. File Box

        public const string MobileScene_FileBox = "file box_m";
        public const string MobileScene_SharedFileBox = "shared box_m";


        // ---------- ---------- ----------
        // 07. Meeting 

        public const string MobileScene_Meeting = "meeting_m";
        public const string MobileScene_MeetingCreate = "meeting create_m";
        public const string MobileScene_MeetingDetail = "meeting detail_m";
        public const string MobileScene_MeetingHistory = "meeting history_m";
        public const string MobileScene_MeetingMembers = "meeting members_m";


        // ---------- ---------- ----------
        // 08. Seminar 

        public const string MobileScene_Seminar = "seminar_m";
        public const string MobileScene_SeminarCreate = "seminar create_m";
        public const string MobileScene_SeminarDetail = "seminar detail_m";
        public const string MobileScene_SeminarHistory = "seminar history_m";


        // ---------- ---------- ----------
        // 09. my page 

        public const string MobileScene_Bookmark = "bookmark_m";
        public const string MobileScene_Contact = "contact_m";
        public const string MobileScene_MyInfo = "my info_m";
        public const string MobileScene_MyEmotion = "my emotion_m";
        public const string MobileScene_Attendance = "attendance_m";
        public const string MobileScene_AttendanceHistory = "attendance history_m";
        public const string MobileScene_MyWorkspace = "my workspace_m";
        public const string MobileScene_Settings = "settings_m";
        public const string MobileScene_Subscriptions = "subscriptions_m";
        public const string MobileScene_Feedback = "feedback_m";


        // ---------- ---------- ----------
        // 10. Popup
        
        public const string MobileScene_MemberDetail = "member detail_m";
        public const string MobileScene_SlidePopup = "slide popup_m";

    #endregion  // MobileScene FixedView Name 


    #region Language

        public const string LANGUAGE_KOREAN = "Korean";
        public const string LANGUAGE_ENGLISH = "English";

        public const string REGION_KOREAN = "ko";
        public const string REGION_ENGLISH = "en";

    #endregion  // Language


    #region Value

        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string WORLD = "world";

    #endregion  // Value


    #region Member type

        public const string USER = "일반사용자";
        public const string MANAGER = "매니저";
        public const string ADMIN = "관리자";
        public const string OWNER = "소유자";
        public const string COLLABORATOR = "협업자";
        public const string GUEST = "게스트"; 

    #endregion  // Member type
    }
}
