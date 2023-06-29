/// <summary>
/// View ID 값 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 06. 16
/// @version        : 0.2
/// @update
///     v0.1 (2023. 04. 19) : 최초 생성. 기존 Joycollab 에서 사용하는 ID 와 Mobile View ID 정리
///     v0.2 (2023. 06. 16) : Mobile Seminar 관련 ID 추가
/// </summary>

namespace Joycollab.v2
{
	public class ID
	{
		// ---------- ---------- ----------
		// UI Panel List - TOP
		public const int TOP = 0;
		public const int CHANNEL = 1;
		public const int ALARM = 2;
		public const int PROFILE = 3;
		public const int STATE = 4;
		public const int BOOKMARK = 5;
		public const int CONFIG = 6;
		public const int USER_INFO = 7;
		public const int FEEDBACK = 8;


		// ---------- ---------- ----------
		// UI Window List - FLOAT
		public const int FLOATING_BAR = 9;
		public const int CONTACT = 10;
		public const int BOARD_LIST = 11;
		public const int BOARD_CREATE = 12;
		public const int BOARD_DETAIL = 13;
		public const int NOTICE_LIST = 14;
		public const int NOTICE_CREATE = 15;
		public const int NOTICE_DETAIL = 16;
		public const int MEETING_LIST = 17;
		public const int MEETING_CREATE = 18;
		public const int MEETING_ADD_MEMBER = 19;
		public const int MEETING_DETAIL = 20;
		public const int WORKSPACE_MANAGEMENT = 21;
		public const int SYSTEM_NOTICE_LIST = 24;
		public const int NEW_WORK = 26;
		public const int SETTING = 27; // HJ Lee. 2022. 02. 04. NEW_WORK 와 SETTING 모두 26이라서 SETTING 을 27로 수정.
		public const int ATTENDANCE = 30;
		public const int SEMINAR_LIST = 31;
		public const int SEMINAR_CREATE = 32;
		public const int SEMINAR_ADD_MEMBER = 33;
		public const int SEMINAR_DETAIL = 34;
		public const int MEETING_AND_SEMINAR = 35;
		public const int SEMINAR_DESCRIPTION = 36;
		public const int HARD_UI_BOARD = 37;
		public const int HARD_UI_NOTICE = 38;
		public const int HARD_UI_SEMINAR = 39;


		public const int TODO_LIST = 500;// 공유 -500, 개인 - 500+seq


		public const int SYSTEM_NOTICE_DETAIL = 1000; //1000 +seq가 해당 seq시스템 공지사항의 win id


		// ---------- ---------- ----------
		// Arrange View - 
		public const int ARRANGE_SPACE = 22;
		public const int ARRANGE_MEMBER = 23;

		// ---------- ---------- ----------
		// Meeting History - 
		public const int MEETING_HISTORY = 25;

		// ---------- ---------- ----------
		// main scene background - 
		public const int MAIN_BACKGROUND = 28;

		// ---------- ---------- ----------
		// main scene background - 
		public const int SETTING_WINDOW = 29;

		// ---------- ---------- ----------
		// UI Window List - Multi Window, ID 는 그 때 그 때 추가 배정.
		public const int LOBBY_WAITING = 40;


	#region MobileScene ID

		// ---------- ---------- ----------
		// 00. common

		public const int MobileScene_NaviTop = 10000;
		public const int MobileScene_NaviBottom = 10001;
		public const int MobileScene_Template = 10002;


		// ---------- ---------- ----------
		// 01. login

		public const int MobileScene_Login = 10010;
		public const int MobileScene_Reset = 10011;
		public const int MobileScene_PatchNote = 10012;
		public const int MobileScene_LoadInfo = 10013;


		// ---------- ---------- ----------
		// 02. main (top, bottom navigation menu & main menu) 

        public const int MobileScene_MyPage = 10020;
        public const int MobileScene_Alarm = 10021;
        public const int MobileScene_Channel = 10022;

        public const int MobileScene_Office = 10023;
        public const int MobileScene_MySeat = 10024;
        public const int MobileScene_FileRoot = 10025;
        public const int MobileScene_MeetingRoot = 10026;
        public const int MobileScene_Chat = 10027;


        // ---------- ---------- ----------
        // 03. To-Do 

        public const int MobileScene_ToDo = 10030;
        public const int MobileScene_ToDoDetail = 10031;
        public const int MobileScene_CreateTodo = 10032;
		public const int MobileScene_ShareToDo = 10033;
		public const int MobileScene_Okr = 10034;
		public const int MobileScene_OkrDetail = 10035;
        public const int MobileScene_CreateOkr = 10036;


        // ---------- ---------- ----------
        // 04. Board

        public const int MobileScene_Board = 10040;
        public const int MobileScene_PostWrite = 10041;
        public const int MobileScene_PostDetail = 10042;


        // ---------- ---------- ----------
        // 05. Notice

        public const int MobileScene_Notice = 10050;
        public const int MobileScene_NoticeWrite = 10051;
        public const int MobileScene_NoticeDetail = 10052;
        public const int MobileScene_SystemNotice = 10053;
        public const int MobileScene_SystemNoticeDetail = 10054;


        // ---------- ---------- ----------
        // 06. File Box

        public const int MobileScene_FileBox = 10060;
        public const int MobileScene_SharedFileBox = 10061;


        // ---------- ---------- ----------
        // 07. Meeting 

		public const int MobileScene_Meeting = 10070;
        public const int MobileScene_MeetingCreate = 10071;
        public const int MobileScene_MeetingDetail = 10072;
        public const int MobileScene_MeetingHistory = 10073;
        public const int MobileScene_MeetingMembers = 10074;


        // ---------- ---------- ----------
        // 08. Seminar 

		public const int MobileScene_Seminar = 10080;
		public const int MobileScene_SeminarCreate = 10081;
		public const int MobileScene_SeminarDetail = 10082;


		// ---------- ---------- ----------
        // 09. my page 

        public const int MobileScene_Bookmark = 10090;
        public const int MobileScene_Contact = 10091;
        public const int MobileScene_MyInfo = 10092;
        public const int MobileScene_MyEmotion = 10093;
        public const int MobileScene_Attendance = 10094;
        public const int MobileScene_AttendanceHistory = 10095;
        public const int MobileScene_MyWorkspace = 10096;
        public const int MobileScene_Settings = 10097;
        public const int MobileScene_Subscriptions = 10098;
        public const int MobileScene_Feedback = 10099;


        // ---------- ---------- ----------
        // 10. Popup
        
        public const int MobileScene_MemberDetail = 10100;
        public const int MobileScene_SlidePopup = 10101;

	#endregion	// MobileScene ID
	}
}