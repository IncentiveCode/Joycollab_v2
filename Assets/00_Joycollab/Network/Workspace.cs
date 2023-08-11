/// <summary>
/// Network 통신 - 워크스페이스 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 08. 11
/// @version        : 0.5
/// @update
///     v0.1 (2023. 02. 22) : v1 에서 사용하던 클래스 정리 및 통합 진행 (SimpleWorkspace, ResWorkspaceList, ResWorkspaceInfo)
///     v0.2 (2023. 03. 22) : v1 에서 사용하던 클래스 정리 및 통합 진행 (LobbyInfo)
///     v0.3 (2023. 06. 15) : array 를 list 로 변경 
///     v0.4 (2023. 06. 19) : WorkspaceStatus 클래스 추가
///     v0.5 (2023. 08. 11) : v1 에서 사용하던 클래스 정리 및 통합 진행 (ReqWorkspaceSeq)
/// </summary>

using System;
using System.Collections.Generic;

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
        public List<ResWorkspaceInfo> list;
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
    public class ReqWorkspaceSeq 
    {
        public int workspaceSeq;

        public ReqWorkspaceSeq(int seq) 
        {
            workspaceSeq = seq;
        }
    }

    [Serializable]
    public class LobbyInfo
    {
        public bool guest;
        public List<int> lobbyMembers;
        public string monitType;
        public string monitFile;
        public string monitUrl;
        public string monitImgUrl;
        public string noticeImgUrl;
        public bool jcShow;
        public string jcMsg;
        public bool showSeminarBtn;
    }
    
    [Serializable]
    public class WorkspaceStatus 
    {
        public string useYn;
    }
}