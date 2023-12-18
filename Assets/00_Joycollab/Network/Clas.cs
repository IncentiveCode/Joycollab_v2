/// <summary>
/// Network 통신 - 모임방 관련 요청과 응답
/// @author         : HJ Lee
/// @last update    : 2023. 12. 15 
/// @version        : 0.7
/// @update
///     v0.1 (2023. 08. 31) : 최초 생성
///     v0.2 (2023. 09. 14) : InfiniteScrollData class 수정. Request class 추가.
///     v0.3 (2023. 09. 22) : 모임방 생성용 Request class 추가. (RequestCreateClas, RequestCreateClasDetail)
///     v0.4 (2023. 10. 04) : 모임방 카테고리 관련 InfiniteScrollData class 추가.
///     v0.5 (2023. 11. 21) : 모임방 생성, 참여 등을 위한 match message struct 추가. -> WorldMessage.cs 로 이동
///     v0.6 (2023. 12. 14) : 모임방 생성/수정에 사용하는 RequestCreateClasDetail class 에 category 추가.
///     v0.7 (2023. 12. 15) : 모임방 전체 목록 받아올 수 있도록 SimpelClasInfo 와 SimpleClasList, SimpleClasDetail 추가.
/// </summary>
 
using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    /**
     *  client 에서 필요한 모임방 목록 조회 
     */

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


    /**
     *  mirror server 에서 필요한 모임방 전체 목록 조회 
     */

    [Serializable]
    public class SimpleClasList
    {
        public List<SimpleClasInfo> list;
    }

    [Serializable]
    public class SimpleClasInfo
    {
        public int seq;
        public string workspaceType;
        public string nm;
        public SimpleClasDetail clas;
    }

    [Serializable]
    public class SimpleClasDetail
    {
        public int seq;
        public TpsInfo themes; 
        public string openType;
    }


    /**
     *  Server 에 필요한 정보를 요청하기 위해 전달하는 request class
     */

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
    public class RequestClas
    {
        public RequestClasDetail clas;
        public string nm;
        public string logo;

        public RequestClas() 
        {
            clas = new RequestClasDetail();
            nm = logo = string.Empty;
        }
    }

    [Serializable]
    public class RequestClasDetail 
    {
        public Cd themes; 
        public Cd category;
        public string bigo;
        public string openType;

        public RequestClasDetail() 
        {
            themes = new Cd(0);
            category = new Cd(0);
            bigo = openType = string.Empty;
        }
    }


    /**
     *  infinite scroll 을 출력하기 위한 data class 
     */

    public class ClasData : InfiniteScrollData 
    {
        public ClasInfo info;
        public bool loadMore;


        public ClasData() 
        {
            info = null;
            loadMore = true;
        }

        public ClasData(ClasInfo c) 
        {
            info = c;
            loadMore = false;
        }
    }


    public class RoomCategoryData : InfiniteScrollData
    {
        public TpsInfo info;


        public RoomCategoryData() 
        {
            this.info = null;
        }

        public RoomCategoryData(TpsInfo info) 
        {
            this.info = info;
        }
    }
}