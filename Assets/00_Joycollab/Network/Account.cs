/// <summary>
/// Network 통신 - 토큰 & 계정 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 15
/// @version        : 0.5
/// @update
///     v0.1 (2023. 02. 20) : 사용하던 클래스 정리 (ResToken, ResCheckToken)
///     v0.2 (2023. 03. 20) : 사용하던 클래스 정리 (ResCheckId)
///     v0.3 (2023. 03. 31) : 사용하던 클래스 정리 (ReqResetPassword)
///     v0.4 (2023. 04. 03) : 사용하던 클래스 정리 (ResGuest)
///     v0.5 (2023. 06. 15) : array 를 list 로 변경
/// </summary>

using System;
using System.Collections.Generic;

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

        public ResToken() 
        {
            access_token = token_type = refresh_token = scope = string.Empty;
            expires_in = 0;
        }
    }
    
    [Serializable]
    public class ResCheckToken
    {
        public List<string> aud;
        public string account_role;
        public string user_name;
        public List<string> scope;
        public bool active;
        public long exp;
        public List<string> authorities;
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