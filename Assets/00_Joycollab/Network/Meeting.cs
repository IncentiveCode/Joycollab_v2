/// <summary>
/// Network 통신 - 미팅 관련 요청 및 응답
/// @author         : HJ Lee
/// @last update    : 2023. 10. 27
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 27) : v1 에서 사용하던 클래스 정리 및 통합
/// </summary>

using System;
using System.Collections.Generic;

namespace Joycollab.v2
{
    [Serializable]
    public class ReqMeetingInfo
    {
        public string dt;
        public bool email;
        public string etm;
        public bool isOpen;
        public bool isPrivate;
        public List<Seq> members;
        public List<string> participants;
        public string pin;
        public List<Seq> publishers;
        public bool run;
        public bool seminar;
        public bool sms;
        public string stm;
        public string title;
        public bool alarm;
        public bool sendAlarm;
        public string useYn;
        public bool zoom;
        public string description;
        public List<string> attachedFile;
        public List<string> enterParticipants;
        public List<string> participantsList;       
    }

    [Serializable]
    public class MeetingList
    {
        public List<MeetingInfo> list;
    }

    [Serializable]
    public class MeetingInfo 
    {
        public bool seminar;
        public int seq;
        public bool isPrivate;
        public bool isOpen;
        public SimpleMemberInfo createMember;
        public List<string> participants;
        public string pin;
        public List<SimpleMemberInfo> publishers;
        public string nm;
        public string title;
        public string dt;
        public string stm;
        public string etm;
        public List<SimpleMemberInfo> members;
        public List<SimpleMemberInfo> enterMembers;
        public bool sms;
        public bool email;
        public bool run;
        public bool zoom;
        public string zoomId;
        public string startUrl;
        public string joinUrl;
        public string description;
        public List<string> attachedFile;
        public List<string> enterParticipants;
        public List<string> participantsList;
    }

    [Serializable]
    public class MeetingHistory 
    {
        public int totalPages;
        public int totalElements;
        public List<MeetingHistoryContent> content;
    }

    [Serializable]
    public class MeetingHistoryContent 
    {
        public bool seminar;
        public int seq;
        public bool isPrivate;
        public bool isOpen;
        public string useYn;
        public string title;
        public SimpleMemberInfo createMember;
        public List<SimpleMemberInfo> members;
        public List<SimpleMemberInfo> enterMembers;
        public List<SimpleMemberInfo> participantMembers;
        public List<string> participants;
        public string pin;
        public List<Seq> publishers;
        public string stm;
        public string etm;
        public string dt;
        public string nm;
        public string startTime;
        public string lastPingTime;
        public bool run;
        public bool zoom;
        public string zoomId;
        public string startUrl;
        public string joinUrl;
        public string description;
        public List<string> attachedFile;
        public List<string> enterParticipants;
        public List<string> participantsList;
    }
}