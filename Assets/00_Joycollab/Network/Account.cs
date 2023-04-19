/// <summary>
/// Network 통신 - 토큰 & 계정 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 04. 03
/// @version        : 1.3
/// @update
///     v1.0 (2023. 02. 20) : 사용하던 클래스 정리 (ResToken, ResCheckToken)
///     v1.1 (2023. 03. 20) : 사용하던 클래스 정리 (ResCheckId)
///     v1.2 (2023. 03. 31) : 사용하던 클래스 정리 (ReqResetPassword)
///     v1.3 (2023. 04. 03) : 사용하던 클래스 정리 (ResGuest)
/// </summary>

using System;

namespace Joycollab.v2
{
    [Serializable]
    public class ResToken
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public int expires_in;
        public string scope;
    }
    
    [Serializable]
    public class ResCheckToken
    {
        public string[] aud;
        public string account_role;
        public string user_name;
        public string[] scope;
        public bool active;
        public long exp;
        public string[] authorities;
        public string client_id;
    }

    [Serializable] 
    public class ResCheckId 
    {
        public string useYn;
        public string id;
    }

    [Serializable] 
    public class ResGuest 
    {
        public string id;
        public string pw;
        public int seq;
    }
}