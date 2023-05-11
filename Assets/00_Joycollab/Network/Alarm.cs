/// <summary>
/// Network 통신 - 알림 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 05. 10
/// @version        : 0.1
/// @update
/// 	v0.1 (2023. 05. 10) : Joycollab 에서 사용하던 클래스 정리 및 통합
/// </summary>

using System;

namespace Joycollab.v2
{
	[Serializable]
	public class ResAlarmCount 
	{
		public int count;
	}

	[Serializable]
	public class ResAlarmList 
	{
		public ResAlarmInfo[] list;
	}

	[Serializable] 
	public class ResAlarmInfo
	{
		public int seq;
		public TpsInfo tp;
		public string title;
		public string img;
		public string sender;
		public string content;
		public string contentJson;
		public string dtm;
		public bool read;
	}
}