/// <summary>
/// Network 통신 - To-Do 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 03
/// @version        : 0.5
/// @update
///     v0.1 (2023. 03. 24) : Joycollab 에서 사용하던 클래스 정리 및 통합.
/// 	v0.2 (2023. 06. 12) : ToDoModule 을 사용하기 위한 ReqToDoList 추가.
/// 	v0.3 (2023. 06. 14) : infinite scroll data 추가.
/// 	v0.4 (2023. 06. 14) : array 를 list 로 변경.
/// 	v0.5 (2023. 07. 03) : ReqToDoInfo 클래스 추가.
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

namespace Joycollab.v2
{
	[Serializable]
	public class ReqToDoList
	{
		public bool share;
		public string startDate;
		public int targetMemberSeq;
		public int viewOpt;		// 0 : daily, 1 : weekly, 2 : monthly
		public int filterOpt;	// 0 : 모든 공유, 1 : 부서 공유, 2 : 전사 공유
		public string keyword;
		public int pageNo;
		public int pageSize;
		public bool sortDescending;
		public string sortProperty;

		public ReqToDoList() 
		{
			share = sortDescending = false;
			startDate = keyword = sortProperty = string.Empty;
			targetMemberSeq = viewOpt = filterOpt = pageNo = pageSize = -1;
		}

		public string url
		{
			get {
            	int memberSeq = R.singleton.memberSeq;

				string url = share ?
					string.Format(URL.SHARE_TODO_LIST, memberSeq, viewOpt, filterOpt, startDate) :
					string.Format(URL.TARGET_TODO_LIST, memberSeq, targetMemberSeq, viewOpt, startDate) ;
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
	public class ReqToDoInfo
	{
		public string content;
		public string ed;
		public int repetition;
		public string sd;
		public int shereType;
		public string st;
		public string et;
		public string title;
	}

	[Serializable]
	public class ResToDoList 
	{
		public bool hasNext;
		public List<ResToDoInfo> content;
	}	

	[Serializable]
	public class ResToDoInfo 
	{
		public string useYn;
		public int seq;
		public string title;
		public string content;
		public string sd;
		public string ed;
		public string st;
		public string et;
		public string completeTime;
		public MemberSeq createMember;
		public string createdDate;
		public int repetition;
		public int shereType;
		public string completeYn;
		public string alarm;
		public string pushedYn;
		public CreatorSpaceInfo space;
	}


	public class ToDoData : InfiniteScrollData 
	{
		public ResToDoInfo info;
		public bool loadMore;

		public ToDoData() 
		{
			info = new ResToDoInfo();
			loadMore = false;
		}
	}
}