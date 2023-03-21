/// <summary>
/// Network 통신 - 사용자, 워크스페이스 멤버 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 02. 23
/// @version        : 1.0
/// @update
///     [2023. 02. 23] v1.0 - Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
/// </summary>

using System;

namespace Joycollab.v2
{
    [Serializable]
    public class WorldAvatarInfo 
    {
        public int seq;
        public string nickNm;
        public string photo;

        public WorldAvatarInfo() 
        {
            seq = -1;
            nickNm = photo = string.Empty;
        }

        public WorldAvatarInfo(int seq, string nickNm, string photo)
        {
            this.seq = seq;
            this.nickNm = nickNm;
            this.photo = photo;
        }
    }

    [Serializable]
    public class ResUserInfo 
    {
        public string useYn;
        public string id;
        public string nm;
        public string tel;
        public string birthday;
        public string gender;
        public string photo;
        public string addr;
        public string addrDtl;
        public float lat;
        public float lng;
        public string googleId;
    }

    [Serializable]
    public class ResMemberInfo 
    {
        public string useYn;
        public int seq;
        public ResUserInfo user;
        public string memberType;
        public InfoSpace space;
        public string nickNm;
        public string photo;
        public string jobGrade;
        public string description;
        public string addr;
        public string addrDtl;
        public float lat;
        public float lng;
        public int position;
        public float x;
        public float y;
        public InfoSpace cspace;
        public float cx;
        public float cy;
        public MemberAuthInfo[] memberAuths;
        public SettingInfo alarmOpt;
        public string backgroundImg;
        public string timeZone;
        public string dateFormatStr;
        public string hourFormatStr;
        public int weekStart;
        public LangSettingInfo lan;
        public TpsInfo status;
        public TpsInfo emotion;
        public string xmppPw;
        public float camX;
        public float camY;
        public float camSize;
        public bool lobby;
        public string zoomId;
        public string uiType;
    }
}