/// <summary>
/// Network 통신 - Board 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 15
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 24) : Joycollab 에서 사용하던 클래스 정리 및 통합, infinite scroll data 추가.
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
	    public string space;
    }


    public class BoardData : InfiniteScrollData 
    {
        public BoardContent info;
    }
}