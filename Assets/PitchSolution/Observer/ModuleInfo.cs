using System;

namespace PitchSolution
{
	[Serializable]
	public class FloatingMenu 
	{
		public bool fileBoxUse;
		public bool todoUse;
		public bool contactUse;
		public bool meetingUse;
		public bool seminarUse;
		public bool calendarUse;
		public bool kanbanUse;
		public bool chatUse;

		public FloatingMenu() 
		{
			fileBoxUse = todoUse = contactUse = meetingUse = seminarUse = calendarUse = kanbanUse = chatUse = false;
		}
	}

	// TODO. 좌측메뉴, 우측메뉴 추가 예정.	
}