/// <summary>
/// Network 통신 - 모임방 관련 요청과 응답
/// @author         : HJ Lee
/// @last update    : 2023. 09. 22 
/// @version        : 0.3
/// @update
///     v0.1 (2023. 08. 31) : 최초 생성
///     v0.2 (2023. 09. 14) : InfiniteScrollData class 수정. Request class 추가.
///     v0.3 (2023. 09. 22) : 모임방 생성용 Request class 추가. (RequestCreateClas, RequestCreateClasDetail)
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

    [Serializable]
    public class RequestForClas 
    {
        public string keyword; 
        public int pageNo;
        public int pageSize;
        public bool refresh;

        public RequestForClas() 
        {
            keyword = string.Empty;
            pageNo = pageSize = 0;
            refresh = false;
        }

        public string url 
        {
            get {
        	    string url = URL.CLAS_LIST_REQUEST;
                url += "?";

                if (! string.IsNullOrEmpty(keyword)) url += "keyword="+ keyword +"&";
                url += "page="+ pageNo +"&";
                url += "size="+ pageSize;

                return url;
            }
        }
    }

    [Serializable] 
    public class RequestCreateClas
    {
        public string nm;
        public string logo;
        public RequestCreateClasDetail clas;

        public RequestCreateClas() 
        {
            nm = logo = string.Empty;
            clas = new RequestCreateClasDetail();
        }
    }

    [Serializable]
    public class RequestCreateClasDetail 
    {
        public Cd themes; 
        public string bigo;
        public string openType;

        public RequestCreateClasDetail() 
        {
            themes.cd = 0;
            bigo = openType = string.Empty;
        }
    }


    public class ClasData : InfiniteScrollData 
    {
        public ClasInfo info;
        public bool loadMore;

        public ClasData(ClasInfo c) 
        {
            info = c;
            loadMore = false;
        }

        public ClasData() 
        {
            info = null;
            loadMore = true;
        }
    }
}