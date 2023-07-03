/// <summary>
/// Network 통신 - OKR 관련 요청과 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 03
/// @version        : 0.1
/// @update
/// 	v0.1 (2023. 07. 03) : Joycollab 에서 사용하던 클래스 정리 및 통합.
/// </summary>

using System;
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
				url += "page="+ pageNo +"&size="+ pageSize +"&";
				url += "sortDirection=";
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
        public MemberSeq createMember;
        public string ed;
        public MemberSeq modifyMember;
        public string sd;
        public string title;
    }

    [Serializable] 
    public class ResOkrList 
    {

    }
}