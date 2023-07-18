/// <summary>
/// Network 통신 - Xmpp 관련
/// @author         : HJ Lee
/// @last update    : 2023. 07. 18
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 18) : Joycollab 에서 사용하던 것, 일부 수정해서 작성.
/// </summary>

using System;
using UnityEngine;

namespace Joycollab.v2
{
    [Serializable]
    public class XmppContent 
    {
        public string alarmType;
        public int seq;
        public int cd;
        public string date1;
        public string date2;
        public string content;
        public int unReadCnt;

        // 알림 형태로 받은 xmpp
        public string contentJson;
        public string title;
        public string tp;

        public XmppContent() 
        {
            alarmType = content = string.Empty;
            seq = cd = unReadCnt = 0;

            contentJson = title = tp = string.Empty;
        }
    }

    [SerializeField] 
    public class XmppTaskInfo
    {
        public int mseq;
        public int task;
        public int topTask;
        public int proj;
        public string photo;
        public string title;

        public XmppTaskInfo() 
        {
            mseq = task = topTask = proj = 0;
            photo = title = string.Empty;
        }
    }
}