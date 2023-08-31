/// <summary>
/// Network 통신 - 모임방 관련 요청과 응답
/// @author         : HJ Lee
/// @last update    : 2023. 08. 31 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 31) : 최초 생성
/// </summary>
 
using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    [Serializable]
    public class ClasList
    {
        public bool first;
        public bool last;
        public bool empty;
        public bool hasNext;
        public int size;
        public string sort;
        public List<ClasInfo> content;
        public int number;
        public int totalElements;
        public int totalPages;
        public int numberOfElements;
    }

    [Serializable]
    public class ClasInfo
    {
        public string useYn;
        public int seq;
        public string workspaceType;
        public string nm;
        public string logo;
        public string headerLogo;
        public string edtm;
        public string regularDate;
        public string businessNum;
        public string business;
        public string tel;
        public string ceoNm;
        public string mainBusiness;
        public string homepage;
        public string openDate;
        public string addr;
        public string addrDtl;
        public ClasDetail clas;
    }

    [Serializable]
    public class ClasDetail
    {
        public string useYn;
        public int seq;
        public TpsInfo themes; 
        public string bigo;
        public string openType;
        // "inviteClas": [],
        public List<SimpleMemberInfo> blockMembers;
    }


    public class ClasData : InfiniteScrollData 
    {
        public ClasInfo info;

        public ClasData() 
        {
            info = new ClasInfo();
        }
    }
}