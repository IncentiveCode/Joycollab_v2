/// <summary>
/// NetworkTask 를 위한 API URL 정리 문서 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 17
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 17) : Joycollab 에서 사용하던 열거형 정리. (정리 중)
///     v0.2 (2023. 03. 20) : Tray app 관련 URL 추가. (아직 정리 중)
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

        // ---------- ---------- ----------
        // Basic token
        public const string BASIC_TOKEN = "Basic YWRtOmdhbnNpbmk=";
        public const string MOBILE_BASIC_TOKEN = "Basic YXBwOmdhbnNpbmk=";

        // ---------- ---------- ----------
        // Grant type
        public const string GRANT_TYPE = "grant_type";
        public const string PASSWORD = "password";
        public const string REFRESH_TOKEN = "refresh_token";
        public const string USERNAME = "username";

        // ---------- ---------- ----------
        // Scope
        public const string SCOPE = "scope";
        public const string SCOPE_ADM = "adm";
        public const string SCOPE_APP = "app";

        // ---------- ---------- ----------
        // for NetworkTask
        public const string ACCEPT_LANGUAGE = "Accept-Language";
        public const string CONTENT_TYPE = "Content-Type";
        public const string CONTENT_JSON = "application/json";
        public const string AUTHORIZATION = "Authorization";
    #endregion  // Constants


    #region Server address
        public const string PATH = DEV ? 
            "https://dev.jcollab.com" : 
            "https://jcollab.com";

        public const string SERVER_PATH = PATH +"/serv";
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


        // ---------- ---------- ----------
        // 03. Member API

        /// <summary>
        /// desc : 특정 workspace 에 가입
        /// method : post
        /// </summary>
        /// <value>{0} : workspace seq</value>
        public const string JOIN_MEMBER = SERVER_PATH +"/api/member/{0}"; 

    #endregion
    }
}