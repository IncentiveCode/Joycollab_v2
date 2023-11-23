/// <summary>
/// Network 통신 - 사용자, 워크스페이스 멤버 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 15
/// @version        : 0.15
/// @update
///     v0.1 (2023. 02. 23) : Joycollab 에서 사용하던 클래스 정리 및 통합.
///     v0.2 (2023. 06. 14) : MemberSeq 클래스 추가.
///     v0.3 (2023. 06. 15) : array 를 list 로 변경, SimpleMemberList, SimpleMemberInfo, SimpleUser 클래스 추가.
///     v0.4 (2023. 06. 19) : MemberName 클래스 추가.
///     v0.5 (2023. 06. 27) : 클래스 추가. (WorkspaceMemberInfo, WorkspaceMemberList)
///                           클래스 수정. (SimpleUser 에 tel 추가)
///     v0.6 (2023. 08. 31) : 클래스 추가. (ReqSignUpInfo)
///     v0.7 (2023. 09. 15) : ResUserInfo 에 zoom id, zoom e-mail 필드 추가.
///     v0.8 (2023. 09. 19) : ResMemberInfo 에 추가된 필드 추가 (fcmToken, plan, sdt, businessNum, compName, business, tel, ceoNm, mainBusiness, homepage)
///     v0.9 (2023. 09. 21) : Request 용 class 추가 (ReqMemberInfo, ReqMemberCompanyInfo, ReqMemberEnvironmentInfo, ReqMemberAlarmInfo and...)
///     v0.10 (2023. 10. 06) : Infinite scroll data 추가. (WorldAvatarData)
///     v0.11 (2023. 10. 26) : 전화를 걸기 위한 ReqCallInfo class 추가.
///     v0.12 (2023. 11. 02) : WorkspaceMemberInfo 에 회사 정보 추가. (API 업데이트)
///     v0.13 (2023. 11. 06) : WorldAvatarInfo 에 상태 관련 코드 추가.
///     v0.14 (2023. 11. 09) : ResMemberInfo 에 몇 가지 항목 추가. (hiddenTel, tag 및 활동지수 등)
///     v0.15 (2023. 11. 15) : ReqMemberInfo 와 WorkspaceMemberInfo 에 몇 가지 항목 추가 (hiddenTel, tag 및 활동 지수 관련)
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    [Serializable]
    public class WorldAvatarInfo 
    {
        public int seq;
        public string nickNm;
        public string photo;
        public string memberType;
        public string stateId;
        public Guid roomId;

        public WorldAvatarInfo() 
        {
            seq = -1;
            nickNm = photo = memberType = stateId = string.Empty;
            roomId = Guid.Empty;
        }

        public WorldAvatarInfo(int seq, string nickNm, string photo, string memberType, string stateId)
        {
            this.seq = seq;
            this.nickNm = nickNm;
            this.photo = photo;
            this.memberType = memberType;
            this.stateId = stateId;
            roomId = Guid.Empty;
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
        public string zoomId;
        public string zoomEmail;
    }

    [Serializable]
    public class ReqSignUpInfo 
    {
        public string birthday;
        public string gender;
        public string nm;
        public string photo;
        public string tel;
        public string address1;
        public string address2;
        public float lat;
        public float lng;

        public ReqSignUpInfo() 
        {
            birthday = gender = nm = photo = tel = address1 = address2 = string.Empty;
            lat = lng = 0f;
        }

        // for office
        public ReqSignUpInfo(
            string photo, string birthday, string gender, string nm, string tel, string address1, string address2,
            float lat, float lng
        )
        {
            this.birthday = birthday;
            this.gender = gender;
            this.nm = nm;
            this.photo = photo;
            this.tel = tel;
            this.address1 = address1;
            this.address2 = address2;
            this.lat = lat;
            this.lng = lng;
        }

        // for world
        public ReqSignUpInfo(
            string photo, string nm, string tel, string address1, string address2,
            float lat, float lng
        )
        {
            this.birthday = string.Empty;
            this.gender = string.Empty;
            this.nm = nm;
            this.photo = photo;
            this.tel = tel;
            this.address1 = address1;
            this.address2 = address2;
            this.lat = lat;
            this.lng = lng;
        }
    }

    [Serializable]
    public class ResMemberInfo 
    {
        public string useYn;
        public int seq;
        public ResUserInfo user;
        public string memberType;
        public SpaceInfo space;
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
        public SpaceInfo cspace;
        public float cx;
        public float cy;
        public List<MemberAuthInfo> memberAuths;
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
        public int fontSize;
        public string uiType;
        public string fcmToken;
        public string plan;
        public string sdt;
        public string businessNum;
        public string compName;
        public string business;
        public string tel;
        public bool hiddenTel;
        public string ceoNm;
        public string mainBusiness;
        public string homepage;
        public string tag;
        public int boardCnt;
        public int commentCnt;
        public int loginCnt;
        public int score;
    }

    [Serializable]
    public class ReqMemberInfo 
    {
        public string addr;
        public string addrDtl;
        public string birthday;
        public string description;
        public string gender;
        public string jobGrade;
        public float lat;
        public float lng;
        public string nickNm;
        public string photo;
        public string tel;
        public bool hiddenTel;
        public string tag;

        public ReqMemberInfo(ResMemberInfo info) 
        {
            addr = info.addr;
            addrDtl = info.addrDtl;
            birthday = info.user.birthday;
            description = info.description;
            gender = info.user.gender;
            jobGrade = info.jobGrade;
            nickNm = info.nickNm;
            photo = info.photo;
            tel = info.user.tel;
            hiddenTel = info.hiddenTel;
            tag = info.tag;

            lat = info.lat;
            lng = info.lng;
        }
    }

    public class ReqMemberCompanyInfo 
    {
        public string business;
        public string businessNum;
        public string ceoNm;
        public string compName;
        public string homepage;
        public string mainBusiness;
        public string tel;

        public ReqMemberCompanyInfo(ResMemberInfo info) 
        {
            business = info.business;
            businessNum = info.businessNum;
            ceoNm = info.ceoNm; 
            compName = info.compName;
            homepage = info.homepage; 
            mainBusiness = info.mainBusiness;
            tel = info.tel;
        }
    }

    public class ReqMemberEnvironmentInfo 
    {
        public string dateFormatStr;
        public int fontSize;
        public string hourFormatStr;
        public string lanId;
        public string timeZone;
        public int weekStart;

        public ReqMemberEnvironmentInfo(ResMemberInfo info)
        {
            dateFormatStr = info.dateFormatStr;
            fontSize = info.fontSize;
            hourFormatStr = info.hourFormatStr;
            lanId = info.lan.id;
            timeZone = info.timeZone;
            weekStart = info.weekStart;
        }
    }

    public class ReqMemberAlarmInfo 
    {
        public List<alarmOptItemInfo> alarmOptItems;
        public List<alarmOptItemInfo> alarmOptSounds;
        public bool bottomPopupNotice;

        public ReqMemberAlarmInfo() 
        {
            alarmOptItems = new List<alarmOptItemInfo>();
            alarmOptItems.Clear();

            alarmOptSounds = new List<alarmOptItemInfo>();
            alarmOptSounds.Clear();

            bottomPopupNotice = false;
        }
    }

    [Serializable]
    public class MemberSeq 
    {
        public string useYn;
        public int seq;
        public string nickNm;
        public Seq space;
    }

    [Serializable] 
    public class MemberName 
    {
        public string useYn;
        public string nickNm;
    }

    [Serializable]
    public class SimpleMemberList 
    {
        public List<SimpleMemberInfo> list;
    }

    [Serializable] 
    public class SimpleMemberInfo 
    {
        public string useYn;
        public SimpleUser user;
        public int seq;
        public string memberType;
        public string nickNm;
        public string photo;
        public string jobGrade;
        public string description; 
    }

    [Serializable]
    public class SimpleUser 
    {
        public string tel;
        public string id;
    }

    [Serializable] 
    public class WorkspaceMemberList 
    {
        public List<WorkspaceMemberInfo> list;
    }

    [Serializable] 
    public class WorkspaceMemberInfo 
    {
        public string useYn;
        public int seq;
        public SimpleUser user;
        public string memberType;
        public Seq space;
        public string nickNm;
        public string photo;
        public string jobGrade = "";
        public string description;
        public int position;
        public float x;
        public float y;
        public Seq cspace;
        public float cx;
        public float cy;
        public string setMemberYn;
        public TpsInfo status;
        public TpsInfo emotion;
        public string businessNum;
        public string compName;
        public string business;
        public string tel;
        public bool hiddenTel;
        public string ceoNm;
        public string mainBusiness;
        public string homepage;
        public string tag;
        public int boardCnt;
        public int commentCnt;
        public int loginCnt;
        public int score;
    }

    [Serializable] 
    public class ReqCallInfo
    {
        public List<Seq> members;

        public ReqCallInfo() 
        {
            members = new List<Seq>();
            members.Clear();
        }
    }


#region infinite scroll data

	public class ContactData : InfiniteScrollData 
	{
		public WorkspaceMemberInfo info;

		public ContactData() 
		{
			info = new WorkspaceMemberInfo();
		}

        public ContactData(WorkspaceMemberInfo data) 
        {
            info = data;
        }
	}

    public class WorldAvatarData : InfiniteScrollData 
    {
        public WorldAvatarInfo info;

        public WorldAvatarData() 
        {
            info = new WorldAvatarInfo();
        }

        public WorldAvatarData(WorldAvatarInfo data) 
        {
            info = data;
        }
    }

#endregion  // infinite scroll data
}