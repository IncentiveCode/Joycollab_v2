/// <summary>
/// 자주 사용하는 string 값 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 03. 20
/// @version        : 1.0
/// @update
///     v1.0 (2023. 03. 20) : Scene 이름, LoginView, MobileView 이름 정리
/// </summary>

namespace Joycollab.v2
{
    public class S
    {
    #region Scene name
        public const string SceneName_Login = "Login";
        public const string SceneName_Graphic = "Main";
        public const string SceneName_Text = "TextUI";
        public const string SceneName_Arrange = "ArrangeSpace";
        public const string SceneName_Management = "Management";
        public const string SceneName_World = "World";
        public const string SceneName_Square = "Square";
        public const string SceneName_Mobile = "Mobile";
        public const string SceneName_Loading = "Loading";
        public const string SceneName_Test = "Test";
    #endregion  // Scene name


    #region Canvas
        public const string POPUP_CANVAS = "Popup Canvas";
        public const string WORLD_CANVAS = "World Canvas";
    #endregion


    #region LoginScene FixedView Name 
        // workspace login
        public const string LoginScene_ViewTag = "LoginView";
        public const string LoginScene_Login = "Login";
        public const string LoginScene_SubLogin = "SubLogin";
        public const string LoginScene_GuestLogin = "GuestLogin";
        public const string LoginScene_PatchNote = "PatchNote";
        public const string LoginScene_Reset = "Reset";
        public const string LoginScene_Restore = "Restore";
        public const string LoginScene_Agreement = "Agreement";
        public const string LoginScene_Terms = "Terms";
        public const string LoginScene_Greetings = "Greetings";
        public const string LoginScene_Join = "Join";
        public const string LoginScene_Info = "Info";
        public const string LoginScene_JoinDone = "JoinDone";
        public const string LoginScene_CreateOffice = "CreateOffice";

        // world login view
        public const string LoginScene_World_Login = "Login_w";
        public const string LoginScene_World_SubLogin = "SubLogin_w";
        public const string LoginScene_World_GuestLogin = "GuestLogin_w";
        public const string LoginScene_World_Reset = "Reset_w";
        public const string LoginScene_World_Restore = "Restore_w";
        public const string LoginScene_World_Agreement = "Agreement_w";
        public const string LoginScene_World_Terms = "Terms_w";
        public const string LoginScene_World_Join = "Join_w";
        public const string LoginScene_World_Info = "Info_w";
    #endregion  // LoginScene FixedView Name 


    #region MobileScene FixedView Name 
        public const string MobileScene_ViewTag = "MobileView";
        public const string MobileScene_Login = "login_m";
        public const string MobileScene_Reset = "reset_m";
        public const string MobileScene_PatchNote = "patch note_m";
        public const string MobileScene_LoadInfo = "load info_m";
        public const string MobileScene_Office = "office_m";
        public const string MobileScene_MySeat = "my seat_m";
        public const string MobileScene_FileBox = "file box_m";
        public const string MobileScene_Meeting = "meeting_m";
        public const string MobileScene_Chat = "chat_m";
        public const string MobileScene_Dummy = "dummy_m";  // test 를 위한 임시 화면
    #endregion  // MobileScene FixedView Name 
    }
}
