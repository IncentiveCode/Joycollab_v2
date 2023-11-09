/// <summary>
/// Network 통신 - Bookmark 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 09
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 09) : Bookmark 에 seminar 와 member 정보 추가. 
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    [Serializable]
    public class ResBookmarkList 
    {
        public List<ResBookmarkInfo> list;
    }

    [Serializable]
    public class ResBookmarkInfo 
    {
        public string useYn;
        public int seq;
        public BookmarkContent board;
        public BookmarkContent noti;
        public BookmarkMember member;
        public BookmarkMeeting meeting;
        public string regDtm;
    }

    [Serializable]
    public class BookmarkContent 
    {
        public string useYn;
        public int seq;
        public string title;
        public string content;
        public string sdtm;
        public string edtm;
        public SimpleMemberInfo createMember;
        public string createdDate;
        public SpaceSeq space;
    }
    
    [Serializable]
    public class BookmarkMeeting 
    {
        public string useYn;
        public int seq;
        public bool seminar;
        public SimpleMemberInfo createMember;
        public string nm;
        public string title;
        public string dt;
        public string stm;
        public string etm;
    }

    [Serializable]
    public class BookmarkMember 
    {
        public string useYn;
        public int seq;
        public string memberType;
        public string nickNm;
        public string photo;
        public string jobGrade;
        public string description;
    }

    [Serializable]
    public class Bookmark 
    {
        public eBookmarkType type;
        public int seq;
        public int targetSeq;
        public string title;
        public string desc;

        public Bookmark(eBookmarkType type, int seq, int targetSeq, string title, string desc) 
        {
            this.type = type;
            this.seq = seq;
            this.targetSeq = targetSeq;
            this.title = title;
            this.desc = desc;
        }
    }


    public class BookmarkData : InfiniteScrollData 
    {
        public Bookmark info;

        public BookmarkData(Bookmark info) 
        {
            this.info = info;
        }
    }
}