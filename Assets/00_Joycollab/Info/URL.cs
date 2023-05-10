/// <summary>
/// NetworkTask 를 위한 API URL 정리 문서 
/// @author         : HJ Lee
/// @last update    : 2023. 05. 10
/// @version        : 0.4
/// @update
///     v0.1 (2023. 03. 17) : Joycollab 에서 사용하던 열거형 정리.
///     v0.2 (2023. 03. 20) : Tray app 관련 URL 추가.
///     v0.3 (2023. 05. 03) : Index, World Index 추가.
///     v0.4 (2023. 05. 10) : 사용하지 않는 일부 항목 삭제 
/// </summary>

#define DEV // Dev Server
// #define RELEASE // Live Server

namespace Joycollab.v2
{
    public class URL
    {
    #region Constants
    
        // ---------- ---------- ----------
        // Develop mode
        #if DEV 
        public const bool DEV = true;
        #elif RELEASE
        public const bool DEV = false;
        #endif

    #endregion  // Constants


    #region Server address

        public const string PATH = DEV ? 
            "https://dev.jcollab.com" : 
            "https://jcollab.com";

        public const string SERVER_PATH = PATH +"/serv";

        public const string XMPP_SERVER = DEV ?
            "dev.jcollab.com" :
            "chat.jcollab.com";

        public const string INDEX = PATH +"/workspace/";
        public const string SUB_INDEX = PATH +"/{0}/workspace/";
        public const string WORLD_INDEX = PATH +"/world/";

    #endregion  // Server address


    #region Tray App

        public const string JCINFO_OPEN = "jcinfo://open?id=onlytree&url={0}&seq={1}&token={2}&time={3}";
        public const string JCINFO_CLOSE = "jcinfo://close";
        public const string JCINFO_PATH_WIN = PATH + "/bundles/tray/jcinfoSetup.msi";
        public const string JCINFO_PATH_MAC = PATH + "/bundles/tray/jcinfo.pkg";
        public const string JCINFO_DOWNLOAD_PATH_WIN = "C:/Users/{0}/Downloads/jcinfoSetup.msi";
        public const string JCINFO_DOWNLOAD_PATH_MAC = "/Users/{0}/Downloads/jcinfo.pkg";

    #endregion  // Tray App


    #region TokenApi

        /// <summary>
        /// desc : token 만료 확인
        /// method : post
        /// </summary>
        /// <value>{0} : token</value>
        public const string CHECK_TOKEN = SERVER_PATH + "/oauth/check_token?token={0}";

        /// <summary>
        /// desc : token 요청 [Login]
        /// method : post
        /// </summary>
        public const string REQUEST_TOKEN = SERVER_PATH + "/oauth/token";

    #endregion  // TokenApi


    #region NonAuthApi

        // ---------- ---------- ----------
        // 001. Member API (without authorization)

        /// <summary>
        /// desc : 비밀번호 재설정 인증키 요청
        /// method : get
        /// </summary>
        /// <value>{0} : id</value>
        /// <value>{1} : phone</value>
        public const string REQUEST_RESET = SERVER_PATH + "/npr/api/user/ckey/{0}/{1}";

        /// <summary>
        /// desc : 회원 ID 로 회원 정보 조회
        /// method : get
        /// </summary>
        /// <value>{0} : member id</value>
        public const string CHECK_ID = SERVER_PATH + "/npr/api/user/getUser/{0}";


        // ---------- ---------- ----------
        // 007. Workspace API

        /// <summary>
        /// desc : domain 확인
        /// method : get
        /// </summary>
        /// <value>{0} : domain name</value>
        public const string CHECK_DOMAIN = SERVER_PATH + "/npr/workspace/getByDomain/{0}";

    #endregion  // NonAuthApi


    #region UserApi

        // ---------- ---------- ----------
        // 02. Workspace API

        /// <summary>
        /// desc : 내가 속한 workspace 의 목록 확인
        /// method : post, get
        /// </summary>
        public const string WORKSPACE_LIST = SERVER_PATH +"/api/workspace";

        /// <summary>
        /// desc : 특정 workspace 의 정보 확인
        /// method : put, get, delete
        /// </summary>
        /// <value>{0} : workspace seq</value>
        public const string WORKSPACE_INFO = SERVER_PATH +"/api/workspace/{0}";

        /// <summary>
        /// desc : 특정 workspace 의 설정 정보 확인
        /// method : get, patch
        /// </summary>
        /// <value>{0} : workspace seq</value>
        public const string WORKSPACE_LOBBY_INFO = SERVER_PATH +"/api/workspace/{0}/setting";


        // ---------- ---------- ----------
        // 03. Member API

        /// <summary>
        /// desc : 특정 사용자 정보 확인
        /// method : get, put, delete
        /// </summary>
        /// <value>{0} : member seq</value>
        public const string MEMBER_INFO = SERVER_PATH +"/api/member/{0}";

        /// <summary>
        /// desc : 특정 workspace 에 가입
        /// method : post
        /// </summary>
        /// <value>{0} : workspace seq</value>
        public const string JOIN_MEMBER = SERVER_PATH +"/api/member/{0}"; 


        // ---------- ---------- ----------
        // 06. Alarm API

        /// <summary>
        /// desc : 알림 갯수 정보 확인
        /// method : get
        /// </summary>
        /// <value></value>
        public const string ALARM_COUNT = SERVER_PATH +"/api/alarm/cnt/{0}";

    #endregion
    }
}