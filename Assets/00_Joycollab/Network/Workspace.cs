/// <summary>
/// Network 통신 - 워크스페이스 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 22
/// @version        : 0.2
/// @update
///     v0.1 (2023. 02. 22) : Joycollab 에서 사용하던 클래스 정리 및 통합 진행 (SimpleWorkspace, ResWorkspaceList, ResWorkspaceInfo)
///     v0.2 (2023. 03. 22) : Joycollab 에서 사용하던 클래스 정리 및 통합 진행 (LobbyInfo)
/// </summary>

using System;

namespace Joycollab.v2
{
    [Serializable]
    public class SimpleWorkspace
    {
        public string useYn;
        public int seq;
        public string workspaceType;
        public string nm;
        public string logo;
        public string domain;
        public int limitNum;
        public int meetingTimeMax;
        public int meetingTimeSum;
        public int fileSizeMax;
        public int fileSizeSum;
        public string sdtm;
        public string edtm;
        public string plan;
        public bool nextFreePlan;
        public string backgroundImg;
        public bool lockBackgroundImg;
        public bool guest;
        public string monitUrl;
        public bool attendance;
        public string attm;
        public string lvtm;
        public string rtst;
        public string rtet;
        public int period;
        public int nextPeriod;  // 무료, free : 0 / other : 1 or 12
        public string businessNum;
        public string business;
        public string tel;
        public string ceoNm;
        public string mainBusiness;
        public string homepage;
        public string openDate;
        public string addr;
        public string addrDtl;
    }


    [Serializable]
    public class ResWorkspaceList 
    {
        public ResWorkspaceInfo[] list;
    }


    [Serializable]
    public class ResWorkspaceInfo
    {
        public string useYn;
        public int seq;
        public string memberType;
        public SimpleWorkspace workspace;
        public string nickNm;
        public string photo;
        public string jobGrade;
        public string uiType;
    }


    [Serializable]
    public class LobbyInfo
    {
        public bool guest;
        public int[] lobbyMembers;
        public string monitType;
        public string monitFile;
        public string monitUrl;
        public string monitImgUrl;
        public string noticeImgUrl;
        public bool jcShow;
        public string jcMsg;
        public bool showSeminarBtn;
    }
}