/// <summary>
/// 자주 사용하는 string 값 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 07. 31
/// @version        : 0.5
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성. Scene 이름, LoginView, MobileView 이름 정리
///     v0.2 (2023. 04. 07) : Language, region 관련 상수 정리 
///     v0.3 (2023. 06. 28) : avatar state 값 정리
///     v0.4 (2023. 07. 18) : mobile navigation 값 추가
///     v0.5 (2023. 07. 31) : subscription plan 값 추가
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
        public const string Canvas_Webview = "Webview Canvas";
        public const string Canvas_Webview_M = "SafeArea _ webview";

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
        // 00. navigation

        public const string MobileNaviTag = "MobileNavigation";
        public const string MobileScene_Top = "top_m";
        public const string MobileScene_Bottom = "bottom_m";


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
        public const string MobileScene_ShareToDo = "todo share_m";
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
        public const string MobileScene_Test = "test";

    #endregion  // MobileScene FixedView Name 


    #region Language

        public const string LANGUAGE_KOREAN = "Korean";
        public const string LANGUAGE_ENGLISH = "English";

        public const string REGION_KOREAN = "ko";
        public const string REGION_ENGLISH = "en";
        public const string REGION_JAPANESE = "jp";

    #endregion  // Language


    #region Value

        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string WORLD = "world";
        public const string EDITOR = "editor";
        public const string SYSTEM_UPDATE_CODE = "gansini88";
        public const string WORKSPACE = "workspace";
        public const string WORKSPACE_MOBILE = "workspace_app";
        public const string SEMINAR = "세미나";

    #endregion  // Value


    #region Member type

        public const string USER = "일반사용자";
        public const string MANAGER = "매니저";
        public const string ADMIN = "관리자";
        public const string OWNER = "소유자";
        public const string COLLABORATOR = "협업자";
        public const string GUEST = "게스트"; 

    #endregion  // Member type


    #region permission

        // - 로비, 로비 권한
        public const string LOBBY = "로비";
        public const string AUTH_CREATE_NOTICE = "공지사항 작성";

        // - 회의실, 회의실 권한
        public const string MEETING_ROOM = "회의실";
        public const string AUTH_CREATE_MEETING = "회의 생성/수정";
        public const string AUTH_READ_MEETING = "회의 조회";
        public const string AUTH_INVITE_MEETING = "회의 초대/추방";

        // - 휴게실
        public const string LOUNGE = "휴게실";

        // - 게시판 권한
        public const string AUTH_READ_BOARD = "게시글 조회";
        public const string AUTH_CREATE_BOARD = "게시글 작성";
        public const string AUTH_CREATE_COMMENT = "게시글 댓글 작성";

        // - 파일함 권한
        public const string AUTH_CREATE_FOLDER = "폴더 생성";
        public const string AUTH_DELETE_FOLDER = "폴더 삭제";
        public const string AUTH_READ_FILE = "파일 조회";
        public const string AUTH_UPLOAD_FILE = "파일 업로드";
        public const string AUTH_DOWNLOAD_FILE = "파일 다운로드";
        public const string AUTH_DELETE_FILE = "파일 삭제";

        // - 스페이스에서 사용하는 권한
        public const string AUTH_SEATING_ARRANGE = "자리 배치";
    
    #endregion  // permission


    #region avatar state

        public const string ONLINE = "온라인";
        public const string OFFLINE = "오프라인";
        public const string MEETING = "회의중";
        public const string LINE_BUSY = "통화중";
        public const string BUSY = "바쁨";
        public const string OUT_ON_BUSINESS = "외근중";
        public const string NOT_HERE = "자리비움";
        public const string DO_NOT_DISTURB = "방해금지";
        public const string VACATION = "휴가중";
        public const string NOT_AVAILABLE = "부재중";
    
    #endregion  // avatar status


    #region share, reminder option

        public const int SHARE_NONE = 0;
        public const int SHARE_DEPARTMENT = 1;
        public const int SHARE_COMPANY = 2;

        public const int TYPE_ALL = 0;
        public const int TYPE_DEPARTMENT = 1;
        public const int TYPE_COMPANY = 2;

        public const int TYPE_DAILY = 0;
        public const int TYPE_WEEKLY = 1;
        public const int TYPE_MONTHLY = 2;

    #endregion  // share, reminder option


    #region alarm contents

        // - 회의 관련 Alarm ID
        public const string ALARM_ID_RESERVE_MEETING = "회의 생성/예약";
        public const string ALARM_ID_START_MEETING = "회의 시작";
        public const string ALARM_ID_DONE_MEETING = "회의 종료";

        // - 음성 통화 관련 Alarm ID
        public const string ALARM_ID_REQUEST_VOICE = "음성통화 요청";
        public const string ALARM_ID_REJECT_VOICE = "음성통화 요청";

        // - 배치, 정보 변경 관련 Alarm ID
        public const string ALARM_ID_UPDATE_MEMBER = "멤버 정보 변경";
        public const string ALARM_ID_ARRANGE_SPACE = "공간 배치 변경";
        public const string ALARM_ID_ARRANGE_SEAT = "자리 배치 변경";


        public const string ALARM_RESERVE_MEETING = "회의 예약";
        public const string ALARM_UPDATE_MEETING = "회의 변경";
        public const string ALARM_DELETE_MEETING = "회의 삭제";
        public const string ALARM_INVITE_MEETING = "회의 초대";
        public const string ALARM_INVITE_MEETING_CANCEL = "회의 초대 취소";
        public const string ALARM_START_MEETING = "회의 시작";
        public const string ALARM_DONE_MEETING = "회의 종료";

        public const string ALARM_RESERVE_SEMINAR = "세미나 예약";
        public const string ALARM_UPDATE_SEMINAR = "세미나 변경";
        public const string ALARM_DELETE_SEMINAR = "세미나 삭제";

        public const string ALARM_VOICE_CALL = "음성 통화";
        public const string ALARM_REJECT_CALL = "음성 통화 거절";
        
        public const string ALARM_TO_DO = "TODO";
        public const string ALARM_TASK = "일감";

        public const string ALARM_UPDATE_MEMBER = "멤버 정보 변경";
        public const string ALARM_UPDATE_SPACE = "공간 배치 변경";
        public const string ALARM_UPDATE_SEAT = "멤버 배치 변경";

    #endregion  // alarm contents


    #region plan

        public const string PLAN_FREE = "무료 플랜";
        public const string PLAN_BASIC = "베이직";
        public const string PLAN_STANDARD = "스탠다드";
        public const string PLAN_PREMIUM = "프리미엄";

    #endregion  // plan
    }
}
