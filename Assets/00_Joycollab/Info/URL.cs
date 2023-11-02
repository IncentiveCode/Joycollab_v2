/// <summary>
/// NetworkTask 를 위한 API URL 정리 문서 
/// @author         : HJ Lee
/// @last update    : 2023. 09. 06
/// @version        : 1.3
/// @update
///     v0.1 (2023. 03. 17) : Joycollab 에서 사용하던 열거형 정리
///     v0.2 (2023. 03. 20) : Tray app 관련 URL 추가
///     v0.3 (2023. 04. 04) : Joycollab 에서 사용하던 URL 정리. (Server address, System link, Google Drive API, Joycollab Web Page Link)
///     v0.4 (2023. 04. 05) : Joycollab 에서 사용하던 URL 정리. (AdmApi, UserApi - workspace, meeting)
///     v0.5 (2023. 04. 14) : Joycollab 에서 사용하던 URL 정리. (UserApi - member)
///     v0.6 (2023. 06. 19) : Joycollab 에서 사용하던 URL 정리. (UserApi - File)
///     v0.7 (2023. 07. 03) : Joycollab 에서 사용하던 URL 정리. (UserApi - OKR)
///     v0.8 (2023. 07. 06) : Joycollab 에서 사용하던 URL 정리. (UserApi - Board)
///     v0.9 (2023. 07. 07) : Joycollab 에서 사용하던 URL 정리. (UserApi - Bookmark)
///     v0.10 (2023. 08. 01) : Joycollab 에서 사용하던 URL 정리. (UserApi - Attendance)
///     v0.11 (2023. 08. 30) : v1 에서 사용하던 URL 정리 완료. World API 정리
///     v0.12 (2023. 09. 01) : File API V2 정리
///     v0.13 (2023. 09. 06) : google token 조회 및 연동 해제 부분의 parameter 삭제.
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

        #if DEV     // dev server
        public const bool DEV = true;
        #else
        public const bool DEV = false;
        #endif

    #endregion  // Constants


    #region Server address

        public const string PATH = DEV ? 
            "https://dev.jcollab.com" : 
            "https://jcollab.com";

        public const string XMPP_SERVER = DEV ? 
            "dev.jcollab.com" :
            "chat.jcollab.com";

        public const string INDEX = PATH +"/workspace2/";

        /// <summary>
        /// desc : 특정 domain 으로 바로 이동할 수 있는 INDEX
        /// param
        ///     {0} : sub domain
        /// </summary>
        public const string SUB_INDEX = PATH +"/{0}/workspace2/";

        public const string WORLD_INDEX = PATH +"/world2/";

        public const string SERVER_PATH = PATH +"/serv";

        public const string SYSTEM_NOTICE_PATH = PATH +"/noti_serv/notice";

        public const string SYSTEM_SOUND_PATH = SERVER_PATH +"/alarm/";

    #endregion  // Server address


    #region System Link 

        /// <summary>
        /// desc : XMPP 연동 링크
        /// param 
        ///     {0} : member seq
        ///     {1} : xmpp password
        /// </summary>
        public const string XMPP_AUTH = PATH +"/xmpp_client/{0}/{1}";

        /// <summary>
        /// desc : kakao api - 위도와 경도값으로 한국 주소를 획득
        /// param 
        ///     {0} : latitude
        ///     {1} : longitude
        /// </summary>
        public const string KAKAO_ADDRESS = "https://dapi.kakao.com/v2/local/geo/coord2address.json?y={0}&x={1}";

        /// <summary>
        /// desc : weather api - 아이콘 리스트 획득
        /// </summary>
        public const string GET_WEATHER_ICONS = "https://www.weatherapi.com/docs/weather_conditions.json";

        /// <summary>
        /// desc : weather api - 위도와 경도값으로 현재 날씨 획득 
        /// param 
        ///     {0} : latitude
        ///     {1} : longitude
        /// </summary>
        public const string WEATHER_API = "https://api.weatherapi.com/v1/current.json?key=1c4893f0d31e4ca0a9154107221403&q={0},{1}&aqi=no"; 

        /// <summary>
        /// desc : zoom 연동 인증 링크 
        /// param
        ///     {0} : member seq
        /// </summary>  
        public const string ZOOM_AUTH = DEV ? 
            "https://zoom.us/oauth/authorize?response_type=code&client_id=yVrQCroLSlu7OBBoRNwhvw&redirect_uri=https://dev.jcollab.com/serv/zoom/oauth&state={0}" :
            "https://zoom.us/oauth/authorize?response_type=code&client_id=g_YGfwmQeyz3t1GWY4EIg&redirect_uri=https://jcollab.com/serv/zoom/oauth&state={0}";

        /// <summary>
        /// desc : google 연동 인증 링크 
        /// param
        ///     {0} : joycollab id
        /// </summary>  
        public const string GOOGLE_AUTH = DEV ?
            "https://accounts.google.com/o/oauth2/v2/auth?redirect_uri=https://dev.jcollab.com/serv/google/oauth&client_id=1029682796818-n46k67eoufrue8p504pl12h554b7ktvt.apps.googleusercontent.com&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile&prompt=consent&response_type=code&access_type=offline&state={0}" :
            "https://accounts.google.com/o/oauth2/v2/auth?redirect_uri=https://jcollab.com/serv/google/oauth&client_id=1029682796818-j4hl9h8fqnb6qe9a9lsmstra8s89evs6.apps.googleusercontent.com&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fcalendar+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile&prompt=consent&response_type=code&access_type=offline&state={0}";   

    #endregion  // System Link


    #region Google Drive API

        public const string GOOGLE_PATH = "https://www.googleapis.com/";

        /// <summary>
        /// desc : 파일 목록 관리
        /// method : get (파일 리스트 획득), post (새 폴더 생성)
        /// </summary>
        public const string GOOGLE_DRIVE = GOOGLE_PATH +"drive/v3/files";

        /// <summary>
        /// desc : 파일 관리
        /// method : delete (파일 삭제), patch (파일 이름 변경)
        /// param
        ///     {0} : file id
        /// </summary>
        public const string CONTROL_GOOGLE_DRIVE = GOOGLE_PATH +"drive/v3/files/{0}";

        /// <summary>
        /// desc : 파일 업로드
        /// method : post
        /// </summary>
        public const string UPLOAD_GOOGLE_FILE = GOOGLE_PATH +"upload/drive/v3/files?uploadType=multipart";

        /// <summary>
        /// desc : 파일 다운로드
        /// method : get
        /// param
        ///     {0} : file id
        /// </summary>
        public const string DOWNLOAD_GOOGLE_FILE = GOOGLE_PATH +"drive/v3/files/{0}?alt=media";

        /// <summary>
        /// desc : 구글 독스 파일 다운로드
        /// method : get
        /// param
        ///     {0} : file id
        ///     {1} : mime type
        /// </summary>
        public const string DOWNLOAD_GOOGLE_DOCS = GOOGLE_PATH +"drive/v3/files/{0}/export?mimeType={1}";

        /// <summary>
        /// desc : 파일의 부모 정보 확인
        /// method : get
        /// param
        ///     {0} : file id
        /// </summary>
        public const string GET_GOOGLE_FILE_PARENTS = GOOGLE_PATH +"drive/v3/files/{0}?fields=parents, name, id";

        /// <summary>
        /// desc : 파일에 부모 정보 추가
        /// method : patch
        /// param
        ///     {0} : file id
        ///     {1} : parent id
        /// </summary>
        /// <value></value>
        public const string ADD_GOOGLE_FILE_PARENT = GOOGLE_PATH +"drive/v3/files/{0}?addParents={1}";

        /// <summary>
        /// desc : 파일 복사
        /// method : post
        /// param
        ///     {0} : file id
        /// </summary>
        public const string COPY_GOOGLE_FILE = GOOGLE_PATH +"drive/v3/files/{0}/copy";

        /// <summary>
        /// desc : 파일 붙여넣기
        /// method : patch
        /// param
        ///     {0} : file id
        ///     {1} : parent id
        /// </summary>
        public const string PASTE_GOOGLE_FILE = GOOGLE_PATH +"drive/v3/files/{0}?addParents={1}";

        /// <summary>
        /// desc : 권한 확인
        /// method : post
        /// param
        ///     {0} : file id
        /// </summary>
        public const string CHECK_GOOGLE_PERMISSION = GOOGLE_PATH +"drive/v3/files/{0}/permissions";

    #endregion  // Google Drive API


    #region Tray App

        /// <summary>
        /// desc : jcinfo 실행 scheme
        /// param
        ///     {0} : URL.PATH
        ///     {1} : member seq
        ///     {2} : token (bearer + accessToken)
        ///     {3} : 자리 비움 설정 시간 (해당 시간 동안 입력이 없으면 사용자의 상태가 자리비움으로 변경됨)
        /// </summary>
        public const string JCINFO_OPEN = "jcinfo://open?id=onlytree&url={0}&seq={1}&token={2}&time={3}";
        
        /// <summary>
        /// desc : jcinfo 종료 scheme
        /// </summary>
        public const string JCINFO_CLOSE = "jcinfo://close";

        /// <summary>
        /// desc : tray 설치 파일 경로 (win / mac)
        /// </summary>
        public const string JCINFO_PATH_WIN = PATH +"/bundles/tray/jcinfoSetup.msi";
        public const string JCINFO_PATH_MAC = PATH +"/bundles/tray/jcinfo.pkg";

        /// <summary>
        /// desc : PC 에서 jcinfo 설치 파일 다운로드 받을 경로 (win / mac)
        /// param
        ///     {0} : System.Environment.UserName
        /// </summary>
        public const string JCINFO_DOWNLOAD_PATH_WIN = "C:/Users/{0}/Downloads/jcinfoSetup.msi";
        public const string JCINFO_DOWNLOAD_PATH_MAC = "/Users/{0}/Downloads/jcinfo.pkg";

    #endregion  // Tray App


    #region Joycollab Web Page Link

        /// <summary>
        /// desc : 사용자 관리 화면
        /// param
        ///     {0} : workspace seq
        ///     {1} : member seq
        ///     {2} : token
        ///     {3} : datetime
        ///     {4} : language code (ko / en)
        /// </summary>
        public const string MEMBER_MANAGE_PAGE = PATH +"/setting_service/member?workspaceSeq={0}&memberSeq={1}&token={2}&dt={3}&lan={4}";  

        /// <summary>
        /// desc : 워크스페이스 관리 화면
        /// param
        ///     {0} : workspace seq
        ///     {1} : token
        ///     {2} : datetime
        ///     {3} : language code (ko / en)
        /// </summary>
        public const string SPACE_MANAGE_PAGE = PATH +"/setting_service/space?workspaceSeq={0}&token={1}&dt={2}&lan={3}";

        /// <summary>
        /// desc : 권한 관리 화면
        /// param
        ///     {0} : workspace seq
        ///     {1} : token
        ///     {2} : datetime
        ///     {3} : language code (ko / en)
        /// </summary>
        public const string AUTH_MANAGE_PAGE = PATH +"/setting_service/auth?workspaceSeq={0}&token={1}&dt={2}&lan={3}";      

        /// <summary>
        /// desc : 워크스페이스 대시보드 화면
        /// param
        ///     {0} : workspace seq
        ///     {1} : member seq
        ///     {2} : token
        ///     {3} : language code (ko / en)
        /// </summary>
        public const string DASHBOARD_PAGE = PATH +"/setting_service/dashboard?workspaceSeq={0}&memberSeq={1}&token={2}&lan={3}";  

        /// <summary>
        /// desc : 칸반 보드 화면
        /// param
        ///     {0} : token
        ///     {1} : language code (ko / en)
        ///     {2} : member seq
        ///     {3} : ui type (graphic ui = 1 / text ui = 0)
        /// </summary>
        public const string KANBAN_PAGE = PATH + "/proj_service/proj?token={0}&lan={1}&memberSeq={2}&softUI={3}"; 

        /// <summary>
        /// desc : 캘린더 화면
        /// param
        ///     {0} : workspace seq
        ///     {1} : member seq
        ///     {2} : token
        ///     {3} : language code (ko / en)
        ///     {4} : ui type (graphic ui = 1 / text ui = 0)
        /// </summary>
        public const string CALANDAR_PAGE = PATH +"/calendar/?workspaceSeq={0}&memberSeq={1}&token={2}&lan={3}&softUI={4}";  

        /// <summary>
        /// desc : 정보 게시판 화면
        /// param
        ///     {0} : member seq
        ///     {1} : token
        ///     {2} : language code
        /// </summary>
        public const string BUILLETIN_BOARD = PATH +"/board_service/main/0/{0}?token={1}&lan={2}";

        /// <summary>
        /// desc : 튜토리얼 화면
        /// param 
        ///     {0} : language (빈 값이면 한글 튜토리얼 / _EN 이 붙으면 영문 튜토리얼)
        /// </summary>
        public const string TUTORIAL_PATH = PATH +"/help/help{0}.html";

        /// <summary>
        /// desc : 결제창 링크
        /// param
        ///     {0} : workspace seq
        ///     {1} : language (ko or en)
        ///     {2} : plan id
        ///     {3} : monthly or yearly (1 or 12)
        ///     {4} : user number
        /// </summary>
        public const string PAYMENT = SERVER_PATH +"/npr/workspace/pay/{0}/{1}/PAY/{2}/{3}/{4}";

        /// <summary>
        /// desc : 결제 카드 설정
        /// param
        ///     {0} : workspace seq
        ///     {1} : language (ko or en)
        /// </summary>
        public const string PAYMENT_CARD = SERVER_PATH +"/npr/workspace/pay/{0}/{1}/AUTH";

    #endregion  // Manage Web Page Link


    #region TokenApi

        /// <summary>
        /// desc : token 만료 확인
        /// method : post
        /// param
        ///     {0} : token
        /// </summary>
        public const string CHECK_TOKEN = SERVER_PATH +"/oauth/check_token?token={0}";

        /// <summary>
        /// desc : token 요청 [Login]
        /// method : post
        /// </summary>
        public const string REQUEST_TOKEN = SERVER_PATH +"/oauth/token";

    #endregion  // TokenApi


    #region AdmApi

        // ---------- ---------- ----------
        // 01. Space API

        /// <summary>
        /// desc : 공간 관리
        /// method : get (공간 조회), put (공간 수정), delete (공간 삭제)
        /// param
        ///     {0} : space seq
        /// </summary>
        public const string CONTROL_SPACE = SERVER_PATH +"/api/space/{0}";

        /// <summary>
        /// desc : 공간 내 멤버 조회
        /// method : get
        /// param
        ///     {0} : space seq
        /// </summary>
        public const string MEMBER_LIST_IN_SPACE = SERVER_PATH +"/api/space/{0}/members"; 

        /// <summary>
        /// desc : 공간 등록
        /// method : post
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string REGIST_SPACE = SERVER_PATH +"/api/space/{0}";

        /// <summary>
        /// desc : 공간 배치 설정 
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string SET_SPACE_POSITION = SERVER_PATH +"/api/space/position/{0}";

        /// <summary>
        /// desc : 워크스페이스 내 공간 조회
        /// method : get
        /// param 
        ///     {0} : workspace seq
        /// </summary>
        public const string SPACE_LIST = SERVER_PATH +"/api/space/search/{0}";


        // ---------- ---------- ----------
        // 02. Member API

        /// <summary>
        /// desc : 해당 워크스페이스의 모든 사용자 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// TODO. check
        ///     WORKSPACE_MEMBER_LIST 와 동일한 기능을 함. 굳이 추가로 필요한지 확인 요망
        /// </summary>
        public const string ENTIRE_MEMBER_LIST = SERVER_PATH +"/member/{0}";

        /// <summary>
        /// desc : 사용자 지정석 배치 (여러 사용자 한번에 배치 가능)
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string SET_POSITION_FOR_ADMIN = SERVER_PATH +"/member/position/fixe/{0}";

        /// <summary>
        /// desc : 해당 스페이스에 지정된 사용자 목록 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string SPACE_MEMBER_LIST = SERVER_PATH +"/member/space/{0}/{1}"; 

        /// <summary>
        /// desc : 부서에 지정된 사용자 목록 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string TOPSPACE_MEMBER_LIST = SERVER_PATH +"/member/topSpace/{0}/{1}";

    #endregion  // AdmApi


    #region AdminApi

        // ---------- ---------- ----------
        // 07. Code Ctrl

        /// <summary>
        /// desc : top code 로 하위 코드 조회
        /// method : get
        /// param
        ///     {0} : top code
        /// </summary>
        /// <value></value>
        public const string GET_CODE = SERVER_PATH + "/code/getCode/{0}";

    #endregion  // AdminApi


    #region MngApi

        // ---------- ---------- ----------
        // 01. Noti API

        /// <summary>
        /// desc : 공지사항 목록 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : language
        ///     {3} : keyword
        ///     {4} : member type 이 guest 면 true, 그 외엔 false
        /// </summary>
        public const string NOTICE_LIST = SERVER_PATH +"/api/noti/{0}/{1}?lanId={2}&title={3}&guest={4}";  

        /// <summary>
        /// desc : 공지사항 등록
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string REGIST_NOTICE = SERVER_PATH +"/api/noti/{0}/{1}"; 

        /// <summary>
        /// desc : 공지사항 항목 관리
        /// method : put (수정), get (상세 조회), delete (삭제)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : notice seq
        /// </summary>
        public const string CONTROL_NOTICE = SERVER_PATH +"/api/noti/{0}/{1}/{2}";

        /// <summary>
        /// desc : 공지사항 파일 업로드
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string UPLOAD_NOTICE_FILE = SERVER_PATH +"/api/noti/files/{0}/{1}"; 

        /// <summary>
        /// desc : 공지사항 파일 다운로드
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file path
        /// </summary>
        public const string DOWNLOAD_NOTICE_FILE = SERVER_PATH +"/api/noti/down/{0}/{1}?filePath={2}";

        /// <summary>
        /// desc : 최초 진입시 시스템 공지사항 확인 (팝업으로 출력할 항목 확인)
        /// method : get
        /// param
        ///     {0} : page no
        ///     {1} : popup true / false, 빈 값으로 던질시 전체 항목 조회
        ///     {2} : page size
        /// </summary>
        public const string CHECK_SYSTEM_NOTICE = SERVER_PATH +"/api/noti/sys?page={0}&popup={1}&size={2}";  

        /// <summary>
        /// desc : 시스템 공지사항 목록 조회
        /// method : get
        /// param
        ///     {0} : page no
        ///     {1} : page size
        /// </summary>
        public const string SYSTEM_NOTICE_LIST = SERVER_PATH +"/api/noti/sys/list?page={0}&size={1}";

        /// <summary>
        /// desc : 시스템 공지사항 게시글 조회
        /// method : get
        /// param
        ///     {0} : system notice seq
        /// </summary>
        public const string SYSTEM_NOTICE_INFO = SERVER_PATH +"/api/noti/sys/{0}";


        // ---------- ---------- ----------
        // 22. History API 

        /// <summary>
        /// desc : 로비 대기자 명단 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string GUEST_WAITING_LIST = SERVER_PATH +"/api/hist/{0}/guest"; 

    #endregion  // MngApi


    #region NonAuthApi

        // ---------- ---------- ----------
        // 000. Timezone API (without authorization)

        /// <summary>
        /// desc : Timezone List 조회
        /// method : get
        /// </summary>
        public const string TIMEZONE_LIST = SERVER_PATH +"/npr/api/timezone";


        // ---------- ---------- ----------
        // 001. Member API (without authorization)

        /// <summary>
        /// desc : 회원가입
        /// method : post
        /// </summary>
        public const string REQUEST_JOIN = SERVER_PATH +"/npr/api/user";

        /// <summary>
        /// desc : 초대 받은 메일인지 확인 (member)
        /// method : get
        /// param 
        ///     {0} : e-mail
        /// </summary>
        public const string CHECK_INVITE = SERVER_PATH +"/npr/api/user/{0}/invites";

        /// <summary>
        /// desc : 초대 받은 사용자인지 확인 (new user)
        /// method : get
        /// param
        ///     {0} : e-mail
        ///     {1} : ckey
        /// </summary>
        /// <value></value>
        public const string CHECK_INVITE_WITH_CKEY = SERVER_PATH +"/npr/api/user/{0}/invite?ckey={1}";

        /// <summary>
        /// desc : 비밀번호 재설정 인증키 요청
        /// method : get
        /// param
        ///     {0} : id
        ///     {1} : phone
        /// </summary>
        public const string REQUEST_CODE = SERVER_PATH +"/npr/api/user/ckey/{0}/{1}";

        /// <summary>
        /// desc : 회원 ID 로 회원 정보 조회
        /// method : get
        /// param
        ///     {0} : member id
        /// </summary>
        public const string CHECK_ID = SERVER_PATH +"/npr/api/user/getUser/{0}";

        /// <summary>
        /// desc : 게스트 로그인
        /// method : post
        /// param
        ///     {0} : guest name
        ///     {1} : workspace seq
        /// </summary>
        public const string SIGN_IN_AS_GUEST = SERVER_PATH +"/npr/api/user/guest?nm={0}&workspaceSeq={1}";

        /// <summary>
        /// desc : ckey 없이 회원가입
        /// method : post
        /// </summary>
        public const string REGISTER = SERVER_PATH +"/npr/api/user/register";

        /// <summary>
        /// desc : 회원 계정 복구
        /// method : patch
        /// param
        ///     {0} : code
        ///     {1} : id (e-mail)
        ///     {2} : password
        ///     {3} : phone number
        /// </summary>
        public const string RESTORE_ID = SERVER_PATH +"/npr/api/user/restore?ckey={0}&id={1}&newPw={2}&tel={3}";

        /// <summary>
        /// desc : 회원 비밀번호 재설정
        /// method : patch
        /// param
        ///     {0} : 인증 코드
        ///     {1} : id (e-mail)
        ///     {2} : password
        ///     {3} : phone number
        /// </summary>
        public const string RESET_PW = SERVER_PATH +"/npr/api/user/restPw?ckey={0}&id={1}&newPw={2}&tel={3}";


        // ---------- ---------- ----------
        // 002. Terms API (without authorization)

        /// <summary>
        /// desc : 이용약관 조회
        /// method : get
        /// param
        ///     {0} : 약관 코드
        /// </summary>
        public const string TERMS = SERVER_PATH +"/npr/api/terms/{0}";


        // ---------- ---------- ----------
        // 007. Workspace API

        /// <summary>
        /// desc : domain 확인
        /// method : get
        /// param
        ///     {0} : domain name
        /// </summary>
        public const string CHECK_DOMAIN = SERVER_PATH +"/npr/workspace/getByDomain/{0}";

        /// <summary>
        /// desc : seq 로 workspace 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string CHECK_WORKSPACE = SERVER_PATH +"/npr/workspace/getByWorkspaceSeq/{0}";


        // ---------- ---------- ----------
        // 011. Plan API

        /// <summary>
        /// desc : plan list 조회
        /// method : get
        /// </summary>
        public const string PLAN_LIST = SERVER_PATH + "/npr/plan";


        // ---------- ---------- ----------
        // 012. Meeting Npr API

        /// <summary>
        /// desc : seminar 에 등록된 파일 다운로드 (for Guest)
        /// method : get
        /// param
        ///     {0} : seminar seq
        ///     {1} : file path
        /// </summary>
        public const string DOWNLOAD_SEMINAR_FILE_FOR_GUEST = SERVER_PATH +"/npr/meeting/down/{0}?filePath={1}";

    #endregion  // NonAuthApi


    #region UserApi

        // ---------- ---------- ----------
        // 01. User API

        /// <summary>
        /// desc : 회원 정보 관리
        /// method : put (수정), get (조회), delete (탈퇴)
        /// </summary>
        public const string USER_INFO = SERVER_PATH +"/api/user";

        /// <summary>
        /// desc : 로그아웃
        /// method : delete
        /// param 
        ///     {0} : user id (e-mail)
        /// </summary>
        public const string USER_LOGOUT = SERVER_PATH +"/api/user/logout?name={0}";

        /// <summary>
        /// desc : Google access token 조회
        /// method : get
        /// </summary>
        public const string GOOGLE_ACCESS_TOKEN = SERVER_PATH +"/api/user/googleAccessToken";

        /// <summary>
        /// desc : Google refresh token 조회
        /// method : get
        /// </summary>
        public const string GOOGLE_REFRESH_TOKEN = SERVER_PATH +"/api/user/googleRefreshToken";

        /// <summary>
        /// desc : Google 연동 해제
        /// method : delete
        /// </summary>
        public const string REVOKE_GOOGLE_CONNECTION = SERVER_PATH +"/api/user/revokingGoogleToken";


        // ---------- ---------- ----------
        // 02. Workspace API

        /// <summary>
        /// desc : 내가 속한 workspace 정보 관리
        /// method : post (workspace 생성), get (공간, 멤버 조회)
        /// </summary>
        public const string WORKSPACE_LIST = SERVER_PATH +"/api/workspace";

        /// <summary>
        /// desc : 특정 workspace 의 정보 관리
        /// method : put (workspace 수정), get (workspace 조회), delete (workspace 삭제)
        /// param
        ///     {0} : workspace seq 
        /// </summary>
        public const string WORKSPACE_INFO = SERVER_PATH +"/api/workspace/{0}";

        /// <summary>
        /// desc : 특정 worksapce 의 특정 member 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : member seq
        /// </summary>
        public const string WORKSPACE_MEMBER_INFO = SERVER_PATH +"/api/workspace/{0}/member/{1}";

        /// <summary>
        /// desc : 특정 worksapce 의 모든 member 간단 정보 조회 (탈퇴 사용자도 포함)
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_MEMBER_SIMPLE_ALL = SERVER_PATH +"/api/workspace/{0}/member/all";
        
        /// <summary>
        /// desc : 특정 worksapce 의 모든 member 간단 정보 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_MEMBER_SIMPLE_LIST = SERVER_PATH +"/api/workspace/{0}/member/list";

        /// <summary>
        /// desc : 특정 worksapce 의 모든 member 모든 정보 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_MEMBER_LIST = SERVER_PATH +"/api/workspace/{0}/members";

        /// <summary>
        /// desc : 초대 거절
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string REJECT_WORKSPACE_INVITE = SERVER_PATH +"/api/workspace/{0}/reject";

        /// <summary>
        /// desc : 로비 정보 조회 및 설정
        /// method : get (조회), patch (설정 변경)
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_LOBBY_INFO = SERVER_PATH +"/api/workspace/{0}/setting";

        /// <summary>
        /// desc : 로비 기타 정보 조회 
        /// method : get 
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_LOBBY_INFO2 = SERVER_PATH +"/api/workspace/{0}/setting2";

        /// <summary>
        /// desc : 로비 모니터 이미지 설정
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_MONITOR_IMAGE = SERVER_PATH +"/api/workspace/{0}/settingMonitImg";

        /// <summary>
        /// desc : 로비 모니터 영상 정보 설정
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_MONITOR_URL = SERVER_PATH +"/api/workspace/{0}/settingMonitUrl";

        /// <summary>
        /// desc : 로비 공지사항 이미지 설정
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_NOTICE_IMAGE = SERVER_PATH +"/api/workspace/{0}/settingNoticeImg";

        /// <summary>
        /// desc : 근태 정보 조회 및 설정
        /// method : get (조회), patch (설정 변경)
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_ATTENDANCE_INFO = SERVER_PATH +"/api/workspace/attSet/{0}";

        /// <summary>
        /// desc : 결제 기록 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_PAY_HISTORY = SERVER_PATH +"/api/workspace/history/{0}";

        /// <summary>
        /// desc : 상단 로고 이미지 설정
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORKSPACE_TOP_LOGO = SERVER_PATH +"/api/workspace/logo/{0}";

        /// <summary>
        /// desc : 플랜 변경
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : user count
        ///     {2} : period (1 or 12)
        ///     {3} : plan id
        /// </summary>
        public const string CHANGE_PLAN = SERVER_PATH +"/api/workspace/plan/{0}?limitNum={1}&period={2}&planId={3}";

        /// <summary>
        /// desc : 플랜 변경 결과 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : user count
        ///     {2} : period (1 or 12)
        ///     {3} : plan id
        /// </summary>
        public const string CHECK_NEXT_PLAN = SERVER_PATH +"/api/workspace/plan/changeResult/{0}?limitNum={1}&period={2}&planId={3}";

        /// <summary>
        /// desc : 특정 workspace 의 plan 구독 정보 확인
        /// method : get
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string CHECK_PLAN = SERVER_PATH +"/api/workspace/plan/chk/{0}";


        // ---------- ---------- ----------
        // 03. Member API

        /// <summary>
        /// desc : 특정 사용자 정보 확인
        /// method : get (정보 조회), put (수정), delete (삭제)
        /// param 
        ///     {0} : member seq
        /// </summary>
        public const string MEMBER_INFO = SERVER_PATH +"/api/member/{0}";

        /// <summary>
        /// desc : 특정 workspace 에 가입
        /// method : post
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string JOIN_MEMBER = SERVER_PATH +"/api/member/{0}"; 

        /// <summary>
        /// desc : 알람 옵션 변경
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : alarm option seq
        /// </summary>
        public const string SET_ALARM_OPTION = SERVER_PATH +"/api/member/alarm/{0}/{1}";

        /// <summary>
        /// desc : 배경 이미지 변경
        /// method : patch
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string SET_BACKGROUND_IMAGE = SERVER_PATH +"/api/member/backgroundImg/{0}";
        
        /// <summary>
        /// desc : [world] 멤버 기업 정보 수정
        /// method : put
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string COMPANY_INFO = SERVER_PATH +"/api/member/comp/{0}";

        /// <summary>
        /// desc : 감정 변경
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : emotion code
        /// </summary>
        public const string SET_EMOTION = SERVER_PATH +"/api/member/emotion/{0}/{1}";

        /// <summary>
        /// desc : FCM 토큰 저장
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : fcm token
        /// </summary>
        /// <value></value>
        public const string SET_FCM_TOKEN = SERVER_PATH +"/api/member/fcm/{0}?token={1}";

        /// <summary>
        /// desc : 게스트가 로비 담당자 호출
        /// method : patch
        /// param 
        ///     {0} : member seq
        ///     {1} : e-mail (optional)
        /// </summary>
        public const string SEND_GUEST_ALARM = SERVER_PATH +"/api/member/guest/alarm/{0}?email={1}";  

        /// <summary>
        /// desc : heartbeat 전송
        /// method : patch
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string SEND_PING = SERVER_PATH +"/api/member/ping/{0}";

        /// <summary>
        /// desc : 사용자 마지막 카메라 값 설정
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : camera orthographic size
        ///     {2} : camera x
        ///     {3} : camera y
        /// </summary>
        public const string SET_CAMERA = SERVER_PATH +"/api/member/position/cam/{0}?camSize={1}&camX={2}&camY={3}";

        /// <summary>
        /// desc : 사용자 위치 설정
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : space seq
        ///     {2} : current pos x
        ///     {3} : current pos y
        /// </summary>
        public const string SET_POSITION = SERVER_PATH +"/api/member/position/current/{0}/{1}?cx={2}&cy={3}";

        /// <summary>
        /// desc : 사용자 지정석 설정
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : space seq
        /// </summary>
        public const string SET_SEAT = SERVER_PATH +"/api/member/position/fixe/{0}/{1}";

        /// <summary>
        /// desc : zoom token 삭제
        /// method : delete
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string REVOKE_ZOOM_CONNECTION = SERVER_PATH +"/api/member/revokingToken/{0}";

        /// <summary>
        /// desc : 언어 및 지역 설정
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : dateFormatStr (yyyy-MM-dd 로 고정)
        ///     {2} : fontSize (1, 2, 3)
        ///     {3} : hourFormatStr (hh, HH)
        ///     {4} : lanId (ko, en, jp)
        ///     {5} : timeZone (현재는 GMT+9 로 고정)
        ///     {6} : weekStart (0 = 일요일, 1 = 월요일)
        /// </summary>
        public const string SET_ENVIRONMENT = SERVER_PATH +"/api/member/setting/{0}?dateFormatStr={1}&fontSize={2}&hourFormatStr={3}&lanId={4}&timeZone={5}&weekStart={6}";

        /// <summary>
        /// desc : 상태 변경
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : status code
        /// </summary>
        public const string SET_STATUS = SERVER_PATH +"/api/member/status/{0}/{1}";

        /// <summary>
        /// desc : 상태 변경 (결과를 전달 받을 때, 요청 시간과 완료 시간 까지 전달 받음)
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : status code
        /// </summary>
        public const string SET_STATUS2 = SERVER_PATH +"/api/member/status2/{0}/{1}";

        /// <summary>
        /// desc : UI 타입 변경
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : ui type (text / graph)
        /// </summary>
        public const string SET_UI_TYPE = SERVER_PATH +"/api/member/uiType/{0}?uiType={1}";


        // ---------- ---------- ----------
        // 04. Call API

        /// <summary>
        /// desc : Voice call 수신을 위한 link (for WebGL)
        /// param 
        ///     {0} : workspace seq
        ///     {1} : call seq
        ///     {2} : member seq
        ///     {3} : language code (ko / en)
        /// </summary>
        public const string CALL_LINK = PATH +"/meeting_service/{0}/{1}/{2}/call?lan={3}";

        /// <summary>
        /// desc : Voice call 수신을 위한 link (for mobile)
        /// param 
        ///     {0} : workspace seq
        ///     {1} : call seq
        ///     {2} : member seq
        ///     {3} : language code (ko / en)
        ///     {4} : access token
        /// </summary>
        public const string CALL_LINK_M = PATH +"/meeting_service/{0}/{1}/{2}/call?lan={3}&token={4}";

        /// <summary>
        /// desc : Voice call 발신을 위한 link
        /// param 
        ///     {0} : workspace seq
        /// </summary>
        public const string MAKE_CALL = SERVER_PATH +"/api/call/{0}";


        // ---------- ---------- ----------
        // 04. Meeting API

        /// <summary>
        /// desc : 화상회의 / 세미나 화면을 출력하기 위한 link
        /// param 
        ///     {0} : workspace seq
        ///     {1} : meeting seq
        ///     {2} : member seq
        ///     {3} : language code (ko / en)
        ///     {4} : mobile 의 경우, token
        /// </summary>
        public const string MEETING_LINK = PATH +"/meeting_service/{0}/{1}/{2}/meeting?lan={3}";
        public const string SEMINAR_LINK = PATH +"/meeting_service/{0}/{1}/{2}/seminar?lan={3}";
        public const string MEETING_LINK_M = PATH +"/meeting_service/{0}/{1}/{2}/meeting_mobile?lan={3}&token={4}";


        /// <summary>
        /// desc : 회의 생성
        /// method : post
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string CREATE_MEETING = SERVER_PATH +"/api/meeting/{0}";

        /// <summary>
        /// desc : 특정 회의 관리
        /// method : put (수정), get (조회), delete (삭제)
        /// param
        ///     {0} : workspace seq
        ///     {1} : meeting seq
        /// </summary>
        public const string MEETING_INFO = SERVER_PATH +"/api/meeting/{0}/{1}";

        /// <summary>
        /// desc : 회의 참여
        /// method : patch
        /// param 
        ///     {0} : workspace seq
        ///     {1} : meeting seq
        /// </summary>
        public const string ATTEND_MEETING = SERVER_PATH +"/api/meeting/attend/{0}/{1}";

        /// <summary>
        /// desc : 회의 / 세미나 리스트 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : room name
        ///     {2} : meeting / seminar 구분 (seminar 가 true)
        ///     {3} : meeting title
        /// </summary>
        public const string SEARCH_MEETING = SERVER_PATH +"/api/meeting/search/{0}?nm={1}&seminar={2}&title={3}";

        /// <summary>
        /// desc : 회의 삭제 / 종료
        /// method : delete
        /// param
        ///     {0} : workspace seq
        ///     {1} : meeting seq
        /// </summary>
        public const string END_MEETING = SERVER_PATH +"/api/meeting/end/{0}/{1}";

        /// <summary>
        /// desc : 미팅 이력 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : 종료일시
        ///     {2} : 내 회의 여부 (true / false)
        ///     {3} : paging 
        ///     {4} : 시작일시
        ///     {5} : meeting / seminar 구분 (seminar 가 true)
        ///     {6} : size (현재 50 정도로 검색하고 있음)
        /// </summary>
        public const string MEETING_HISTORY = SERVER_PATH +"/api/meeting/history/{0}?edt={1}&mine={2}&page={3}&sdt={4}&seminar={5}&size={6}";

        /// <summary>
        /// desc : 종료된 회의를 포함한 특정 회의 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : meeting seq
        /// </summary>
        public const string SEARCH_MEETING_ALL = SERVER_PATH + "/api/meeting/dtl/{0}/{1}";

        /// <summary>
        /// desc : 파일 첨부 
        /// method : post
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string UPLOAD_MEETING_FILE = SERVER_PATH + "/api/meeting/files/{0}";

        /// <summary>
        /// desc : 첨부 파일 다운로드
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : meeting seq
        ///     {2} : file path
        /// </summary>
        public const string DOWNLOAD_MEETING_FILE = SERVER_PATH +"/api/meeting/down/{0}/{1}?filePath={2}";


        // ---------- ---------- ----------
        // 05. File API

        /// <summary>
        /// desc : 파일 리스트 관련 기능
        /// method : post (파일 업로드), get (조회)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : folder path
        /// </summary>
        public const string FILE_LIST = SERVER_PATH +"/api/file/{0}/{1}?folder={2}";

        /// <summary>
        /// desc : 파일 삭제
        /// method : delete
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file path
        /// </summary>
        public const string DELETE_FILE = SERVER_PATH +"/api/file/{0}/{1}?filePath={2}";  

        /// <summary>
        /// desc : 파일/폴더 복사 붙여넣기
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : source space seq
        ///     {2} : target space seq
        ///     {3} : source
        ///     {4} : target
        /// </summary>
        public const string COPY_AND_PASTE = SERVER_PATH +"/api/file/copy/{0}/{1}/{2}?source={3}&target={4}";  

        /// <summary>
        /// desc : 파일 다운로드
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file path
        /// </summary>
        public const string DOWNLOAD_FILE = SERVER_PATH +"/api/file/down/{0}/{1}?filePath={2}";

        /// <summary>
        /// desc : 파일 경로 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : member seq
        ///     {3} : file seq
        /// </summary>
        public const string GET_FULLPATH = SERVER_PATH +"/api/file/fullPath/{0}/{1}/{2}/{3}";

        /// <summary>
        /// desc : 파일함 키워드 검색
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : member seq
        ///     {3} : keyword
        ///     {4} : page
        ///     {5} : size
        /// </summary>
        public const string SEARCH_FILE = SERVER_PATH +"/api/file/keyword/{0}/{1}/{2}?keyword={3}&page={4}&size={5}";  

        /// <summary>
        /// desc : 새 폴더 생성
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : folder path
        /// </summary>
        public const string CREATE_FOLDER = SERVER_PATH +"/api/file/mkdir/{0}/{1}?folder={2}";

        /// <summary>
        /// desc : 파일/폴더명 변경
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file path
        ///     {3} : file path with new name
        /// </summary>
        /// <value></value>
        public const string RENAME_FILE = SERVER_PATH +"/api/file/rename/{0}/{1}?newFilePath={2}&sourceFilePath={3}";    

        ///  <summary>
        /// desc : 폴더 삭제
        /// method : delete
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : folder path
        /// </summary>
        public const string DELETE_FOLDER = SERVER_PATH +"/api/file/rm/{0}/{1}?folder={2}";


        // ---------- ---------- ----------
        // 05. File API v2

        /// <summary>
        /// desc : 파일 리스트 관련 기능
        /// method : post (파일 업로드), get (조회)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq (0 입력시 전사 파일, -1 입력시 내 파일)
        ///     {2} : folder seq (0 입력시 root folder)
        /// </summary>
        public const string FILE_LIST_V2 = SERVER_PATH +"/api/v2/file/{0}/{1}/{2}";

        /// <summary>
        /// desc : 파일/폴더 복사 붙여넣기 or 이동
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq (0 입력시 전사 파일, -1 입력시 내 파일)
        ///     {2} : source seq (복사 or 이동할 파일 or 폴더의 seq)
        ///     {3} : target seq (복사 or 이동시킬 폴더의 seq)
        ///     {4} : type (copy | move)
        /// </summary>
        public const string CONTROL_FILE_V2 = SERVER_PATH +"/api/v2/file/change/{0}/{1}/{2}/{3}?type={4}";  

        /// <summary>
        /// desc : 파일 다운로드
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file path
        /// </summary>
        public const string DOWNLOAD_FILE_V2 = SERVER_PATH +"/api/v2/file/down/{0}/{1}?filePath={2}";

        /// <summary>
        /// desc : 파일 경로 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file seq
        /// </summary>
        public const string GET_FULLPATH_v2 = SERVER_PATH +"/api/v2/file/fullPath/{0}/{1}/{2}";

        /// <summary>
        /// desc : 파일함 키워드 검색
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : member seq
        ///     {3} : keyword
        ///     {4} : page
        ///     {5} : size
        /// </summary>
        public const string SEARCH_FILE_V2 = SERVER_PATH +"/api/v2/file/keyword/{0}/{1}/{2}?keyword={3}&page={4}&size={5}";  

        /// <summary>
        /// desc : 새 폴더 생성
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq (0 입력시 전사 파일, -1 입력시 내 파일)
        ///     {2} : top folder seq (0 입력시 root folder)
        ///     {3} : req folder name
        /// </summary>
        public const string CREATE_FOLDER_V2 = SERVER_PATH +"/api/v2/file/mkdir/{0}/{1}/{2}?folder={3}";

        /// <summary>
        /// desc : 파일/폴더명 변경
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file seq
        ///     {3} : new name
        /// </summary>
        /// <value></value>
        public const string RENAME_FILE_V2 = SERVER_PATH +"/api/v2/file/rename/{0}/{1}/{2}?name={3}";

        ///  <summary>
        /// desc : 파일, 폴더 삭제
        /// method : delete
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : file / folder seq
        /// </summary>
        public const string DELETE_FILE_V2 = SERVER_PATH +"/api/v2/file/rm/{0}/{1}/{2}";


        // ---------- ---------- ----------
        // 06. Alarm API

        /// <summary>
        /// desc : 알림 리스트 조회
        /// method : get
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string ALARM_LIST = SERVER_PATH +"/api/alarm/{0}";

        /// <summary>
        /// desc : 특정 알림 삭제
        /// method : delete
        /// param
        ///     {0} : member seq
        ///     {1} : alarm seq
        /// </summary>
        public const string DELETE_ALARM = SERVER_PATH +"/api/alarm/{0}/{1}";

        /// <summary>
        /// desc : 모든 알림 삭제 
        /// method : delete
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string TRUNCATE_ALARM = SERVER_PATH +"/api/alarm/all/{0}";

        /// <summary>
        /// desc : 알림 카운터 초기화
        /// method : patch
        /// param 
        ///     {0} : member seq
        /// </summary>
        public const string RESET_ALARM_COUNT = SERVER_PATH +"/api/alarm/clearCnt/{0}";

        /// <summary>
        /// desc : 알림 갯수 정보 확인
        /// method : get
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string ALARM_COUNT = SERVER_PATH +"/api/alarm/cnt/{0}";

        /// <summary>
        /// desc : 알림 확인
        /// method : patch
        /// param
        ///     {0} : member seq
        ///     {1} : alarm seq
        /// </summary>
        public const string READ_ALARM = SERVER_PATH + "/api/alarm/readed/{0}/{1}";


        // ---------- ---------- ----------
        // 08. Chatting API

        /// <summary>
        /// desc : 채팅 목록을 출력하기 위한 link
        /// param 
        ///     {0} : member seq
        ///     {1} : language code (ko / en)
        /// </summary>
        public const string CHAT_LINK = PATH +"/chat_service/{0}?lan={1}";
        public const string GUEST_CHAT_LINK = PATH +"/chat_service/{0}/guest?lan={1}";

        /// <summary>
        /// desc : 대화방을 출력하기 위한 link
        /// param 
        ///     {0} : member seq
        ///     {1} : 대화 상대의 member seq
        ///     {2} : language code (ko / en)
        /// </summary>
        public const string CHATVIEW_LINK = PATH +"/chat_service/{0}?otherMemberSeq={1}&lan={2}"; 

        /// <summary>
        /// desc : 모바일에서 채팅 목록을 출력하기 위한 link
        /// param 
        ///     {0} : member seq
        ///     {1} : language code (ko / en)
        ///     {2} : token
        /// </summary>
        public const string CHAT_LINK_M = PATH +"/chat_service/{0}/mobile?lan={1}&token={2}";

        /// <summary>
        /// desc : 모바일에서 대화방을 출력하기 위한 link
        /// param 
        ///     {0} : member seq
        ///     {1} : 대화 상대의 member seq
        ///     {2} : language code (ko / en)
        ///     {3} : token
        /// </summary>
        public const string CHATVIEW_LINK_M = PATH +"/chat_service/{0}/mobile?otherMemberSeq={1}&lan={2}&token={3}"; 

        /// <summary>
        /// desc : 미확인 채팅 카운트 조회
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string UNREAD_CHAT_COUNT = SERVER_PATH +"/api/chatting/unReadCnt/{0}";


        // ---------- ---------- ----------
        // 10. Board API

        /// <summary>
        /// desc : 게시물 목록 관리 기능
        /// method : post (게시글 등록), get (조회)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string BOARD_LIST = SERVER_PATH +"/api/board/{0}/{1}";

        /// <summary>
        /// desc : 게시글 관리 기능
        /// method : put (게시글 수정), get (상세 조회), delete (삭제)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : board seq
        /// </summary>
        public const string BOARD_CONTENT = SERVER_PATH +"/api/board/{0}/{1}/{2}";

        /// <summary>
        /// desc : 게시글 파일 다운로드
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string DOWNLOAD_BOARD_FILE = SERVER_PATH +"/api/board/down/{0}/{1}";

        /// <summary>
        /// desc : 게시글 파일 업로드 with MultipartFormFileSection (section : files)
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        /// </summary>
        public const string UPLOAD_BOARD_FILE = SERVER_PATH +"/api/board/files/{0}/{1}";


        // ---------- ---------- ----------
        // 11. Comment API

        /// <summary>
        /// desc : 댓글 등록 및 조회
        /// method : post (댓글 등록), get (댓글 목록 조회)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : board seq
        /// </summary>
        public const string COMMENT_INFO = SERVER_PATH +"/api/comment/{0}/{1}/{2}";

        /// <summary>
        /// desc : 댓글 관리
        /// method : put (수정), delete (삭제)
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : comment seq
        /// </summary>
        public const string CONTROL_COMMENT = SERVER_PATH +"/api/comment/{0}/{1}/{2}";

        /// <summary>
        /// desc : 대댓글 등록
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : space seq
        ///     {2} : top comment seq
        /// </summary>
        public const string REG_REPLY = SERVER_PATH +"/api/comment/reply/{0}/{1}/{2}"; 


        // ---------- ---------- ----------
        // 12. Bookmakr API

        /// <summary>
        /// desc : 북마크 조회
        /// method : get
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string GET_BOOKMARKS = SERVER_PATH +"/api/bookmark/{0}";

        /// <summary>
        /// desc : 북마크 삭제
        /// method : delete
        /// param
        ///     {0} : member seq
        ///     {1} : bookmark seq
        /// </summary>
        public const string DELETE_BOOKMARK = SERVER_PATH +"/api/bookmark/{0}/{1}";

        /// <summary>
        /// desc : 북마크 등록
        /// method : post
        /// param
        ///     {0} : member seq
        ///     {1} : type ('board' or 'noti')
        ///     {2} : post seq
        /// </summary>
        public const string SET_BOOKMARK = SERVER_PATH +"/api/bookmark/{0}/{1}/{2}";

        /// <summary>
        /// desc : 모든 북마크 삭제
        /// method : delete
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string TRUNCATE_BOOKMARK = SERVER_PATH +"/api/bookmark/{0}/all";


        // ---------- ---------- ----------
        // 13. To-Do API

        /// <summary>
        /// desc : To-Do 등록
        /// method : post
        /// param
        ///     {0} : member seq
        ///     {1} : share type
        ///     {2} : reminder time
        /// </summary>
        public const string REGIST_TODO = SERVER_PATH +"/api/todo/{0}/{1}/{2}";

        /// <summary>
        /// desc : To-Do 수정
        /// method : put
        /// param
        ///     {0} : member seq
        ///     {1} : to-do seq
        ///     {2} : share type
        ///     {3} : reminder time
        /// </summary>
        public const string MODIFY_TODO = SERVER_PATH +"/api/todo/{0}/{1}/{2}/{3}";

        /// <summary>
        /// desc : To-Do 항목 관리
        /// method : put (완료 처리), delete (삭제)
        /// param
        ///     {0} : member seq
        ///     {1} : to-do seq
        /// </summary>
        public const string CONTROL_TODO = SERVER_PATH +"/api/todo/{0}/{1}";

        /// <summary>
        /// desc : 특정 대상의 To-Do 조회
        /// method : get
        /// param
        ///     {0} : member seq
        ///     {1} : target member seq
        ///     {2} : view option (일간, 주간, 월간)
        ///     {3} : 조회일자
        /// </summary>
        public const string TARGET_TODO_LIST = SERVER_PATH +"/api/todo/target/{0}/{1}/{2}/{3}";

        /// <summary>
        /// desc : 공유 옵션이 있는 To-Do 조회
        /// method : get
        /// param
        ///     {0} : my member seq
        ///     {1} : view option (일간, 주간, 월간)
        ///     {2} : filter type
        ///     {3} : 조회 일자
        /// </summary>
        public const string SHARE_TODO_LIST = SERVER_PATH +"/api/todo/{0}/{1}/{2}/{3}";


        // ---------- ---------- ----------
        // 14. OKR API

        /// <summary>
        /// desc : 공유 옵션이 있는 OKR 조회 
        /// method : get
        /// param
        ///     {0} : member seq
        ///     {1} : filter type (0 : 통합, 1 : 부서, 2 : 전사)
        ///     {2} : view type (일간, 주간, 월간)
        ///     {3} : 조회 일자
        /// </summary>
        public const string SHARE_OKR_LIST = SERVER_PATH +"/api/okr/{0}/{1}/{2}/{3}";

        /// <summary>
        /// desc : OKR 항목 관리
        /// method : put (수정), delete (삭제)
        /// param
        ///     {0} : member seq
        ///     {1} : OKR seq
        /// </summary>
        public const string CONTROL_OKR = SERVER_PATH +"/api/okr/{0}/{1}";

        /// <summary>
        /// desc : objective 목록 조회
        /// method : get
        /// param
        ///     {0} : member seq
        ///     {1} : share type (개인, 부서, 전사)
        /// </summary>
        public const string OBJECTIVE_LIST = SERVER_PATH +"/api/okr/{0}/{1}";

        /// <summary>
        /// desc : key result 등록
        /// method : post
        /// param
        ///     {0} : member seq
        ///     {1} : 상위 OKR seq
        /// </summary>
        public const string REGIST_KEY_RESULT = SERVER_PATH +"/api/okr/kr/{0}/{1}";

        /// <summary>
        /// desc : 내 OKR 조회
        /// method : get
        /// param
        ///     {0} : member seq
        ///     {1} : view type (일간, 주간, 월간)
        ///     {2} : 조회 일자
        /// </summary>
        public const string OKR_LIST = SERVER_PATH +"/api/okr/my/{0}/{1}/{2}";

        /// <summary>
        /// desc : objective 등록
        /// method : post
        /// param
        ///     {0} : member seq
        ///     {1} : filter type (1. 개인, 2. 부서, 3. 전사) 
        /// </summary>
        public const string REGIST_OBJECTIVE = SERVER_PATH +"/api/okr/o/{0}/{1}";


        // ---------- ---------- ----------
        // 15. Feedback API

        /// <summary>
        /// desc : feedback 등록
        /// method : post
        /// param
        ///     {0} : member seq
        ///     {1} : title code
        ///     {2} : rating
        /// </summary>
        public const string REGIST_FEEDBACK = SERVER_PATH +"/api/feedback/{0}/{1}/{2}"; 

        /// <summary>
        /// desc : feedback file 등록
        /// method : post
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string UPLOAD_FEEDBACK_FILE = SERVER_PATH +"/api/feedback/{0}/files";


        // ---------- ---------- ----------
        // 16. TalkArea API

        /// <summary>
        /// desc : talk area 생성
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : height
        ///     {2} : width
        ///     {3} : pos x
        ///     {4} : pos y
        /// </summary>
        public const string CREATE_TALK_AREA = SERVER_PATH +"/api/voice/{0}?h={1}&w={2}&x={3}&y={4}";

        /// <summary>
        /// desc : talk area 정보 확인
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : member seq
        /// </summary>
        public const string TALK_AREA_INFO = SERVER_PATH +"/api/voice/{0}/{1}";

        /// <summary>
        /// desc : talk area 참가
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : talk area seq
        ///     {2} : height
        ///     {3} : width
        ///     {4} : pos x
        ///     {5} : pos y
        /// </summary>
        public const string JOIN_TALK_AREA = SERVER_PATH +"/api/voice/attend/{0}/{1}?h={2}&w={3}&x={4}&y={5}";  

        /// <summary>
        /// desc : talk area 떠나기
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : talk area seq
        ///     {2} : height
        ///     {3} : width
        ///     {4} : pos x
        ///     {5} : pos y
        /// </summary>
        public const string LEAVE_TALK_AREA = SERVER_PATH +"/api/voice/leave/{0}/{1}?h={2}&w={3}&x={4}&y={5}";

        /// <summary>
        /// desc : talk area 상태 확인 
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : talk area seq
        /// </summary>
        public const string CHECK_TALK_AREA = SERVER_PATH +"/api/voice/get/{0}/{1}";  


        // ---------- ---------- ----------
        // 17. Attendance API

        /// <summary>
        /// desc : 특정 근태 기록 삭제
        /// method : delete
        /// param 
        ///     {0} : member seq
        ///     {1} : attendance seq
        /// </summary>
        public const string DELETE_ATTENDANCE = SERVER_PATH +"/api/attendance/{0}/{1}";

        /// <summary>
        /// desc : 근태 기록 조회
        /// method : get
        /// param
        ///     {0} : workspace seq
        ///     {1} : search range
        ///     {2} : space seq
        ///     {3} : target member seq
        ///     {4} : start date
        ///     {5} : end date
        /// </summary>
        public const string ATTENDANCE_LIST = SERVER_PATH +"/api/attendance/{0}/{1}/{2}/{3}/{4}/{5}";

        /// <summary>
        /// desc : 근태 기록 입력 
        /// method : post
        /// param
        ///     {0} : workspace seq
        ///     {1} : target member seq
        /// </summary>
        public const string CREATE_ATTENDANCE = SERVER_PATH +"/api/attendance/{0}/{1}";  

        /// <summary>
        /// desc : 근태 기록 수정
        /// method : patch
        /// param
        ///     {0} : workspace seq
        ///     {1} : target member seq
        ///     {2} : attendance seq
        /// </summary>
        public const string MODIFY_ATTENDANCE = SERVER_PATH +"/api/attendance/{0}/{1}/{2}";

        /// <summary>
        /// desc : 출근 체크
        /// method : post
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string ENTER_WORK = SERVER_PATH +"/api/attendance/goToWork/{0}";

        /// <summary>
        /// desc : 퇴근 체크
        /// method : patch
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string LEAVE_WORK = SERVER_PATH +"/api/attendance/leaveWork/{0}";


        // ---------- ---------- ----------
        // 18. XMPP API

        /// <summary>
        /// desc : xmpp 전달
        /// method : post
        /// param
        ///     {0} : content
        ///     {1} : target member seq
        ///     {2} : space seq -> 일반적으로 0 으로 전송
        ///     {3} : workspace seq
        /// </summary>
        public const string SEND_XMPP = SERVER_PATH +"/xmpp/forward?content={0}&memberSeq={1}&spaceSeq={2}&workspaceSeq={3}";  

    #endregion  // UserApi


    #region UserApi - for World

        // ---------- ---------- ----------
        // 21. World API

        /// <summary>
        /// desc : 월드 옵션 설정 / 조회
        /// method : post (정보 변경), get (조회)
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string WORLD_INFO = SERVER_PATH +"/api/world/{0}";

        /// <summary>
        /// desc : 월드 옵션 - 파일 업로드
        /// method : post
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string UPLOAD_WORLD_FILE = SERVER_PATH +"/api/world/file/{0}";


        // ---------- ---------- ----------
        // 22. Clas API

        /// <summary>
        /// desc : 모임방 생성
        /// method : post
        /// param
        ///     {0} : member seq
        /// </summary>
        public const string REGIST_CLAS = SERVER_PATH +"/api/clas/{0}";

        /// <summary>
        /// desc : 모임방 목록 조회
        /// method : get
        /// param
        ///     {0} : page no
        ///     {1} : page size
        /// </summary>
        public const string CLAS_LIST = SERVER_PATH +"/api/clas?page={0}&size={1}";

        /// <summary>
        /// desc : 모임방 목록 조회 (with request)
        /// method : get
        /// </summary>
        public const string CLAS_LIST_REQUEST = SERVER_PATH +"/api/clas";

        /// <summary>
        /// desc : 모임방 관리
        /// method : put (옵션 수정), get (옵션 조회)
        /// param
        ///     {0} : workspace seq
        /// </summary>
        public const string CLAS_INFO = SERVER_PATH +"/api/clas/{0}";

    #endregion  // UserApi - for World
    }
}