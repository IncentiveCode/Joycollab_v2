/// <summary>
/// Network 통신 - 권한 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 15
/// @version        : 0.2
/// @update
///     v0.1 (2023. 02. 23) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
///     v0.2 (2023. 06. 15) : array 를 list 로 변경
/// </summary>

using System;
using System.Collections.Generic;

namespace Joycollab.v2
{
    [Serializable]
    public class MemberAuthInfo 
    {
        public string useYn;
        public int seq;
        public AuthSpaceInfo space;
        public List<MemberAuthMngInfo> authMngs;
    }

    [Serializable]
    public class MemberAuthMngInfo 
    {
        public string useYn;
        public int seq;
        public string nm;
    }

     [Serializable]
    public class AuthSpaceInfo
    {
        public string useYn;
        public int seq;
        public string nm;
        public AuthSpaceMng spaceMng;

        public AuthSpaceInfo() 
        {
            useYn = "Y";
            seq = 0;
            nm = "";
            spaceMng = new AuthSpaceMng();
        }
    } 

    [Serializable]
    public class AuthSpaceMng
    {
        public string useYn;
        public int seq;
        public int num;

        public AuthSpaceMng() 
        {
            useYn = "Y";
            seq = 0;
            num = -1;
        }
    }
}