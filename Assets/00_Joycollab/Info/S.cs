/// <summary>
/// 자주 사용하는 string 값 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 10. 06
/// @version        : 1.2
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성. Scene 이름, LoginView, MobileView 이름 정리
///     v0.2 (2023. 04. 07) : Language, region 관련 상수 정리 
///     v0.3 (2023. 06. 28) : avatar state 값 정리
///     v0.4 (2023. 07. 18) : mobile navigation 값 추가
///     v0.5 (2023. 07. 31) : subscription plan 값 추가
///     v0.6 (2023. 08. 11) : view tag 추가, WorldScene FixedView Name 추가, 이용약관 관련 값 추가.
///     v0.7 (2023. 08. 16) : WorldScene view name 수정.
///     v0.8 (2023. 08. 23) : 일부 클래스 이름 변경으로 인해 상수값 수정.
///     v0.9 (2023. 08. 24) : Management Scene 상수값 추가.
///     v1.0 (2023. 08. 25) : URL.GET_CODE 에서 사용하는 Top Code 상수값 추가. 
///     v1.1 (2023. 09. 14) : World 에서 사용할 WindowView 이름 정리 (진행 중)
///     v1.2 (2023. 10. 06) : Popup menu string 정리
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

        public const string Canvas_Top = "Top Canvas";          // order 2
        public const string Canvas_Window = "Window Canvas";    // order 1
        public const string Canvas_System = "System Canvas";    // order 0

        public const string Canvas_Master = "Master Canvas";
        public const string Canvas_Menu = "Menu Canvas";
        public const string Canvas_Popup = "Popup Canvas";
        public const string Canvas_Popup_M = "SafeArea _ popup";
        public const string Canvas_Progress_M = "SafeArea _ progress";
        public const string Canvas_Webview = "Webview Canvas";
        public const string Canvas_Webview_M = "SafeArea _ webview";

    #endregion  // Canvas name


    #region SignInScene FixedView Name 

        public const string SignInScene_ViewTag = "SignInView";

        public const string SignInScene_SignIn = "sign in";
        public const string SignInScene_SubSignIn = "sub sign in";
        public const string SignInScene_Guest = "guest";
        public const string SignInScene_PatchNote = "patch note";
        public const string SignInScene_Reset = "reset";
        public const string SignInScene_Restore = "UIView_Restore"; // "Restore";
        public const string SignInScene_Agreement = "UIView_Agreement"; // "Agreement";
        public const string SignInScene_Terms = "UIView_Terms"; // "Terms";
        public const string SignInScene_Greetings = "UIView_Greetings"; // "Greetings";
        public const string SignInScene_SignUp = "UIView_Join"; // "sign up";
        public const string SignInScene_Info = "UIView_Info"; // "Info";
        public const string SignInScene_SignDone = "UIView_JoinDone"; // "sign done";
        public const string SignInScene_CreateOffice = "UIView_CreateOffice"; // "CreateOffice";

    #endregion  // SignInScene FixedView Name 


    #region ManagementScene FixedView Name

        public const string ManagementScene_ViewTag = "ManagementView";

        public const string ManagementScene_MyInfo = "my info";
        public const string ManagementScene_CreateWorkspace = "create workspace";
        public const string ManagementScene_WorkspaceInfo = "workspace info";
        public const string ManagementScene_Plan = "plan";

    #endregion  // ManagementScene FixedView Name


    #region MobileScene FixedView Name 

        public const string MobileScene_ViewTag = "MobileView";

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


    #region World FixedView Name

        public const string WorldScene_ViewTag = "WorldView";

        public const string WorldScene_SignIn = "sign in_w";
        public const string WorldScene_Guest = "guest_w";
        public const string WorldScene_Reset = "reset_w";
        public const string WorldScene_Restore = "restore_w";
        public const string WorldScene_Agreement = "agreement_w";
        public const string WorldScene_Terms = "terms_w";
        public const string WorldScene_SignUp = "sign up_w";
        public const string WorldScene_Info = "info_w";

    #endregion  // World FixedView Name


    #region World WindowView Name

        public const string WorldScene_AlarmList = "alarm list_w";
        public const string WorldScene_Bookmark = "bookmark_w";
        public const string WorldScene_Settings = "settings_w";
        public const string WorldScene_UserList = "user list_w";
        public const string WorldScene_RoomList = "room list_w";
        public const string WorldScene_CreateRoom = "create room_w";

    #endregion  // World WindowView Name


    #region Language

        public const string LANGUAGE_KOREAN = "Korean";
        public const string LANGUAGE_ENGLISH = "English";
        public const string LANGUAGE_JAPANESE = "Japanese";

        public const string REGION_KOREAN = "ko";
        public const string REGION_ENGLISH = "en";
        public const string REGION_JAPANESE = "ja";

    #endregion  // Language


    #region Value

        public const string TRUE = "true";
        public const string FALSE = "false";
        public const string ID = "id";
        public const string PW = "pw";
        public const string WORLD = "world";
        public const string EDITOR = "editor";
        public const string SYSTEM_UPDATE_CODE = "gansini88";
        public const string WORKSPACE = "workspace";
        public const string WORKSPACE_MOBILE = "workspace_app";
        public const string SEMINAR = "세미나";

    #endregion  // Value


    #region Terms

        public const string TERMS_OF_USAGE = "이용약관";
        public const string TERMS_OF_PRIVACY = "개인정보취급방침";
        public const string TERMS_OF_MARKETING = "3자 정보 제공 동의";
        public const string CURRENT_AGREEMENT = "current_agreement_check_state";

    #endregion  // Terms


    #region Top code

        public const string TC_MEMBER_STATUS = "멤버 상태";
        public const string TC_ALARM_SOUND = "알림음 구분"; 

    #endregion  // Top code


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
        public const string ALARM_ID_REJECT_VOICE = "음성통화 거절";
        public const string ALARM_ID_MISSED_VOICE = "부재중 음성통화 요청";

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


    #region Popup menu

        public const string MENU_SHARE_FILEBOX = "공유 파일";
        public const string MENU_DEPT_BOARD = "부서 게시판";
        public const string MENU_DEPT_TODO = "부서 할 일";
        public const string MENU_DEPT_CALL = "부서 음성 통화";
        public const string MENU_DEPT_MEETING = "부서 화상 회의";


        public const string MENU_GROUP = "그룹";
        public const string MENU_GROUP_CALL = "그룹 음성 통화";
        public const string MENU_GROUP_CHAT = "그룹 채팅";
        public const string MENU_GROUP_MEETING = "그룹 화상 회의";


        public const string MENU_VIEW = "상세 보기";
        public const string MENU_CALL = "음성 통화";
        public const string MENU_CHAT = "채팅";
        public const string MENU_MEETING = "화상 회의";
        public const string MENU_FILEBOX = "파일함 보기";
        public const string MENU_TODO = "할 일 보기";


        public const string MENU_MOVE_MY_SEAT = "내 자리로 이동";
        public const string MENU_MOVE_LOBBY = "로비로 이동";
        public const string MENU_MOVE_LOUNGE = "휴게실로 이동";


        public const string MENU_RENAME = "이름 변경";
        public const string MENU_NEW_FOLDER = "새 폴더";
        public const string MENU_OPEN = "열기";
        public const string MENU_DOWNLOAD = "다운로드";
        public const string MENU_COPY = "복사";
        public const string MENU_PASTE = "붙여넣기";
        public const string MENU_DELETE = "삭제";
        public const string MENU_DELETE_FILE = "파일 삭제";
        public const string MENU_SELECT_ALL = "전체 선택";


        public const string MENU_CANCEL_ARRANGE = "배치 취소";


        public const string MENU_DETAILS = "상세보기";
        public const string MENU_ENTER = "입장";
        public const string MENU_MOVE_TO_OFFICE = "오피스로 이동";

    #endregion  // Popup menu
    }
}
