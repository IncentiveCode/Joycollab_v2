/// <summary>
/// Network 통신 - 워크스페이스 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2024. 01. 03
/// @version        : 0.7
/// @update
///     v0.1 (2023. 02. 22) : v1 에서 사용하던 클래스 정리 및 통합 진행 (SimpleWorkspace, ResWorkspaceList, WorkspaceInfo)
///     v0.2 (2023. 03. 22) : v1 에서 사용하던 클래스 정리 및 통합 진행 (LobbyInfo)
///     v0.3 (2023. 06. 15) : array 를 list 로 변경 
///     v0.4 (2023. 06. 19) : WorkspaceStatus 클래스 추가
///     v0.5 (2023. 08. 11) : v1 에서 사용하던 클래스 정리 및 통합 진행 (ReqWorkspaceSeq)
///     v0.6 (2023. 12. 14) : 그동안 추가된 항목들 추가 (worldOpt, clasOpt 등)
///     v0.7 (2024. 01. 03) : WorldOpt -> WorldOption 에 일부 값 추가. WorldUsage 새로 추가.
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
        public string headerLogo;
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
        public bool attendance;
        public int attendanceMngType;
        public string attm;
        public string lvtm;
        public string rtst;
        public string rtet;
        public string regularPay;
        public string regularDate;
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
        public string description;
        public string monitType;
        public string monitFile;
        public string monitUrl;
        public string monitImgUrl;
        public string noticeImgUrl;
        public bool jcShow;
        public string jcMsg;
        public bool showSeminarBtn;
        public string webHockSeminar;
        public bool isEvent;
        public WorldUsage world; 
        public ClasOpt clas;
    }

    [Serializable]
    public class WorldOption 
    {
        public string useYn;
        public int seq;
        public string miniMap;
        public string infoDesk;
        public string board;
        public string guestBoot;
        public string billboardL1;
        public string billboardL1Url;
        public string billboardL2;
        public string billboardL2Url;
        public string billboard;
        public string billboardUrl;
        public string billboardR1;
        public string billboardR1Url;
        public string billboardR2;
        public string billboardR2Url;
        public string monitL;
        public string monitLUrl;
        public string monitR;
        public string monitRUrl;
        public string youtube;
        public string youtubeUrl;
        public string instagram;
        public string instagramUrl;
        public string blog;
        public string blogUrl;
        public string homep;
        public string homepUrl;
        public bool seminarMascotIsUse;
        public string seminarMascot;
        public string seminarList;
        public string experienceOffice;
    }

    [Serializable]
    public class WorldUsage 
    {
        public string useYn;
        public int seq;
    }

    [Serializable]
    public class ClasOpt 
    {
        public string useYn;
        public int seq;
        public TpsInfo themes;
        public string openType;
    }


    [Serializable]
    public class ResWorkspaceList 
    {
        public List<WorkspaceInfo> list;
    }


    [Serializable]
    public class WorkspaceInfo
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