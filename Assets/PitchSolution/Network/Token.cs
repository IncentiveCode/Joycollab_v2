/// <summary>
/// Network 통신 - 토큰 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 02. 20
/// @version        : 1.0
/// @update
///     [2023. 02. 20] v1.0 - Joycollab 에서 사용하던 클래스 정리 및 통합
/// </summary>

using System;

namespace PitchSolution
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
}