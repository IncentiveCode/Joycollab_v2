/// <summary>
/// Network 통신 - 워크스페이스 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 02. 22
/// @version        : 1.0
/// @update
///     [2023. 02. 22] v1.0 - Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
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
    }
}