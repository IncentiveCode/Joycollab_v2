/// <summary>
/// Network 통신 - 알림 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 18
/// @version        : 0.4
/// @update
/// 	v0.1 (2023. 05. 10) : Joycollab 에서 사용하던 클래스 정리 및 통합
/// 	v0.2 (2023. 06. 14) : infinite scroll data 추가
/// 	v0.3 (2023. 06. 15) : array 를 list 로 변경
/// 	v0.4 (2023. 07. 18) : Instant alarm 을 위한 클래스 추가.
/// </summary>

using System;
using System.Collections.Generic;
using Gpm.Ui;

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
		public List<ResAlarmInfo> list;
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

	[Serializable] 
	public class InstantAlarmInfo 
	{
		public int seq;
		public string type;
		public string text;
		public bool isDone;

		public InstantAlarmInfo(int seq, string type, string text) 
		{
			this.seq = seq;
			this.type = type;
			this.text = text;
			this.isDone = false;
		}
	}


	/// <summary>
	/// NHN Gpm 의 infinite scroll view 를 위한 클래스.
	/// </summary>
	public class AlarmData : InfiniteScrollData 
	{
		public ResAlarmInfo info;

		public AlarmData() 
		{
			info = new ResAlarmInfo();
		}

		public AlarmData(ResAlarmInfo data) 
		{
			info = data;
		}
	}
}