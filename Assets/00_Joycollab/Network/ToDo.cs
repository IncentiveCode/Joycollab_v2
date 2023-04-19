/// <summary>
/// Network 통신 - Space 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 24
/// @version        : 1.0
/// @update
///     v1.0 (2023. 03. 24) : Joycollab 에서 사용하던 클래스 정리 및 통합 (진행 중)
/// </summary>

using System;

namespace Joycollab.v2
{
	[Serializable]
	public class ResToDoList 
	{
		public bool hasNext;
		public ResToDoInfo[] content;
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
		// public MemberSeq createMember;
		public string createdDate;
		public int repetition;
		public int shereType;
		public string completeYn;
		public string alarm;
		public string pushedYn;
		// public CreatorSpaceInfo space;
	}
}