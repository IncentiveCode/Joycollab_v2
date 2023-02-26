#define DEV // Dev Server
// #define RELEASE // Live Server

namespace PitchSolution
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
        private const string PATH = DEV ? 
            "https://dev.jcollab.com" : 
            "https://jcollab.com";

        private const string SERVER_PATH = PATH +"/serv";
    #endregion  // Server address


    #region TokenApi

        /// <summary>
        /// desc : token 만료 확인
        /// method : post
        /// </summary>
        /// <value>token</value>
        public const string CHECK_TOKEN = SERVER_PATH + "/oauth/check_token?token={0}";

        /// <summary>
        /// desc : token 요청 [Login]
        /// method : post
        /// </summary>
        public const string REQUEST_TOKEN = SERVER_PATH + "/oauth/token";

    #endregion  // TokenApi


    #region NonAuthApi

        // ---------- ---------- ----------
        // 007. Workspace API

        /// <summary>
        /// desc : domain 확인
        /// method : get
        /// </summary>
        /// <value>domain name</value>
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
        /// <value>workspace seq</value>
        public const string WORKSPACE_INFO = SERVER_PATH +"/api/workspace/{0}";


        // ---------- ---------- ----------
        // 03. Member API

        /// <summary>
        /// desc : 특정 workspace 에 가입
        /// method : post
        /// </summary>
        /// <value>workspace seq</value>
        public const string JOIN_MEMBER = SERVER_PATH +"/api/member/{0}"; 

    #endregion
    }
}