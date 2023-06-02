using System;

namespace Joycollab.v2
{
	[Serializable]
	public class FloatingMenu 
	{
		public bool fileBox;
		public bool todo;
		public bool contact;
		public bool chat;
		public bool call;
		public bool meeting;
		public bool seminar;
		public bool calendar;
		public bool kanban;

		public FloatingMenu() 
		{
			// default settings
			fileBox = todo = contact = chat = meeting = true;
			call = seminar = calendar = kanban = false;
		}

		public void UpdateState(FloatingMenu pref) 
		{
			this.fileBox = pref.fileBox;
			this.todo = pref.todo;
			this.contact = pref.contact;
			this.chat = pref.chat;
			this.call = pref.call;
			this.meeting = pref.meeting;
			this.seminar = pref.seminar;
			this.calendar = pref.calendar;
			this.kanban = pref.kanban;
		}
	}

	// TODO. 좌측메뉴, 우측메뉴 추가 예정.	

	[Serializable] 
	public class ConfigMenu 
	{

	}
}