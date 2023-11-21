/// <summary>
/// Network 통신 - 모임방 관련 요청과 응답
/// @author         : HJ Lee
/// @last update    : 2023. 11. 21 
/// @version        : 0.5
/// @update
///     v0.1 (2023. 08. 31) : 최초 생성
///     v0.2 (2023. 09. 14) : InfiniteScrollData class 수정. Request class 추가.
///     v0.3 (2023. 09. 22) : 모임방 생성용 Request class 추가. (RequestCreateClas, RequestCreateClasDetail)
///     v0.4 (2023. 10. 04) : 모임방 카테고리 관련 InfiniteScrollData class 추가.
///     v0.5 (2023. 11. 21) : 모임방 생성, 참여 등을 위한 match message struct 추가.
/// </summary>
 
using System;
using System.Collections.Generic;
using Gpm.Ui;
using Mirror;

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

        // 추가가 필요할 수 있는 항목
        public Guid roomId;
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


#region Network message 

    /// <summary>
    /// Match operation to execute on the server
    /// </summary>
    public enum ServerRoomOperation : byte
    {
        None,
        Create,
        Cancel,
        Start,
        Join,
        Leave,
        Ready
    }

    /// <summary>
    /// Match operation to execute on the client
    /// </summary>
    public enum ClientRoomOperation : byte
    {
        None,
        List,
        Created,
        Cancelled,
        Joined,
        Departed,
        UpdateRoom,
        Started
    }

    /// Room message to be sent to the server
    /// </summary>
    public struct ServerRoomMessage : NetworkMessage
    {
        public ServerRoomOperation serverRoomOperation;
        public Guid roomId;
    }

    /// <summary>
    /// Room message to be sent to the client
    /// </summary>
    public struct ClientRoomMessage : NetworkMessage
    {
        public ClientRoomOperation clientRoomOperation;
        public Guid roomId;
        public ClasInfo[] roomInfos;
        public WorldAvatarInfo[] playerInfos;
    }

#endregion  // Network message 
}