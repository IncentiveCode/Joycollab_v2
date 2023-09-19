/// <summary>
/// Network 통신 - 사용자, 워크스페이스 멤버 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 09. 19
/// @version        : 0.8
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
        public string uiType;
        public string fcmToken;
        public string plan;
        public string sdt;
        public string businessNum;
        public string compName;
        public string business;
        public string tel;
        public string ceoNm;
        public string mainBusiness;
        public string homepage;
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

#endregion  // infinite scroll data
}