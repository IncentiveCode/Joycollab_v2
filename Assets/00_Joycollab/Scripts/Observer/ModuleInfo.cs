/// <summary>
/// module 사용 변경 여부를 기록하기 위한 serializalbe class 모음.
/// @author         : HJ Lee
/// @last update    : 2023. 08. 01
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 14) : 최초 생성.
///     v0.2 (2023. 06. 02) : module interface 관련 정리. 
///     v0.3 (2023. 08. 01) : 통합 게시판 추가. '미팅' 만 있는 메뉴와 '미팅+세미나' 가 있는 메뉴 분리.
///                           구독 플랜에 따라 모듈 설정되는 기능 추가.
/// </summary>

using System;
using UnityEngine;

namespace Joycollab.v2
{
	[Serializable]
	public class FloatingMenu 
	{
		public ePlanType planType;
		public bool fileBox;
		public bool todo;
		public bool contact;
		public bool chat;
		public bool call;
		public bool meeting;
		public bool meetingAndSeminar;
		public bool calendar;
		public bool kanban;
		public bool builtInBoard;

		public FloatingMenu() 
		{
			// default settings
			planType = ePlanType.Free;
			fileBox = todo = contact = call = chat = true;
			meeting = meetingAndSeminar = calendar = kanban = builtInBoard = false;
		}

		public void UpdateState(FloatingMenu pref) 
		{
            // Debug.Log($"FloatingMenu | current plan : {pref.planType}");
			this.planType = pref.planType;

			switch (planType) 
			{
				case ePlanType.Free :
					fileBox = todo = contact = call = chat = true;
					meeting = meetingAndSeminar = calendar = builtInBoard = false;
					kanban = false;
					break;

				case ePlanType.Basic :
					fileBox = todo = contact = call = chat = true;
					meeting = true;
					meetingAndSeminar = calendar = builtInBoard = false;
					kanban = false;
					break;

				case ePlanType.Standard :
					fileBox = todo = contact = call = chat = true;
					meeting = false;
				 	meetingAndSeminar = calendar = builtInBoard = true;
					kanban = false;
					break;

				case ePlanType.Premium :
				case ePlanType.Trial :
					fileBox = todo = contact = call = chat = true;
					meeting = false;
					meetingAndSeminar = calendar = builtInBoard = true;
					kanban = true;
					break;

				case ePlanType.Custom :
					this.fileBox = pref.fileBox;
					this.todo = pref.todo;
					this.contact = pref.contact;
					this.chat = pref.chat;
					this.call = pref.call;
					this.meeting = pref.meeting;
					this.meetingAndSeminar = pref.meetingAndSeminar;
					this.calendar = pref.calendar;
					this.kanban = pref.kanban;
					this.builtInBoard = pref.builtInBoard;
					break;
			}
		}
	}

	// TODO. 우측메뉴 추가 예정.	
	[Serializable] 
	public class ConfigMenu 
	{

	}
}