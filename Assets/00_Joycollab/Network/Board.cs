/// <summary>
/// Network 통신 - Board 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 09
/// @version        : 0.3
/// @update
///     v0.1 (2023. 06. 15) : v1 에서 사용하던 클래스 정리 및 통합, infinite scroll data 추가.
///     v0.2 (2023. 07. 07) : v1 에서 사용하던 클래스 정리 및 추가 (Notice, Bookmark 관련 정보 추가)
///     v0.3 (2023. 11. 09) : Bookmark 분리.
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    [Serializable]
    public class ReqBoardInfo
    {
        public List<string> attachedFile;
        public string content;
        public Seq createMember;
        public Seq modifyMember;
        public string title;
    }

    [Serializable]
    public class ResBoardList 
    {
        public bool first;
        public bool last;
        public bool empty;
        public bool hasNext;
        public int size;
        public List<BoardContent> content;
        public int number;
        public int numberOfElements;
        public int totalPages;
        public int totalElements;
    }

    [Serializable]
    public class BoardContent 
    {
        public string useYn;
        public int seq;
        public string title;
        public string content;
        public int readCount;
        public SimpleMemberInfo createMember;
        public SimpleMemberInfo modifyMember;
        public string createdDate;
        public string modifiedDate;
        public List<string> attachedFile;
        public List<string> originalFileName;
	    public SpaceSeq space;
    }


    [Serializable]
    public class RequestForBoard 
    {
        public int workspaceSeq;
        public int spaceSeq;
        public string keyword;
        public int pageNo;
        public int pageSize;
        public bool refresh;

        public RequestForBoard() 
        {
            workspaceSeq = spaceSeq = pageNo = pageSize = 0;
            keyword = string.Empty;
            refresh = false;
        }

        public string url 
        {
            get {
                string url = string.Format(URL.BOARD_LIST, workspaceSeq, spaceSeq);
                url += "?";

				if (! string.IsNullOrEmpty(keyword)) url += "keyword="+ keyword +"&";
                url += "page="+ pageNo +"&";
                url += "size="+ pageSize;

                return url;
            }
        }
    }


    public class BoardData : InfiniteScrollData 
    {
        public BoardContent info;
        public bool loadMore;

        public BoardData(BoardContent c) 
        {
            info = c;
            loadMore = false;
        }

        public BoardData() 
        {
            info = null;
            loadMore = true;
        }
    }


#region Notice



#endregion  // Notice


#region System notice



#endregion  // System notice
}