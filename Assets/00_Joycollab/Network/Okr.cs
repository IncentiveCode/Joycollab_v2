/// <summary>
/// Network 통신 - OKR 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 05
/// @version        : 0.2
/// @update
/// 	v0.1 (2023. 07. 03) : Joycollab 에서 사용하던 클래스 정리 및 통합.
///     v0.2 (2023. 07. 05) : CreatorSpaceInfo 형태의 space 와 topSpace 추가. ReqOkrInfo 클래스 수정.
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
    [Serializable] 
    public class ReqOkrList 
    {
        public bool share;
        public string startDate;
		public int viewOpt;		// 0 : daily, 1 : weekly, 2 : monthly
		public int filterOpt;	// 0 : 모든 공유 (부서 + 전사), 1 : 부서 공유, 2 : 전사 공유
        public string keyword;
        public int pageNo;
        public int pageSize;
		public bool sortDescending;
		public string sortProperty;

        public ReqOkrList() 
        {
            share = sortDescending = false;
            startDate = keyword = sortProperty = string.Empty;
            viewOpt = filterOpt = pageNo = pageSize = -1;
        }

        public string url 
        {
            get {
                int memberSeq = R.singleton.memberSeq;

                string url = share ?
                    string.Format(URL.GET_SHARE_OKR_LIST, memberSeq, filterOpt, viewOpt, startDate) : 
                    string.Format(URL.GET_OKR_LIST, memberSeq, viewOpt, startDate) ;
                url += "?";

				if (! string.IsNullOrEmpty(keyword)) url += "keyword="+ keyword +"&";
				url += "page="+ pageNo +"&size="+ pageSize;
				url += "&sortDirection=";
				url += sortDescending ? "descending" : "ascending";
				url += "&sortProperty=";
				url += sortProperty;

				return url;
            }
        }
    } 

    [Serializable]
    public class ReqOkrInfo 
    {
        public string content;
        public Seq createMember;
        public string ed;
        public Seq modifyMember;
        public string sd;
        public string title;
    }

    [Serializable]
    public class ResOkrList
    { 
        public bool hasNext;
		public List<ResOkrInfo> content;
	}

    [Serializable] 
    public class ResOkrInfo 
    {
        public string useYn;
        public int seq;
        public string title;
        public string content;
        public string sd;
        public string ed;
        public MemberSeq createMember;
        public MemberSeq modifyMember;
        public string createdDate;
        public string modifiedDate;
        public int shereType;
        public TopOkrInfo topOkr;
        public CreatorSpaceInfo space;
        public CreatorSpaceInfo topSpace;
        public List<SubOkrInfo> subOkr;
    }

    [Serializable]
    public class TopOkrList 
    {
        public List<TopOkrInfo> list;
    }

    [Serializable]
    public class TopOkrInfo
    { 
	    public string useYn;
        public int seq;
        public string title;
        public string content;
        public string sd;
        public string ed;
        public MemberSeq createMember;
        public MemberSeq modifyMember;
        public string createdDate;
        public string modifiedDate;
        public int shereType;
        public CreatorSpaceInfo space;
        public CreatorSpaceInfo topSpace;
	}

    [Serializable]
    public class SubOkrInfo
    { 
		public string useYn;
        public int seq;
        public string title;
        public string content;
        public string sd;
        public string ed;
        public MemberSeq createMember;
        public MemberSeq modifyMember;
        public string createdDate;
        public string modifiedDate;
        public int shereType;
        public TopOkrInfo topOkr;
        public CreatorSpaceInfo space;
        public CreatorSpaceInfo topSpace;
	}

    public class OkrData : InfiniteScrollData 
    {
        public ResOkrInfo info;
        public SubOkrInfo subInfo;

        public int seq;
        public bool isShare;
        public bool isKeyResult;
        public int shareType;
        public string objective;
        public bool loadMore;

        public OkrData(ResOkrInfo o, bool isKR=false) 
        {
            info = o;
            subInfo = null;

            seq = o.seq;
            isShare = true;
            isKeyResult = isKR;
            shareType = isKR ? o.topOkr.shereType : o.shereType;
            objective = isKR ? o.topOkr.title : o.title;
            loadMore = false;
        }

        public OkrData(SubOkrInfo kr, int share, string obj)
        {
            info = null;
            subInfo = kr;

            seq = kr.seq;
            isShare = false;
            isKeyResult = true;
            shareType = share;
            objective = obj;
            loadMore = false;
        }

        public OkrData() 
        {
            info = null;
            subInfo = null;

            seq = -1;
            isShare = false;
            isKeyResult = false;
            shareType = -1;
            loadMore = true;
        }
    }
}